using System;
using OpenQA.Selenium;

using ArbibetProgram.Models;
using OpenQA.Selenium.Support.UI;
using System.Globalization;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
namespace ArbibetProgram.Crawling
{
    public partial class Nba369 : CasinoAPI
    {
        public override decimal getBalance()
        {

            chromeDriver.SwitchTo().DefaultContent();

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.Id("balance")));
            string balanceStr = chromeDriver.FindElementById("balance").GetAttribute("innerHTML");

            balanceStr = balanceStr.Replace("(USD)", "").Replace(" ", "");

            decimal balance = (decimal.Parse(balanceStr, CultureInfo.InvariantCulture.NumberFormat));

            return balance;
        }
    }
}
