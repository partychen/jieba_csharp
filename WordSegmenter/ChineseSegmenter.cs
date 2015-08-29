using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WordSegmenter.Models;
using WordSegmenter.viterbi;


namespace WordSegmenter
{
    public class ChineseSegmenter : ISegmenter
    {
        private readonly string _file = ConfigurationManager.AppSettings["dict_file_path"];
        private WordFrequency _wordFrequency;
        public WordFrequency LoadDictWords(string path)
        {
            var dict = new Dictionary<string, int>();
            int total = 0;
            using (var sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        var lineDatas = line.Trim().Split(' ');
                        if (dict.ContainsKey(lineDatas[0]))
                        {
                            dict.Remove(lineDatas[0]);
                        }
                        dict.Add(lineDatas[0], int.Parse(lineDatas[1]));
                        total += int.Parse(lineDatas[1]);
                        var builder = new StringBuilder();
                        foreach (var w in lineDatas[0])
                        {
                            builder = builder.Append(w);
                            if (!dict.ContainsKey(builder.ToString()))
                            {
                                dict.Add(builder.ToString(), 0);
                            }
                        }
                    }
                }
            }
            _wordFrequency = new WordFrequency(dict, total);
            return _wordFrequency;
        }

        public Dictionary<int, List<int>> GenerateDAG(string sentence)
        {
            int len = sentence.Length;
            var dags = new Dictionary<int, List<int>>();
            for (int i = 0; i < len; i++)
            {
                var toList = new List<int>();
                var sb = new StringBuilder();
                int k = i;
                sb = sb.Append(sentence[i]);
                while (k < len && _wordFrequency.WordsFrequencies.ContainsKey(sb.ToString()))
                {
                    if (_wordFrequency.WordsFrequencies[sb.ToString()] != 0)
                    {
                        toList.Add(k);
                    }
                    k++;
                    if (k < len)
                    {
                        sb = sb.Append(sentence[k]);
                    }
                }
                if (toList.Count == 0)
                {
                    toList.Add(i);
                }
                dags.Add(i, toList);
            }
            return dags;
        }

        public Route[] GetBestRoute(string sentence, Dictionary<int, List<int>> dags)
        {
            int len = sentence.Length;
            double logTotal = Math.Log(_wordFrequency.TotalFrequency);
            var routes = new Route[len + 1];
            routes[len] = new Route(0, 0);
            for (int i = len - 1; i >= 0; i--)
            {
                foreach (var to in dags[i])
                {
                    double wordLog = Math.Log(_wordFrequency.WordsFrequencies.DictionaryValueGet(sentence.Sub(i, to)));
                    if (routes[i] == null || wordLog - logTotal + routes[to + 1].Weight > routes[i].Weight)
                    {
                        routes[i] = new Route(to, wordLog - logTotal + routes[to + 1].Weight);
                    }
                }
            }
            return routes;
        }

        private IEnumerable<string> ResolveGapWords(string word, bool withHmm = true)
        {
            if (word.Length > 1)
            {
				if (!_wordFrequency.WordsFrequencies.DictionaryHasValue(word))
                {
                    var result = new List<string>();
                    result.AddRange(withHmm ? ViterbiAlogorithm.Instance.Viterbi(word) : word.Select(w => "" + w));
                    return result;
                }
            }
            return new List<string>() { word };
        }

        public List<string> CutDAG(string sentence, Route[] routes, bool withHmm = true)
        {
            var result = new List<string>();
            int len = sentence.Length;
            int start = 0;
            var sb = new StringBuilder();
            while (start < len)
            {
                int end = routes[start].To + 1;
                string word = sentence.Sub(start, routes[start].To);
                if (start + 1 == end)
                {
                    sb.Append(word);
                }
                else
                {
                    if (sb.Length > 0)
                    {
                        result.AddRange(ResolveGapWords(sb.ToString(), withHmm));
                        sb.Clear();
                    }
                    result.Add(word);
                }
                start = end;
            }
            if (sb.Length > 0)
            {
                result.AddRange(ResolveGapWords(sb.ToString(), withHmm));
                sb.Clear();
            }
            return result;
        }

        private ChineseSegmenter()
        {
            LoadDictWords(_file);
        }
        private static ChineseSegmenter _instance;
        public static ChineseSegmenter Instance
        {
            get { return _instance ?? (_instance = new ChineseSegmenter()); }
        }

        public string Run(string sentence, bool withHmm = true)
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
                    var dags = GenerateDAG(block);
                    var routes = GetBestRoute(block, dags);
                    finalResult.AddRange(CutDAG(block, routes, withHmm));
                }
                else
                {
                    finalResult.AddRange((from p in regex2.Split(block)
                                          where !string.IsNullOrEmpty(p)
                                          let charArray = p.ToCharArray()
                                          from c in charArray
                                          select "" +c).ToList());
                }
            }
            return string.Join("/", finalResult);
        }
    }
}