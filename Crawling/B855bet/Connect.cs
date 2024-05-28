using System;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using System.IO;
using System.Drawing;
using System.Linq;
//using Tesseract;
using System.Drawing.Imaging;
using Syncfusion.OCRProcessor;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class B855bet : CasinoAPI
    {
	    public override void Connect(AccountData prm_AccountAPI = null)
	    {
		    if (prm_AccountAPI != null)
		    {
			    base.accountAPI = prm_AccountAPI;
		    }

		    NLogger.Log(EventLevel.Info, $"Connect to {prm_AccountAPI.AccountBookmakerName}...");

            //try
            //{
            chromeOptions = new ChromeOptions();

                if (accountAPI.BookmakerShowPage)
                {
                    chromeOptions.AddArguments("headless");
                }

                chromeDriver = new ChromeDriver(chromeOptions);
                chromeDriver.Url = accountAPI.AccountBookmakerUrl;

                Login(60);

                //wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(30));
                //wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.Id("mainFrame")));
                chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
                //chromeDriver.SwitchTo().Frame("frameset1");

                chromeDriver.SwitchTo().Frame("mainFrame");
            //}
            //catch
            //{
            //    NLogger.Log(EventLevel.Error,"855bet : Fail to login");
            //}
        }

        protected override void removingPopup()
        {
        }
    }
}
