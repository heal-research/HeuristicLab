#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;

namespace HeuristicLab.Modeling {
  public abstract class ModelingResultCalculators {
    private static readonly Dictionary<ModelingResult, Func<double[,], double>> ClassificationModelingResults;
    private static readonly Dictionary<ModelingResult, Func<double[,], double>> RegressionModelingResults;
    private static readonly Dictionary<ModelingResult, Func<double[,], double>> TimeSeriesPrognosisModelingResults;

    static ModelingResultCalculators() {
      RegressionModelingResults = new Dictionary<ModelingResult, Func<double[,], double>>();

      //Mean squared errors
      RegressionModelingResults[ModelingResult.TrainingMeanSquaredError] = SimpleMSEEvaluator.Calculate;
      RegressionModelingResults[ModelingResult.ValidationMeanSquaredError] = SimpleMSEEvaluator.Calculate;
      RegressionModelingResults[ModelingResult.TestMeanSquaredError] = SimpleMSEEvaluator.Calculate;

      //Normalized mean squared errors
      RegressionModelingResults[ModelingResult.TrainingNormalizedMeanSquaredError] = SimpleNMSEEvaluator.Calculate;
      RegressionModelingResults[ModelingResult.ValidationNormalizedMeanSquaredError] = SimpleNMSEEvaluator.Calculate;
      RegressionModelingResults[ModelingResult.TestNormalizedMeanSquaredError] = SimpleNMSEEvaluator.Calculate;

      //Mean absolute percentage error
      RegressionModelingResults[ModelingResult.TrainingMeanAbsolutePercentageError] = SimpleMeanAbsolutePercentageErrorEvaluator.Calculate;
      RegressionModelingResults[ModelingResult.ValidationMeanAbsolutePercentageError] = SimpleMeanAbsolutePercentageErrorEvaluator.Calculate;
      RegressionModelingResults[ModelingResult.TestMeanAbsolutePercentageError] = SimpleMeanAbsolutePercentageErrorEvaluator.Calculate;

      //Mean absolute percentage of range error
      RegressionModelingResults[ModelingResult.TrainingMeanAbsolutePercentageOfRangeError] = SimpleMeanAbsolutePercentageOfRangeErrorEvaluator.Calculate;
      RegressionModelingResults[ModelingResult.ValidationMeanAbsolutePercentageOfRangeError] = SimpleMeanAbsolutePercentageOfRangeErrorEvaluator.Calculate;
      RegressionModelingResults[ModelingResult.TestMeanAbsolutePercentageOfRangeError] = SimpleMeanAbsolutePercentageOfRangeErrorEvaluator.Calculate;

      //Coefficient of determination
      RegressionModelingResults[ModelingResult.TrainingCoefficientOfDetermination] = SimpleR2Evaluator.Calculate;
      RegressionModelingResults[ModelingResult.ValidationCoefficientOfDetermination] = SimpleR2Evaluator.Calculate;
      RegressionModelingResults[ModelingResult.TestCoefficientOfDetermination] = SimpleR2Evaluator.Calculate;

      //Variance accounted for
      RegressionModelingResults[ModelingResult.TrainingVarianceAccountedFor] = SimpleVarianceAccountedForEvaluator.Calculate;
      RegressionModelingResults[ModelingResult.ValidationVarianceAccountedFor] = SimpleVarianceAccountedForEvaluator.Calculate;
      RegressionModelingResults[ModelingResult.TestVarianceAccountedFor] = SimpleVarianceAccountedForEvaluator.Calculate;

      //Accuracy
      ClassificationModelingResults[ModelingResult.TrainingAccuracy] = SimpleAccuracyEvaluator.Calculate;
      ClassificationModelingResults[ModelingResult.ValidationAccuracy] = SimpleAccuracyEvaluator.Calculate;
      ClassificationModelingResults[ModelingResult.TestAccuracy] = SimpleAccuracyEvaluator.Calculate;

      //Theil inequality
      TimeSeriesPrognosisModelingResults[ModelingResult.TrainingTheilInequality] = SimpleTheilInequalityCoefficientEvaluator.Calculate;
      TimeSeriesPrognosisModelingResults[ModelingResult.ValidationTheilInequality] = SimpleTheilInequalityCoefficientEvaluator.Calculate;
      TimeSeriesPrognosisModelingResults[ModelingResult.TestTheilInequality] = SimpleTheilInequalityCoefficientEvaluator.Calculate;

      //Directional symmetry
      TimeSeriesPrognosisModelingResults[ModelingResult.TrainingDirectionalSymmetry] = SimpleDirectionalSymmetryEvaluator.Calculate;
      TimeSeriesPrognosisModelingResults[ModelingResult.ValidationDirectionalSymmetry] = SimpleDirectionalSymmetryEvaluator.Calculate;
      TimeSeriesPrognosisModelingResults[ModelingResult.TestDirectionalSymmetry] = SimpleDirectionalSymmetryEvaluator.Calculate;

      //Weighted directional symmetry
      TimeSeriesPrognosisModelingResults[ModelingResult.TrainingWeightedDirectionalSymmetry] = SimpleWeightedDirectionalSymmetryEvaluator.Calculate;
      TimeSeriesPrognosisModelingResults[ModelingResult.ValidationWeightedDirectionalSymmetry] = SimpleWeightedDirectionalSymmetryEvaluator.Calculate;
      TimeSeriesPrognosisModelingResults[ModelingResult.TestWeightedDirectionalSymmetry] = SimpleWeightedDirectionalSymmetryEvaluator.Calculate;
    }

    public static Dictionary<ModelingResult, Func<double[,], double>> GetModelingResult(ModelType modelType) {
      IEnumerable<KeyValuePair<ModelingResult,Func<double[,],double>>> ret = new Dictionary<ModelingResult,Func<double[,],double>>();
      switch (modelType) {
        case ModelType.Regression:
          ret = ret.Union( RegressionModelingResults);
          break;
        case ModelType.Classification:
          ret = ret.Union(RegressionModelingResults);
          ret = ret.Union(ClassificationModelingResults);
          break;
        case ModelType.TimeSeriesPrognosis:
          ret = ret.Union(RegressionModelingResults);
          ret = ret.Union(TimeSeriesPrognosisModelingResults);
          break;
        default:
          throw new ArgumentException("Modeling result mapping for ModelType " + modelType + " not defined.");          
      }
      return ret.ToDictionary<KeyValuePair<ModelingResult, Func<double[,], double>>, ModelingResult, Func<double[,], double>>(x => x.Key, x => x.Value);
    }

    public static Func<double[,], double> GetModelingResultCalculator(ModelingResult modelingResult) {
      if (RegressionModelingResults.ContainsKey(modelingResult))
        return RegressionModelingResults[modelingResult];
      else if (ClassificationModelingResults.ContainsKey(modelingResult))
        return ClassificationModelingResults[modelingResult];
      else if (TimeSeriesPrognosisModelingResults.ContainsKey(modelingResult))
        return TimeSeriesPrognosisModelingResults[modelingResult];
      else
        throw new ArgumentException("Calculator for modeling reuslt " + modelingResult + " not defined.");
    }
  }
}
