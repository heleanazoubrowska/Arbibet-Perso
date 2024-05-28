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
    public partial class Aa2888 : CasinoAPI
    {
	    public override void Connect(AccountData prm_AccountAPI = null)
	    {
		    if (prm_AccountAPI != null)
		    {
			    base.accountAPI = prm_AccountAPI;
		    }

            isLogin = false;
            NLogger.Log(EventLevel.Info, $"Connect to {prm_AccountAPI.AccountBookmakerName}...");

            try
            {
                chromeOptions = new ChromeOptions();

                if (accountAPI.BookmakerShowPage)
                {
                    chromeOptions.AddArguments("headless");
                }

                chromeDriver = new ChromeDriver(chromeOptions);
                try
                {
                    chromeDriver.Url = accountAPI.AccountBookmakerUrl;
                }
                catch
                {

                    return;
                }

                //bool loginFormFounded = false;
                bool oddTablesFounded = false;
                try
                {
	                Login(30);


                    // Get Odds Data after connected

                    try
                    {
                        chromeDriver.SwitchTo().Frame("frame_main_content");
                        chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
                        chromeDriver.SwitchTo().Frame("frame_sports_main");
                        isLogin = true;
                        try
                        {
                            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("tbl2")));

                            try
                            {
                                //IWebElement gridFutur = chromeDriver.FindElementByXPath("//*[@id='tbl2']/tbody");
                                //oddTablesFounded = true;
                                //string html = gridFutur.GetAttribute("innerHTML");

                                //HtmlDocument htmlDoc = new HtmlDocument();
                                //htmlDoc.LoadHtml(html);

                                //getOdds(aa2888, htmlDoc);
                            }
                            catch (NoSuchElementException elemNotFound)
                            {
                                NLogger.Log(EventLevel.Error, $"Aa2888 Odd Table not found 6 : {elemNotFound}");
                            }
                        }
                        catch (NoSuchElementException elemNotFound)
                        {
                            NLogger.Log(EventLevel.Error, $"Aa2888 Odd Table not found 3 : {elemNotFound}");
                        }
                    }
                    catch
                    {
                        NLogger.Log(EventLevel.Error, "Aa2888 login not found 2");
                    }
                }
                catch (NoSuchElementException elemNotFound)
                {
                    NLogger.Log(EventLevel.Error, $"Aa2888 login not found 1 : {elemNotFound}");
                }
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Aa2888 : Fail to login");
            }
        }

        protected override void removingPopup()
        {
	        throw new NotImplementedException();
        }
    }
}
