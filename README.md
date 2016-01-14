###test

# jieba_csharp
"结巴"中文分词的C#版本
首先感谢jieba分词原作者fxsjy，没有他的无私贡献，我们也不会结识到结巴分词，更不会有现在的C#版本。

#简介
结巴分词(C#版)只保留的原项目中All和Hmm，no Hmm 三种模式。

#支持分词模式

* All模式，用于对用户查询词分词,显示出所有可能性
* Hmm模式，用于对用户查询词分词，采用 Viterbi (维特比)算法实现未登录词识别
* No Hmm模式，用于对用户查询词分词，不能识别的词独立成词

#特性
* 支持多种分词模式
* 全角统一转成半角
* 用户词典功能

#如何使用
```C#
public void testDemo() {
    var segmenter = new ChineseSegmenter();
    var sentences ={"这是一个伸手不见五指的黑夜。我叫孙悟空，我爱北京，我爱Python和C++。", "我不喜欢日本和服。", "雷猴回归人间。", "工信处女干事每月经过下属科室都要亲口交代24口交换机等技术性器件的安装工作", "结果婚的和尚未结过婚的"};
    foreach (String sentence in sentences) {
        Console.WriteLine(segmenter.Cut(input[i]));
    }
}

public void testDemo() {
    var segmenter = new ChineseSegmenter(CutCommandType.Hmm, AddWordCommandType.Hmm);
    segmenter.LoadUserDictWord(@"..\..\test_files\user_dict.txt");
    segmenter.AddWord("这是",null,null);
    var sentences ={"这是一个伸手不见五指的黑夜。我叫孙悟空，我爱北京，我爱Python和C++。"};
    foreach (String sentence in sentences) {
        Console.WriteLine(segmenter.Cut(input[i]));
    }
}

```
#算法(wiki补充…)
* 1.基于 trie 树结构实现高效词图扫描
* 2.生成所有切词可能的有向无环图 DAG
* 3.采用动态规划算法计算最佳切词组合
* 4.基于 HMM 模型，采用 Viterbi (维特比)算法实现未登录词识别
