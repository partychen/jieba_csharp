using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WordSegmenter.CutCommand;
using WordSegmenter.DagGenerator;
using WordSegmenter.RouteGenerator;
using WordSegmenter.viterbi;

namespace WordSegmenter
{
    public class ChineseSegmenter : ISegmenter
    {
        private readonly IRouteGenerator _routeGenerator;
        private readonly IDagGenerator _dagGenerator;

        public ChineseSegmenter()
        {
            _dagGenerator = new DagGenerator.DagGenerator();
            _routeGenerator = new BestRouteGenerator(_dagGenerator);
        }

        public ICutDagCommand GetCutDagCommand(CutCommandType cutCommandType)
        {
            if (cutCommandType == CutCommandType.All)
            {
                return new CutAllCommand(_dagGenerator, _routeGenerator);
            }
            if (cutCommandType == CutCommandType.Index)
            {
                return new CutWithHmmCommand(_dagGenerator, _routeGenerator, new ViterbiAlgorithm());
            }
            return new CutWithoutHmmCommand(_dagGenerator, _routeGenerator,new NoHmmAlgorithm());
        }

        public string Run(string sentence, CutCommandType cutCommandType = CutCommandType.Index)
        {
            var regex1 = new Regex(@"([\u4E00-\u9FA5a-zA-Z0-9+#&\._]+)", RegexOptions.None);
            var regex2 = new Regex(@"(\r\n|\s)", RegexOptions.None);
            var blocks = regex1.Split(sentence);
            var finalResult = new List<string>();
            foreach (var block in blocks)
            {
                if (string.IsNullOrEmpty(block)) continue;
                if (regex1.IsMatch(block))
                {
                    finalResult.AddRange(GetCutDagCommand(cutCommandType).Cut(block));
                }
                else
                {
                    finalResult.AddRange((from p in regex2.Split(block)
                                          where !string.IsNullOrEmpty(p)
                                          let charArray = p.ToCharArray()
                                          from c in charArray
                                          select "" + c).ToList());
                }
            }
            return string.Join("/", finalResult);
        }
    }
}