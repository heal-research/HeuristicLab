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
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("93696570-A410-4885-A210-7220771B6050")]
  [Item("Classification Problem", "A general classification problem.")]
  public class ClassificationProblem : DataAnalysisProblem<IClassificationProblemData>, IClassificationProblem, IStorableContent {
    public string Filename { get; set; }

    [StorableConstructor]
    protected ClassificationProblem(StorableConstructorFlag _) : base(_) { }
    protected ClassificationProblem(ClassificationProblem original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new ClassificationProblem(this, cloner); }

    public ClassificationProblem() : base(new ClassificationProblemData()) { }
  }
}
