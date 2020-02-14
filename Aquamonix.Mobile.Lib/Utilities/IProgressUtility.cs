using System;
using BigTed;

namespace Aquamonix.Mobile.Lib.Utilities
{
    /// <summary>
    /// Interface for showing platform-specific UI progress spinners to the user.
    /// </summary>
	public interface IProgressUtility
	{
        /// <summary>
        /// Show a progress spinner.
        /// </summary>
        /// <param name="progressMessage">Optional message to show with spinner.</param>
        /// <param name="blockUI">If true, all other UI elements will be blocked while spinner is showing.</param>
		void Show(string progressMessage, bool blockUI = true);

        /// <summary>
        /// Shows a spinner inside of a try/catch, such that it's guaranteed that the spinner will be dismissed 
        /// in the event of error.
        /// </summary>
        /// <param name="progressMessage">Optional message to show with spinner.</param>
        /// <param name="activity">Action to perform while spinner is shown.</param>
        /// <param name="blockUI">If true, all other UI elements will be blocked while spinner is showing.</param>
		void SafeShow(string progressMessage, Action activity, bool blockUI = true);

        /// <summary>
        /// Shows a spinner inside of a try/catch, such that it's guaranteed that the spinner will be dismissed 
        /// in the event of error.
        /// </summary>
        /// <param name="progressMessage">Optional message to show with spinner.</param>
        /// <param name="activity">Func to perform while spinner is shown.</param>
        /// <param name="blockUI">If true, all other UI elements will be blocked while spinner is showing.</param>
        /// <returns>The output of the activity Func</returns>
		T SafeShow<T>(string progressMessage, Func<T> activity, bool blockUI = true);

        /// <summary>
        /// Hide the currently shown spinner.
        /// </summary>
		void Dismiss();
	}
}
