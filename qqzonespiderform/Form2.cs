using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Text.RegularExpressions;
using Winista.Text.HtmlParser;
using Winista.Text.HtmlParser.Filters;

namespace qqzonespiderform
{
    public partial class Form2 : Form
    {
        string realUrl1 = "http://ic2.s12.qzone.qq.com/cgi-bin/feeds/feeds_html_module?i_uin=";
        string realUrl2 = "&i_login_uin=1822683003&mode=4&previewV8=1&style=31&version=8&needDelOpr=true&transparence=true&hideExtend=false&showcount=5&MORE_FEEDS_CGI=http:%2F%2Fic2.s12.qzone.qq.com%2Fcgi-bin%2Ffeeds%2Ffeeds_html_act_all&refer=2&paramstring=os-win7|100";
        int seed = 1800000000;
       

        CookieContainer cookieContainer = new CookieContainer(10);

        public Form2()
        {
            InitializeComponent();
           
            webBrowser1.Navigate("http://m.qzone.com/");
            //webBrowser1.Navigating += (s, ev) =>
            //{
            //    var x = ev.Url;
            // };
        }
        /// <summary>
        /// 爬取网页
        /// </summary>
        /// <param name="cookie"></param>
        private async void getPage(string cookie)
        {
            var cookievalues = cookie.Split(';');
            //var rand = new Random();
            //rand.NextDouble();
            foreach (var cookievalue in cookievalues)
            {
                var val = cookievalue.Split('=');
                var coo = new Cookie(val[0].Trim(), val[1].Trim(), "/", "qq.com");
                cookieContainer.Add(coo);
            }
            var entity = new starEntities();


            var tasks = new Task[10];//10000
            for (int i = 0; i < 10; i++)//10000
            {
                tasks[i] = new Task(new Action<object>(insertDatabase), i);

            }
            foreach (var t in tasks)
            {
                t.Start();
            }


        }
        /// <summary>
        /// 插入数据库
        /// </summary>
        /// <param name="num"></param>
        /// <param name="html"></param>
        private async void insertDatabase(object i)
        {
            var rand = new Random((int)i);
            var entity = new starEntities();
            var qzones = new qzonespider[200];
            int qzonesi = 0;
            for (int j = 0; j < 2000000; j++)//2000
            {

                var qqnum = 0;
               
               qqnum = (j * 10000 + seed + (int)i);
                            
                var realUrl = realUrl1 + qqnum + realUrl2;
                var request = (HttpWebRequest)WebRequest.Create(realUrl);
                
                request.CookieContainer = cookieContainer;
                // Console.Write(qqnum);
                try
                {
                    using (var res = request.GetResponse())
                    {
                        using (var responseStream = res.GetResponseStream())
                        {
                            using (var reader = new StreamReader(responseStream))
                            {

                                var responseString = reader.ReadToEnd();
                                if (responseString.Contains("对方空间未开通") || responseString.Contains("您没有权限访问对方的空间"))
                                {

                                    continue;
                                }
                                qzones[qzonesi] = new qzonespider { html = responseString, qqnumber = qqnum };
                                qzonesi++;
                                //Console.WriteLine("成功");
                            }
                        }
                    }
                }
                catch
                {
                    //Console.WriteLine("失败");
                }
                if (qzonesi >= 200)
                {
                    entity.qzonespider.AddRange(qzones);
                    await entity.SaveChangesAsync();
                    qzonesi = 0;
                }
                //qzonesi++;
            }

            // var count = num.Count();
            // for (int i = 0; i < count; i++)
            // {
            //     entity.qzonespider.Add(new qzonespider() { qqnumber = num[i], html = html[i] });
            // }

            entity.Dispose();
        }

        private void getTalk(string responseString)
        {
            var parser = Parser.CreateParser(responseString, "utf-8");
            var nodes = parser.Parse(null);


        }
        /// <summary>
        /// 开始爬取数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            var qqzoneCookie = webBrowser1.Document.Cookie;
            //webBrowser1.Navigate("http://user.qzone.qq.com/1822683110");
            // var responseString = getPage(qqzoneCookie);
            //getTalk(responseString);
            getPage(qqzoneCookie);
        }
    }
}
