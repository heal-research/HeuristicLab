#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Optimization.Model2 {
  [Item("Stochastic Algorithm", "Stochastic context-based algorithms to facilitate applying operators.")]
  [StorableClass]
  public abstract class StochasticAlgorithm<TContext> : ContextAlgorithm<TContext>
    where TContext : class, IStochasticContext, new() {
    
    [Storable]
    private FixedValueParameter<BoolValue> setSeedRandomlyParameter;
    private IFixedValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return setSeedRandomlyParameter; }
    }
    [Storable]
    private FixedValueParameter<IntValue> seedParameter;
    private IFixedValueParameter<IntValue> SeedParameter {
      get { return seedParameter; }
    }
    
    public bool SetSeedRandomly {
      get { return setSeedRandomlyParameter.Value.Value; }
      set { setSeedRandomlyParameter.Value.Value = value; }
    }
    public int Seed {
      get { return seedParameter.Value.Value; }
      set { seedParameter.Value.Value = value; }
    }

    [StorableConstructor]
    protected StochasticAlgorithm(bool deserializing) : base(deserializing) { }
    protected StochasticAlgorithm(StochasticAlgorithm<TContext> original, Cloner cloner)
      : base(original, cloner) {
      setSeedRandomlyParameter = cloner.Clone(original.setSeedRandomlyParameter);
      seedParameter = cloner.Clone(original.seedParameter);
    }
    protected StochasticAlgorithm()
      : base() {
      Parameters.Add(setSeedRandomlyParameter = new FixedValueParameter<BoolValue>("SetSeedRandomly", "Whether to overwrite the seed with a random value each time the algorithm is run.", new BoolValue(true)));
      Parameters.Add(seedParameter = new FixedValueParameter<IntValue>("Seed", "The random seed that is used in the stochastic algorithm", new IntValue(0)));
    }

    protected override void Initialize(CancellationToken cancellationToken) {
      base.Initialize(cancellationToken);

      if (SetSeedRandomly) {
        var rnd = new System.Random();
        Seed = rnd.Next();
      }

      Context.Random = new MersenneTwister((uint)Seed);
    }
  }
}
