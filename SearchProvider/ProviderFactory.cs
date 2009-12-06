using FullSearch.Provider.MySql;

namespace FullSearch.Provider
{
    public class ProviderFactory
    {
        public static ISearchProvider GetMySqlProvider()
        {
            return new MySqlProvider(new MySqlDao());
        }
    }
}
