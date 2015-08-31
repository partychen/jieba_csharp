﻿using System;
namespace WordSegmenter.Run
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = new String[]
            {
                "我不喜欢日本和服。",
                "雷猴回归人间。",
                "工信处女干事每月经过下属科室都要亲口交代24口交换机等技术性器件的安装工作。",
                "我需要廉租房。",
                "永和服装饰品有限公司。",
                "我爱北京天安门。",
                "abc",
                "隐马尔可夫",
                "雷猴是个好网站",
                "“Microsoft”一词由“MICROcomputer（微型计算机）”和“SOFTware（软件）”两部分组成",
                "草泥马和欺实马是今年的流行词汇",
                "伊藤洋华堂总府店",
                "中国科学院计算技术研究所",
                "罗密欧与朱丽叶",
                "我购买了道具和服装",
                "PS: 我觉得开源有一个好处，就是能够敦促自己不断改进，避免敞帚自珍",
                "湖北省石首市",
                "湖北省十堰市",
                "总经理完成了这件事情",
                "这是一个伸手不见五指的黑夜。我叫孙悟空，我爱北京，我爱Python和C++。",
                "电脑修好了",
                "做好了这件事情就一了百了了",
                "人们审美的观点是不同的。",
                "我们买了一个美的空调",
                "线程初始化时我们要注意",
                "一个分子是由好多原子组织成的",
                "祝你马到功成",
                "他掉进了无底洞里",
                "中国的首都是北京",
                "孙君意",
                "外交部发言人马朝旭",
                "领导人会议和第四届东亚峰会",
                "在过去的这五年",
                "还需要很长的路要走",
                "60周年首都阅兵",
                "你好人们审美的观点是不同的",
                "买水果然后来世博园",
                "买水果然后去世博园",
                "张晓梅去人民医院做了个B超然后去买了件T恤",
                "AT&T是一件不错的公司，给你发offer了吗？",
                "C++和c#是什么关系？11+122=133，是吗？"
            };
            var expected = new[]
            {
                "我/不/喜欢/日本/和服/。",
                "雷猴/回归/人间/。",
                "工信处/女干事/每月/经过/下属/科室/都/要/亲口/交代/24/口/交换机/等/技术性/器件/的/安装/工作/。",
                "我/需要/廉租房/。",
                "永和/服装/饰品/有限公司/。",
                "我/爱/北京/天安门/。",
                "abc",
                "隐/马尔可夫",
                "雷猴/是/个/好/网站",
                "“/Microsoft/”/一词/由/“/MICROcomputer/（/微型/计算机/）/”/和/“/SOFTware/（/软件/）/”/两/部分/组成",
                "草泥马/和/欺实/马/是/今年/的/流行/词汇",
                "伊藤/洋华堂/总府/店",
                "中国科学院计算技术研究所",
                "罗密欧/与/朱丽叶",
                "我/购买/了/道具/和/服装",
                "PS/:/ /我/觉得/开源/有/一个/好处/，/就是/能够/敦促/自己/不断改进/，/避免/敞帚/自珍",
                "湖北省/石首市",
                "湖北省/十堰市",
                "总经理/完成/了/这件/事情",
                "这是/一个/伸手不见五指/的/黑夜/。/我/叫/孙悟空/，/我/爱/北京/，/我/爱/Python/和/C++/。",
                "电脑/修好/了",
                "做好/了/这件/事情/就/一了百了/了",
                "人们/审美/的/观点/是/不同/的/。",
                "我们/买/了/一个/美的/空调",
                "线程/初始化/时/我们/要/注意",
                "一个/分子/是/由/好多/原子/组织/成/的",
                "祝/你/马到功成",
                "他/掉/进/了/无底洞/里",
                "中国/的/首都/是/北京",
                "孙君意",
                "外交部/发言人/马朝旭",
                "领导人/会议/和/第四届/东亚/峰会",
                "在/过去/的/这/五年",
                "还/需要/很长/的/路/要/走",
                "60/周年/首都/阅兵",
                "你好/人们/审美/的/观点/是/不同/的",
                "买/水果/然后/来/世博园",
                "买/水果/然后/去/世博园",
                "张晓梅/去/人民/医院/做/了/个/B超/然后/去/买/了/件/T恤",
                "AT&T/是/一件/不错/的/公司/，/给/你/发/offer/了/吗/？",
                "C++/和/c#/是/什么/关系/？/11/+/122/=/133/，/是/吗/？",
            };
            var segmenter = new ChineseSegmenter();
            for (var i = 0; i < input.Length; i ++)
            {
                string result = segmenter.Run(input[i]);
                if (result != expected[i])
                {
                    Console.WriteLine(expected[i]);
                    Console.WriteLine(result);
                    Console.WriteLine("-----------------------------------------------------");
                }
            }
        }
    }
}