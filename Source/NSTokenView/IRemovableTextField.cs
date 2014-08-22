using System;

namespace NSTokenView
{
	internal interface IRemovableTextField
	{
		void TextFieldDidEnterBackspace(BackspaceTextField textField);
	}
}