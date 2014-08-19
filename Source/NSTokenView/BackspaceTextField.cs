using System;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using System.Drawing;
using MonoTouch.Foundation;

namespace NSTokenView
{
	internal class BackspaceTextField : UITextField
	{
		[Export("initWithFrame:")]
		public BackspaceTextField(RectangleF frame) : base(frame)
		{

		}

		public override void DeleteBackward ()
		{
			Console.WriteLine ("DeleteBackward");
			if (Text.Length == 0)
			{ 
			}
			base.DeleteBackward ();
		}
	}
}