using System;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Ibet789 : CasinoAPI
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
                if (accountAPI.BookmakerShowPage)
                {
                    chromeOptions.AddArguments("headless");
                }
                chromeDriver = new ChromeDriver(chromeOptions);
                chromeDriver.Url = accountAPI.AccountBookmakerUrl;
                //chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

                //bool loginFormFounded = false;
                //bool oddTablesFounded = false;
                try
                {
	                Login(30);


                    // Get Odds Data after connected

                    NLogger.Log(EventLevel.Debug, "before switch");
                    chromeDriver.SwitchTo().Frame("fraMain");
                    NLogger.Log(EventLevel.Debug, "after switch");


                    IWebElement Depart = chromeDriver.FindElement(By.Name("accTpLst"));
                    SelectElement select = new SelectElement(Depart);
                    select.SelectByIndex(0);
                }
                catch
                {
                    NLogger.Log(EventLevel.Error, "Ibet789 login not found : ");
                }
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Ibet789 : Fail to login ");
            }

        }

        protected override void removingPopup()
        {
        }
    }
}
