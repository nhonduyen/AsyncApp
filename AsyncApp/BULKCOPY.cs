using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsyncApp
{
    public class BULKCOPY
    {
        public string NAME { get; set; }
        public int PRICE { get; set; }


        public BULKCOPY(string NAME, int PRICE)
        {
            this.NAME = NAME;
            this.PRICE = PRICE;
        }
        public BULKCOPY() { }


        public virtual async Task<IEnumerable<BULKCOPY>> Select(int ID = 0, string listcolumn = "")
        {
            var sql = "SELECT * FROM BULKCOPY ";
            if (!string.IsNullOrEmpty(listcolumn)) sql = sql.Replace("*", listcolumn);
            if (ID == 0)
            {
                var row = await DBManager<BULKCOPY>.ExecuteReader(sql);
                return row;
            }
            sql += " WHERE ID=@ID";
            var result = await DBManager<BULKCOPY>.ExecuteReader(sql, new { ID = ID });
            return result;
        }

        public virtual async Task<IEnumerable<BULKCOPY>> SelectByName(string Name, string listcolumn = "")
        {
            var sql = "SELECT TOP 1 * FROM BULKCOPY WHERE NAME LIKE @NAME+'%';";
            if (!string.IsNullOrEmpty(listcolumn)) sql = sql.Replace("*", listcolumn);

            var result = await DBManager<BULKCOPY>.ExecuteReader(sql, new { NAME = Name });
            return result;
        }

        public virtual async Task<IEnumerable<BULKCOPY>> SelectPaging(int start = 0, int end = 10, string query = "", string listcolumn = "")
        {
            var sql = "SELECT * FROM(SELECT ROW_NUMBER() OVER (order by name) AS ROWNUM, * FROM BULKCOPY WHERE 1=1 " + query + ") as u  WHERE   RowNum BETWEEN @start AND @end ORDER BY RowNum;";
            if (!string.IsNullOrEmpty(listcolumn)) sql = sql.Replace("*", listcolumn);

            var result = await DBManager<BULKCOPY>.ExecuteReader(sql, new { start = start, end = end });
            return result;
        }

        public virtual async Task<int> GetCount(string query = "")
        {
            var sql = "SELECT COUNT(1) AS CNT FROM BULKCOPY WHERE 1=1 " + query;
            var result = await DBManager<BULKCOPY>.ExecuteScalar(sql);
            return Convert.ToInt32(result);
        }
        public virtual async Task<int> Update1Column(int ID, string COLUMN, string VALUE)
        {
            var sql = string.Format(@"UPDATE BULKCOPY SET {0}=@VALUE WHERE ID=@ID", COLUMN);

            var result = await DBManager<BULKCOPY>.Execute(sql, new { ID = ID, VALUE = VALUE });
            return result;
        }
        public virtual async Task<int> Delete(int ID = 0)
        {
            var sql = "DELETE FROM BULKCOPY ";
            if (ID == 0)
            {
                var row = await DBManager<BULKCOPY>.Execute(sql);
                return row;
            }
            sql += " WHERE ID=@ID ";
            var result = await DBManager<BULKCOPY>.Execute(sql, new { ID = ID });
            return result;
        }

        public virtual async Task<int> DeleteByName(string NAME)
        {
            var sql = "DELETE FROM BULKCOPY WHERE NAME=@NAME";

            var result = await DBManager<BULKCOPY>.Execute(sql, new { NAME = NAME });
            return result;
        }

        public async Task DeleteAll()
        {
            await DBManager<BULKCOPY>.Execute("TRUNCATE TABLE BULKCOPY");
        }
        public async Task<int> SpecialCount()
        {
            var sql = string.Format(@"
SELECT SUM (row_count)
FROM sys.dm_db_partition_stats
WHERE object_id=OBJECT_ID('BULKCOPY')   
AND (index_id=0 or index_id=1);
");
            var result = await DBManager<BULKCOPY>.ExecuteScalar(sql);
            return Convert.ToInt32(result);
        }

        public virtual async Task<int> Insert(string NAME, int PRICE)
        {
            var sql = "INSERT INTO BULKCOPY(NAME,PRICE) VALUES(@NAME,@PRICE)";
            var result = await DBManager<BULKCOPY>.Execute(sql, new { NAME = NAME, PRICE = PRICE });
            return result;
        }

        public virtual async Task<int> UpdateByName(string NAME, int PRICE)
        {
            var sql = "UPDATE BULKCOPY SET PRICE=@PRICE WHERE NAME=@NAME";
            var result = await DBManager<BULKCOPY>.Execute(sql, new { NAME = NAME, PRICE = PRICE });
            return result;
        }

        public virtual async Task<int> Update(string NAME, int PRICE)
        {
            var sql = "UPDATE BULKCOPY SET NAME=@NAME,PRICE=@PRICE WHERE ID=@ID";

            var result = await DBManager<BULKCOPY>.Execute(sql, new { NAME = NAME, PRICE = PRICE });
            return result;
        }

    }
}
