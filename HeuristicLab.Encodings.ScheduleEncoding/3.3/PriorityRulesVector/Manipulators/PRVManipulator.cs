#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("PRVManipulator", "An operator which manipulates a PRV representation.")]
  [StorableClass]
  public abstract class PRVManipulator : ScheduleManipulator, IPRVRulesOperator {

    public ILookupParameter<IntValue> NumberOfRulesParameter {
      get { return (ILookupParameter<IntValue>)Parameters["NumberOfRulesParameter"]; }
    }


    [StorableConstructor]
    protected PRVManipulator(bool deserializing) : base(deserializing) { }
    protected PRVManipulator(PRVManipulator original, Cloner cloner) : base(original, cloner) { }

    public PRVManipulator()
      : base() {
      Parameters.Add(new LookupParameter<IntValue>("NumberOfRulesParameter"));
    }

    protected abstract void Manipulate(IRandom random, PRVEncoding individual, int numberOfRules);

    public override IOperation InstrumentedApply() {
      var solution = ScheduleParameter.ActualValue as PRVEncoding;
      if (solution == null) throw new InvalidOperationException("ScheduleEncoding was not found or is not of type PRVEncoding.");
      Manipulate(RandomParameter.ActualValue, solution, NumberOfRulesParameter.ActualValue.Value);
      return base.InstrumentedApply();
    }

  }
}
