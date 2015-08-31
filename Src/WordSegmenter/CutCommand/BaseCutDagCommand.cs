using System.Collections.Generic;
using System.Text;
using WordSegmenter.DagGenerator;
using WordSegmenter.Models;
using WordSegmenter.RouteGenerator;
using WordSegmenter.viterbi;

namespace WordSegmenter.CutCommand
{
    public abstract class BaseCutDagCommand : ICutDagCommand
    {
        protected IDagGenerator DagGenerator;
        protected IRouteGenerator RouteGenerator;
        protected IAlgorithm Algorithm;
        protected WordDictionary WordDictionary = WordDictionary.Instance;

        protected BaseCutDagCommand(IDagGenerator dagGenerator,IRouteGenerator routeGenerator, IAlgorithm algorithm)
        {
            DagGenerator = dagGenerator;
            RouteGenerator = routeGenerator;
            Algorithm = algorithm;
        }

        public virtual void ResolveGapWords(List<string> result, StringBuilder sb)
        {
            if (sb.Length > 0)
            {
                string word = sb.ToString();
                if (word.Length > 1 && !WordDictionary.HasEffectiveKey(word))
                {
                    result.AddRange(Algorithm.Core(word));
                }
                else
                {
                    result.Add(word);
                }
                sb.Clear();
            }
        }

        public virtual List<string> Cut(string sentence)
        {
            Route route = RouteGenerator.GetRoutes(sentence);
            var result = new List<string>();
            var sb = new StringBuilder();
            for (var start = 0; start < sentence.Length; start = route.Segments[start].To + 1)
            {
                string word = sentence.Sub(start, route.Segments[start].To);
                if (start == route.Segments[start].To)
                {
                    sb.Append(word);
                }
                else
                {
                    ResolveGapWords(result, sb);
                    result.Add(word);
                }
            }
            ResolveGapWords(result, sb);
            return result;
        }
    }
}