using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsyncApp
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await LoadData(Convert.ToInt32(lblCurPage.Text));
            DisableButton();
        }

        private async void btnInsert_Click(object sender, EventArgs e)
        {
            BULKCOPY bulk = new BULKCOPY();
            var result = await bulk.Insert(txtName.Text.Trim(), Convert.ToInt32(txtPrice.Text));
            MessageBox.Show(result.ToString() + " Inserted", "Info");
            await LoadData(Convert.ToInt32(lblCurPage.Text)).ConfigureAwait(false);
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            BULKCOPY bulk = new BULKCOPY();
            var result = await bulk.UpdateByName(txtName.Text.Trim(), Convert.ToInt32(txtPrice.Text));
            MessageBox.Show(result.ToString() + " Updateted", "Info");
            await LoadData(Convert.ToInt32(lblCurPage.Text)).ConfigureAwait(false);
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Are you sure?", "Confirm delete", MessageBoxButtons.YesNo);
            if (confirm.Equals(DialogResult.Yes))
            {
                BULKCOPY bulk = new BULKCOPY();
                var result = await bulk.DeleteByName(txtName.Text.Trim());
                MessageBox.Show(result.ToString() + " Deleted", "Info");
                await LoadData(Convert.ToInt32(lblCurPage.Text)).ConfigureAwait(false);
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            var name = txtName.Text.Trim();
            if (!string.IsNullOrWhiteSpace(name))
            {
                await LoadData(1, "AND NAME LIKE '" + name + "%'");
                DisableButton();
            }

        }

        private async void btnPrev_Click(object sender, EventArgs e)
        {
            var query = string.IsNullOrWhiteSpace(txtName.Text.Trim()) ? "" : "AND NAME='" + txtName.Text.Trim() + "'";
            var page = Convert.ToInt32(lblCurPage.Text) - 1;
            await LoadData(page, query);
            DisableButton();
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            var query = string.IsNullOrWhiteSpace(txtName.Text.Trim()) ? "" : "AND NAME='" + txtName.Text.Trim() + "'";
            var page = Convert.ToInt32(lblCurPage.Text) + 1;
            await LoadData(page, query);
            DisableButton();
        }

        private async void btnCount_Click(object sender, EventArgs e)
        {
            BULKCOPY bulk = new BULKCOPY();
            var count = await bulk.GetCount().ConfigureAwait(false);
            MessageBox.Show(count.ToString(), "Total record");
        }

        private async void grvData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = grvData.CurrentRow;
            if (!row.IsNewRow)
            {
                string name = row.Cells[0].Value.ToString().Trim();
                var bulk = new BULKCOPY();
                var detail = await bulk.SelectByName(name);
                if (detail.Any())
                {
                    txtName.Text = detail.First().NAME;
                    txtPrice.Text = detail.First().PRICE.ToString();
                }
            }
        }

        private async Task LoadData(int page = 1, string query = "")
        {
            var start = (page - 1) * 10 + 1;
            var end = start + 10 - 1;
            BULKCOPY bulk = new BULKCOPY();
            var count = bulk.GetCount(query);
            var lstBulk = bulk.SelectPaging(start, end, query);
            await Task.WhenAll(count, lstBulk);
            grvData.DataSource = lstBulk.Result;
            lblTotal.Text = count.Result.ToString();
            lblCurPage.Text = page.ToString();
            lblttPage.Text = (count.Result / 10) == 0 ? "1" : (count.Result / 10).ToString();
        }

        private async void btnReload_Click(object sender, EventArgs e)
        {
            await LoadData();
            DisableButton();
            txtName.Text = txtPrice.Text = "";
        }

        private void DisableButton()
        {
            var currentPage = Convert.ToInt32(lblCurPage.Text);
            var numPage = Convert.ToInt32(lblttPage.Text);
            btnPrev.Enabled = currentPage > 1;
            btnNext.Enabled = currentPage < numPage;
        }
    }
}
