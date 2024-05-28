using System;
using OpenQA.Selenium;

using ArbibetProgram.Models;
using OpenQA.Selenium.Support.UI;
using System.Globalization;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
namespace ArbibetProgram.Crawling
{
    public partial class Aa2888 : CasinoAPI
    {
        public override decimal getBalance()
        {
            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.Id("spanBetCredit")));
            string balanceStr = chromeDriver.FindElementById("spanBetCredit").GetAttribute("innerHTML");

            decimal balance = (decimal.Parse(balanceStr, CultureInfo.InvariantCulture.NumberFormat));

            return balance;
        }
    }
}
