using System;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using System.Drawing;
using MonoTouch.Foundation;

namespace NSTokenView
{
	internal class BackspaceTextField : UITextField
	{
		private readonly IRemovableTextField _removableProtocol;

		public BackspaceTextField(RectangleF frame, IRemovableTextField protocol) : base(frame)
		{
			_removableProtocol = protocol;
		}

		public override void DeleteBackward ()
		{
			if (Text.Length == 0)
			{ 
				_removableProtocol.TextFieldDidEnterBackspace (this);
			}
			base.DeleteBackward ();
		}
	}
}