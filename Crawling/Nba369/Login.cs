using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArbibetProgram.Functions;
using ArbibetProgram.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
	public partial class Nba369 : CasinoAPI
	{
		public override void Login(int prm_WaitTime)
		{
			try
			{
				chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(60);

				NLogger.Log(EventLevel.Debug, "H 1");

				WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(60));
				NLogger.Log(EventLevel.Debug, "H 2");
				// Connect From Login Page
				chromeDriver.FindElement(By.Id("txtAccount")).SendKeys(accountAPI.AccountUsername);
				chromeDriver.FindElement(By.Id("txtPassword")).SendKeys(accountAPI.AccountPassword);
				chromeDriver.FindElement(By.Id("chkRememberMe")).Click();
				NLogger.Log(EventLevel.Debug, "H 3");
				chromeDriver.FindElement(By.XPath("//html/body/div/section/div/div/div[1]/div/form/div[4]/div/label/input")).Click();
				NLogger.Log(EventLevel.Debug, "H 4");
				//Thread.Sleep(1000);

				IJavaScriptExecutor ex = (IJavaScriptExecutor)chromeDriver;
				ex.ExecuteScript("return document.getElementsByClassName('five_img')[0].remove();");

				chromeDriver.FindElement(By.Id("btnLogin")).Click();
            }
			catch (Exception prv_Exception)
			{
				NLogger.Log(EventLevel.Error, $"{base.accountAPI.AccountBookmakerName} failed to log in");
				NLogger.LogError(prv_Exception);
			}
		}
	}
}
