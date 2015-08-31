using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WordSegmenter.DagGenerator;
using WordSegmenter.Models;
using WordSegmenter.RouteGenerator;
using WordSegmenter.viterbi;

namespace WordSegmenter.Test
{
    [TestClass]
    public class ChineseSegmenterTest
    {
        private ISegmenter _segmenter;
        private IDagGenerator _dagGenerator;
        private IRouteGenerator _routeGeneratorgenerator;
        private IAlgorithm _hhmAlgorithm;
        [TestInitialize]
        public void Initialize()
        {
            _dagGenerator = new DagGenerator.DagGenerator();
            _routeGeneratorgenerator = new BestRouteGenerator(_dagGenerator);
            _hhmAlgorithm = new ViterbiAlgorithm();
            _segmenter = new ChineseSegmenter();
        }
        [TestMethod]
        public void TestLoadDictWords()
        {
            WordDictionary.Instance.LoadDictWords(@"..\..\test_files\dict1.txt");
            var result = WordDictionary.Instance;
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
            WordDictionary.Instance.LoadDictWords(@"..\..\test_files\dict2.txt");
            var dags = _dagGenerator.Generate("他来到了浙江省网易杭研大厦");
            Assert.AreEqual(JSON.stringify(dags[0]), JSON.stringify(new List<int>() { 0 }));
            Assert.AreEqual(JSON.stringify(dags[1]), JSON.stringify(new List<int>() { 1,2 }));
            Assert.AreEqual(JSON.stringify(dags[2]), JSON.stringify(new List<int>() { 2 }));
            Assert.AreEqual(JSON.stringify(dags[3]), JSON.stringify(new List<int>() { 3 }));
            Assert.AreEqual(JSON.stringify(dags[4]), JSON.stringify(new List<int>() { 4,5, 6 }));
            Assert.AreEqual(JSON.stringify(dags[5]), JSON.stringify(new List<int>() { 5 }));
            Assert.AreEqual(JSON.stringify(dags[6]), JSON.stringify(new List<int>() { 6 }));
            Assert.AreEqual(JSON.stringify(dags[7]), JSON.stringify(new List<int>() { 7,8 }));
            Assert.AreEqual(JSON.stringify(dags[8]), JSON.stringify(new List<int>() { 8 }));
            Assert.AreEqual(JSON.stringify(dags[9]), JSON.stringify(new List<int>() { 9 }));
            Assert.AreEqual(JSON.stringify(dags[10]), JSON.stringify(new List<int>() { 10 }));
            Assert.AreEqual(JSON.stringify(dags[11]), JSON.stringify(new List<int>() { 11,12 }));
            Assert.AreEqual(JSON.stringify(dags[12]), JSON.stringify(new List<int>() { 12 }));
        }
        
        [TestMethod]
        public void TestGetBestRoute()
        {
            WordDictionary.Instance.LoadDictWords(@"..\..\test_files\dict2.txt");
            const string sentence = @"他来到了浙江省网易杭研大厦";
            var result = _routeGeneratorgenerator.GetRoutes(sentence);
            Assert.IsNotNull(result);
            var routes = result.Segments;
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
        public void TestViterbi()
        {
            var result = _hhmAlgorithm.Core(@"杭研杭研杭研");
            var segment = string.Join("/", result);
            Assert.AreEqual("杭研/杭研/杭研", segment);
            result = _hhmAlgorithm.Core(@"杭杭研杭研");
            segment = string.Join("/", result);
            Assert.AreEqual("杭杭研/杭研", segment);
            result = _hhmAlgorithm.Core(@"研杭杭研杭研");
            segment = string.Join("/", result);
            Assert.AreEqual("研杭/杭研/杭研", segment);
        }

        [TestMethod]
        public void TestCutDAG_No_HMM()
        {
            WordDictionary.Instance.LoadDictWords(@"..\..\test_files\dict2.txt");
            const string sentence = @"他来到了浙江省网易杭研大厦";

            var result = _segmenter.Run(sentence, CutCommandType.Search);
            var segment = string.Join("/", result);
            Assert.AreEqual("他/来到/了/浙江省/网易/杭/研/大厦", segment);
        }

        [TestMethod]
        public void TestCutDAG_With_HMM()
        {
            WordDictionary.Instance.LoadDictWords(@"..\..\test_files\dict2.txt");
            const string sentence = @"他来到了浙江省网易杭研大厦";
            var result = _segmenter.Run(sentence, CutCommandType.Index);
            var segment = string.Join("/", result);
            Assert.AreEqual("他/来到/了/浙江省/网易/杭研/大厦", segment);
        }

        [TestMethod]
        public void TestCutDAG_All()
        {
            WordDictionary.Instance.LoadDictWords(@"..\..\test_files\dict2.txt");
            const string sentence = @"他来到了浙江省网易杭研大厦";
            var result = _segmenter.Run(sentence, CutCommandType.All);
            var segment = string.Join("/", result);
            Assert.AreEqual("他/来到/了/浙江/浙江省/网易/杭/研/大厦", segment);
        }

        [TestMethod]
        public void TestAddWord()
        {
            WordDictionary.Instance.LoadDictWords(@"..\..\test_files\dict2.txt");
            WordDictionary.Instance.Command = _segmenter.GetCutDagCommand(CutCommandType.Search);
            const string sentence = @"他来到了浙江省网易大厦";
            var result = _segmenter.Run(sentence);
            var segment = string.Join("/", result);
            Assert.AreEqual("他/来到/了/浙江省/网易/大厦", segment);

            WordDictionary.Instance.LoadUserDictWord(@"..\..\test_files\user_dict.txt");
            result = _segmenter.Run(sentence);
            segment = string.Join("/", result);
            Assert.AreEqual("他/来到/了/浙江省/网易大厦", segment);
        }

        [TestMethod]
        public void TestAddWord2()
        {
            WordDictionary.Instance.LoadDictWords(@"..\..\test_files\dict2.txt");
            WordDictionary.Instance.Command = _segmenter.GetCutDagCommand(CutCommandType.Search);
            const string sentence = @"他来到了浙江省网易大厦";
            var result = _segmenter.Run(sentence);
            var segment = string.Join("/", result);
            Assert.AreEqual("他/来到/了/浙江省/网易/大厦", segment);

            WordDictionary.Instance.LoadUserDictWord(@"..\..\test_files\user_dict2.txt");
            result = _segmenter.Run(sentence);
            segment = string.Join("/", result);
            Assert.AreEqual("他/来到/了/浙江省/网易大厦", segment);
        }
    }
}
