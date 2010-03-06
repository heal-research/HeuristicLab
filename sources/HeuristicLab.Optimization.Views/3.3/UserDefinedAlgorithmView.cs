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

    protected override void DeregisterContentEvents() {
      Content.OperatorGraphChanged -= new EventHandler(Content_OperatorGraphChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.OperatorGraphChanged += new EventHandler(Content_OperatorGraphChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        operatorGraphViewHost.Content = null;
        globalScopeView.Content = null;
      } else {
        operatorGraphViewHost.ViewType = null;
        operatorGraphViewHost.Content = Content.OperatorGraph;
        globalScopeView.Content = Content.GlobalScope;
      }
    }

    private void Content_OperatorGraphChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_OperatorGraphChanged), sender, e);
      else {
        operatorGraphViewHost.ViewType = null;
        operatorGraphViewHost.Content = Content.OperatorGraph;
      }
    }

    private void newOperatorGraphButton_Click(object sender, EventArgs e) {
      Content.OperatorGraph = new OperatorGraph();
    }
    private void openOperatorGraphButton_Click(object sender, EventArgs e) {
      openFileDialog.Title = "Open Operator Graph";
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        OperatorGraph operatorGraph = null;
        try {
          operatorGraph = XmlParser.Deserialize(openFileDialog.FileName) as OperatorGraph;
        }
        catch (Exception ex) {
          Auxiliary.ShowErrorMessageBox(ex);
        }
        if (operatorGraph == null)
          MessageBox.Show(this, "The selected file does not contain an operator graph.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
        else
          Content.OperatorGraph = operatorGraph;
      }
    }
    private void saveOperatorGraphButton_Click(object sender, EventArgs e) {
      saveFileDialog.Title = "Save Operator Graph";
      if (saveFileDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          if (saveFileDialog.FilterIndex == 1)
            XmlGenerator.Serialize(Content.OperatorGraph, saveFileDialog.FileName, 0);
          else
            XmlGenerator.Serialize(Content.OperatorGraph, saveFileDialog.FileName, 9);
        }
        catch (Exception ex) {
          Auxiliary.ShowErrorMessageBox(ex);
        }
      }
    }
  }
}
