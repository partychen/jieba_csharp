using System.Collections.Generic;

namespace WordSegmenter.CutCommand
{
    public interface ICutDagCommand
    {
        List<string> Cut(string sentence);
    }
}