using System;
using System.Collections.Generic;

namespace WordSegmenter
{
    public static class Helper
    {
        public static double MinValue = -3.14e+100;
        public static string Sub(this string sentence, int start, int end)
        {
            return sentence.Substring(start, end - start + 1);
        }

        public static int GetEffectiveFrequency(this IDictionary<string, int> dict, string key)
        {
            return dict.ContainsKey(key) && dict[key] > 0 ? dict[key] : 1;
        }

        public static bool HasEffectiveValue(this IDictionary<string, int> dict, string key)
        {
            return dict.ContainsKey (key) && dict [key] > 0;
        }

        public static double GetProb(this IDictionary<char, double> dict, char key)
        {
            return dict.ContainsKey(key) ? dict[key] : MinValue;
        }
    }
}