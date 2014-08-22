using System;

namespace NSTokenView
{
	public class TokenViewSource
	{
		public virtual int NumberOfTokens(TokenView tokenView)
		{
			throw new NotImplementedException ("This method is required");
		}

		public virtual string GetToken(TokenView tokenView, int index)
		{
			throw new NotImplementedException ("This method is required");
		}
	}
}