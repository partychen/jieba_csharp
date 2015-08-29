using System;
using System.Collections.Generic;

namespace WordSegmenter
{
    public static class Helper
    {
        public static string Sub(this string sentence, int start, int end)
        {
            return sentence.Substring(start, end - start + 1);
        }

        public static int DictionaryValueGet(this IDictionary<string, int> dict, string key)
        {
			if (dict.ContainsKey (key)) {
				if (dict [key] == 0)
					return 1;
				return dict [key];
			}
            return 1;
        }

        public static double DictionaryValueGet(this IDictionary<char, double> dict, char key)
        {
            if (dict.ContainsKey(key)) return dict[key];
            return -3.14e+100;
        }

		public static bool DictionaryHasValue(this IDictionary<string, int> dict, string key)
		{
			if (dict.ContainsKey (key) && dict [key] > 0)
				return true;
			return false;
		}
    }
}