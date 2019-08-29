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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("JobSequenceMatrixCreator", "Creator class used to create Job Sequence Matrix solutions for standard JobShop scheduling problems.")]
  [StorableType("F8053C69-31C2-4E05-8FA0-5AED15FAF804")]
  public class JSMRandomCreator : ScheduleCreator<JSMEncoding>, IStochasticOperator {

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected JSMRandomCreator(StorableConstructorFlag _) : base(_) { }
    protected JSMRandomCreator(JSMRandomCreator original, Cloner cloner) : base(original, cloner) { }
    public JSMRandomCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new JSMRandomCreator(this, cloner);
    }

    public static JSMEncoding Apply(int jobs, int resources, IRandom random) {
      var solution = new JSMEncoding(random.Next());
      for (int i = 0; i < resources; i++) {
        solution.JobSequenceMatrix.Add(new Permutation(PermutationTypes.Absolute, jobs, random));
      }
      return solution;
    }

    protected override JSMEncoding CreateSolution() {
      return Apply(JobsParameter.ActualValue.Value, ResourcesParameter.ActualValue.Value, RandomParameter.ActualValue);
    }
  }
}
