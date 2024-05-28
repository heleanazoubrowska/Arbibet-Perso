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
    public partial class Va2888AFB : CasinoAPI
    {
        public override decimal getBalance()
        {
            chromeDriver.SwitchTo().DefaultContent();

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.Id("member_creadit")));
            string balanceStr = chromeDriver.FindElementById("member_creadit").GetAttribute("innerHTML");

            balanceStr = balanceStr.Replace("(USD)", "").Replace(" ", "");

            decimal balance = (decimal.Parse(balanceStr, CultureInfo.InvariantCulture.NumberFormat));

            chromeDriver.SwitchTo().Frame(chromeDriver.FindElement(By.XPath("//html/body/div[3]/div/iframe")));
            return balance;
        }
    }
}
