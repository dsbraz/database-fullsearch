using System;
using System.Data;
using FullSearch.Provider;

namespace FullSearch.Client
{
    class SearchService
    {

        private SearchProvider provider;
        private MatchCondition condition;
        private SearchExpansion expansion;
        private bool verbose;

        public SearchProvider Provider
        {
            get
            {
                return provider;
            }
            set
            {
                provider = value;
            }
        }

        public MatchCondition Condition
        {
            get
            {
                return condition;
            }
            set
            {
                condition = value;
            }
        }

        public SearchExpansion Expansion
        {
            get
            {
                return expansion;
            }
            set
            {
                expansion = value;
            }
        }

        public bool Verbose
        {
            get
            {
                return verbose;
            }
            set
            {
                verbose = value;
            }
        }

        public DataSet Search(string expr)
        {
            DataSet dataSet = null;
            switch (provider)
            {
                case SearchProvider.MYSQL:
                    ISearchProvider prv = ProviderFactory.GetMySqlProvider();
                    prv.Feedback += WriteToConsole;
                    dataSet = prv.Search(expr, condition, expansion);
                    break;
                case SearchProvider.SQLSERVER:
                    throw new NotImplementedException();
            }
            return dataSet;
        }

        private void WriteToConsole(object source, string message)
        {
            if (verbose)
            {
                Console.WriteLine(message);
            }
        }

    }
}
