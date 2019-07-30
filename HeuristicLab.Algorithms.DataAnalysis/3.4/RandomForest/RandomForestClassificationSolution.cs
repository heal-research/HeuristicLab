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
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Represents a random forest solution for a classification problem which can be visualized in the GUI.
  /// </summary>
  [Item("RandomForestClassificationSolution", "Represents a random forest solution for a classification problem which can be visualized in the GUI.")]
  [StorableType("11D108AD-931C-4504-BA87-0D5CEB2C95A9")]
  public sealed class RandomForestClassificationSolution : ClassificationSolution, IRandomForestClassificationSolution {

    public new IRandomForestModel Model {
      get { return (IRandomForestModel)base.Model; }
      set { base.Model = value; }
    }

    public int NumberOfTrees {
      get { return Model.NumberOfTrees; }
    }

    [StorableConstructor]
    private RandomForestClassificationSolution(StorableConstructorFlag _) : base(_) { }
    private RandomForestClassificationSolution(RandomForestClassificationSolution original, Cloner cloner)
      : base(original, cloner) {
    }
    public RandomForestClassificationSolution(IRandomForestModel randomForestModel, IClassificationProblemData problemData)
      : base(randomForestModel, problemData) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomForestClassificationSolution(this, cloner);
    }
  }
}
