using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenQA.Selenium;

namespace ArbibetProgram.Models
{
	public class FindGame_Result
	{
		public IWebElement OddsElement { get; set; } = null;

		/// <summary>
		/// true means odds >= 1 [and that we found the odds element]
		/// false either means we couldn't find the odds element, or odds less than 1
		/// </summary>
		public bool CanPlaceBet_Flag { get; set; }

		/// <summary>
		/// true = located the odds element
		/// </summary>
		public bool FoundOdds_Flag { get; set; }

		/// <summary>
		/// true = located the handicap element
		/// </summary>
		public bool FoundHandicap_Flag { get; set; }

		/// <summary>
		/// true = located the match element
		/// </summary>
		public bool FoundMatch_Flag { get; set; }

		/// <summary>
		/// true = we couldn't find the match + handicap at the original XPath, but found it after DeepSearch
		/// </summary>
		public bool Handicap_Moved_Flag { get; set; }

		/// <summary>
		/// true = we find the match at the original XPath, but found it after DeepSearch
		/// </summary>
		public bool Match_Moved_Flag { get; set; }

	}

}
