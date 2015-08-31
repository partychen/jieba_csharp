using WordSegmenter.CutCommand;

namespace WordSegmenter
{
    public interface ISegmenter
    {
        string Cut(string sentence);
    }
}
