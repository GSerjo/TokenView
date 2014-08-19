using System;

namespace NSTokenView
{
	public class TokenDelegate
	{
		public virtual void FilterToken (TokenView tokenField, string text)
		{
		}

		public virtual void DidDeleteTokenAtIndex(TokenView tokenField, int index)
		{
		}
	}
}