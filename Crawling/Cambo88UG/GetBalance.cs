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
    public partial class Cambo88UG : CasinoAPI
    {
        public override decimal getBalance()
        {

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@id='divBlance']/div[1]/div/div/div/div/span")));
            string balanceStr = chromeDriver.FindElementByXPath("//*[@id='divBlance']/div[1]/div/div/div/div/span").GetAttribute("innerHTML");

            decimal balance = (decimal.Parse(balanceStr, CultureInfo.InvariantCulture.NumberFormat));

            return balance;
        }
    }
}
