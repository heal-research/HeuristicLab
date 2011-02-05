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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimization.Views {
  /// <summary>
  /// The base class for visual representations of items.
  /// </summary>
  [View("UserDefinedAlgorithm View")]
  [Content(typeof(UserDefinedAlgorithm), true)]
  public sealed partial class UserDefinedAlgorithmView : EngineAlgorithmView {
    public new UserDefinedAlgorithm Content {
      get { return (UserDefinedAlgorithm)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public UserDefinedAlgorithmView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null)
        globalScopeView.Content = null;
      else
        globalScopeView.Content = Content.GlobalScope;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      globalScopeView.Enabled = Content != null;
      newOperatorGraphButton.Enabled = Content != null && !ReadOnly;
      openOperatorGraphButton.Enabled = Content != null && !ReadOnly;
      operatorGraphViewHost.ReadOnly = Content == null || ReadOnly;
    }

    private void newOperatorGraphButton_Click(object sender, EventArgs e) {
      Content.OperatorGraph = new OperatorGraph();
    }
    private void openOperatorGraphButton_Click(object sender, EventArgs e) {
      openFileDialog.Title = "Open Operator Graph";
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        newOperatorGraphButton.Enabled = openOperatorGraphButton.Enabled = false;
        operatorGraphViewHost.Enabled = false;

        ContentManager.LoadAsync(openFileDialog.FileName, delegate(IStorableContent content, Exception error) {
          try {
            if (error != null) throw error;
            OperatorGraph operatorGraph = content as OperatorGraph;
            if (operatorGraph == null)
              MessageBox.Show(this, "The selected file does not contain an operator graph.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
              Content.OperatorGraph = operatorGraph;
          }
          catch (Exception ex) {
            ErrorHandling.ShowErrorDialog(this, ex);
          }
          finally {
            Invoke(new Action(delegate() {
              operatorGraphViewHost.Enabled = true;
              newOperatorGraphButton.Enabled = openOperatorGraphButton.Enabled = true;
            }));
          }
        });
      }
    }
  }
}
