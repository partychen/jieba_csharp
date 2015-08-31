using System;
using WordSegmenter.DagGenerator;
using WordSegmenter.Models;

namespace WordSegmenter.RouteGenerator
{
    public class BestRouteGenerator : IRouteGenerator
    {
        private readonly IDagGenerator _dagGenerator;
        private readonly WordDictionary _wordDictionary = WordDictionary.Instance;
        public BestRouteGenerator(IDagGenerator dagGenerator)
        {
            _dagGenerator = dagGenerator;
        }
        public Route GetRoutes(string sentence)
        {
            var dags = _dagGenerator.Generate(sentence);
            int len = sentence.Length;
            double logTotal = Math.Log(_wordDictionary.TotalFrequency);
            var segments = new Segment[len + 1];
            segments[len] = new Segment(0, 0);
            for (int i = len - 1; i >= 0; i--)
            {
                var route = new Segment(i, Helper.MinValue);
                foreach (var to in dags[i])
                {
                    double wordLog = Math.Log(_wordDictionary.GetEffectiveFrequency(sentence.Sub(i, to)));
                    if (wordLog - logTotal + segments[to + 1].Weight > route.Weight)
                    {
                        route.To = to;
                        route.Weight = wordLog - logTotal + segments[to + 1].Weight;
                    }
                }
                segments[i] = route;
            }
            return new Route(segments);
        }
    }
}