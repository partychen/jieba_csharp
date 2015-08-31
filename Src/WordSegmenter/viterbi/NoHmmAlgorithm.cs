using System.Collections.Generic;
using System.Linq;

namespace WordSegmenter.viterbi
{
    public class NoHmmAlgorithm : IAlgorithm
    {
        public IList<string> Core(string sentence)
        {
            return sentence.Select(w => "" + w).ToList();
        }
    }
}
