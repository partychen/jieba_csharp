using WordSegmenter.CutCommand;

namespace WordSegmenter
{
    public interface ISegmenter
    {
        string Cut(string sentence);
        void LoadUserDictWord(string path);
        void AddWord(string word, int? frequency, string tag);
    }
}
