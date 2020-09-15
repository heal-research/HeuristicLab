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
using System.Threading;
using HEAL.Attic;
using HeuristicLab.Core;

namespace HeuristicLab.Optimization {
  [StorableType("80849d87-5cc6-4dc0-8c10-966ecb68d582")]
  public interface ISingleObjectiveProblemDefinition {
    bool Maximization { get; }
    bool IsBetter(double quality, double bestQuality);
  }

  [StorableType("7ec7bf7e-aaa7-4681-828b-3401cf67e2b3")]
  public interface ISingleObjectiveProblemDefinition<TEncoding, TEncodedSolution> : ISingleObjectiveProblemDefinition, IProblemDefinition<TEncoding, TEncodedSolution>
    where TEncoding : class, IEncoding
    where TEncodedSolution : class, IEncodedSolution {

    ISingleObjectiveEvaluationResult Evaluate(TEncodedSolution solution, IRandom random);
    ISingleObjectiveEvaluationResult Evaluate(TEncodedSolution solution, IRandom random, CancellationToken cancellationToken);

    void Evaluate(ISingleObjectiveSolutionContext<TEncodedSolution> solutionContext, IRandom random);
    void Evaluate(ISingleObjectiveSolutionContext<TEncodedSolution> solutionContext, IRandom random, CancellationToken cancellationToken);

    //TODO add cancellation token?
    void Analyze(ISingleObjectiveSolutionContext<TEncodedSolution>[] solutionContexts, IRandom random);

    //TODO remove neighbors from ProblemDefinition?
    IEnumerable<TEncodedSolution> GetNeighbors(TEncodedSolution solution, IRandom random);
    IEnumerable<ISingleObjectiveSolutionContext<TEncodedSolution>> GetNeighbors(ISingleObjectiveSolutionContext<TEncodedSolution> solutionContext, IRandom random);
  }
}