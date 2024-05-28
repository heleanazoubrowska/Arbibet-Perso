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
    public partial class Ibc05 : CasinoAPI
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

                //bool oddTablesFounded = false;
                try
                {
	                Login(30);


                    // Get Odds Data after connected

                    chromeDriver.SwitchTo().Frame(0);
                    chromeDriver.SwitchTo().Frame("mainIframe3");
                    //chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

                    var wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(20));
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("aSetting")));

                    // Set Single line settings
                    chromeDriver.FindElement(By.Id("aSetting")).Click();
                    chromeDriver.FindElement(By.Id("chkSport2")).Click();
                    chromeDriver.FindElement(By.Id("btnSave")).Click();

                    try
                    {
                        wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(15));
                        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("tableTodayN")));
                        try
                        {
                            //IWebElement tableTodayN = chromeDriver.FindElementById("tableTodayN");
                            //oddTablesFounded = true;
                            //string html = tableTodayN.GetAttribute("innerHTML");

                            //HtmlDocument htmlDoc = new HtmlDocument();
                            //htmlDoc.LoadHtml(html);

                            //getOdds(ibc05, htmlDoc);
                        }
                        catch (NoSuchElementException elemNotFound)
                        {
                            NLogger.Log(EventLevel.Error, $"Ibc05 Odd Table not found : {elemNotFound}");
                        }
                    }
                    catch (NoSuchElementException elemNotFound)
                    {
                        NLogger.Log(EventLevel.Error, $"Ibc05 Odd Table not found : {elemNotFound}");
                    }
                }
                catch
                {
                    NLogger.Log(EventLevel.Error, "Ibc05 login not found : ");
                }
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Ibc05 : Fail to login ");
            }

        }

        protected override void removingPopup()
        {
        }
    }
}
