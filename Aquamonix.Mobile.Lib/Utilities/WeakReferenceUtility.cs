using System;
using System.Linq;
using System.Reflection;

namespace Aquamonix.Mobile.Lib.Utilities
{
    /// <summary>
    /// Weak references are to be used mainly for storing references to function wrappers; a weak reference will ensure 
    /// that those objects do not interfere with garbage collection. 
    /// </summary>
	public static class WeakReferenceUtility
	{
		public static Action MakeWeakAction(Action action)
		{
			if (action != null)
			{
				var reference = new WeakReference(action.Target);
				var method = action.GetInvocationList().FirstOrDefault().GetMethodInfo();
				return () =>
				{
					var target = reference.Target;
					if (target != null)
					{
						method.Invoke(target, null);
					}
				};
			}
			else
				return null;
		}

		public static Action<T> MakeWeakAction<T>(Action<T> action)
		{
			if (action != null)
			{
				var reference = new WeakReference(action.Target);
				var method = action.GetInvocationList().FirstOrDefault().GetMethodInfo();
				return t =>
				{
					var target = reference.Target;
					if (target != null)
					{
						method.Invoke(target, new object[] { t });
					}
				};
			}
			else
				return null;
   		}

		public static Action<T1, T2> MakeWeakAction<T1, T2>(Action<T1, T2> action)
		{
			if (action != null)
			{
				var reference = new WeakReference(action.Target);
				var method = action.GetInvocationList().FirstOrDefault().GetMethodInfo();
				return (t1, t2) =>
				{
					var target = reference.Target;
					if (target != null)
					{
						method.Invoke(target, new object[] { t1, t2 });
					}
				};
			}
			else
				return null;
   		}

		public static Action<T1, T2, T3> MakeWeakAction<T1, T2, T3>(Action<T1, T2, T3> action)
		{
			if (action != null)
			{
				var reference = new WeakReference(action.Target);
				var method = action.GetInvocationList().FirstOrDefault().GetMethodInfo();
				return (t1, t2, t3) =>
				{
					var target = reference.Target;
					if (target != null)
					{
						method.Invoke(target, new object[] { t1, t2, t3 });
					}
				};
			}
			else
				return null;
		}

		public static Action<T1, T2, T3, T4> MakeWeakAction<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
		{
			if (action != null)
			{
				var reference = new WeakReference(action.Target);
				var method = action.GetInvocationList().FirstOrDefault().GetMethodInfo();
				return (t1, t2, t3, t4) =>
				{
					var target = reference.Target;
					if (target != null)
					{
						method.Invoke(target, new object[] { t1, t2, t3, t4});
					}
				};
			}
			else
				return null;
		}

		public static Func<K, V> MakeWeakAction<K, V>(Func<K, V> action)
		{
			if (action != null)
			{
				var reference = new WeakReference(action.Target);
				var method = action.GetInvocationList().FirstOrDefault().GetMethodInfo();
				return (k) =>
				{
					var target = reference.Target;
					if (target != null)
					{
						return (V)method.Invoke(target, new object[] { k });
					}
					return default(V);
				};
			}
			else
				return null;
		}

		public static EventHandler MakeWeakHandler(Action action)
		{
			if (action != null)
			{
				var reference = new WeakReference(action.Target);
				var method = action.GetInvocationList().FirstOrDefault().GetMethodInfo();
				return (sender, e) =>
				{
					var target = reference.Target;
					if (target != null)
					{
						method.Invoke(target, null);
					}
				};
			}
			else
				return null;
		}

		public static EventHandler MakeWeakHandler(EventHandler handler)
		{
			if (handler != null)
			{
				var reference = new WeakReference(handler.Target);
				var method = handler.GetInvocationList().FirstOrDefault().GetMethodInfo();
				return (sender, e) =>
				{
					var target = reference.Target;
					if (target != null)
					{
						method.Invoke(target, new object[] { sender, e });
					}
				};
			}
			else
				return null;
		}
	}
}
