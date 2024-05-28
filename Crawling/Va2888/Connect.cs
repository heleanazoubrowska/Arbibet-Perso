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
	public partial class Va2888 : CasinoAPI
	{
        // mainIframe2 = left menu
        // mainIframe3 = odd Iframe

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
	                Login(10);


                    // Get Odds Data after connected
                    try
                    {
                        chromeDriver.SwitchTo().Frame(chromeDriver.FindElement(By.XPath("//html/body/div[3]/div/iframe")));
                        chromeDriver.SwitchTo().Frame("mainIframe2");


                        var wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(30));
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='SoccerMenu']/div[2]/a")));
                        chromeDriver.FindElementByXPath("//*[@id='SoccerMenu']/div[2]/a").Click();

                        Thread.Sleep(2000);

                        chromeDriver.SwitchTo().ParentFrame();
                        chromeDriver.SwitchTo().Frame("mainIframe3");
                        wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(30));
                        chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                        try
                        {
                            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(30));
                            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("tableTodayN")));
                            IWebElement tableTodayN = chromeDriver.FindElementById("tableTodayN");
                            //oddTablesFounded = true;
                            try
                            {
                                //string html = tableTodayN.GetAttribute("innerHTML");

                                //HtmlDocument htmlDoc = new HtmlDocument();
                                //htmlDoc.LoadHtml(html);
                                //getOdds(va2888, htmlDoc);
                            }

                            catch
                            {
                                NLogger.Log(EventLevel.Error, "Va2888 odds table not found : 003 ");
                            }

                        }
                        catch (NoSuchElementException elemNotFound)
                        {
                            NLogger.Log(EventLevel.Error, $"Va2888 odds table not found : 002 {elemNotFound}");
                        }
                    }
                    catch
                    {
                        NLogger.Log(EventLevel.Error, "Va2888 probably in maintenance");
                    }
                }
                catch (NoSuchElementException elemNotFound)
                {
                    NLogger.Log(EventLevel.Error, $"Va2888 login not found : 001 {elemNotFound}");
                }
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Va2888 : Fail to connect ");
            }
        }

		protected override void removingPopup()
		{
		}

	}
}
