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
	public partial class Cambo88UG : CasinoAPI
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

				Login(10);

				isLogin = true;

				// +++++++++++++++++++++++
				// Removing Popup
				// +++++++++++++++++++++++
				removingPopup();
			}
			catch
			{
				NLogger.Log(EventLevel.Error, "Cambo88 UG : Fail to login");
			}
		}

        protected override void removingPopup()
        {
            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            NLogger.Log(EventLevel.Debug, chromeDriver.Url);

            try
            {
                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(30));
                wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("dialog-box")));
                int nbPopup = chromeDriver.FindElementsByClassName("dialog-box").Count;
                NLogger.Log(EventLevel.Debug, $"Popup found : {nbPopup}");

                try
                {
                    if (nbPopup == 2)
                    {
                        for (int i = 0; i < nbPopup; i++)
                        {
                            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(5));
                            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath($"html/body/div[{(5 - i)}]/div/div[4]/div[2]")));
                            chromeDriver.FindElement(By.XPath($"html/body/div[{(5 - i)}]/div/div[4]/div[2]")).Click();
                        }
                    }
                    if (nbPopup == 1)
                    {
                        NLogger.Log(EventLevel.Error, "Cambo88 UG : Maintenance");
                        return;
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

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(60));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("ug_sports")));
            chromeDriver.FindElementById("ug_sports").Click();

            Thread.Sleep(5000);

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
            try
            {
                //IWebElement gridFutur = chromeDriver.FindElementByXPath("//*[@id='grid']/table/tbody");

                //string html = gridFutur.GetAttribute("innerHTML");

                //HtmlDocument htmlDoc = new HtmlDocument();
                //htmlDoc.LoadHtml(html);
                //getOdds(cambo88UG, htmlDoc);
            }
            catch (NoSuchElementException elemNotFound)
            {
                NLogger.Log(EventLevel.Error, $"Cambo88 UG odds table not found : {elemNotFound}");
            }
        }

    }
}
