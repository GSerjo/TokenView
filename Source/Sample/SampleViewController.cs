using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using NSTokenView;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.CoreFoundation;

namespace Sample
{
	public partial class SampleViewController : UIViewController
	{
		private List<string> _nameSource = new List<string>();
		private List<string> _tokenSource = new List<string>();
		private NameSelectorDelegate _nameSelectorDelegate;
		private NameSelectorSource _nameSelectorSource;
		private TableSource _tableSource;

		public SampleViewController (IntPtr handle) : base (handle)
		{
			_tableSource = new TableSource (this);
			_nameSelectorSource = new NameSelectorSource (this);
			_nameSelectorDelegate = new NameSelectorDelegate (this);
		}

		public override void ViewDidLoad ()
		{
			InitTokenSource ();
			InitTokenView ();
			tokens.Source = _tableSource;

			base.ViewDidLoad (); 
		}

		private void InitTokenView()
		{
			tokenView.TokenDataSource = _nameSelectorSource;
			tokenView.TokenDelegate = _nameSelectorDelegate;

			tokenView.SetupInit ();
			tokenView.Layer.CornerRadius = 5;
			tokenView.Layer.BorderColor = UIColor.LightTextColor.CGColor;
			tokenView.PlaceholderText = "Enter a token";
			tokenView.ColorScheme = new UIColor (62 / 255.0f, 149 / 255.0f, 206 / 255.0f, 1.0f);
		}

		private void InitTokenSource()
		{
			for (int i = 0; i < 30; i++)
			{
				_nameSource.Add (string.Format ("Token: {0}", i));
			}
		}

		private void ReloadData()
		{
			DispatchQueue.MainQueue.DispatchAsync (() => tokens.ReloadData ());
		}

		private sealed class TableSource : UITableViewSource
		{
			private string cellId = "TableCell";
			private List<string> _nameSource;
			private List<string> _initialNameSource;
			private List<string> _tokenSource;
			private readonly SampleViewController _controller;

			public TableSource (SampleViewController controller)
			{
				_nameSource = controller._nameSource;
				_initialNameSource = controller._nameSource;
				_tokenSource = controller._tokenSource;
				_controller = controller;
			}

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				var item = _nameSource [indexPath.Item];
				_nameSource.Remove (item);
				_initialNameSource.Remove (item);
				_tokenSource.Add (item);
				_controller.tokenView.ReloadData ();
				_controller.ReloadData ();
			}

			public void ResetFilter ()
			{
				_nameSource = _initialNameSource;
				_controller.ReloadData ();
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell = tableView.DequeueReusableCell (cellId);
				if (cell == null)
				{
					cell = new UITableViewCell (UITableViewCellStyle.Default, cellId);
				}
				cell.TextLabel.Text = _nameSource [indexPath.Row];
				return cell;
			}

			public override int RowsInSection (UITableView tableview, int section)
			{
				return _nameSource.Count;
			}

			public void Filter(string text)
			{
				if (string.IsNullOrWhiteSpace (text))
				{
					_nameSource = _initialNameSource;
				} 
				else
				{
					_nameSource = _initialNameSource.Where (x => x.Contains(text)).ToList();
				}
				_controller.ReloadData ();
			}
		}

		private sealed class  NameSelectorDelegate : TokenViewDelegate
		{

			private readonly TableSource _tableSource;
			private List<string> _tokenSource;
			private SampleViewController _controller;

			public NameSelectorDelegate (SampleViewController controller)
			{
				_tableSource = controller._tableSource;
				_tokenSource = controller._tokenSource;
				_controller = controller;
			}

			public override void FilterToken (TokenView tokenView, string text)
			{
				_tableSource.Filter (text);
			}

			public override void DidEnterToken (TokenView tokenView, string text)
			{
				_tokenSource.Add (text);
				_tableSource.ResetFilter ();
				_controller.tokenView.ReloadData();
			}

			public override void DidDeleteTokenAtIndex (TokenView tokenView, int index)
			{
				_tokenSource.RemoveAt (index);
				tokenView.ReloadData ();
			}
		}

		private sealed class NameSelectorSource : TokenViewSource
		{

			private List<string> _tokenSource;

			public NameSelectorSource (SampleViewController controller)
			{
				_tokenSource = controller._tokenSource;
			}

			public override string GetToken (TokenView tokenView, int index)
			{
				return _tokenSource[index];
			}

			public override int NumberOfTokens (TokenView tokenView)
			{
				return _tokenSource.Count;
			}
		}
	}
}