using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class ObjectPool<T> where T : Node3D
{
		private Stack<T> _pool;
		private PackedScene _scene;
		public ObjectPool(PackedScene scene)
		{
				_pool = new();
				_scene = scene;
		}
		public void Add(T node)
		{
				_pool.Push(node);
				node.ProcessMode = Node.ProcessModeEnum.Disabled;
				node.Visible = false;

		}
		public T Pull()
		{
				Node3D node;
				if (_pool.Count == 0)
				{
						node = _scene.Instantiate<T>();
				}
				else
				{
						node = _pool.Pop();
				}
				node.ProcessMode = Node.ProcessModeEnum.Inherit;
				node.Visible = true;
				return (T)node;
		}
		public int Count
		{
				get
				{
						return _pool.Count;
				}
		}
}
