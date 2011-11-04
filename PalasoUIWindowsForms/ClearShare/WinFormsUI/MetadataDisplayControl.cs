﻿using System;
using System.Diagnostics;
using System.Windows.Forms;
using Palaso.UI.WindowsForms.Widgets;

namespace Palaso.UI.WindowsForms.ClearShare.WinFormsUI
{
	/// <summary>
	/// This control is just for displaying metadata in a compact way, not editing it.
	/// </summary>
	public partial class MetadataDisplayControl : UserControl
	{
		public MetadataDisplayControl()
		{
			InitializeComponent();
		}

		public void SetMetadata(Metadata metaData)
		{
			_table.SuspendLayout();
			_table.Controls.Clear();
			_table.RowCount = 0;
			_table.RowStyles.Clear();
			if(!string.IsNullOrEmpty(metaData.Creator))
			{
				AddRow("Creator", new Label(){Text=metaData.Creator});
			}
			if (!string.IsNullOrEmpty(metaData.CollectionName))
			{
				if(!string.IsNullOrEmpty(metaData.CollectionUri))
				{
					AddHyperLink(metaData.CollectionName, metaData.CollectionUri,2);
				}
				else
				{
					AddRow(metaData.CollectionName);
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(metaData.CollectionUri))
				{
					AddHyperLink(metaData.CollectionUri, metaData.CollectionUri,2);
				}
			}

			if (!string.IsNullOrEmpty(metaData.CopyrightNotice))
			{
				AddRow(metaData.ShortCopyrightNotice);
			}
			if(!string.IsNullOrEmpty(metaData.AttributionUrl))
			{
				AddHyperLink(metaData.AttributionUrl.Replace("http://",""), metaData.AttributionUrl,2);
			}
			if(metaData.License!=null)
			{
				var image = new PictureBox(){Image = metaData.License.GetImage()};
				_table.Controls.Add(image);
				if (!string.IsNullOrEmpty(metaData.License.Url))
				{
					AddHyperLink("License Info", metaData.License.Url,1);
				}
				else
				{
					_table.SetColumnSpan(image,2);
				}
				_table.RowCount++;
			}
			_table.ResumeLayout();
		}
		private void AddHyperLink(string label, string url, int columns)
		{
			using (var g = this.CreateGraphics())
			{
				var w = this.Width - 10;
				var h = g.MeasureString(label, this.Font, w).Height;
				var linkLabel = new LinkLabel() {Text = label, Width = this.Width - 10, Height = (int) (h + 5)};

				linkLabel.Click += new EventHandler((x, y) => Process.Start(url));
				_table.Controls.Add(linkLabel);
				_table.SetColumnSpan(linkLabel, columns);
			   _table.RowCount++;
			}
		}

		public void AddRow(string label, Control control)
		{
			_table.Controls.Add(new Label() { Text = label });
			_table.Controls.Add(control);
			_table.RowCount++;
		}
		public void AddRow(string label)
		{
			var control = new BetterLabel() { Text = label };
			_table.Controls.Add(control);
			_table.SetColumnSpan(control,2);
			_table.RowCount++;
		}

//        public void LayoutRows()
//        {
//            foreach (Control c in _table.Controls)
//            {
//                c.TabIndex = _table.Controls.GetChildIndex(c);
//            }
//
//            float h = 0;
//            _table.RowStyles.Clear();
//            for (int r = 0; r < _table.RowCount; r++)
//            {
//                Control c = _table.GetControlFromPosition(0, r);
//                if (c != null)// null happens at design time
//                {
//                    RowStyle style = new RowStyle(SizeType.Absolute, c.Height + _table.Margin.Vertical);
//                    _table.RowStyles.Add(style);
//                    h += style.Height;
//                }
//            }
//            _table.Height = (int)h;
//            //_table.Invalidate();
//            _table.Refresh();
//        }

		private void UpdateDisplay()
		{

		}

		private void MetadataDisplayControl_Load(object sender, EventArgs e)
		{
			UpdateDisplay();
		}
	}
}