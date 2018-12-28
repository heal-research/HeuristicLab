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
#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HEAL.Fossil;

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// A base class for items that perform similarity calculation between two solutions.
  /// </summary>
  [Item("SimilarityCalculator", "A base class for items that perform similarity calculation between two solutions.")]
  [StorableType("ACDF2895-0C4E-4C34-A091-F41EF5C78241")]
  public abstract class SolutionSimilarityCalculator : Item, ISolutionSimilarityCalculator {
    protected abstract bool IsCommutative { get; }

    #region Properties
    [Storable]
    public string SolutionVariableName { get; set; }
    [Storable]
    public string QualityVariableName { get; set; }
    [Storable]
    public bool ExecuteInParallel { get; set; }
    [Storable]
    public int MaxDegreeOfParallelism { get; set; }
    #endregion

    [StorableConstructor]
    protected SolutionSimilarityCalculator(StorableConstructorFlag _) : base(_) { }

    protected SolutionSimilarityCalculator(SolutionSimilarityCalculator original, Cloner cloner)
      : base(original, cloner) {
      SolutionVariableName = original.SolutionVariableName;
      QualityVariableName = original.QualityVariableName;
      ExecuteInParallel = original.ExecuteInParallel;
      MaxDegreeOfParallelism = original.MaxDegreeOfParallelism;
    }

    protected SolutionSimilarityCalculator() : base() {
      ExecuteInParallel = false;
      MaxDegreeOfParallelism = -1;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (MaxDegreeOfParallelism == 0) {
        ExecuteInParallel = false;
        MaxDegreeOfParallelism = -1;
      }
    }

    public double[][] CalculateSolutionCrowdSimilarity(IScope leftSolutionCrowd, IScope rightSolutionCrowd) {
      if (leftSolutionCrowd == null || rightSolutionCrowd == null)
        throw new ArgumentException("Cannot calculate similarity because one of the provided crowds or both are null.");

      var leftIndividuals = leftSolutionCrowd.SubScopes;
      var rightIndividuals = rightSolutionCrowd.SubScopes;

      if (!leftIndividuals.Any() || !rightIndividuals.Any())
        throw new ArgumentException("Cannot calculate similarity because one of the provided crowds or both are empty.");

      var similarityMatrix = new double[leftIndividuals.Count][];
      for (int i = 0; i < leftIndividuals.Count; i++) {
        similarityMatrix[i] = new double[rightIndividuals.Count];
        for (int j = 0; j < rightIndividuals.Count; j++) {
          similarityMatrix[i][j] = CalculateSolutionSimilarity(leftIndividuals[i], rightIndividuals[j]);
        }
      }

      return similarityMatrix;
    }

    public double[][] CalculateSolutionCrowdSimilarity(IScope solutionCrowd) {
      if (solutionCrowd == null) {
        throw new ArgumentException("Cannot calculate similarity because the provided crowd is null.");
      }
      var individuals = solutionCrowd.SubScopes;

      if (!individuals.Any()) {
        throw new ArgumentException("Cannot calculate similarity because the provided crowd is empty.");
      }

      var similarityMatrix = new double[individuals.Count][];
      for (int i = 0; i < individuals.Count; i++) {
        similarityMatrix[i] = new double[individuals.Count];
      }

      if (ExecuteInParallel) {
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeOfParallelism };
        if (IsCommutative) {
          Parallel.For(0, individuals.Count, parallelOptions, i => {
            for (int j = i; j < individuals.Count; j++) {
              similarityMatrix[i][j] =
                similarityMatrix[j][i] = CalculateSolutionSimilarity(individuals[i], individuals[j]);
            }
          });
        } else {
          Parallel.For(0, individuals.Count, parallelOptions, i => {
            for (int j = i; j < individuals.Count; j++) {
              similarityMatrix[i][j] = CalculateSolutionSimilarity(individuals[i], individuals[j]);
              if (i == j) continue;
              similarityMatrix[j][i] = CalculateSolutionSimilarity(individuals[j], individuals[i]);
            }
          });
        }
      } else {
        if (IsCommutative) {
          for (int i = 0; i < individuals.Count; i++) {
            for (int j = i; j < individuals.Count; j++) {
              similarityMatrix[i][j] =
                similarityMatrix[j][i] = CalculateSolutionSimilarity(individuals[i], individuals[j]);
            }
          }
        } else {
          for (int i = 0; i < individuals.Count; i++) {
            for (int j = i; j < individuals.Count; j++) {
              similarityMatrix[i][j] = CalculateSolutionSimilarity(individuals[i], individuals[j]);
              if (i == j) continue;
              similarityMatrix[j][i] = CalculateSolutionSimilarity(individuals[j], individuals[i]);
            }
          }
        }
      }

      return similarityMatrix;
    }

    public abstract double CalculateSolutionSimilarity(IScope leftSolution, IScope rightSolution);

    public virtual bool Equals(IScope x, IScope y) {
      if (ReferenceEquals(x, y)) return true;
      if (x == null || y == null) return false;

      var q1 = x.Variables[QualityVariableName].Value;
      var q2 = y.Variables[QualityVariableName].Value;

      return CheckQualityEquality(q1, q2) && CalculateSolutionSimilarity(x, y).IsAlmost(1.0);
    }

    public virtual int GetHashCode(IScope scope) {
      var quality = scope.Variables[QualityVariableName].Value;
      var dv = quality as DoubleValue;
      if (dv != null)
        return dv.Value.GetHashCode();

      var da = quality as DoubleArray;
      if (da != null) {
        int hash = 17;
        unchecked {
          for (int i = 0; i < da.Length; ++i) {
            hash += hash * 23 + da[i].GetHashCode();
          }
          return hash;
        }
      }
      return 0;
    }

    private static bool CheckQualityEquality(IItem q1, IItem q2) {
      var d1 = q1 as DoubleValue;
      var d2 = q2 as DoubleValue;

      if (d1 != null && d2 != null)
        return d1.Value.IsAlmost(d2.Value);

      var da1 = q1 as DoubleArray;
      var da2 = q2 as DoubleArray;

      if (da1 != null && da2 != null) {
        if (da1.Length != da2.Length)
          throw new ArgumentException("The quality arrays must have the same length.");

        for (int i = 0; i < da1.Length; ++i) {
          if (!da1[i].IsAlmost(da2[i]))
            return false;
        }

        return true;
      }

      throw new ArgumentException("Could not determine quality equality.");
    }
  }
}
