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
      Content.EngineChanged -= new System.EventHandler(Content_EngineChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.EngineChanged += new System.EventHandler(Content_EngineChanged);
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
        engineTypes.Insert(0, null);
        engineComboBox.Items.Insert(0, "-");
      }

      if (Content == null) {
        engineComboBox.Enabled = false;
        engineViewHost.Content = null;
        engineViewHost.Enabled = false;
      } else {
        engineComboBox.Enabled = true;
        if (Content.Engine == null)
          engineComboBox.SelectedIndex = engineTypes.IndexOf(null);
        else
          engineComboBox.SelectedIndex = engineTypes.IndexOf(Content.Engine.GetType());
        engineViewHost.Enabled = true;
        engineViewHost.Content = Content.Engine;
      }
    }

    protected void Content_EngineChanged(object sender, System.EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_EngineChanged), sender, e);
      else {
        if (Content.Engine == null)
          engineComboBox.SelectedIndex = engineTypes.IndexOf(null);
        else
          engineComboBox.SelectedIndex = engineTypes.IndexOf(Content.Engine.GetType());
        engineViewHost.Content = Content.Engine;
      }
    }

    protected void engineComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (Content != null) {
        Type t = engineTypes[engineComboBox.SelectedIndex];
        if (t == null)
          Content.Engine = null;
        else
          Content.Engine = (IEngine)Activator.CreateInstance(t);
      }
    }

    protected void createUserDefinedAlgorithmButton_Click(object sender, EventArgs e) {
      MainFormManager.CreateDefaultView(Content.CreateUserDefinedAlgorithm()).Show();
    }
  }
}
