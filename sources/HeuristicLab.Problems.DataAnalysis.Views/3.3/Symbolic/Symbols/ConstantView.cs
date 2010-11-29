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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;

namespace HeuristicLab.Problems.DataAnalysis.Views.Symbolic.Symbols {
  [View("Constant View")]
  [Content(typeof(Constant), true)]
  public partial class ConstantView : SymbolView {
    public new Constant Content {
      get { return (Constant)base.Content; }
      set { base.Content = value; }
    }

    public ConstantView() {
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
      minValueTextBox.Enabled = Content != null;
      minValueTextBox.ReadOnly = ReadOnly;
      maxValueTextBox.Enabled = Content != null;
      maxValueTextBox.ReadOnly = ReadOnly;
      valueChangeMuTextBox.Enabled = Content != null;
      valueChangeMuTextBox.ReadOnly = ReadOnly;
      valueChangeSigmaTextBox.Enabled = Content != null;
      valueChangeSigmaTextBox.ReadOnly = ReadOnly;
    }

    #region content event handlers
    private void Content_Changed(object sender, EventArgs e) {
      UpdateControl();
    }
    #endregion

    #region control event handlers
    private void minValueTextBox_TextChanged(object sender, EventArgs e) {
      double min;
      if (double.TryParse(minValueTextBox.Text, out min)) {
        Content.MinValue = min;
        errorProvider.SetError(minValueTextBox, string.Empty);
      } else {
        errorProvider.SetError(minValueTextBox, "Invalid value");
      }
    }
    private void maxValueTextBox_TextChanged(object sender, EventArgs e) {
      double max;
      if (double.TryParse(maxValueTextBox.Text, out max)) {
        Content.MaxValue = max;
        errorProvider.SetError(maxValueTextBox, string.Empty);
      } else {
        errorProvider.SetError(maxValueTextBox, "Invalid value");
      }
    }

    private void valueChangeMuTextBox_TextChanged(object sender, EventArgs e) {
      double nu;
      if (double.TryParse(valueChangeMuTextBox.Text, out nu)) {
        Content.ManipulatorMu = nu;
        errorProvider.SetError(valueChangeMuTextBox, string.Empty);
      } else {
        errorProvider.SetError(valueChangeMuTextBox, "Invalid value");
      }
    }

    private void valueChangeSigmaTextBox_TextChanged(object sender, EventArgs e) {
      double sigma;
      if (double.TryParse(valueChangeSigmaTextBox.Text, out sigma) && sigma >= 0.0) {
        Content.ManipulatorSigma = sigma;
        errorProvider.SetError(valueChangeSigmaTextBox, string.Empty);
      } else {
        errorProvider.SetError(valueChangeSigmaTextBox, "Invalid value");
      }
    }
    #endregion

    #region helpers
    private void UpdateControl() {
      if (Content == null) {
        minValueTextBox.Text = string.Empty;
        maxValueTextBox.Text = string.Empty;
        minValueTextBox.Text = string.Empty;
        valueChangeSigmaTextBox.Text = string.Empty;
      } else {
        minValueTextBox.Text = Content.MinValue.ToString();
        maxValueTextBox.Text = Content.MaxValue.ToString();
        valueChangeMuTextBox.Text = Content.ManipulatorMu.ToString();
        valueChangeSigmaTextBox.Text = Content.ManipulatorSigma.ToString();
      }
      SetEnabledStateOfControls();
    }
    #endregion
  }
}
