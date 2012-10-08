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
  [Item("ScheduleManipulator", "A scheduling manipulation operation.")]
  [StorableClass]
  public abstract class ScheduleManipulator<T> : SingleSuccessorOperator, IScheduleManipulator, IStochasticOperator where T : Item {
    #region IScheduleManipulator Members

    public ILookupParameter<T> ScheduleEncodingParameter {
      get { return (ILookupParameter<T>)Parameters["ScheduleEncoding"]; }
    }

    #endregion

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected ScheduleManipulator(bool deserializing) : base(deserializing) { }
    protected ScheduleManipulator(ScheduleManipulator<T> original, Cloner cloner)
      : base(original, cloner) {
    }
    public ScheduleManipulator()
      : base() {
      Parameters.Add(new LookupParameter<T>("ScheduleEncoding", "The scheduling solution to be manipulated."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
    }

  }
}
