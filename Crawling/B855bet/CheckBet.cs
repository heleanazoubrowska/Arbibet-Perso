using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using HtmlAgilityPack;
using System;
using System.Threading;
using System.Globalization;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class B855bet : CasinoAPI
    {
        //protected override BetResult checkBet(string oddPath, string idLine, string hdpToFind)
        protected override BetResult checkBet(string oddPath, string line1I, string hdpToFind, CheckBet_ExtraParams prm_ExtraParams = null)
        {
            bool betPlaced = false;

            BetResult betResult = new BetResult();
            string betMessage = "";
            decimal maxBet = 0;
            string hdpSens = "";

            string teamFav = "";
            string teamNoFav = "";
            try
            {
                NLogger.Log(EventLevel.Info, "Step 1");
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                var watch = System.Diagnostics.Stopwatch.StartNew();
                NLogger.Log(EventLevel.Info, "Step 1");
                // +++++++++++++++++++++++
                // Click The Odd
                // +++++++++++++++++++++++

                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                IWebElement line;
                try
                {
                    line = chromeDriver.FindElementByCssSelector($"tr[id='{line1I}']");
                    try
                    {
                        NLogger.Log(EventLevel.Debug, $"oddPath : {oddPath}");
                        IWebElement oddToClick = line.FindElement(By.XPath($"./{oddPath}"));

                        //line.FindElement(By.XPath("./" + oddPath)).Click();

                        IJavaScriptExecutor ex = (IJavaScriptExecutor)chromeDriver;
                        ex.ExecuteScript("arguments[0].click();", oddToClick);
                        focusOnBet = true;
                    }
                    catch (NoSuchElementException e)
                    {
                        NLogger.Log(EventLevel.Error, $"Error - Click not found : {e}");
                        betResult.BetStatus = 2;
                        betResult.BetErrorStatus = 8;
                        betResult.BetIsConfirmed = false;
                        betResult.BetIsPlaced = false;
                        betResult.BetMessage = "Click not found";
                        return betResult;
                    }
                }
                catch (NoSuchElementException e)
                {
                    NLogger.Log(EventLevel.Error, $"Error - line not found : {e}");
                    betResult.BetStatus = 2;
                    betResult.BetErrorStatus = 8;
                    betResult.BetIsConfirmed = false;
                    betResult.BetIsPlaced = false;
                    betResult.BetMessage = "Line click not founded";
                    return betResult;

                }


                NLogger.Log(EventLevel.Info, "Step 3");
                Thread.Sleep(1000);
                // Check if popup shows up
                try
                {
                    IAlert alertOdd = chromeDriver.SwitchTo().Alert();
                    alertOdd.Accept();
                }
                catch (NoAlertPresentException Ex)
                {

                }
                NLogger.Log(EventLevel.Info, "Step 4");
                // +++++++++++++++++++++++
                // Input Amount
                // +++++++++++++++++++++++
                watch = System.Diagnostics.Stopwatch.StartNew();

                try
                {
                    chromeDriver.SwitchTo().DefaultContent();
                }
                catch (NoSuchFrameException e)
                {
                    NLogger.Log(EventLevel.Error, $"Error - cannot switch to default content : {e}");
                    betResult.BetStatus = 2;
                    betResult.BetIsConfirmed = false;
                    betResult.BetIsPlaced = false;
                    betResult.BetMessage = "cannot switch to default content";
                    return betResult;
                }
                NLogger.Log(EventLevel.Info, "Step 5");


                Thread.Sleep(2000);

                // Check if popup shows up
                try
                {
                    IAlert alert = chromeDriver.SwitchTo().Alert();
                    alert.Accept();
                }
                catch (NoAlertPresentException Ex)
                {

                }

                watch.Stop();
                NLogger.Log(EventLevel.Info, $"Time step 2 {watch.ElapsedMilliseconds} ms");

                watch = System.Diagnostics.Stopwatch.StartNew();

                try
                {
                    chromeDriver.SwitchTo().Frame("leftFrame");

                }

                catch (NoSuchFrameException e)
                {
                    NLogger.Log(EventLevel.Error, $"Error - cannot switch to leftFrame : {e}");
                    betResult.BetStatus = 2;
                    betResult.BetIsConfirmed = false;
                    betResult.BetIsPlaced = false;
                    betResult.BetMessage = "Error - cannot switch to leftFrame";
                    return betResult;
                }


                watch.Stop();
                NLogger.Log(EventLevel.Info, $"Time step 2.1 {watch.ElapsedMilliseconds} ms");


                // +++++++++++++++++++++++
                // Parse bet panel HTML
                // +++++++++++++++++++++++

                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                string bet = "";
                try
                {
                    wait.Until(ExpectedConditions.ElementExists(By.Id("bet_div")));
                    bet = chromeDriver.FindElementById("bet_div").GetAttribute("innerHTML");
                }
                catch
                {
                    NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot find bet_div 855BET ");
                }

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(bet);


                watch.Stop();
                NLogger.Log(EventLevel.Info, $"Time step 4 {watch.ElapsedMilliseconds} ms");

                // +++++++++++++++++++++++
                // Control the odd is > 1
                // +++++++++++++++++++++++
                string oddToBet = "";

                try
                {
                    oddToBet = htmlDoc.DocumentNode.SelectSingleNode("//span[@id='bodds']").InnerText;
                }
                catch
                {
                    NLogger.Log(EventLevel.Error, "Error - bodds not found");
                    betResult.BetStatus = 2;
                    betResult.BetErrorStatus = 5;
                    betResult.BetIsConfirmed = false;
                    betResult.BetIsPlaced = false;
                    betResult.BetMessage = "Error - bodds not found";
                    return betResult;
                }


                oddToBet = oddToBet.Replace("&nbsp;", "");
                decimal oddDecimal = decimal.Parse(oddToBet, CultureInfo.InvariantCulture.NumberFormat);
                if (oddDecimal < MainClass.minOddRequired)
                {
                    betResult.BetStatus = 2;
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

                    oddType = htmlDoc.DocumentNode.SelectSingleNode("//span[@id='sbBetHdp']").InnerText;
                }
                catch (ElementNotVisibleException e)
                {

                }

                if (Common.isOverUnder(hdpToFind))
                {

                }
                else
                {
                    if (htmlDoc.DocumentNode.SelectSingleNode("//span[@id='sbBetHdp']").Attributes["class"] != null)
                    {
                        if (htmlDoc.DocumentNode.SelectSingleNode("//span[@id='sbBetHdp']").Attributes["class"].Value == "NegativeOddsClass")
                        {
                            hdpSens = "-";
                        }
                        else
                        {
                            hdpSens = "+";
                        }
                    }
                    else
                    {
                        hdpSens = "+";
                    }
                }

                oddType = Common.getQuarterBet(oddType);
                oddType = Common.cleanOddPlaceBet(oddType);
                string hdpToFindClean = Common.cleanOddPlaceBet(hdpToFind);
                NLogger.Log(EventLevel.Debug, $"Odd converted : {oddType}");

                if (oddType == "0")
                {
                    hdpSens = "0";
                    NLogger.Log(EventLevel.Debug, $"hdpSens = 0");
                }

                if (oddType != hdpToFindClean)
                {
                    NLogger.Log(EventLevel.Error,  $"Error - odd type 855bet is different : oddType : {oddType} - hdpToFind : {hdpToFind}");
                    //chromeDriver.FindElement(By.XPath("//*[@id='divBetZone']/table/tbody/tr[12]/td/button[1]")).Click();
                    betResult.BetStatus = 2;
                    betResult.BetErrorStatus = 3;
                    betResult.BetIsConfirmed = false;
                    betResult.BetIsPlaced = false;
                    betResult.BetMessage = "odd type 855bet is different";
                    return betResult;
                }

                // +++++++++++++++++++++++
                // Get Max Bet
                // +++++++++++++++++++++++
                string maxBetStr = "";

                try
                {
                    maxBetStr = htmlDoc.DocumentNode.SelectSingleNode("//span[@id='spMaxBetValue']").InnerText;
                }
                catch
                {
                    NLogger.Log(EventLevel.Error, "Max bet not founded");
                    betResult.BetStatus = 2;
                    betResult.BetErrorStatus = 9;
                    betResult.BetIsConfirmed = false;
                    betResult.BetIsPlaced = false;
                    betResult.BetMessage = "Error - Max bet not founded";
                    return betResult;
                }


                maxBetStr = maxBetStr.Replace("&nbsp;", "");
                maxBetStr = maxBetStr.Replace(",", "");
                maxBet = (decimal.Parse(maxBetStr, CultureInfo.InvariantCulture.NumberFormat));
                NLogger.Log(EventLevel.Debug, $"Max bet : {maxBet}");


                // +++++++++++++++++++++++
                // Get Team Name
                // +++++++++++++++++++++++


                try
                {
                    teamFav = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='spHome']").InnerText;
                    teamNoFav = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='spAway']").InnerText;
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


                // Check if popup shows up
                try
                {
                    IAlert alert = chromeDriver.SwitchTo().Alert();
                    alert.Accept();
                }
                catch (NoAlertPresentException Ex)
                {

                }


                Thread.Sleep(1000);

                betPlaced = true;
                betMessage = "Bet is placed";
            }
            catch (NoSuchElementException e)
            {
                betPlaced = false;
                betMessage = "Error placing bet";
            }

            betResult.BetStatus = 0;
            betResult.BetIsConfirmed = false;
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
