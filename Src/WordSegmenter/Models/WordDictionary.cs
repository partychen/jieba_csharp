using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace WordSegmenter.Models
{
    public class WordDictionary
    {
        private readonly string _file = ConfigurationManager.AppSettings["dict_file_path"];
        public IDictionary<string, int> WordsFrequencies { get; private set; }
        public int TotalFrequency { get; private set; }
        private WordDictionary()
        {
            LoadDictWords(_file);
        }
        private static WordDictionary _instance;
        public static WordDictionary Instance
        {
            get { return _instance ?? (_instance = new WordDictionary()); }
        }

        public int GetEffectiveFrequency(string key)
        {
            return HasEffectiveKey(key) ? WordsFrequencies[key] : 1;
        }

        public bool HasEffectiveKey(string key)
        {
            return HasKey(key) && WordsFrequencies[key] > 0;
        }
        public bool HasKey(string key)
        {
            return WordsFrequencies.ContainsKey(key);
        }

        public void LoadDictWords(string path)
        {
            var dict = new Dictionary<string, int>();
            int total = 0;
            using (var sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        var lineDatas = line.Trim().Split(' ');
                        if (dict.ContainsKey(lineDatas[0]))
                        {
                            dict.Remove(lineDatas[0]);
                        }
                        dict.Add(lineDatas[0], int.Parse(lineDatas[1]));
                        total += int.Parse(lineDatas[1]);
                        var builder = new StringBuilder();
                        foreach (var w in lineDatas[0])
                        {
                            builder = builder.Append(w);
                            if (!dict.ContainsKey(builder.ToString()))
                            {
                                dict.Add(builder.ToString(), 0);
                            }
                        }
                    }
                }
            }
            WordsFrequencies = dict;
            TotalFrequency = total;
        }
    }
}
