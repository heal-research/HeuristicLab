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

using System;
using System.Collections.Generic;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Optimization {
  [StorableType("e16ad337-8c18-4c29-a893-e83f671e804c")]
  /// <summary>
  /// Interface to represent an optimization problem.
  /// </summary>
  public interface IProblem : IResultsProducingItem, IStorableContent {
    event EventHandler Reset;
  }

  //TODO Intermediate class for compatibility 
  //TODO move members to generic IProblem after every problem used the new architecture
  //TODO ABE: We can maybe use it as non-generic interface that exports IEncoding Encoding { get; }
  //TODO ABE: and which is explicitly implemented in some base class
  [StorableType("1dbe48d6-c008-4e40-86ad-c222450a3187")]
  public interface IEncodedProblem : IProblem {
    IEnumerable<IItem> Operators { get; }

    IEnumerable<IParameterizedItem> ExecutionContextItems { get; }
    event EventHandler OperatorsChanged;
  }

  [StorableType("1b4af8b9-bdf5-4ffd-86e6-35b481bfbf45")]
  public interface IProblem<TEncoding, TEncodedSolution> : IHeuristicOptimizationProblem
    where TEncoding : class, IEncoding
    where TEncodedSolution : class, IEncodedSolution {
  }
}