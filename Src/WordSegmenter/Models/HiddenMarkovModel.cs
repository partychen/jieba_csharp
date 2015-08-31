using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace WordSegmenter.Models
{
    public class HiddenMarkovModel
    {
        private readonly Dictionary<char, Dictionary<char, double>> _emitProb;
        private readonly string _file = ConfigurationManager.AppSettings["prob_emit_file_path"];
        public readonly char[] State = { 'B', 'E', 'M', 'S' };
        public readonly double[] InitProb = { -0.26268660809250016, -3.14e+100, -3.14e+100, -1.4652633398537678 };

        public readonly double[,] TransProb =
        {
            {-3.14e+100, -0.510825623765990, -0.916290731874155, -3.14e+100},
            {-0.5897149736854513, -3.14e+100, -3.14e+100, -0.8085250474669937},
            {-3.14e+100, -0.33344856811948514, -1.2603623820268226, -3.14e+100},
            {-0.7211965654669841, -3.14e+100, -3.14e+100, -0.6658631448798212}
        };

        private static HiddenMarkovModel _instance;
        private HiddenMarkovModel()
        {
            _emitProb = LoadEmitProb();
        }

        public static HiddenMarkovModel Instance
        {
            get { return _instance ?? (_instance = new HiddenMarkovModel()); }
        }

        private Dictionary<char, Dictionary<char, double>> LoadEmitProb()
        {
            using (var sr = new StreamReader(_file))
            {
                var content = sr.ReadToEnd();
                return JSON.Parse<Dictionary<char, Dictionary<char, double>>>(content);
            }
        }

        public char GetCurrentState(int current)
        {
            return State[current];
        }

        public double GetInitProb(int state,char c)
        {
            return InitProb[state] + GetProb(_emitProb[State[state]], c);
        }

        public double GetNextProb(int currentState,int toState,char c)
        {
            return TransProb[currentState, toState] + GetProb(_emitProb[State[toState]], c);
        }

        private double GetProb(IDictionary<char,double> dict, char key)
        {
            return dict.ContainsKey(key) ? dict[key] : Helper.MinValue;
        }
    }
}
