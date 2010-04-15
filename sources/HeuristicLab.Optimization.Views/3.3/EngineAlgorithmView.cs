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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimization.Views {
  /// <summary>
  /// The base class for visual representations of items.
  /// </summary>
  [View("EngineAlgorithm View")]
  [Content(typeof(EngineAlgorithm), true)]
  public partial class EngineAlgorithmView : AlgorithmView {
    private List<Type> engineTypes;

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
      Content.EngineChanged -= new EventHandler(Content_EngineChanged);
      Content.OperatorGraphChanged -= new EventHandler(Content_OperatorGraphChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.EngineChanged += new EventHandler(Content_EngineChanged);
      Content.OperatorGraphChanged += new EventHandler(Content_OperatorGraphChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();

      if (engineTypes == null) {
        engineTypes = ApplicationManager.Manager.GetTypes(typeof(IEngine)).
                      OrderBy(x => {
                        string name = ItemAttribute.GetName(x);
                        if (name != null) return name;
                        else return x.GetPrettyName();
                      }).ToList();
        foreach (Type t in engineTypes) {
          string name = ItemAttribute.GetName(t);
          if (name == null) name = t.GetPrettyName();
          engineComboBox.Items.Add(name);
        }
      }

      if (Content == null) {
        engineViewHost.Content = null;
        createUserDefinedAlgorithmButton.Enabled = false;
        operatorGraphViewHost.Content = null;
      } else {
        if (Content.Engine == null)
          engineComboBox.SelectedIndex = -1;
        else
          engineComboBox.SelectedIndex = engineTypes.IndexOf(Content.Engine.GetType());
        engineViewHost.ViewType = null;
        engineViewHost.Content = Content.Engine;
        operatorGraphViewHost.ViewType = null;
        operatorGraphViewHost.Content = Content.OperatorGraph;
        createUserDefinedAlgorithmButton.Enabled = true;
      }
    }

    protected override void Content_ExecutionStateChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ExecutionStateChanged), sender, e);
      else {
        createUserDefinedAlgorithmButton.Enabled = Content.ExecutionState != ExecutionState.Started;
        engineComboBox.Enabled = Content.ExecutionState != ExecutionState.Started;
        engineViewHost.Enabled = Content.ExecutionState != ExecutionState.Started;
        operatorGraphViewHost.Enabled = Content.ExecutionState != ExecutionState.Started;
        base.Content_ExecutionStateChanged(sender, e);
      }
    }
    protected virtual void Content_EngineChanged(object sender, System.EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_EngineChanged), sender, e);
      else {
        if (Content.Engine == null)
          engineComboBox.SelectedIndex = -1;
        else
          engineComboBox.SelectedIndex = engineTypes.IndexOf(Content.Engine.GetType());
        engineViewHost.ViewType = null;
        engineViewHost.Content = Content.Engine;
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

    protected virtual void engineComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (Content != null) {
        if (engineComboBox.SelectedIndex == -1)
          Content.Engine = null;
        else {
          Type t = engineTypes[engineComboBox.SelectedIndex];
          if ((Content.Engine == null) || (Content.Engine.GetType() != t))
            Content.Engine = (IEngine)Activator.CreateInstance(t);
        }
      }
    }

    protected virtual void createUserDefinedAlgorithmButton_Click(object sender, EventArgs e) {
      MainFormManager.CreateDefaultView(Content.CreateUserDefinedAlgorithm()).Show();
    }
  }
}
