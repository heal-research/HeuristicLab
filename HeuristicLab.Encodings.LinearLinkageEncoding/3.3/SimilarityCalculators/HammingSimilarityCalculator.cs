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
#endregion

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("Hamming Similarity Calculator for LinearLinkage", "Calculates the hamming similarity for two linear linkage encoded solutions.")]
  [StorableClass]
  public sealed class HammingSimilarityCalculator : SingleObjectiveSolutionSimilarityCalculator {

    protected override bool IsCommutative {
      get { return true; }
    }

    [StorableConstructor]
    private HammingSimilarityCalculator(bool deserializing) : base(deserializing) { }
    private HammingSimilarityCalculator(HammingSimilarityCalculator original, Cloner cloner) : base(original, cloner) { }
    public HammingSimilarityCalculator() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new HammingSimilarityCalculator(this, cloner);
    }

    public static double CalculateSimilarity(LinearLinkage left, LinearLinkage right) {
      if (left.Length != right.Length) throw new ArgumentException("Comparing encodings of unequal length");
      var dist = 0;
      for (var i = 0; i < left.Length; i++) {
        if (left[i] != right[i]) dist++;
      }
      return dist / (double)left.Length;
    }

    public override double CalculateSolutionSimilarity(IScope leftSolution, IScope rightSolution) {
      var left = leftSolution.Variables[SolutionVariableName].Value as LinearLinkage;
      var right = rightSolution.Variables[SolutionVariableName].Value as LinearLinkage;

      return CalculateSimilarity(left, right);
    }
  }
}
