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
using HeuristicLab.Data;
using HeuristicLab.Encodings.ScheduleEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;


namespace HeuristicLab.Problems.Scheduling {
  [Item("Scheduling Evaluator", "Represents a evaluator class for standard scheduling problems.")]
  [StorableClass]
  public abstract class SchedulingEvaluator : SingleSuccessorOperator, IScheduleEvaluator {
    [StorableConstructor]
    protected SchedulingEvaluator(bool deserializing) : base(deserializing) { }
    protected SchedulingEvaluator(SchedulingEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get {
        if (Parameters.ContainsKey("Quality"))
          return (ILookupParameter<DoubleValue>)Parameters["Quality"];
        else
          return null;
      }
    }
    public ILookupParameter<Schedule> ScheduleParameter {
      get { return (ILookupParameter<Schedule>)Parameters["Schedule"]; }
    }

    public SchedulingEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality value aka fitness value of the solution."));
      Parameters.Add(new LookupParameter<Schedule>("Schedule", "The decoded scheduling solution represented as generalized schedule."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator."));
    }

    protected abstract DoubleValue Evaluate(Schedule schedule);

    public override IOperation Apply() {
      Schedule schedule = ScheduleParameter.ActualValue;
      QualityParameter.ActualValue = Evaluate(schedule);
      return base.Apply();
    }
  }
}
