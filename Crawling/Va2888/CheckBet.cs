using System;
using HtmlAgilityPack;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;

using System.Globalization;
using System.Linq;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using System.Threading;
using Newtonsoft.Json;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
	public partial class Va2888 : CasinoAPI
	{
        // private BetResult placebet(string oddPath,  string idLine)
        protected override BetResult checkBet(string oddPath, string line1I, string hdpToFind, CheckBet_ExtraParams prm_ExtraParams = null)
        {
            bool betPlaced = false;
            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            var watch = System.Diagnostics.Stopwatch.StartNew();
            BetResult betResult = new BetResult();
            string betMessage = "";
            decimal maxBet = 0;
            string hdpSens = "";

            string teamFav = "";
            string teamNoFav = "";
            try
            {
                // +++++++++++++++++++++++
                // Click The Odd
                // +++++++++++++++++++++++

                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                IWebElement line;
                try
                {
                    line = chromeDriver.FindElementByCssSelector($"tr[oddsid='{line1I}']");
                }
                catch (NoSuchElementException e)
                {
                    NLogger.Log(EventLevel.Error, $"Error - line find odd : {e}");
                    betResult.BetStatus = 2;
                    betResult.BetErrorStatus = 8;
                    betResult.BetIsConfirmed = false;
                    betResult.BetIsPlaced = false;
                    betResult.BetMessage = "Line odd not found";
                    return betResult;
                }

                watch = System.Diagnostics.Stopwatch.StartNew();

                try
                {
                    line.FindElement(By.XPath($"./{oddPath}")).Click();

                }
                catch
                {
                    NLogger.Log(EventLevel.Error, "Error - click find odd : ");
                    //chromeDriver.FindElement(By.XPath("//*[@id='tbBetBox']/tbody/tr[2]/td/table/tbody/tr[8]/td/span[1]")).Click();
                    //chromeDriver.SwitchTo().ParentFrame();
                    //chromeDriver.SwitchTo().Frame("fraMain");
                    betResult.BetStatus = 2;
                    betResult.BetErrorStatus = 8;
                    betResult.BetIsConfirmed = false;
                    betResult.BetIsPlaced = false;
                    betResult.BetMessage = "Click not found";
                    return betResult;
                }



                watch.Stop();
                NLogger.Log(EventLevel.Info, $"Time step 1 {watch.ElapsedMilliseconds} ms");

                try
                {
                    chromeDriver.SwitchTo().DefaultContent();
                    focusOnBet = true;
                }
                catch (NoSuchFrameException e)
                {
                    NLogger.Log(EventLevel.Error, $"Error - DefaultContent cannot switch : {e}");
                }

                try
                {
                    chromeDriver.SwitchTo().Frame(chromeDriver.FindElement(By.XPath("//html/body/div[3]/div/iframe")));
                }
                catch (NoSuchFrameException e)
                {
                    NLogger.Log(EventLevel.Error, $"Error - Second frame cannot switch : {e}");
                }

                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(5));

                try
                {
                    chromeDriver.SwitchTo().Frame(chromeDriver.FindElement(By.XPath("//html/body/div/div[2]/iframe")));
                }
                catch (NoSuchFrameException e)
                {
                    NLogger.Log(EventLevel.Error, $"Error - Third frame cannot switch : {e}");
                }


                // +++++++++++++++++++++++
                // Control the odd is > 1
                // +++++++++++++++++++++++
                Thread.Sleep(1000);


                string oddToBet = chromeDriver.FindElementById("socBetOdds").Text;

                oddToBet = oddToBet.Replace("&nbsp;", "");
                oddToBet = oddToBet.Replace("@", "");
                decimal oddDecimal = decimal.Parse(oddToBet, CultureInfo.InvariantCulture.NumberFormat);
                NLogger.Log(EventLevel.Debug, $"odd to bet : {oddDecimal}");
                if (oddDecimal < MainClass.minOddRequired)
                {
                    NLogger.Log(EventLevel.Error, "Odd is under than 1, cannot place the bet");
                    betResult.BetStatus = 2;
                    betResult.BetErrorStatus = 5;
                    betResult.BetIsConfirmed = false;
                    betResult.BetIsPlaced = false;
                    betResult.BetMessage = "Odd is under than 1, cannot place the bet";
                    return betResult;
                }


                // +++++++++++++++++++++++
                // Control Type Odd is Correct
                // +++++++++++++++++++++++

                string oddType = "";

                try
                {

                    wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='socBetHdp']/span")));
                    oddType = chromeDriver.FindElementByXPath("//*[@id='socBetHdp']/span").Text;

                }
                catch (ElementNotVisibleException e)
                {

                }

                if (Common.isOverUnder(hdpToFind))
                {

                }
                else
                {
                    if (oddType.Substring(0, 1) == "-")
                    {
                        hdpSens = "-";
                    }
                    else
                    {
                        hdpSens = "+";
                    }
                }

                oddType = Common.getQuarterBet(oddType);
                oddType = Common.getQuarterBet(oddType);
                oddType = Common.cleanOddPlaceBet(oddType);
                string hdpToFindClean = Common.cleanOddPlaceBet(hdpToFind);

                if (oddType == "0")
                {
                    hdpSens = "0";
                    NLogger.Log(EventLevel.Debug, $"hdpSens = 0");
                }

                if (oddType != hdpToFindClean)
                {
                    NLogger.Log(EventLevel.Error, $"Error - odd type Nba is different : oddType : {oddType} - hdpToFind : {hdpToFind}");
                    //chromeDriver.FindElement(By.XPath("//*[@id='divBetZone']/table/tbody/tr[12]/td/button[1]")).Click();
                    betResult.BetStatus = 2;
                    betResult.BetErrorStatus = 3;
                    betResult.BetIsConfirmed = false;
                    betResult.BetIsPlaced = false;
                    betResult.BetMessage = "odd type Nba is different";
                    return betResult;
                }

                // +++++++++++++++++++++++
                // Get Max Bet
                // +++++++++++++++++++++++

                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("betMaxLimit")));
                string maxBetStr = chromeDriver.FindElementById("betMaxLimit").Text;
                maxBetStr = maxBetStr.Replace("&nbsp;", "");
                maxBetStr = maxBetStr.Replace(" ", "");
                maxBet = (decimal.Parse(maxBetStr, CultureInfo.InvariantCulture.NumberFormat));
                NLogger.Log(EventLevel.Debug, $"Max bet : {maxBet}");


                // +++++++++++++++++++++++
                // Get Team Name
                // +++++++++++++++++++++++

                try
                {
                    teamFav = chromeDriver.FindElementById("socHome").Text;
                    teamNoFav = chromeDriver.FindElementById("socAway").Text;
                }
                catch
                {
                    NLogger.Log(EventLevel.Error, "Team name bet not founded");
                    betResult.BetStatus = 2;
                    betResult.BetErrorStatus = 9;
                    betResult.BetIsConfirmed = false;
                    betResult.BetIsPlaced = false;
                    betResult.BetMessage = "Error - Team name not founded";
                    return betResult;
                }

                betMessage = "Bet is placed";
                betPlaced = true;

                Thread.Sleep(1000);
                betMessage = "Bet is Placed";
                betPlaced = true;
            }
            catch (NoSuchElementException e)
            {
                NLogger.Log(EventLevel.Error, $"Error while placing bet on Va2888 : {e}");

                betPlaced = false;
                betMessage = "Error while placing bet on Va2888";
            }

            betResult.BetStatus = 0;
            betResult.BetIsConfirmed = true;
            betResult.BetIsPlaced = betPlaced;
            betResult.BetMessage = betMessage;
            betResult.BetMaxBet = maxBet;
            betResult.BetHdpSens = hdpSens;
            betResult.BetTeamFav = teamFav;
            betResult.BetTeamNoFav = teamNoFav;
            return betResult;
        }

    }
}
