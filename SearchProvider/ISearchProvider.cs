using System.Data;

namespace FullSearch.Provider
{

    public delegate void ExecutionFeedback(object source, string message);

    public interface ISearchProvider
    {
        event ExecutionFeedback Feedback;
        DataSet Search(string expr, MatchCondition condition, SearchExpansion expansion);
    }

}
