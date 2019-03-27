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

using HeuristicLab.Common;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  [StorableType("5AC365B5-2F64-4F10-8BD3-E98790E8F3C4")]
  public abstract class SymbolicClassificationMultiObjectiveEvaluator : SymbolicDataAnalysisMultiObjectiveEvaluator<IClassificationProblemData>, ISymbolicClassificationMultiObjectiveEvaluator {
    [StorableConstructor]
    protected SymbolicClassificationMultiObjectiveEvaluator(StorableConstructorFlag _) : base(_) { }
    protected SymbolicClassificationMultiObjectiveEvaluator(SymbolicClassificationMultiObjectiveEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected SymbolicClassificationMultiObjectiveEvaluator() : base() { }
  }
}
