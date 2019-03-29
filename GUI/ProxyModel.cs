using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using Engine.GameStructure.Classes;

namespace GUI
{
	public class ProxyModel
	{
		public string Name => Game.LocalPlayer.Name;

		public int Level = 0;

		public int Health => Game.LocalPlayer.Health;

		public int Mana => Game.LocalPlayer.Mana;

		public string Position 
			=> $"[{Game.LocalPlayer.Position.X} {Game.LocalPlayer.Position.Y} {Game.LocalPlayer.Position.Z}]";

		public string[] TargetNames
		{
			get => Core.Config.TargetNames;
			set => Core.Config.TargetNames = value;
		}	

		public int HealTrigger
		{
			get => Core.Config.HealTrigger;
			set => Core.Config.HealTrigger = value;
		}	
	}
}
