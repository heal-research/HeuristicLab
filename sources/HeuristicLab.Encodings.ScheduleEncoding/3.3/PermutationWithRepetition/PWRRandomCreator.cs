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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.ScheduleEncoding.PermutationWithRepetition {
  [Item("PermutationWithRepetitionRandomCreator", "Creates PWR-individuals at random.")]
  [StorableClass]
  public class PWRRandomCreator : ScheduleCreator<PWREncoding>, IStochasticOperator {
    public IRandom Random {
      get { return RandomParameter.ActualValue; }
    }

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
    protected PWRRandomCreator(bool deserializing) : base(deserializing) { }
    protected PWRRandomCreator(PWRRandomCreator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PWRRandomCreator(this, cloner);
    }

    public PWRRandomCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Jobs", "The number of jobs handled in this problem instance."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Resources", "The number of resources used in this problem instance."));

      ScheduleEncodingParameter.ActualName = "PermutationWithRepetition";
    }

    public static PWREncoding Apply(int jobs, int resources, IRandom random) {
      return new PWREncoding(jobs, resources, random);
    }


    protected override PWREncoding CreateSolution() {
      return Apply(JobsParameter.ActualValue.Value, ResourcesParameter.ActualValue.Value, Random);
    }
  }
}
