using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbibetProgram.Models
{
	public class BetPlaced
	{
		public string ArbitrageGui { get; set; }

		public string AccountName { get; set; }

		public string MatchName { get; set; }

		public int BookmakerIndex { get; set; }

		public string BookmakerName { get; set; }

		public string TeamFav { get; set; }

		public string TeamNoFav { get; set; }

		public string League { get; set; }

		public bool BetFound { get; set; }

		public bool BetWaiting { get; set; }

		public bool BetReject { get; set; }

		public bool BetConfirmed { get; set; }

		public string LastStatusSent { get; set; }
	}
}
