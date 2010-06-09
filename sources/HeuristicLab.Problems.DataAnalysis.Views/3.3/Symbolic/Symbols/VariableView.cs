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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Core.Views;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols.Views {
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
      weightNuTextBox.Enabled = Content != null;
      weightSigmaTextBox.Enabled = Content != null;
      weightChangeNuTextBox.Enabled = Content != null;
      weightChangeSigmaTextBox.Enabled = Content != null;
    }

    #region content event handlers
    private void Content_Changed(object sender, EventArgs e) {
      UpdateControl();
    }
    #endregion

    #region control event handlers
    private void weightNuTextBox_TextChanged(object sender, EventArgs e) {
      double nu;
      if (double.TryParse(weightNuTextBox.Text, out nu)) {
        Content.WeightNu = nu;
        errorProvider.SetError(weightNuTextBox, string.Empty);
      } else {
        errorProvider.SetError(weightNuTextBox, "Invalid value");
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

    private void weightChangeNuTextBox_TextChanged(object sender, EventArgs e) {
      double nu;
      if (double.TryParse(weightChangeNuTextBox.Text, out nu)) {
        Content.WeightManipulatorNu = nu;
        errorProvider.SetError(weightChangeNuTextBox, string.Empty);
      } else {
        errorProvider.SetError(weightChangeNuTextBox, "Invalid value");
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
        weightNuTextBox.Text = string.Empty;
        weightSigmaTextBox.Text = string.Empty;
        weightNuTextBox.Text = string.Empty;
        weightChangeSigmaTextBox.Text = string.Empty;
      } else {
        weightNuTextBox.Text = Content.WeightNu.ToString();
        weightSigmaTextBox.Text = Content.WeightSigma.ToString();
        weightChangeNuTextBox.Text = Content.WeightManipulatorNu.ToString();
        weightChangeSigmaTextBox.Text = Content.WeightManipulatorSigma.ToString();
      }
      SetEnabledStateOfControls();
    }
    #endregion
  }
}
