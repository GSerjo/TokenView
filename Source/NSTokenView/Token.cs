using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace NSTokenView
{
    [Register("Token")]
    internal partial class Token : UIView
    {
        private UIColor _colorScheme;
        private bool _highlighted;

        public Token(IntPtr handle) : base(handle)
        {
        }

        public UIColor ColorScheme
        {
            get { return _colorScheme; }
            set
            {
                _colorScheme = value;
                titleLabel.TextColor = _colorScheme;
                Highlighted = false;
            }
        }

        public bool Highlighted
        {
            get { return _highlighted; }
            set
            {
                _highlighted = value;
                UIColor textColor = _highlighted ? UIColor.White : ColorScheme;
                UIColor backgroundColor = _highlighted ? ColorScheme : UIColor.Clear;
                titleLabel.TextColor = textColor;
                backgroundView.BackgroundColor = backgroundColor;
            }
        }

        public Action<Token> OnDidTapToken { get; set; }

        public void SetTitleText(string title)
        {
            titleLabel.Text = title;
            titleLabel.TextColor = ColorScheme;
            titleLabel.SizeToFit();
            Frame = new RectangleF(Frame.X, Frame.Y, titleLabel.Frame.Width + 3, Frame.Height);
            titleLabel.SizeToFit();
        }

        public void SetupInit()
        {
            backgroundView.Layer.CornerRadius = 5;
            ColorScheme = UIColor.Blue;
            titleLabel.TextColor = ColorScheme;
            var gesture = new UITapGestureRecognizer(DidTapToken);
            AddGestureRecognizer(gesture);
        }

        private void DidTapToken(UITapGestureRecognizer gesture)
        {
            if (OnDidTapToken != null)
            {
                OnDidTapToken(this);
            }
            Console.WriteLine("DidTapToken");
        }
    }
}
