using System;

namespace NSTokenView
{
	public class TokenViewDelegate
	{
		public virtual void FilterToken (TokenView tokenField, string text)
		{
		}

		public virtual void DidDeleteTokenAtIndex(TokenView tokenField, int index)
		{
		}

		public virtual void DidEnterToken(TokenView tokenField, string text)
		{
		}
	}
}