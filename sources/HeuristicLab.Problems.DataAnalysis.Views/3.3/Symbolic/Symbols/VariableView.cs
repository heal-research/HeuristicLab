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
  [View("Variable View")]
  [Content(typeof(Variable), true)]
  public partial class VariableView : SymbolView {
    public new Variable Content {
      get { return (Variable)base.Content; }
      set { base.Content = value; }
    }

    public VariableView() {
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
      weightMuTextBox.Enabled = Content != null;
      weightMuTextBox.ReadOnly = ReadOnly;
      weightSigmaTextBox.Enabled = Content != null;
      weightSigmaTextBox.ReadOnly = ReadOnly;
      weightChangeMuTextBox.Enabled = Content != null;
      weightChangeMuTextBox.ReadOnly = ReadOnly;
      weightChangeSigmaTextBox.Enabled = Content != null;
      weightChangeSigmaTextBox.ReadOnly = ReadOnly;
    }

    #region content event handlers
    private void Content_Changed(object sender, EventArgs e) {
      UpdateControl();
    }
    #endregion

    #region control event handlers
    private void weightMuTextBox_TextChanged(object sender, EventArgs e) {
      double nu;
      if (double.TryParse(weightMuTextBox.Text, out nu)) {
        Content.WeightMu = nu;
        errorProvider.SetError(weightMuTextBox, string.Empty);
      } else {
        errorProvider.SetError(weightMuTextBox, "Invalid value");
      }
    }
    private void weightSigmaTextBox_TextChanged(object sender, EventArgs e) {
      double sigma;
      if (double.TryParse(weightSigmaTextBox.Text, out sigma) && sigma >= 0.0) {
        Content.WeightSigma = sigma;
        errorProvider.SetError(weightSigmaTextBox, string.Empty);
      } else {
        errorProvider.SetError(weightSigmaTextBox, "Invalid value");
      }
    }

    private void weightChangeMuTextBox_TextChanged(object sender, EventArgs e) {
      double nu;
      if (double.TryParse(weightChangeMuTextBox.Text, out nu)) {
        Content.WeightManipulatorMu = nu;
        errorProvider.SetError(weightChangeMuTextBox, string.Empty);
      } else {
        errorProvider.SetError(weightChangeMuTextBox, "Invalid value");
      }
    }

    private void weightChangeSigmaTextBox_TextChanged(object sender, EventArgs e) {
      double sigma;
      if (double.TryParse(weightChangeSigmaTextBox.Text, out sigma) && sigma >= 0.0) {
        Content.WeightManipulatorSigma = sigma;
        errorProvider.SetError(weightChangeSigmaTextBox, string.Empty);
      } else {
        errorProvider.SetError(weightChangeSigmaTextBox, "Invalid value");
      }
    }
    #endregion

    #region helpers
    private void UpdateControl() {
      if (Content == null) {
        weightMuTextBox.Text = string.Empty;
        weightSigmaTextBox.Text = string.Empty;
        weightMuTextBox.Text = string.Empty;
        weightChangeSigmaTextBox.Text = string.Empty;
      } else {
        weightMuTextBox.Text = Content.WeightMu.ToString();
        weightSigmaTextBox.Text = Content.WeightSigma.ToString();
        weightChangeMuTextBox.Text = Content.WeightManipulatorMu.ToString();
        weightChangeSigmaTextBox.Text = Content.WeightManipulatorSigma.ToString();
      }
      SetEnabledStateOfControls();
    }
    #endregion
  }
}
