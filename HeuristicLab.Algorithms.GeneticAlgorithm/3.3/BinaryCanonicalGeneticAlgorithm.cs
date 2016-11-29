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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Selection;
using TProblem = HeuristicLab.Optimization.SingleObjectiveProblem<HeuristicLab.Encodings.BinaryVectorEncoding.BinaryVectorEncoding, HeuristicLab.Encodings.BinaryVectorEncoding.BinaryVector>;
using TContext = HeuristicLab.Algorithms.GeneticAlgorithm.EvolutionaryAlgorithmContext<HeuristicLab.Optimization.SingleObjectiveProblem<HeuristicLab.Encodings.BinaryVectorEncoding.BinaryVectorEncoding, HeuristicLab.Encodings.BinaryVectorEncoding.BinaryVector>, HeuristicLab.Encodings.BinaryVectorEncoding.BinaryVectorEncoding, HeuristicLab.Encodings.BinaryVectorEncoding.BinaryVector>;

namespace HeuristicLab.Algorithms.GeneticAlgorithm {
  [Item("Canonical Genetic Algorithm (binary) (CGA)", "Performs a canonical genetic algorithm.")]
  [Creatable(CreatableAttribute.Categories.PopulationBasedAlgorithms, Priority = 999)]
  [StorableClass]
  public sealed class BinaryCanonicalGeneticAlgorithm : CanonicalGeneticAlgorithm<TProblem, BinaryVectorEncoding, BinaryVector> {

    [StorableConstructor]
    private BinaryCanonicalGeneticAlgorithm(bool deserializing) : base(deserializing) { }
    private BinaryCanonicalGeneticAlgorithm(BinaryCanonicalGeneticAlgorithm original, Cloner cloner)
      : base(original, cloner) { }
    public BinaryCanonicalGeneticAlgorithm() {
      SelectorParameter.ValidValues.Add(new TournamentSelector<TContext, TProblem, BinaryVectorEncoding, BinaryVector>());
      MutatorParameter.ValidValues.Add(new SingleBitflipManipulator<TContext>());
      MutatorParameter.ValidValues.Add(new MultiBitflipManipulator<TContext>());
      CrossoverParameter.ValidValues.Add(new NPointCrossover<TContext>());
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinaryCanonicalGeneticAlgorithm(this, cloner);
    }
  }
}
