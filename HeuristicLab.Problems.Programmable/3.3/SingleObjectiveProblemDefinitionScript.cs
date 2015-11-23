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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Programmable {
  [Item("Single-objective Problem Definition Script", "Script that defines the parameter vector and evaluates the solution for a programmable problem.")]
  [StorableClass]
  public class SingleObjectiveProblemDefinitionScript<TEncoding, TSolution> : ProblemDefinitionScript<TEncoding, TSolution>, ISingleObjectiveProblemDefinition<TEncoding, TSolution>, IStorableContent
    where TEncoding : class, IEncoding<TSolution>
    where TSolution : class, ISolution {
    public string Filename { get; set; }

    protected new ISingleObjectiveProblemDefinition<TEncoding, TSolution> CompiledProblemDefinition {
      get { return (ISingleObjectiveProblemDefinition<TEncoding, TSolution>)base.CompiledProblemDefinition; }
    }

    [StorableConstructor]
    protected SingleObjectiveProblemDefinitionScript(bool deserializing) : base(deserializing) { }
    protected SingleObjectiveProblemDefinitionScript(SingleObjectiveProblemDefinitionScript<TEncoding, TSolution> original, Cloner cloner) : base(original, cloner) { }
    public SingleObjectiveProblemDefinitionScript(string codeTemplate) : base(codeTemplate) { }
    public SingleObjectiveProblemDefinitionScript() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveProblemDefinitionScript<TEncoding, TSolution>(this, cloner);
    }

    bool ISingleObjectiveProblemDefinition<TEncoding, TSolution>.Maximization {
      get { return CompiledProblemDefinition.Maximization; }
    }

    double ISingleObjectiveProblemDefinition<TEncoding, TSolution>.Evaluate(TSolution individual, IRandom random) {
      return CompiledProblemDefinition.Evaluate(individual, random);
    }

    void ISingleObjectiveProblemDefinition<TEncoding, TSolution>.Analyze(TSolution[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      CompiledProblemDefinition.Analyze(individuals, qualities, results, random);
    }
    IEnumerable<TSolution> ISingleObjectiveProblemDefinition<TEncoding, TSolution>.GetNeighbors(TSolution individual, IRandom random) {
      return CompiledProblemDefinition.GetNeighbors(individual, random);
    }
  }
}
