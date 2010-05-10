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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using System.Drawing;

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

    public override bool ReadOnly {
      get { return base.ReadOnly; }
      set { /*not needed because results are always readonly */}
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with caption "Variable".
    /// </summary>
    public RunView() {
      InitializeComponent();
      Caption = "Run";
      base.ReadOnly = true;
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Changed += new EventHandler(Content_Changed);
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.Changed -= new EventHandler(Content_Changed);
    }
    private void Content_Changed(object sender, EventArgs e) {
      if (InvokeRequired)
        this.Invoke(new EventHandler(Content_Changed), sender, e);
      else
        UpdateColorPictureBox();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      FillListView();
      viewHost.ViewType = null;
      viewHost.Content = null;
      if (Content == null)
        Caption = "Run";
      else {
        Caption = Content.Name + " (" + Content.GetType().Name + ")";
        UpdateColorPictureBox();
      }
      SetEnabledStateOfControls();
    }
    protected override void OnReadOnlyChanged() {
      base.OnReadOnlyChanged();
      SetEnabledStateOfControls();
    }
    protected override void OnLockedChanged() {
      base.OnLockedChanged();
      SetEnabledStateOfControls();
    }
    private void SetEnabledStateOfControls() {
      listView.Enabled = Content != null;
      viewHost.Enabled = Content != null;
      changeColorButton.Enabled = Content != null;
      showAlgorithmButton.Enabled = Content != null && !Locked;
    }

    private void changeColorButton_Click(object sender, EventArgs e) {
      if (colorDialog.ShowDialog(this) == DialogResult.OK) {
        this.Content.Color = this.colorDialog.Color;
      }
    }
    private void UpdateColorPictureBox() {
      this.colorDialog.Color = this.Content.Color;
      this.colorPictureBox.Image = this.GenerateImage(colorPictureBox.Width, colorPictureBox.Height, this.Content.Color);
    }
    private Image GenerateImage(int width, int height, Color fillColor) {
      Image colorImage = new Bitmap(width, height);
      using (Graphics gfx = Graphics.FromImage(colorImage)) {
        using (SolidBrush brush = new SolidBrush(fillColor)) {
          gfx.FillRectangle(brush, 0, 0, width, height);
        }
      }
      return colorImage;
    }

    private void FillListView() {
      listView.Items.Clear();
      listView.SmallImageList.Images.Clear();
      if (Content != null) {
        foreach (string key in Content.Parameters.Keys)
          CreateListViewItem(key, Content.Parameters[key], listView.Groups["parametersGroup"]);
        foreach (string key in Content.Results.Keys)
          CreateListViewItem(key, Content.Results[key], listView.Groups["resultsGroup"]);
        if (listView.Items.Count > 0) {
          for (int i = 0; i < listView.Columns.Count; i++)
            listView.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
      }
    }

    private void CreateListViewItem(string name, IItem value, ListViewGroup group) {
      ListViewItem item = new ListViewItem(new string[] { name, value != null ? value.ToString() : "-" });
      item.Tag = value;
      item.Group = group;
      listView.SmallImageList.Images.Add(value == null ? HeuristicLab.Common.Resources.VS2008ImageLibrary.Nothing : value.ItemImage);
      item.ImageIndex = listView.SmallImageList.Images.Count - 1;
      listView.Items.Add(item);
    }

    private void listView_SelectedIndexChanged(object sender, EventArgs e) {
      if (listView.SelectedItems.Count == 1) {
        viewHost.ViewType = null;
        viewHost.Content = (IContent)listView.SelectedItems[0].Tag;
      } else {
        viewHost.Content = null;
      }
    }
    private void listView_DoubleClick(object sender, EventArgs e) {
      if (listView.SelectedItems.Count == 1) {
        IItem item = (IItem)listView.SelectedItems[0].Tag;
        IContentView view = MainFormManager.MainForm.ShowContent(item);
        if (view != null) {
          view.ReadOnly = ReadOnly;
          view.Locked = Locked;
        }
      }
    }
    private void listView_ItemDrag(object sender, ItemDragEventArgs e) {
      if (!Locked) {
        ListViewItem listViewItem = (ListViewItem)e.Item;
        IItem item = (IItem)listViewItem.Tag;
        if (item != null) {
          DataObject data = new DataObject();
          data.SetData("Type", item.GetType());
          data.SetData("Value", item);
          DragDropEffects result = DoDragDrop(data, DragDropEffects.Copy);
        }
      }
    }
    private void showAlgorithmButton_Click(object sender, EventArgs e) {
      if (!Locked) {
        MainFormManager.MainForm.ShowContent((IContent)Content.Algorithm.Clone());
      }
    }
  }
}
