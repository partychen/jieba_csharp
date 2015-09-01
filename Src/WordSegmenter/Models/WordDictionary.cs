using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using WordSegmenter.CutCommand;

namespace WordSegmenter.Models
{
    public class WordDictionary
    {
        private readonly string _file = ConfigurationManager.AppSettings["dict_file_path"];
        private readonly string _userFile = ConfigurationManager.AppSettings["user_dict_file_path"];
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
            TotalFrequency = 0;
            WordsFrequencies = new Dictionary<string, int>();
            using (var sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        var lineDatas = line.Trim().Split(' ');
                        AddWord(lineDatas[0], int.Parse(lineDatas[1]),"");
                    }
                }
            }
        }

        public void AddWord(string word, int frequency, string tag)
        {
            if (HasKey(word))
            {
                TotalFrequency = TotalFrequency - WordsFrequencies[word] + frequency;
                WordsFrequencies[word] = frequency;
            }
            else
            {
                TotalFrequency = TotalFrequency + frequency;
                WordsFrequencies.Add(word, frequency);
            }
            var builder = new StringBuilder();
            foreach (var w in word)
            {
                builder = builder.Append(w);
                if (!HasKey(builder.ToString()))
                {
                    WordsFrequencies.Add(builder.ToString(), 0);
                }
            }
        }
        
    }
}
