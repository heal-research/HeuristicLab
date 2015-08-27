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

using System;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  public abstract class SymbolicExpressionTreeProblem : SingleObjectiveBasicProblem<SymbolicExpressionTreeEncoding> {

    // persistence
    [StorableConstructor]
    protected SymbolicExpressionTreeProblem(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }


    // cloning
    protected SymbolicExpressionTreeProblem(SymbolicExpressionTreeProblem original, Cloner cloner)
      : base(original, cloner) {
    }

    protected SymbolicExpressionTreeProblem() : base() { }

    public virtual bool IsBetter(double quality, double bestQuality) {
      return (Maximization && quality > bestQuality || !Maximization && quality < bestQuality);
    }

    public abstract double Evaluate(ISymbolicExpressionTree tree, IRandom random);
    public sealed override double Evaluate(Individual individual, IRandom random) {
      return Evaluate(individual.SymbolicExpressionTree(), random);
    }

    public virtual void Analyze(ISymbolicExpressionTree[] trees, double[] qualities, ResultCollection results,
      IRandom random) {
      if (!results.ContainsKey("Best Solution Quality")) {
        results.Add(new Result("Best Solution Quality", typeof(DoubleValue)));
      }
      if (!results.ContainsKey("Best Solution")) {
        results.Add(new Result("Best Solution", typeof(ISymbolicExpressionTree)));
      }

      var bestQuality = Maximization ? qualities.Max() : qualities.Min();

      if (results["Best Solution Quality"].Value == null ||
          IsBetter(bestQuality, ((DoubleValue)results["Best Solution Quality"].Value).Value)) {
        var bestIdx = Array.IndexOf(qualities, bestQuality);
        var bestClone = (IItem)trees[bestIdx].Clone();
        results["Best Solution"].Value = bestClone;
        results["Best Solution Quality"].Value = new DoubleValue(bestQuality);
      }
    }

    public sealed override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      Analyze(individuals.Select(ind => ind.SymbolicExpressionTree()).ToArray(), qualities, results, random);
    }
  }
}
