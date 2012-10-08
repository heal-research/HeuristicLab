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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("ScheduleCreator", "Represents the generalized form of creators for Scheduling Problems.")]
  [StorableClass]
  public abstract class ScheduleCreator<T> : SingleSuccessorOperator, IScheduleCreator where T : Item {
    public ILookupParameter<T> ScheduleEncodingParameter {
      get { return (ILookupParameter<T>)Parameters["ScheduleEncoding"]; }
    }


    [StorableConstructor]
    protected ScheduleCreator(bool deserializing) : base(deserializing) { }
    protected ScheduleCreator(ScheduleCreator<T> original, Cloner cloner)
      : base(original, cloner) {
    }
    public ScheduleCreator()
      : base() {
      Parameters.Add(new LookupParameter<T>("ScheduleEncoding", "The new scheduling solutioncandidate."));
    }


    public override IOperation Apply() {
      ScheduleEncodingParameter.ActualValue = CreateSolution();
      return base.Apply();
    }
    protected abstract T CreateSolution();
  }
}
