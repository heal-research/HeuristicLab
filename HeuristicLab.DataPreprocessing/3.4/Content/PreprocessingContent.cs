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
using HeuristicLab.Core;
using HEAL.Fossil;

namespace HeuristicLab.DataPreprocessing {
  [Item("PreprocessingContent", "")]
  [StorableType("16A5E913-08C6-4286-AD2F-4598ACB60F74")]
  public abstract class PreprocessingContent : Item {

    public IFilteredPreprocessingData PreprocessingData { get; private set; }

    #region Constructor, Cloning & Persistence
    protected PreprocessingContent(IFilteredPreprocessingData preprocessingData) {
      PreprocessingData = preprocessingData;
    }

    protected PreprocessingContent(PreprocessingContent original, Cloner cloner)
      : base(original, cloner) {
      PreprocessingData = cloner.Clone(original.PreprocessingData);
    }

    [StorableConstructor]
    protected PreprocessingContent(StorableConstructorFlag _) : base(_) { }
    #endregion
  }
}