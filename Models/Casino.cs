using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbibetProgram.Models
{
	public class Casino
	{
		public string BookmakerName { get; set; }
		public AccountInfo AccountInfo { get; set; }
		public CasinoAPI API { get; set; }
	}
}
