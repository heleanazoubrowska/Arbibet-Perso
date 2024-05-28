using System;
using OpenQA.Selenium;

using ArbibetProgram.Models;
using OpenQA.Selenium.Support.UI;
using System.Globalization;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
namespace ArbibetProgram.Crawling
{
    public partial class Ibc05 : CasinoAPI
    {
        public override decimal getBalance()
        {

            chromeDriver.SwitchTo().DefaultContent();

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.Id("member_creadit")));
            string balanceStr = chromeDriver.FindElementById("member_creadit").GetAttribute("innerHTML");

            decimal balance = (decimal.Parse(balanceStr, CultureInfo.InvariantCulture.NumberFormat));

            chromeDriver.SwitchTo().DefaultContent();
            chromeDriver.SwitchTo().Frame(0);
            chromeDriver.SwitchTo().Frame("mainIframe3");

            return balance;
        }
    }
}
