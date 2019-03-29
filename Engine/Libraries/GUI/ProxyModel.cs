using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Engine;
using Engine.GameStructure.Classes;

namespace Engine.Libraries.GUI
{
	public class ProxyModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged([CallerMemberName] string propertyName = null) 
			=> PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

		public string Name => Game.LocalPlayer.Name;

		public int Level = 0;

		public int Health => Game.LocalPlayer.Health;

		public int Mana => Game.LocalPlayer.Mana;

		public string IsPaused => Core.IsBreakEnabled ? "True" : "False";

		public Entity Target => Game.TargetObject;

		public double TargetDistance => Vector3.DistanceSquared(Target.Position, Game.LocalPlayer.Position);

		public string Position 
			=> $"[{Game.LocalPlayer.Position.X} {Game.LocalPlayer.Position.Y} {Game.LocalPlayer.Position.Z}]";

		public bool KeepDistance
		{
			get => Core.Config.KeepDistance;
			set
			{
				Core.Config.KeepDistance = value;

				OnPropertyChanged("KeepDistance");
			}
		}

		public int AoeSize
		{
			get => Core.Config.AoeSize;
			set
			{
				Core.Config.AoeSize = value;

				OnPropertyChanged("AoeSize");
			}
		}

		public string TargetNames
		{
			get => Core.Config.TargetNames;
			set
			{
				Core.Config.TargetNames = value;

				OnPropertyChanged("TargetNames");
			}
		}

		public int HealTrigger
		{
			get => Core.Config.HealTrigger;
			set
			{
				Core.Config.HealTrigger = Convert.ToInt32(value);

				OnPropertyChanged("HealTrigger");
			}
		}	
	}
}
