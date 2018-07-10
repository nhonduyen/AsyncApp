using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsyncApp
{
    public class PRODUCT
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public int PRICE { get; set; }
        public string CATEGORY { get; set; }


        public PRODUCT(int ID, string NAME, int PRICE, string CATEGORY)
        {
            this.ID = ID;
            this.NAME = NAME;
            this.PRICE = PRICE;
            this.CATEGORY = CATEGORY;
        }
        public PRODUCT() { }


        public virtual async Task<IEnumerable<PRODUCT>> Select(int ID = 0, string listcolumn = "")
        {
            var sql = "SELECT * FROM PRODUCT ";
            if (!string.IsNullOrEmpty(listcolumn)) sql = sql.Replace("*", listcolumn);
            if (ID == 0)
            {
                var row = await DBManager<PRODUCT>.ExecuteReader(sql);
                return row;
            }
            sql += " WHERE ID=@ID";
            var result = await DBManager<PRODUCT>.ExecuteReader(sql, new { ID = ID });
            return result;
        }

        public virtual async Task<IEnumerable<PRODUCT>> SelectPaging(int start = 0, int end = 10, string query = "", string listcolumn = "")
        {
            var sql = "SELECT * FROM(SELECT ROW_NUMBER() OVER (order by id) AS ROWNUM, * FROM PRODUCT WHERE 1=1 " + query + ") as u  WHERE   RowNum BETWEEN @start AND @end ORDER BY RowNum;";
            if (!string.IsNullOrEmpty(listcolumn)) sql = sql.Replace("*", listcolumn);

            var result = await DBManager<PRODUCT>.ExecuteReader(sql, new { start = start, end = end });
            return result;
        }

        public virtual async Task<int> GetCount(string query = "")
        {
            var sql = "SELECT COUNT(1) AS CNT FROM PRODUCT WHERE 1=1 " + query;
            var result = await DBManager<PRODUCT>.ExecuteScalar(sql);
            return result;
        }
        public virtual async Task<int> Update1Column(int ID, string COLUMN, string VALUE)
        {
            var sql = string.Format(@"UPDATE PRODUCT SET {0}=@VALUE WHERE ID=@ID", COLUMN);

            var result = await DBManager<PRODUCT>.Execute(sql, new { ID = ID, VALUE = VALUE });
            return result;
        }
        public virtual async Task<int> Delete(int ID = 0)
        {
            var sql = "DELETE FROM PRODUCT ";
            if (ID == 0)
            {
                var row = await DBManager<PRODUCT>.Execute(sql);
                return row;
            }
            sql += " WHERE ID=@ID ";
            var result = await DBManager<PRODUCT>.Execute(sql, new { ID = ID });
            return result;
        }

        public async void DeleteAll()
        {
            await DBManager<PRODUCT>.Execute("TRUNCATE TABLE PRODUCT");
        }
        public async Task<int> SpecialCount()
        {
            var sql = string.Format(@"
SELECT SUM (row_count)
FROM sys.dm_db_partition_stats
WHERE object_id=OBJECT_ID('PRODUCT')   
AND (index_id=0 or index_id=1);
");
            var result = await DBManager<PRODUCT>.ExecuteScalar(sql);
            return Convert.ToInt32(result);
        }

        public virtual async Task<int> Insert(string NAME, int PRICE)
        {
            var sql = "INSERT INTO PRODUCT(NAME,PRICE) VALUES(@NAME,@PRICE)";
            var result = await DBManager<PRODUCT>.Execute(sql, new { NAME = NAME, PRICE = PRICE });
            return result;
        }

        public virtual async Task<int> Update(string NAME, int PRICE)
        {
            var sql = "UPDATE PRODUCT SET NAME=@NAME,PRICE=@PRICE WHERE ID=@ID";

            var result = await DBManager<PRODUCT>.Execute(sql, new { NAME = NAME, PRICE = PRICE });
            return result;
        }

    }
}
