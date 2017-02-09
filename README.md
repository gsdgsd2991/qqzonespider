# qqzonespider
Getting published tweets of tencent's QQ Zone.
## qqzonespiderform
* 使用winform的webview获得登录qq空间的cookie并加入到webrequest的请求中，从而模拟浏览器行为。
* 因为QQ空间的请求有多次跳转，所以我截取了几次跳转所使用的url，发现获取用户说说的那次跳转中将qq号改为所要爬取用户的qq号就可以获得用户的说说html。
* 程序使用了entity framework连接sql server，将爬取到的html存入数据库。

## QQZoneParser
* 因为QQ空间的日期信息和说说信息在html中是分别存储的所以说说和日期信息要分别提取。
* 爬取到的html利用Winista.Text.HtmlParser获取说说相关的标签，从而获得说说内容等数据。
* 日期通过正则表达式识别并转换为datetime类型。

## SplitWord
* 利用结巴分词对爬取到的说说信息进行分词处理。
* 利用知网词库对说说中的爬到的各类词频进行统计，计算每个说说的特征，输出文件方便使用matlab等处理。

## dataAnalyse
* 粗略统计用户使用QQ空间的频率等信息。
