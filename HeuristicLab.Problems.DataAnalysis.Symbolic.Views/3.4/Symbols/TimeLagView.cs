#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  [View("LaggedSymbol View")]
  [Content(typeof(LaggedSymbol), true)]
  public partial class LaggedSymbolView : SymbolView {
    public new LaggedSymbol Content {
      get { return (LaggedSymbol)base.Content; }
      set { base.Content = value; }
    }

    public LaggedSymbolView() {
      InitializeComponent();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Changed += new EventHandler(Content_Changed);
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.Changed -= new EventHandler(Content_Changed);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateControl();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      minTimeOffsetTextBox.Enabled = Content != null;
      minTimeOffsetTextBox.ReadOnly = ReadOnly;
      maxTimeOffsetTextBox.Enabled = Content != null;
      maxTimeOffsetTextBox.ReadOnly = ReadOnly;
    }

    #region content event handlers
    private void Content_Changed(object sender, EventArgs e) {
      UpdateControl();
    }
    #endregion

    #region control event handlers

    private void minTimeOffsetTextBox_TextChanged(object sender, EventArgs e) {
      int timeOffset;
      if (int.TryParse(minTimeOffsetTextBox.Text, out timeOffset) && timeOffset <= 0) {
        Content.MinLag = timeOffset;
        errorProvider.SetError(minTimeOffsetTextBox, string.Empty);
      } else {
        errorProvider.SetError(minTimeOffsetTextBox, "Time offset must be negative or zero.");
      }
    }

    private void maxTimeOffsetTextBox_TextChanged(object sender, EventArgs e) {
      int timeOffset;
      if (int.TryParse(maxTimeOffsetTextBox.Text, out timeOffset) && timeOffset <= 0) {
        Content.MaxLag = timeOffset;
        errorProvider.SetError(maxTimeOffsetTextBox, string.Empty);
      } else {
        errorProvider.SetError(maxTimeOffsetTextBox, "Time offset must be negative or zero.");
      }
    }

    #endregion

    #region helpers
    private void UpdateControl() {
      if (Content == null) {
        minTimeOffsetTextBox.Text = string.Empty;
        maxTimeOffsetTextBox.Text = string.Empty;
      } else {
        minTimeOffsetTextBox.Text = Content.MinLag.ToString();
        maxTimeOffsetTextBox.Text = Content.MaxLag.ToString();
      }
      SetEnabledStateOfControls();
    }
    #endregion

  }
}
