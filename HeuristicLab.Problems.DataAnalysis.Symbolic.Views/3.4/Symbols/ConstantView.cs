#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
      additiveChangeSigmaTextBox.Enabled = Content != null;
      additiveChangeSigmaTextBox.ReadOnly = ReadOnly;
      multiplicativeChangeSigmaTextBox.Enabled = Content != null;
      multiplicativeChangeSigmaTextBox.ReadOnly = ReadOnly;
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

    private void additiveChangeSigmaTextBox_TextChanged(object sender, EventArgs e) {
      double sigma;
      if (double.TryParse(additiveChangeSigmaTextBox.Text, out sigma) && sigma >= 0.0) {
        Content.ManipulatorSigma = sigma;
        errorProvider.SetError(additiveChangeSigmaTextBox, string.Empty);
      } else {
        errorProvider.SetError(additiveChangeSigmaTextBox, "Invalid value");
      }
    }

    private void multiplicativeChangeSigmaTextBox_TextChanged(object sender, EventArgs e) {
      double sigma;
      if (double.TryParse(multiplicativeChangeSigmaTextBox.Text, out sigma) && sigma >= 0.0) {
        Content.MultiplicativeManipulatorSigma = sigma;
        errorProvider.SetError(multiplicativeChangeSigmaTextBox, string.Empty);
      } else {
        errorProvider.SetError(multiplicativeChangeSigmaTextBox, "Invalid value");
      }
    }
    #endregion

    #region helpers
    private void UpdateControl() {
      if (Content == null) {
        minValueTextBox.Text = string.Empty;
        maxValueTextBox.Text = string.Empty;
        minValueTextBox.Text = string.Empty;
        multiplicativeChangeSigmaTextBox.Text = string.Empty;
        additiveChangeSigmaTextBox.Text = string.Empty;
      } else {
        minValueTextBox.Text = Content.MinValue.ToString();
        maxValueTextBox.Text = Content.MaxValue.ToString();
        additiveChangeSigmaTextBox.Text = Content.ManipulatorSigma.ToString();
        multiplicativeChangeSigmaTextBox.Text = Content.MultiplicativeManipulatorSigma.ToString();
      }
      SetEnabledStateOfControls();
    }
    #endregion
  }
}
