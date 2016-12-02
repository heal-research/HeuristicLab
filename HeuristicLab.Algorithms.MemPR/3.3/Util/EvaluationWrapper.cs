#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Algorithms.MemPR.Interfaces;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.Algorithms.MemPR.Util {
  public sealed class EvaluationWrapper<TSolution>
      where TSolution : class, IItem {
    private readonly ISingleObjectiveProblemDefinition problem;
    private readonly ISingleObjectiveSolutionScope<TSolution> scope;
    private readonly SingleEncodingIndividual individual;

    public EvaluationWrapper(ISingleObjectiveProblemDefinition problem, ISingleObjectiveSolutionScope<TSolution> solution) {
      this.problem = problem;
      this.scope = (ISingleObjectiveSolutionScope<TSolution>)solution.Clone();
      this.individual = new SingleEncodingIndividual(problem.Encoding, this.scope);
    }

    public double Evaluate(TSolution b) {
      scope.Solution = b;
      return problem.Evaluate(individual, null);
    }

    public double Evaluate(TSolution b, IRandom random) {
      scope.Solution = b;
      return problem.Evaluate(individual, random);
    }
  }
}
