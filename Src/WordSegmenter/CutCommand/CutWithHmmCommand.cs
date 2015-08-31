using WordSegmenter.DagGenerator;
using WordSegmenter.RouteGenerator;
using WordSegmenter.viterbi;

namespace WordSegmenter.CutCommand
{
    public class CutWithHmmCommand : BaseCutDagCommand
    {
        public CutWithHmmCommand(IDagGenerator dagGenerator, IRouteGenerator routeGenerator, IAlgorithm algorithm) :
            base(dagGenerator, routeGenerator, algorithm ?? new ViterbiAlgorithm())
        {
        }
    }
}