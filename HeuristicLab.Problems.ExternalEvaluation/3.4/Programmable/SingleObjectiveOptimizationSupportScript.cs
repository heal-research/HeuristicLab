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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HEAL.Attic;
using HeuristicLab.Problems.ExternalEvaluation.Programmable;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("ProblemDefinitionScript", "Script that defines the parameter vector and evaluates the solution for a programmable problem.")]
  [StorableType("3EB74171-6ECB-4479-861E-DA4EB39FE70C")]
  public sealed class SingleObjectiveOptimizationSupportScript : OptimizationSupportScript<ISingleObjectiveOptimizationSupport>, ISingleObjectiveOptimizationSupport {
    [StorableConstructor]
    private SingleObjectiveOptimizationSupportScript(StorableConstructorFlag _) : base(_) { }
    private SingleObjectiveOptimizationSupportScript(SingleObjectiveOptimizationSupportScript original, Cloner cloner) : base(original, cloner) { }
    public SingleObjectiveOptimizationSupportScript() : base(Templates.CompiledSingleObjectiveOptimizationSupport) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveOptimizationSupportScript(this, cloner);
    }

    void ISingleObjectiveOptimizationSupport.Analyze(IEncodedSolution[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      CompiledInstance.Analyze(individuals, qualities, results, random);
    }

    IEnumerable<IEncodedSolution> ISingleObjectiveOptimizationSupport.GetNeighbors(IEncodedSolution individual, IRandom random) {
      return CompiledInstance.GetNeighbors(individual, random);
    }
  }
}
