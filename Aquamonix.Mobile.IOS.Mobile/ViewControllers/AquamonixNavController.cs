using System;
using System.Collections.Generic;

using Aquamonix.Mobile.Lib.Utilities;

using UIKit;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
	public class AquamonixNavController : UINavigationController
	{
		//private Stack<UIViewController> _vcStack = new System.Collections.Generic.Stack<UIViewController>();

		public AquamonixNavController(UIViewController rootViewController) : base(rootViewController)
		{
		}

		public override void PresentViewController(UIViewController viewControllerToPresent, bool animated, Action completionHandler)
		{
			ExceptionUtility.Try(() =>
			{
				base.PresentViewController(viewControllerToPresent, animated, completionHandler);

				if (viewControllerToPresent is TopLevelViewControllerBase)
				{
					((TopLevelViewControllerBase)viewControllerToPresent).Predecessor = TopLevelViewControllerBase.CurrentViewController;
					TopLevelViewControllerBase.CurrentViewController = viewControllerToPresent as TopLevelViewControllerBase;
				}
			});
		}

		public override void PushViewController(UIViewController viewController, bool animated)
		{
			ExceptionUtility.Try(() =>
			{
				base.PushViewController(viewController, animated);
				//CurrentViewController = viewController;

				if (viewController is TopLevelViewControllerBase)
				{
					((TopLevelViewControllerBase)viewController).Predecessor = TopLevelViewControllerBase.CurrentViewController;
					TopLevelViewControllerBase.CurrentViewController = viewController as TopLevelViewControllerBase;
				}
			});
		}

        public override UIViewController PopViewController(bool animated)
		{
            return base.PopViewController(animated);
		}


    }
   
}
 