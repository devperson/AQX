using System;
using UIKit;

using CoreAnimation;
using CoreGraphics;
using System.Linq;

using Aquamonix.Mobile.Lib.Utilities; 

namespace Aquamonix.Mobile.IOS
{
    /// <summary>
    /// Extension methods for UIView.
    /// </summary>
	public static class UIViewExtensions
	{
        /// <summary>
        /// Gets the view's immediate parent.
        /// </summary>
        /// <returns>The parent view, or null if view has none</returns>
		public static UIViewController GetParentViewController(this UIView view)
		{
			var nextResponder = view.NextResponder;
			if (nextResponder is UIViewController)
				return (UIViewController)nextResponder;
			else if (nextResponder is UIView)
				return GetParentViewController((UIView)nextResponder);
			return null;
		}

        /// <summary>
        /// Recursively finds in the view's child views, the first found view of the given type.
        /// </summary>
        /// <returns>The first found child view of the given type, or null.</returns>
		public static T FindFirstSubviewOfType<T>(this UIView view) where T : UIView
		{
			T output = null;

			if (view != null)
			{
				foreach (UIView subView in view.Subviews)
				{
					if (subView is T)
					{
						output = (T)subView;
						break;
					}
				}

				//still not found, go a level deeper
				if (output == null)
				{
					foreach (UIView subView in view.Subviews)
					{
						output = subView.FindFirstSubviewOfType<T>();
						if (output != null)
							break;
					}
				}
			}

			return output;
		}

        /// <summary>
        /// Rounds off the corners of the view.
        /// </summary>
        /// <param name="corners">Which corners to affect</param>
        /// <param name="cornerRadius">Radius of curve</param>
		public static void MakeRoundedCorners(this UIView view, UIRectCorner corners, float cornerRadius)
		{
			CAShapeLayer maskLayer = new CAShapeLayer();
			maskLayer.Path = UIBezierPath.FromRoundedRect(view.Bounds, corners, new CGSize(cornerRadius, cornerRadius)).CGPath;
			view.Layer.Mask = maskLayer;
		}

        /// <summary>
        /// Calculate the size of a view.
        /// </summary>
        /// <returns>The size of the view.</returns>
		public static CGSize GetSize(this UIView view)
		{
			return new CGSize(view.Frame.Width, view.Frame.Height);
		}

        /// <summary>
        /// Calculate the size of a view frame.
        /// </summary>
        /// <returns>The size of the view frame.</returns>
		public static CGSize GetSize(this CGRect frame)
		{
			return new CGSize(frame.Width, frame.Height);
		}

        /// <summary>
        /// Set only the X coordinate of the view.
        /// </summary>
        /// <param name="x">The value to set.</param>
		public static void SetFrameX(this UIView view, nfloat x)
		{
			CGRect frame = view.Frame;
			frame.X = x;
			view.Frame = frame;
		}

        /// <summary>
        /// Set only the Y coordinate of the view.
        /// </summary>
        /// <param name="y">The value to set.</param>
		public static void SetFrameY(this UIView view, nfloat y)
		{
			CGRect frame = view.Frame;
			frame.Y = y;
			view.Frame = frame;
		}

        /// <summary>
        /// Sets only the width component of the frame size.
        /// </summary>
        /// <param name="width">The width component value</param>
		public static void SetFrameWidth(this UIView view, nfloat width)
		{
			CGRect frame = view.Frame;
			frame.Width = width;
			view.Frame = frame;
		}

        /// <summary>
        /// Sets only the height component of the frame size.
        /// </summary>
        /// <param name="height">The height component value</param>
		public static void SetFrameHeight(this UIView view, nfloat height)
		{
			CGRect frame = view.Frame;
			frame.Height = height;
			view.Frame = frame;
		}

        /// <summary>
        /// Sets the location of the view, as a point.
        /// </summary>
        /// <param name="point">The location to set.</param>
		public static void SetFrameLocation(this UIView view, CGPoint point)
		{
			CGRect frame = view.Frame;
			frame.Location = point;
			view.Frame = frame;
		}

        /// <summary>
        /// Sets the location of the view frame, by x and y coordinate.
        /// </summary>
        /// <param name="x">The x coordinate of location to set.</param>
        /// <param name="y">The y coordinate of location to set.</param>
		public static void SetFrameLocation(this UIView view, nfloat x, nfloat y)
		{
			SetFrameLocation(view, new CGPoint(x, y)); 
		}

        /// <summary>
        /// Sets the frame size of the view.
        /// </summary>
        /// <param name="size">Size to set</param>
		public static void SetFrameSize(this UIView view, CGSize size)
		{
			CGRect frame = view.Frame;
			frame.Size = size;
			view.Frame = frame;
		}

        /// <summary>
        /// Sets the frame size of the view.
        /// </summary>
        /// <param name="height">Desired height to set</param>
        /// <param name="width">Desired width to set</param>
		public static void SetFrameSize(this UIView view, nfloat width, nfloat height)
		{
			SetFrameSize(view, new CGSize(width, height));
		}

        /// <summary>
        /// Adds a drop shadow with the specified properties.
        /// </summary>
        /// <param name="offset">Offset size</param>
        /// <param name="width">Width of drop shadow</param>
        /// <param name="opacity"></param>
        /// <param name="color"></param>
		public static void AddDropShadow(this UIView view, CGSize offset, float width = 0.3f, float opacity = 0.3f, UIColor color = null)
		{
			if (color == null)
				color = UIColor.Black;

			UIBezierPath shadowPath = UIBezierPath.FromRect(view.Bounds);
			view.Layer.MasksToBounds = false;
			view.Layer.ShadowColor = color.CGColor;
			view.Layer.ShadowOffset = offset;
			view.Layer.ShadowOpacity = opacity;
			view.Layer.ShadowPath = shadowPath.CGPath;
		}

        /// <summary>
        /// Removes a drop shadow from a view.
        /// </summary>
		public static void RemoveDropShadow(this UIView view)
		{
			view.Layer.ShadowColor = UIColor.Clear.CGColor;
			view.Layer.ShadowOpacity = 0f;
		}

        /// <summary>
        /// Sets the frame size of the view to 0,0.
        /// </summary>
		public static void ZeroFrame(this UIView view)
		{
			view.Frame = new CGRect(0, 0, 0, 0);
		}

        /// <summary>
        /// Iterates back through the view's superviews until it finds one of the given type.
        /// </summary>
        /// <param name="t">The type of parent view we're looking for.</param>
        /// <returns>The first ancestor of given type, or null.</returns>
		public static UIView GetAncestorOfType(this UIView view, Type t)
		{
			while (view.Superview != null && view.Superview.GetType() != t)
			{
				view = view.Superview;
			}
			return view.Superview;
		}

        /// <summary>
        /// Returns the top-level view of the view heirarchy to which the view belongs.
        /// </summary>
        /// <returns>The top-level view in the heirarchy.</returns>
		public static UIView GetOutermostView(this UIView view)
		{
			UIView output = view;

			while (output.Superview != null)
			{
				output = output.Superview;
			}

			return output;
		}

        /// <summary>
        /// For debugging purposes, sets a prominent red border around the view.
        /// </summary>
		public static void SetDebugBorder(this UIView view)
		{
			#if DEBUG
			view.Layer.BorderWidth = 1;
			view.Layer.BorderColor = UIColor.Blue.CGColor;
			#endif
		}

		public static void CenterVerticallyInParent(this UIView view)
		{
			if (view != null && view.Superview != null)
				view.SetFrameY(view.Superview.Frame.Height / 2 - view.Frame.Height / 2);
		}

		public static void CenterHorizontallyInParent(this UIView view)
		{
			if (view != null && view.Superview != null)
				view.SetFrameX(view.Superview.Frame.Width / 2 - view.Frame.Width / 2);
		}

		public static void CenterInParent(this UIView view)
		{
			view.CenterVerticallyInParent();
			view.CenterHorizontallyInParent();
		}

		public static void AlignToRightOfParent(this UIView view, int offsetFromRight = 0)
		{
			if (view != null && view.Superview != null)
				view.SetFrameX(view.Superview.Frame.Width - view.Frame.Width - offsetFromRight);
		}

		public static void AlignToBottomOfParent(this UIView view, int offsetFromBottom = 0)
		{
			if (view != null && view.Superview != null)
				view.SetFrameY(view.Superview.Frame.Bottom - view.Frame.Height - offsetFromBottom);
		}

		public static void SetHeightToContent(this UITableView tableView, int padding = 0, bool disableScrolling = true)
		{
            System.Diagnostics.Debug.WriteLine(tableView.GetType().ToString() + " content height is " + tableView.ContentSize.Height.ToString() + ", frame height is " + tableView.Frame.Height.ToString());
            if (tableView.ContentSize.Height < tableView.Frame.Height)
            {
                tableView.SetFrameHeight(tableView.ContentSize.Height + padding);
                if (disableScrolling)
                    tableView.ScrollEnabled = false;
            }
            else
            {
                tableView.ScrollEnabled = true;
            }
		}

		public static void DisposeEx(this UIView view)
		{
			ExceptionUtility.Try(() =>
			{
				if (view == null)
					return;

				if (view.Handle == IntPtr.Zero)
					return;

				var skipDispose = false;
				var desc = view.Description;

				if (view.Subviews != null)
				{
					foreach (var subView in view.Subviews)
					{
						ExceptionUtility.Try(() =>
						{
							subView.RemoveFromSuperview();
							subView.DisposeEx();
						});
					}
				}

				if (view is UIActivityIndicatorView)
				{
					var indicatorView = (UIActivityIndicatorView)view;
					if (indicatorView.IsAnimating)
					{
						indicatorView.StopAnimating();
					}

				}
				else if (view is UITableView)
				{
					var tableView = (UITableView)view;
					if (tableView.DataSource != null)
					{
						tableView.DataSource.Dispose();
					}
					tableView.Source = null;
					tableView.Delegate = null;
					tableView.DataSource = null;
					tableView.WeakDelegate = null;
					tableView.WeakDataSource = null;
					foreach (var cell in (tableView.VisibleCells ?? new UITableViewCell[0]))
					{
						cell.DisposeEx();
					}
				}
				else if (view is UICollectionView)
				{
					skipDispose = true; // UICollectionViewController will throw if we dispose it before it
					var collectionView = (UICollectionView)view;
					if (collectionView.DataSource != null)
					{
						collectionView.DataSource.Dispose();
					}
					collectionView.Source = null;
					collectionView.Delegate = null;
					collectionView.DataSource = null;
					collectionView.WeakDelegate = null;
					collectionView.WeakDataSource = null;
					foreach (var cell in (collectionView.VisibleCells ?? new UICollectionViewCell[0]))
					{
						cell.DisposeEx();
					}
				}
				else if (view is UIWebView)
				{
					var webView = (UIWebView)view;
					if (webView.IsLoading)
						webView.StopLoading();
					webView.LoadHtmlString(string.Empty, null); // clear display
					webView.Delegate = null;
					webView.WeakDelegate = null;
				}
				else if (view is UIScrollView)
				{
					var scrollView = view as UIScrollView;
					if (scrollView != null)
					{
						//scrollView.UnsetZoomableContentViewIfApplicable();
					}
				}

				// the below throws intermittently, investigate further since hard-cycles occur with GR's
				/*var gestures = view.GestureRecognizers;
                if (gestures != null) {
                    foreach (var gr in gestures) {
                        view.RemoveGestureRecognizer(gr);
                        gr.Dispose();
                    }
                }*/

				var constraints = view.Constraints;
				if (constraints != null && constraints.Any() && constraints.All(c => c.Handle != IntPtr.Zero))
				{
					view.RemoveConstraints(constraints);
					foreach (var c in constraints)
						c.Dispose();
					//constraints.ForEach(c => c.Dispose());
				}
				if (view.Layer != null)
				{
					view.Layer.RemoveAllAnimations();
				}

				if (!skipDispose)
				{
					view.Dispose();
				}
			});
		}
	}
}

