using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.GameStructure.Classes
{
	public class ObjectMap
	{
		public IntPtr Base { get; }

		public unsafe int Count
		{
			get
			{
				return *(int*)(Base + 4);
			}
		}

		public ObjectMap(IntPtr ptr)
		{
			Base = ptr;
		}

		public IEnumerable<Entity> GetObjects()
		{
			var nodeList = new List<Entity>();

			unsafe
			{
				var pInitNode = *(IntPtr*)(Base);
				if (pInitNode == IntPtr.Zero)
					return null;

				var initNode = (RBTNode*)(pInitNode);

				if (Count == 0)
					return nodeList;

				var rootNode = initNode->Parent;

				// Add the root node
				nodeList.Add(new Entity(rootNode->Value));

				// Recursion for the root's left node
				Recursive(rootNode->Left, ref nodeList);

				// Recursion for the root's right node
				Recursive(rootNode->Right, ref nodeList);
			}

			return nodeList;
		}

		private unsafe void Recursive(RBTNode* node, ref List<Entity> nodeList)
		{
			if (node->IsInitNode)
				return;

			// Add node
			nodeList.Add(new Entity(node->Value));

			// Recursion for the root's left node
			Recursive(node->Left, ref nodeList);

			// Recursion for the root's right node
			Recursive(node->Right, ref nodeList);
		}
	}
}
