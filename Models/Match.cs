using System;
using System.Collections.Generic;

namespace ArbibetProgram.Models
{
    public class Match
    {
        /// <summary>
        /// TeamFav_TeamNoFav
        /// </summary>
        public string MatchName { get; set; }

        /// <summary>
        /// TeamNoFav_TeamFav
        /// </summary>
        public string MatchName2 { get; set; }

        public string MatchId { get; set; }
        public string MatchDate { get; set; }
        public string MatchTime { get; set; }
        public string Matchleague { get; set; }
        public bool MatchIsLive { get; set; }
        public Team TeamFav { get; set; }
        public Team TeamNoFav { get; set; }
        public List<Odd> Odds { get; set; } = new List<Odd>();
        public bool betIsPlaced { get; set; }

        public Match()
        {
            TeamFav = new Team();
            TeamNoFav = new Team();
        }
    }

    public class Team
    {
        public string TeamName { get; set; }
        public int TeamId { get; set; }

        public Team()
        {
        }
    }

    public class Odd
    {
	    public string oddType { get; set; }
	    //public bool oddIsOverUnder { get; set; }
	    public decimal oddFav { get; set; }
	    public decimal oddNoFav { get; set; }

        /// <summary>
        /// handicap of the game on the site eg 1/1.5
        /// </summary>
        public string HandicapText { get; set; }

        /// <summary>
        /// time scope of this betting game, either FT or FH [full time or first half]
        /// </summary>
        public string GameScope { get; set; }

        /// <summary>
        /// one of "o/u", "+", or "-"
        /// </summary>
        public string GameModifier { get; set; }

        
        public string XPath_Match { get; set; }
		
        /// <summary>
        /// XPath of the handicap; path is relative to the match row
        /// </summary>
        public string XPath_Handicap { get; set; }

        /// <summary>
        /// XPath of the home team's odds; path is relative to the match row
        /// </summary>
        public string XPath_Odds_Home { get; set; }

        /// <summary>
        /// XPath of the away team's odds; path is relative to the match row
        /// </summary>
        public string XPath_Odds_Away { get; set; }


        public Odd()
	    {

	    }
    }
}
//*[@id="rj-asian-view-events"]/sb-comp/div[2]/div/div[3]/div[1]
//*[@id="trMatch2_2285274"]/td[3]/span[2]

//*[@id="malai"]/div[1]/span[2]/span/span

//*[@id="hr-bot-Top_ResponsiveHeader_19064-page-header-right2"]/div/div[1]/a

//*[@id="rj-asian-view-events"]/sb-comp/div/div/div[3]/div[1]/div[1]


//*[@id="rj-asian-view-events"]/sb-comp/div[2]/div/div[3]/div[1]/div[1]/h4

//html/body/div[1]/div[4]/div[1]/main/div/sb-comp/div[1]/div/div[3]
//*[@id="doubleLine"]/div[1]/span[2]/span/span

//*[@id="doubleLine"]/div[2]/ul/li[2]/span

//*[@id="2390847-0-1"]/td[4]/b/a

//*[@id="rj-asian-view-events"]/sb-comp/div[2]/div/div[3]/div[1]/div[2]/div[1]/div[1]/div[3]/div/div[1]/div[2]/div[1]