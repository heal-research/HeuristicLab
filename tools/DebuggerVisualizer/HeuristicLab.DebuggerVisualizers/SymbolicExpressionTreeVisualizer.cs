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
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views;
using HeuristicLab.Persistence.Default.Xml;
using Microsoft.VisualStudio.DebuggerVisualizers;

[assembly: DebuggerVisualizer(
typeof(HeuristicLab.DebuggerVisualizers.SymbolicExpressionTreeVisualizer),
typeof(HeuristicLab.DebuggerVisualizers.HeuristicLabVisualizerObjectSource),
Target = typeof(HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.SymbolicExpressionTreeNode),
Description = "Visual Symbolic Expression Tree Node Debugger")]

namespace HeuristicLab.DebuggerVisualizers {
  public class SymbolicExpressionTreeVisualizer : DialogDebuggerVisualizer {
    public SymbolicExpressionTreeVisualizer() : base() { }

    protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider) {
      if (windowService == null)
        throw new ArgumentNullException("windowService");
      if (objectProvider == null)
        throw new ArgumentNullException("objectProvider");

      ISymbolicExpressionTreeNode treeNode;
      byte[] serializedObjectByteArray = objectProvider.GetObject() as byte[];
      using (MemoryStream memStream = new MemoryStream(serializedObjectByteArray)) {
        treeNode = XmlParser.Deserialize<ISymbolicExpressionTreeNode>(memStream);
      }

      using (Form displayForm = new Form()) {
        var graphicalSymbolicExpressionTreeChart = new SymbolicExpressionTreeChart();
        graphicalSymbolicExpressionTreeChart.Tree = new SymbolicExpressionTree(treeNode);
        graphicalSymbolicExpressionTreeChart.Dock = DockStyle.Fill;

        displayForm.Controls.Add(graphicalSymbolicExpressionTreeChart);
        displayForm.Text = treeNode.ToString();

        windowService.ShowDialog(displayForm);
      }
    }

    public static void TestShowVisualizer(object objectToVisualize) {
      VisualizerDevelopmentHost visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(SymbolicExpressionTreeVisualizer), typeof(HeuristicLabVisualizerObjectSource));
      visualizerHost.ShowVisualizer();
    }
  }
}
