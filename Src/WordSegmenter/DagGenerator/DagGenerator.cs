using System.Collections.Generic;
using System.Text;
using WordSegmenter.Models;

namespace WordSegmenter.DagGenerator
{
    public class DagGenerator : IDagGenerator
    {
        private readonly WordDictionary _wordDictionary = WordDictionary.Instance;
        public Dictionary<int, List<int>> Generate(string sentence)
        {
            var len = sentence.Length;
            var dags = new Dictionary<int, List<int>>();
            for (var i = 0; i < len; i++)
            {
                var toList = new List<int>() {i};
                var sb = new StringBuilder("" + sentence[i]);
                for (var k = i + 1; k < len; k++)
                {
                    sb = sb.Append(sentence[k]);
                    if (!_wordDictionary.HasKey(sb.ToString())) break;
                    if (_wordDictionary.HasEffectiveKey(sb.ToString()))
                    {
                        toList.Add(k);
                    }
                }
                dags.Add(i, toList);
            }
            return dags;
        }
    }
}