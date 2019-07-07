#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimization.Views {
  [View("Result Value View")]
  [Content(typeof(Result), true)]
  [Content(typeof(IResult), false)]
  public sealed partial class ResultValueView : ItemView {
    private const string infoLabelToolTipSuffix = "Double-click to open description editor.";

    public new IResult Content {
      get { return (IResult)base.Content; }
      set { base.Content = value; }
    }

    public override bool ReadOnly {
      get { return true; }
      set { /*not needed because results are always readonly */}
    }

    public ResultValueView() {
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
          this.Caption = "ResultValue View";
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

    private void Content_NameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_NameChanged), sender, e);
      else
        Caption = Content.Name;
    }
    private void Content_DescriptionChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_DescriptionChanged), sender, e);
      else
        toolTip.SetToolTip(infoLabel, string.IsNullOrEmpty(Content.Description) ? infoLabelToolTipSuffix : Content.Description + Environment.NewLine + Environment.NewLine + infoLabelToolTipSuffix);
    }
    private void Content_ValueChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ValueChanged), sender, e);
      else {
        viewHost.Content = Content.Value;
      }
    }

    private void infoLabel_DoubleClick(object sender, System.EventArgs e) {
      using (TextDialog dialog = new TextDialog("Description of " + Content.Name, Content.Description, ReadOnly || !Content.CanChangeDescription)) {
        if (dialog.ShowDialog(this) == DialogResult.OK)
          Content.Description = dialog.Content;
      }
    }
  }
}
