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
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Concurrent;

namespace qqzonespiderform
{
    public partial class Form2 : Form
    {
        string realUrl1 = "http://ic2.s12.qzone.qq.com/cgi-bin/feeds/feeds_html_module?i_uin=";
        string realUrl2 = "&i_login_uin=1822683003&mode=4&previewV8=1&style=31&version=8&needDelOpr=true&transparence=true&hideExtend=false&showcount=5&MORE_FEEDS_CGI=http:%2F%2Fic2.s12.qzone.qq.com%2Fcgi-bin%2Ffeeds%2Ffeeds_html_act_all&refer=2&paramstring=os-win7|100";
        int seed = 1800000000;

        MongoClient mongoClient = new MongoClient("mongodb://localhost:27017");
        IMongoDatabase database;
        IMongoCollection<BsonDocument> followCollection;
        IMongoCollection<BsonDocument> fansCollection;
        IMongoCollection<BsonDocument> infoCollection;
        //用于去重
        ConcurrentDictionary<String,int> unusedIds = new ConcurrentDictionary<String,int>();
        
        CookieContainer cookieContainer = new CookieContainer(10);
        String infoURL = "https://weibo.cn/{0}/info";
        String followURL = "https://weibo.cn/{0}/follow?page={1}";
        String fansURL = "https://weibo.cn/{0}/fans?page={1}";

        public Form2()
        {
            InitializeComponent();
            database = mongoClient.GetDatabase("sina");
            followCollection = database.GetCollection<BsonDocument>("follow");
            fansCollection = database.GetCollection<BsonDocument>("fans");
            infoCollection = database.GetCollection<BsonDocument>("detail");
            //起始微博id
            var weiboIds = new String[]{"1797054534", "2509414473", "2611478681", "5861859392", "2011086863", "5127716917", "1259110474", "5850775634", "1886437464",
    "3187474530", "2191982701", "1940562032", "5874450550", "1337925752", "2081079420", "5664530558", "3493173952", "1202806915",
    "1864507535", "2032640064", "5585682587", "3083673764", "5342109866", "5878685868", "5728706733", "2103050415", "5876752562",
    "3138085045", "5775974583", "1879400644", "2417139911", "5836619975", "5353816265", "5219508427", "1766613205", "2480158031",
    "5660754163", "2456764664", "3637354755", "1940087047", "5508473104", "1004454162", "2930327837", "1874608417", "5379621155",
    "1720664360", "2714280233", "3769073964", "5624119596", "2754904375", "5710151998", "5331042630", "5748179271", "2146132305",
    "2313896275", "3193618787", "5743059299", "1742930277", "5310538088", "1794474362", "2798510462", "3480076671", "5678653833",
    "5743657357", "5460191980", "1734164880", "5876988653", "5678031258", "5860163996", "1496924574", "5878970110", "1679704482",
    "1142210982", "3628925351", "1196397981", "1747485107", "5675893172", "5438521785", "2192269762", "1992614343", "5878686155",
    "2407186895", "5559116241", "2528477652", "1295950295", "5038203354", "3659276765", "2126733792", "5878350307", "2761179623",
    "5484511719", "5825708520", "1578230251", "5878686190", "5810946551", "3833070073", "1795047931", "5855789570", "3580125714",
    "5709578773", "5236539926", "2907633071", "1709244961", "5405450788", "3251257895", "5054538290", "2713199161", "5698445883",
    "1784537661", "3195290182", "1824506454", "5738766939", "5565915740", "5336031840", "5098775138", "5685568105", "1774289524",
    "2932662914", "5433223957", "2680044311", "1111523983", "5067889432", "5878686362", "2844992161", "3878314663", "1766548141",
    "5763269297", "5878383287", "5235499706", "5876375670", "5866447563", "5129945819", "1704116960", "1929380581", "1223762662",
    "1193476843", "2899591923", "5162099453", "5072151301", "5385741066", "5411455765", "2685535005", "2297905950", "1216766752",
    "5838668577", "5359133478", "3077460103", "5577802539", "5862392623", "1786700611", "1259258694", "1845191497", "1731838797",
    "1740301135", "2816074584", "1217733467", "5345035105", "5050827618", "5486257001", "5767857005", "2050605943", "5733778298",
    "1914725244", "5872583558", "5604377483", "1253491601", "5554922386", "3170223002", "5662737311", "3217179555", "1538163622",
    "5304533928", "5644198830", "1896650227", "5298774966", "2795873213", "1834378177", "5769651141", "2656256971", "5876433869",
    "1826792401", "3002246100", "3082519511", "5780366296", "5704696797", "5204108258", "2090615793", "1739746131", "1378010100",
    "5741331445", "2376442895", "3638486041", "5781365789", "1827234850", "5703214121", "1855398955", "1227908142", "5703820334" };
            foreach(String id in weiboIds) {
                unusedIds.TryAdd(id, 0);
            }
            webBrowser1.Navigate(urlString: "https://login.sina.com.cn/sso/login.php?client=ssologin.js(v1.4.18)");
          
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
            cookieContainer.Capacity = 50;

            foreach (var cookievalue in cookievalues)
            {
                var val = cookievalue.Split('=');
                var coo = new Cookie(val[0].Trim(), val[1].Replace(",","%2C").Trim(), "/", "weibo.cn");
                cookieContainer.Add(coo);
               
            }
           // var entity = new starEntities();
            //cookieContainer.SetCookies(new Uri("https://weibo.cn"), cookie);

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
           // var rand = new Random((int)i);
           // var entity = new starEntities();
          //  var qzones = new qzonespider[200];
         //   int qzonesi = 0;
            for (int j = 0; j < 50000000; j++)//2000
            {

                // var qqnum = 0;

                // qqnum = (j * 10000 + seed + (int)i);
                string userId = unusedIds.First().Key;
                int a = 0;
                unusedIds.TryRemove(userId,out a);
                //用户详情页url
                var realUrl = string.Format(infoURL, userId);//realUrl1 + qqnum + realUrl2;
              
                var request = (HttpWebRequest)WebRequest.Create(realUrl);
                
                request.CookieContainer = cookieContainer;
                // Console.Write(qqnum);
                try
                {
                    using (var res = request.GetResponse())
                    {
                        using (var responseStream = res.GetResponseStream())
                        {
                            using (var reader = new StreamReader(responseStream,Encoding.UTF8))
                            {
                               
                                var responseString = reader.ReadToEnd();
                                string regStr = @"location.replace\(""[a-zA-z]+://[^\s]*""\)";
                                string realURL = Regex.Match(responseString, regStr).Value;
                                realURL = realURL.Split('"')[1];
                                var realRequest = (HttpWebRequest)WebRequest.Create(realURL);
                                realRequest.CookieContainer = cookieContainer;
                                var realResponse = realRequest.GetResponse();
                                var realStream = realResponse.GetResponseStream();
                                var realReader = new StreamReader(realStream,Encoding.UTF8);
                                responseString = realReader.ReadToEnd();
                                realStream.Close();
                                realReader.Close();
                                //用户信息直接存储html加快爬虫速度
                                
                                await infoCollection.InsertOneAsync(new BsonDocument(new Dictionary<String, String> { { "id", userId }, { "html", responseString } }));
                                //用户信息获取成功后获取用户的粉丝信息
                                string firstPageUrl = string.Format(fansURL, userId, 1);
                                var firstPageRequest = (HttpWebRequest)WebRequest.Create(firstPageUrl);
                                firstPageRequest.CookieContainer = cookieContainer;
                                var firstPageStream = firstPageRequest.GetResponse().GetResponseStream();
                     
                                var firstPageReader = new StreamReader(firstPageStream,Encoding.UTF8);
                                string maxPageStr = Regex.Match(firstPageReader.ReadToEnd(),@"< input name = ""mp"" type = ""hidden"" value = ""\d+"" >").Value;
                                firstPageStream.Close();
                                firstPageReader.Close();
                                int maxPage = int.Parse(maxPageStr.Split('\"')[5]);

                                StringBuilder fansString = new StringBuilder();
                                for (int pageIndex = 1; pageIndex <= maxPage; pageIndex++) {
                                    var fansPageRequest = (HttpWebRequest)WebRequest.Create(string.Format(fansURL,userId,pageIndex));
                                    fansPageRequest.CookieContainer = cookieContainer;
                                    var fansPageStream = fansPageRequest.GetResponse().GetResponseStream();

                                    var fansPageReader = new StreamReader(fansPageStream,Encoding.UTF8);
                                    string pageStr = Regex.Match(fansPageReader.ReadToEnd(), @"<a href=""[a-zA-z]+://[^\s]*"">关注[她他]</a>").Value;
                                    fansPageStream.Close();
                                    fansPageReader.Close();
                                    fansString.Append(",");
                                    string fansId = pageStr.Split('=')[1].Split('&')[0];
                                    fansString.Append(fansId);
                                    if (!unusedIds.ContainsKey(fansId)) {
                                        unusedIds.TryAdd(fansId,0);
                                    }
                                }
                                await fansCollection.InsertOneAsync(new BsonDocument(new Dictionary<String, String> { { "id", userId }, { "fansId", fansString.ToString() } }));

                                //用户关注信息
                                string firstFollowPageUrl = string.Format(followURL, userId, 1);
                                var firstFollowPageRequest = (HttpWebRequest)WebRequest.Create(firstFollowPageUrl);
                                firstFollowPageRequest.CookieContainer = cookieContainer;
                                var firstFollowPageStream = firstFollowPageRequest.GetResponse().GetResponseStream();

                                var firstFollowPageReader = new StreamReader(firstFollowPageStream,Encoding.UTF8);
                                string maxFollowPageStr = Regex.Match(firstFollowPageReader.ReadToEnd(), @"< input name = ""mp"" type = ""hidden"" value = ""\d+"" >").Value;
                                firstFollowPageStream.Close();
                                firstFollowPageReader.Close();
                                int maxFollowPage = int.Parse(maxFollowPageStr.Split('\"')[5]);

                                StringBuilder followString = new StringBuilder();
                                for (int pageIndex = 1; pageIndex <= maxPage; pageIndex++)
                                {
                                    var followPageRequest = (HttpWebRequest)WebRequest.Create(string.Format(fansURL, userId, pageIndex));
                                    followPageRequest.CookieContainer = cookieContainer;
                                    var followPageStream = followPageRequest.GetResponse().GetResponseStream();

                                    var followPageReader = new StreamReader(followPageStream,Encoding.UTF8);
                                    string pageStr = Regex.Match(followPageReader.ReadToEnd(), @"<a href=""[a-zA-z]+://[^\s]*"">关注[她他]</a>").Value;
                                    followPageStream.Close();
                                    followPageReader.Close();
                                    followString.Append(",");
                                    string followerId = pageStr.Split('=')[1].Split('&')[0];
                                    followString.Append(followerId);
                                    if (!unusedIds.ContainsKey(followerId))
                                    {
                                        unusedIds.TryAdd(followerId, 0);
                                    }
                                }
                                await followCollection.InsertOneAsync(new BsonDocument(new Dictionary<String, String> { { "id", userId }, { "fansId", followString.ToString() } }));


                                // if (responseString.Contains("对方空间未开通") || responseString.Contains("您没有权限访问对方的空间"))
                                // {

                                //       continue;
                                //   }
                                //qzones[qzonesi] = new qzonespider { html = responseString, qqnumber = qqnum };
                                // qzonesi++;
                                //Console.WriteLine("成功");
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("失败");
                }
               // if (qzonesi >= 200)
                //{
               //     entity.qzonespider.AddRange(qzones);
               //     await entity.SaveChangesAsync();
               //     qzonesi = 0;
               // }
                //qzonesi++;
            }

            // var count = num.Count();
            // for (int i = 0; i < count; i++)
            // {
            //     entity.qzonespider.Add(new qzonespider() { qqnumber = num[i], html = html[i] });
            // }

            //entity.Dispose();
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
