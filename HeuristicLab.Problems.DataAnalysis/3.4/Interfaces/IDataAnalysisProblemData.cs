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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableType("3f50defe-1f79-4122-a96b-11ca90202210")]
  public interface IDataAnalysisProblemData : INamedItem {
    bool IsEmpty { get; }

    IDataset Dataset { get; }
    ICheckedItemList<StringValue> InputVariables { get; }
    IEnumerable<string> AllowedInputVariables { get; }

    double[,] AllowedInputsTrainingValues { get; }
    double[,] AllowedInputsTestValues { get; }

    IntRange TrainingPartition { get; }
    IntRange TestPartition { get; }

    IEnumerable<int> AllIndices { get; }
    IEnumerable<int> TrainingIndices { get; }
    IEnumerable<int> TestIndices { get; }

    IEnumerable<ITransformation> Transformations { get; }

    bool IsTrainingSample(int index);
    bool IsTestSample(int index);

    event EventHandler Changed;
  }
}
