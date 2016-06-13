using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using Winista.Text.HtmlParser;
using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser.Filters;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data.Entity.Validation;

namespace QQZoneParser
{
    class Program
    {
        static void Main(string[] args)
        {
            getData();
            //using (var stream= new FileStream(@"C:\Users\gsdgs_000\Desktop\test22.html",FileMode.Open)) 
            //{
            //    using (var reader = new StreamReader(stream,Encoding.GetEncoding("gb2312")))
            //    {
            //        var html = reader.ReadToEnd();
            //        var scriptFilter = new NodeClassFilter(typeof(Winista.Text.HtmlParser.Tags.ScriptTag));//获取script类型
            //        var divFilter = new NodeClassFilter(typeof(Winista.Text.HtmlParser.Tags.Div));//获取div类型
            //        var classFilter = new HasAttributeFilter("class", "f-info");//获取说说文字
            //        var nickFilter = new HasAttributeFilter("class", "f-nick");//获取昵称
            //        var filter = new AndFilter(divFilter, classFilter);
            //        var par = Parser.CreateParser(html,"utf-8");
            //        var nodes = par.Parse(scriptFilter);//filter
            //        for(int i = 0;i < nodes.Count;i++)
            //        {
            //            //traverse(nodes[i],0);
            //            feedTimes(nodes.ToHtml());
            //        }
            //    }
            //}
        }
        /// <summary>
        /// 获取说说发布时间
        /// </summary>
        /// <param name="str"></param>
        static List<DateTime> feedTimes(string str)
        {
            var pat = "feedstime:'(\\d+年)?\\d+月\\d+日 \\d+:\\d+'";
            var matches = Regex.Matches(str, pat);
            var pat2 = "(\\d+年)?\\d+月\\d+日 \\d+:\\d+";
            var times = new List<DateTime>();
            foreach(var match in matches)
            {
                var x = Regex.Match(match.ToString(), pat2).ToString();
                var tim = Convert.ToDateTime(x);
                times.Add(tim);
            }
            return times;
        }

        static void traverse(INode node,int tabCount)
        {
            for (int i = 0; i < tabCount; i++)
                Console.Write('\t');
            Console.WriteLine('-'+node.GetText());
            if (node.Children == null||node.Children.Count == 0)
                return;
            for(int i = 0;i < node.Children.Count;i++)
            {
                node.GetType();
                
                traverse(node.Children[i],tabCount+1);
            }
        }
        
        static IEnumerable<QQData> getQQData(string html,int qqnum)
        {
            var datas = new List<QQData>();
            try
            {
                var scriptFilter = new NodeClassFilter(typeof(Winista.Text.HtmlParser.Tags.ScriptTag));//获取script类型
                var divFilter = new NodeClassFilter(typeof(Winista.Text.HtmlParser.Tags.Div));//获取div类型
                var classFilter = new HasAttributeFilter("class", "f-info");//获取说说文字
                var nickFilter = new HasAttributeFilter("class", "f-name q_namecard ");//获取昵称
                var filter = new AndFilter(divFilter, classFilter);
                var par = Parser.CreateParser(html, "utf-8");
                var nodes = par.Parse(scriptFilter);//filter
                List<DateTime> times = new List<DateTime>();
                //获得说说时间
                for (int i = 0; i < nodes.Count; i++)
                {
                    //traverse(nodes[i],0);
                    var x = feedTimes(nodes.ToHtml());
                    if (x != null && x.Count != 0) times = x;
                }
                par = Parser.CreateParser(html, "utf-8");
                nodes = par.Parse(filter);
                List<string> texts = new List<string>();
                //获得说说内容
                for(int i = 0;i < nodes.Count;i++)
                {
                    texts.Add(nodes[i].Children[0].ToHtml());
                }
                par = Parser.CreateParser(html, "utf-8");
                nodes = par.Parse(nickFilter);
                string nick = "";
                for (int n = 0;n < nodes[0].Children.Count;n++)
                {
                    //var ty = nodes[0].Children[n].GetType();
                    if (nodes[0].Children[n].GetType() == typeof(Winista.Text.HtmlParser.Nodes.TextNode))
                        nick = nodes[0].Children[n].ToHtml();
                   // nodes = nodes[0].Children;
                }
                
                for(int i = 0;i < texts.Count;i++)
                {
                    datas.Add(new QQData {
                        nickName = nick,
                        publishTime = times[i],
                        qqNum = qqnum,
                        text = texts[i],
                        dataIndex = Guid.NewGuid().ToString()
                    });
                }
            }
            catch
            {
                Console.WriteLine("解析错误");
            }
            return datas;
        }

        static void getData()
        {
            using (var fromContext = new starEntities())
            {
                int i = 0;
                int count = fromContext.qzonespider.Where(a=>a.qqnumber >= 10000000).Count();
                for (int j = 0;j < count/200;j++)
                {
                    var d = fromContext.qzonespider.OrderBy(a=>a.qqnumber).Where(a=>a.qqnumber >= 10000000).Skip(200*j).Take(200).ToList();
                    foreach (var item in d)
                    {
                        fromContext.QQData.AddRange(getQQData(item.html, item.qqnumber));
                        Console.WriteLine(item.qqnumber);
                        if (i == 100)
                        {
                            Console.WriteLine("写入数据库");
                            try
                            {
                                fromContext.SaveChanges();
                            }
                            catch (DbEntityValidationException dbEx) {
                                Console.WriteLine(dbEx.HResult);
                            }
                            i = 0;
                        }
                        i++;
                    }
                }
            }
        }
    }
}
