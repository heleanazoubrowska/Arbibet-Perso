using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArbibetProgram.Functions;
using ArbibetProgram.Models;
using OpenQA.Selenium;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
	public partial class Aa2888 : CasinoAPI
	{
		public override void Login(int prm_WaitTime)
		{
			try
			{
				chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(prm_WaitTime);
				IWebElement form = chromeDriver.FindElementById("myForm");

				// Connect From Login Page
				form.FindElement(By.Id("txtAccount")).SendKeys(accountAPI.AccountUsername);
				form.FindElement(By.Id("txtPassword")).SendKeys(accountAPI.AccountPassword);
				form.FindElement(By.Id("chkRememberMe")).Click();
				Thread.Sleep(1000);
				form.FindElement(By.Id("btnLogin")).Click();
			}
			catch (Exception prv_Exception)
			{
				NLogger.Log(EventLevel.Error, $"{base.accountAPI.AccountBookmakerName} failed to log in");
				NLogger.LogError(prv_Exception);
			}
        }
	}
}
