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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.MultiVariate.Regression.Symbolic {
  /// <summary>
  /// Represents a solution for a symbolic vector regression problem which can be visualized in the GUI.
  /// </summary>
  [Item("SymbolicVectorRegressionSolution", "Represents a solution for a symbolic vector regression problems which can be visualized in the GUI.")]
  [StorableClass]
  public sealed class SymbolicVectorRegressionSolution : NamedItem, IMultiVariateDataAnalysisSolution {
    [Storable]
    private Dictionary<string, SymbolicRegressionSolution> regressionSolutions;

    public IEnumerable<string> TargetVariables {
      get { return regressionSolutions.Keys; }
    }

    public SymbolicVectorRegressionSolution() : base() { }
    public SymbolicVectorRegressionSolution(MultiVariateDataAnalysisProblemData problemData, SymbolicExpressionTree tree, ISymbolicExpressionTreeInterpreter interpreter)
      : base() {
      var selectedTargetVariables = (from targetVariable in problemData.TargetVariables
                                     where problemData.TargetVariables.ItemChecked(targetVariable)
                                     select targetVariable.Value).ToArray();


      regressionSolutions = new Dictionary<string, SymbolicRegressionSolution>();
      for (int i = 0; i < selectedTargetVariables.Length; i++) {
        SymbolicExpressionTree componentTree = (SymbolicExpressionTree)tree.Clone();
        List<SymbolicExpressionTreeNode> componentBranches = new List<SymbolicExpressionTreeNode>(componentTree.Root.SubTrees[0].SubTrees);
        // use only the i-th vector component
        while (componentTree.Root.SubTrees[0].SubTrees.Count > 0) componentTree.Root.SubTrees[0].RemoveSubTree(0);
        componentTree.Root.SubTrees[0].AddSubTree(componentBranches[i]);
        var componentSolution = CreateSymbolicRegressionSolution(problemData, componentTree, selectedTargetVariables[i], interpreter);
        regressionSolutions.Add(selectedTargetVariables[i], componentSolution);
      }

    }

    private static SymbolicRegressionSolution CreateSymbolicRegressionSolution(
      MultiVariateDataAnalysisProblemData problemData,
      SymbolicExpressionTree symbolicExpressionTree,
      string targetVariable,
      ISymbolicExpressionTreeInterpreter interpreter) {
      return new SymbolicRegressionSolution(problemData.ConvertToDataAnalysisProblemData(targetVariable), new SymbolicRegressionModel(interpreter, symbolicExpressionTree), double.NegativeInfinity, double.PositiveInfinity);
    }

    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Function; }
    }

    public SymbolicRegressionSolution GetModelFor(string targetVariable) {
      return regressionSolutions[targetVariable];
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      SymbolicVectorRegressionSolution clone = (SymbolicVectorRegressionSolution)base.Clone(cloner);
      clone.regressionSolutions = new Dictionary<string, SymbolicRegressionSolution>(regressionSolutions.Count);
      foreach (var pair in regressionSolutions)
        clone.regressionSolutions.Add(pair.Key, (SymbolicRegressionSolution)cloner.Clone(pair.Value));
      return clone;
    }
  }
}
