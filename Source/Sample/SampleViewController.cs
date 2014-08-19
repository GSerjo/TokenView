using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using NSTokenView;

namespace Sample
{
	public partial class SampleViewController : UIViewController
	{
		public SampleViewController (IntPtr handle) : base (handle)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad (); 

			tokenField.TokenDataSource = new TokenViewDataSource ();

			tokenField.SetupInit ();
			tokenField.Layer.CornerRadius = 5;
			tokenField.Layer.BorderColor = UIColor.LightTextColor.CGColor;
			tokenField.PlaceholderText = "Enter a token";

			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}

		#endregion

		public class TokenViewDataSource : TokenDataSource
		{
			public override string GetToken (TokenView tokenField, int index)
			{
				return string.Empty;
			}

			public override int NumberOfTokens (TokenView tokenField)
			{
				return 0;
			}
		}

	}
}