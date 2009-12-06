using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace FullSearch.Provider.MySql
{
    internal class MySqlDao
    {
        private readonly int MAX_RESULT_ROWS = 1000;
        private MySqlConnection conn;

        private MySqlConnection GetConnection()
        {
            if (conn == null)
            {
                conn = new MySqlConnection("server=localhost;uid=root;password=root");
            }
            return conn;
        }

        public DataSet GetTables()
        {
            DataSet dataSet = null;
            try
            {
                MySqlCommand command = GetConnection().CreateCommand();
                command.CommandText = "SELECT table_schema, table_name FROM information_schema.tables";
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                dataSet = new DataSet();
                adapter.Fill(dataSet);
            }
            finally
            {
                GetConnection().Close();
            }
            return dataSet;
        }

        public int CountPages(string schema, string table)
        {
            int result = 0;
            try
            {
                GetConnection().Open();
                MySqlCommand command = GetConnection().CreateCommand();
                command.CommandText = "SELECT count(1) FROM " + schema + "." + table;
                int count = int.Parse(command.ExecuteScalar().ToString());
                result = (count % MAX_RESULT_ROWS) == 0 ? (count / MAX_RESULT_ROWS) : (count / MAX_RESULT_ROWS + 1);
            }
            finally
            {
                GetConnection().Close();
            }
            return result;
        }

        public DataSet GetRows(string schema, string table, int page)
        {
            DataSet dataSet = null;
            try
            {
                string sql = "SELECT * FROM " + schema + "." + table;
                MySqlCommand command = GetConnection().CreateCommand();
                command.CommandText = sql.ToString();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                dataSet = new DataSet();
                adapter.Fill(dataSet, page * MAX_RESULT_ROWS, MAX_RESULT_ROWS, schema + "." + table);
            }
            finally
            {
                GetConnection().Close();
            }
            return dataSet;
        }

    }
}