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
using HeuristicLab.Encodings.BinaryVectorEncoding.LocalSearch;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.LocalSearch {
  [Item("Iterated Local Search (ILS)", "Performs a repeated local search by applying a kick to each previous local optimum.")]
  [Creatable(CreatableAttribute.Categories.SingleSolutionAlgorithms, Priority = 999)]
  [StorableClass]
  public sealed class BinaryIteratedLocalSearch : IteratedLocalSearch<SingleObjectiveProblem<BinaryVectorEncoding, BinaryVector>, BinaryVectorEncoding, BinaryVector> {

    [StorableConstructor]
    private BinaryIteratedLocalSearch(bool deserializing) : base(deserializing) { }
    private BinaryIteratedLocalSearch(BinaryIteratedLocalSearch original, Cloner cloner)
      : base(original, cloner) { }
    public BinaryIteratedLocalSearch() {
      var ls = new ExhaustiveBitflip<SingleObjectiveProblem<BinaryVectorEncoding, BinaryVector>, LocalSearchContext<SingleObjectiveProblem<BinaryVectorEncoding, BinaryVector>, BinaryVectorEncoding, BinaryVector>>();
      LocalSearchParameter.ValidValues.Add(ls);
      //foreach (var ls in ApplicationManager.Manager.GetInstances<IBinaryLocalSearch<LocalSearchContext<SingleObjectiveProblem<BinaryVectorEncoding, BinaryVector>, BinaryVectorEncoding, BinaryVector>>>())
        //LocalSearchParameter.ValidValues.Add(ls);
      KickerParameter.ValidValues.Add(new SingleBitflipManipulator<LocalSearchContext<SingleObjectiveProblem<BinaryVectorEncoding, BinaryVector>, BinaryVectorEncoding, BinaryVector>>());
      KickerParameter.ValidValues.Add(new MultiBitflipManipulator<LocalSearchContext<SingleObjectiveProblem<BinaryVectorEncoding, BinaryVector>, BinaryVectorEncoding, BinaryVector>>());
      //foreach (var ls in ApplicationManager.Manager.GetInstances<IManipulator<LocalSearchContext<SingleObjectiveProblem<BinaryVectorEncoding, BinaryVector>, BinaryVectorEncoding, BinaryVector>>>())
      //KickerParameter.ValidValues.Add(ls);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinaryIteratedLocalSearch(this, cloner);
    }
  }
}
