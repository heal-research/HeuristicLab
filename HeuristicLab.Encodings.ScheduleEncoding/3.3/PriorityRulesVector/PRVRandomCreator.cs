#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("PriorityRulesRandomCreator", "Creator class used to create PRV encoding objects for scheduling problems.")]
  [StorableType("5FF2A11E-86F9-4A8B-8E1C-713D6801506C")]
  public class PRVRandomCreator : ScheduleCreator<PRVEncoding>, IStochasticOperator {

    [Storable]
    public int NrOfRules { get; set; }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected PRVRandomCreator(StorableConstructorFlag _) : base(_) { }
    protected PRVRandomCreator(PRVRandomCreator original, Cloner cloner)
      : base(original, cloner) {
      this.NrOfRules = original.NrOfRules;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PRVRandomCreator(this, cloner);
    }

    public PRVRandomCreator()
      : base() {
      NrOfRules = 10;
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator."));
    }

    public static PRVEncoding Apply(int jobs, int resources, IRandom random, int nrOfRules) {
      return new PRVEncoding(jobs * resources, random, 0, nrOfRules);
    }

    protected override PRVEncoding CreateSolution() {
      return Apply(JobsParameter.ActualValue.Value, ResourcesParameter.ActualValue.Value, RandomParameter.ActualValue, NrOfRules);
    }
  }
}
