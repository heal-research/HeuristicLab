#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Represents a nearest neighbour model for regression and classification
  /// </summary>
  [StorableClass]
  [Item("NearestNeighbourModel", "Represents a nearest neighbour model for regression and classification.")]
  public sealed class NearestNeighbourModel : ClassificationModel, INearestNeighbourModel {

    private readonly object kdTreeLockObject = new object();
    private alglib.nearestneighbor.kdtree kdTree;
    public alglib.nearestneighbor.kdtree KDTree {
      get { return kdTree; }
      set {
        if (value != kdTree) {
          if (value == null) throw new ArgumentNullException();
          kdTree = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }


    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return allowedInputVariables; }
    }

    [Storable]
    private string[] allowedInputVariables;
    [Storable]
    private double[] classValues;
    [Storable]
    private int k;
    [Storable(DefaultValue = null)]
    private double[] weights; // not set for old versions loaded from disk
    [Storable(DefaultValue = null)]
    private double[] offsets; // not set for old versions loaded from disk

    [StorableConstructor]
    private NearestNeighbourModel(bool deserializing)
      : base(deserializing) {
      if (deserializing)
        kdTree = new alglib.nearestneighbor.kdtree();
    }
    private NearestNeighbourModel(NearestNeighbourModel original, Cloner cloner)
      : base(original, cloner) {
      kdTree = new alglib.nearestneighbor.kdtree();
      kdTree.approxf = original.kdTree.approxf;
      kdTree.boxmax = (double[])original.kdTree.boxmax.Clone();
      kdTree.boxmin = (double[])original.kdTree.boxmin.Clone();
      kdTree.buf = (double[])original.kdTree.buf.Clone();
      kdTree.curboxmax = (double[])original.kdTree.curboxmax.Clone();
      kdTree.curboxmin = (double[])original.kdTree.curboxmin.Clone();
      kdTree.curdist = original.kdTree.curdist;
      kdTree.debugcounter = original.kdTree.debugcounter;
      kdTree.idx = (int[])original.kdTree.idx.Clone();
      kdTree.kcur = original.kdTree.kcur;
      kdTree.kneeded = original.kdTree.kneeded;
      kdTree.n = original.kdTree.n;
      kdTree.nodes = (int[])original.kdTree.nodes.Clone();
      kdTree.normtype = original.kdTree.normtype;
      kdTree.nx = original.kdTree.nx;
      kdTree.ny = original.kdTree.ny;
      kdTree.r = (double[])original.kdTree.r.Clone();
      kdTree.rneeded = original.kdTree.rneeded;
      kdTree.selfmatch = original.kdTree.selfmatch;
      kdTree.splits = (double[])original.kdTree.splits.Clone();
      kdTree.tags = (int[])original.kdTree.tags.Clone();
      kdTree.x = (double[])original.kdTree.x.Clone();
      kdTree.xy = (double[,])original.kdTree.xy.Clone();

      k = original.k;
      isCompatibilityLoaded = original.IsCompatibilityLoaded;
      if (!IsCompatibilityLoaded) {
        weights = new double[original.weights.Length];
        Array.Copy(original.weights, weights, weights.Length);
        offsets = new double[original.offsets.Length];
        Array.Copy(original.offsets, this.offsets, this.offsets.Length);
      }
      allowedInputVariables = (string[])original.allowedInputVariables.Clone();
      if (original.classValues != null)
        this.classValues = (double[])original.classValues.Clone();
    }
    public NearestNeighbourModel(IDataset dataset, IEnumerable<int> rows, int k, string targetVariable, IEnumerable<string> allowedInputVariables, IEnumerable<double> weights = null, double[] classValues = null)
      : base(targetVariable) {
      Name = ItemName;
      Description = ItemDescription;
      this.k = k;
      this.allowedInputVariables = allowedInputVariables.ToArray();
      double[,] inputMatrix;
      if (IsCompatibilityLoaded) {
        // no scaling
        inputMatrix = dataset.ToArray(
          this.allowedInputVariables.Concat(new string[] { targetVariable }),
          rows);
      } else {
        this.offsets = this.allowedInputVariables
          .Select(name => dataset.GetDoubleValues(name, rows).Average() * -1)
          .Concat(new double[] { 0 }) // no offset for target variable
          .ToArray();
        if (weights == null) {
          // automatic determination of weights (all features should have variance = 1)
          this.weights = this.allowedInputVariables
            .Select(name => {
              var pop = dataset.GetDoubleValues(name, rows).StandardDeviationPop();
              return  pop.IsAlmost(0) ? 1.0 : 1.0/pop;
            })
            .Concat(new double[] { 1.0 }) // no scaling for target variable
            .ToArray();
        } else {
          // user specified weights (+ 1 for target)
          this.weights = weights.Concat(new double[] { 1.0 }).ToArray();
          if (this.weights.Length - 1 != this.allowedInputVariables.Length)
            throw new ArgumentException("The number of elements in the weight vector must match the number of input variables");
        }
        inputMatrix = CreateScaledData(dataset, this.allowedInputVariables.Concat(new string[] { targetVariable }), rows, this.offsets, this.weights);
      }

      if (inputMatrix.ContainsNanOrInfinity())
        throw new NotSupportedException(
          "Nearest neighbour model does not support NaN or infinity values in the input dataset.");

      this.kdTree = new alglib.nearestneighbor.kdtree();

      var nRows = inputMatrix.GetLength(0);
      var nFeatures = inputMatrix.GetLength(1) - 1;

      if (classValues != null) {
        this.classValues = (double[])classValues.Clone();
        int nClasses = classValues.Length;
        // map original class values to values [0..nClasses-1]
        var classIndices = new Dictionary<double, double>();
        for (int i = 0; i < nClasses; i++)
          classIndices[classValues[i]] = i;

        for (int row = 0; row < nRows; row++) {
          inputMatrix[row, nFeatures] = classIndices[inputMatrix[row, nFeatures]];
        }
      }
      alglib.nearestneighbor.kdtreebuild(inputMatrix, nRows, inputMatrix.GetLength(1) - 1, 1, 2, kdTree);
    }

    private static double[,] CreateScaledData(IDataset dataset, IEnumerable<string> variables, IEnumerable<int> rows, double[] offsets, double[] factors) {
      var transforms =
        variables.Select(
          (_, colIdx) =>
            new LinearTransformation(variables) { Addend = offsets[colIdx] * factors[colIdx], Multiplier = factors[colIdx] });
      return dataset.ToArray(variables, transforms, rows);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NearestNeighbourModel(this, cloner);
    }

    public IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      double[,] inputData;
      if (IsCompatibilityLoaded) {
        inputData = dataset.ToArray(allowedInputVariables, rows);
      } else {
        inputData = CreateScaledData(dataset, allowedInputVariables, rows, offsets, weights);
      }

      int n = inputData.GetLength(0);
      int columns = inputData.GetLength(1);
      double[] x = new double[columns];
      double[] dists = new double[k];
      double[,] neighbours = new double[k, columns + 1];

      for (int row = 0; row < n; row++) {
        for (int column = 0; column < columns; column++) {
          x[column] = inputData[row, column];
        }
        int numNeighbours;
        lock (kdTreeLockObject) { // gkronber: the following calls change the kdTree data structure
          numNeighbours = alglib.nearestneighbor.kdtreequeryknn(kdTree, x, k, false);
          alglib.nearestneighbor.kdtreequeryresultsdistances(kdTree, ref dists);
          alglib.nearestneighbor.kdtreequeryresultsxy(kdTree, ref neighbours);
        }

        double distanceWeightedValue = 0.0;
        double distsSum = 0.0;
        for (int i = 0; i < numNeighbours; i++) {
          distanceWeightedValue += neighbours[i, columns] / dists[i];
          distsSum += 1.0 / dists[i];
        }
        yield return distanceWeightedValue / distsSum;
      }
    }

    public override IEnumerable<double> GetEstimatedClassValues(IDataset dataset, IEnumerable<int> rows) {
      if (classValues == null) throw new InvalidOperationException("No class values are defined.");
      double[,] inputData;
      if (IsCompatibilityLoaded) {
        inputData = dataset.ToArray(allowedInputVariables, rows);
      } else {
        inputData = CreateScaledData(dataset, allowedInputVariables, rows, offsets, weights);
      }
      int n = inputData.GetLength(0);
      int columns = inputData.GetLength(1);
      double[] x = new double[columns];
      int[] y = new int[classValues.Length];
      double[] dists = new double[k];
      double[,] neighbours = new double[k, columns + 1];

      for (int row = 0; row < n; row++) {
        for (int column = 0; column < columns; column++) {
          x[column] = inputData[row, column];
        }
        int numNeighbours;
        lock (kdTreeLockObject) {
          // gkronber: the following calls change the kdTree data structure
          numNeighbours = alglib.nearestneighbor.kdtreequeryknn(kdTree, x, k, false);
          alglib.nearestneighbor.kdtreequeryresultsdistances(kdTree, ref dists);
          alglib.nearestneighbor.kdtreequeryresultsxy(kdTree, ref neighbours);
        }
        Array.Clear(y, 0, y.Length);
        for (int i = 0; i < numNeighbours; i++) {
          int classValue = (int)Math.Round(neighbours[i, columns]);
          y[classValue]++;
        }

        // find class for with the largest probability value
        int maxProbClassIndex = 0;
        double maxProb = y[0];
        for (int i = 1; i < y.Length; i++) {
          if (maxProb < y[i]) {
            maxProb = y[i];
            maxProbClassIndex = i;
          }
        }
        yield return classValues[maxProbClassIndex];
      }
    }


    IRegressionSolution IRegressionModel.CreateRegressionSolution(IRegressionProblemData problemData) {
      return new NearestNeighbourRegressionSolution(this, new RegressionProblemData(problemData));
    }
    public override IClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData) {
      return new NearestNeighbourClassificationSolution(this, new ClassificationProblemData(problemData));
    }

    #region events
    public event EventHandler Changed;
    private void OnChanged(EventArgs e) {
      var handlers = Changed;
      if (handlers != null)
        handlers(this, e);
    }
    #endregion


    // BackwardsCompatibility3.3
    #region Backwards compatible code, remove with 3.4

    private bool isCompatibilityLoaded = false; // new kNN models have the value false, kNN models loaded from disc have the value true
    [Storable(DefaultValue = true)]
    public bool IsCompatibilityLoaded {
      get { return isCompatibilityLoaded; }
      set { isCompatibilityLoaded = value; }
    }
    #endregion
    #region persistence
    [Storable]
    public double KDTreeApproxF {
      get { return kdTree.approxf; }
      set { kdTree.approxf = value; }
    }
    [Storable]
    public double[] KDTreeBoxMax {
      get { return kdTree.boxmax; }
      set { kdTree.boxmax = value; }
    }
    [Storable]
    public double[] KDTreeBoxMin {
      get { return kdTree.boxmin; }
      set { kdTree.boxmin = value; }
    }
    [Storable]
    public double[] KDTreeBuf {
      get { return kdTree.buf; }
      set { kdTree.buf = value; }
    }
    [Storable]
    public double[] KDTreeCurBoxMax {
      get { return kdTree.curboxmax; }
      set { kdTree.curboxmax = value; }
    }
    [Storable]
    public double[] KDTreeCurBoxMin {
      get { return kdTree.curboxmin; }
      set { kdTree.curboxmin = value; }
    }
    [Storable]
    public double KDTreeCurDist {
      get { return kdTree.curdist; }
      set { kdTree.curdist = value; }
    }
    [Storable]
    public int KDTreeDebugCounter {
      get { return kdTree.debugcounter; }
      set { kdTree.debugcounter = value; }
    }
    [Storable]
    public int[] KDTreeIdx {
      get { return kdTree.idx; }
      set { kdTree.idx = value; }
    }
    [Storable]
    public int KDTreeKCur {
      get { return kdTree.kcur; }
      set { kdTree.kcur = value; }
    }
    [Storable]
    public int KDTreeKNeeded {
      get { return kdTree.kneeded; }
      set { kdTree.kneeded = value; }
    }
    [Storable]
    public int KDTreeN {
      get { return kdTree.n; }
      set { kdTree.n = value; }
    }
    [Storable]
    public int[] KDTreeNodes {
      get { return kdTree.nodes; }
      set { kdTree.nodes = value; }
    }
    [Storable]
    public int KDTreeNormType {
      get { return kdTree.normtype; }
      set { kdTree.normtype = value; }
    }
    [Storable]
    public int KDTreeNX {
      get { return kdTree.nx; }
      set { kdTree.nx = value; }
    }
    [Storable]
    public int KDTreeNY {
      get { return kdTree.ny; }
      set { kdTree.ny = value; }
    }
    [Storable]
    public double[] KDTreeR {
      get { return kdTree.r; }
      set { kdTree.r = value; }
    }
    [Storable]
    public double KDTreeRNeeded {
      get { return kdTree.rneeded; }
      set { kdTree.rneeded = value; }
    }
    [Storable]
    public bool KDTreeSelfMatch {
      get { return kdTree.selfmatch; }
      set { kdTree.selfmatch = value; }
    }
    [Storable]
    public double[] KDTreeSplits {
      get { return kdTree.splits; }
      set { kdTree.splits = value; }
    }
    [Storable]
    public int[] KDTreeTags {
      get { return kdTree.tags; }
      set { kdTree.tags = value; }
    }
    [Storable]
    public double[] KDTreeX {
      get { return kdTree.x; }
      set { kdTree.x = value; }
    }
    [Storable]
    public double[,] KDTreeXY {
      get { return kdTree.xy; }
      set { kdTree.xy = value; }
    }
    #endregion
  }
}
