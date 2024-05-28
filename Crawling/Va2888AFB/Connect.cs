using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArbibetProgram.Functions;
using ArbibetProgram.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
	public partial class Va2888AFB : CasinoAPI
	{
		public override void Connect(AccountData prm_AccountAPI = null)
		{
			if (prm_AccountAPI != null)
			{
				base.accountAPI = prm_AccountAPI;
			}

			NLogger.Log(EventLevel.Info, $"Connect to {prm_AccountAPI.AccountBookmakerName}...");

            chromeOptions = new ChromeOptions();
            if (accountAPI.BookmakerShowPage)
            {
                chromeOptions.AddArguments("headless");
            }
            chromeDriver = new ChromeDriver(chromeOptions);
            chromeDriver.Url = accountAPI.AccountBookmakerUrl;
            //chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

            bool loginFormFounded = false;
            bool oddTablesFounded = false;
            try
            {
	            Login(10);


                // Get AFP Odds

                var wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(60));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//html/body/div[2]/div/div/div/a[3]")));
                chromeDriver.FindElementByXPath("//html/body/div[2]/div/div/div/a[3]").Click();
                NLogger.Log(EventLevel.Debug, chromeDriver.Url);

                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(60));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//html/body/div[3]/div/div/div/div/div[2]/div/a[1]")));
                chromeDriver.FindElementByXPath("//html/body/div[3]/div/div/div/div/div[2]/div/a[1]").Click();


                // Get Odds Data after connected
                try
                {
                    wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(60));
                    wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.XPath("/html/body/div[3]/div/iframe")));
                    //chromeDriver.SwitchTo().Frame(0);
                    NLogger.Log(EventLevel.Debug, "We are in Iframe");
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("txtSpName_1_1_3")));
                    chromeDriver.FindElementById("txtSpName_1_1_3").Click();


                }
                catch
                {
                    NLogger.Log(EventLevel.Error, "va2888AFB probably in maintenance");
                }
            }
            catch (NoSuchElementException elemNotFound)
            {
                NLogger.Log(EventLevel.Error, $"va2888AFB login not found : 001 {elemNotFound}");
            }

            if (!loginFormFounded)
            {
                return;
            }

            if (!oddTablesFounded)
            {
                return;
            }
        }

		protected override void removingPopup()
		{
		}

	}
}
