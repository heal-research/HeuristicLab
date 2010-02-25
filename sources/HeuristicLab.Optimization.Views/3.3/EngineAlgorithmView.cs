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

namespace HeuristicLab.Optimization.Views {
  /// <summary>
  /// The base class for visual representations of items.
  /// </summary>
  [Content(typeof(EngineAlgorithm), true)]
  public partial class EngineAlgorithmView : AlgorithmView {
    private TypeSelectorDialog engineTypeSelectorDialog;

    public new EngineAlgorithm Content {
      get { return (EngineAlgorithm)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public EngineAlgorithmView() {
      InitializeComponent();
    }
    /// <summary>
    /// Intializes a new instance of <see cref="ItemBaseView"/> with the given <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The item that should be displayed.</param>
    public EngineAlgorithmView(EngineAlgorithm content)
      : this() {
      Content = content;
    }

    protected override void DeregisterContentEvents() {
      Content.EngineChanged -= new System.EventHandler(Content_EngineChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.EngineChanged += new System.EventHandler(Content_EngineChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        engineTextBox.Text = "-";
        engineTextBox.Enabled = false;
        setEngineButton.Enabled = false;
      } else {
        engineTextBox.Text = Content.Engine == null ? "-" : Content.Engine.ToString();
        engineTextBox.Enabled = true;
        setEngineButton.Enabled = true;
      }
    }

    protected void Content_EngineChanged(object sender, System.EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_EngineChanged), sender, e);
      else
        engineTextBox.Text = Content.Engine == null ? "-" : Content.Engine.ToString();
    }

    protected void setEngineButton_Click(object sender, System.EventArgs e) {
      if (engineTypeSelectorDialog == null) {
        engineTypeSelectorDialog = new TypeSelectorDialog();
        engineTypeSelectorDialog.Caption = "Select Engine";
        engineTypeSelectorDialog.TypeSelector.Configure(typeof(IEngine), false, false);
      }
      if (engineTypeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        Content.Engine = (IEngine)engineTypeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
      }
    }

    protected void engineTextBox_DoubleClick(object sender, System.EventArgs e) {
      if (Content.Engine != null)
        MainFormManager.CreateDefaultView(Content.Engine).Show();
    }

    protected void createUserDefinedAlgorithmButton_Click(object sender, EventArgs e) {
      MainFormManager.CreateDefaultView(Content.CreateUserDefinedAlgorithm()).Show();
    }
  }
}
