using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbibetProgram.Functions;

namespace ArbibetProgram.Strategy
{
	public static partial class Football
	{
        public static void CheckForArbitrage()
        {
            // foreach bookmaker
            for (var i = 0; i < MainClass.gs_NumBookmakers; i++)
            {
                // foreach match of current bookmaker
                int prv_NumBookmakerMatches = MainClass.bookmakers[i].BookmakerMatches.Count;
                for (var j = 0; j < prv_NumBookmakerMatches; j++)
                {
                    // foreach other bookmaker after current bookmaker
                    for (var x = i + 1; x < MainClass.gs_NumBookmakers; x++)
                    {
                        // foreach other bookmaker's matches
                        int prv_NumOtherBookmakersMatches = MainClass.bookmakers[x].BookmakerMatches.Count;
                        for (var y = 0; y < prv_NumOtherBookmakersMatches; y++)
                        {
                            //if (bookmakers[i].BookmakerMatches[j].MatchName == bookmakers[x].BookmakerMatches[y].MatchName)
                            if
                            (
                                (
                                    MainClass.bookmakers[i].BookmakerMatches[j].TeamFav.TeamName == MainClass.bookmakers[x].BookmakerMatches[y].TeamFav.TeamName
                                    &&
                                    MainClass.bookmakers[i].BookmakerMatches[j].TeamNoFav.TeamName == MainClass.bookmakers[x].BookmakerMatches[y].TeamNoFav.TeamName
                                )
                            //||
                            //(
                            //    MainClass.bookmakers[i].BookmakerMatches[j].TeamFav.TeamName == MainClass.bookmakers[x].BookmakerMatches[y].TeamNoFav.TeamName
                            //    &&
                            //    MainClass.bookmakers[i].BookmakerMatches[j].TeamNoFav.TeamName == MainClass.bookmakers[x].BookmakerMatches[y].TeamFav.TeamName
                            //)
                            //(MainClass.bookmakers[i].BookmakerMatches[j].MatchName == MainClass.bookmakers[x].BookmakerMatches[y].MatchName
                            //||
                            //MainClass.bookmakers[i].BookmakerMatches[j].MatchName == MainClass.bookmakers[x].BookmakerMatches[y].MatchName2)
                            //&& MainClass.bookmakers[i].BookmakerMatches[j].MatchName != " " && MainClass.bookmakers[x].BookmakerMatches[y].MatchName != " "
                            )
                            {
                                //NLogger.Log(EventLevel.Debug, "+++++++++++++++");
                                //NLogger.Log(EventLevel.Debug, "Match found ! : " + MainClass.bookmakers[i].BookmakerMatches[j].MatchName);
                                MainClass.NbMatchCheck++;

                                try
                                {
                                    Football.checkHandicapMatch(i, x, MainClass.bookmakers[i].BookmakerMatches[j], MainClass.bookmakers[x].BookmakerMatches[y], MainClass.bookmakers[i].BookmakerName, MainClass.bookmakers[x].BookmakerName, i, x, MainClass.bookmakers[i].ActiveAccount.AccountInfo.AccountName, MainClass.bookmakers[x].ActiveAccount.AccountInfo.AccountName);
                                }
                                catch (System.Exception prv_Exception)
                                {
                                    NLogger.Log(EventLevel.Info, "Check Handicap Strategy MAIN ERROR");
                                    NLogger.LogError(prv_Exception);
                                }

                                //NLogger.Log(EventLevel.Debug,"+++++++++++++++");
                            }
                        }
                    }
                }
            }
        }
    }
}
