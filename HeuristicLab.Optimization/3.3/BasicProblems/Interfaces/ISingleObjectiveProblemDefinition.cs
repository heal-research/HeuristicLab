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
using HeuristicLab.Core;

namespace HeuristicLab.Optimization {
  public interface ISingleObjectiveProblemDefinition {
    bool Maximization { get; }
    bool IsBetter(double quality, double bestQuality);
  }

  [StorableType("7ec7bf7e-aaa7-4681-828b-3401cf67e2b3")]
  public interface ISingleObjectiveProblemDefinition<TEncoding, TEncodedSolution> : ISingleObjectiveProblemDefinition, IProblemDefinition<TEncoding, TEncodedSolution>
    where TEncoding : class, IEncoding<TEncodedSolution>
    where TEncodedSolution : class, IEncodedSolution {
    double Evaluate(TEncodedSolution solution, IRandom random);
    void Analyze(TEncodedSolution[] solutions, double[] qualities, ResultCollection results, IRandom random);
    IEnumerable<TEncodedSolution> GetNeighbors(TEncodedSolution solution, IRandom random);
  }
}