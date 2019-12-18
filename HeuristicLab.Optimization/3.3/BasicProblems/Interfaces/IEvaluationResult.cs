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

using HEAL.Attic;
using HeuristicLab.Core;

namespace HeuristicLab.Optimization {
  [StorableType("406EB24F-A59C-440C-8B83-49FC05F48855")]
  public interface IEvaluationResult : IItem {

    //TODO: make methods generic for get/set additional data
    void SetAdditionalData(string identifier, object o);
    object GetAdditionalData(string identifier);
  }

  public interface ISingleObjectiveEvaluationResult : IEvaluationResult {
    double Quality { get; }
  }

  public interface IMultiObjectiveEvaluationResult : IEvaluationResult {
    //TODO change to unmodifiable Type from double[]
    double[] Quality { get; }
  }
}
