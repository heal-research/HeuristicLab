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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("PermutationWithRepetitionRandomCreator", "Creates PWR-individuals at random.")]
  [StorableType("6E753916-C0FD-4585-B6A6-47FD66ED098F")]
  public class PWRRandomCreator : ScheduleCreator<PWREncoding>, IStochasticOperator {

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected PWRRandomCreator(StorableConstructorFlag _) : base(_) { }
    protected PWRRandomCreator(PWRRandomCreator original, Cloner cloner) : base(original, cloner) { }
    public PWRRandomCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PWRRandomCreator(this, cloner);
    }

    public static PWREncoding Apply(int jobs, int resources, IRandom random) {
      return new PWREncoding(jobs, resources, random);
    }

    protected override PWREncoding CreateSolution() {
      return Apply(JobsParameter.ActualValue.Value, ResourcesParameter.ActualValue.Value, RandomParameter.ActualValue);
    }
  }
}
