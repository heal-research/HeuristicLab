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
  [Content(typeof(Variable), true)]
  public partial class VariableView : SymbolView {
    private CheckedItemCollectionView<StringValue> variableNamesView;

    public new Variable Content {
      get { return (Variable)base.Content; }
      set { base.Content = value; }
    }

    public VariableView() {
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

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      weightInitializationMuTextBox.Enabled = Content != null;
      weightInitializationMuTextBox.ReadOnly = ReadOnly;
      weightInitializationSigmaTextBox.Enabled = Content != null;
      weightInitializationSigmaTextBox.ReadOnly = ReadOnly;
      additiveWeightChangeSigmaTextBox.Enabled = Content != null;
      additiveWeightChangeSigmaTextBox.ReadOnly = ReadOnly;
      multiplicativeWeightChangeSigmaTextBox.Enabled = Content != null;
      multiplicativeWeightChangeSigmaTextBox.ReadOnly = ReadOnly;
    }

    #region content event handlers
    private void Content_Changed(object sender, EventArgs e) {
      UpdateControl();
    }
    #endregion

    #region control event handlers
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

    private void weightMuTextBox_TextChanged(object sender, EventArgs e) {
      double nu;
      if (double.TryParse(weightInitializationMuTextBox.Text, out nu)) {
        Content.WeightMu = nu;
        errorProvider.SetError(weightInitializationMuTextBox, string.Empty);
      } else {
        errorProvider.SetError(weightInitializationMuTextBox, "Invalid value");
      }
    }
    private void weightSigmaTextBox_TextChanged(object sender, EventArgs e) {
      double sigma;
      if (double.TryParse(weightInitializationSigmaTextBox.Text, out sigma) && sigma >= 0.0) {
        Content.WeightSigma = sigma;
        errorProvider.SetError(weightInitializationSigmaTextBox, string.Empty);
      } else {
        errorProvider.SetError(weightInitializationSigmaTextBox, "Invalid value");
      }
    }

    private void additiveWeightChangeSigmaTextBox_TextChanged(object sender, EventArgs e) {
      double sigma;
      if (double.TryParse(additiveWeightChangeSigmaTextBox.Text, out sigma) && sigma >= 0.0) {
        Content.WeightManipulatorSigma = sigma;
        errorProvider.SetError(additiveWeightChangeSigmaTextBox, string.Empty);
      } else {
        errorProvider.SetError(additiveWeightChangeSigmaTextBox, "Invalid value");
      }
    }
    private void multiplicativeWeightChangeSigmaTextBox_TextChanged(object sender, EventArgs e) {
      double sigma;
      if (double.TryParse(multiplicativeWeightChangeSigmaTextBox.Text, out sigma) && sigma >= 0.0) {
        Content.MultiplicativeWeightManipulatorSigma = sigma;
        errorProvider.SetError(multiplicativeWeightChangeSigmaTextBox, string.Empty);
      } else {
        errorProvider.SetError(multiplicativeWeightChangeSigmaTextBox, "Invalid value");
      }
    }
    #endregion

    #region helpers
    private void UpdateControl() {
      if (Content == null) {
        weightInitializationMuTextBox.Text = string.Empty;
        weightInitializationSigmaTextBox.Text = string.Empty;
        additiveWeightChangeSigmaTextBox.Text = string.Empty;
        multiplicativeWeightChangeSigmaTextBox.Text = string.Empty;
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

        weightInitializationMuTextBox.Text = Content.WeightMu.ToString();
        weightInitializationSigmaTextBox.Text = Content.WeightSigma.ToString();
        additiveWeightChangeSigmaTextBox.Text = Content.WeightManipulatorSigma.ToString();
        multiplicativeWeightChangeSigmaTextBox.Text = Content.MultiplicativeWeightManipulatorSigma.ToString();
      }
      SetEnabledStateOfControls();
    }
    #endregion
  }
}
