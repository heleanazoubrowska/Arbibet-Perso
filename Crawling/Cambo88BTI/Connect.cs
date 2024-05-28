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
    public partial class Cambo88BTI : CasinoAPI
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

            Login(10);


            // +++++++++++++++++++++++
            // Removing Popup
            // +++++++++++++++++++++++

            removingPopup();

        }

        protected override void removingPopup()
        {

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            try
            {
                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(30));
                wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("dialog-box")));
                Thread.Sleep(1500);
                int nbPopup = chromeDriver.FindElementsByClassName("dialog-box").Count;
                NLogger.Log(EventLevel.Debug, $"Popup found : {nbPopup}");

                try
                {
                    if (nbPopup > 0)
                    {
                        for (int i = 0; i < nbPopup; i++)
                        {
                            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(5));
                            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath($"html/body/div[{(5 - i)}]/div/div[4]/div[2]")));
                            chromeDriver.FindElement(By.XPath($"html/body/div[{(5 - i)}]/div/div[4]/div[2]")).Click();
                        }
                    }
                }
                catch
                {
                    NLogger.Log(EventLevel.Debug, "look not working");
                }
            }
            catch
            {
                NLogger.Log(EventLevel.Debug, "Popup NOT found");
            }

            NLogger.Log(EventLevel.Debug, "here 1");

            // +++++++++++++++++++++++
            // Switch to New Window
            // +++++++++++++++++++++++

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(30));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("bti_sports")));
            chromeDriver.FindElementById("bti_sports").Click();

            Thread.Sleep(3000);

            int nbWindow = chromeDriver.WindowHandles.Count;
            if (nbWindow < 2)
            {
                nbWindow = chromeDriver.WindowHandles.Count;
            }
            else
            {
                NLogger.Log(EventLevel.Error, "Cambo88 UG Cannot switch windows");
            }

            chromeDriver.SwitchTo().Window(chromeDriver.WindowHandles[1]);

            NLogger.Log(EventLevel.Debug, chromeDriver.Url);

        }
    }
}
