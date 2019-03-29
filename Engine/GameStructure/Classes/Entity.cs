using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.GameStructure.Classes
{
	public class Entity
	{
		public IntPtr Base { get; set; }

		public Entity(IntPtr ptr) => Base = ptr;

		public unsafe bool IsPlayer => Convert.ToBoolean(*(int*)(Base + 0x34C));

		public unsafe uint ObjectId => *(uint*)(Base + 0x2F0);

		public unsafe int Health => *(int*)(Base + 0x768);

		public unsafe int Mana => *(int*)(Base + 0x76C);

		public string Name => Marshal.PtrToStringAnsi(Base + 0x1A7C);

		public unsafe Vector3 Position
		{
			get
			{
				if (Base == IntPtr.Zero)
					return Vector3.Zero;

				return *(Vector3*)(Base + 0x160);
			}

			set => *(Vector3*)(Base + 0x160) = value;
		}

		public override string ToString()
		{
			if (Base == IntPtr.Zero)
				return "[IntPtr.Zero]";

			return $"[{Base}] => [{Name}][{ObjectId}][{Health}]";
		}
	}
}
