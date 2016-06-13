using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiebaNet.Segmenter;
using System.IO;

namespace SplitWord
{
    class SplitWord
    {
        //private HashSet<string> perceptionWords = new HashSet<string>();
        private static HashSet<string>[] WordsHash = new HashSet<string>[6];

        public static void Main()
        {
            //var seg = new JiebaNet.Segmenter.PosSeg.PosSegmenter();//new JiebaSegmenter();
            // var ans = seg.Cut("不要为幸福冒险，不要追求成功的假象。你需要的你已经拥有，只需要发现和觉察。而当你不去追求幸福，幸福似乎就自己掉入你的心灵；当你不再追求成功，成功却悄悄敲响你的大门。");
            //foreach (var a in ans)
            //  Console.WriteLine(a);     
            //NoneAds();
            // string perW = @"C:\Users\gsdgs_000\Desktop\情感\words";
            string[] wordDict =
                { @"C:\Users\gsdgs_000\Desktop\情感\words\主张词语（中文）.txt",
                @"C:\Users\gsdgs_000\Desktop\情感\words\正面情感词语（中文）.txt",
                 @"C:\Users\gsdgs_000\Desktop\情感\words\正面评价词语（中文）.txt",
                 @"C:\Users\gsdgs_000\Desktop\情感\words\负面情感词语（中文）.txt",
                 @"C:\Users\gsdgs_000\Desktop\情感\words\负面评价词语（中文）.txt",
                 @"C:\Users\gsdgs_000\Desktop\情感\words\程度级别词语（中文）.txt"
            };
            for (int i = 0; i < 6; i++)
            {
                WordsHash[i] = new HashSet<string>();
                initHash(wordDict[i], WordsHash[i]);
            }
            //var text = Console.ReadLine();
            //GetFeature(text);
            using (var enti = new starEntities())
            {
                var dataCount = enti.QQData.Count();
                var seg = new JiebaSegmenter();
                using (var stream = new FileStream(@"e:\qqdata.csv", FileMode.Create))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        using (var dateStream = new FileStream(@"e:\date.txt", FileMode.Create))
                        {
                            using (var datewriter = new StreamWriter(dateStream))
                            {
                                for (int i = 0; i < dataCount / 200; i++)
                                {
                                    var datas = enti.QQData.OrderBy(a => a.dataIndex).Skip(i * 200).Take(200).ToList();
                                    foreach (var data in datas)
                                    {
                                        var feature = new float[6];

                                        var ans = seg.Cut(data.text);

                                        Parallel.ForEach(ans, (a) => // foreach (var a in ans)
                                        {
                                            //词义特征
                                            for (int x = 0; x < 6; x++)
                                            {
                                                feature[x] += (WordsHash[x].Contains(a) ? 1 : 0);
                                            }

                                        });
                                        // if (feature.Count(a => a != 0) == 0)
                                        //{
                                        //  continue;
                                        //}
                                        foreach (var fea in feature)
                                        {
                                            writer.Write(fea / ans.Count() + ",");
                                        }
                                        datewriter.WriteLine(data.publishTime);
                                        writer.WriteLine();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 初始化词表
        /// </summary>
        public static void initHash(string fileName,HashSet<string> hash)
        {
            using (var inStream = new FileStream(fileName, FileMode.Open))
            {
                using (var inReader = new StreamReader(inStream,System.Text.Encoding.GetEncoding("gb2312")))
                {
                    while(!inReader.EndOfStream)
                    {
                        hash.Add(inReader.ReadLine().Trim());
                    }
                }
            }
        }

        public static void GetFeature(string fileName)
        {
            var seg = new JiebaSegmenter();//new JiebaNet.Segmenter.PosSeg.PosSegmenter();
            using (var inStream = new FileStream(fileName,FileMode.Open))
            {
                using (var inReader = new StreamReader(inStream, System.Text.Encoding.GetEncoding("gb2312")))
                {
                    using (var outStream = new FileStream(fileName + "feature", FileMode.Create))
                    {
                        using (var outWriter = new StreamWriter(outStream))
                        {
                            int error = 0;
                            while(!inReader.EndOfStream)
                            {
                                var feature = new float[6];
                                var str = inReader.ReadLine();
                               // try
                               // {
                                    var ans = seg.Cut(str);

                                    Parallel.ForEach(ans, (a) => // foreach (var a in ans)
                                    {
                                        //词义特征
                                        for (int i = 0; i < 6; i++)
                                        {
                                            feature[i] += (WordsHash[i].Contains(a) ? 1 : 0);
                                        }
                                        //词性特征
                                       // switch (a.Flag)
                                      //  {
                                      //      case "ns": feature[6]++; break;
                                       //     case "v": feature[7]++; break;
                                       //     case "n": feature[8]++; break;
                                       //     case "ul": feature[9]++; break;
                                       // }
                                    });
                                    if (feature.Count(a => a != 0) == 0)
                                    {
                                        continue;
                                    }
                                    foreach (var fea in feature)
                                    {
                                        outWriter.Write(fea / ans.Count() + ",");
                                    }
                                    outWriter.WriteLine();
                                //}
                                //catch { Console.WriteLine("error");error++; }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 中文语料获取
        /// </summary>
        public static void NoneAds()
        {
            using (var stream = new FileStream(@"E:\619892\corpus_0.05", FileMode.Open))
            {
                using (var outputStream = new FileStream(@"E:\619892\Chinese.txt", FileMode.Create))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        using (var writer = new StreamWriter(outputStream))
                        {
                            while (!reader.EndOfStream)
                            {
                                var row = reader.ReadLine();
                                row.Trim();
                                if (row[0] <= 'z' && row[0] >= 'a' || row[0] <= 'Z' && row[0] >= 'A')
                                {
                                    continue;
                                }
                                writer.WriteLine(row);
                            }
                        }
                    }
                }
            }
        }
    }
}
