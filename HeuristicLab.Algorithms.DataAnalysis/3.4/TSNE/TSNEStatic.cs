#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

//Code is based on an implementation from Laurens van der Maaten

/*
*
* Copyright (c) 2014, Laurens van der Maaten (Delft University of Technology)
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
* 1. Redistributions of source code must retain the above copyright
*    notice, this list of conditions and the following disclaimer.
* 2. Redistributions in binary form must reproduce the above copyright
*    notice, this list of conditions and the following disclaimer in the
*    documentation and/or other materials provided with the distribution.
* 3. All advertising materials mentioning features or use of this software
*    must display the following acknowledgement:
*    This product includes software developed by the Delft University of Technology.
* 4. Neither the name of the Delft University of Technology nor the names of
*    its contributors may be used to endorse or promote products derived from
*    this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY LAURENS VAN DER MAATEN ''AS IS'' AND ANY EXPRESS
* OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
* OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
* EVENT SHALL LAURENS VAN DER MAATEN BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
* SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
* PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
* BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
* CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING
* IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY
* OF SUCH DAMAGE.
*
*/
#endregion

using System;
using System.Collections.Generic;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Fossil;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("23E56F69-7AA1-4853-BFEC-7B4DBC346B47")]
  public class TSNEStatic<T> {
    [StorableConstructor]
    protected TSNEStatic(StorableConstructorFlag _) {
    }

    [StorableType("1878DFC2-5DDF-4BBD-85BA-4A6E19AB59C2")]
    public sealed class TSNEState : DeepCloneable {
      #region Storables
      // initialized once
      [Storable]
      public IDistance<T> distance;
      [Storable]
      public IRandom random;
      [Storable]
      public double perplexity;
      [Storable]
      public bool exact;
      [Storable]
      public int noDatapoints;
      [Storable]
      public double finalMomentum;
      [Storable]
      public int momSwitchIter;
      [Storable]
      public int stopLyingIter;
      [Storable]
      public double theta;
      [Storable]
      public double eta;
      [Storable]
      public int newDimensions;

      // for approximate version: sparse representation of similarity/distance matrix
      [Storable]
      public double[] valP; // similarity/distance
      [Storable]
      public int[] rowP; // row index
      [Storable]
      public int[] colP; // col index

      // for exact version: dense representation of distance/similarity matrix
      [Storable]
      public double[,] p;

      // mapped data
      [Storable]
      public double[,] newData;

      [Storable]
      public int iter;
      [Storable]
      public double currentMomentum;

      // helper variables (updated in each iteration)
      [Storable]
      public double[,] gains;
      [Storable]
      public double[,] uY;
      [Storable]
      public double[,] dY;
      #endregion

      #region Constructors & Cloning
      private TSNEState(TSNEState original, Cloner cloner) : base(original, cloner) {
        distance = cloner.Clone(original.distance);
        random = cloner.Clone(original.random);
        perplexity = original.perplexity;
        exact = original.exact;
        noDatapoints = original.noDatapoints;
        finalMomentum = original.finalMomentum;
        momSwitchIter = original.momSwitchIter;
        stopLyingIter = original.stopLyingIter;
        theta = original.theta;
        eta = original.eta;
        newDimensions = original.newDimensions;
        if (original.valP != null) {
          valP = new double[original.valP.Length];
          Array.Copy(original.valP, valP, valP.Length);
        }
        if (original.rowP != null) {
          rowP = new int[original.rowP.Length];
          Array.Copy(original.rowP, rowP, rowP.Length);
        }
        if (original.colP != null) {
          colP = new int[original.colP.Length];
          Array.Copy(original.colP, colP, colP.Length);
        }
        if (original.p != null) {
          p = new double[original.p.GetLength(0), original.p.GetLength(1)];
          Array.Copy(original.p, p, p.Length);
        }
        newData = new double[original.newData.GetLength(0), original.newData.GetLength(1)];
        Array.Copy(original.newData, newData, newData.Length);
        iter = original.iter;
        currentMomentum = original.currentMomentum;
        gains = new double[original.gains.GetLength(0), original.gains.GetLength(1)];
        Array.Copy(original.gains, gains, gains.Length);
        uY = new double[original.uY.GetLength(0), original.uY.GetLength(1)];
        Array.Copy(original.uY, uY, uY.Length);
        dY = new double[original.dY.GetLength(0), original.dY.GetLength(1)];
        Array.Copy(original.dY, dY, dY.Length);
      }

      public override IDeepCloneable Clone(Cloner cloner) {
        return new TSNEState(this, cloner);
      }

      [StorableConstructor]
      private TSNEState(StorableConstructorFlag _) { }

      public TSNEState(IReadOnlyList<T> data, IDistance<T> distance, IRandom random, int newDimensions, double perplexity,
        double theta, int stopLyingIter, int momSwitchIter, double momentum, double finalMomentum, double eta, bool randomInit) {
        this.distance = distance;
        this.random = random;
        this.newDimensions = newDimensions;
        this.perplexity = perplexity;
        this.theta = theta;
        this.stopLyingIter = stopLyingIter;
        this.momSwitchIter = momSwitchIter;
        currentMomentum = momentum;
        this.finalMomentum = finalMomentum;
        this.eta = eta;

        // initialize
        noDatapoints = data.Count;
        if (noDatapoints - 1 < 3 * perplexity)
          throw new ArgumentException("Perplexity too large for the number of data points!");

        exact = Math.Abs(theta) < double.Epsilon;
        newData = new double[noDatapoints, newDimensions];
        dY = new double[noDatapoints, newDimensions];
        uY = new double[noDatapoints, newDimensions];
        gains = new double[noDatapoints, newDimensions];
        for (var i = 0; i < noDatapoints; i++)
        for (var j = 0; j < newDimensions; j++)
          gains[i, j] = 1.0;

        p = null;
        rowP = null;
        colP = null;
        valP = null;

        //Calculate Similarities
        if (exact) p = CalculateExactSimilarites(data, distance, perplexity);
        else CalculateApproximateSimilarities(data, distance, perplexity, out rowP, out colP, out valP);

        // Lie about the P-values (factor is 4 in the MATLAB implementation)
        if (exact) for (var i = 0; i < noDatapoints; i++) for (var j = 0; j < noDatapoints; j++) p[i, j] *= 12.0;
        else for (var i = 0; i < rowP[noDatapoints]; i++) valP[i] *= 12.0;

        // Initialize solution (randomly)
        var rand = new NormalDistributedRandom(random, 0, 1);
        for (var i = 0; i < noDatapoints; i++)
        for (var j = 0; j < newDimensions; j++)
          newData[i, j] = rand.NextDouble() * .0001;

        if (!(data[0] is IReadOnlyList<double>) || randomInit) return;
        for (var i = 0; i < noDatapoints; i++)
        for (var j = 0; j < newDimensions; j++) {
          var row = (IReadOnlyList<double>) data[i];
          newData[i, j] = row[j % row.Count];
        }
      }
      #endregion

      public double EvaluateError() {
        return exact ? EvaluateErrorExact(p, newData, noDatapoints, newDimensions) : EvaluateErrorApproximate(rowP, colP, valP, newData, theta);
      }

      #region Helpers
      private static void CalculateApproximateSimilarities(IReadOnlyList<T> data, IDistance<T> distance, double perplexity, out int[] rowP, out int[] colP, out double[] valP) {
        // Compute asymmetric pairwise input similarities
        ComputeGaussianPerplexity(data, distance, out rowP, out colP, out valP, perplexity, (int) (3 * perplexity));
        // Symmetrize input similarities
        int[] sRowP, symColP;
        double[] sValP;
        SymmetrizeMatrix(rowP, colP, valP, out sRowP, out symColP, out sValP);
        rowP = sRowP;
        colP = symColP;
        valP = sValP;
        var sumP = .0;
        for (var i = 0; i < rowP[data.Count]; i++) sumP += valP[i];
        for (var i = 0; i < rowP[data.Count]; i++) valP[i] /= sumP;
      }
      private static double[,] CalculateExactSimilarites(IReadOnlyList<T> data, IDistance<T> distance, double perplexity) {
        // Compute similarities
        var p = new double[data.Count, data.Count];
        ComputeGaussianPerplexity(data, distance, p, perplexity);
        // Symmetrize input similarities
        for (var n = 0; n < data.Count; n++) {
          for (var m = n + 1; m < data.Count; m++) {
            p[n, m] += p[m, n];
            p[m, n] = p[n, m];
          }
        }
        var sumP = .0;
        for (var i = 0; i < data.Count; i++) {
          for (var j = 0; j < data.Count; j++) {
            sumP += p[i, j];
          }
        }
        for (var i = 0; i < data.Count; i++) {
          for (var j = 0; j < data.Count; j++) {
            p[i, j] /= sumP;
          }
        }
        return p;
      }
      private static void ComputeGaussianPerplexity(IReadOnlyList<T> x, IDistance<T> distance, out int[] rowP, out int[] colP, out double[] valP, double perplexity, int k) {
        if (perplexity > k) throw new ArgumentException("Perplexity should be lower than k!");

        var n = x.Count;
        // Allocate the memory we need
        rowP = new int[n + 1];
        colP = new int[n * k];
        valP = new double[n * k];
        var curP = new double[n - 1];
        rowP[0] = 0;
        for (var i = 0; i < n; i++) rowP[i + 1] = rowP[i] + k;

        var objX = new List<IndexedItem<T>>();
        for (var i = 0; i < n; i++) objX.Add(new IndexedItem<T>(i, x[i]));

        // Build ball tree on data set
        var tree = new VantagePointTree<IndexedItem<T>>(new IndexedItemDistance<T>(distance), objX);

        // Loop over all points to find nearest neighbors
        for (var i = 0; i < n; i++) {
          IList<IndexedItem<T>> indices;
          IList<double> distances;

          // Find nearest neighbors
          tree.Search(objX[i], k + 1, out indices, out distances);

          // Initialize some variables for binary search
          var found = false;
          var beta = 1.0;
          var minBeta = double.MinValue;
          var maxBeta = double.MaxValue;
          const double tol = 1e-5;

          // Iterate until we found a good perplexity
          var iter = 0;
          double sumP = 0;
          while (!found && iter < 200) {
            // Compute Gaussian kernel row
            for (var m = 0; m < k; m++) curP[m] = Math.Exp(-beta * distances[m + 1]);

            // Compute entropy of current row
            sumP = double.Epsilon;
            for (var m = 0; m < k; m++) sumP += curP[m];
            var h = .0;
            for (var m = 0; m < k; m++) h += beta * (distances[m + 1] * curP[m]);
            h = h / sumP + Math.Log(sumP);

            // Evaluate whether the entropy is within the tolerance level
            var hdiff = h - Math.Log(perplexity);
            if (hdiff < tol && -hdiff < tol) {
              found = true;
            }
            else {
              if (hdiff > 0) {
                minBeta = beta;
                if (maxBeta.IsAlmost(double.MaxValue) || maxBeta.IsAlmost(double.MinValue))
                  beta *= 2.0;
                else
                  beta = (beta + maxBeta) / 2.0;
              }
              else {
                maxBeta = beta;
                if (minBeta.IsAlmost(double.MinValue) || minBeta.IsAlmost(double.MaxValue))
                  beta /= 2.0;
                else
                  beta = (beta + minBeta) / 2.0;
              }
            }

            // Update iteration counter
            iter++;
          }

          // Row-normalize current row of P and store in matrix
          for (var m = 0; m < k; m++) curP[m] /= sumP;
          for (var m = 0; m < k; m++) {
            colP[rowP[i] + m] = indices[m + 1].Index;
            valP[rowP[i] + m] = curP[m];
          }
        }
      }
      private static void ComputeGaussianPerplexity(IReadOnlyList<T> x, IDistance<T> distance, double[,] p, double perplexity) {
        // Compute the distance matrix
        var dd = ComputeDistances(x, distance);

        var n = x.Count;
        // Compute the Gaussian kernel row by row
        for (var i = 0; i < n; i++) {
          // Initialize some variables
          var found = false;
          var beta = 1.0;
          var minBeta = double.MinValue;
          var maxBeta = double.MaxValue;
          const double tol = 1e-5;
          double sumP = 0;

          // Iterate until we found a good perplexity
          var iter = 0;
          while (!found && iter < 200) { // 200 iterations as in tSNE implementation by van der Maarten

            // Compute Gaussian kernel row
            for (var m = 0; m < n; m++) p[i, m] = Math.Exp(-beta * dd[i][m]);
            p[i, i] = double.Epsilon;

            // Compute entropy of current row
            sumP = double.Epsilon;
            for (var m = 0; m < n; m++) sumP += p[i, m];
            var h = 0.0;
            for (var m = 0; m < n; m++) h += beta * (dd[i][m] * p[i, m]);
            h = h / sumP + Math.Log(sumP);

            // Evaluate whether the entropy is within the tolerance level
            var hdiff = h - Math.Log(perplexity);
            if (hdiff < tol && -hdiff < tol) {
              found = true;
            }
            else {
              if (hdiff > 0) {
                minBeta = beta;
                if (maxBeta.IsAlmost(double.MaxValue) || maxBeta.IsAlmost(double.MinValue))
                  beta *= 2.0;
                else
                  beta = (beta + maxBeta) / 2.0;
              }
              else {
                maxBeta = beta;
                if (minBeta.IsAlmost(double.MinValue) || minBeta.IsAlmost(double.MaxValue))
                  beta /= 2.0;
                else
                  beta = (beta + minBeta) / 2.0;
              }
            }

            // Update iteration counter
            iter++;
          }

          // Row normalize P
          for (var m = 0; m < n; m++) p[i, m] /= sumP;
        }
      }
      private static double[][] ComputeDistances(IReadOnlyList<T> x, IDistance<T> distance) {
        var res = new double[x.Count][];
        for (var r = 0; r < x.Count; r++) {
          var rowV = new double[x.Count];
          // all distances must be symmetric 
          for (var c = 0; c < r; c++) {
            rowV[c] = res[c][r];
          }
          rowV[r] = 0.0; // distance to self is zero for all distances
          for (var c = r + 1; c < x.Count; c++) {
            rowV[c] = distance.Get(x[r], x[c]);
          }
          res[r] = rowV;
        }
        return res;
        // return x.Select(m => x.Select(n => distance.Get(m, n)).ToArray()).ToArray();
      }
      private static double EvaluateErrorExact(double[,] p, double[,] y, int n, int d) {
        // Compute the squared Euclidean distance matrix
        var dd = new double[n, n];
        var q = new double[n, n];
        ComputeSquaredEuclideanDistance(y, n, d, dd);

        // Compute Q-matrix and normalization sum
        var sumQ = double.Epsilon;
        for (var n1 = 0; n1 < n; n1++) {
          for (var m = 0; m < n; m++) {
            if (n1 != m) {
              q[n1, m] = 1 / (1 + dd[n1, m]);
              sumQ += q[n1, m];
            }
            else q[n1, m] = double.Epsilon;
          }
        }
        for (var i = 0; i < n; i++) for (var j = 0; j < n; j++) q[i, j] /= sumQ;

        // Sum t-SNE error
        var c = .0;
        for (var i = 0; i < n; i++)
        for (var j = 0; j < n; j++) {
          c += p[i, j] * Math.Log((p[i, j] + float.Epsilon) / (q[i, j] + float.Epsilon));
        }
        return c;
      }
      private static double EvaluateErrorApproximate(IReadOnlyList<int> rowP, IReadOnlyList<int> colP, IReadOnlyList<double> valP, double[,] y, double theta) {
        // Get estimate of normalization term
        var n = y.GetLength(0);
        var d = y.GetLength(1);
        var tree = new SpacePartitioningTree(y);
        var buff = new double[d];
        var sumQ = 0.0;
        for (var i = 0; i < n; i++) tree.ComputeNonEdgeForces(i, theta, buff, ref sumQ);

        // Loop over all edges to compute t-SNE error
        var c = .0;
        for (var k = 0; k < n; k++) {
          for (var i = rowP[k]; i < rowP[k + 1]; i++) {
            var q = .0;
            for (var j = 0; j < d; j++) buff[j] = y[k, j];
            for (var j = 0; j < d; j++) buff[j] -= y[colP[i], j];
            for (var j = 0; j < d; j++) q += buff[j] * buff[j];
            q = (1.0 / (1.0 + q)) / sumQ;
            c += valP[i] * Math.Log((valP[i] + float.Epsilon) / (q + float.Epsilon));
          }
        }
        return c;
      }
      private static void SymmetrizeMatrix(IReadOnlyList<int> rowP, IReadOnlyList<int> colP, IReadOnlyList<double> valP, out int[] symRowP, out int[] symColP, out double[] symValP) {
        // Count number of elements and row counts of symmetric matrix
        var n = rowP.Count - 1;
        var rowCounts = new int[n];
        for (var j = 0; j < n; j++) {
          for (var i = rowP[j]; i < rowP[j + 1]; i++) {
            // Check whether element (col_P[i], n) is present
            var present = false;
            for (var m = rowP[colP[i]]; m < rowP[colP[i] + 1]; m++) {
              if (colP[m] == j) present = true;
            }
            if (present) rowCounts[j]++;
            else {
              rowCounts[j]++;
              rowCounts[colP[i]]++;
            }
          }
        }
        var noElem = 0;
        for (var i = 0; i < n; i++) noElem += rowCounts[i];

        // Allocate memory for symmetrized matrix
        symRowP = new int[n + 1];
        symColP = new int[noElem];
        symValP = new double[noElem];

        // Construct new row indices for symmetric matrix
        symRowP[0] = 0;
        for (var i = 0; i < n; i++) symRowP[i + 1] = symRowP[i] + rowCounts[i];

        // Fill the result matrix
        var offset = new int[n];
        for (var j = 0; j < n; j++) {
          for (var i = rowP[j]; i < rowP[j + 1]; i++) { // considering element(n, colP[i])

            // Check whether element (col_P[i], n) is present
            var present = false;
            for (var m = rowP[colP[i]]; m < rowP[colP[i] + 1]; m++) {
              if (colP[m] != j) continue;
              present = true;
              if (j > colP[i]) continue; // make sure we do not add elements twice
              symColP[symRowP[j] + offset[j]] = colP[i];
              symColP[symRowP[colP[i]] + offset[colP[i]]] = j;
              symValP[symRowP[j] + offset[j]] = valP[i] + valP[m];
              symValP[symRowP[colP[i]] + offset[colP[i]]] = valP[i] + valP[m];
            }

            // If (colP[i], n) is not present, there is no addition involved
            if (!present) {
              symColP[symRowP[j] + offset[j]] = colP[i];
              symColP[symRowP[colP[i]] + offset[colP[i]]] = j;
              symValP[symRowP[j] + offset[j]] = valP[i];
              symValP[symRowP[colP[i]] + offset[colP[i]]] = valP[i];
            }

            // Update offsets
            if (present && (j > colP[i])) continue;
            offset[j]++;
            if (colP[i] != j) offset[colP[i]]++;
          }
        }

        for (var i = 0; i < noElem; i++) symValP[i] /= 2.0;
      }
      #endregion
    }

    /// <summary>
    /// Static interface to tSNE
    /// </summary>
    /// <param name="data"></param>
    /// <param name="distance">The distance function used to differentiate similar from non-similar points, e.g. Euclidean distance.</param>
    /// <param name="random">Random number generator</param>
    /// <param name="newDimensions">Dimensionality of projected space (usually 2 for easy visual analysis).</param>
    /// <param name="perplexity">Perplexity parameter of tSNE. Comparable to k in a k-nearest neighbour algorithm. Recommended value is floor(number of points /3) or lower</param>
    /// <param name="iterations">Maximum number of iterations for gradient descent.</param>
    /// <param name="theta">Value describing how much appoximated gradients my differ from exact gradients. Set to 0 for exact calculation and in [0,1] otherwise. CAUTION: exact calculation of forces requires building a non-sparse N*N matrix where N is the number of data points. This may exceed memory limitations.</param>
    /// <param name="stopLyingIter">Number of iterations after which p is no longer approximated.</param>
    /// <param name="momSwitchIter">Number of iterations after which the momentum in the gradient descent is switched.</param>
    /// <param name="momentum">The initial momentum in the gradient descent.</param>
    /// <param name="finalMomentum">The final momentum in gradient descent (after momentum switch).</param>
    /// <param name="eta">Gradient descent learning rate.</param>
    /// <returns></returns>
    public static double[,] Run(T[] data, IDistance<T> distance, IRandom random,
      int newDimensions = 2, double perplexity = 25, int iterations = 1000,
      double theta = 0, int stopLyingIter = 0, int momSwitchIter = 0, double momentum = .5,
      double finalMomentum = .8, double eta = 10.0
    ) {
      var state = CreateState(data, distance, random, newDimensions, perplexity,
        theta, stopLyingIter, momSwitchIter, momentum, finalMomentum, eta);

      for (var i = 0; i < iterations - 1; i++) {
        Iterate(state);
      }
      return Iterate(state);
    }

    public static TSNEState CreateState(T[] data, IDistance<T> distance, IRandom random,
      int newDimensions = 2, double perplexity = 25, double theta = 0,
      int stopLyingIter = 0, int momSwitchIter = 0, double momentum = .5,
      double finalMomentum = .8, double eta = 10.0, bool randomInit = true
    ) {
      return new TSNEState(data, distance, random, newDimensions, perplexity, theta, stopLyingIter, momSwitchIter, momentum, finalMomentum, eta, randomInit);
    }

    public static double[,] Iterate(TSNEState state) {
      if (state.exact)
        ComputeExactGradient(state.p, state.newData, state.noDatapoints, state.newDimensions, state.dY);
      else
        ComputeApproximateGradient(state.rowP, state.colP, state.valP, state.newData, state.noDatapoints, state.newDimensions, state.dY, state.theta);

      // Update gains
      for (var i = 0; i < state.noDatapoints; i++) {
        for (var j = 0; j < state.newDimensions; j++) {
          state.gains[i, j] = Math.Sign(state.dY[i, j]) != Math.Sign(state.uY[i, j])
            ? state.gains[i, j] + .2 // +0.2 nd *0.8 are used in two separate implementations of tSNE -> seems to be correct
            : state.gains[i, j] * .8;
          if (state.gains[i, j] < .01) state.gains[i, j] = .01;
        }
      }

      // Perform gradient update (with momentum and gains)
      for (var i = 0; i < state.noDatapoints; i++)
      for (var j = 0; j < state.newDimensions; j++)
        state.uY[i, j] = state.currentMomentum * state.uY[i, j] - state.eta * state.gains[i, j] * state.dY[i, j];

      for (var i = 0; i < state.noDatapoints; i++)
      for (var j = 0; j < state.newDimensions; j++)
        state.newData[i, j] = state.newData[i, j] + state.uY[i, j];

      // Make solution zero-mean
      ZeroMean(state.newData);

      // Stop lying about the P-values after a while, and switch momentum
      if (state.iter == state.stopLyingIter) {
        if (state.exact)
          for (var i = 0; i < state.noDatapoints; i++)
          for (var j = 0; j < state.noDatapoints; j++)
            state.p[i, j] /= 12.0;
        else
          for (var i = 0; i < state.rowP[state.noDatapoints]; i++)
            state.valP[i] /= 12.0;
      }

      if (state.iter == state.momSwitchIter)
        state.currentMomentum = state.finalMomentum;

      state.iter++;
      return state.newData;
    }

    #region Helpers
    private static void ComputeApproximateGradient(int[] rowP, int[] colP, double[] valP, double[,] y, int n, int d, double[,] dC, double theta) {
      var tree = new SpacePartitioningTree(y);
      var sumQ = 0.0;
      var posF = new double[n, d];
      var negF = new double[n, d];
      SpacePartitioningTree.ComputeEdgeForces(rowP, colP, valP, n, posF, y, d);
      var row = new double[d];
      for (var n1 = 0; n1 < n; n1++) {
        Array.Clear(row, 0, row.Length);
        tree.ComputeNonEdgeForces(n1, theta, row, ref sumQ);
        Buffer.BlockCopy(row, 0, negF, (sizeof(double) * n1 * d), d * sizeof(double));
      }

      // Compute final t-SNE gradient
      for (var i = 0; i < n; i++)
      for (var j = 0; j < d; j++) {
        dC[i, j] = posF[i, j] - negF[i, j] / sumQ;
      }
    }

    private static void ComputeExactGradient(double[,] p, double[,] y, int n, int d, double[,] dC) {
      // Make sure the current gradient contains zeros
      for (var i = 0; i < n; i++) for (var j = 0; j < d; j++) dC[i, j] = 0.0;

      // Compute the squared Euclidean distance matrix
      var dd = new double[n, n];
      ComputeSquaredEuclideanDistance(y, n, d, dd);

      // Compute Q-matrix and normalization sum
      var q = new double[n, n];
      var sumQ = .0;
      for (var n1 = 0; n1 < n; n1++) {
        for (var m = 0; m < n; m++) {
          if (n1 == m) continue;
          q[n1, m] = 1 / (1 + dd[n1, m]);
          sumQ += q[n1, m];
        }
      }

      // Perform the computation of the gradient
      for (var n1 = 0; n1 < n; n1++) {
        for (var m = 0; m < n; m++) {
          if (n1 == m) continue;
          var mult = (p[n1, m] - q[n1, m] / sumQ) * q[n1, m];
          for (var d1 = 0; d1 < d; d1++) {
            dC[n1, d1] += (y[n1, d1] - y[m, d1]) * mult;
          }
        }
      }
    }

    private static void ComputeSquaredEuclideanDistance(double[,] x, int n, int d, double[,] dd) {
      var dataSums = new double[n];
      for (var i = 0; i < n; i++) {
        for (var j = 0; j < d; j++) {
          dataSums[i] += x[i, j] * x[i, j];
        }
      }
      for (var i = 0; i < n; i++) {
        for (var m = 0; m < n; m++) {
          dd[i, m] = dataSums[i] + dataSums[m];
        }
      }
      for (var i = 0; i < n; i++) {
        dd[i, i] = 0.0;
        for (var m = i + 1; m < n; m++) {
          dd[i, m] = 0.0;
          for (var j = 0; j < d; j++) {
            dd[i, m] += (x[i, j] - x[m, j]) * (x[i, j] - x[m, j]);
          }
          dd[m, i] = dd[i, m];
        }
      }
    }

    private static void ZeroMean(double[,] x) {
      // Compute data mean
      var n = x.GetLength(0);
      var d = x.GetLength(1);
      var mean = new double[d];
      for (var i = 0; i < n; i++) {
        for (var j = 0; j < d; j++) {
          mean[j] += x[i, j];
        }
      }
      for (var i = 0; i < d; i++) {
        mean[i] /= n;
      }
      // Subtract data mean
      for (var i = 0; i < n; i++) {
        for (var j = 0; j < d; j++) {
          x[i, j] -= mean[j];
        }
      }
    }
    #endregion
  }
}