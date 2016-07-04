#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 * and the BEACON Center for the Study of Evolution in Action.
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  // This class implements a greedy decision tree learner which selects splits with the maximum reduction in sum of squared errors.
  // The tree builder also tracks variable relevance metrics based on the splits and improvement after the split.
  // The implementation is tuned for gradient boosting where multiple trees have to be calculated for the same training data 
  // each time with a different target vector. Vectors of idx to allow iteration of intput variables in sorted order are
  // pre-calculated so that optimal thresholds for splits can be calculated in O(n) for each input variable.
  // After each split the row idx are partitioned in a left an right part.
  internal class RegressionTreeBuilder {
    private readonly IRandom random;
    private readonly IRegressionProblemData problemData;

    private readonly int nCols;
    private readonly double[][] x; // all training data (original order from problemData), x is constant
    private double[] originalY; // the original target labels (from problemData), originalY is constant
    private double[] curPred; // current predictions for originalY (in case we are using gradient boosting, otherwise = zeros), only necessary for line search

    private double[] y; // training labels (original order from problemData), y can be changed

    private Dictionary<string, double> sumImprovements; // for variable relevance calculation

    private readonly string[] allowedVariables; // all variables in shuffled order
    private Dictionary<string, int> varName2Index; // maps the variable names to column indexes 
    private int effectiveVars; // number of variables that are used from allowedVariables

    private int effectiveRows; // number of rows that are used from 
    private readonly int[][] sortedIdxAll;
    private readonly int[][] sortedIdx; // random selection from sortedIdxAll (for r < 1.0)

    // helper arrays which are allocated to maximal necessary size only once in the ctor
    private readonly int[] internalIdx, which, leftTmp, rightTmp;
    private readonly double[] outx;
    private readonly int[] outSortedIdx;

    private RegressionTreeModel.TreeNode[] tree; // tree is represented as a flat array of nodes
    private int curTreeNodeIdx; // the index where the next tree node is stored

    // This class represents information about potential splits.
    // For each node generated the best splitting variable and threshold as well as
    // the improvement from the split are stored in a priority queue
    private class PartitionSplits {
      public int ParentNodeIdx { get; set; } // the idx of the leaf node representing this partition 
      public int StartIdx { get; set; } // the start idx of the partition
      public int EndIndex { get; set; } // the end idx of the partition
      public string SplittingVariable { get; set; } // the best splitting variable
      public double SplittingThreshold { get; set; } // the best threshold
      public double SplittingImprovement { get; set; } // the improvement of the split (for priority queue)
    }

    // this list hold partitions with the information about the best split (organized as a sorted queue)
    private readonly IList<PartitionSplits> queue;

    // prepare and allocate buffer variables in ctor
    public RegressionTreeBuilder(IRegressionProblemData problemData, IRandom random) {
      this.problemData = problemData;
      this.random = random;

      var rows = problemData.TrainingIndices.Count();

      this.nCols = problemData.AllowedInputVariables.Count();

      allowedVariables = problemData.AllowedInputVariables.ToArray();
      varName2Index = new Dictionary<string, int>(allowedVariables.Length);
      for (int i = 0; i < allowedVariables.Length; i++) varName2Index.Add(allowedVariables[i], i);

      sortedIdxAll = new int[nCols][];
      sortedIdx = new int[nCols][];
      sumImprovements = new Dictionary<string, double>();
      internalIdx = new int[rows];
      which = new int[rows];
      leftTmp = new int[rows];
      rightTmp = new int[rows];
      outx = new double[rows];
      outSortedIdx = new int[rows];
      queue = new List<PartitionSplits>(100);

      x = new double[nCols][];
      originalY = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, problemData.TrainingIndices).ToArray();
      y = new double[originalY.Length];
      Array.Copy(originalY, y, y.Length); // copy values (originalY is fixed, y is changed in gradient boosting)
      curPred = Enumerable.Repeat(0.0, y.Length).ToArray(); // zeros

      int col = 0;
      foreach (var inputVariable in problemData.AllowedInputVariables) {
        x[col] = problemData.Dataset.GetDoubleValues(inputVariable, problemData.TrainingIndices).ToArray();
        sortedIdxAll[col] = Enumerable.Range(0, rows).OrderBy(r => x[col][r]).ToArray();
        sortedIdx[col] = new int[rows];
        col++;
      }
    }

    // specific interface that allows to specify the target labels and the training rows which is necessary when for gradient boosted trees
    public IRegressionModel CreateRegressionTreeForGradientBoosting(double[] y, double[] curPred, int maxSize, int[] idx, ILossFunction lossFunction, double r = 0.5, double m = 0.5) {
      Debug.Assert(maxSize > 0);
      Debug.Assert(r > 0);
      Debug.Assert(r <= 1.0);
      Debug.Assert(y.Count() == this.y.Length);
      Debug.Assert(m > 0);
      Debug.Assert(m <= 1.0);

      // y and curPred are changed in gradient boosting
      this.y = y; 
      this.curPred = curPred; 

      // shuffle row idx
      HeuristicLab.Random.ListExtensions.ShuffleInPlace(idx, random);

      int nRows = idx.Count();

      // shuffle variable names
      HeuristicLab.Random.ListExtensions.ShuffleInPlace(allowedVariables, random);

      // only select a part of the rows and columns randomly
      effectiveRows = (int)Math.Ceiling(nRows * r);
      effectiveVars = (int)Math.Ceiling(nCols * m);

      // the which array is used for partitioing row idxs  
      Array.Clear(which, 0, which.Length);

      // mark selected rows
      for (int row = 0; row < effectiveRows; row++) {
        which[idx[row]] = 1; // we use the which vector as a temporary variable here
        internalIdx[row] = idx[row];
      }

      for (int col = 0; col < nCols; col++) {
        int i = 0;
        for (int row = 0; row < nRows; row++) {
          if (which[sortedIdxAll[col][row]] > 0) {
            Debug.Assert(i < effectiveRows);
            sortedIdx[col][i] = sortedIdxAll[col][row];
            i++;
          }
        }
      }

      this.tree = new RegressionTreeModel.TreeNode[maxSize];
      this.queue.Clear();
      this.curTreeNodeIdx = 0;

      // start out with only one leaf node (constant prediction)
      // and calculate the best split for this root node and enqueue it into a queue sorted by improvement throught the split
      // start and end idx are inclusive
      CreateLeafNode(0, effectiveRows - 1, lossFunction);

      // process the priority queue to complete the tree
      CreateRegressionTreeFromQueue(maxSize, lossFunction);

      return new RegressionTreeModel(tree.ToArray(), problemData.TargetVariable);
    }


    // processes potential splits from the queue as long as splits are remaining and the maximum size of the tree is not reached
    private void CreateRegressionTreeFromQueue(int maxNodes, ILossFunction lossFunction) {
      while (queue.Any() && curTreeNodeIdx + 1 < maxNodes) { // two nodes are created in each loop
        var f = queue[queue.Count - 1]; // last element has the largest improvement
        queue.RemoveAt(queue.Count - 1);

        var startIdx = f.StartIdx;
        var endIdx = f.EndIndex;

        Debug.Assert(endIdx - startIdx >= 0);
        Debug.Assert(startIdx >= 0);
        Debug.Assert(endIdx < internalIdx.Length);

        // split partition into left and right
        int splitIdx;
        SplitPartition(f.StartIdx, f.EndIndex, f.SplittingVariable, f.SplittingThreshold, out splitIdx);
        Debug.Assert(splitIdx + 1 <= endIdx);
        Debug.Assert(startIdx <= splitIdx);

        // create two leaf nodes (and enqueue best splits for both)
        var leftTreeIdx = CreateLeafNode(startIdx, splitIdx, lossFunction);
        var rightTreeIdx = CreateLeafNode(splitIdx + 1, endIdx, lossFunction);

        // overwrite existing leaf node with an internal node
        tree[f.ParentNodeIdx] = new RegressionTreeModel.TreeNode(f.SplittingVariable, f.SplittingThreshold, leftTreeIdx, rightTreeIdx, weightLeft: (splitIdx - startIdx + 1) / (double)(endIdx - startIdx + 1));
      }
    }


    // returns the index of the newly created tree node
    private int CreateLeafNode(int startIdx, int endIdx, ILossFunction lossFunction) {
      // write a leaf node
      var val = lossFunction.LineSearch(originalY, curPred, internalIdx, startIdx, endIdx);
      tree[curTreeNodeIdx] = new RegressionTreeModel.TreeNode(RegressionTreeModel.TreeNode.NO_VARIABLE, val);

      EnqueuePartitionSplit(curTreeNodeIdx, startIdx, endIdx);
      curTreeNodeIdx++;
      return curTreeNodeIdx - 1;
    }


    // calculates the optimal split for the partition [startIdx .. endIdx] (inclusive) 
    // which is represented by the leaf node with the specified nodeIdx
    private void EnqueuePartitionSplit(int nodeIdx, int startIdx, int endIdx) {
      double threshold, improvement;
      string bestVariableName;
      // only enqueue a new split if there are at least 2 rows left and a split is possible
      if (startIdx < endIdx &&
        FindBestVariableAndThreshold(startIdx, endIdx, out threshold, out bestVariableName, out improvement)) {
        var split = new PartitionSplits() {
          ParentNodeIdx = nodeIdx,
          StartIdx = startIdx,
          EndIndex = endIdx,
          SplittingThreshold = threshold,
          SplittingVariable = bestVariableName
        };
        InsertSortedQueue(split);
      }
    }


    // routine for splitting a partition of rows stored in internalIdx between startIdx and endIdx into
    // a left partition and a right partition using the given splittingVariable and threshold
    // the splitIdx is the last index of the left partition 
    // splitIdx + 1 is the first index of the right partition
    // startIdx and endIdx are inclusive
    private void SplitPartition(int startIdx, int endIdx, string splittingVar, double threshold, out int splitIdx) {
      int bestVarIdx = varName2Index[splittingVar];
      // split - two pass

      // store which index goes into which partition 
      for (int k = startIdx; k <= endIdx; k++) {
        if (x[bestVarIdx][internalIdx[k]] <= threshold)
          which[internalIdx[k]] = -1; // left partition
        else
          which[internalIdx[k]] = 1; // right partition
      }

      // partition sortedIdx for each variable
      int i;
      int j;
      for (int col = 0; col < nCols; col++) {
        i = 0;
        j = 0;
        int k;
        for (k = startIdx; k <= endIdx; k++) {
          Debug.Assert(Math.Abs(which[sortedIdx[col][k]]) == 1);

          if (which[sortedIdx[col][k]] < 0) {
            leftTmp[i++] = sortedIdx[col][k];
          } else {
            rightTmp[j++] = sortedIdx[col][k];
          }
        }
        Debug.Assert(i > 0); // at least on element in the left partition
        Debug.Assert(j > 0); // at least one element in the right partition
        Debug.Assert(i + j == endIdx - startIdx + 1);
        k = startIdx;
        for (int l = 0; l < i; l++) sortedIdx[col][k++] = leftTmp[l];
        for (int l = 0; l < j; l++) sortedIdx[col][k++] = rightTmp[l];
      }

      // partition row indices
      i = startIdx;
      j = endIdx;
      while (i <= j) {
        Debug.Assert(Math.Abs(which[internalIdx[i]]) == 1);
        Debug.Assert(Math.Abs(which[internalIdx[j]]) == 1);
        if (which[internalIdx[i]] < 0) i++;
        else if (which[internalIdx[j]] > 0) j--;
        else {
          Debug.Assert(which[internalIdx[i]] > 0);
          Debug.Assert(which[internalIdx[j]] < 0);
          // swap
          int tmp = internalIdx[i];
          internalIdx[i] = internalIdx[j];
          internalIdx[j] = tmp;
          i++;
          j--;
        }
      }
      Debug.Assert(j + 1 == i);
      Debug.Assert(i <= endIdx);
      Debug.Assert(startIdx <= j);

      splitIdx = j;
    }

    private bool FindBestVariableAndThreshold(int startIdx, int endIdx, out double threshold, out string bestVar, out double improvement) {
      Debug.Assert(startIdx < endIdx + 1); // at least 2 elements

      int rows = endIdx - startIdx + 1;
      Debug.Assert(rows >= 2);

      double sumY = 0.0;
      for (int i = startIdx; i <= endIdx; i++) {
        sumY += y[internalIdx[i]];
      }

      // see description of calculation in FindBestThreshold
      double bestImprovement = 1.0 / rows * sumY * sumY; // any improvement must be larger than this baseline
      double bestThreshold = double.PositiveInfinity;
      bestVar = RegressionTreeModel.TreeNode.NO_VARIABLE;

      for (int col = 0; col < effectiveVars; col++) {
        // sort values for variable to prepare for threshold selection 
        var curVariable = allowedVariables[col];
        var curVariableIdx = varName2Index[curVariable];
        for (int i = startIdx; i <= endIdx; i++) {
          var sortedI = sortedIdx[curVariableIdx][i];
          outSortedIdx[i - startIdx] = sortedI;
          outx[i - startIdx] = x[curVariableIdx][sortedI];
        }

        double curImprovement;
        double curThreshold;
        FindBestThreshold(outx, outSortedIdx, rows, y, sumY, out curThreshold, out curImprovement);

        if (curImprovement > bestImprovement) {
          bestImprovement = curImprovement;
          bestThreshold = curThreshold;
          bestVar = allowedVariables[col];
        }
      }
      if (bestVar == RegressionTreeModel.TreeNode.NO_VARIABLE) {
        // not successfull
        threshold = double.PositiveInfinity;
        improvement = double.NegativeInfinity;
        return false;
      } else {
        UpdateVariableRelevance(bestVar, sumY, bestImprovement, rows);
        improvement = bestImprovement;
        threshold = bestThreshold;
        return true;
      }
    }

    // x [0..N-1] contains rows sorted values in the range from [0..rows-1]
    // sortedIdx [0..N-1] contains the idx of the values in x in the original dataset in the range from [0..rows-1]
    // rows specifies the number of valid entries in x and sortedIdx
    // y [0..N-1] contains the target values in original sorting order
    // sumY is y.Sum()
    // 
    // the routine returns the best threshold (x[i] + x[i+1]) / 2 for i = [0 .. rows-2] by calculating the reduction in squared error
    // additionally the reduction in squared error is returned in bestImprovement
    // if all elements of x are equal the routing fails to produce a threshold
    private static void FindBestThreshold(double[] x, int[] sortedIdx, int rows, double[] y, double sumY, out double bestThreshold, out double bestImprovement) {
      Debug.Assert(rows >= 2);

      double sl = 0.0;
      double sr = sumY;
      double nl = 0.0;
      double nr = rows;

      bestImprovement = 1.0 / rows * sumY * sumY; // this is the baseline for the improvement
      bestThreshold = double.NegativeInfinity;
      // for all thresholds
      // if we have n rows there are n-1 possible splits
      for (int i = 0; i < rows - 1; i++) {
        sl += y[sortedIdx[i]];
        sr -= y[sortedIdx[i]];

        nl++;
        nr--;
        Debug.Assert(nl > 0);
        Debug.Assert(nr > 0);

        if (x[i] < x[i + 1]) { // don't try to split when two elements are equal

          // goal is to find the split with leading to minimal total variance of left and right parts
          // without partitioning the variance is var(y) = E(y²) - E(y)²  
          //    = 1/n * sum(y²) - (1/n * sum(y))²
          //      -------------   ---------------
          //         constant       baseline for improvement
          // 
          // if we split into right and left part the overall variance is the weigthed combination nl/n * var(y_l) + nr/n * var(y_r)  
          //    = nl/n * (1/nl * sum(y_l²) - (1/nl * sum(y_l))²) + nr/n * (1/nr * sum(y_r²) - (1/nr * sum(y_r))²)
          //    = 1/n * sum(y_l²) - 1/nl * 1/n * sum(y_l)² + 1/n * sum(y_r²) - 1/nr * 1/n * sum(y_r)²
          //    = 1/n * (sum(y_l²) + sum(y_r²)) - 1/n * (sum(y_l)² / nl + sum(y_r)² / nr)
          //    = 1/n * sum(y²) - 1/n * (sum(y_l)² / nl + sum(y_r)² / nr)
          //      -------------
          //       not changed by split (and the same for total variance without partitioning)
          //
          //   therefore we need to find the maximum value (sum(y_l)² / nl + sum(y_r)² / nr) (ignoring the factor 1/n)
          //   and this value must be larger than 1/n * sum(y)² to be an improvement over no split 

          double curQuality = sl * sl / nl + sr * sr / nr;

          if (curQuality > bestImprovement) {
            bestThreshold = (x[i] + x[i + 1]) / 2.0;
            bestImprovement = curQuality;
          }
        }
      }

      // if all elements where the same then no split can be found
    }


    private void UpdateVariableRelevance(string bestVar, double sumY, double bestImprovement, int rows) {
      if (string.IsNullOrEmpty(bestVar)) return;
      // update variable relevance
      double baseLine = 1.0 / rows * sumY * sumY; // if best improvement is equal to baseline then the split had no effect

      double delta = (bestImprovement - baseLine);
      double v;
      if (!sumImprovements.TryGetValue(bestVar, out v)) {
        sumImprovements[bestVar] = delta;
      }
      sumImprovements[bestVar] = v + delta;
    }

    public IEnumerable<KeyValuePair<string, double>> GetVariableRelevance() {
      // values are scaled: the most important variable has relevance = 100
      double scaling = 100 / sumImprovements.Max(t => t.Value);
      return
        sumImprovements
        .Select(t => new KeyValuePair<string, double>(t.Key, t.Value * scaling))
        .OrderByDescending(t => t.Value);
    }


    // insert a new parition split (find insertion point and start at first element of the queue)
    // elements are removed from the queue at the last position
    // O(n), splits could be organized as a heap to improve runtime (see alglib tsort)
    private void InsertSortedQueue(PartitionSplits split) {
      // find insertion position
      int i = 0;
      while (i < queue.Count && queue[i].SplittingImprovement < split.SplittingImprovement) { i++; }

      queue.Insert(i, split);
    }
  }
}



















