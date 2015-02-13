#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  [View("Variable Value View")]
  [Content(typeof(Variable), false)]
  [Content(typeof(IVariable), false)]
  public partial class VariableValueView : ItemView {
    private const string infoLabelToolTipSuffix = "Double-click to open description editor.";

    public new IVariable Content {
      get { return (IVariable)base.Content; }
      set { base.Content = value; }
    }

    public VariableValueView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.NameChanged -= new EventHandler(Content_NameChanged);
      Content.DescriptionChanged -= new EventHandler(Content_DescriptionChanged);
      Content.ValueChanged -= new EventHandler(Content_ValueChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.NameChanged += new EventHandler(Content_NameChanged);
      Content.DescriptionChanged += new EventHandler(Content_DescriptionChanged);
      Content.ValueChanged += new EventHandler(Content_ValueChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        viewHost.Content = null;
        toolTip.SetToolTip(infoLabel, string.Empty);
        if (ViewAttribute.HasViewAttribute(this.GetType()))
          this.Caption = ViewAttribute.GetViewName(this.GetType());
        else
          this.Caption = "VariableValue View";
      } else {
        viewHost.ViewType = null;
        viewHost.Content = Content.Value;
        toolTip.SetToolTip(infoLabel, string.IsNullOrEmpty(Content.Description) ? infoLabelToolTipSuffix : Content.Description + Environment.NewLine + Environment.NewLine + infoLabelToolTipSuffix);
        Caption = Content.Name;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      viewHost.Enabled = Content != null;
      viewHost.ReadOnly = this.ReadOnly;
      infoLabel.Enabled = Content != null;
    }

    protected virtual void Content_NameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_NameChanged), sender, e);
      else
        Caption = Content.Name;
    }
    protected virtual void Content_DescriptionChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_DescriptionChanged), sender, e);
      else
        toolTip.SetToolTip(infoLabel, string.IsNullOrEmpty(Content.Description) ? infoLabelToolTipSuffix : Content.Description + Environment.NewLine + Environment.NewLine + infoLabelToolTipSuffix);
    }
    protected virtual void Content_ValueChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ValueChanged), sender, e);
      else {
        viewHost.ViewType = null;
        viewHost.Content = Content.Value;
      }
    }

    protected virtual void VariableValueView_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (!ReadOnly && (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IItem)) {
        if ((e.KeyState & 32) == 32) e.Effect = DragDropEffects.Link;  // ALT key
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Copy)) e.Effect = DragDropEffects.Copy;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Move)) e.Effect = DragDropEffects.Move;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Link)) e.Effect = DragDropEffects.Link;
      }
    }
    protected virtual void VariableValueView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        IItem item = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as IItem;
        if (e.Effect.HasFlag(DragDropEffects.Copy)) item = (IItem)item.Clone();
        Content.Value = item;
      }
    }
    protected virtual void infoLabel_DoubleClick(object sender, EventArgs e) {
      using (TextDialog dialog = new TextDialog("Description of " + Content.Name, Content.Description, ReadOnly || !Content.CanChangeDescription)) {
        if (dialog.ShowDialog(this) == DialogResult.OK)
          Content.Description = dialog.Content;
      }
    }
  }
}
