using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using WordSegmenter.CutCommand;
using WordSegmenter.DagGenerator;
using WordSegmenter.Models;
using WordSegmenter.RouteGenerator;
using WordSegmenter.viterbi;

namespace WordSegmenter
{
    public class ChineseSegmenter : ISegmenter
    {
        private readonly WordDictionary _wordDictionary = WordDictionary.Instance;
        public IDagGenerator DagGenerator { get; set; }
        public IRouteGenerator RouteGenerator { get; set; }
        public IAlgorithm Algorithm { get; set; }

        public ICutDagCommand CutDagCommand { get; set; }

        public ICutDagCommand AddWordCutCommand { get; set; }

        public ChineseSegmenter()
        {
            DagGenerator = new DagGenerator.DagGenerator();
            RouteGenerator = new BestRouteGenerator(DagGenerator);
            Algorithm = new ViterbiAlgorithm();
            CutDagCommand = new CutWithHmmCommand(DagGenerator,RouteGenerator,Algorithm);
            AddWordCutCommand = new CutWithHmmCommand(DagGenerator, RouteGenerator, Algorithm);
        }

        public ChineseSegmenter(CutCommandType cutCommandType)
        {
            DagGenerator = new DagGenerator.DagGenerator();
            RouteGenerator = new BestRouteGenerator(DagGenerator);
            if (cutCommandType == CutCommandType.All)
            {
                CutDagCommand = new CutAllCommand(DagGenerator, RouteGenerator);
            }
            else if (cutCommandType == CutCommandType.Hmm)
            {
                Algorithm = new ViterbiAlgorithm();
                CutDagCommand = new CutWithHmmCommand(DagGenerator, RouteGenerator, Algorithm);
            }
            else
            {
                Algorithm = new NoHmmAlgorithm();
                CutDagCommand = new CutWithoutHmmCommand(DagGenerator, RouteGenerator, Algorithm);
            }
            AddWordCutCommand = new CutWithHmmCommand(DagGenerator, RouteGenerator, Algorithm);
        }

        public ChineseSegmenter(CutCommandType cutCommandType, AddWordCommandType addWordCommandType)
        {
            DagGenerator = new DagGenerator.DagGenerator();
            RouteGenerator = new BestRouteGenerator(DagGenerator);
            if (cutCommandType == CutCommandType.All)
            {
                CutDagCommand = new CutAllCommand(DagGenerator, RouteGenerator);
            }
            else if (cutCommandType == CutCommandType.Hmm)
            {
                Algorithm = new ViterbiAlgorithm();
                CutDagCommand = new CutWithHmmCommand(DagGenerator, RouteGenerator, Algorithm);
            }
            else
            {
                Algorithm = new NoHmmAlgorithm();
                CutDagCommand = new CutWithoutHmmCommand(DagGenerator, RouteGenerator, Algorithm);
            }

            if (addWordCommandType == AddWordCommandType.Hmm)
            {
                Algorithm = new ViterbiAlgorithm();
                CutDagCommand = new CutWithHmmCommand(DagGenerator, RouteGenerator, Algorithm);
            }
            else
            {
                Algorithm = new NoHmmAlgorithm();
                CutDagCommand = new CutWithoutHmmCommand(DagGenerator, RouteGenerator, Algorithm);
            }
        }

        public string Cut(string sentence)
        {
            var regex1 = new Regex(@"([\u4E00-\u9FA5a-zA-Z0-9+#&\._]+)", RegexOptions.None);
            var regex2 = new Regex(@"(\r\n|\s)", RegexOptions.None);
            var blocks = regex1.Split(sentence);
            var finalResult = new List<string>();
            foreach (var block in blocks)
            {
                if (string.IsNullOrEmpty(block)) continue;
                if (regex1.IsMatch(block))
                {
                    finalResult.AddRange(CutDagCommand.Cut(block));
                }
                else
                {
                    finalResult.AddRange((from p in regex2.Split(block)
                                          where !string.IsNullOrEmpty(p)
                                          let charArray = p.ToCharArray()
                                          from c in charArray
                                          select "" + c).ToList());
                }
            }
            return string.Join("/", finalResult);
        }

        public void LoadUserDictWord(string path)
        {
            using (var sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        var lineDatas = line.Trim().Split(' ');
                        int? frequency = null;
                        string tag = string.Empty;
                        if (lineDatas.Length > 2)
                        {
                            frequency = int.Parse(lineDatas[1]);
                            tag = lineDatas[2];
                        }
                        else if (lineDatas.Length == 2)
                        {
                            int f;
                            if (int.TryParse(lineDatas[1], out f))
                            {
                                frequency = f;
                            }
                            else
                            {
                                tag = lineDatas[1];
                            }
                        }
                        AddWord(lineDatas[0], frequency, tag);
                    }
                }
            }
        }

        public void AddWord(string word, int? frequency, string tag)
        {
            int f = frequency ?? SuggestFrequency(word);
            _wordDictionary.AddWord(word,f,tag);
        }

        private int SuggestFrequency(string word)
        {
            if (AddWordCutCommand != null)
            {
                double freq = AddWordCutCommand.Cut(word).Aggregate(1.0,
                    (current, seg) => current * (_wordDictionary.GetEffectiveFrequency(seg) * 1.0 / _wordDictionary.TotalFrequency));
                return Math.Max((int)freq * _wordDictionary.TotalFrequency + 1, _wordDictionary.GetEffectiveFrequency(word));
            }
            return _wordDictionary.GetEffectiveFrequency(word);
        }
    }
}