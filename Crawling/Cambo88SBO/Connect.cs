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
    public partial class Cambo88SBO : CasinoAPI
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
                NLogger.Log(EventLevel.Error, "Cambo88 SBO : Fail to login");
            }
        }

        protected override void removingPopup()
        {

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));

            try
            {
                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(60));
                wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("dialog-box")));
                int nbPopup = chromeDriver.FindElementsByClassName("dialog-box").Count;
                NLogger.Log(EventLevel.Debug, $"Popup found : {nbPopup}");

                try
                {
                    if (nbPopup > 0)
                    {
                        for (int i = 0; i < nbPopup; i++)
                        {
                            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(15));
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

            // +++++++++++++++++++++++
            // Switch to New Window
            // +++++++++++++++++++++++

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='js_headerNav']/a[2]")));
            chromeDriver.FindElementByXPath("//*[@id='js_headerNav']/a[2]").Click();

            chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);

            chromeDriver.SwitchTo().Frame("bsportFrame");
            NLogger.Log(EventLevel.Debug, chromeDriver.Url);

            try
            {
                //IWebElement gridFutur = chromeDriver.FindElementByXPath("//*[@id='grid']/table/tbody");

                //string html = gridFutur.GetAttribute("innerHTML");

                //HtmlDocument htmlDoc = new HtmlDocument();
                //htmlDoc.LoadHtml(html);
                //getOdds(cambo88SBO, htmlDoc);
            }
            catch (NoSuchElementException elemNotFound)
            {
                NLogger.Log(EventLevel.Error, $"Cambo88 SBO odds table not found : {elemNotFound}");
            }
        }
    }
}
