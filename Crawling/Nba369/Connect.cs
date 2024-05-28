using System;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Nba369 : CasinoAPI
    {
	    public override void Connect(AccountData prm_AccountAPI = null)
	    {
		    if (prm_AccountAPI != null)
		    {
			    base.accountAPI = prm_AccountAPI;
		    }

		    NLogger.Log(EventLevel.Info, $"Connect to {prm_AccountAPI.AccountBookmakerName}...");

            try
            {
                chromeOptions = new ChromeOptions();
                chromeOptions.AddArguments("--window-size=1920,1080");
                if (accountAPI.BookmakerShowPage)
                {
                    chromeOptions.AddArguments("headless");

                }
                chromeDriver = new ChromeDriver(chromeOptions);
                chromeDriver.Url = accountAPI.AccountBookmakerUrl;
                try
                {
	                Login(30);
                    NLogger.Log(EventLevel.Debug, "H 5");

                    var wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(20));
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("tabsport")));
                    chromeDriver.FindElementById("tabsport").Click();
                    NLogger.Log(EventLevel.Debug, "H 6");
                    bool loginFormFounded = false;
                    bool oddTablesFounded = false;
                }

                catch
                {
                    return;
                }
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Nba369 : Fail to connect ");
            }

        }

        protected override void removingPopup()
        {
        }
    }
}
