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
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Persistence.Default.Xml;

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
    /// <summary>
    /// Intializes a new instance of <see cref="ItemBaseView"/> with the given <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The item that should be displayed.</param>
    public UserDefinedAlgorithmView(UserDefinedAlgorithm content)
      : this() {
      Content = content;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null)
        globalScopeView.Content = null;
      else
        globalScopeView.Content = Content.GlobalScope;
      SetEnableStateOfControls();
    }

    protected override void OnReadOnlyChanged() {
      base.OnReadOnlyChanged();
      SetEnableStateOfControls();
    }
    private void SetEnableStateOfControls() {
      globalScopeView.ReadOnly = ReadOnly;
      operatorGraphViewHost.ReadOnly = ReadOnly;
      if (Content == null) 
        newOperatorGraphButton.Enabled = openOperatorGraphButton.Enabled = saveOperatorGraphButton.Enabled = false;
      else
        newOperatorGraphButton.Enabled = openOperatorGraphButton.Enabled = saveOperatorGraphButton.Enabled = Content.ExecutionState != ExecutionState.Started;
    }

    private void newOperatorGraphButton_Click(object sender, EventArgs e) {
      Content.OperatorGraph = new OperatorGraph();
    }
    private void openOperatorGraphButton_Click(object sender, EventArgs e) {
      openFileDialog.Title = "Open Operator Graph";
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        this.Cursor = Cursors.AppStarting;
        newOperatorGraphButton.Enabled = openOperatorGraphButton.Enabled = saveOperatorGraphButton.Enabled = false;
        operatorGraphViewHost.Enabled = false;

        var call = new Func<string, object>(XmlParser.Deserialize);
        call.BeginInvoke(openFileDialog.FileName, delegate(IAsyncResult a) {
          OperatorGraph operatorGraph = null;
          try {
            operatorGraph = call.EndInvoke(a) as OperatorGraph;
          }
          catch (Exception ex) {
            Auxiliary.ShowErrorMessageBox(ex);
          }
          Invoke(new Action(delegate() {
            if (operatorGraph == null)
              MessageBox.Show(this, "The selected file does not contain an operator graph.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
              Content.OperatorGraph = operatorGraph;
            operatorGraphViewHost.Enabled = true;
            newOperatorGraphButton.Enabled = openOperatorGraphButton.Enabled = saveOperatorGraphButton.Enabled = true;
            this.Cursor = Cursors.Default;
          }));
        }, null);
      }
    }
    private void saveOperatorGraphButton_Click(object sender, EventArgs e) {
      saveFileDialog.Title = "Save Operator Graph";
      if (saveFileDialog.ShowDialog(this) == DialogResult.OK) {
        this.Cursor = Cursors.AppStarting;
        newOperatorGraphButton.Enabled = openOperatorGraphButton.Enabled = saveOperatorGraphButton.Enabled = false;
        operatorGraphViewHost.Enabled = false;

        var call = new Action<OperatorGraph, string, int>(XmlGenerator.Serialize);
        int compression = 9;
        if (saveFileDialog.FilterIndex == 1) compression = 0;
        call.BeginInvoke(Content.OperatorGraph, saveFileDialog.FileName, compression, delegate(IAsyncResult a) {
          try {
            call.EndInvoke(a);
          }
          catch (Exception ex) {
            Auxiliary.ShowErrorMessageBox(ex);
          }
          Invoke(new Action(delegate() {
            operatorGraphViewHost.Enabled = true;
            newOperatorGraphButton.Enabled = openOperatorGraphButton.Enabled = saveOperatorGraphButton.Enabled = true;
            this.Cursor = Cursors.Default;
          }));
        }, null);
      }
    }
  }
}
