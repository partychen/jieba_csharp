using System.Collections.Generic;
using WordSegmenter.DagGenerator;
using WordSegmenter.RouteGenerator;

namespace WordSegmenter.CutCommand
{
    public class CutAllCommand : BaseCutDagCommand
    {
        public CutAllCommand(IDagGenerator dagGenerator, IRouteGenerator routeGenerator) : 
            base(dagGenerator, routeGenerator, null)
        {
        }

        public override List<string> Cut(string sentence)
        {
            var dags = DagGenerator.Generate(sentence);
            var result = new List<string>();
            int prev = -1;
            foreach (var dag in dags)
            {
                if (dag.Value.Count == 1 && dag.Key > prev)
                {
                    prev = dag.Value[0];
                    result.Add(sentence.Sub(dag.Key, prev));
                }
                else
                {
                    foreach (var to in dag.Value)
                    {
                        if (to > dag.Key)
                        {
                            result.Add(sentence.Sub(dag.Key, to));
                            prev = to;
                        }
                    }
                }
            }
            return result;
        }
    }
}