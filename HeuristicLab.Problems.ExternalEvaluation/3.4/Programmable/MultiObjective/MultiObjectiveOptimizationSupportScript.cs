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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.ExternalEvaluation.Programmable;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("ProblemDefinitionScript", "Script that defines the parameter vector and evaluates the solution for a programmable problem.")]
  [StorableType("617A7BEE-1B2F-4E39-A814-54CC4DFA2F02")]
  public sealed class MultiObjectiveOptimizationSupportScript<TEncodedSolution> : OptimizationSupportScript<IMultiObjectiveOptimizationSupport<TEncodedSolution>>, IMultiObjectiveOptimizationSupport<TEncodedSolution> {
    [StorableConstructor]
    private MultiObjectiveOptimizationSupportScript(StorableConstructorFlag _) : base(_) { }
    private MultiObjectiveOptimizationSupportScript(MultiObjectiveOptimizationSupportScript<TEncodedSolution> original, Cloner cloner) : base(original, cloner) { }
    public MultiObjectiveOptimizationSupportScript() {
      var codeTemplate = Templates.CompiledMultiObjectiveOptimizationSupport;
      codeTemplate = codeTemplate.Replace("ENCODING_NAMESPACE", typeof(TEncodedSolution).Namespace);
      codeTemplate = codeTemplate.Replace("SOLUTION_CLASS", typeof(TEncodedSolution).Name);
      Code = codeTemplate;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveOptimizationSupportScript<TEncodedSolution>(this, cloner);
    }

    void IMultiObjectiveOptimizationSupport<TEncodedSolution>.Analyze(TEncodedSolution[] individuals, double[][] qualities, ResultCollection results, IRandom random) {
      CompiledInstance.Analyze(individuals, qualities, results, random);
    }
  }
}
