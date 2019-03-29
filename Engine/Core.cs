using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Engine.Libraries;
using Engine.GameStructure.Classes;
using System.Diagnostics;
using System.Numerics;
using Engine.Libraries.GUI;

namespace Engine
{
	public static class Core
	{
		public static Config Config { get; set; } = new Config();

		public static bool IsBreakEnabled { get; set; } = false;

		public static bool IsBreakPushed() => Native.GetAsyncKeyState(35) == 1
			|| Native.GetAsyncKeyState(35) == short.MinValue;

		[HandleProcessCorruptedStateExceptions]
		[SecurityCritical]
		public static int Initialize()
		{
			try
			{
				Task.Run(() =>
				{
					while (true)
						if (IsBreakPushed())
						{
							IsBreakEnabled = !IsBreakEnabled;

							Console.WriteLine($"Break mode: {IsBreakEnabled}.");

							Thread.Sleep(1000);

							Entities.Clear();
						}
				});

				var guiThread = new Thread(() =>
				{
					var window = new GUIWindow();

					window.ShowDialog();
				});
				guiThread.SetApartmentState(ApartmentState.STA);
				guiThread.Start();

				Task.Run(() =>
				{
					while (true)
					{
						if (!IsBreakEnabled)
							Support();
					}
				});

				Task.Run(() =>
				{
					while (true)
					{
						if (!IsBreakEnabled)
							Targetting();
					}
				});
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			return 1;
		}

		public static unsafe void Support()
		{
			if (Game.LocalPlayer.Health < Config.HealTrigger)
			{
				Native.PostMessage(Game.WindowHandle, Native.WM_KEYDOWN, Config.HealKey, 0);
				Thread.Sleep(50);
				Native.PostMessage(Game.WindowHandle, Native.WM_KEYUP, Config.HealKey, 0);

				Thread.Sleep(Config.HealDelay);

				return;
			}

			Thread.Sleep(Config.SupportIntervals);
		}

		public static Vector3 VecOut;
		public static Vector3 VecPos = new Vector3();
		public static Vector3 VecEnd = new Vector3();

		public static List<Entity> Entities { get; set; } = new List<Entity>();

		[HandleProcessCorruptedStateExceptions]
		[SecurityCritical]
		public static unsafe void Targetting()
		{
			Native.AllocConsole();

			try
			{
				//List not specified yet
				if (Config.TargetNameList == null || !Config.TargetNameList.Any())
					return;

				//Only refill when empty
				if (!Entities.Any())
					Entities.AddRange(Game.ObjectMap.GetObjects());

				//Sort by distance
				Entities.Sort((x, y) => Vector3.DistanceSquared(Game.LocalPlayer.Position, x.Position)
					.CompareTo(Vector3.DistanceSquared(Game.LocalPlayer.Position, y.Position)));

				//Remove non-monsters, dead, unwanted, invalid or too far away
				Entities.RemoveAll(x => x.Base == IntPtr.Zero);

				Entities.RemoveAll(x => x.IsPlayer);

				Entities.RemoveAll(x => !Config.TargetNameList.Contains(x.Name));

				Entities.RemoveAll(x => x.Health == 0);

				Entities.RemoveAll(x => Vector3.DistanceSquared(Game.LocalPlayer.Position, x.Position) > 2000);

				//Leave only up to N possible targets, thus reducing 
				Entities.RemoveRange(Math.Min(Entities.Count, 15),
					Entities.Count - Math.Min(Entities.Count, 15));

				//If no enemies after removal, skip routine
				if (!Entities.Any())
				{
					Console.WriteLine("No targets found. Skipping cycle.");

					Thread.Sleep(Config.TargettingIntervals);

					return;
				}

				OneVersusOneAttackRoutine();
				//OneVersusManyAttackRoutine();

				Entities.Clear();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		//F3 attack, F4 spell
		public static void OneVersusManyAttackRoutine()
		{
			//Gather targets
			var targets = new List<Entity>();
			while(Entities.Any() && !IsBreakEnabled && targets.Count < Config.AoeSize)
			{
				var first = Entities.First();
				var hp = first.Health;

				Game.SetObjFocus(first);

				VecPos.X = Game.TargetObject.Position.X;
				VecPos.Y = Game.TargetObject.Position.Y + 0.05f;
				VecPos.Z = Game.TargetObject.Position.Z;

				VecEnd.X = Game.LocalPlayer.Position.X;
				VecEnd.Y = Game.LocalPlayer.Position.Y;
				VecEnd.Z = Game.LocalPlayer.Position.Z;

				//Behind obstacle, remove from list and try next ones
				if (Game.IntersectObjLine(ref VecOut, ref VecPos, ref VecEnd))
				{
					Game.SetObjFocus(null);

					Entities.RemoveAt(0);

					continue;
				}

				Native.PostMessage(Game.WindowHandle, Native.WM_KEYDOWN, Config.AttackKey, 0);
				Thread.Sleep(50);
				Native.PostMessage(Game.WindowHandle, Native.WM_KEYUP, Config.AttackKey, 0);

				while (hp == first.Health) { }

				Native.PostMessage(Game.WindowHandle, Native.WM_KEYDOWN, 27, 0);
				Thread.Sleep(50);
				Native.PostMessage(Game.WindowHandle, Native.WM_KEYUP, 27, 0);
				Thread.Sleep(50);
				Native.PostMessage(Game.WindowHandle, Native.WM_KEYDOWN, 87, 0);
				Thread.Sleep(50);
				Native.PostMessage(Game.WindowHandle, Native.WM_KEYUP, 87, 0);

				Thread.Sleep(50);

				var movepoint = new Vector3
				(
					Game.LocalPlayer.Position.X + 1, 
					Game.LocalPlayer.Position.Y + 1, 
					Game.LocalPlayer.Position.Z + 1
				);

				Game.SetDestPos(ref movepoint, true);

				//If was killed by wand
				if (first.Health == 0)
					continue;

				targets.Add(first);

				Entities.RemoveAt(0);

				//Sort by distance from last targetted
				Entities.Sort((x, y) => Vector3.DistanceSquared(targets.Last().Position, x.Position)
					.CompareTo(Vector3.DistanceSquared(targets.Last().Position, y.Position)));

				Thread.Sleep(Config.TargettingIntervals);
			}

			//Do AoE
			while (targets.Any() && !IsBreakEnabled)
			{
				targets.RemoveAll(x => x.Base == IntPtr.Zero || x.Health == 0);

				if (!targets.Any())
					break;

				//Sort by closest
				targets.Sort((x, y) => Vector3.DistanceSquared(Game.LocalPlayer.Position, x.Position)
					.CompareTo(Vector3.DistanceSquared(Game.LocalPlayer.Position, y.Position)));

				Game.SetObjFocus(targets.First());

				VecPos.X = Game.TargetObject.Position.X;
				VecPos.Y = Game.TargetObject.Position.Y;
				VecPos.Z = Game.TargetObject.Position.Z;

				VecEnd.X = Game.LocalPlayer.Position.X;
				VecEnd.Y = Game.LocalPlayer.Position.Y;
				VecEnd.Z = Game.LocalPlayer.Position.Z;

				var distance = Vector3.DistanceSquared(VecEnd, VecPos);

				//Move away if too close
				if(distance < Config.PreferedDistance)
				{
					var movepoint = Game.ExtrapolatePoint(VecPos, VecEnd, Config.PreferedDistance + 10);

					Game.SetDestPos(ref movepoint, true);

					Thread.Sleep(Config.TargettingIntervals);

					continue;
				}

				//Cast a spell
				Native.PostMessage(Game.WindowHandle, Native.WM_KEYDOWN, Config.SpellKey, 0);
				Thread.Sleep(50);
				Native.PostMessage(Game.WindowHandle, Native.WM_KEYUP, Config.SpellKey, 0);

				Thread.Sleep(Config.TargettingIntervals);
			}
		}

		public static void OneVersusOneAttackRoutine()
		{
			//Find an appropriate target
			while (Game.TargetObject.Base == IntPtr.Zero && Entities.Any() && !IsBreakEnabled)
			{
				Game.SetObjFocus(Entities.First());

				Native.PostMessage(Game.WindowHandle, Native.WM_KEYDOWN, Config.AttackKey, 0);
				Thread.Sleep(50);
				Native.PostMessage(Game.WindowHandle, Native.WM_KEYUP, Config.AttackKey, 0);

				if (Game.TargetObject.Base == IntPtr.Zero)
				{
					Console.WriteLine("No target set. Skipping cycle.");

					break;
				}

				VecPos.X = Game.TargetObject.Position.X;
				VecPos.Y = Game.TargetObject.Position.Y + 0.05f;
				VecPos.Z = Game.TargetObject.Position.Z;

				VecEnd.X = Game.LocalPlayer.Position.X;
				VecEnd.Y = Game.LocalPlayer.Position.Y;
				VecEnd.Z = Game.LocalPlayer.Position.Z;

				//Behind obstacle, remove from list and try next ones
				if (Game.IntersectObjLine(ref VecOut, ref VecPos, ref VecEnd))
				{
					Game.SetObjFocus(null);

					Entities.RemoveAt(0);
				}

				Thread.Sleep(Config.TargettingIntervals);
			}

			//Wait for target to stop breathing 
			while (Game.TargetObject.Base != IntPtr.Zero && !IsBreakEnabled)
			{
				VecPos.X = Game.TargetObject.Position.X;
				VecPos.Y = Game.TargetObject.Position.Y + 0.05f;
				VecPos.Z = Game.TargetObject.Position.Z;

				VecEnd.X = Game.LocalPlayer.Position.X;
				VecEnd.Y = Game.LocalPlayer.Position.Y;
				VecEnd.Z = Game.LocalPlayer.Position.Z;

				//Make sure current target isn't behind obstacle
				if (Game.IntersectObjLine(ref VecOut, ref VecPos, ref VecEnd))
				{
					Game.SetObjFocus(null);

					Entities.RemoveAt(0);
				}

				Console.WriteLine(Config.KeepDistance.ToString());
				var distance = Vector3.DistanceSquared(VecEnd, VecPos);
				if (Config.KeepDistance && distance < Config.PreferedDistance)
				{
					var movepoint = Game.ExtrapolatePoint(VecPos, VecEnd, Config.PreferedDistance + 10);

					Game.SetDestPos(ref movepoint, true);

					continue;
				}

				Native.PostMessage(Game.WindowHandle, Native.WM_KEYDOWN, Config.AttackKey, 0);
				Thread.Sleep(50);
				Native.PostMessage(Game.WindowHandle, Native.WM_KEYUP, Config.AttackKey, 0);

				//Set position to target closest to your target
				//if (Game.NextTargetObject.Base == IntPtr.Zero && Entities.Count > 1)
				//	Game.SetNextTargetObject(Entities.Skip(1).OrderBy(x => Vector3.DistanceSquared(Game.TargetObject.Position, x.Position)).First());

				Thread.Sleep(Config.TargettingIntervals);
			}
		}
	}
}
