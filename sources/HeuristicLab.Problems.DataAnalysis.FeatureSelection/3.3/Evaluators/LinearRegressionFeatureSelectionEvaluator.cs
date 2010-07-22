#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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


using HeuristicLab.Optimization;
using HeuristicLab.Core;
using HeuristicLab.Common;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Data;
using HeuristicLab.Problems.DataAnalysis.Regression.LinearRegression;
using System.Linq;
using System.Collections.Generic;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Evaluators;
namespace HeuristicLab.Problems.DataAnalysis.FeatureSelection {
  public class LinearRegressionFeatureSelectionEvaluator : SingleSuccessorOperator, IFeatureSelectionEvaluator {
    #region parameter properties
    public ILookupParameter<DataAnalysisProblemData> DataAnalysisProblemDataParameter {
      get { return (ILookupParameter<DataAnalysisProblemData>)Parameters["DataAnalysisProblemData"]; }
    }

    public ILookupParameter<BinaryVector> SolutionParameter {
      get { return (ILookupParameter<BinaryVector>)Parameters["FeatureArray"]; }
    }
    public ILookupParameter<DoubleArray> QualitiesParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["Qualities"]; }
    }

    #endregion
    #region properties
    public DataAnalysisProblemData DataAnalysisProblemData {
      get { return DataAnalysisProblemDataParameter.ActualValue; }
    }
    public BinaryVector FeatureArray {
      get { return SolutionParameter.ActualValue; }
    }
    #endregion

    public LinearRegressionFeatureSelectionEvaluator()
      : base() {
        Parameters.Add(new LookupParameter<DataAnalysisProblemData>("DataAnalysisProblemData", "The data for the data analysis problem."));
      Parameters.Add(new LookupParameter<BinaryVector>("FeatureArray", "The binary array of features to use for linear regression."));
      Parameters.Add(new LookupParameter<DoubleArray>("Qualities", "The qualities of the linear regression solution (MSE, size)."));
    }

    public override IOperation Apply() {
      var dataset = DataAnalysisProblemData.Dataset;
      string targetVariable = DataAnalysisProblemData.TargetVariable.Value;

      int start = DataAnalysisProblemData.TrainingSamplesStart.Value;
      int end = DataAnalysisProblemData.TrainingSamplesEnd.Value;
      List<string> allowedInputVariables = new List<string>();
      int c = 0;
      foreach (var indexedItem in DataAnalysisProblemData.InputVariables.CheckedItems) {
        if (FeatureArray[c]) {
          allowedInputVariables.Add(indexedItem.Value.Value);
        }
        c++;
      }
      int featureCount;
      double mse;
      if (allowedInputVariables.Count > 0) {
        double rmsError, cvRmsError;
        var tree = LinearRegressionSolutionCreator.CreateSymbolicExpressionTree(dataset, targetVariable, allowedInputVariables, start, end, out rmsError, out cvRmsError);
        featureCount = allowedInputVariables.Count;
        mse = cvRmsError;
      } else {
        featureCount = 0;
        // when zero features are selected the linear regression should produce a constant (the mean)
        // the mse is then the variance of the target variable values
        mse = dataset.GetEnumeratedVariableValues(targetVariable, start, end).Variance();
      }
      DoubleArray qualities = new DoubleArray(2);
      qualities[0] = featureCount;
      qualities[1] = mse;

      QualitiesParameter.ActualValue = qualities;
      return base.Apply();
    }
  }
}
