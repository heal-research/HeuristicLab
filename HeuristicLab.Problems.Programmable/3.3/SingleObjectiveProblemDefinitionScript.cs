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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HEAL.Attic;

namespace HeuristicLab.Problems.Programmable {
  [Item("Single-objective Problem Definition Script", "Script that defines the parameter vector and evaluates the solution for a programmable problem.")]
  [StorableType("D0B2A649-EDDE-4A6E-A3B5-F40F5FD1B2C0")]
  public sealed class SingleObjectiveProblemDefinitionScript : ProblemDefinitionScript, ISingleObjectiveProblemDefinition, IStorableContent {
    public string Filename { get; set; }

    private new ISingleObjectiveProblemDefinition CompiledProblemDefinition {
      get { return (ISingleObjectiveProblemDefinition)base.CompiledProblemDefinition; }
    }

    [StorableConstructor]
    private SingleObjectiveProblemDefinitionScript(StorableConstructorFlag _) : base(_) { }
    private SingleObjectiveProblemDefinitionScript(SingleObjectiveProblemDefinitionScript original, Cloner cloner) : base(original, cloner) { }
    public SingleObjectiveProblemDefinitionScript() : base(ScriptTemplates.CompiledSingleObjectiveProblemDefinition) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveProblemDefinitionScript(this, cloner);
    }

    bool ISingleObjectiveProblemDefinition.Maximization {
      get { return CompiledProblemDefinition.Maximization; }
    }

    double ISingleObjectiveProblemDefinition.Evaluate(Individual individual, IRandom random) {
      return CompiledProblemDefinition.Evaluate(individual, random);
    }

    void ISingleObjectiveProblemDefinition.Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      CompiledProblemDefinition.Analyze(individuals, qualities, results, random);
    }
    IEnumerable<Individual> ISingleObjectiveProblemDefinition.GetNeighbors(Individual individual, IRandom random) {
      return CompiledProblemDefinition.GetNeighbors(individual, random);
    }
  }
}
