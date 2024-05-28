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
	public partial class Va2888AFB : CasinoAPI
	{
		protected override BetResult findBet(HtmlDocument htmlDoc, string matchToFind, string hdpToFind, decimal winOddToFind, bool winOddFav, bool hedgeBet)
        {
            string currentRawMatch = "";
            int currentTeamFavPos = 0;
            int currentTeamNoFavPos = 0;
            int iCurrentMatch = -1;
            string matchName = "";
            string teamFav = "";
            string teamNoFav = "";
            string matchDate = "";
            string matchTime = "";
            string currentTeamFav = "";
            string currentTeamNoFav = "";

            List<Odd> listOdds = new List<Odd>();
            bool hasOdd = false;

            List<string> listHdp = new List<string>();

            string league = "";
            string typeLeague = "";
            string currentLeague = "";
            string currentLeagueType = "";
            string currentSensHdp = "";
            bool canUseCurrentLeague = true;
            int line1I = 0;

            BetResult betResult = new BetResult();

            try
            {
                //var watch = System.Diagnostics.Stopwatch.StartNew();
                int nbLine = 0;
                foreach (HtmlNode rowMatch in htmlDoc.DocumentNode.SelectNodes(".//tr"))
                {

                    if (rowMatch.Attributes["class"] == null)
                    {
                        continue;
                    }

                    if (rowMatch.Attributes["class"].Value.Contains("oddsLeague"))
                    {
                        canUseCurrentLeague = true;
                        league = rowMatch.SelectSingleNode(".//td[2]/span[2]").InnerText;
                        league.ToUpper();

                        foreach (string str in Config.Config.uselessLeague)
                        {
                            if (league.Contains(str))
                            {
                                canUseCurrentLeague = false;
                                continue;
                            }
                        }

                        typeLeague = Common.getLeagueType(league);

                    }
                    else if (rowMatch.Attributes["class"].Value.Contains("oddsRow"))
                    {
                        if (!canUseCurrentLeague)
                        {
                            continue;
                        }


                        teamFav = rowMatch.SelectSingleNode(".//td[3]/div[2]/span/span[@class='HomeNamePC']").InnerText;
                        teamNoFav = rowMatch.SelectSingleNode(".//td[3]/div[3]/span/span[@class='AwayNamePC']").InnerText;

                        teamFav = Common.cleanTeamName(teamFav) + typeLeague;
                        teamNoFav = Common.cleanTeamName(teamNoFav) + typeLeague;

                        matchName = teamFav + " - " + teamNoFav;

                        if (currentRawMatch == "")
                        {
                            currentRawMatch = matchName;
                            currentTeamFav = teamFav;
                            currentTeamNoFav = teamNoFav;
                            currentLeague = league;
                            currentLeagueType = typeLeague;
                        }

                        if (currentRawMatch != matchName)
                        {

                            currentRawMatch = matchName;
                            currentTeamFav = teamFav;
                            currentTeamNoFav = teamNoFav;
                            currentLeague = league;
                            hasOdd = false;
                            listOdds = new List<Odd>();
                            listHdp = new List<string>();
                        }

                        if (currentRawMatch != matchToFind)
                        {
                            continue;
                        }

                        // ++++++++++++++++++++
                        // OVER UNDER ODD
                        // ++++++++++++++++++++


                        int iCol;
                        string typeTime = "";

                        for (int i = 0; i < 2; i++)
                        {
                            if (i == 0)
                            {
                                iCol = 6;
                                typeTime = "ft";
                            }
                            else
                            {
                                iCol = 9;
                                typeTime = "fh";
                            }

                            if (rowMatch.SelectSingleNode(".//td[" + iCol + "]/div[1]/span[1]/span[2]") != null)
                            {

                                string overUnder = rowMatch.SelectSingleNode(".//td[" + iCol + "]/div[1]/span[1]/span[2]").InnerText;
                                string strOddsOver = rowMatch.SelectSingleNode(".//td[" + iCol + "]/div[1]/span[2]/span[1]").InnerText;
                                string strOddsUnder = rowMatch.SelectSingleNode(".//td[" + iCol + "]/div[2]/span[2]/span[1]").InnerText;
                                overUnder = overUnder.Replace("/", "-");
                                overUnder = $"{typeTime} o/u {overUnder}";
                                strOddsOver = strOddsOver.Replace("&nbsp;", "");
                                strOddsUnder = strOddsUnder.Replace("&nbsp;", "");
                                hasOdd = true;

                                if (overUnder == hdpToFind)
                                {
                                    if (winOddFav)
                                    {
                                        if (decimal.Parse(strOddsOver, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                        {
                                            string oddPath = "/td[" + (iCol + 1) + "]/div[2]/a[1]";
                                            NLogger.Log(EventLevel.Trace, "Click on odds : 004");
                                            //betResult = placebet(rowMatch.SelectSingleNode(".//td[6]/div[1]/span[2]/span[1]").Attributes["abet"].Value);
                                            betResult = checkBet("", rowMatch.SelectSingleNode(".//td[" + iCol + "]/div[1]/span[2]/span[1]").Attributes["abet"].Value, hdpToFind);
                                            return betResult;
                                        }
                                        else
                                        {
                                            betResult.BetStatus = 1;
                                            betResult.BetErrorStatus = 4;
                                            betResult.BetIsConfirmed = false;
                                            betResult.BetIsPlaced = false;
                                            betResult.BetMessage = "Odd from panel > 1";
                                            return betResult;
                                        }
                                    }
                                    else
                                    {
                                        if (decimal.Parse(strOddsUnder, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                        {
                                            string oddPath = "/td[" + (iCol + 1) + "]/div[2]/a[2]";
                                            NLogger.Log(EventLevel.Trace, "Click on odds : 003");
                                            //betResult = placebet(rowMatch.SelectSingleNode(".//td[6]/div[2]/span[2]/span[1]").Attributes["abet"].Value);
                                            betResult = checkBet("", rowMatch.SelectSingleNode(".//td[" + iCol + "]/div[2]/span[2]/span[1]").Attributes["abet"].Value, hdpToFind);
                                            return betResult;
                                        }
                                        else
                                        {
                                            betResult.BetStatus = 1;
                                            betResult.BetErrorStatus = 4;
                                            betResult.BetIsConfirmed = false;
                                            betResult.BetIsPlaced = false;
                                            betResult.BetMessage = "Odd from panel > 1";
                                            return betResult;
                                        }
                                    }
                                }

                            }
                        }

                        // ++++++++++++++++++++
                        // HANDICAP ODD
                        // ++++++++++++++++++++


                        for (int i = 0; i < 2; i++)
                        {
                            if (i == 0)
                            {
                                iCol = 5;
                                typeTime = "ft";
                            }
                            else
                            {
                                iCol = 8;
                                typeTime = "fh";
                            }

                            string sensHdp = "";
                            //if (rowMatch.SelectSingleNode(".//td[" + colOddPos + "]/span[1]") == null && rowMatch.SelectSingleNode(".//td[" + colOddPos + "]/span[2]") == null)
                            //{
                            //    continue;
                            //}

                            // Get the position of the favorite

                            string handicapType = "";
                            if (handicapType == "")
                            {
                                continue;
                            }

                            if (rowMatch.SelectSingleNode(".//td[" + iCol + "]/div[1]/span[1]/span") != null)
                            {
                                handicapType = rowMatch.SelectSingleNode(".//td[" + iCol + "]/div[1]/span[1]/span").InnerText;
                                sensHdp = "+ ";
                            }
                            else if (rowMatch.SelectSingleNode(".//td[" + iCol + "]/div[2]span[1]/span") != null)
                            {
                                handicapType = rowMatch.SelectSingleNode(".//td[" + iCol + "]/div[2]span[1]/span").InnerText;
                                sensHdp = "- ";
                            }
                            else
                            {
                                continue;
                            }



                            handicapType = handicapType.Replace("/", "-");
                            handicapType = handicapType.Replace("<br>", "");
                            handicapType = handicapType.Replace("&nbsp;", "");
                            handicapType = handicapType.Replace("\n", "");

                            if (handicapType != "0")
                            {
                                handicapType = sensHdp + handicapType;
                            }

                            handicapType = $"{typeTime} {handicapType}";
                            if (rowMatch.SelectSingleNode(".//td[" + (iCol + 1) + "]/div[2]/a[1]") == null)
                            {
                                continue;
                            }

                            if (handicapType == hdpToFind)
                            {
                                string strOddsFav = rowMatch.SelectSingleNode(".//td[" + iCol + "]/div[1]/span[2]/span").InnerText;
                                string strOddsNoFav = rowMatch.SelectSingleNode(".//td[" + iCol + "]/div[2]/span[2]/span").InnerText;

                                strOddsFav = strOddsFav.Replace("&nbsp;", "");
                                strOddsFav = strOddsFav.Replace("\n", "");
                                strOddsNoFav = strOddsNoFav.Replace("&nbsp;", "");
                                strOddsNoFav = strOddsNoFav.Replace("\n", "");

                                hasOdd = true;
                                if (winOddFav)
                                {
                                    if (decimal.Parse(strOddsFav, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                    {
                                        string oddPath = "/td[" + iCol + "]/span";
                                        NLogger.Log(EventLevel.Trace, "Click on odds : 002");
                                        //betResult = placebet(rowMatch.SelectSingleNode(".//td[5]/div[1]/span[2]/span").Attributes["abet"].Value);
                                        betResult = checkBet("", rowMatch.SelectSingleNode(".//td[" + iCol + "]/div[1]/span[2]/span").Attributes["abet"].Value, hdpToFind);
                                        return betResult;
                                    }
                                    else
                                    {
                                        betResult.BetStatus = 1;
                                        betResult.BetErrorStatus = 4;
                                        betResult.BetIsConfirmed = false;
                                        betResult.BetIsPlaced = false;
                                        betResult.BetMessage = "Odd from panel > 1";
                                        return betResult;
                                    }
                                }
                                else
                                {
                                    if (decimal.Parse(strOddsNoFav, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                    {
                                        string oddPath = "/td[" + (iCol + 1) + "]/span";
                                        NLogger.Log(EventLevel.Trace, "Click on odds : 001");
                                        //betResult = placebet(rowMatch.SelectSingleNode(".//td[5]/div[2]/span[2]/span").Attributes["abet"].Value);
                                        betResult = checkBet("", rowMatch.SelectSingleNode(".//td[" + iCol + "]/div[2]/span[2]/span").Attributes["abet"].Value, hdpToFind);
                                        return betResult;
                                    }
                                    else
                                    {
                                        betResult.BetStatus = 1;
                                        betResult.BetErrorStatus = 4;
                                        betResult.BetIsConfirmed = false;
                                        betResult.BetIsPlaced = false;
                                        betResult.BetMessage = "Odd from panel > 1";
                                        return betResult;
                                    }
                                }
                            }
                        }
                    }
                    //NLogger.Log(EventLevel.Debug,JsonConvert.SerializeObject(va2888.BookmakerMatches[iCurrentMatch], Formatting.Indented));
                }
                //watch.Stop();
                //NLogger.Log(EventLevel.Info,"Va28888 update "+ nbLine +" rows in " + watch.ElapsedMilliseconds + " ms");

            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Error update Va28888");
            }

            betResult.BetStatus = 1;
            betResult.BetErrorStatus = 2;
            betResult.BetIsConfirmed = false;
            betResult.BetIsPlaced = false;
            betResult.BetMessage = "Match and odd not founded";
            return betResult;
        }
    }
}
