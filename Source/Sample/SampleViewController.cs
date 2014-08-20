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
		private List<string> _tokenSource = new List<string>();

		public SampleViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			InitTokenSource ();
			var tokenSource = new TableSource (this);
			InitTokenView (tokenSource);
			tokens.Source = tokenSource;

			base.ViewDidLoad (); 
		}

		private void InitTokenView(TableSource tokenSource)
		{
			tokenView.TokenDataSource = new NameSelectorSource ();
			tokenView.TokenDelegate = new NameSelectorDelegate (tokenSource);

			tokenView.SetupInit ();
			tokenView.Layer.CornerRadius = 5;
			tokenView.Layer.BorderColor = UIColor.LightTextColor.CGColor;
			tokenView.PlaceholderText = "Enter a token";
		}

		private void InitTokenSource()
		{
			for (int i = 0; i < 30; i++)
			{
				_tokenSource.Add (string.Format ("Token: {0}", i));
			}
		}

		private sealed class TableSource : UITableViewSource
		{
			private string cellId = "TableCell";
			private List<string> _source;
			private List<string> _initialSource;
			private readonly SampleViewController _controller;

			public TableSource (SampleViewController controller)
			{
				_source = controller._tokenSource;
				_initialSource = controller._tokenSource;
				_controller = controller;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell = tableView.DequeueReusableCell (cellId);
				if (cell == null)
				{
					cell = new UITableViewCell (UITableViewCellStyle.Default, cellId);
				}
				cell.TextLabel.Text = _source [indexPath.Row];
				return cell;
			}

			public override int RowsInSection (UITableView tableview, int section)
			{
				return _source.Count;
			}

			public void Filter(string text)
			{
				if (string.IsNullOrWhiteSpace (text))
				{
					_source = _initialSource;
				} 
				else
				{
					_source = _initialSource.Where (x => x.Contains(text)).ToList();
				}
				DispatchQueue.MainQueue.DispatchAsync (() => _controller.tokens.ReloadData ());
			}
		}

		private sealed class  NameSelectorDelegate : TokenViewDelegate
		{

			private readonly TableSource _source;

			public NameSelectorDelegate (TableSource source)
			{
				_source = source;
			}

			public override void FilterToken (TokenView tokenField, string text)
			{
				_source.Filter (text);
			}
		}

		private sealed class NameSelectorSource : TokenViewSource
		{
			public override string GetToken (TokenView tokenField, int index)
			{
				return string.Empty;
			}

			public override int NumberOfTokens (TokenView tokenField)
			{
				return 0;
			}
		}

	}
}