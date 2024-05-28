using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbibetProgram.Models
{
	public class Bookmaker
	{
		public string BookmakerName { get; set; }
		public int BookmakerIndex { get; set; }
		public string BookmakerUrl { get; set; }
		public bool BookmakerActive { get; set; }
		public bool BookmakerAccountActive { get; set; }
		public string BookmakerUser { get; set; }
		public string BookmakerPass { get; set; }
		public List<Match> BookmakerMatches { get; set; }
		public string BookmakerOddType { get; set; }
		public bool BookmakerShowPage { get; set; }
		public bool BookmakerCanPolling { get; set; }
		public decimal BookmakerAccountBalance { get; set; }
		public List<string> BookmakerMatchBetted { get; set; }
		public List<Opportunity> BookmakerOpportunities { get; set; }

		public Account ActiveAccount { get; set; } = null;
		public List<Account> Accounts { get; set; }

		public Bookmaker()
		{
			BookmakerMatches = new List<Match>();
			BookmakerMatchBetted = new List<string>();
			BookmakerOpportunities = new List<Opportunity>();
			Accounts = new List<Account>();
		}

		
	}

	public class Account
	{
		public AccountInfo AccountInfo { get; set; }
		public CasinoAPI API { get; set; }
	}


}
