using System;

using UIKit;
using Foundation;
using CoreGraphics;

using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.IOS.UI;

namespace Aquamonix.Mobile.IOS.Views
{
	public class AquamonixTextField : UITextField
	{
		public const int StandardHeight = 36;

		private int _leftIndent = 4;

		public bool DismissOnButtonClick
		{
			get { return this.TFDelegate.DismissOnButtonClick; }
			set { this.TFDelegate.DismissOnButtonClick = value; }
		}

		public int MaxLength
		{
			get { return this.TFDelegate.MaxLength; }
			set { this.TFDelegate.MaxLength = value; }
		}

		public int LeftIndent
		{
			get { return this._leftIndent; }
			set { this._leftIndent = value; }
		}

		public UIControl NextControl
		{
			get { return this.TFDelegate.NextControl; }
			set { this.TFDelegate.NextControl = value; }
		}

		public Func<string, string> ValidationFunction { get; set;}

		public Action<string> OnTextChanged
		{
			get { return this.TFDelegate.OnTextChanged; }
			set { this.TFDelegate.OnTextChanged = value; }
		}

		public Action OnDoneClicked
		{
			get { return this.TFDelegate.OnDoneClicked; }
			set { this.TFDelegate.OnDoneClicked = value; }
		}

		public Action OnGotFocus
		{
			get { return this.TFDelegate.OnGotFocus; }
			set { this.TFDelegate.OnGotFocus = value; }
		}

		public Action OnLostFocus
		{
			get { return this.TFDelegate.OnLostFocus; }
			set { this.TFDelegate.OnLostFocus = value; }
		}

		private TextFieldDelegate TFDelegate
		{
			get { return ((TextFieldDelegate)this.Delegate); }
			set { this.Delegate = value; }
		}

		public AquamonixTextField() : base()
		{
			this.Initialize();
		}

		public AquamonixTextField(IntPtr handle) : base(handle)
		{
			this.Initialize();
		}

		public AquamonixTextField(CGRect frame) : base(frame)
		{
			this.Initialize();
		}

		public override void LayoutSubviews()
		{
			ExceptionUtility.Try(() =>
			{
				base.LayoutSubviews();

				this.MakeRoundedCorners(UIRectCorner.AllCorners, 6);
			});
		}

		public bool Validate()
		{
			string errorMsg;
			return (this.Validate(out errorMsg)); 
		}

		public bool Validate(out string errorMsg)
		{
			errorMsg = null;

			if (this.ValidationFunction != null)
			{
				errorMsg = this.ValidationFunction(this.TFDelegate.TextForValidation != null ? this.TFDelegate.TextForValidation : this.Text);
			}

			return (errorMsg == null);
		}

		public void SetFontAndColor(FontWithColor fontWithColor)
		{
			ExceptionUtility.Try(() =>
			{
				this.Font = fontWithColor.Font;
				this.TextColor = fontWithColor.Color;
			});
		}

		public void DoDispose()
		{
			ExceptionUtility.Try(() =>
			{
				this.ValidationFunction = null;
				if (this.TFDelegate != null)
				{
					this.TFDelegate.Dispose();
					this.TFDelegate = null;
				}

				this.Dispose();
			});
		}

		private void Initialize()
		{
			ExceptionUtility.Try(() =>
			{
				this.Delegate = new TextFieldDelegate();
				this.BackgroundColor = UIColor.White;
			});
		}

		public override CGRect TextRect(CGRect forBounds)
		{
			return ExceptionUtility.Try<CGRect>(() =>
			{
				var orig = base.TextRect(forBounds);
				return new CGRect(_leftIndent, orig.Y, orig.Width, orig.Height);
			});
		}

		public override CGRect EditingRect(CGRect forBounds)
		{
			return ExceptionUtility.Try<CGRect>(() =>
			{
				var orig = base.TextRect(forBounds);
				return new CGRect(_leftIndent, orig.Y, orig.Width, orig.Height);
			});
		}

		private class TextFieldDelegate : UITextFieldDelegate
		{
			private Action<string> _onTextChanged;
			private Action _onDoneClicked;
			private Action _onGotFocus;
			private Action _onLostFocus;

			public string TextForValidation { get; set;}
			public bool DismissOnButtonClick { get; set;}
			public int MaxLength { get; set;}

			public Action<string> OnTextChanged
			{
				get { return _onTextChanged;}
				set
				{
					this._onTextChanged = WeakReferenceUtility.MakeWeakAction(value);
				}
			}
			public Action OnGotFocus
			{
				get { return _onGotFocus; }
				set
				{
					this._onGotFocus = WeakReferenceUtility.MakeWeakAction(value);
				}
			}
			public Action OnLostFocus
			{
				get { return _onLostFocus; }
				set
				{
					this._onLostFocus = WeakReferenceUtility.MakeWeakAction(value);
				}
			}
			public Action OnDoneClicked
			{
				get { return _onDoneClicked; }
				set
				{
					this._onDoneClicked = WeakReferenceUtility.MakeWeakAction(value);
				}
			}
			public UIControl NextControl { get; set;}

			public TextFieldDelegate() : base()
			{
			}

			public override bool ShouldReturn(UITextField textField)
			{
				ExceptionUtility.Try(() =>
				{
					if (textField.ReturnKeyType == UIReturnKeyType.Next)
					{
						if (this.NextControl != null)
							this.NextControl.BecomeFirstResponder();
					}

					else if (textField.ReturnKeyType == UIReturnKeyType.Done)
					{
						if (this.OnDoneClicked != null)
							this.OnDoneClicked();
					}

					if (this.DismissOnButtonClick)
						textField.ResignFirstResponder(); 
				});

				return false;
			}

			public override bool ShouldChangeCharacters(UITextField textField, NSRange range, string replacementString)
			{
				bool output = false;

				return ExceptionUtility.Try<bool>(() =>
				{
					bool maxLengthExceeded = false;

					if (this.MaxLength > 0)
					{
						int currentCharacterCount = textField.Text.Length;

						if (range.Length + range.Location > currentCharacterCount)
						{
							return false;
						}

						int newLength = currentCharacterCount + replacementString.Length - (int)range.Length;

						maxLengthExceeded = newLength > this.MaxLength;
					}

					if (maxLengthExceeded)
					{
						this.TextForValidation = textField.Text;
						output = false;
					}
					else {
						this.TextForValidation = ReplaceText(textField.Text, range, replacementString);
						output = true;
					}

					if (this.OnTextChanged != null)
						this.OnTextChanged(this.TextForValidation);

					this.TextForValidation = null;

					return output;
				});
			}

			public override bool ShouldBeginEditing(UITextField textField)
			{
				if (this.OnGotFocus != null)
					this.OnGotFocus();
				return true;
			}

			public override bool ShouldEndEditing(UITextField textField)
			{
				if (this.OnLostFocus != null)
					this.OnLostFocus();
				return true;
			}

			private string ReplaceText(string text, NSRange range, string replacementString)
			{
				string output = text;

				ExceptionUtility.Try(() =>
				{
					string start = text.Substring(0, (int)range.Location);

					int startIndex = (int)(range.Location + range.Length);
					string end = String.Empty;
					if (startIndex < text.Length)
						end = text.Substring(startIndex, text.Length - startIndex);

					output = start + replacementString + end;
				});

				return output;
			}
		}
	}
}
