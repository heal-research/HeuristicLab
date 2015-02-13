#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  [View("Symbolic Expression Grammar Sample Tree")]
  public partial class SymbolicExpressionGrammarSampleExpressionTreeView : NamedItemView {
    private IRandom random;
    public SymbolicExpressionGrammarSampleExpressionTreeView() {
      InitializeComponent();
      random = new MersenneTwister();
      maxSampleTreeLength = int.Parse(maxTreeLengthTextBox.Text);
      maxSampleTreeDepth = int.Parse(maxTreeDepthTextBox.Text);
      foreach (var treeCreator in ApplicationManager.Manager.GetInstances<ISymbolicExpressionTreeCreator>()) {
        treeCreatorComboBox.Items.Add(treeCreator);
      }
      treeCreatorComboBox.SelectedItem = treeCreatorComboBox.Items.OfType<ProbabilisticTreeCreator>().First();
    }

    private int maxSampleTreeLength;
    public int MaxSampleTreeLength {
      get { return maxSampleTreeLength; }
      set {
        if (maxSampleTreeLength != value) {
          maxSampleTreeLength = value;
          UpdateSampleTreeView();
        }
      }
    }

    private int maxSampleTreeDepth;
    public int MaxSampleTreeDepth {
      get { return maxSampleTreeDepth; }
      set {
        if (maxSampleTreeDepth != value) {
          maxSampleTreeDepth = value;
          UpdateSampleTreeView();
        }
      }
    }

    public new ISymbolicExpressionGrammar Content {
      get { return (ISymbolicExpressionGrammar)base.Content; }
      set { base.Content = value; }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      maxTreeLengthTextBox.Enabled = Content != null;
      maxTreeDepthTextBox.Enabled = Content != null;
      generateSampleTreeButton.Enabled = Content != null;
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Changed += new EventHandler(Content_Changed);
    }

    protected override void DeregisterContentEvents() {
      Content.Changed -= new EventHandler(Content_Changed);
      base.DeregisterContentEvents();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) sampleTreeView.Content = null;
      else UpdateSampleTreeView();
    }

    private void Content_Changed(object sender, System.EventArgs e) {
      UpdateSampleTreeView();
    }

    private void UpdateSampleTreeView() {
      try {
        ISymbolicExpressionTreeCreator creator = (ISymbolicExpressionTreeCreator)treeCreatorComboBox.SelectedItem;
        ISymbolicExpressionTree tree = creator.CreateTree(random, Content, MaxSampleTreeLength, MaxSampleTreeDepth);
        foreach (var node in tree.Root.IterateNodesPrefix().OfType<SymbolicExpressionTreeTopLevelNode>())
          node.SetGrammar(null);
        sampleTreeView.Content = tree;
      }
      catch (Exception ex) {
        sampleTreeView.Content = null;
        ErrorHandling.ShowErrorDialog(ex);
      }
    }

    #region control events
    private void generateSampleTreeButton_Click(object sender, EventArgs e) {
      UpdateSampleTreeView();
    }

    private void maxTreeLengthTextBox_Validating(object sender, CancelEventArgs e) {
      int maxTreeLength;
      if (int.TryParse(maxTreeLengthTextBox.Text, out maxTreeLength) && maxTreeLength > 3) {
        errorProvider.SetError(maxTreeLengthTextBox, string.Empty);
        e.Cancel = false;
      } else {
        errorProvider.SetError(maxTreeLengthTextBox, "Invalid value: maximum tree length must be larger than 3.");
        e.Cancel = true;
      }
    }
    private void maxTreeLengthTextBox_Validated(object sender, EventArgs e) {
      int maxTreeLength;
      if (int.TryParse(maxTreeLengthTextBox.Text, out maxTreeLength) && maxTreeLength > 3) {
        MaxSampleTreeLength = maxTreeLength;
      }
    }

    private void maxTreeDepthTextBox_Validating(object sender, CancelEventArgs e) {
      int maxTreeDepth;
      if (int.TryParse(maxTreeDepthTextBox.Text, out maxTreeDepth) && maxTreeDepth > 3) {
        errorProvider.SetError(maxTreeDepthTextBox, string.Empty);
        e.Cancel = false;
      } else {
        errorProvider.SetError(maxTreeDepthTextBox, "Invalid value: maximum tree depth must be larger than 3.");
        e.Cancel = true;
      }
    }
    private void maxTreeDepthTextBox_Validated(object sender, EventArgs e) {
      int maxTreeDepth;
      if (int.TryParse(maxTreeDepthTextBox.Text, out maxTreeDepth) && maxTreeDepth > 3) {
        MaxSampleTreeDepth = maxTreeDepth;
      }
    }
    #endregion

    private void treeCreatorComboBox_SelectedIndexChanged(object sender, EventArgs e) {
    }
  }
}
