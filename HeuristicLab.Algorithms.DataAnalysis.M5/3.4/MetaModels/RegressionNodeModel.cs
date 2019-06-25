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
using HeuristicLab.Common;                                                  
using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("C20C7DF1-CE33-4CCD-88D3-E145CFE239AC")]
  public class RegressionNodeModel : RegressionModel {
    #region Properties
    public double PruningStrength = double.NaN;
    private IReadOnlyList<string> Variables {
      get {
        if (IsLeaf && Model == null) return new List<string>();
        if (IsLeaf) return Model.VariablesUsedForPrediction.ToList();
        var set = new HashSet<string> {SplitAttribute};
        var vl = Left.Variables;
        var vr = Right.Variables;
        for (var i = 0; i < vl.Count; i++) set.Add(vl[i]);
        for (var i = 0; i < vr.Count; i++) set.Add(vr[i]);
        return set.ToList();
      }
    }
    [Storable]
    internal int NumSamples { get; private set; }
    [Storable]
    internal bool IsLeaf { get; private set; }
    [Storable]
    private IRegressionModel Model { get; set; }

    [Storable]
    public string SplitAttribute { get; private set; }
    [Storable]
    public double SplitValue { get; private set; }
    [Storable]
    public RegressionNodeModel Left { get; private set; }
    [Storable]
    public RegressionNodeModel Right { get; private set; }
    [Storable]
    public RegressionNodeModel Parent { get; private set; }
    #endregion

    #region HLConstructors
    [StorableConstructor]
    protected RegressionNodeModel(StorableConstructorFlag _) : base(_) { }
    protected RegressionNodeModel(RegressionNodeModel original, Cloner cloner) : base(original, cloner) {
      IsLeaf = original.IsLeaf;
      Model = cloner.Clone(original.Model);
      SplitValue = original.SplitValue;
      SplitAttribute = original.SplitAttribute;
      Left = cloner.Clone(original.Left);
      Right = cloner.Clone(original.Right);
      Parent = cloner.Clone(original.Parent);
      NumSamples = original.NumSamples;
    }
    private RegressionNodeModel(string targetAttr) : base(targetAttr) {
      IsLeaf = true;
    }
    private RegressionNodeModel(RegressionNodeModel parent) : this(parent.TargetVariable) {
      Parent = parent;
      IsLeaf = true;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RegressionNodeModel(this, cloner);
    }
    public static RegressionNodeModel CreateNode(string targetAttr, RegressionTreeParameters regressionTreeParams) {
      return regressionTreeParams.LeafModel.ProvidesConfidence ? new ConfidenceRegressionNodeModel(targetAttr) : new RegressionNodeModel(targetAttr);
    }
    private static RegressionNodeModel CreateNode(RegressionNodeModel parent, RegressionTreeParameters regressionTreeParams) {
      return regressionTreeParams.LeafModel.ProvidesConfidence ? new ConfidenceRegressionNodeModel(parent) : new RegressionNodeModel(parent);
    }
    #endregion

    #region RegressionModel
    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return Variables; }
    }
    public override IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      if (!IsLeaf) return rows.Select(row => GetEstimatedValue(dataset, row));
      if (Model == null) throw new NotSupportedException("The model has not been built correctly");
      return Model.GetEstimatedValues(dataset, rows);
    }
    public override IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new RegressionSolution(this, problemData);
    }
    #endregion

    internal void Split(RegressionTreeParameters regressionTreeParams, string splitAttribute, double splitValue, int numSamples) {
      NumSamples = numSamples;
      SplitAttribute = splitAttribute;
      SplitValue = splitValue;
      Left = CreateNode(this, regressionTreeParams);
      Right = CreateNode(this, regressionTreeParams);
      IsLeaf = false;
    }

    internal void ToLeaf() {
      IsLeaf = true;
      Right = null;
      Left = null;
    }

    internal void SetLeafModel(IRegressionModel model) {
      Model = model;
    }

    internal IEnumerable<RegressionNodeModel> EnumerateNodes() {
      var queue = new Queue<RegressionNodeModel>();
      queue.Enqueue(this);
      while (queue.Count != 0) {
        var cur = queue.Dequeue();
        yield return cur;
        if (cur.Left == null && cur.Right == null) continue;
        if (cur.Left != null) queue.Enqueue(cur.Left);
        if (cur.Right != null) queue.Enqueue(cur.Right);
      }
    }

    #region Helpers
    private double GetEstimatedValue(IDataset dataset, int row) {
      if (!IsLeaf) return (dataset.GetDoubleValue(SplitAttribute, row) <= SplitValue ? Left : Right).GetEstimatedValue(dataset, row);
      if (Model == null) throw new NotSupportedException("The model has not been built correctly");
      return Model.GetEstimatedValues(dataset, new[] {row}).First();
    }
    #endregion

    [StorableType("1FF9E216-6AF1-4282-A7EF-3FA0C1DB29C8")]
    private sealed class ConfidenceRegressionNodeModel : RegressionNodeModel, IConfidenceRegressionModel {
      #region HLConstructors
      [StorableConstructor]
      private ConfidenceRegressionNodeModel(StorableConstructorFlag _) : base(_) { }
      private ConfidenceRegressionNodeModel(ConfidenceRegressionNodeModel original, Cloner cloner) : base(original, cloner) { }
      public ConfidenceRegressionNodeModel(string targetAttr) : base(targetAttr) { }
      public ConfidenceRegressionNodeModel(RegressionNodeModel parent) : base(parent) { }
      public override IDeepCloneable Clone(Cloner cloner) {
        return new ConfidenceRegressionNodeModel(this, cloner);
      }
      #endregion

      public IEnumerable<double> GetEstimatedVariances(IDataset dataset, IEnumerable<int> rows) {
        return IsLeaf ? ((IConfidenceRegressionModel)Model).GetEstimatedVariances(dataset, rows) : rows.Select(row => GetEstimatedVariance(dataset, row));
      }

      private double GetEstimatedVariance(IDataset dataset, int row) {
        return !IsLeaf ? ((IConfidenceRegressionModel)(dataset.GetDoubleValue(SplitAttribute, row) <= SplitValue ? Left : Right)).GetEstimatedVariances(dataset, row.ToEnumerable()).Single() : ((IConfidenceRegressionModel)Model).GetEstimatedVariances(dataset, new[] {row}).First();
      }

      public override IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
        return new ConfidenceRegressionSolution(this, problemData);
      }
    }
  }
}