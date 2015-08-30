# jieba_csharp
"结巴"中文分词的C#版本

首先感谢jieba分词原作者fxsjy，没有他的无私贡献，我们也不会结识到结巴分词，更不会有现在的C#版本。

结巴分词(C#版)只保留的原项目针对搜索引擎分词的功能(cut_for_index、cut_for_search)，词性标注，关键词提取没有实现(今后如用到，可以考虑实现)。

#简介
支持分词模式

Search模式，用于对用户查询词分词
Index模式，用于对索引文档分词
#特性

支持多种分词模式
全角统一转成半角
用户词典功能
conf 目录有整理的搜狗细胞词库
因为性能原因，最新的快照版本去除词性标注，也希望有更好的 Pull Request 可以提供该功能。

如何使用
@Test
public void testDemo() {
    var sentences ={"这是一个伸手不见五指的黑夜。我叫孙悟空，我爱北京，我爱Python和C++。", "我不喜欢日本和服。", "雷猴回归人间。", "工信处女干事每月经过下属科室都要亲口交代24口交换机等技术性器件的安装工作", "结果婚的和尚未结过婚的"};
    foreach (String sentence in sentences) {
        Console.WriteLine(ChineseSegmenter.Instance.Run(input[i]));
    }
}
#算法(wiki补充…)
#[ ] 基于 trie 树结构实现高效词图扫描
#[ ] 生成所有切词可能的有向无环图 DAG
#[ ] 采用动态规划算法计算最佳切词组合
#[ ] 基于 HMM 模型，采用 Viterbi (维特比)算法实现未登录词识别
