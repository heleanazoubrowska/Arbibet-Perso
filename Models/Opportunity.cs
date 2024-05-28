using System;
using System.Collections.Generic;
using System.Text;

namespace ArbibetProgram.Models
{
	public class Opportunity
	{
		public string SecondBookmakerName { get; set; }
		public string MatchName { get; set; }
		public int Duration { get; set; }
		public int Timestamp { get; set; }
		public bool HasSentApi { get; set; }
	}
}
