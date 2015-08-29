using System.Collections.Generic;

namespace WordSegmenter.Models
{
    public class WordFrequency
    {
        public IDictionary<string, int> WordsFrequencies { get; set; }
        public int TotalFrequency { get; set; }

        public WordFrequency(IDictionary<string, int> wordsFrequencies, int totalFrequency)
        {
            WordsFrequencies = wordsFrequencies;
            TotalFrequency = totalFrequency;
        }
    }
}
