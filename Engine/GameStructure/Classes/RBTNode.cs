using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.GameStructure.Classes
{
	public unsafe struct RBTNode
	{
		public RBTNode* Left;
		public RBTNode* Parent;
		public RBTNode* Right;
		#pragma warning disable IDE0044 // Add readonly modifier
		private byte bIsBlack;
		#pragma warning restore IDE0044 // Add readonly modifier
		#pragma warning disable IDE0044 // Add readonly modifier
		private byte bIsInitNode;
		#pragma warning restore IDE0044 // Add readonly modifier
		private ushort Unk;
		public uint Key;
		public IntPtr Value;

		public bool IsInitNode { get { return Convert.ToBoolean(bIsInitNode); } }
		public bool IsBlack { get { return Convert.ToBoolean(bIsBlack); } }
	}
}
