using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Aquamonix.Mobile.Lib.Utilities
{
    /// <summary>
    /// Utility for invoking code on the UI (main) thread.
    /// </summary>
	public class MainThreadUtility
	{
		private static readonly Lazy<MainThreadUtility> _instance = new Lazy<MainThreadUtility>(() => new MainThreadUtility());

        /// <summary>
        /// Gets the current instance. 
        /// </summary>
		public static MainThreadUtility Instance
		{
			get
			{
				return _instance.Value;
			}
		}

        /// <summary>
        /// Call on app startup to specify what thread is the application's 'main' thread.
        /// </summary>
        /// <param name="mainThread">The main UI thread of app</param>
        /// <param name="invokeOnMainThreadHandler">Handler for invoking on main thread.</param>
        /// <param name="beginInvokeOnMainThreadHandler">Handler for async invoke on main thread.</param>
		public void SetMainThread(
			Thread mainThread,
			Action<Action> invokeOnMainThreadHandler,
			Action<Action> beginInvokeOnMainThreadHandler
			)
		{
			MainThread = mainThread;
			InvokeOnMainThreadHandler = invokeOnMainThreadHandler;
			BeginInvokeOnMainThreadHandler = beginInvokeOnMainThreadHandler;
		}

        /// <summary>
        /// Gets/sets a reference to the app main thread.
        /// </summary>
		public Thread MainThread { get; set; }

        /// <summary>
        /// Gets a boolean value indicating whether or not current thread is main thread (false if equal, true if not)
        /// </summary>
		public bool InvokeRequired
		{
			get { return Thread.CurrentThread != MainThread; }
		}

        public static bool IsOnMainThread
        {
            get { return !Instance.InvokeRequired; }
        }

		public Action<Action> BeginInvokeOnMainThreadHandler { get; set; }

		public Action<Action> InvokeOnMainThreadHandler { get; set; }

        /// <summary>
        /// Invokes an action synchronously on the main UI thread.
        /// </summary>
        /// <param name="action">Action to invoke.</param>
        /// <param name="handleErrors">If true, will wrap given action in a try/catch.</param>
		public void InvokeOnMainThread(Action action, bool handleErrors = true)
		{
			if (handleErrors)
				action = WrapActionInErrorHandler(action);
			
			if (!InvokeRequired || InvokeOnMainThreadHandler == null)
				action();
			else
				InvokeOnMainThreadHandler(action);
		}

        /// <summary>
        /// Invokes an action asynchronously on the main UI thread.
        /// </summary>
        /// <param name="action">Action to invoke.</param>
        /// <param name="blockIfAlreadyOnMainThread">If true, then the given action will be performed synchronously if and 
        /// only if current thread is the main thread.</param>
        /// <param name="handleErrors">If true, will wrap given action in a try/catch.</param>
		public void BeginInvokeOnMainThread(Action action, bool blockIfAlreadyOnMainThread = true, bool handleErrors = true)
		{
			if (handleErrors)
				action = WrapActionInErrorHandler(action);

			if (blockIfAlreadyOnMainThread)
			{
				//will block if handler is null, or we're already on main thread 
				if (!InvokeRequired || BeginInvokeOnMainThreadHandler == null)
					action();
				else
					BeginInvokeOnMainThreadHandler(action);
			}
			else
			{
				//will only block if the handler is null 
				if (BeginInvokeOnMainThreadHandler != null)
					BeginInvokeOnMainThreadHandler(action);
				else
					action();
			}
		}


        /// <summary>
        /// Invokes an action synchronously on the main UI thread.
        /// </summary>
        /// <param name="action">Action to invoke.</param>
		public static void InvokeOnMain(Action action)
		{
			_instance.Value.InvokeOnMainThread(action);
		}

        /// <summary>
        /// Invokes an action asynchronously on the main UI thread.
        /// </summary>
        /// <param name="action">Action to invoke.</param>
        /// <param name="blockIfAlreadyOnMainThread">If true, then the given action will be performed synchronously if and 
        /// only if current thread is the main thread.</param>
		public static void BeginInvokeOnMain(Action action, bool blockIfAlreadyOnMainThread = true)
		{
			_instance.Value.BeginInvokeOnMainThread(action, blockIfAlreadyOnMainThread);
		}

		private Action WrapActionInErrorHandler(Action action)
		{
			return () =>
			{
				//ExceptionUtility.Try(() => action);
				try
				{
					action();
				}
				catch (Exception e)
				{
					LogUtility.LogException(e);
				}
			};
		}
	}
}
