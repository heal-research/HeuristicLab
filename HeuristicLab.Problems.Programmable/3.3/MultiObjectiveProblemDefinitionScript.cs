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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Programmable {
  [Item("Multi-objective Problem Definition Script", "Script that defines the parameter vector and evaluates the solution for a programmable problem.")]
  [StorableClass]
  public sealed class MultiObjectiveProblemDefinitionScript : ProblemDefinitionScript, IMultiObjectiveProblemDefinition, IStorableContent {
    public string Filename { get; set; }

    private new IMultiObjectiveProblemDefinition CompiledProblemDefinition {
      get { return (IMultiObjectiveProblemDefinition)base.CompiledProblemDefinition; }
    }

    [StorableConstructor]
    private MultiObjectiveProblemDefinitionScript(bool deserializing) : base(deserializing) { }
    private MultiObjectiveProblemDefinitionScript(MultiObjectiveProblemDefinitionScript original, Cloner cloner) : base(original, cloner) { }
    public MultiObjectiveProblemDefinitionScript() : base(ScriptTemplates.CompiledMultiObjectiveProblemDefinition) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveProblemDefinitionScript(this, cloner);
    }

    bool[] IMultiObjectiveProblemDefinition.Maximization {
      get { return CompiledProblemDefinition.Maximization; }
    }

    double[] IMultiObjectiveProblemDefinition.Evaluate(Individual individual, IRandom random) {
      return CompiledProblemDefinition.Evaluate(individual, random);
    }

    void IMultiObjectiveProblemDefinition.Analyze(Individual[] individuals, double[][] qualities, ResultCollection results, IRandom random) {
      CompiledProblemDefinition.Analyze(individuals, qualities, results, random);
    }
  }
}
