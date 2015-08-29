using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WordSegmenter.viterbi
{
    public class ViterbiAlogorithm
    {
        private readonly char[] _state = new[] { 'B', 'E', 'M', 'S' };
        private readonly double[] _initProb = new[] { -0.26268660809250016, -3.14e+100, -3.14e+100, -1.4652633398537678 };
        private readonly double[,] _transProb =
        {
            {-3.14e+100, -0.510825623765990, -0.916290731874155, -3.14e+100},
            {-0.5897149736854513, -3.14e+100, -3.14e+100, -0.8085250474669937},
            {-3.14e+100, -0.33344856811948514, -1.2603623820268226, -3.14e+100},
            {-0.7211965654669841, -3.14e+100, -3.14e+100, -0.6658631448798212}
        };

        private readonly string _file = ConfigurationManager.AppSettings["prob_emit_file_path"];
        private Dictionary<char, Dictionary<char, double>> _emitProb = new Dictionary<char, Dictionary<char, double>>();

        private static ViterbiAlogorithm _instance;
        private ViterbiAlogorithm()
        {
            LoadEmitProb();
        }
        public static ViterbiAlogorithm Instance
        {
            get { return _instance ?? (_instance = new ViterbiAlogorithm()); }
        }

        private void LoadEmitProb()
        {
            using (var sr = new StreamReader(_file))
            {
                var content = sr.ReadToEnd();
                _emitProb = JSON.parse<Dictionary<char, Dictionary<char, double>>>(content);
            }
        }
        private Tuple<int[,], int> ViterbiCore(string sentence)
        {
            int len = sentence.Length;
            var weight = new double[len, 4];
            var path = new int[len, 4];

            for (int i = 0; i < len; i++)
            {
                if (i == 0)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        weight[i, j] = _initProb[j] + _emitProb[_state[j]].DictionaryValueGet(sentence[i]);
                    }
                    continue;
                }
                for (int j = 0; j < 4; j++)
                {
                    weight[i, j] = -3.14e+100;
                    path[i, j] = -1;
                    for (int k = 0; k < 4; k++)
                    {
                        double tmp = weight[i - 1, k] + _transProb[k, j] + _emitProb[_state[j]].DictionaryValueGet(sentence[i]);
                        if (tmp > weight[i, j])
                        {
                            weight[i, j] = tmp;
                            path[i, j] = k;
                        }
                    }
                }
            }

            return new Tuple<int[,], int>(path, weight[len - 1, 1] >= weight[len - 1, 3] ? 1 : 3);
        }

        private void GetResult(string sentence, int[,] path, int current, int index, List<string> result, string word)
        {
            if (index == 0)
            {
                result.Add(sentence[index] + word);
                return;
            }

            if ("BS".Contains("" + _state[current]))
            {
                result.Add(sentence[index] + word);
                word = string.Empty;
            }
            else
            {
                word = sentence[index] + word;
            }
            GetResult(sentence, path, path[index, current], index - 1, result, word);

        }
        public List<string> Viterbi(string sentence)
        {
            var regex1 = new Regex(@"([\u4E00-\u9FA5]+)", RegexOptions.None);
            var regex2 = new Regex(@"(\d+\.\d+|[a-zA-Z0-9]+)", RegexOptions.None);

            var blocks = regex1.Split(sentence);
            var finalResult = new List<string>();
            
            foreach (var block in blocks)
            {
                if (string.IsNullOrEmpty(block)) continue;
                if (regex1.IsMatch(block))
                {
                    var path = ViterbiCore(block);
                    var result = new List<string>();
                    GetResult(block, path.Item1, path.Item2, block.Length - 1, result, string.Empty);
                    result.Reverse();
                    finalResult.AddRange(result);
                }
                else
                {
                    finalResult.AddRange(regex2.Split(block).Where(split => !string.IsNullOrEmpty(split)));
                }
            }
            return finalResult;
        }
    }
}
