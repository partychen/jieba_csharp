using WordSegmenter.CutCommand;

namespace WordSegmenter
{
    public interface ISegmenter
    {
        string Run(string sentence, CutCommandType cutCommandType = CutCommandType.Index);
        ICutDagCommand GetCutDagCommand(CutCommandType cutCommandType);
    }
}
