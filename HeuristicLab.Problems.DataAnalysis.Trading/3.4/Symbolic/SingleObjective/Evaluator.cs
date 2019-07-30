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
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Trading.Symbolic {
  [StorableType("0EEE96AC-9D20-4154-BDA8-0A34C4D5658A")]
  public abstract class SingleObjectiveEvaluator : SymbolicDataAnalysisSingleObjectiveEvaluator<IProblemData>, ISingleObjectiveEvaluator {
    [StorableConstructor]
    protected SingleObjectiveEvaluator(StorableConstructorFlag _) : base(_) { }
    protected SingleObjectiveEvaluator(SingleObjectiveEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected SingleObjectiveEvaluator() : base() { }
  }
}
