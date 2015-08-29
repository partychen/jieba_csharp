using System;
using System.Collections.Generic;
using System.IO;
using WordSegmenter.Models;

namespace WordSegmenter
{
    public interface ISegmenter
    {
        WordFrequency LoadDictWords(string path);
        Dictionary<int, List<int>> GenerateDAG(string sentence);
        Route[] GetBestRoute(string sentence, Dictionary<int, List<int>> dags);
        List<string> CutDAG(string sentence, Route[] routes, bool withHmm = true);
    }
}
