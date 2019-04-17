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

namespace HeuristicLab.Problems.Programmable {
  [Item("Multi-objective Problem Definition Script", "Script that defines the parameter vector and evaluates the solution for a programmable problem.")]
  [StorableType("17741D64-CF9D-4CCF-9892-0590C325D4E6")]
  public sealed class MultiObjectiveProblemDefinitionScript<TEncoding, TEncodedSolution> : ProblemDefinitionScript<TEncoding, TEncodedSolution>, IMultiObjectiveProblemDefinition<TEncoding, TEncodedSolution>, IStorableContent
    where TEncoding : class, IEncoding<TEncodedSolution>
    where TEncodedSolution : class, IEncodedSolution {
    public string Filename { get; set; }

    private new IMultiObjectiveProblemDefinition<TEncoding, TEncodedSolution> CompiledProblemDefinition {
      get { return (IMultiObjectiveProblemDefinition<TEncoding, TEncodedSolution>)base.CompiledProblemDefinition; }
    }

    [StorableConstructor]
    private MultiObjectiveProblemDefinitionScript(StorableConstructorFlag _) : base(_) { }
    private MultiObjectiveProblemDefinitionScript(MultiObjectiveProblemDefinitionScript<TEncoding, TEncodedSolution> original, Cloner cloner) : base(original, cloner) { }
    public MultiObjectiveProblemDefinitionScript(string codeTemplate) : base(codeTemplate) { }
    public MultiObjectiveProblemDefinitionScript() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveProblemDefinitionScript<TEncoding, TEncodedSolution>(this, cloner);
    }

    int IMultiObjectiveProblemDefinition<TEncoding, TEncodedSolution>.Objectives => CompiledProblemDefinition.Objectives;

    bool[] IMultiObjectiveProblemDefinition<TEncoding, TEncodedSolution>.Maximization {
      get { return CompiledProblemDefinition.Maximization; }
    }

    double[] IMultiObjectiveProblemDefinition<TEncoding, TEncodedSolution>.Evaluate(TEncodedSolution individual, IRandom random) {
      return CompiledProblemDefinition.Evaluate(individual, random);
    }

    void IMultiObjectiveProblemDefinition<TEncoding, TEncodedSolution>.Analyze(TEncodedSolution[] individuals, double[][] qualities, ResultCollection results, IRandom random) {
      CompiledProblemDefinition.Analyze(individuals, qualities, results, random);
    }
  }
}
