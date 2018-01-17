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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Model2 {
  [Item("Stochastic Context", "A base class for stochastic algorithms' contexts.")]
  [StorableClass]
  public class StochasticContext : BasicContext, IStochasticContext {
    
    [Storable]
    private IValueParameter<IRandom> random;
    public IRandom Random {
      get { return random.Value; }
      set { random.Value = value; }
    }

    [StorableConstructor]
    protected StochasticContext(bool deserializing) : base(deserializing) { }
    protected StochasticContext(StochasticContext original, Cloner cloner)
    : base(original, cloner) {
      random = cloner.Clone(original.random);
    }
    protected StochasticContext() : base() {
      Parameters.Add(random = new ValueParameter<IRandom>("Random"));
    }
    protected StochasticContext(string name) : base(name) {
      Parameters.Add(random = new ValueParameter<IRandom>("Random"));
    }
    protected StochasticContext(string name, ParameterCollection parameters) : base(name, parameters) {
      Parameters.Add(random = new ValueParameter<IRandom>("Random"));
    }
    protected StochasticContext(string name, string description) : base(name, description) {
      Parameters.Add(random = new ValueParameter<IRandom>("Random"));
    }
    protected StochasticContext(string name, string description, ParameterCollection parameters) : base(name, description, parameters) {
      Parameters.Add(random = new ValueParameter<IRandom>("Random"));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StochasticContext(this, cloner);
    }
  }
}
