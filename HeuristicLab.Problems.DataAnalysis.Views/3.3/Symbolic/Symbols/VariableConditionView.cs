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
  [Content(typeof(VariableCondition), true)]
  public partial class VariableConditionView : SymbolView {
    public new VariableCondition Content {
      get { return (VariableCondition)base.Content; }
      set { base.Content = value; }
    }

    public VariableConditionView() {
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
    private void Content_Changed(object sender, EventArgs e) {
      UpdateControl();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      thresholdInitializationMuTextBox.Enabled = Content != null;
      thresholdInitializationMuTextBox.ReadOnly = ReadOnly;
      thresholdInitializationSigmaTextBox.Enabled = Content != null;
      thresholdInitializationSigmaTextBox.ReadOnly = ReadOnly;
      slopeInitializationMuTextBox.Enabled = Content != null;
      slopeInitializationMuTextBox.ReadOnly = ReadOnly;
      slopeInitializationSigmaTextBox.Enabled = Content != null;
      slopeInitializationSigmaTextBox.ReadOnly = ReadOnly;

      thresholdChangeMuTextBox.Enabled = Content != null;
      thresholdChangeMuTextBox.ReadOnly = ReadOnly;
      thresholdChangeSigmaTextBox.Enabled = Content != null;
      thresholdChangeSigmaTextBox.ReadOnly = ReadOnly;
      slopeChangeMuTextBox.Enabled = Content != null;
      slopeChangeMuTextBox.ReadOnly = ReadOnly;
      slopeChangeSigmaTextBox.Enabled = Content != null;
      slopeChangeSigmaTextBox.ReadOnly = ReadOnly;
    }



    private void UpdateControl() {
      if (Content == null) {
        thresholdInitializationMuTextBox.Text = string.Empty;
        thresholdInitializationSigmaTextBox.Text = string.Empty;
        slopeInitializationMuTextBox.Text = string.Empty;
        slopeInitializationSigmaTextBox.Text = string.Empty;
        thresholdChangeMuTextBox.Text = string.Empty;
        thresholdChangeSigmaTextBox.Text = string.Empty;
        slopeChangeMuTextBox.Text = string.Empty;
        slopeChangeSigmaTextBox.Text = string.Empty;
      } else {
        thresholdInitializationMuTextBox.Text = Content.ThresholdInitializerMu.ToString();
        thresholdInitializationSigmaTextBox.Text = Content.ThresholdInitializerSigma.ToString();
        slopeInitializationMuTextBox.Text = Content.SlopeInitializerMu.ToString();
        slopeInitializationSigmaTextBox.Text = Content.SlopeInitializerSigma.ToString();
        thresholdChangeMuTextBox.Text = Content.ThresholdManipulatorMu.ToString();
        thresholdChangeSigmaTextBox.Text = Content.ThresholdManipulatorSigma.ToString();
        slopeChangeMuTextBox.Text = Content.SlopeManipulatorMu.ToString();
        slopeChangeSigmaTextBox.Text = Content.SlopeManipulatorSigma.ToString();
      }
      SetEnabledStateOfControls();
    }

    #region control events
    private void thresholdMuTextBox_TextChanged(object sender, EventArgs e) {
      double value;
      if (double.TryParse(thresholdInitializationMuTextBox.Text, out value)) {
        Content.ThresholdInitializerMu = value;
        errorProvider.SetError(thresholdInitializationMuTextBox, string.Empty);
      } else errorProvider.SetError(thresholdInitializationMuTextBox, "Invalid value");
    }
    private void thresholdInitializationSigmaTextBox_TextChanged(object sender, EventArgs e) {
      double value;
      if (double.TryParse(thresholdInitializationSigmaTextBox.Text, out value)) {
        Content.ThresholdInitializerSigma = value;
        errorProvider.SetError(thresholdInitializationSigmaTextBox, string.Empty);
      } else errorProvider.SetError(thresholdInitializationSigmaTextBox, "Invalid value");
    }
    private void slopeInitializationMuTextBox_TextChanged(object sender, EventArgs e) {
      double value;
      if (double.TryParse(slopeInitializationMuTextBox.Text, out value)) {
        Content.SlopeInitializerMu = value;
        errorProvider.SetError(slopeInitializationMuTextBox, string.Empty);
      } else errorProvider.SetError(slopeInitializationMuTextBox, "Invalid value");
    }
    private void slopeInitializationSigmaTextBox_TextChanged(object sender, EventArgs e) {
      double value;
      if (double.TryParse(slopeInitializationSigmaTextBox.Text, out value)) {
        Content.SlopeInitializerSigma = value;
        errorProvider.SetError(slopeInitializationSigmaTextBox, string.Empty);
      } else errorProvider.SetError(slopeInitializationSigmaTextBox, "Invalid value");
    }

    private void thresholdChangeMuTextBox_TextChanged(object sender, EventArgs e) {
      double value;
      if (double.TryParse(thresholdChangeMuTextBox.Text, out value)) {
        Content.ThresholdManipulatorMu = value;
        errorProvider.SetError(thresholdChangeMuTextBox, string.Empty);
      } else errorProvider.SetError(thresholdChangeMuTextBox, "Invalid value");
    }

    private void thresholdChangeSigmaTextBox_TextChanged(object sender, EventArgs e) {
      double value;
      if (double.TryParse(thresholdChangeSigmaTextBox.Text, out value)) {
        Content.ThresholdManipulatorSigma = value;
        errorProvider.SetError(thresholdChangeSigmaTextBox, string.Empty);
      } else errorProvider.SetError(thresholdChangeSigmaTextBox, "Invalid value");
    }
    private void slopeChangeMuTextBox_TextChanged(object sender, EventArgs e) {
      double value;
      if (double.TryParse(slopeChangeMuTextBox.Text, out value)) {
        Content.SlopeManipulatorMu = value;
        errorProvider.SetError(slopeChangeMuTextBox, string.Empty);
      } else errorProvider.SetError(slopeChangeMuTextBox, "Invalid value");
    }

    private void slopeChangeSigmaTextBox_TextChanged(object sender, EventArgs e) {
      double value;
      if (double.TryParse(slopeChangeSigmaTextBox.Text, out value)) {
        Content.SlopeManipulatorSigma = value;
        errorProvider.SetError(slopeChangeSigmaTextBox, string.Empty);
      } else errorProvider.SetError(slopeChangeSigmaTextBox, "Invalid value");
    }
    #endregion

  }
}