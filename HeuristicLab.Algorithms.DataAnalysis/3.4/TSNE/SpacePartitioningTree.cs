#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Space partitioning tree (SPTree)
  /// </summary>
  internal class SpacePartitioningTree {
    private const uint QtNodeCapacity = 1;

    #region Fields
    private int dimension;
    private bool isLeaf;
    private uint size;
    private uint cumulativeSize;

    // Axis-aligned bounding box stored as a center with half-dimensions to represent the boundaries of this quad tree
    private Cell boundary;

    private double[,] data;

    // Indices in this space-partitioning tree node, corresponding center-of-mass, and list of all children
    private double[] centerOfMass;
    private readonly int[] index = new int[QtNodeCapacity];

    // Children
    private SpacePartitioningTree[] children;
    private uint noChildren;
    #endregion

    public SpacePartitioningTree(double[,] inpData) {
      var d = inpData.GetLength(1);
      var n = inpData.GetLength(0);
      var meanY = new double[d];
      var minY = new double[d];
      for (var i = 0; i < d; i++) minY[i] = double.MaxValue;
      var maxY = new double[d];
      for (var i = 0; i < d; i++) maxY[i] = double.MinValue;
      for (uint i = 0; i < n; i++) {
        for (uint j = 0; j < d; j++) {
          meanY[j] += inpData[i, j];
          if (inpData[i, j] < minY[j]) minY[j] = inpData[i, j];
          if (inpData[i, j] > maxY[j]) maxY[j] = inpData[i, j];
        }
      }
      for (var i = 0; i < d; i++) meanY[i] /= n;
      var width = new double[d];
      for (var i = 0; i < d; i++) width[i] = Math.Max(maxY[i] - meanY[i], meanY[i] - minY[i]) + 1e-5;
      Init(inpData, meanY, width);
      Fill(n);
    }

    private SpacePartitioningTree(double[,] inpData, IEnumerable<double> impCorner, IEnumerable<double> impWith) {
      Init(inpData, impCorner, impWith);
    }

    public bool Insert(int newIndex) {
      // Ignore objects which do not belong in this quad tree
      var point = new double[dimension];
      Buffer.BlockCopy(data, sizeof(double) * dimension * newIndex, point, 0, sizeof(double) * dimension);
      if (!boundary.ContainsPoint(point)) return false;
      cumulativeSize++;
      // Online update of cumulative size and center-of-mass
      var mult1 = (double)(cumulativeSize - 1) / cumulativeSize;
      var mult2 = 1.0 / cumulativeSize;
      for (var i = 0; i < dimension; i++) centerOfMass[i] *= mult1;
      for (var i = 0; i < dimension; i++) centerOfMass[i] += mult2 * point[i];

      // If there is space in this quad tree and it is a leaf, add the object here
      if (isLeaf && size < QtNodeCapacity) {
        index[size] = newIndex;
        size++;
        return true;
      }

      // Don't add duplicates
      var anyDuplicate = false;
      for (uint n = 0; n < size; n++) {
        var duplicate = true;
        for (var d = 0; d < dimension; d++) {
          if (Math.Abs(point[d] - data[index[n], d]) < double.Epsilon) continue;
          duplicate = false; break;
        }
        anyDuplicate = anyDuplicate | duplicate;
      }
      if (anyDuplicate) return true;

      // Otherwise, we need to subdivide the current cell
      if (isLeaf) Subdivide();
      // Find out where the point can be inserted
      for (var i = 0; i < noChildren; i++) {
        if (children[i].Insert(newIndex)) return true;
      }

      // Otherwise, the point cannot be inserted (this should never happen)
      return false;
    }

    public void ComputeNonEdgeForces(int pointIndex, double theta, double[] negF, ref double sumQ) {
      // Make sure that we spend no time on empty nodes or self-interactions
      if (cumulativeSize == 0 || (isLeaf && size == 1 && index[0] == pointIndex)) return;

      // Compute distance between point and center-of-mass
      var D = .0;
      var buff = new double[dimension];
      for (var d = 0; d < dimension; d++) buff[d] = data[pointIndex, d] - centerOfMass[d];
      for (var d = 0; d < dimension; d++) D += buff[d] * buff[d];

      // Check whether we can use this node as a "summary"
      var maxWidth = 0.0;
      for (var d = 0; d < dimension; d++) {
        var curWidth = boundary.GetWidth(d);
        maxWidth = maxWidth > curWidth ? maxWidth : curWidth;
      }
      if (isLeaf || maxWidth / Math.Sqrt(D) < theta) {

        // Compute and add t-SNE force between point and current node
        D = 1.0 / (1.0 + D);
        var mult = cumulativeSize * D;
        sumQ += mult;
        mult *= D;
        for (var d = 0; d < dimension; d++) negF[d] += mult * buff[d];
      } else {

        // Recursively apply Barnes-Hut to children
        for (var i = 0; i < noChildren; i++) children[i].ComputeNonEdgeForces(pointIndex, theta, negF, ref sumQ);
      }
    }

    public static void ComputeEdgeForces(int[] rowP, int[] colP, double[] valP, int n, double[,] posF, double[,] data, int dimension) {
      // Loop over all edges in the graph
      for (var k = 0; k < n; k++) {
        for (var i = rowP[k]; i < rowP[k + 1]; i++) {

          // Compute pairwise distance and Q-value
          // uses squared distance
          var d = 1.0;
          var buff = new double[dimension];
          for (var j = 0; j < dimension; j++) buff[j] = data[k, j] - data[colP[i], j];
          for (var j = 0; j < dimension; j++) d += buff[j] * buff[j];
          d = valP[i] / d;

          // Sum positive force
          for (var j = 0; j < dimension; j++) posF[k, j] += d * buff[j];
        }
      }
    }

    #region Helpers
    private void Fill(int n) {
      for (var i = 0; i < n; i++) Insert(i);
    }

    private void Init(double[,] inpData, IEnumerable<double> inpCorner, IEnumerable<double> inpWidth) {
      dimension = inpData.GetLength(1);
      noChildren = 2;
      for (uint i = 1; i < dimension; i++) noChildren *= 2;
      data = inpData;
      isLeaf = true;
      size = 0;
      cumulativeSize = 0;
      boundary = new Cell((uint)dimension);

      inpCorner.ForEach((i, x) => boundary.SetCorner(i, x));
      inpWidth.ForEach((i, x) => boundary.SetWidth(i, x));

      children = new SpacePartitioningTree[noChildren];
      centerOfMass = new double[dimension];
    }

    private void Subdivide() {
      // Create new children
      var newCorner = new double[dimension];
      var newWidth = new double[dimension];
      for (var i = 0; i < noChildren; i++) {
        var div = 1;
        for (var d = 0; d < dimension; d++) {
          newWidth[d] = .5 * boundary.GetWidth(d);
          if (i / div % 2 == 1) newCorner[d] = boundary.GetCorner(d) - .5 * boundary.GetWidth(d);
          else newCorner[d] = boundary.GetCorner(d) + .5 * boundary.GetWidth(d);
          div *= 2;
        }
        children[i] = new SpacePartitioningTree(data, newCorner, newWidth);
      }

      // Move existing points to correct children
      for (var i = 0; i < size; i++) {
        var success = false;
        for (var j = 0; j < noChildren; j++) {
          if (!success) success = children[j].Insert(index[i]);
        }
        index[i] = -1; // as in tSNE implementation by van der Maaten
      }
      // Empty parent node
      size = 0;
      isLeaf = false;
    }
    #endregion

    private class Cell {
      private readonly uint dimension;
      private readonly double[] corner;
      private readonly double[] width;

      public Cell(uint inpDimension) {
        dimension = inpDimension;
        corner = new double[dimension];
        width = new double[dimension];
      }

      public double GetCorner(int d) {
        return corner[d];
      }
      public double GetWidth(int d) {
        return width[d];
      }
      public void SetCorner(int d, double val) {
        corner[d] = val;
      }
      public void SetWidth(int d, double val) {
        width[d] = val;
      }
      public bool ContainsPoint(double[] point) {
        for (var d = 0; d < dimension; d++)
          if (corner[d] - width[d] > point[d] || corner[d] + width[d] < point[d]) return false;
        return true;
      }
    }
  }
}
