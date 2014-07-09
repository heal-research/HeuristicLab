#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.ScheduleEncoding.PermutationWithRepetition {
  [Item("PWRManipulator", "An operator which manipulates a PWR representation.")]
  [StorableClass]
  public abstract class PWRManipulator : ScheduleManipulator, IPWROperator {

    [StorableConstructor]
    protected PWRManipulator(bool deserializing) : base(deserializing) { }
    protected PWRManipulator(PWRManipulator original, Cloner cloner) : base(original, cloner) { }
    public PWRManipulator()
      : base() {
      ScheduleEncodingParameter.ActualName = "PermutationWithRepetition";
    }

    protected abstract void Manipulate(IRandom random, PWREncoding individual);

    public override IOperation InstrumentedApply() {
      var solution = ScheduleEncodingParameter.ActualValue as PWREncoding;
      if (solution == null) throw new InvalidOperationException("ScheduleEncoding was not found or is not of type PWREncoding.");
      Manipulate(RandomParameter.ActualValue, solution);
      return base.InstrumentedApply();
    }

  }
}
