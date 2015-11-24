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

namespace HeuristicLab.Problems.Programmable {
  [Item("Multi-objective Problem Definition Script", "Script that defines the parameter vector and evaluates the solution for a programmable problem.")]
  [StorableClass]
  public sealed class MultiObjectiveProblemDefinitionScript<TEncoding, TSolution> : ProblemDefinitionScript<TEncoding, TSolution>, IMultiObjectiveProblemDefinition<TEncoding, TSolution>, IStorableContent
    where TEncoding : class, IEncoding<TSolution>
    where TSolution : class, ISolution {
    public string Filename { get; set; }

    private new IMultiObjectiveProblemDefinition<TEncoding, TSolution> CompiledProblemDefinition {
      get { return (IMultiObjectiveProblemDefinition<TEncoding, TSolution>)base.CompiledProblemDefinition; }
    }

    [StorableConstructor]
    private MultiObjectiveProblemDefinitionScript(bool deserializing) : base(deserializing) { }
    private MultiObjectiveProblemDefinitionScript(MultiObjectiveProblemDefinitionScript<TEncoding, TSolution> original, Cloner cloner) : base(original, cloner) { }
    public MultiObjectiveProblemDefinitionScript(string codeTemplate) : base(codeTemplate) { }
    public MultiObjectiveProblemDefinitionScript() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveProblemDefinitionScript<TEncoding, TSolution>(this, cloner);
    }

    bool[] IMultiObjectiveProblemDefinition<TEncoding, TSolution>.Maximization {
      get { return CompiledProblemDefinition.Maximization; }
    }

    double[] IMultiObjectiveProblemDefinition<TEncoding, TSolution>.Evaluate(TSolution individual, IRandom random) {
      return CompiledProblemDefinition.Evaluate(individual, random);
    }

    void IMultiObjectiveProblemDefinition<TEncoding, TSolution>.Analyze(TSolution[] individuals, double[][] qualities, ResultCollection results, IRandom random) {
      CompiledProblemDefinition.Analyze(individuals, qualities, results, random);
    }
  }
}
