#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 * and the BEACON Center for the Study of Evolution in Action.
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

namespace HeuristicLab.Algorithms.DataAnalysis.RandomForest {

  /// <summary>
  /// Defines what part of the Model should be stored.
  /// QualityOnly - no solution will be created.
  /// SurrogateModel - only the parameters will be stored, the model is calculated during deserialization
  /// Model - the complete model will be stored (consider the amount of memory needed)
  /// </summary>
  [StorableType("3869899B-1848-4628-AF27-3B6FDE5840D6")]
  public enum ModelCreation {
    QualityOnly,
    SurrogateModel,
    Model
  }
}