﻿#region License Information
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

using System.Threading;
using Google.Protobuf;
using HEAL.Attic;
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [StorableType("04901da1-d785-4124-a989-2a9c6bd1a81c")]
  public interface IExternalEvaluationProblem : IProblem {

    EvaluationCache Cache { get; }
    CheckedItemCollection<IEvaluationServiceClient> Clients { get; }

    QualityMessage Evaluate(SolutionMessage solutionMessage);
    ExtensionRegistry GetQualityMessageExtensions();
  }
}
