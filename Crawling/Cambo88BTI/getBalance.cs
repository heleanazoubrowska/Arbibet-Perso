using System;
using OpenQA.Selenium;

using ArbibetProgram.Models;
using OpenQA.Selenium.Support.UI;
using System.Globalization;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
namespace ArbibetProgram.Crawling
{
    public partial class Cambo88BTI : CasinoAPI
    {
        public override decimal getBalance()
        {

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/div[1]/div[2]/div/sb-block[1]/div/div/div[5]/div/div[2]/div[7]/div/div[2]/span[2]")));
            string balanceStr = chromeDriver.FindElementByXPath("/html/body/div[1]/div[2]/div/sb-block[1]/div/div/div[5]/div/div[2]/div[7]/div/div[2]/span[2]").GetAttribute("innerHTML");

            balanceStr = balanceStr.Replace("$", "");

            decimal balance = (decimal.Parse(balanceStr, CultureInfo.InvariantCulture.NumberFormat));

            return balance;
        }
    }
}
