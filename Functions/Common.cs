using System;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using ArbibetProgram.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using OpenQA.Selenium;

namespace ArbibetProgram.Functions
{
    public static class Common
    {
        // -----------------------------------
        // create pre-compiled regex for speed
        // -----------------------------------
        static Regex prv_RegEx_WhiteSpace = new Regex(@"\s+", RegexOptions.Compiled); // \s = match any whitespace character (equal to [\r\n\t\f\v ]); + = as many times as possible
        static Regex prv_RegEx_EndTabsSpace = new Regex(@"[ \t]+$", RegexOptions.Compiled);
        static Regex prv_RegEx_StartWhiteSpace = new Regex(@"^\s", RegexOptions.Compiled);

        public static string cleanTeamName(string prm_CasinoTeamName)
        {
	        // sn: chain is faster than series
	        string newTeam = prm_CasinoTeamName
			    .ToUpper()
                .Replace("&NBSP;", string.Empty)
		        .Replace("&AMP;", string.Empty)

		        .Replace("-NO.OF CORNERS;", " CORNERS")
		        .Replace("NO.OF CORNERS", " CORNERS")
		        .Replace("-NO.OF Cns.", " CORNERS")
		        .Replace("NO. OF CORNERS", " CORNERS")
		        .Replace(" - NO. OF CORNERS", " CORNERS")

                .Replace("(", "[")
		        .Replace(")", "]")
		        .Replace("{", "[")
		        .Replace("}", "]")
		        .Replace("-", " ")

                // italy
			    .Replace(" AC", string.Empty)
			    .Replace("AC ", string.Empty)
			    .Replace(" AS", string.Empty)
			    .Replace("AS ", string.Empty)

                // football club
                .Replace(" FC", string.Empty)
                .Replace("FC ", string.Empty)

                // brazil football club
                .Replace(" EC", string.Empty)
		        .Replace("EC ", string.Empty)

                // BRAZIL SERIE B
		        .Replace(" SP", string.Empty)
		        .Replace("SP ", string.Empty)
			    .Replace(" BA", string.Empty)
			    .Replace("BA ", string.Empty)
                .Replace(" MG", string.Empty)
			    .Replace("MG ", string.Empty)
                .Replace("CLUBE ", string.Empty)
			    .Replace(" CLUBE", string.Empty)
			    .Replace("ESPORTE ", string.Empty)
			    .Replace(" ESPORTE", string.Empty)

                // Champions League
                .Replace("MANCHESTER CITY", "MAN CITY")
                .Replace("ATLETICO DE MADRID", "ATLETICO MADRID")
			    .Replace("INDEPENDIENTE", "INDEPEN")
			    .Replace("CRVENA ZVEZDA", "RED STAR BELGRADE")
			    .Replace("SLAVIA PRAHA", "SLAVIA PRAGUE")
			    .Replace("1959", string.Empty)
			    .Replace("1928", string.Empty)
                .Replace("TSG 1899", string.Empty)
			    .Replace("TSG ", string.Empty)
                .Replace("TOTTENHAM HOTSPUR", "TOTTENHAM")
			    .Replace("TOTTENHAM HOTSPURS", "TOTTENHAM")
			    .Replace("BAYER 04", string.Empty)
			    .Replace("BAYER LEVERKUSEN", "LEVERKUSEN")
                .Replace("MENEMEN BELEDIYEOR", "MENEMENSPOR")
			    .Replace("LEICESTER CITY", "LEICESTER")
			    .Replace("OGC ", string.Empty)
			    .Replace(" OGC", string.Empty)
			    .Replace("NK ", string.Empty)
			    .Replace(" NK", string.Empty)
			    .Replace("FF ", string.Empty)
			    .Replace(" FF", string.Empty)
			    .Replace("IF ", string.Empty)
			    .Replace(" IF", string.Empty)
                .Replace("GAIS ", string.Empty)
                .Replace(" GAIS", string.Empty)
                .Replace("RAPID WIEN", "RAPID VIENNA")
			    .Replace("KAA ", string.Empty)
			    .Replace("ROYAL ", string.Empty)
			    .Replace("REAL ", string.Empty)
			    .Replace("HNK ", string.Empty)

                .Replace("SPORTING ", string.Empty)
                // Combo
                .Replace(" + ", "+")

                .Replace(" CF", string.Empty)
			    .Replace("CF ", string.Empty)
			    .Replace(" HK", string.Empty)
			    .Replace("HK ", string.Empty)
			    .Replace(" IK", string.Empty)
			    .Replace("IK ", string.Empty)
			    .Replace(" A.S", string.Empty)
			    .Replace("A.S ", string.Empty)
			    .Replace(" OSC", string.Empty)
			    .Replace("OSC ", string.Empty)
			    .Replace(" SK", string.Empty)
			    .Replace("SK ", string.Empty)
			    .Replace(" AZ", string.Empty)
			    .Replace("AZ ", string.Empty)
                
			    // England
                .Replace("SHEFFIELD WEDNESDAY", "SHEFFIELD WED")
            
                // Germany
                .Replace("SV ", string.Empty)
                .Replace(" SV", string.Empty)
                .Replace("AUE ", string.Empty)
                .Replace(" AUE", string.Empty)
                .Replace("RED BULL", string.Empty)
                .Replace("RB ", string.Empty)
                .Replace(" RB", string.Empty)


                // Turkey - 2nd Div 
                .Replace("FK ", string.Empty)
                .Replace(" FK", string.Empty)

                // Argentina Club Atletico
                .Replace(" CA", string.Empty)
		        .Replace("CA ", string.Empty)

                // CONCACAF
		        .Replace(" CD", string.Empty)
		        .Replace("CD ", string.Empty)
		        .Replace(" DEPORTIVO", string.Empty)
		        .Replace("DEPORTIVO ", string.Empty)
		        .Replace(" CLUB", string.Empty)
		        .Replace("CLUB ", string.Empty)

                // sporting club
                .Replace(" SC", string.Empty)
		        .Replace("SC ", string.Empty)

		        // football club finland
                .Replace(" JK", string.Empty)
		        .Replace("JK ", string.Empty)

		        // reserves team
                // WHY???
                //.Replace("[R]", string.Empty)

		        .Replace("[N]", string.Empty)

                ;

            //string newTeam = team;
            //newTeam = newTeam.Replace("&nbsp;", "");
            //newTeam = newTeam.Replace("&amp;", "");
            //newTeam = newTeam.Replace("-No.of Corners;", " CORNERS");
            
            //newTeam = newTeam.Replace("No.of Corners", " CORNERS");
            //newTeam = newTeam.Replace("-No.of Cns.", " CORNERS");
            
            //newTeam = newTeam.Replace("No. of Corners", " CORNERS");
            //newTeam = newTeam.Replace(" - No. of Corners", " CORNERS");


            //newTeam = newTeam.Replace("(", "[");
            //newTeam = newTeam.Replace(")", "]");
            //newTeam = newTeam.Replace("{", "[");
            //newTeam = newTeam.Replace("}", "]");

            //// football club
            //newTeam = newTeam.Replace(" FC", "");
            //newTeam = newTeam.Replace("FC ", "");

            //// brazil football club
            //newTeam = newTeam.Replace(" EC", "");
            //newTeam = newTeam.Replace("EC ", "");

            //// sport club
            //newTeam = newTeam.Replace(" SC", "");
            //newTeam = newTeam.Replace("SC ", "");

            //// football club
            //newTeam = newTeam.Replace(" JK", "");
            //newTeam = newTeam.Replace("JK ", "");

            //newTeam = newTeam.Replace("-", " ");
            
            //// reserves team
            //newTeam = newTeam.Replace("[R]", "");

            //newTeam = Regex.Replace(newTeam, @"\([^()]*\)", string.Empty);
            //newTeam = Regex.Replace(newTeam, @"\{[^()]*\}", string.Empty);
            //newTeam = Regex.Replace(newTeam, @"\[[^()]*\]", string.Empty);


            //newTeam = Regex.Replace(newTeam, @"\s+", " ");
            //newTeam = Regex.Replace(newTeam, @"[ \t]+$", string.Empty);
            //newTeam = Regex.Replace(newTeam, @"^\s", string.Empty);

            newTeam = prv_RegEx_WhiteSpace.Replace(newTeam, " ");
            newTeam = prv_RegEx_EndTabsSpace.Replace(newTeam, string.Empty);
            newTeam = prv_RegEx_StartWhiteSpace.Replace(newTeam, string.Empty);

            //newTeam = newTeam.ToUpper();

            return newTeam;
        }

        public static string getLeagueType(string league)
        {
            string typeLeague = "";
            if (league.Contains("KICK OFF") || league.Contains("KICK-OFF"))
            {
                typeLeague = " [KO]";
            }
            //if (league.Contains("RESERVE"))
            //{
            //    typeLeague = " [R]";
            //}

            return typeLeague;

        }

        public static string cleanOddPlaceBet(string hdpToFind)
        {
	        hdpToFind = hdpToFind
		        .Replace("@", string.Empty)
		        .Replace("&nbsp;", string.Empty)
		        .Replace("o-u", string.Empty)
		        .Replace("o/u", string.Empty)
		        .Replace("Over/Under", string.Empty)
		        .Replace("OVER/UNDER", string.Empty)
		        .Replace("HDP", string.Empty)
		        .Replace(":", string.Empty)
		        .Replace("ft", string.Empty)
		        .Replace("fh", string.Empty)
		        .Replace("+", string.Empty)
		        .Replace("-", string.Empty)
		        .Replace("/", string.Empty)
		        .Replace(" ", string.Empty)
		        .Replace("ou", string.Empty);

            return hdpToFind;
        }

        public static string cleanMatchTime(string matchTime)
        {
            if (matchTime != "")
            {
                matchTime = matchTime
                    .Replace(" ", string.Empty)
                    .Replace("\"", string.Empty)
                    .Replace("", string.Empty)
                    .Replace("live", string.Empty);

                return matchTime;
            }
            else
            {
                return matchTime;
            }
        }

        public static string getQuarterBet(string hdpToFind)
        {
            hdpToFind = hdpToFind
	            .Replace(" ", "")
				.Replace("-", "");

            string result = "";

            switch (hdpToFind)
            {
                case "0":
                    result = "0";
                    break;
                case "0.25":
                    result = "0-0.5";
                    break;
                case "0.75":
                    result = "0.5-1";
                    break;
                case "1.25":
                    result = "1-1.5";
                    break;
                case "1.75":
                    result = "1.5-2";
                    break;
                case "2.25":
                    result = "2-2.5";
                    break;
                case "2.75":
                    result = "2.5-3";
                    break;
                case "3.25":
                    result = "3-3.5";
                    break;
                case "3.75":
                    result = "3.5-4";
                    break;
                case "4.25":
                    result = "4-4.5";
                    break;
                case "4.75":
                    result = "4.5-5";
                    break;
                case "5.25":
                    result = "5-5.5";
                    break;
                case "5.75":
                    result = "5.5-6";
                    break;
                default:
                    result = hdpToFind;
                    break;
            }

            return result;

        }

        public static string generateArbId(string bm1, string bm2, string teamFav, string teamNoFav)
        {
            string timestamp = GetTimestamp(DateTime.Now);
            string input = bm1 + bm2 + teamFav + teamNoFav + timestamp;
            using (HashAlgorithm algorithm = SHA256.Create())
            {
                byte[] bytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string generateMatchId(string teamFav, string teamNoFav)
        {
            string input = teamFav + teamNoFav;
            using (HashAlgorithm algorithm = SHA256.Create())
            {
                byte[] bytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string getRealOddType(string oddtype, string match, string teamWinner)
        {
            string result = "";
            string type = "";
            string time = "";

            if (oddtype.Contains("o/u")){

                int indexteam = match.IndexOf(teamWinner);
                if(indexteam == 0)
                {
                    type = "OVER";
                }
                else
                {
                    type = "UNDER";
                }
            }
            else
            {

            }

            if (oddtype.Contains("ft"))
            {
                time = "FULL TIME";
            }

            if (oddtype.Contains("fh"))
            {
                time = "HALF TIME";
            }

            result = time + type;

            return result;
        }

        public static bool isOverUnder(string hdp)
        {
            if (hdp.Contains("o/u"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GetLocalIPAddress()
        {
            var prv_Host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var prv_IP in prv_Host.AddressList)
            {
                if (prv_IP.AddressFamily == AddressFamily.InterNetwork)
                {
                    return prv_IP.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
        public static string GetLocalFQDN()
        {
            var prv_Properties = IPGlobalProperties.GetIPGlobalProperties();
            return $"{prv_Properties.HostName}{(string.IsNullOrWhiteSpace(prv_Properties.DomainName) ? "" : "." + prv_Properties.DomainName)}";
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }



        public static void writeSuccess(string text)
        {
	        Console.ForegroundColor = ConsoleColor.Green;
	        NLogger.Log(EventLevel.Debug, text);
	        Console.ResetColor();
        }

        public static void writeLoss(string text)
        {
	        Console.ForegroundColor = ConsoleColor.Red;
	        NLogger.Log(EventLevel.Debug, text);
	        Console.ResetColor();
        }

        public static void writeError(string text)
        {
	        Console.ForegroundColor = ConsoleColor.DarkRed;
	        NLogger.Log(EventLevel.Debug, text);
	        Console.ResetColor();
        }

        public static void writeGray(string text)
        {
	        Console.ForegroundColor = ConsoleColor.Gray;
	        NLogger.Log(EventLevel.Debug, text);
	        Console.ResetColor();
        }

        public static void writeYellow(string text)
        {
	        Console.ForegroundColor = ConsoleColor.Yellow;
	        NLogger.Log(EventLevel.Debug, text);
	        Console.ResetColor();
        }

        public static void writeCyan(string text)
        {
	        Console.ForegroundColor = ConsoleColor.Cyan;
	        NLogger.Log(EventLevel.Debug, text);
	        Console.ResetColor();
        }

        public static void writeMagenta(string text)
        {
	        Console.ForegroundColor = ConsoleColor.Magenta;
	        NLogger.Log(EventLevel.Debug, text);
	        Console.ResetColor();
        }

        public static string removeStartingBracketText(string prm_Text)
        {
            if (prm_Text.StartsWith("["))
            {
                prm_Text = prm_Text.Substring(prm_Text.IndexOf(']'));
            }
            return prm_Text;
		}
	}
}
