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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.Programmable {
  [Item("Single-objective Problem Definition Script", "Script that defines the parameter vector and evaluates the solution for a programmable problem.")]
  [StorableType("D0B2A649-EDDE-4A6E-A3B5-F40F5FD1B2C0")]
  public sealed class SingleObjectiveProblemDefinitionScript<TEncoding, TEncodedSolution> : ProblemDefinitionScript<TEncoding, TEncodedSolution>, ISingleObjectiveProblemDefinition<TEncoding, TEncodedSolution>, IStorableContent
    where TEncoding : class, IEncoding<TEncodedSolution>
    where TEncodedSolution : class, IEncodedSolution {
    public string Filename { get; set; }

    private new ISingleObjectiveProblemDefinition<TEncoding, TEncodedSolution> CompiledProblemDefinition {
      get { return (ISingleObjectiveProblemDefinition<TEncoding, TEncodedSolution>)base.CompiledProblemDefinition; }
    }

    [StorableConstructor]
    private SingleObjectiveProblemDefinitionScript(StorableConstructorFlag _) : base(_) { }
    private SingleObjectiveProblemDefinitionScript(SingleObjectiveProblemDefinitionScript<TEncoding, TEncodedSolution> original, Cloner cloner) : base(original, cloner) { }
    public SingleObjectiveProblemDefinitionScript(string codeTemplate) : base(codeTemplate) { }
    public SingleObjectiveProblemDefinitionScript() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveProblemDefinitionScript<TEncoding, TEncodedSolution>(this, cloner);
    }

    bool ISingleObjectiveProblemDefinition<TEncoding, TEncodedSolution>.Maximization {
      get { return CompiledProblemDefinition.Maximization; }
    }

    double ISingleObjectiveProblemDefinition<TEncoding, TEncodedSolution>.Evaluate(TEncodedSolution individual, IRandom random) {
      return CompiledProblemDefinition.Evaluate(individual, random);
    }

    void ISingleObjectiveProblemDefinition<TEncoding, TEncodedSolution>.Analyze(TEncodedSolution[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      CompiledProblemDefinition.Analyze(individuals, qualities, results, random);
    }
    IEnumerable<TEncodedSolution> ISingleObjectiveProblemDefinition<TEncoding, TEncodedSolution>.GetNeighbors(TEncodedSolution individual, IRandom random) {
      return CompiledProblemDefinition.GetNeighbors(individual, random);
    }

    bool ISingleObjectiveProblemDefinition<TEncoding, TEncodedSolution>.IsBetter(double quality, double bestQuality) {
      return CompiledProblemDefinition.IsBetter(quality, bestQuality);
    }
  }
}
