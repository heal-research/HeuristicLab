#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("ScheduleCrossover", "A scheduling crossover operation.")]
  [StorableClass]
  public abstract class ScheduleCrossover<T> : SingleSuccessorOperator, IScheduleCrossover, IStochasticOperator where T : Item {
    #region IScheduleCrossover Members
    public ILookupParameter<ItemArray<T>> ParentsParameter {
      get { return (ScopeTreeLookupParameter<T>)Parameters["Parents"]; }
    }

    public ILookupParameter<T> ChildParameter {
      get { return (ILookupParameter<T>)Parameters["Child"]; }
    }
    #endregion

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected ScheduleCrossover(bool deserializing) : base(deserializing) { }
    protected ScheduleCrossover(ScheduleCrossover<T> original, Cloner cloner)
      : base(original, cloner) {
    }
    public ScheduleCrossover()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
      Parameters.Add(new ScopeTreeLookupParameter<T>("Parents", "The parent solution which should be crossed."));
      ParentsParameter.ActualName = "SchedulingSolution";
      Parameters.Add(new LookupParameter<T>("Child", "The child solution resulting from the crossover."));
      ChildParameter.ActualName = "SchedulingSolution";
    }
  }
}
