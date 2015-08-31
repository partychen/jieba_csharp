using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Practices.Unity;
using WordSegmenter.CutCommand;
using WordSegmenter.DagGenerator;
using WordSegmenter.RouteGenerator;
using WordSegmenter.viterbi;

namespace WordSegmenter
{
    public class ChineseSegmenter : ISegmenter
    {
        private readonly ICutDagCommand _cutDagCommand;
        public ChineseSegmenter(ICutDagCommand cutDagCommand)
        {
            _cutDagCommand = cutDagCommand;
        }

        public string Cut(string sentence)
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
                    finalResult.AddRange(_cutDagCommand.Cut(block));
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

    public class SegmenterFactory
    {
        public IUnityContainer Container = new UnityContainer();

        public SegmenterFactory()
        {
            Container.RegisterType<IDagGenerator, DagGenerator.DagGenerator>()
                .RegisterType<IRouteGenerator, BestRouteGenerator>();
        }

        public ICutDagCommand GetCutDagCommand(CutCommandType cutCommandType)
        {
            Container.RegisterType<IAlgorithm, ViterbiAlgorithm>()
                .RegisterType<ICutDagCommand, CutWithHmmCommand>();
            return Container.Resolve<ICutDagCommand>();
        }
        public string Cut(string sentence, CutCommandType cutCommandType)
        {
            if (cutCommandType == CutCommandType.All)
            {
                Container.RegisterType<ICutDagCommand, CutAllCommand>();
            }
            else if (cutCommandType == CutCommandType.Hmm)
            {
                Container.RegisterType<IAlgorithm, ViterbiAlgorithm>()
                    .RegisterType<ICutDagCommand, CutWithHmmCommand>();
            }
            else
            {
                Container.RegisterType<IAlgorithm, NoHmmAlgorithm>()
                    .RegisterType<ICutDagCommand, CutWithoutHmmCommand>();
            }
            Container.RegisterType<ISegmenter, ChineseSegmenter>();
            return Container.Resolve<ISegmenter>().Cut(sentence);
        }
    }
}