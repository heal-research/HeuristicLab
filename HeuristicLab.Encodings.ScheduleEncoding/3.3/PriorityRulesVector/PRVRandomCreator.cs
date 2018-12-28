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
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Encodings.ScheduleEncoding.PriorityRulesVector {
  [Item("PriorityRulesRandomCreator", "Creator class used to create PRV encoding objects for scheduling problems.")]
  [StorableType("5FF2A11E-86F9-4A8B-8E1C-713D6801506C")]
  public class PRVRandomCreator : ScheduleCreator, IStochasticOperator {

    [Storable]
    public IntValue NrOfRules { get; set; }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public IValueLookupParameter<IntValue> JobsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["Jobs"]; }
    }
    public IValueLookupParameter<IntValue> ResourcesParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["Resources"]; }
    }

    [StorableConstructor]
    protected PRVRandomCreator(StorableConstructorFlag _) : base(_) { }
    protected PRVRandomCreator(PRVRandomCreator original, Cloner cloner)
      : base(original, cloner) {
      this.NrOfRules = cloner.Clone(original.NrOfRules);
    }
    public PRVRandomCreator()
      : base() {
      NrOfRules = new IntValue(10);
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Jobs", "The number of jobs handled in this problem instance."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Resources", "The number of resources used in this problem instance."));
      ScheduleEncodingParameter.ActualName = "PriorityRulesVector";
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PRVRandomCreator(this, cloner);
    }

    public static PRVEncoding Apply(int jobs, int resources, IRandom random, IntValue nrOfRules) {
      return new PRVEncoding(jobs * resources, random, 0, nrOfRules.Value, nrOfRules);
    }

    protected override IScheduleEncoding CreateSolution() {
      return Apply(JobsParameter.ActualValue.Value, ResourcesParameter.ActualValue.Value, RandomParameter.ActualValue, NrOfRules);
    }


  }
}
