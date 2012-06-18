#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.LinearAssignment {
  [Item("LinearAssignmentProblemSolver", "Uses the hungarian algorithm to solve linear assignment problems.")]
  [StorableClass]
  public sealed class LinearAssignmentProblemSolver : SingleSuccessorOperator {
    private const int UNASSIGNED = -1;

    public ILookupParameter<DoubleMatrix> CostsParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Costs"]; }
    }
    public ILookupParameter<Permutation> AssignmentParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Assignment"]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    [StorableConstructor]
    private LinearAssignmentProblemSolver(bool deserializing) : base(deserializing) { }
    private LinearAssignmentProblemSolver(LinearAssignmentProblemSolver original, Cloner cloner) : base(original, cloner) { }
    public LinearAssignmentProblemSolver()
      : base() {
      Parameters.Add(new LookupParameter<DoubleMatrix>("Costs", LinearAssignmentProblem.CostsDescription));
      Parameters.Add(new LookupParameter<Permutation>("Assignment", "The assignment solution to create."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality value of the solution."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LinearAssignmentProblemSolver(this, cloner);
    }

    public override IOperation Apply() {
      var costs = CostsParameter.ActualValue;
      double quality;
      var solution = Solve(costs, out quality);

      AssignmentParameter.ActualValue = new Permutation(PermutationTypes.Absolute, solution);
      QualityParameter.ActualValue = new DoubleValue(quality);

      return base.Apply();
    }

    /// <summary>
    /// Uses the Hungarian algorithm to solve the linear assignment problem (LAP).
    /// The LAP is defined as minimize f(p) = Sum(i = 1..N, c_{i, p(i)}) for a permutation p and an NxN cost matrix.
    /// 
    /// The runtime complexity of the algorithm is O(n^3). The algorithm is deterministic and terminates
    /// returning one of the optimal solutions and the corresponding quality.
    /// </summary>
    /// <remarks>
    /// The algorithm is written similar to the fortran implementation given in http://www.seas.upenn.edu/qaplib/code.d/qapglb.f
    /// </remarks>
    /// <param name="costs">An NxN costs matrix.</param>
    /// <param name="quality">The quality value of the optimal solution.</param>
    /// <returns>The optimal solution.</returns>
    public static int[] Solve(DoubleMatrix costs, out double quality) {
      int length = costs.Rows;
      // solve the linear assignment problem f(p) = Sum(i = 1..|p|, c_{i, p(i)})

      int[] rowAssign = new int[length], colAssign = new int[length];
      double[] dualCol = new double[length], dualRow = new double[length];
      for (int i = 0; i < length; i++) { // mark all positions as untouched
        rowAssign[i] = UNASSIGNED;
        colAssign[i] = UNASSIGNED;
      }

      for (int i = 0; i < length; i++) { // find the minimum (base) level for each row
        double min = costs[i, 0];
        int minCol = 0;
        dualCol[0] = min;
        for (int j = 1; j < length; j++) {
          if (costs[i, j] <= min) {
            min = costs[i, j];
            minCol = j;
          }
          if (costs[i, j] > dualCol[j])
            dualCol[j] = costs[i, j];
        }
        dualRow[i] = min; // this will be the value of our dual variable
        if (colAssign[minCol] == UNASSIGNED) {
          colAssign[minCol] = i;
          rowAssign[i] = minCol;
        }
      }

      for (int j = 0; j < length; j++) { // calculate the second dual variable
        if (colAssign[j] != UNASSIGNED) dualCol[j] = 0;
        else {
          int minRow = 0;
          for (int i = 0; i < length; i++) {
            if (dualCol[j] > 0 && costs[i, j] - dualRow[i] < dualCol[j]) {
              dualCol[j] = costs[i, j] - dualRow[i]; // the value is the original costs minus the first dual value
              minRow = i;
            }
          }
          if (rowAssign[minRow] == UNASSIGNED) {
            colAssign[j] = minRow;
            rowAssign[minRow] = j;
          }
        }
      }

      // at this point costs_ij - dualRow_i - dualColumn_j results in a matrix that has at least one zero in every row and every column

      for (int i = 0; i < length; i++) { // try to make the remaining assignments
        if (rowAssign[i] == UNASSIGNED) {
          double min = dualRow[i];
          for (int j = 0; j < length; j++) {
            if (colAssign[j] == UNASSIGNED && (costs[i, j] - min - dualCol[j]).IsAlmost(0.0)) {
              rowAssign[i] = j;
              colAssign[j] = i;
              break;
            }
          }
        }
      }

      bool[] marker = new bool[length];
      double[] dplus = new double[length], dminus = new double[length];
      int[] rowMarks = new int[length];

      for (int u = 0; u < length; u++) {
        if (rowAssign[u] == UNASSIGNED) {
          for (int i = 0; i < length; i++) {
            rowMarks[i] = u;
            marker[i] = false;
            dplus[i] = double.MaxValue;
            dminus[i] = costs[u, i] - dualRow[u] - dualCol[i];
          }

          dplus[u] = 0;
          int index = -1;
          double minD = double.MaxValue;
          while (true) {
            minD = double.MaxValue;
            for (int i = 0; i < length; i++) {
              if (!marker[i] && dminus[i] < minD) {
                minD = dminus[i];
                index = i;
              }
            }

            if (colAssign[index] == UNASSIGNED) break;
            marker[index] = true;
            dplus[colAssign[index]] = minD;
            for (int i = 0; i < length; i++) {
              if (marker[i]) continue;
              double compare = minD + costs[colAssign[index], i] - dualCol[i] - dualRow[colAssign[index]];
              if (dminus[i] > compare) {
                dminus[i] = compare;
                rowMarks[i] = colAssign[index];
              }
            }

          } // while(true)

          while (true) {
            colAssign[index] = rowMarks[index];
            var ind = rowAssign[rowMarks[index]];
            rowAssign[rowMarks[index]] = index;
            if (rowMarks[index] == u) break;

            index = ind;
          }

          for (int i = 0; i < length; i++) {
            if (dplus[i] < double.MaxValue)
              dualRow[i] += minD - dplus[i];
            if (dminus[i] < minD)
              dualCol[i] += dminus[i] - minD;
          }
        }
      }

      quality = 0;
      for (int i = 0; i < length; i++) {
        quality += costs[i, rowAssign[i]];
      }
      return rowAssign;
    }
  }
}
