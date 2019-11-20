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
  [StorableType("383A35EB-03C9-473D-A20B-EE34E00BC174")]
  public interface ISolutionContext : IItem {

    bool IsEvaluated { get; }
    IEncodedSolution EncodedSolution { get; }

    IEvaluationResult EvaluationResult { get; }

    void SetAdditionalData(string identifier, object o);
    object GetAdditionalData(string identifier);
  }

  public interface ISingleObjectiveSolutionContext : ISolutionContext {
    //TODO discuss with abeham hiding of base property
    new ISingleObjectiveEvaluationResult EvaluationResult { get; }
  }

  public interface IMultiObjectiveSolutionContext : ISolutionContext {
    new IMultiObjectiveEvaluationResult EvaluationResult { get; }
  }

}
