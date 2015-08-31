using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WordSegmenter.Models;

namespace WordSegmenter.viterbi
{
    public class ViterbiAlgorithm : IAlgorithm
    {
        public readonly HiddenMarkovModel HiddenMarkovModel = HiddenMarkovModel.Instance;

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
                        weight[i, j] = HiddenMarkovModel.GetInitProb(j, sentence[i]);
                    }
                    continue;
                }
                for (int j = 0; j < 4; j++)
                {
                    weight[i, j] = Helper.MinValue;
                    path[i, j] = -1;
                    for (int k = 0; k < 4; k++)
                    {
                        double tmp = weight[i - 1, k] + HiddenMarkovModel.GetNextProb(k, j, sentence[i]);
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

            if ("BS".Contains(HiddenMarkovModel.GetCurrentState(current)))
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

        public IList<string> Core(string sentence)
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
