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
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimization.Views {
  [View("Result View")]
  [Content(typeof(Result), false)]
  [Content(typeof(IResult), false)]
  public sealed partial class ResultView : NamedItemView {
    public new IResult Content {
      get { return (IResult)base.Content; }
      set { base.Content = value; }
    }

    public override bool ReadOnly {
      get { return true; }
      set { /*not needed because results are always readonly */}
    }

    public ResultView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.ValueChanged -= new EventHandler(Content_ValueChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ValueChanged += new EventHandler(Content_ValueChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        dataTypeTextBox.Text = "-";
        viewHost.Content = null;
      } else {
        dataTypeTextBox.Text = Content.DataType.GetPrettyName();
        viewHost.ViewType = null;
        viewHost.Content = Content.Value;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      dataTypeTextBox.Enabled = Content != null;
      viewHost.Enabled = Content != null;
      viewHost.ReadOnly = this.ReadOnly;
    }

    private void Content_ValueChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ValueChanged), sender, e);
      else {
        viewHost.Content = Content.Value;
      }
    }
  }
}
