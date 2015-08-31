namespace WordSegmenter
{
    public interface ISegmenter
    {
        string Run(string sentence, ChineseSegmenter.CutCommandType cutCommandType = ChineseSegmenter.CutCommandType.Index);
    }
}
