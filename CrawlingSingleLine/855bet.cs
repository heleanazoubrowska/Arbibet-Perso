using System;
using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;

using ArbibetProgram.Models;
using System.Globalization;
using System.Linq;
namespace ArbibetProgram.CrawlingSingleLine
{
    public class B855bet
    {
        public B855bet()
        {
        }

        public static void Connect855bet(Bookmaker aa2888)
        {
            var chromeDriver = new ChromeDriver();
            chromeDriver.Url = aa2888.BookmakerUrl;
            chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            //chromeDriver.ExecuteScript
        }
    }
}
