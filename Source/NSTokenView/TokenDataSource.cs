using System;

namespace NSTokenView
{
	public class TokenDataSource
	{
		public virtual int NumberOfTokens(TokenView tokenField)
		{
			throw new NotImplementedException ();
		}

		public virtual string GetToken(TokenView tokenField, int index)
		{
			throw new NotImplementedException ();
		}
	}
}