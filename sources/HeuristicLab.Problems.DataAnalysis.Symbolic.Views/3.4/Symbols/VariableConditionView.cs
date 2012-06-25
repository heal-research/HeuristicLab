#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  [View("Variable View")]
  [Content(typeof(VariableCondition), true)]
  public partial class VariableConditionView : SymbolView {
    private readonly CheckedItemCollectionView<StringValue> variableNamesView;

    public new VariableCondition Content {
      get { return (VariableCondition)base.Content; }
      set { base.Content = value; }
    }

    public VariableConditionView() {
      InitializeComponent();
      variableNamesView = new CheckedItemCollectionView<StringValue>();
      variableNamesView.Dock = DockStyle.Fill;
      variableNamesTabPage.Controls.Add(variableNamesView);
      variableNamesView.Content = new CheckedItemCollection<StringValue>();

      RegisterVariableNamesViewContentEvents();
    }

    private void RegisterVariableNamesViewContentEvents() {
      variableNamesView.Content.ItemsAdded += new CollectionItemsChangedEventHandler<StringValue>(variableNames_Changed);
      variableNamesView.Content.ItemsRemoved += new CollectionItemsChangedEventHandler<StringValue>(variableNames_Changed);
      variableNamesView.Content.CheckedItemsChanged += new CollectionItemsChangedEventHandler<StringValue>(variableNames_Changed);
      variableNamesView.Content.CollectionReset += new CollectionItemsChangedEventHandler<StringValue>(variableNames_Changed);
      foreach (var variable in variableNamesView.Content) {
        variable.ValueChanged += new EventHandler(variable_ValueChanged);
      }
    }

    private void DeregisterVariableNamesViewContentEvents() {
      variableNamesView.Content.ItemsAdded -= new CollectionItemsChangedEventHandler<StringValue>(variableNames_Changed);
      variableNamesView.Content.ItemsRemoved -= new CollectionItemsChangedEventHandler<StringValue>(variableNames_Changed);
      variableNamesView.Content.CheckedItemsChanged -= new CollectionItemsChangedEventHandler<StringValue>(variableNames_Changed);
      variableNamesView.Content.CollectionReset -= new CollectionItemsChangedEventHandler<StringValue>(variableNames_Changed);
      foreach (var variable in variableNamesView.Content) {
        variable.ValueChanged -= new EventHandler(variable_ValueChanged);
      }
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
        // temporarily deregister to prevent circular calling of events
        DeregisterVariableNamesViewContentEvents();
        variableNamesView.Content.Clear();
        RegisterVariableNamesViewContentEvents();
      } else {
        var existingEntries = variableNamesView.Content.ToList();

        // temporarily deregister to prevent circular calling of events
        DeregisterVariableNamesViewContentEvents();
        // add additional entries
        foreach (var variableName in Content.VariableNames.Except(existingEntries.Select(x => x.Value)))
          variableNamesView.Content.Add(new StringValue(variableName), true);
        foreach (var oldEntry in existingEntries.Where(x => !Content.VariableNames.Contains(x.Value)))
          variableNamesView.Content.Remove(oldEntry);
        RegisterVariableNamesViewContentEvents();

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
    private void variableNames_Changed(object sender, CollectionItemsChangedEventArgs<StringValue> args) {
      if (args.Items != null)
        foreach (var newVar in args.Items)
          newVar.ValueChanged += new EventHandler(variable_ValueChanged);
      if (args.OldItems != null)
        foreach (var oldVar in args.OldItems)
          oldVar.ValueChanged -= new EventHandler(variable_ValueChanged);
      UpdateContent();
    }

    private void variable_ValueChanged(object sender, EventArgs e) {
      UpdateContent();
    }

    private void UpdateContent() {
      if (Content != null) {
        DeregisterContentEvents();
        Content.VariableNames = variableNamesView.Content.CheckedItems.Select(x => x.Value).ToList();
        RegisterContentEvents();
      }
    }

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