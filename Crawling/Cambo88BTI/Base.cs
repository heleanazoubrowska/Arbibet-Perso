using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Cambo88BTI: CasinoAPI
    {
        public Cambo88BTI()
        {
	        prv_NumUselessLeagues = Config.Config.uselessLeague.Count;
        }

        public override void backToFrame()
        {

        }
    }
}
