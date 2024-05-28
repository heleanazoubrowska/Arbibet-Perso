using System;
using System.Collections.Generic;
namespace ArbibetProgram.Config
{
    public class Config
    {

        public static List<string> uselessLeague = new List<string>()
        {
            //"CORNERS",
            "BOOKING",
            "SPECIFIC",
            "MINS",
            "GOAL",
            //"WHICH",
            "TOTAL",
            "HOME",
            "AWAY",
            "SPECIALS",
            "WINNER",
            " VAR",
            "EXTRA",
            //"RESERVE"
        };

        private static int prv_NumUselessLeagues = uselessLeague.Count;

        public static string telegrameToken = "1373599129:AAGsabs850V0vAJNR-oI__ucNDTonzZcY5M";
        public static string chatIdJo = "1375562809";
        public static string chatIdGroup = "-413785037";
        public static string chatIdKenGroup = "-1001270163352";
        
        public static bool CheckForUselessLeague(string prm_LeagueName)
        {
            bool prv_CanUseLeague = true;

            for (int UselessLeagueCounter = 0; UselessLeagueCounter < prv_NumUselessLeagues; UselessLeagueCounter++)
            {
                if (prm_LeagueName.Contains(uselessLeague[UselessLeagueCounter]))
                {
                    //Console.WriteLine(prm_LeagueName);
                    //NLogger.Log(EventLevel.Debug,currentLeague);
                    prv_CanUseLeague = false;
                    break;
                }
            }

            return prv_CanUseLeague;
		}

        public Config()
        {

        }
    }
}
