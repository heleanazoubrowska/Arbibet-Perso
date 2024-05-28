using System;
using OpenQA.Selenium;

using ArbibetProgram.Models;
using OpenQA.Selenium.Support.UI;
using System.Globalization;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
namespace ArbibetProgram.Crawling
{
    public partial class B855bet : CasinoAPI
    {
        public override decimal getBalance()
        {

            chromeDriver.SwitchTo().DefaultContent();
            chromeDriver.SwitchTo().Frame("leftFrame");

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.Id("txt_betcreditInfo")));
            string balanceStr = chromeDriver.FindElementById("txt_betcreditInfo").GetAttribute("innerHTML");

            decimal balance = (decimal.Parse(balanceStr, CultureInfo.InvariantCulture.NumberFormat));

            chromeDriver.SwitchTo().DefaultContent();
            chromeDriver.SwitchTo().Frame("mainFrame");

            return balance;
        }
    }
}
