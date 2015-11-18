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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.ExternalEvaluation.Programmable;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("ProblemDefinitionScript", "Script that defines the parameter vector and evaluates the solution for a programmable problem.")]
  [StorableClass]
  public sealed class MultiObjectiveOptimizationSupportScript : OptimizationSupportScript<IMultiObjectiveOptimizationSupport>, IMultiObjectiveOptimizationSupport {

    protected override string CodeTemplate {
      get { return Templates.CompiledMultiObjectiveOptimizationSupport; }
    }

    [StorableConstructor]
    private MultiObjectiveOptimizationSupportScript(bool deserializing) : base(deserializing) { }
    private MultiObjectiveOptimizationSupportScript(MultiObjectiveOptimizationSupportScript original, Cloner cloner) : base(original, cloner) { }
    public MultiObjectiveOptimizationSupportScript() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveOptimizationSupportScript(this, cloner);
    }

    void IMultiObjectiveOptimizationSupport.Analyze(Individual[] individuals, double[][] qualities, ResultCollection results, IRandom random) {
      CompiledInstance.Analyze(individuals, qualities, results, random);
    }
  }
}
