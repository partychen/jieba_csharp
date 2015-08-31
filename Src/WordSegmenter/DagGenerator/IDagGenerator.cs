using System.Collections.Generic;

namespace WordSegmenter.DagGenerator
{
    public interface IDagGenerator
    {
        Dictionary<int, List<int>> Generate(string sentence);
    }
}