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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization.Operators;
using HEAL.Fossil;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("Hamming Similarity Calculator for RealVector", "Calculates the solution similarity based on the Hamming distance between two real vectors.")]
  [StorableType("D3E14606-0E63-4DF6-BF05-AADF5AA37E44")]
  public sealed class HammingSimilarityCalculator : SingleObjectiveSolutionSimilarityCalculator {
    protected override bool IsCommutative {
      get { return true; }
    }

    [StorableConstructor]
    private HammingSimilarityCalculator(StorableConstructorFlag _) : base(_) { }
    private HammingSimilarityCalculator(HammingSimilarityCalculator original, Cloner cloner) : base(original, cloner) { }
    public HammingSimilarityCalculator() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new HammingSimilarityCalculator(this, cloner);
    }

    public static double CalculateSimilarity(RealVector left, RealVector right) {
      if (left == null || right == null)
        throw new ArgumentException("Cannot calculate similarity because one or both of the provided solutions is null.");
      if (left.Length != right.Length)
        throw new ArgumentException("Cannot calculate similarity because the provided solutions have different lengths.");
      if (left.Length == 0)
        throw new ArgumentException("Cannot calculate similarity because solutions are of length 0.");
      if (ReferenceEquals(left, right)) return 1.0;

      double similarity = 0.0;
      for (int i = 0; i < left.Length; i++)
        if (left[i] == right[i]) similarity++;
      return similarity / left.Length;

    }

    public override double CalculateSolutionSimilarity(IScope leftSolution, IScope rightSolution) {
      var left = leftSolution.Variables[SolutionVariableName].Value as RealVector;
      var right = rightSolution.Variables[SolutionVariableName].Value as RealVector;

      return CalculateSimilarity(left, right);
    }
  }
}
