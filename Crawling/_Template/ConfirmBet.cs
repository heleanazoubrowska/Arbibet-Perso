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
	public partial class NewCasino : CasinoAPI
	{
        public override BetResult confirmBet()
        {
            BetResult betResult = new BetResult();

            return betResult;
        }

    }
}
