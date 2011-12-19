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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.ExternalEvaluation.GP.Views {
  [View("Variable View")]
  [Content(typeof(Variable), IsDefaultView = true)]
  public sealed partial class VariableSymbolView : NamedItemView {
    private CheckedItemCollectionView<StringValue> variableNamesView;
    private CheckedItemCollection<StringValue> variableNames;
    private bool suppressDownstreamSynchronization;

    public new Variable Content {
      get { return (Variable)base.Content; }
      set { base.Content = value; }
    }

    public VariableSymbolView() {
      InitializeComponent();
      variableNamesView = new CheckedItemCollectionView<StringValue>();
      variableNamesView.Dock = DockStyle.Fill;
      variableNamesTabPage.Controls.Add(variableNamesView);
    }

    protected override void DeregisterContentEvents() {
      Content.Changed += new EventHandler(Content_Changed);
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Changed -= new EventHandler(Content_Changed);
    }

    #region Event Handlers (Content)
    private void Content_Changed(object sender, EventArgs e) {
      SynchronizeVariableNames();
      SynchronizeParameters();
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        variableNamesView.Content = null;
      } else {
        SynchronizeVariableNames();
        SynchronizeParameters();
      }
    }

    private void SynchronizeVariableNames() {
      suppressDownstreamSynchronization = true;
      variableNames = new CheckedItemCollection<StringValue>();
      variableNamesView.Content = variableNames;
      variableNames.AddRange(Content.VariableNames.Select(x => new StringValue(x)).ToList());
      variableNames.ItemsAdded += new CollectionItemsChangedEventHandler<StringValue>(variableNames_Changed);
      variableNames.ItemsRemoved += new CollectionItemsChangedEventHandler<StringValue>(variableNames_Changed);
      variableNames.CheckedItemsChanged += new CollectionItemsChangedEventHandler<StringValue>(variableNames_Changed);
      variableNames.CollectionReset += new CollectionItemsChangedEventHandler<StringValue>(variableNames_Changed);
      suppressDownstreamSynchronization = false;
    }

    private void SynchronizeParameters() {
      suppressDownstreamSynchronization = true;
      weightNuTextBox.Text = Content.WeightNu.ToString();
      weightSigmaTextBox.Text = Content.WeightSigma.ToString();
      weightManipulatorNuTextBox.Text = Content.WeightManipulatorNu.ToString();
      weightManipulatorSigmaTextBox.Text = Content.WeightManipulatorSigma.ToString();
      suppressDownstreamSynchronization = false;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      weightNuTextBox.Enabled = !ReadOnly && Content != null;
      weightSigmaTextBox.Enabled = !ReadOnly && Content != null;
      weightManipulatorNuTextBox.Enabled = !ReadOnly && Content != null;
      weightManipulatorSigmaTextBox.Enabled = !ReadOnly && Content != null;
    }

    #region Event Handlers (child controls)
    private void variableNames_Changed(object sender, CollectionItemsChangedEventArgs<StringValue> args) {
      if (!suppressDownstreamSynchronization && Content != null) {
        Content.VariableNames = variableNames.CheckedItems.Select(x => x.Value).ToList();
      }
    }

    private void weightTextBox_Validating(object sender, CancelEventArgs e) {
      e.Cancel = false;
      if (!suppressDownstreamSynchronization && Content != null) {
        TextBox textBox = (sender as TextBox);
        if (textBox == null) throw new ArgumentException("Event handler expected a TextBox as sender", "sender");
        double value;
        if (!double.TryParse(textBox.Text, out value))
          e.Cancel = true;
        else {
          if (textBox == weightNuTextBox) {
            Content.WeightNu = value;
          } else if (textBox == weightSigmaTextBox) {
            Content.WeightSigma = value;
          } else if (textBox == weightManipulatorNuTextBox) {
            Content.WeightManipulatorNu = value;
          } else if (textBox == weightManipulatorSigmaTextBox) {
            Content.WeightManipulatorSigma = value;
          } else throw new ArgumentException("Unknown sender when trying to set the weights of the variable symbol.");
        }
      }
    }
    #endregion
  }
}
