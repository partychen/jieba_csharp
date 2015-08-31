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
        public ICutDagCommand Command { get; set; }
        public int TotalFrequency { get; private set; }
        private WordDictionary()
        {
            LoadDictWords(_file);
            if (!string.IsNullOrEmpty(_userFile))
            {
                LoadUserDictWord(_userFile);
            }
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
                            total -= dict[lineDatas[0]];
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
            if (HasKey(word))
            {
                TotalFrequency = TotalFrequency - WordsFrequencies[word] + f;
                WordsFrequencies[word] = f;
            }
            else
            {
                TotalFrequency = TotalFrequency + f;
                WordsFrequencies.Add(word,f);
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

        private int SuggestFrequency(string word)
        {
            if (Command != null)
            {
                double freq = Command.Cut(word).Aggregate(1.0, (current, seg) => current * (GetEffectiveFrequency(seg) * 1.0 / TotalFrequency));
                return Math.Max((int)freq * TotalFrequency + 1, GetEffectiveFrequency(word));
            }
            return GetEffectiveFrequency(word);
        }
    }
}
