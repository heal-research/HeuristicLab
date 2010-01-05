#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.SimOpt {
  public partial class SimOptProblemInjectorView : ViewBase {
    public SimOptProblemInjector SimOptProblemInjector {
      get { return (SimOptProblemInjector)base.Item; }
      set { base.Item = value; }
    }

    public SimOptProblemInjectorView() {
      InitializeComponent();
      UpdateDataTypeTreeView();
    }

    public SimOptProblemInjectorView(SimOptProblemInjector simOptProblemInjector)
      : this() {
      SimOptProblemInjector = simOptProblemInjector;
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (SimOptProblemInjector == null) {
        geneNameStringDataView.Enabled = false;
        geneNameStringDataView.StringData = null;
        objectParameterDataView.Enabled = false;
        objectParameterDataView.ConstrainedItemList = null;
        maximizationBoolDataView.Enabled = false;
        maximizationBoolDataView.BoolData = null;
      } else {
        objectParameterDataView.ConstrainedItemList = SimOptProblemInjector.Parameters;
        objectParameterDataView.Enabled = true;
        geneNameStringDataView.StringData = SimOptProblemInjector.GeneName;
        geneNameStringDataView.Enabled = true;
        maximizationBoolDataView.BoolData = SimOptProblemInjector.Maximization;
        maximizationBoolDataView.Enabled = true;
      }
    }

    private void UpdateDataTypeTreeView() {
      dataTypeTreeView.Nodes.Clear();
      foreach (IPluginDescription plugin in ApplicationManager.Manager.Plugins) {
        TreeNode pluginNode = new TreeNode(plugin.Name);
        pluginNode.Tag = null;

        foreach (Type type in ApplicationManager.Manager.GetTypes(typeof(IObjectData), plugin)) {
          if (!type.IsAbstract) {
            TreeNode itemNode = new TreeNode();
            itemNode.Text = type.Name;
            itemNode.Tag = type;
            pluginNode.Nodes.Add(itemNode);
          }
        }
        if (pluginNode.Nodes.Count > 0) {
          dataTypeTreeView.Nodes.Add(pluginNode);
        }
      }
      if (dataTypeTreeView.Nodes.Count == 0) {
        dataTypeTreeView.Enabled = false;
        dataTypeTreeView.Nodes.Add(new TreeNode("No item types available"));
      }
    }

    private void dataTypeTreeView_ItemDrag(object sender, ItemDragEventArgs e) {
      DoDragDrop(e.Item, DragDropEffects.Copy);
    }

    private void objectParameterDataView_DragDrop(object sender, DragEventArgs e) {
      TreeNode node = (TreeNode)e.Data.GetData(typeof(TreeNode));
      IVariable var = new Variable();
      var.Name = "Unnamed";
      var.Value = (IItem)Activator.CreateInstance((Type)node.Tag);
      ICollection<IConstraint> tmp;
      objectParameterDataView.ConstrainedItemList.TryAdd(var, out tmp);
    }

    private void objectParameterDataView_DragEnter(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.Copy | DragDropEffects.Move;
    }
  }
}
