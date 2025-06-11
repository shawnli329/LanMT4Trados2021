### 介绍
LanMT4Trados2021是一款用于Trados 2021的机翻引擎插件，使用的是[数译科技](https://openapi.dtranx.com/website/index)的LanMT机翻引擎，该引擎可支持176种语言，涵盖国际上的主流大语种和大量小语种，LanMT在WMT2022国际机器翻译大赛上勇夺2项冠军、8项亚军。单从机翻质量本身来说LanMT是非常不错的一款引擎，但由于该引擎一直没有推出Trados的机翻插件，对于使用Trados的译员不太友好，我也没有C#的开发基础，因此想到使用AI开发。

我是Claude Pro用户，使用Claude Sonnet 3.7和4.0来生成代码，整个开发周期零零星星大概用了3天，因为Claude限定了每日会话数，大概有三分之一的时间在等待。

开发过程参考了[这篇文章](https://blog.xulihang.me/SDL-Trados-plugins-development/)，以及sdl[官网的示例](http://producthelp.sdl.com/SDK/TranslationMemoryApi/2017/html/03670e46-3379-4005-baf3-7b1613115d60.htm)，和数译机翻api[开发文档](https://openapi.dtranx.com/website/guide/textTransApi)，使用了[这款油猴插件](https://greasyfork.org/zh-CN/scripts/486888-easy-web-page-to-markdown)将网页内容转成AI能识别的md文件以投喂给AI形成其Project knowledge。IDE是Visual Studio Community 2022。

加载插件：

![](https://cdn.nlark.com/yuque/0/2025/png/12646172/1749620193901-346c925f-626a-4c8f-abda-8d6d9200e1bc.png)

API设置：

![](https://cdn.nlark.com/yuque/0/2025/png/12646172/1749620219405-10dfab3f-9116-48b1-8c15-2ce18133092d.png)

Trados 编辑器：

![](https://cdn.nlark.com/yuque/0/2025/png/12646172/1749620374327-4c61a321-6c3f-492e-bac6-2c090ee9416a.png)

任何bug及建议可在[这里](https://github.com/shawnli329/LanMT4Trados2021/issues)提交。



