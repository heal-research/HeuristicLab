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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.NSGA2 {
  /// <summary>
  /// The Nondominated Sorting Genetic Algorithm II was introduced in Deb et al. 2002. A Fast and Elitist Multiobjective Genetic Algorithm: NSGA-II. IEEE Transactions on Evolutionary Computation, 6(2), pp. 182-197.
  /// </summary>
  [Item("NSGA2", "The Nondominated Sorting Genetic Algorithm II was introduced in Deb et al. 2002. A Fast and Elitist Multiobjective Genetic Algorithm: NSGA-II. IEEE Transactions on Evolutionary Computation, 6(2), pp. 182-197.")]
  [Creatable("Algorithms")]
  [StorableClass]
  public class NSGA2 : EngineAlgorithm {
    #region Problem Properties
    public override Type ProblemType {
      get { return typeof(IMultiObjectiveProblem); }
    }
    public new IMultiObjectiveProblem Problem {
      get { return (IMultiObjectiveProblem)base.Problem; }
      set { base.Problem = value; }
    }
    #endregion

    #region Parameter Properties
    private ValueLookupParameter<IntValue> PopulationSizeParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["PopulationSize"]; }
    }
    #endregion

    #region Properties
    #endregion

    [StorableConstructor]
    public NSGA2(bool deserializing) : base(deserializing) { }
    public NSGA2() {
      // TODO: Add your parameters here
      Parameters.Add(new ValueParameter<IntValue>("PopulationSize", "The population size of the algorithm.", new IntValue(100)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      NSGA2 clone = (NSGA2)base.Clone(cloner);
      // TODO: Clone additional fields
      return clone;
    }
  }
}
