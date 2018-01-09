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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.QuadraticAssignment {
  /// <summary>
  /// An operator for analyzing the diversity of solutions of Quadratic Assignment Problems regarding their structural identity (number of equal facilty->location assignments).
  /// </summary>
  [Item("QAPPopulationDiversityAnalyzer", "An operator for analyzing the diversity of solutions of Quadratic Assignment Problems regarding their structural identity (number of equal facilty->location assignments).")]
  [StorableClass]
  [Obsolete("Use the PopulationSimilarityAnalyzer in the HeuristicLab.Analysis plugin instead.")]
  [NonDiscoverableType]
#pragma warning disable 0612
  internal sealed class QAPPopulationDiversityAnalyzer : PopulationDiversityAnalyzer<Permutation> {
#pragma warning restore 0612
    public IValueParameter<BoolValue> UsePhenotypeSimilarityParameter {
      get { return (IValueParameter<BoolValue>)Parameters["UsePhenotypeSimilarity"]; }
    }
    public ILookupParameter<DoubleMatrix> WeightsParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Weights"]; }
    }
    public ILookupParameter<DoubleMatrix> DistancesParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Distances"]; }
    }

    [StorableConstructor]
    private QAPPopulationDiversityAnalyzer(bool deserializing) : base(deserializing) { }
    private QAPPopulationDiversityAnalyzer(QAPPopulationDiversityAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public QAPPopulationDiversityAnalyzer()
      : base() {
      Parameters.Add(new ValueParameter<BoolValue>("UsePhenotypeSimilarity", "True if the similarity should be measured a level closer to the phenotype (the number of similar assignments of individual weights to distances). Set to false if the number of equal assignments (facility to location) should be counted.", new BoolValue(false)));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Weights", "The weights matrix."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Distances", "The distances matrix."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QAPPopulationDiversityAnalyzer(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey("UsePhenotypeSimilarity"))
        Parameters.Add(new ValueParameter<BoolValue>("UsePhenotypeSimilarity", "True if the similarity should be measured a level closer to the phenotype (the number of similar assignments of individual weights to distances). Set to false if the number of equal assignments (facility to location) should be counted.", new BoolValue(false)));
      if (!Parameters.ContainsKey("Weights"))
        Parameters.Add(new LookupParameter<DoubleMatrix>("Weights", "The weights matrix."));
      if (!Parameters.ContainsKey("Distances"))
        Parameters.Add(new LookupParameter<DoubleMatrix>("Distances", "The distances matrix."));
      #endregion
    }

    protected override double[,] CalculateSimilarities(Permutation[] solutions) {
      DoubleMatrix weights = WeightsParameter.ActualValue, distances = DistancesParameter.ActualValue;
      bool phenotypeSimilarity = UsePhenotypeSimilarityParameter.Value.Value;
      int count = solutions.Length;
      double[,] similarities = new double[count, count];

      for (int i = 0; i < count; i++) {
        similarities[i, i] = 1;
        for (int j = i + 1; j < count; j++) {
          if (phenotypeSimilarity)
            similarities[i, j] = QAPPermutationProximityCalculator.CalculatePhenotypeSimilarity(solutions[i], solutions[j], weights, distances);
          else similarities[i, j] = HammingSimilarityCalculator.CalculateSimilarity(solutions[i], solutions[j]);
          similarities[j, i] = similarities[i, j];
        }
      }
      return similarities;
    }
  }
}
