using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using HtmlAgilityPack;
using System;
using System.Threading;
using System.Globalization;
using System.Text.RegularExpressions;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
namespace ArbibetProgram.Crawling
{
    public partial class Va2888 : CasinoAPI
    {
        public override decimal getBalance()
        {
            chromeDriver.SwitchTo().DefaultContent();

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            var watch = System.Diagnostics.Stopwatch.StartNew();
            wait.Until(ExpectedConditions.ElementExists(By.Id("member_creadit")));
            string balanceStr = chromeDriver.FindElementById("member_creadit").GetAttribute("innerHTML");

            watch.Stop();
            NLogger.Log(EventLevel.Trace, $"VA balance setp 1 odds in {watch.ElapsedMilliseconds} ms");

            balanceStr = balanceStr.Replace("(USD)", "").Replace(" ", "");

            decimal balance = (decimal.Parse(balanceStr, CultureInfo.InvariantCulture.NumberFormat));

            watch = System.Diagnostics.Stopwatch.StartNew();
            chromeDriver.SwitchTo().Frame(chromeDriver.FindElement(By.XPath("//html/body/div[3]/div/iframe")));
            watch.Stop();
            NLogger.Log(EventLevel.Trace, $"VA balance setp 2 odds in {watch.ElapsedMilliseconds} ms");

            watch = System.Diagnostics.Stopwatch.StartNew();
            chromeDriver.SwitchTo().Frame("fraMain");
            watch.Stop();
            NLogger.Log(EventLevel.Trace, $"VA balance setp 3 odds in {watch.ElapsedMilliseconds} ms");
            return balance;
        }
    }
}
