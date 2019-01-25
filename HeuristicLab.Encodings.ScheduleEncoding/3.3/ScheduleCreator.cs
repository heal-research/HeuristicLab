#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("ScheduleCreator", "Represents the generalized form of creators for Scheduling Problems.")]
  [StorableType("3DDA1485-4518-4F1D-A475-795FFE63C98E")]
  public abstract class ScheduleCreator : InstrumentedOperator, IScheduleCreator {

    public ILookupParameter<IScheduleEncoding> ScheduleEncodingParameter {
      get { return (ILookupParameter<IScheduleEncoding>)Parameters["ScheduleEncoding"]; }
    }

    [StorableConstructor]
    protected ScheduleCreator(StorableConstructorFlag _) : base(_) { }
    protected ScheduleCreator(ScheduleCreator original, Cloner cloner) : base(original, cloner) { }
    public ScheduleCreator()
      : base() {
      Parameters.Add(new LookupParameter<IScheduleEncoding>("ScheduleEncoding", "The new scheduling solutioncandidate."));
    }

    public override IOperation InstrumentedApply() {
      ScheduleEncodingParameter.ActualValue = CreateSolution();
      return base.InstrumentedApply();
    }

    protected abstract IScheduleEncoding CreateSolution();
  }
}
