using System;
using System.Globalization;
using System.Linq;
using System.Threading;

using HtmlAgilityPack;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Cambo88BTI : CasinoAPI
    {

        protected override BetResult findBet(HtmlDocument htmlDoc, string matchToFind, string hdpToFind, decimal winOddToFind, bool winOddFav, bool hedgeBet)
        {
            NLogger.Log(EventLevel.Critical, "Cambobti find bet");
            string teamFav = "";
            string teamNoFav = "";
            string matchName = "";
            string league = "";
            bool canUseCurrentLeague = true;
            string currentLeague = "";
            string typeLeague = "";
            BetResult betResult = new BetResult();

            int line0I = 0;

            var prv_EventLists = htmlDoc.DocumentNode.SelectNodes("./div[@class='rj-asian-events__events-list']");
            int prv_NumEventLists = prv_EventLists.Count;

            //foreach (HtmlNode rowEventList in htmlDoc.DocumentNode.SelectNodes("./div[@class='rj-asian-events__events-list']"))
            for (int Loopy = 0; Loopy < prv_NumEventLists; Loopy++)
            {
	            var rowEventList = prv_EventLists[Loopy];

                line0I++;
                try
                {
                    int line1I = 0;

                    var prv_RowLeagues = rowEventList.SelectNodes("./div[@class='rj-asian-events__events-block']/div[3]/div[@class='rj-asian-events__single-league']");
                    int prv_NumRowLeagues = prv_RowLeagues.Count;

                    //foreach (HtmlNode rowLeague in rowEventList.SelectNodes("./div[@class='rj-asian-events__events-block']/div[3]/div[@class='rj-asian-events__single-league']"))
                    for (int LeagueCounter = 0; LeagueCounter < prv_NumRowLeagues; LeagueCounter++)
                    {
	                    var rowLeague = prv_RowLeagues[LeagueCounter];
                        line1I++;

                        canUseCurrentLeague = true;
                        league = rowLeague.SelectSingleNode(".//div[1]/h4").InnerText;
                        league = currentLeague.ToUpper();

                        //foreach (string str in Config.Config.uselessLeague)
                        for (int UselessLeagueCounter = 0; UselessLeagueCounter < prv_NumUselessLeagues; UselessLeagueCounter++)
                        {
	                        if (league.Contains(Config.Config.uselessLeague[UselessLeagueCounter]))
	                        {
		                        //NLogger.Log(EventLevel.Debug,currentLeague);
		                        canUseCurrentLeague = false;
		                        break;
	                        }
                        }

                        typeLeague = Common.getLeagueType(league);

                        try
                        {
                            int line2I = 0;

                            var prv_RowLists = rowLeague.SelectNodes("./div[2]/div[@class='rj-asian-events__single-event']");
                            int prv_NumRowLists = prv_RowLists.Count;

                            //foreach (HtmlNode rowList in rowLeague.SelectNodes("./div[2]/div[@class='rj-asian-events__single-event']"))
                            for (int RowListCounter = 0; RowListCounter < prv_NumRowLists; RowListCounter++)
                            {
	                            var rowList = prv_RowLists[RowListCounter];
                                line2I++;
                                //NLogger.Log(EventLevel.Debug,"++++++++++++++++");
                                //NLogger.Log(EventLevel.Debug,rowList.InnerText);
                                //NLogger.Log(EventLevel.Debug,"++++++++++++++++");
                                int line3I = 0;

                                var prv_RowMatches = rowList.SelectNodes(".//div[@class='rj-asian-events__row']");
                                int prv_NumRowMatches = prv_RowMatches.Count;

                                //foreach (HtmlNode rowMatch in rowList.SelectNodes(".//div[@class='rj-asian-events__row']"))
                                for (int RowMatchCounter = 0; RowMatchCounter < prv_NumRowMatches; RowMatchCounter++)
                                {
	                                var rowMatch = prv_RowMatches[RowMatchCounter];

                                    line3I++;
                                    //NLogger.Log(EventLevel.Debug,"CASE 1");

                                    CheckBet_ExtraParams prv_ExtraParams = new CheckBet_ExtraParams()
                                    {
	                                    line0I = line0I.ToString(),
	                                    line2I = line2I.ToString(),
	                                    line3I = line3I.ToString()
                                    };

                                    //NLogger.Log(EventLevel.Debug,rowMatch.InnerHtml);
                                    teamFav = rowMatch.SelectSingleNode(".//div[2]/div/div[1]").InnerText;
                                    teamNoFav = rowMatch.SelectSingleNode(".//div[2]/div/div[2]").InnerText;

                                    teamFav = Common.cleanTeamName(teamFav) + typeLeague;
                                    teamNoFav = Common.cleanTeamName(teamNoFav) + typeLeague;

                                    matchName = $"{teamFav} - {teamNoFav}";

                                    if (matchName == matchToFind)
                                    {
                                        NLogger.Log(EventLevel.Critical, "match is find");




                                        // ++++++++++++++++++++
                                        // OVER UNDER ODD FT & FH
                                        // ++++++++++++++++++++


                                        int iCol = 0;
                                        string typeTime = "";

                                        for (int i = 0; i < 2; i++)
                                        {
                                            if (i == 0)
                                            {
                                                iCol = 3;
                                                typeTime = "ft";
                                            }
                                            else
                                            {
                                                iCol = 4;
                                                typeTime = "fh";
                                            }


                                            if (rowMatch.SelectSingleNode($".//div[{iCol}]/div/div[2]/div[1]/div[1]") != null)
                                            {
                                                string overUnder = rowMatch.SelectSingleNode($".//div[{iCol}]/div/div[2]/div[1]/div[1]").InnerText;
                                                string strOddsOver = rowMatch.SelectSingleNode($".//div[{iCol}]/div/div[2]/div[1]/div[2]").InnerText;
                                                string strOddsUnder = rowMatch.SelectSingleNode($".//div[{iCol}]/div/div[2]/div[2]/div[2]").InnerText;
                                                overUnder = overUnder.Replace("/", "-");
                                                overUnder = $"{typeTime} o/u {overUnder}";
                                                strOddsOver = strOddsOver.Replace("&nbsp;", "");
                                                strOddsUnder = strOddsUnder.Replace("&nbsp;", "");
                                                if (overUnder == hdpToFind)
                                                {
                                                    if (winOddFav)
                                                    {
                                                        if (decimal.Parse(strOddsOver, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                                        {
                                                            string oddPath = $"/div[{iCol}]/div/div[2]/div[1]/div[2]";
                                                            NLogger.Log(EventLevel.Trace, $"Click on odds : 004 :{typeTime}");

                                                            //betResult = checkBet(oddPath, line0I, line1I, line2I, line3I, hdpToFind);
                                                            betResult = checkBet(oddPath, line1I.ToString(), hdpToFind, prv_ExtraParams);
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
                                                            string oddPath = $"/div[{iCol}]/div/div[2]/div[2]/div[2]";
                                                            NLogger.Log(EventLevel.Trace, $"Click on odds : 003 {typeTime}");
                                                            //betResult = checkBet(oddPath, line0I, line1I, line2I, line3I, hdpToFind);
                                                            betResult = checkBet(oddPath, line1I.ToString(), hdpToFind, prv_ExtraParams);
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
                                        // HANDICAP ODD FT & FH
                                        // ++++++++++++++++++++



                                        for (int i = 0; i < 2; i++)
                                        {
                                            if (i == 0)
                                            {
                                                iCol = 3;
                                                typeTime = "ft";
                                            }
                                            else
                                            {
                                                iCol = 4;
                                                typeTime = "fh";
                                            }

                                            string sensHdp = "";
                                            int posHdp;

                                            if (rowMatch.SelectSingleNode($".//div[{iCol}]/div/div[1]/div[1]/div[1]") == null)
                                            {
                                                //NLogger.Log(EventLevel.Debug,"Case 3.1");
                                                continue;
                                            }
                                            //NLogger.Log(EventLevel.Debug,"CASE 3111");
                                            // Get the position of the favorite
                                            if (rowMatch.SelectSingleNode($".//div[{iCol}]/div/div[1]/div[1]/div[1]").InnerText == "" || rowMatch.SelectSingleNode(".//div[3]/div/div[1]/div[1]/div[1]").InnerText == "&nbsp;")
                                            {
                                                //NLogger.Log(EventLevel.Debug,"Case 3.2");
                                                sensHdp = "- ";
                                                posHdp = 2;
                                            }
                                            else
                                            {
                                                //NLogger.Log(EventLevel.Debug,"Case 3.3");
                                                sensHdp = "+ ";
                                                posHdp = 1;
                                            }


                                            //NLogger.Log(EventLevel.Debug,"CASE 3222");
                                            string handicapType = rowMatch.SelectSingleNode($".//div[{iCol}]/div/div[1]/div[{posHdp}]/div[1]").InnerText;
                                            //NLogger.Log(EventLevel.Debug,"Case 4");
                                            if (handicapType == "")
                                            {
                                                //NLogger.Log(EventLevel.Debug,"Case 4.1");
                                                continue;
                                            }
                                            handicapType = handicapType.Replace("/", "-");

                                            if (handicapType != "0")
                                            {
                                                handicapType = sensHdp + handicapType;
                                            }

                                            handicapType = $"{typeTime} {handicapType}";

                                            if (handicapType == hdpToFind)
                                            {

                                                string strOddsFav = rowMatch.SelectSingleNode($".//div[{iCol}]/div/div[1]/div[1]/div[2]").InnerText;
                                                string strOddsNoFav = rowMatch.SelectSingleNode($".//div[{iCol}]/div/div[1]/div[2]/div[2]").InnerText;

                                                if (winOddFav)
                                                {
                                                    if (decimal.Parse(strOddsFav, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                                    {
                                                        string oddPath = $"/div[{iCol}]/div/div[1]/div[1]/div[2]";
                                                        NLogger.Log(EventLevel.Trace, $"Click on odds : 002 {typeTime}");
                                                        //betResult = checkBet(oddPath, line0I, line1I, line2I, line3I, hdpToFind);
                                                        betResult = checkBet(oddPath, line1I.ToString(), hdpToFind, prv_ExtraParams);
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
                                                        string oddPath = $"/div[{iCol}]/div/div[1]/div[2]/div[2]";
                                                        NLogger.Log(EventLevel.Trace, $"Click on odds : 001 {typeTime}");
                                                        //betResult = checkBet(oddPath, line0I, line1I, line2I, line3I, hdpToFind);
                                                        betResult = checkBet(oddPath, line1I.ToString(), hdpToFind, prv_ExtraParams);
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
                                }
                            }

                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {

                }
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
