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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("Euclidean Similarity Calculator for RealVector", "Calculates the solution similarity based on the Euclidean distance and a transformation into (0;1] between two real vectors.")]
  [StorableClass]
  public sealed class EuclideanSimilarityCalculator : SingleObjectiveSolutionSimilarityCalculator {
    protected override bool IsCommutative {
      get { return true; }
    }

    [Storable]
    private double scaling;
    /// <summary>
    /// The higher the scaling, the higher the similarity score for solutions of larger Euclidean distance.
    /// A value of 1 means no scaling is applied. The function for squashing the numbers into (0;1] is
    /// 1 / (1 + x / scaling) where x is the Euclidean distance.
    /// </summary>
    public double Scaling {
      get { return scaling; }
      set { scaling = value; }
    }

    [StorableConstructor]
    private EuclideanSimilarityCalculator(bool deserializing) : base(deserializing) { }
    private EuclideanSimilarityCalculator(EuclideanSimilarityCalculator original, Cloner cloner)
      : base(original, cloner) {
      scaling = original.scaling;
    }
    public EuclideanSimilarityCalculator() {
      scaling = 1;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new EuclideanSimilarityCalculator(this, cloner);
    }

    public static double CalculateSimilarity(RealVector left, RealVector right, double scaling = 1.0) {
      if (left == null || right == null)
        throw new ArgumentException("Cannot calculate similarity because one or both of the provided solutions is null.");
      if (left.Length != right.Length)
        throw new ArgumentException("Cannot calculate similarity because the provided solutions have different lengths.");
      if (left.Length == 0)
        throw new ArgumentException("Cannot calculate similarity because solutions are of length 0.");
      if (scaling <= 0)
        throw new ArgumentException("Cannot choose a 0 or negative scaling value.");
      if (ReferenceEquals(left, right)) return 1.0;

      var distance = 0.0;
      for (int i = 0; i < left.Length; i++)
        distance += (left[i] - right[i]) * (left[i] - right[i]);
      return 1.0 / (1.0 + Math.Sqrt(distance) / scaling);

    }

    public override double CalculateSolutionSimilarity(IScope leftSolution, IScope rightSolution) {
      var left = leftSolution.Variables[SolutionVariableName].Value as RealVector;
      var right = rightSolution.Variables[SolutionVariableName].Value as RealVector;

      return CalculateSimilarity(left, right, Scaling);
    }
  }
}
