#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.QuadraticAssignment {
  /// <summary>
  /// An operator for analyzing the diversity of solutions of Quadratic Assignment Problems regarding their structural identity (number of equal facilty->location assignments).
  /// </summary>
  [Item("QAPPopulationDiversityAnalyzer", "An operator for analyzing the diversity of solutions of Quadratic Assignment Problems regarding their structural identity (number of equal facilty->location assignments).")]
  [StorableClass]
  public sealed class QAPPopulationDiversityAnalyzer : PopulationDiversityAnalyzer<Permutation> {
    [StorableConstructor]
    private QAPPopulationDiversityAnalyzer(bool deserializing) : base(deserializing) { }
    private QAPPopulationDiversityAnalyzer(QAPPopulationDiversityAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public QAPPopulationDiversityAnalyzer() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QAPPopulationDiversityAnalyzer(this, cloner);
    }

    protected override double[,] CalculateSimilarities(Permutation[] solutions) {
      int count = solutions.Length;
      double[,] similarities = new double[count, count];

      for (int i = 0; i < count; i++) {
        similarities[i, i] = 1;
        for (int j = i + 1; j < count; j++) {
          similarities[i, j] = CalculateSimilarity(solutions[i], solutions[j]);
          similarities[j, i] = similarities[i, j];
        }
      }
      return similarities;
    }

    private double CalculateSimilarity(Permutation assignment1, Permutation assignment2) {
      int identicalAssignments = 0;
      for (int i = 0; i < assignment1.Length; i++) {
        if (assignment1[i] == assignment2[i])
          identicalAssignments++;
      }
      return ((double)identicalAssignments) / assignment1.Length;
    }
  }
}
