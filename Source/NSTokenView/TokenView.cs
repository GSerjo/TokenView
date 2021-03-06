﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;

namespace NSTokenView
{
    [Register("TokenView")]
    public sealed class TokenView : UIView, IRemovableTextField
    {
        private const float DefaultHorizontalInset = 15.0f;
        private const float DefaultMaxHeight = 150.0f;
        private const float DefaultMinImputWidth = 80.0f;
        private const float DefaultToLabelPadding = 5.0f;
        private const float DefaultTokenPadding = 5.0f;
        private const float DefaultVerticalInset = 7.0f;
        private const float HeightForToken = 30.0f;
        private readonly List<Token> _tokens = new List<Token>();

        private UIColor _colorScheme;
        private float _horizontalInset;
        private BackspaceTextField _inputTextField;
        private UIKeyboardType _inputTextFieldKeyboardType;
        private UIColor _inputTextFieldTextColor;
        private BackspaceTextField _invisibleTextField;
        private float _maxHeight;
        private float _minInputWidth;
        private float _orifinalHeight;
        private string _placeholderText;
        private UIScrollView _scrollView;

        private TokenViewSource _tokenDataSource = new TokenViewSource();
        private TokenViewDelegate _tokenDelegate = new TokenViewDelegate();
        private float _tokenPadding;
        private float _verticalInset;

        public TokenView(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithFrame:")]
        public TokenView(RectangleF frame) : base(frame)
        {
        }

        public UIColor ColorScheme
        {
            get { return _colorScheme; }
            set
            {
                _colorScheme = value;
                InputTextField.TintColor = _colorScheme;
                foreach (Token token in _tokens)
                {
                    token.ColorScheme = _colorScheme;
                }
            }
        }

        public UIKeyboardType InputTextFieldKeyboardType
        {
            get { return _inputTextFieldKeyboardType; }
            set
            {
                _inputTextFieldKeyboardType = value;
                InputTextField.KeyboardType = _inputTextFieldKeyboardType;
            }
        }

        public UIColor InputTextFieldTextColor
        {
            get { return _inputTextFieldTextColor; }
            set
            {
                _inputTextFieldTextColor = value;
                InputTextField.TextColor = _inputTextFieldTextColor;
            }
        }

        public string PlaceholderText
        {
            get { return _placeholderText; }
            set
            {
                _placeholderText = value;
                InputTextField.Placeholder = _placeholderText;
            }
        }

        public TokenViewSource TokenDataSource
        {
            get { return _tokenDataSource; }
            set { _tokenDataSource = value; }
        }

        public TokenViewDelegate TokenDelegate
        {
            get { return _tokenDelegate; }
            set { _tokenDelegate = value; }
        }

        private BackspaceTextField InputTextField
        {
            get
            {
                if (_inputTextField == null)
                {
                    _inputTextField = new BackspaceTextField(RectangleF.Empty, this)
                    {
                        Delegate = new BackspaceDelegate(this),
                        KeyboardType = InputTextFieldKeyboardType,
                        TextColor = InputTextFieldTextColor,
                        Font = UIFont.FromName("HelveticaNeue", 15.5f),
                        AutocorrectionType = UITextAutocorrectionType.No,
                        TintColor = ColorScheme,
                        Placeholder = PlaceholderText
                    };
                    _inputTextField.AddTarget(InputTextFieldDidChange, UIControlEvent.EditingChanged);
                }
                return _inputTextField;
            }
        }

        public override bool BecomeFirstResponder()
        {
            ReloadData();

            InputTextFieldBecomeFirstResponder();
            return true;
        }

        public void ReloadData()
        {
            ClearTokens();

            float currentX = 0;
            float currentY = 0;

            LayoutTokensWithCurrentX(ref currentX, ref currentY);
            LayoutInputTextFieldWithCurrentX(ref currentX, ref currentY);

            AdjustHeightForCurrentY(currentY);

            _scrollView.ContentSize = new SizeF(_scrollView.ContentSize.Width, currentY + HeightForToken);

            UpdateInputTextField();

            bool inputFieldShouldBecomeFirstResponder = InputTextField.IsFirstResponder;
            if (inputFieldShouldBecomeFirstResponder)
            {
                InputTextFieldBecomeFirstResponder();
            }
            else
            {
                FocusInputTextField();
            }
            InputTextFieldBecomeFirstResponder();
        }

        public override bool ResignFirstResponder()
        {
            return InputTextField.ResignFirstResponder();
        }

        public void SetupInit()
        {
            _maxHeight = DefaultMaxHeight;
            _verticalInset = DefaultVerticalInset;
            _horizontalInset = DefaultHorizontalInset;
            _tokenPadding = DefaultTokenPadding;
            _minInputWidth = DefaultMinImputWidth;
            ColorScheme = UIColor.Blue;
            InputTextFieldTextColor = new UIColor(38 / 255.0f, 39 / 255.0f, 41 / 255.0f, 1.0f);

            _orifinalHeight = Frame.Height;
            LayoutInvisibleTextField();
            LayoutScrollView();

            ReloadData();
        }

        void IRemovableTextField.TextFieldDidEnterBackspace(BackspaceTextField textField)
        {
            bool removeToken = false;
            for (int index = 0; index < _tokens.Count; index++)
            {
                if (_tokens[index].Highlighted)
                {
                    TokenDelegate.DidDeleteTokenAtIndex(this, index);
                    removeToken = true;
                    break;
                }
            }
            if (removeToken == false && _tokens.Count > 0)
            {
                Token token = _tokens.Last();
                token.Highlighted = true;
            }
            SetCursorVisibility();
        }

        internal void DidTapToken(Token token)
        {
            foreach (Token item in _tokens)
            {
                if (item == token)
                {
                    item.Highlighted = !item.Highlighted;
                }
                else
                {
                    item.Highlighted = false;
                }
            }
            SetCursorVisibility();
        }

        private void AdjustHeightForCurrentY(float currentY)
        {
            float height;
            if (currentY + HeightForToken > Frame.Height)
            {
                if (currentY + HeightForToken <= _maxHeight)
                {
                    height = currentY + HeightForToken + _verticalInset * 2;
                }
                else
                {
                    height = _maxHeight;
                }
            }
            else
            {
                if (currentY + HeightForToken > _orifinalHeight)
                {
                    height = currentY + HeightForToken + _verticalInset * 2;
                }
                else
                {
                    height = _orifinalHeight;
                }
            }
            Frame = new RectangleF(Frame.X, Frame.Y, Frame.Width, height);
        }

        private void ClearTokens()
        {
            List<UIView> removeSubviews = _scrollView.Subviews.ToList();
            foreach (UIView view in removeSubviews)
            {
                var token = view as Token;
                if (token != null)
                {
                    token.RemoveFromSuperview();
                }
            }

            _scrollView.Hidden = false;
            _tokens.Clear();
        }

        private void FocusInputTextField()
        {
            PointF contentOffset = _scrollView.ContentOffset;
            float targetY = InputTextField.Frame.Y + HeightForToken - _maxHeight;
            if (targetY > contentOffset.Y)
            {
                _scrollView.SetContentOffset(new PointF(contentOffset.X, targetY), false);
            }
        }

        private void HandleSingleTap(UITapGestureRecognizer gestureRecognizer)
        {
            BecomeFirstResponder();
        }

        private string InputText()
        {
            return InputTextField.Text;
        }

        private void InputTextFieldBecomeFirstResponder()
        {
            if (InputTextField.IsFirstResponder)
            {
                return;
            }
            InputTextField.BecomeFirstResponder();
        }

        private void InputTextFieldDidChange(object sender, EventArgs ea)
        {
            TokenDelegate.FilterToken(this, InputText());
        }

        private void LayoutInputTextFieldWithCurrentX(ref float currentX, ref float currentY)
        {
            float inputTextFieldWidth = _scrollView.ContentSize.Width - currentX;
            if (inputTextFieldWidth < _minInputWidth)
            {
                inputTextFieldWidth = _scrollView.ContentSize.Width;
                currentY += HeightForToken;
                currentX = 0;
            }
            InputTextField.Text = string.Empty;
            InputTextField.Frame = new RectangleF(currentX, currentY + 1, inputTextFieldWidth, HeightForToken - 1);
            InputTextField.TintColor = ColorScheme;
            _scrollView.AddSubview(InputTextField);
        }

        private void LayoutInvisibleTextField()
        {
            _invisibleTextField = new BackspaceTextField(RectangleF.Empty, this);
            _invisibleTextField.Delegate = new BackspaceDelegate(this);
            AddSubview(_invisibleTextField);
        }

        private void LayoutScrollView()
        {
            _scrollView = new UIScrollView(new RectangleF(0, 0, Frame.Width, Frame.Height));
            _scrollView.ContentSize = new SizeF(Frame.Width - _horizontalInset * 2, Frame.Height - _verticalInset * 2);
            _scrollView.ContentInset = new UIEdgeInsets(_verticalInset, _horizontalInset, _verticalInset, _horizontalInset);
            _scrollView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
            AddSubview(_scrollView);
        }

        private void LayoutTokensWithCurrentX(ref float currentX, ref float currentY)
        {
            for (int i = 0; i < NumberOfTokens(); i++)
            {
                string title = TitleForTokenAtIndex(i);

                NSArray nibObjects = NSBundle.MainBundle.LoadNib("Token", this, null);
                var token = (Token)Runtime.GetNSObject(nibObjects.ValueAt(0));
                token.SetupInit();
                token.ColorScheme = ColorScheme;
                token.OnDidTapToken = DidTapToken;
                token.SetTitleText(title);
                if (currentX + token.Frame.Width <= _scrollView.Frame.Width)
                {
                    token.Frame = new RectangleF(currentX, currentY, token.Frame.Width, token.Frame.Height);
                }
                else
                {
                    currentY += token.Frame.Height;
                    currentX = 0;
                    float tokenWidth = token.Frame.Width;
                    if (tokenWidth > _scrollView.ContentSize.Width)
                    {
                        tokenWidth = _scrollView.ContentSize.Width;
                    }
                    token.Frame = new RectangleF(currentX, currentY, tokenWidth, token.Frame.Height);
                }
                currentX += token.Frame.Width + _tokenPadding;
                _scrollView.AddSubview(token);
                _tokens.Add(token);
            }
        }

        private int NumberOfTokens()
        {
            return TokenDataSource.NumberOfTokens(this);
        }

        private void SetCursorVisibility()
        {
            List<Token> highlightedTokens = _tokens.Where(x => x.Highlighted).ToList();
            if (highlightedTokens.Count == 0)
            {
                InputTextFieldBecomeFirstResponder();
            }
            else
            {
                _invisibleTextField.BecomeFirstResponder();
            }
        }

        private string TitleForTokenAtIndex(int index)
        {
            return TokenDataSource.GetToken(this, index);
        }

        private void UnhighlightAllTokens()
        {
            foreach (Token token in _tokens)
            {
                token.Highlighted = false;
            }
            SetCursorVisibility();
        }

        private void UpdateInputTextField()
        {
            InputTextField.Placeholder = _tokens.Count != 0 ? string.Empty : PlaceholderText;
        }


        private sealed class BackspaceDelegate : UITextFieldDelegate
        {
            private readonly TokenView _tokenView;

            public BackspaceDelegate(TokenView tokenView)
            {
                _tokenView = tokenView;
            }

            public override void EditingStarted(UITextField textField)
            {
                if (textField == _tokenView.InputTextField)
                {
                    _tokenView.UnhighlightAllTokens();
                }
            }

            public override bool ShouldChangeCharacters(UITextField textField, NSRange range, string replacementString)
            {
                _tokenView.UnhighlightAllTokens();
                return true;
            }

            public override bool ShouldReturn(UITextField textField)
            {
                if (!string.IsNullOrWhiteSpace(textField.Text))
                {
                    _tokenView.TokenDelegate.DidEnterToken(_tokenView, textField.Text);
                }
                return false;
            }
        }
    }
}
