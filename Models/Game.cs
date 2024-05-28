using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenQA.Selenium;

namespace ArbibetProgram.Models
{

	public class Game
	{
		/// <summary>
		/// unique identifier: [{League_Clean}] {TeamName_Home_Clean} - {TeamName_Away_Clean} [{GameScope} {GameModifier} {Handicap_Clean}]
		/// GameScope: time scope of this betting game, either FT or FH [full time or first half]
		/// GameModifier: one of "o/u", "+", or "-"
		/// Handicap_Clean: eg 1-1.5
		/// </summary>
		public string Identifier { get; set; }

		public string MatchName_Base { get; set; }

		public DateTime Match_DateTime { get; set; }

		/// <summary>
		/// true if this game has active odds
		/// </summary>
		public bool isActive { get; set; }
    
		public List<CasinoOdd> Team_Home = new List<CasinoOdd>();
		public List<CasinoOdd> Team_Away = new List<CasinoOdd>();


		public static bool CheckDictionary()
		{
			var prv_Conflicts = new Dictionary<string, Game>();
			var prv_Singles = new Dictionary<string, Game>();
			var prv_Singles_BaseMatch = new List<SimpleMatchData>();
			var prv_Singles_Simple = new Dictionary<string, SimpleMatchData>();

			foreach (var prv_Game in MainClass.gs_Games)
			{
				string prv_Identifier = prv_Game.Key;

				if (prv_Game.Value.Team_Home.Count > MainClass.gs_NumBookmakers)
				{
					prv_Conflicts.Add(prv_Identifier, prv_Game.Value);
				}
				else if (prv_Game.Value.Team_Home.Count == 1)
				{
					prv_Singles.Add(prv_Identifier, prv_Game.Value);

					string prv_BookmakerName = MainClass.bookmakers[prv_Game.Value.Team_Home[0].BookmakerIndex].BookmakerName;
					prv_Singles_BaseMatch.Add(new SimpleMatchData()
					{
						BookmakerName = prv_BookmakerName,
						Identifer = prv_Identifier
					});

					string prv_MatchName_Base = prv_Game.Value.MatchName_Base;
					if (!prv_Singles_Simple.ContainsKey(prv_MatchName_Base))
					{
						prv_Singles_Simple.Add(prv_MatchName_Base, new SimpleMatchData()
						{
							BookmakerName = prv_BookmakerName
						}) ;
					}
				}
			}

			// ------------------------------------------------------------------------------
			// this block is useful for seeing what's going on inside the games dictionary
			// ------------------------------------------------------------------------------
			if (MainClass.gs_DemoMode)
			{
				//var prv_Games = JsonConvert.SerializeObject(MainClass.gs_Games, Formatting.Indented);
				//var prv_ConflictsString = JsonConvert.SerializeObject(prv_Conflicts, Formatting.Indented);
				//var prv_SinglesString = JsonConvert.SerializeObject(prv_Singles, Formatting.Indented);
				//var prv_BaseMatchesString = JsonConvert.SerializeObject(prv_Singles_BaseMatch, Formatting.Indented);
				//var prv_BaseMatches_Simple = JsonConvert.SerializeObject(prv_Singles_Simple, Formatting.Indented);

				//foreach (var prv_Game in MainClass.gs_Games)
				//{
				//	var prv_CasinoOdd = prv_Game.Value.Team_Home[0];
				//	var prv_FindGameResult = MainClass.bookmakers[0].ActiveAccount.API.findGame(prv_CasinoOdd);
				//	break;
				//}

				//var prv_HighOddsEntry = MainClass.gs_Games.FirstOrDefault(g => g.Value.Team_Home[0].WinOdds > 1);
				//var prv_FindGameResult = MainClass.bookmakers[0].ActiveAccount.API.findGame(prv_HighOddsEntry.Value.Team_Home[0]);
			}

			//return false;
			return (prv_Conflicts.Count != 0);
		}

		public static void ResetDictionary()
		{
			foreach (var prv_Game in MainClass.gs_Games)
			{
				var prv_Item = prv_Game.Value;
				prv_Item.isActive = false;
				prv_Item.Team_Home.Clear();
				prv_Item.Team_Away.Clear();
			}
		}
	}

	public class SimpleMatchData
	{
		public string Identifer { get; set; }
		public string MatchName_Base { get; set; }
		public string BookmakerName { get; set; }
	}

	public class CasinoOdd
	{
		/// <summary>
		/// in position in global bookmakers list
		/// </summary>
		public int BookmakerIndex { get; set; }

		/// <summary>
		/// the exact name of the league on the website
		/// </summary>
		public string LeagueText { get; set; }

		/// <summary>
		/// innerText of the Match div
		/// </summary>
		public string MatchText { get; set; }

		/// <summary>
		/// innerText of the Handicap div
		/// </summary>
		public string HandicapText { get; set; }

		/// <summary>
		/// XPath of the handicap; path is relative to the match row
		/// </summary>
		public string XPath_Handicap { get; set; }

		/// <summary>
		/// XPath of the odds for this team; path is relative to the match row
		/// </summary>
		public string XPath_Odds { get; set; }

		public string XPath_Match { get; set; }
		public string XPath_League { get; set; }

		/// <summary>
		/// ROI if you win the bet
		/// </summary>
		public decimal WinOdds { get; set; }
	}
}
