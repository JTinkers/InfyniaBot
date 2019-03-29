using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
	public class Config
	{
		public string TargetNames { get; set; }

		public List<string> TargetNameList
		{
			get
			{
				if (string.IsNullOrEmpty(TargetNames))
					return null;

				return TargetNames?.Split(';').ToList();
			}
		}

		public bool KeepDistance = false;

		public int AoeSize = 5;

		public int PreferedDistance = 300;

		public int SpellKey = 115;

		public int AttackKey = 114;

		public int HealKey = 113;

		public int SupportIntervals = 250;

		public int TargettingIntervals = 100;

		public int HealTrigger { get; set; }

		public int HealDelay = 750;
	}
}
