using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WordSegmenter.viterbi;

namespace WordSegmenter.Test
{
    [TestClass]
    public class ChineseSegmenterTest
    {
        private ISegmenter _segmenter;
        [TestInitialize]
        public void Initialize()
        {
            _segmenter = ChineseSegmenter.Instance;
        }

        [TestMethod]
        public void TestLoadDictWords()
        {
            var result = _segmenter.LoadDictWords(@"..\..\test_files\dict1.txt");
            Assert.IsNotNull(result);
            Assert.AreEqual(result.TotalFrequency, 6);
            Assert.IsNotNull(result.WordsFrequencies);
            Assert.AreEqual(result.WordsFrequencies.Count, 5);
            Assert.AreEqual(result.WordsFrequencies["龟年鹤寿"], 3);
            Assert.AreEqual(result.WordsFrequencies["龟年鹤算"], 3);
            Assert.AreEqual(result.WordsFrequencies["龟"], 0);
            Assert.AreEqual(result.WordsFrequencies["龟年"], 0);
            Assert.AreEqual(result.WordsFrequencies["龟年鹤"], 0);
            Assert.IsFalse(result.WordsFrequencies.ContainsKey("XXXX"));
        }

        [TestMethod]
        public void TestGenerateDAG()
        {
            _segmenter.LoadDictWords(@"..\..\test_files\dict2.txt");
            var dags = _segmenter.GenerateDAG("他来到了浙江省网易杭研大厦");
            Assert.AreEqual(JSON.stringify(dags[0]), JSON.stringify(new List<int>() { 0 }));
            Assert.AreEqual(JSON.stringify(dags[1]), JSON.stringify(new List<int>() { 2 }));
            Assert.AreEqual(JSON.stringify(dags[2]), JSON.stringify(new List<int>() { 2 }));
            Assert.AreEqual(JSON.stringify(dags[3]), JSON.stringify(new List<int>() { 3 }));
            Assert.AreEqual(JSON.stringify(dags[4]), JSON.stringify(new List<int>() { 5, 6 }));
            Assert.AreEqual(JSON.stringify(dags[5]), JSON.stringify(new List<int>() { 5 }));
            Assert.AreEqual(JSON.stringify(dags[6]), JSON.stringify(new List<int>() { 6 }));
            Assert.AreEqual(JSON.stringify(dags[7]), JSON.stringify(new List<int>() { 8 }));
            Assert.AreEqual(JSON.stringify(dags[8]), JSON.stringify(new List<int>() { 8 }));
            Assert.AreEqual(JSON.stringify(dags[9]), JSON.stringify(new List<int>() { 9 }));
            Assert.AreEqual(JSON.stringify(dags[10]), JSON.stringify(new List<int>() { 10 }));
            Assert.AreEqual(JSON.stringify(dags[11]), JSON.stringify(new List<int>() { 12 }));
            Assert.AreEqual(JSON.stringify(dags[12]), JSON.stringify(new List<int>() { 12 }));
        }

        [TestMethod]
        public void TestGetBestRoute()
        {
            _segmenter.LoadDictWords(@"..\..\test_files\dict2.txt");
            const string sentence = @"他来到了浙江省网易杭研大厦";
            var dags = _segmenter.GenerateDAG(sentence);
            var routes = _segmenter.GetBestRoute(sentence, dags);
            Assert.IsNotNull(routes);
            Assert.AreEqual(14, routes.Length);
            Assert.AreEqual(0, routes[0].To);
            Assert.AreEqual(2, routes[1].To);
            Assert.AreEqual(2, routes[2].To);
            Assert.AreEqual(3, routes[3].To);
            Assert.AreEqual(6, routes[4].To);
            Assert.AreEqual(5, routes[5].To);
            Assert.AreEqual(6, routes[6].To);
            Assert.AreEqual(8, routes[7].To);
            Assert.AreEqual(8, routes[8].To);
            Assert.AreEqual(9, routes[9].To);
            Assert.AreEqual(10, routes[10].To);
            Assert.AreEqual(12, routes[11].To);
            Assert.AreEqual(12, routes[12].To);
        }

        [TestMethod]
        public void TestCutDAG_No_HMM()
        {
            _segmenter.LoadDictWords(@"..\..\test_files\dict2.txt");
            const string sentence = @"他来到了浙江省网易杭研大厦";
            var dags = _segmenter.GenerateDAG(sentence);
            var routes = _segmenter.GetBestRoute(sentence, dags);

            var result = _segmenter.CutDAG(sentence, routes, false);
            var segment = string.Join("/", result);
            Assert.AreEqual("他/来到/了/浙江省/网易/杭/研/大厦", segment);
        }

        [TestMethod]
        public void TestViterbi()
        {
            var result = ViterbiAlogorithm.Instance.Viterbi(@"杭研杭研杭研");
            var segment = string.Join("/", result);
            Assert.AreEqual("杭研/杭研/杭研", segment);
            result = ViterbiAlogorithm.Instance.Viterbi(@"杭杭研杭研");
            segment = string.Join("/", result);
            Assert.AreEqual("杭杭研/杭研", segment);
            result = ViterbiAlogorithm.Instance.Viterbi(@"研杭杭研杭研");
            segment = string.Join("/", result);
            Assert.AreEqual("研杭/杭研/杭研", segment);
        }
        [TestMethod]
        public void TestCutDAG_With_HMM()
        {
            _segmenter.LoadDictWords(@"..\..\test_files\dict2.txt");
            const string sentence = @"他来到了浙江省网易杭研大厦";
            var dags = _segmenter.GenerateDAG(sentence);
            var routes = _segmenter.GetBestRoute(sentence, dags);
            var result = _segmenter.CutDAG(sentence, routes);
            var segment = string.Join("/", result);
            Assert.AreEqual("他/来到/了/浙江省/网易/杭研/大厦", segment);
        }


        [TestMethod]
        public void Test()
        {
            var regex = new Regex(@"(\d+\.\d+|[a-zA-Z0-9]+)", RegexOptions.None);
            var collections = regex.Split(@"工信处女干事每月经过下属科室都要亲口交代24口交换机等技术性器件的安装工作。");
            foreach (var collection in collections)
            {
                collection.ToString();
            }

//            var dict = new Dictionary<char, Dictionary<char, double>>();
//
//            var fre = new Dictionary<char, double>();
//            using (var sr = new StreamReader(@"..\..\test_files\emit_prob.txt"))
//            {
//                while (!sr.EndOfStream)
//                {
//                    string line = sr.ReadLine();
//                    if (!string.IsNullOrEmpty(line))
//                    {
//                        if ("BMES".Contains(line))
//                        {
//                            dict.Add(char.Parse(line), fre);
//                            fre = new Dictionary<char, double>();
//                            continue;
//                        }
//                        var linedata = line.Split('\t');
//                        fre.Add(char.Parse(linedata[0]), double.Parse(linedata[1]));
//                    }
//                }
//            }
//            var content = JSON.stringify(dict);
//            using (var sw = new StreamWriter(@"..\..\test_files\emit_prob.json"))
//            {
//                sw.WriteLine(content);
//            }
//            var result = JSON.parse<Dictionary<char, Dictionary<char, double>>>(content);
        }
    }
}
