using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquamonix.Mobile.Lib.Extensions 
{
	/// <summary>
	/// Class containing helper extension methods
	/// </summary>
	public static class TaskExtensions {

		/// <summary>
		/// Attaches a continuation on a task on the UI Thread
		/// </summary>
		/// <param Name="task"></param>
		/// <param Name="callback"></param>
		/// <returns></returns>
		public static Task ContinueOnCurrentThread (this Task task, Action<Task> callback)
		{
			#if NCRUNCH
			return task.ContinueWith (callback);
			#else
			return task.ContinueWith (callback, TaskScheduler.FromCurrentSynchronizationContext ());
			#endif
		}

		/// <summary>
		/// Attaches a continuation on a task on the UI Thread
		/// </summary>
		/// <typeparam Name="T"></typeparam>
		/// <param Name="task"></param>
		/// <param Name="callback"></param>
		/// <returns></returns>
		public static Task<T> ContinueOnCurrentThread<T> (this Task<T> task, Func<Task<T>, T> callback)
		{
			#if NCRUNCH
			return task.ContinueWith<T> (callback);
			#else
			return task.ContinueWith<T> (callback, TaskScheduler.FromCurrentSynchronizationContext ());
			#endif
		}

		/// <summary>
		/// A quick helper to be able to chain 2 tasks together
		/// </summary>
		public static Task ContinueWith (this Task task, Task continuation)
		{
			return task.ContinueWith (t => continuation).Unwrap ();
		}

		/// <summary>
		/// A quick helper to be able to chain 2 tasks together
		/// </summary>
		public static Task<T> ContinueWith<T> (this Task task, Task<T> continuation)
		{
			return task.ContinueWith (t => continuation).Unwrap ();
		}
	}
}
