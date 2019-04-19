#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("22DCCF28-8943-4622-BBD3-B2AB04F28C36")]
  [Item("SplitterBase", "Abstract base class for splitters")]
  public abstract class SplitterBase : ParameterizedNamedItem, ISplitter {
    public const string SplittingStateVariableName = "SplittingState";

    #region Constructors & Cloning
    [StorableConstructor]
    protected SplitterBase(StorableConstructorFlag _) { }
    protected SplitterBase(SplitterBase original, Cloner cloner) : base(original, cloner) { }
    public SplitterBase() { }
    #endregion

    #region ISplitType
    public void Initialize(IScope states) {
      states.Variables.Add(new Variable(SplittingStateVariableName, new SplittingState()));
    }

    public void Split(RegressionNodeTreeModel tree, IReadOnlyList<int> trainingRows, IScope stateScope, CancellationToken cancellationToken) {
      var regressionTreeParams = (RegressionTreeParameters)stateScope.Variables[M5Regression.RegressionTreeParameterVariableName].Value;
      var splittingState = (SplittingState)stateScope.Variables[SplittingStateVariableName].Value;
      var variables = regressionTreeParams.AllowedInputVariables.ToArray();
      var target = regressionTreeParams.TargetVariable;

      if (splittingState.Code <= 0) {
        splittingState.nodeQueue.Enqueue(tree.Root);
        splittingState.trainingRowsQueue.Enqueue(trainingRows);
        splittingState.Code = 1;
      }
      while (splittingState.nodeQueue.Count != 0) {
        var n = splittingState.nodeQueue.Dequeue();
        var rows = splittingState.trainingRowsQueue.Dequeue();

        string attr;
        double splitValue;
        var isLeaf = !DecideSplit(new RegressionProblemData(RegressionTreeUtilities.ReduceDataset(regressionTreeParams.Data, rows, variables, target), variables, target), regressionTreeParams.MinLeafSize, out attr, out splitValue);
        if (isLeaf) continue;

        IReadOnlyList<int> leftRows, rightRows;
        RegressionTreeUtilities.SplitRows(rows, regressionTreeParams.Data, attr, splitValue, out leftRows, out rightRows);
        n.Split(regressionTreeParams, attr, splitValue, rows.Count);

        splittingState.nodeQueue.Enqueue(n.Left);
        splittingState.nodeQueue.Enqueue(n.Right);
        splittingState.trainingRowsQueue.Enqueue(leftRows);
        splittingState.trainingRowsQueue.Enqueue(rightRows);
        cancellationToken.ThrowIfCancellationRequested();
      }
    }

    protected virtual bool DecideSplit(IRegressionProblemData splitData, int minLeafSize, out string splitAttr, out double splitValue) {
      var bestPos = -1;
      var bestImpurity = double.MinValue;
      var bestSplitValue = 0.0;
      var bestSplitAttr = string.Empty;
      splitAttr = bestSplitAttr;
      splitValue = bestSplitValue;
      if (splitData.Dataset.Rows < minLeafSize) return false;

      // find best attribute for the splitter
      foreach (var attr in splitData.AllowedInputVariables) {
        int pos;
        double impurity, sValue;
        var sortedData = splitData.Dataset.GetDoubleValues(attr).Zip(splitData.Dataset.GetDoubleValues(splitData.TargetVariable), Tuple.Create).OrderBy(x => x.Item1).ToArray();
        AttributeSplit(sortedData.Select(x => x.Item1).ToArray(), sortedData.Select(x => x.Item2).ToArray(), minLeafSize, out pos, out impurity, out sValue);
        if (!(bestImpurity < impurity)) continue;
        bestImpurity = impurity;
        bestPos = pos;
        bestSplitValue = sValue;
        bestSplitAttr = attr;
      }

      splitAttr = bestSplitAttr;
      splitValue = bestSplitValue;
      //if no suitable split exists => leafNode
      return bestPos + 1 >= minLeafSize && bestPos <= splitData.Dataset.Rows - minLeafSize;
    }

    protected abstract void AttributeSplit(IReadOnlyList<double> attValues, IReadOnlyList<double> targetValues, int minLeafSize, out int position, out double maxImpurity, out double splitValue);
    #endregion

    [StorableType("BC1149FD-370E-4F3A-92F5-6E519736D09A")]
    public class SplittingState : Item {
      [Storable]
      public Queue<RegressionNodeModel> nodeQueue = new Queue<RegressionNodeModel>();
      [Storable]
      public Queue<IReadOnlyList<int>> trainingRowsQueue = new Queue<IReadOnlyList<int>>();

      //State.Code values denote the current action (for pausing)
      //0...nothing has been done;
      //1...splitting nodes;
      [Storable]
      public int Code = 0;

      #region HLConstructors & Cloning
      [StorableConstructor]
      protected SplittingState(StorableConstructorFlag _) : base(_) { }
      protected SplittingState(SplittingState original, Cloner cloner) : base(original, cloner) {
        nodeQueue = new Queue<RegressionNodeModel>(original.nodeQueue.Select(cloner.Clone));
        trainingRowsQueue = new Queue<IReadOnlyList<int>>(original.trainingRowsQueue.Select(x => (IReadOnlyList<int>)x.ToArray()));
        Code = original.Code;
      }
      public SplittingState() { }
      public override IDeepCloneable Clone(Cloner cloner) {
        return new SplittingState(this, cloner);
      }
      #endregion
    }
  }
}