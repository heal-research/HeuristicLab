#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.Permutation;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TSP {
  /// <summary>
  /// A base class for operators which evaluate TSP solutions given in path representation.
  /// </summary>
  [Item("TSPPathEvaluator", "A base class for operators which evaluate TSP solutions given in path representation.")]
  [EmptyStorableClass]
  public abstract class TSPPathEvaluator : TSPEvaluator, ITSPPathEvaluator {
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }

    protected TSPPathEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The TSP solution given in path representation which should be evaluated."));
    }

    public sealed override IOperation Apply() {
      Permutation permutation = PermutationParameter.ActualValue;

      double length = 0;
      for (int i = 0; i < permutation.Length - 1; i++)
        length += CalculateDistance(permutation[i], permutation[i + 1]);
      length += CalculateDistance(permutation[permutation.Length - 1], permutation[0]);
      QualityParameter.ActualValue = new DoubleData(length);

      return base.Apply();
    }

    /// <summary>
    /// Calculates the distance between two cities.
    /// </summary>
    /// <param name="a">The index of the first city.</param>
    /// <param name="b">The index of the second city.</param>
    /// <returns>The calculated distance.</returns>
    protected abstract double CalculateDistance(int a, int b);
  }
}
