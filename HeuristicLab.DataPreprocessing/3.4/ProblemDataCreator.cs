#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.DataPreprocessing {
  public class ProblemDataCreator {

    private readonly IPreprocessingContext context;

    private Dataset ExportedDataset {
      get { return exporteDataset ?? (exporteDataset = context.Data.ExportToDataset()); }
    }
    private Dataset exporteDataset;

    private IList<ITransformation> Transformations { get { return context.Data.Transformations; } }

    public ProblemDataCreator(IPreprocessingContext context) {
      this.context = context;
    }

    public IDataAnalysisProblemData CreateProblemData() {
      if (context.Data.Rows == 0 || context.Data.Columns == 0) return null;

      var oldProblemData = context.ProblemData;
      IDataAnalysisProblemData problemData;

      if (oldProblemData is RegressionProblemData) {
        problemData = CreateRegressionData((RegressionProblemData)oldProblemData);
      } else if (oldProblemData is ClassificationProblemData) {
        problemData = CreateClassificationData((ClassificationProblemData)oldProblemData);
      } else if (oldProblemData is ClusteringProblemData) {
        problemData = CreateClusteringData((ClusteringProblemData)oldProblemData);
      } else {
        throw new NotImplementedException("The type of the DataAnalysisProblemData is not supported.");
      }

      SetTrainingAndTestPartition(problemData);
      // set the input variables to the correct checked state
      var inputVariables = problemData.InputVariables.ToDictionary(x => x.Value, x => x);
      foreach (var variable in oldProblemData.InputVariables) {
        bool @checked = oldProblemData.InputVariables.ItemChecked(variable);
        problemData.InputVariables.SetItemCheckedState(inputVariables[variable.Value], @checked);
      }

      return problemData;
    }

    private IDataAnalysisProblemData CreateRegressionData(RegressionProblemData oldProblemData) {
      var targetVariable = oldProblemData.TargetVariable;
      // target variable must be double and must exist in the new dataset
      return new RegressionProblemData(ExportedDataset, GetDoubleInputVariables(targetVariable), targetVariable, Transformations);
    }

    private IDataAnalysisProblemData CreateClassificationData(ClassificationProblemData oldProblemData) {
      var targetVariable = oldProblemData.TargetVariable;
      // target variable must be double and must exist in the new dataset
      return new ClassificationProblemData(ExportedDataset, GetDoubleInputVariables(targetVariable), targetVariable, Transformations);
    }

    private IDataAnalysisProblemData CreateClusteringData(ClusteringProblemData oldProblemData) {
      return new ClusteringProblemData(ExportedDataset, GetDoubleInputVariables(String.Empty), Transformations);
    }

    private void SetTrainingAndTestPartition(IDataAnalysisProblemData problemData) {
      var ppData = context.Data;

      problemData.TrainingPartition.Start = ppData.TrainingPartition.Start;
      problemData.TrainingPartition.End = ppData.TrainingPartition.End;
      problemData.TestPartition.Start = ppData.TestPartition.Start;
      problemData.TestPartition.End = ppData.TestPartition.End;
    }

    private IEnumerable<string> GetDoubleInputVariables(string targetVariable) {
      var variableNames = new List<string>();
      for (int i = 0; i < context.Data.Columns; ++i) {
        var variableName = context.Data.GetVariableName(i);
        if (context.Data.VariableHasType<double>(i)
          && variableName != targetVariable
          && IsNotConstantInputVariable(context.Data.GetValues<double>(i))) {

          variableNames.Add(variableName);
        }
      }
      return variableNames;
    }

    private bool IsNotConstantInputVariable(IList<double> list) {
      return context.Data.TrainingPartition.End - context.Data.TrainingPartition.Start > 1 || list.Range() > 0;
    }
  }
}
