using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Engine.GameStructure.Classes
{
	public static class Game
	{
		static Game()
		{
			SetObjFocusSingleton = Marshal.GetDelegateForFunctionPointer<SetObjFocusDelegate>(Base + 0x5811E0);
			SetDestObjSingleton = Marshal.GetDelegateForFunctionPointer<SetDestObjDelegate>(Base + 0x4ED230);
			SetDestPosSingleton = Marshal.GetDelegateForFunctionPointer<SetDestPosDelegate>(Base + 0x4EC0C0);
			IntersectObjLineSingleton = Marshal.GetDelegateForFunctionPointer<IntersectObjLineDelegate>(Base + 0x59E4E0);
		}

		public static IntPtr WindowHandle => Process.GetCurrentProcess().MainWindowHandle;

		public static IntPtr Base => Process.GetCurrentProcess().MainModule.BaseAddress;

		private static unsafe Entity localPlayer = new Entity(*(IntPtr*)(Base + 0xA57B8C));

		public static unsafe Entity LocalPlayer
		{
			get
			{
				if (localPlayer.Base == IntPtr.Zero || localPlayer.Position == Vector3.Zero)
					localPlayer = new Entity(*(IntPtr*)(Base + 0xA57B8C));

				return localPlayer;
			}

			set => localPlayer = value;
		}

		public static unsafe IntPtr ProjectPtr = Base + 0xA5C398;

		public static unsafe ObjectMap ObjectMap = new ObjectMap(ProjectPtr + 0x54);

		public static unsafe IntPtr WorldPtr => *(IntPtr*)(LocalPlayer.Base + 0x16C);

		public static unsafe Entity TargetObject => new Entity(*(IntPtr*)(*(IntPtr*)(Base + 0xA5C35C) + 0x20));

		public static unsafe Entity NextTargetObject => new Entity(*(IntPtr*)(*(IntPtr*)(Base + 0xC9C9EC) + 0x12A4));

		public static unsafe IntPtr NextTargetObjectPtr = *(IntPtr*)(Base + 0xC9C9EC) + 0x12A4;

		public static unsafe void SetNextTargetObject(Entity target) 
			=> Marshal.WriteIntPtr(NextTargetObjectPtr, target != null ? target.Base : IntPtr.Zero);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void SetObjFocusDelegate(IntPtr world, IntPtr target, int send = 0);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate int IntersectObjLineDelegate(IntPtr world, ref Vector3 hitVec, ref Vector3 pos, ref Vector3 end, int skipTrans = 0, int withTerrain = 0, int withObject = 1);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void SetDestObjDelegate(IntPtr moverPtr, uint objId, float range = 0, int send = 1);

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void SetDestPosDelegate(IntPtr moverPtr, ref Vector3 pos, int forward, int send = 1);

		private static SetObjFocusDelegate SetObjFocusSingleton;

		private static IntersectObjLineDelegate IntersectObjLineSingleton;

		private static SetDestObjDelegate SetDestObjSingleton;

		private static SetDestPosDelegate SetDestPosSingleton;

		public static unsafe void SetObjFocus(Entity target, bool send = false)
		{
			if (SetObjFocusSingleton == null)
				throw new NullReferenceException("SetObjFocus has not been initialized!");

			SetObjFocusSingleton(WorldPtr, target != null ? target.Base : IntPtr.Zero, Convert.ToInt32(send));
		}

		public static void SetDestObj(Entity target, float range = 0, bool send = true)
		{
			if (SetDestObjSingleton == null)
				throw new NullReferenceException("SetDestObj has not been initialized!");

			SetDestObjSingleton(LocalPlayer.Base, target != null ? target.ObjectId : 0, range, send ? 1 : 0);
		}

		public static void SetDestPos(ref Vector3 pos, bool forward, bool send = true)
		{
			if (SetDestPosSingleton == null)
				throw new NullReferenceException("SetDestPos has not been initialized!");

			SetDestPosSingleton(LocalPlayer.Base, ref pos, forward ? 1 : 0, send ? 1 : 0);
		}

		public static bool IntersectObjLine(ref Vector3 vecOut, ref Vector3 vPos, ref Vector3 vEnd, bool bSkipTrans = false, bool bWithTerrain = false, bool bWithObject = true)
		{
			if (IntersectObjLineSingleton == null)
				throw new NullReferenceException("IntersectObjLine has not been initialized!");;

			return Convert.ToBoolean(IntersectObjLineSingleton(WorldPtr, ref vecOut, ref vPos, ref vEnd,
				Convert.ToInt32(bSkipTrans), Convert.ToInt32(bWithTerrain), Convert.ToInt32(bWithObject)));
		}

		public static Vector3 ExtrapolatePoint(Vector3 vecFrom, Vector3 vecTo, int distance)
		{
			var normal = Vector3.Normalize(vecTo - vecFrom);

			var movepoint = vecFrom + normal * (float)Math.Sqrt(distance);
			movepoint.Y = vecFrom.Y;

			return movepoint;
		}
	}
}
