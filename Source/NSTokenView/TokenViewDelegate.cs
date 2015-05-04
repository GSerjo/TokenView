using System;

namespace NSTokenView
{
    public class TokenViewDelegate
    {
        public virtual void DidDeleteTokenAtIndex(TokenView tokenView, int index)
        {
        }

        public virtual void DidEnterToken(TokenView tokenView, string text)
        {
        }

        public virtual void FilterToken(TokenView tokenView, string text)
        {
        }
    }
}
