using System;
using System.Data;
using FullSearch.Provider;

namespace FullSearch.Client
{

    class Program
    {

        static void Main(string[] args)
        {
            if (args.Length == 1 && (args[0] == "help" || args[0] == "?"))
            {
                Console.WriteLine("Database Full Search :: Console Client");
                Console.WriteLine("Parameters:");
                Console.WriteLine("1. expr - Expression to search");
                Console.WriteLine("2. provider - Search provider [mysql/sqlserver]");
                Console.WriteLine("3. matchCondition - Match condition [e = exact/ c = contains/ ca = contains all]");
                Console.WriteLine("4. expansion - Search expansion [none/simple]");
                Console.WriteLine("5. [verbose]");
                Console.WriteLine("Example: ConsoleClient blahh mysql c none verbose");
            }
            else
            {
                if (args.Length < 4)
                {
                    throw new ArgumentException("Parameters: expr provider matchCodition expansion [verbose]");
                }

                SearchService srv = new SearchService();

                if (args[1] == "mysql")
                {
                    srv.Provider = SearchProvider.MYSQL;
                }
                else if (args[1] == "sqlserver")
                {
                    srv.Provider = SearchProvider.SQLSERVER;
                }
                else
                {
                    throw new ArgumentException("invalid parameter " + args[1] + " for provider");
                }

                if (args[2] == "e")
                {
                    srv.Condition = MatchCondition.EXACT;
                }
                else if (args[2] == "c")
                {
                    srv.Condition = MatchCondition.CONTAINS;
                }
                else if (args[2] == "ca")
                {
                    srv.Condition = MatchCondition.CONTAINS_ALL;
                }
                else
                {
                    throw new ArgumentException("invalid parameter " + args[2] + " for matchCondition");
                }

                if (args[3] == "none")
                {
                    srv.Expansion = SearchExpansion.NONE;
                }
                else if (args[3] == "simple")
                {
                    srv.Expansion = SearchExpansion.SIMPLE;
                }
                else
                {
                    throw new ArgumentException("invalid parameter " + args[3] + " for expansion");
                }

                if (args.Length > 4 && args[4] == "verbose")
                {
                    if (args[4] == "verbose")
                    {
                        srv.Verbose = true;
                    }
                    else
                    {
                        throw new ArgumentException("invalid parameter " + args[4] + " for verbose");
                    }
                }

                DataSet ds = srv.Search(args[0]);

                Console.WriteLine(ds.GetXml());
            }
        }

    }
}
