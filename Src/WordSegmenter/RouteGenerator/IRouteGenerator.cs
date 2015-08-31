using WordSegmenter.Models;

namespace WordSegmenter.RouteGenerator
{
    public interface IRouteGenerator
    {
        Route GetRoutes(string sentence);
    }
}