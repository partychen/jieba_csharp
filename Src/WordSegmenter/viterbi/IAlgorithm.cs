using System.Collections.Generic;

namespace WordSegmenter.viterbi
{
    public interface IAlgorithm
    {
        IList<string> Core(string sentence);
    }
}