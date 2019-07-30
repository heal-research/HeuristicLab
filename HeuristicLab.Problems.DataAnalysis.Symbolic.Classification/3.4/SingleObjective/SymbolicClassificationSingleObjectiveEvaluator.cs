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

using HeuristicLab.Common;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  [StorableType("48C7BDD3-9EED-4D61-AF72-A895556052A7")]
  public abstract class SymbolicClassificationSingleObjectiveEvaluator : SymbolicDataAnalysisSingleObjectiveEvaluator<IClassificationProblemData>, ISymbolicClassificationSingleObjectiveEvaluator {
    [StorableConstructor]
    protected SymbolicClassificationSingleObjectiveEvaluator(StorableConstructorFlag _) : base(_) { }
    protected SymbolicClassificationSingleObjectiveEvaluator(SymbolicClassificationSingleObjectiveEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected SymbolicClassificationSingleObjectiveEvaluator() : base() { }
  }
}
