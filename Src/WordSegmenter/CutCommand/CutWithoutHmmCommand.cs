using WordSegmenter.DagGenerator;
using WordSegmenter.RouteGenerator;
using WordSegmenter.viterbi;

namespace WordSegmenter.CutCommand
{
    public class CutWithoutHmmCommand : BaseCutDagCommand
    {
        public CutWithoutHmmCommand(IDagGenerator dagGenerator, IRouteGenerator routeGenerator, IAlgorithm algorithm) :
            base(dagGenerator, routeGenerator, algorithm ?? new NoHmmAlgorithm())
        {
        }
    }
}