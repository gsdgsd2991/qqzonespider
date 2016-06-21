using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using JiebaNet.Segmenter;

namespace dataAnalyse
{
    class Program
    {
        private static HashSet<string>[] WordsHash = new HashSet<string>[6];

        static void Main(string[] args)
        {
            //    string[] wordDict =
            //        { //@"C:\Users\gsdgs_000\Desktop\情感\words\主张词语（中文）.txt",
            //        @"C:\Users\gsdgs_000\Desktop\情感\words\正面情感词语（中文）.txt",
            //        // @"C:\Users\gsdgs_000\Desktop\情感\words\正面评价词语（中文）.txt",
            //         @"C:\Users\gsdgs_000\Desktop\情感\words\负面情感词语（中文）.txt",
            //        // @"C:\Users\gsdgs_000\Desktop\情感\words\负面评价词语（中文）.txt",
            //        // @"C:\Users\gsdgs_000\Desktop\情感\words\程度级别词语（中文）.txt"
            //    };
            //    var seg = new JiebaSegmenter();
            //    for (int i = 0; i < 2; i++)
            //    {
            //        WordsHash[i] = new HashSet<string>();
            //        initHash(wordDict[i], WordsHash[i]);
            //    }
            //    using (var enti = new starEntities())
            //    {
            //        using (var stream = new FileStream(@"C:\Users\gsdgs_000\Desktop\情感\words\emo.txt", FileMode.Create)) {
            //            using (var writer = new StreamWriter(stream))
            //            {
            //                var dataCount = enti.QQData.Count();
            //                for (int i = 0; i < dataCount / 200; i++)
            //                {
            //                    var datas = enti.QQData.OrderBy(a => a.dataIndex).Skip(i * 200).Take(200).ToList();
            //                    foreach (var data in datas)
            //                    {
            //                        float feature = 0;

            //                        var ans = seg.Cut(data.text);

            //                        Parallel.ForEach(ans, (a) => // foreach (var a in ans)
            //                        {
            //                        //词义特征
            //                        //for (int x = 0; x < 2; x++)
            //                           // {
            //                                feature += (WordsHash[0].Contains(a) ? 1 : 0);
            //                            feature += (WordsHash[1].Contains(a) ? (-1) : 0);
            //                            //}

            //                        });
            //                        // if (feature.Count(a => a != 0) == 0)
            //                        //{
            //                        //  continue;
            //                        //}
            //                        // foreach (var fea in feature)
            //                        //{
            //                        if (feature > 0)
            //                            writer.WriteLine(1);
            //                        else if (feature < 0)
            //                            writer.WriteLine(-1);
            //                        else
            //                            writer.WriteLine(0);
            //                       // }
            //                        //datewriter.WriteLine(data.publishTime);
            //                        //writer.WriteLine();
            //                    }
            //                }
            //            }
            //    }
            //}

            //var dataPath = Console.ReadLine();
            //var timePath = Console.ReadLine();
            //int[] ans = new int[10];
            //int[] mid = new int[10];
            //int[] neg = new int[10];
            //int[] pos = new int[10];
            ////int[] all = new int[10];
            //using (var wStream = new FileStream(dataPath + "ans", FileMode.Create))
            //{
            //    using (var writer = new StreamWriter(wStream))
            //    {
            //        using (var dStream = new FileStream(dataPath, FileMode.Open))
            //        {
            //            using (var dReader = new StreamReader(dStream))
            //            {
            //                using (var tStream = new FileStream(timePath, FileMode.Open))
            //                {
            //                    using (var tReader = new StreamReader(tStream))
            //                    {
            //                        while (!dReader.EndOfStream)
            //                        {
            //                            var time = DateTime.Parse(tReader.ReadLine().Trim()).Year;
            //                            time = time > 2016 ? 2016 : time;
            //                            time = time < 2006 ? 2006 : time;
            //                            // all[2016 - time]++;
            //                            try
            //                            {
            //                                int x = int.Parse(dReader.ReadLine().Trim());
            //                                switch (x) {
            //                                    case 1: pos[2016 - time]++;break;
            //                                    case 0:mid[2016 - time]++;break;
            //                                    case -1:neg[2016 - time]++;break;
            //                                }
            //                            }
            //                            catch { }
            //                        }
            //                        foreach (var a in pos)
            //                        {
            //                            writer.Write(a + " ");
            //                        }
            //                        writer.WriteLine();
            //                        foreach (var a in mid)
            //                        {
            //                            writer.Write(a + " ");
            //                        }
            //                        writer.WriteLine();
            //                        foreach (var a in neg)
            //                        {
            //                            writer.Write(a + " ");
            //                        }
            //                        writer.WriteLine();
            //                        //foreach (var an in ans)
            //                       // {
            //                       //     writer.Write(an + " ");
            //                       // }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
             Week();
        }

        /// <summary>
        /// 初始化词表
        /// </summary>
        public static void initHash(string fileName, HashSet<string> hash)
        {
            using (var inStream = new FileStream(fileName, FileMode.Open))
            {
                using (var inReader = new StreamReader(inStream, System.Text.Encoding.GetEncoding("gb2312")))
                {
                    while (!inReader.EndOfStream)
                    {
                        hash.Add(inReader.ReadLine().Trim());
                    }
                }
            }
        }

        public static void Week()
        {
            var dataPath = Console.ReadLine();
            var timePath = Console.ReadLine();
            int[] pos = new int[7];
            int[] mid = new int[7];
            int[] neg = new int[7];
            //int[] all = new int[7];
            using (var wStream = new FileStream(dataPath + "weekans", FileMode.Create))
            {
                using (var writer = new StreamWriter(wStream))
                {
                    using (var dStream = new FileStream(dataPath, FileMode.Open))
                    {
                        using (var dReader = new StreamReader(dStream))
                        {
                            using (var tStream = new FileStream(timePath, FileMode.Open))
                            {
                                using (var tReader = new StreamReader(tStream,Encoding.GetEncoding("gb2312")))
                                {
                                    while (!dReader.EndOfStream)
                                    {
                                        var time = tReader.ReadLine().Trim();
                                        //time = time > 2016 ? 2016 : time;
                                        //time = time < 2006 ? 2006 : time;
                                        var ti = 0;
                                        switch(time)
                                        {
                                            case "周一": ti = 0; break;
                                            case "周二": ti = 1; break;
                                            case "周三": ti = 2; break;
                                            case "周四": ti = 3; break;
                                            case "周五": ti = 4; break;
                                            case "周六": ti = 5; break;
                                            case "周日": ti = 6; break;

                                        }
                                        //all[ti]++;
                                        try
                                        {
                                            switch(int.Parse(dReader.ReadLine().Trim()))
                                            {
                                                case 1:pos[ti]++;break;
                                                case 0:mid[ti]++;break;
                                                case -1:neg[ti]++;break;
                                            }
                                        }
                                        catch { }
                                    }
                                    foreach (var a in pos)
                                    {
                                        writer.Write(a + " ");
                                    }
                                    writer.WriteLine();
                                    foreach (var a in mid)
                                    {
                                        writer.Write(a + " ");
                                    }
                                    writer.WriteLine();
                                    foreach (var a in neg)
                                    {
                                        writer.Write(a + " ");
                                    }
                                    writer.WriteLine();
                                   
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
