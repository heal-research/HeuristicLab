#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimization.Views {
  /// <summary>
  /// The visual representation of a <see cref="Variable"/>.
  /// </summary>
  [View("Run View")]
  [Content(typeof(IRun), true)]
  public sealed partial class RunView : NamedItemView {
    /// <summary>
    /// Gets or sets the variable to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public new IRun Content {
      get { return (IRun)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with caption "Variable".
    /// </summary>
    public RunView() {
      InitializeComponent();
      Caption = "Run";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with the given <paramref name="variable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="VariableView()"/>.</remarks>
    /// <param name="variable">The variable to represent visually.</param>
    public RunView(IRun content)
      : this() {
      Content = content;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      FillListView();
      viewHost.ViewType = null;
      viewHost.Content = null;
      if (Content == null) {
        Caption = "Run";
        parametersResultsGroupBox.Enabled = false;
      } else {
        Caption = Content.Name + " (" + Content.GetType().Name + ")";
        parametersResultsGroupBox.Enabled = true;
      }
    }

    private void FillListView() {
      if (!listView.SmallImageList.Images.ContainsKey("Default"))
        listView.SmallImageList.Images.Add("Default", HeuristicLab.Common.Resources.VS2008ImageLibrary.Nothing);

      listView.Items.Clear();
      if (Content != null) {
        foreach (string key in Content.Parameters.Keys) {
          IItem value = Content.Parameters[key];
          if ((value != null) && (!listView.SmallImageList.Images.ContainsKey(value.GetType().FullName)))
            listView.SmallImageList.Images.Add(value.GetType().FullName, value.ItemImage);

          ListViewItem item = new ListViewItem(new string[] { key, value != null ? value.ToString() : "-" });
          item.Tag = value;
          item.Group = listView.Groups["parametersGroup"];
          item.ImageIndex = listView.SmallImageList.Images.IndexOfKey(value != null ? value.GetType().FullName : "Default");
          listView.Items.Add(item);
        }
        foreach (string key in Content.Results.Keys) {
          IItem value = Content.Results[key];
          if ((value != null) && (!listView.SmallImageList.Images.ContainsKey(value.GetType().FullName)))
            listView.SmallImageList.Images.Add(value.GetType().FullName, value.ItemImage);

          ListViewItem item = new ListViewItem(new string[] { key, value != null ? value.ToString() : "-" });
          item.Tag = value;
          item.Group = listView.Groups["resultsGroup"];
          item.ImageIndex = listView.SmallImageList.Images.IndexOfKey(value != null ? value.GetType().FullName : "Default");
          listView.Items.Add(item);
        }
        if (listView.Items.Count > 0) {
          for (int i = 0; i < listView.Columns.Count; i++)
            listView.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
      }
    }

    private void listView_SelectedIndexChanged(object sender, EventArgs e) {
      if (listView.SelectedItems.Count == 1) {
        viewHost.ViewType = null;
        viewHost.Content = listView.SelectedItems[0].Tag;
      } else {
        viewHost.Content = null;
      }
    }
  }
}
