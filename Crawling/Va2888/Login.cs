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
	public partial class Va2888 : CasinoAPI
	{
		public override void Login(int prm_WaitTime)
		{
			try
			{
				IWebElement form = chromeDriver.FindElementByClassName("form-login");
				//loginFormFounded = true;
				// Connect From Login Page
				WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(prm_WaitTime));
				form.FindElement(By.Id("login_account")).SendKeys(accountAPI.AccountUsername);
				form.FindElement(By.Id("login_password")).SendKeys(accountAPI.AccountPassword);
				form.FindElement(By.Id("remeber_me")).Click();
				Thread.Sleep(1000);
				form.FindElement(By.Name("save")).Click();
			}
			catch (Exception prv_Exception)
			{
				NLogger.Log(EventLevel.Error, $"{base.accountAPI.AccountBookmakerName} failed to log in");
				NLogger.LogError(prv_Exception);
			}
		}
	}
}
