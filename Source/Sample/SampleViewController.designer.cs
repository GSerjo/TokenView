// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Sample
{
	[Register ("SampleViewController")]
	partial class SampleViewController
	{
		[Outlet]
		MonoTouch.UIKit.UITableView tokens { get; set; }

		[Outlet]
		NSTokenView.TokenView tokenView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (tokens != null) {
				tokens.Dispose ();
				tokens = null;
			}

			if (tokenView != null) {
				tokenView.Dispose ();
				tokenView = null;
			}
		}
	}
}
