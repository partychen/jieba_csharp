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
    }
}