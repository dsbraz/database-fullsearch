using System;
using System.Data;
using System.Text;

namespace FullSearch.Provider.MySql
{
    internal class MySqlProvider : ISearchProvider
    {
        private MySqlDao dao;
        private ExecutionFeedback feedback;

        public MySqlProvider(MySqlDao dao) : base()
        {
            this.dao = dao;
        }

        public event ExecutionFeedback Feedback
        {
            add
            {
                feedback += value;
            }
            remove
            {
                feedback -= value;
            }
        }

        private void FireFeedbackEvent(string schema, string table, string column, int row, string text)
        {
            if (feedback != null)
            {
                feedback(this, "schema:" + schema + ";table:" + table + ";column:" + column + ";row:" + row + ";cexpr:" + text);
            }
        }

        public DataSet Search(string expr, MatchCondition condition, SearchExpansion expansion)
        {
            DataSet result = GetResultDataSet();

            DataSet dataSet = dao.GetTables();
            DataTableReader tablesReader = null;
            try
            {
                tablesReader = dataSet.CreateDataReader();
                while (tablesReader.Read())
                {
                    string schema = tablesReader.GetString(0);
                    string table = tablesReader.GetString(1);

                    int totalPages = dao.CountPages(schema, table);

                    for (int page = 0; page < totalPages; page++)
                    {
                        dataSet = dao.GetRows(schema, table, page);

                        DataTableReader rowsReader = null;
                        try
                        {
                            rowsReader = dataSet.CreateDataReader();
                            for (int rowCount = 0; rowsReader.Read(); rowCount++)
                            {
                                for (int columnCount = 0; columnCount < rowsReader.FieldCount; columnCount++)
                                {
                                    if (!rowsReader.IsDBNull(columnCount))
                                    {
                                        string column = rowsReader.GetName(columnCount);
                                        string cexpr = rowsReader.GetValue(columnCount).ToString();
                                        switch (expansion)
                                        {
                                            case SearchExpansion.NONE:
                                                bool isValid = Filter(cexpr, expr, condition);
                                                if (isValid)
                                                {
                                                    AddResultDataSetRow(result, schema, table, column, rowCount, cexpr);
                                                }
                                                break;
                                            case SearchExpansion.SIMPLE:
                                                throw new NotImplementedException();
                                        }
                                        FireFeedbackEvent(schema, table, column, rowCount, cexpr);
                                    }
                                }
                            }
                        }
                        finally
                        {
                            if (rowsReader != null)
                            {
                                rowsReader.Close();
                            }
                        }
                    }
                }
            }
            finally
            {
                if (tablesReader != null)
                {
                    tablesReader.Close();
                }
            }

            return result;
        }

        private bool Filter(string expr, string filter, MatchCondition condition)
        {
            bool result = false;
            string[] tokens = filter.Split();
            switch (condition)
            {
                case MatchCondition.CONTAINS:
                    foreach (string token in tokens)
                    {
                        result = expr.ToLower().IndexOf(token.ToLower()) >= 0;
                        if (result)
                        {
                            break;
                        }
                    }
                    break;
                case MatchCondition.CONTAINS_ALL:
                    foreach (string token in tokens)
                    {
                        result = expr.ToLower().IndexOf(token.ToLower()) >= 0;
                        if (!result)
                        {
                            break;
                        }
                    }
                    break;
                case MatchCondition.EXACT:
                    result = expr.ToLower().Equals(filter.ToLower());
                    break;
            }
            return result;
        }

        private DataSet GetResultDataSet()
        {
            DataSet result = new DataSet("DatabaseFullSearch");
            result.Tables.Add("SearchResult");
            result.Tables[0].Columns.Add("Schema");
            result.Tables[0].Columns.Add("Table");
            result.Tables[0].Columns.Add("Column");
            result.Tables[0].Columns.Add("Row");
            result.Tables[0].Columns.Add("Text");
            return result;
        }

        private void AddResultDataSetRow(DataSet dataSet, string schema, string table, string column, int row, string text)
        {
            string[] rows = new string[5];
            rows[0] = schema;
            rows[1] = table;
            rows[2] = column;
            rows[3] = row.ToString();
            rows[4] = text;
            dataSet.Tables[0].Rows.Add(rows);
        }

    }
}
