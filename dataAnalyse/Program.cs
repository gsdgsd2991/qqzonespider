using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace dataAnalyse
{
    class Program
    {
        static void Main(string[] args)
        {
            //var dataPath = Console.ReadLine();
            //var timePath = Console.ReadLine();
            //int[] ans = new int[10];
            //int[] all = new int[10];
            //using (var wStream = new FileStream(dataPath+"ans",FileMode.Create))
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
            //                        while(!dReader.EndOfStream)
            //                        {
            //                            var time = int.Parse(tReader.ReadLine().Trim());
            //                            time = time > 2016 ? 2016 : time;
            //                            time = time < 2006 ? 2006 : time;
            //                            all[2016-time]++;
            //                            try
            //                            {
            //                                ans[2016 - time] += int.Parse(dReader.ReadLine().Trim());
            //                            }
            //                            catch { }
            //                        }
            //                        foreach(var a in all)
            //                        {
            //                            writer.Write(a + " ");
            //                        }
            //                        writer.WriteLine();
            //                        foreach(var an in ans)
            //                        {
            //                            writer.Write(an + " ");
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
           // Week();
        }

        public static void Week()
        {
            var dataPath = Console.ReadLine();
            var timePath = Console.ReadLine();
            int[] ans = new int[7];
            int[] all = new int[7];
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
                                        all[ti]++;
                                        try
                                        {
                                            ans[ti] += int.Parse(dReader.ReadLine().Trim());
                                        }
                                        catch { }
                                    }
                                    foreach (var a in all)
                                    {
                                        writer.Write(a + " ");
                                    }
                                    writer.WriteLine();
                                    foreach (var an in ans)
                                    {
                                        writer.Write(an + " ");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
