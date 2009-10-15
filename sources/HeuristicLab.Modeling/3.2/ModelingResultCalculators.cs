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
using HeuristicLab.Core;

namespace HeuristicLab.Modeling {
  public abstract class ModelingResultCalculators {
    private enum DatasetPart { Training, Validation, Test };

    private static readonly Dictionary<ModelingResult, Func<double[,], double>> ClassificationModelingResults;
    private static readonly Dictionary<ModelingResult, Func<double[,], double>> RegressionModelingResults;
    private static readonly Dictionary<ModelingResult, Func<double[,], double>> TimeSeriesPrognosisModelingResults;
    private static readonly Dictionary<ModelingResult, IOperator> ClassificationModelingResultEvaluators;
    private static readonly Dictionary<ModelingResult, IOperator> RegressionModelingResultEvaluators;
    private static readonly Dictionary<ModelingResult, IOperator> TimeSeriesPrognosisModelingResultEvaluators;

    private static readonly Dictionary<Type, IEnumerable<ModelingResult>> regressionResults =
      new Dictionary<Type, IEnumerable<ModelingResult>>() {
        { typeof(SimpleMSEEvaluator), 
          new ModelingResult[] { 
            ModelingResult.TrainingMeanSquaredError, 
            ModelingResult.ValidationMeanSquaredError,
            ModelingResult.TestMeanSquaredError
          }},
        { typeof(SimpleNMSEEvaluator), 
          new ModelingResult[] {
            ModelingResult.TrainingNormalizedMeanSquaredError,
            ModelingResult.ValidationNormalizedMeanSquaredError,
            ModelingResult.TestNormalizedMeanSquaredError
          }
        },
        { typeof(SimpleR2Evaluator),
          new ModelingResult[] {
            ModelingResult.TrainingCoefficientOfDetermination,
            ModelingResult.ValidationCoefficientOfDetermination,
            ModelingResult.TestCoefficientOfDetermination
          }
        },
        { typeof(SimplePearsonCorrelationCoefficientEvaluator),
          new ModelingResult[] {
            ModelingResult.TrainingPearsonsCorrelationCoefficient,
            ModelingResult.ValidationPearsonCorrelationCoefficient,
            ModelingResult.TestPearsonCorrelationCoefficient
          }
        },
        { typeof(SimpleStableCorrelationCoefficientEvaluator),
          new ModelingResult[] {
            ModelingResult.TrainingStablePearsonsCorrelationCoefficient,
            ModelingResult.ValidationStablePearsonCorrelationCoefficient,
            ModelingResult.TestStablePearsonCorrelationCoefficient
          }
        },
        { typeof(SimpleSpearmansRankCorrelationCoefficientEvaluator),
          new ModelingResult[] {
            ModelingResult.TrainingSpearmansRankCorrelationCoefficient,
            ModelingResult.ValidationSpearmansRankCorrelationCoefficient,
            ModelingResult.TestSpearmansRankCorrelationCoefficient
          }
        },
        { typeof(SimpleVarianceAccountedForEvaluator),
          new ModelingResult[] {
            ModelingResult.TrainingVarianceAccountedFor,
            ModelingResult.ValidationVarianceAccountedFor,
            ModelingResult.TestVarianceAccountedFor
          }
        },
        { typeof(SimpleMeanAbsolutePercentageErrorEvaluator),
          new ModelingResult[] {
            ModelingResult.TrainingMeanAbsolutePercentageError,
            ModelingResult.ValidationMeanAbsolutePercentageError,
            ModelingResult.TestMeanAbsolutePercentageError
          }
        },
        { typeof(SimpleMeanAbsolutePercentageOfRangeErrorEvaluator),
          new ModelingResult[] {
            ModelingResult.TrainingMeanAbsolutePercentageOfRangeError,
            ModelingResult.ValidationMeanAbsolutePercentageOfRangeError,
            ModelingResult.TestMeanAbsolutePercentageOfRangeError
          }
        }
      };

    private static readonly Dictionary<Type, IEnumerable<ModelingResult>> timeSeriesResults =
      new Dictionary<Type, IEnumerable<ModelingResult>>() {
        { typeof(SimpleTheilInequalityCoefficientEvaluator),
          new ModelingResult[] {
            ModelingResult.TrainingTheilInequality,
            ModelingResult.ValidationTheilInequality,
            ModelingResult.TestTheilInequality
          }
        },
        { typeof(SimpleDirectionalSymmetryEvaluator),
          new ModelingResult[] {
            ModelingResult.TrainingDirectionalSymmetry,
            ModelingResult.ValidationDirectionalSymmetry,
            ModelingResult.TestDirectionalSymmetry
          }
        },
        { typeof(SimpleWeightedDirectionalSymmetryEvaluator),
          new ModelingResult[] {
            ModelingResult.TrainingWeightedDirectionalSymmetry,
            ModelingResult.ValidationWeightedDirectionalSymmetry,
            ModelingResult.TestWeightedDirectionalSymmetry
          }
        }
      };

    private static readonly Dictionary<Type, IEnumerable<ModelingResult>> classificationResults =
      new Dictionary<Type, IEnumerable<ModelingResult>>() {
        { typeof(SimpleAccuracyEvaluator),
          new ModelingResult[] {
            ModelingResult.TrainingAccuracy,
            ModelingResult.ValidationAccuracy,
            ModelingResult.TestAccuracy
          }
        }
      };


    static ModelingResultCalculators() {
      RegressionModelingResults = new Dictionary<ModelingResult, Func<double[,], double>>();
      ClassificationModelingResults = new Dictionary<ModelingResult, Func<double[,], double>>();
      TimeSeriesPrognosisModelingResults = new Dictionary<ModelingResult, Func<double[,], double>>();

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

      //Pearson Correlation Coefficient
      RegressionModelingResults[ModelingResult.TrainingPearsonsCorrelationCoefficient] = SimplePearsonCorrelationCoefficientEvaluator.Calculate;
      RegressionModelingResults[ModelingResult.ValidationPearsonCorrelationCoefficient] = SimplePearsonCorrelationCoefficientEvaluator.Calculate;
      RegressionModelingResults[ModelingResult.TestPearsonCorrelationCoefficient] = SimplePearsonCorrelationCoefficientEvaluator.Calculate;

      //Stable Pearson Correlation Coefficient
      RegressionModelingResults[ModelingResult.TrainingStablePearsonsCorrelationCoefficient] = SimpleStableCorrelationCoefficientEvaluator.Calculate;
      RegressionModelingResults[ModelingResult.ValidationStablePearsonCorrelationCoefficient] = SimpleStableCorrelationCoefficientEvaluator.Calculate;
      RegressionModelingResults[ModelingResult.TestStablePearsonCorrelationCoefficient] = SimpleStableCorrelationCoefficientEvaluator.Calculate;

      //Spearman's rank correlation coefficient
      RegressionModelingResults[ModelingResult.TrainingSpearmansRankCorrelationCoefficient] = SimpleSpearmansRankCorrelationCoefficientEvaluator.Calculate;
      RegressionModelingResults[ModelingResult.ValidationSpearmansRankCorrelationCoefficient] = SimpleSpearmansRankCorrelationCoefficientEvaluator.Calculate;
      RegressionModelingResults[ModelingResult.TestSpearmansRankCorrelationCoefficient] = SimpleSpearmansRankCorrelationCoefficientEvaluator.Calculate;

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

      #region result evaluators

      RegressionModelingResultEvaluators = new Dictionary<ModelingResult, IOperator>();
      foreach (Type evaluatorT in regressionResults.Keys) {
        foreach (ModelingResult r in regressionResults[evaluatorT]) {
          RegressionModelingResultEvaluators[r] = CreateEvaluator(evaluatorT, r);
        }
      }

      timeSeriesResults = CombineDictionaries(regressionResults, timeSeriesResults);
      TimeSeriesPrognosisModelingResultEvaluators = new Dictionary<ModelingResult, IOperator>();
      foreach (Type evaluatorT in timeSeriesResults.Keys) {
        foreach (ModelingResult r in timeSeriesResults[evaluatorT]) {
          TimeSeriesPrognosisModelingResultEvaluators[r] = CreateEvaluator(evaluatorT, r);
        }
      }

      classificationResults = CombineDictionaries(regressionResults, classificationResults);
      ClassificationModelingResultEvaluators = new Dictionary<ModelingResult, IOperator>();
      foreach (Type evaluatorT in classificationResults.Keys) {
        foreach (ModelingResult r in classificationResults[evaluatorT]) {
          ClassificationModelingResultEvaluators[r] = CreateEvaluator(evaluatorT, r);
        }
      }

      #endregion
    }

    public static Dictionary<ModelingResult, Func<double[,], double>> GetModelingResult(ModelType modelType) {
      switch (modelType) {
        case ModelType.Regression:
          return CombineDictionaries(RegressionModelingResults, new Dictionary<ModelingResult, Func<double[,], double>>());
        case ModelType.Classification:
          return CombineDictionaries(RegressionModelingResults, ClassificationModelingResults);
        case ModelType.TimeSeriesPrognosis:
          return CombineDictionaries(RegressionModelingResults, TimeSeriesPrognosisModelingResults);
        default:
          throw new ArgumentException("Modeling result mapping for ModelType " + modelType + " not defined.");
      }
    }

    public static Func<double[,], double> GetModelingResultCalculator(ModelingResult modelingResult) {
      if (RegressionModelingResults.ContainsKey(modelingResult))
        return RegressionModelingResults[modelingResult];
      else if (ClassificationModelingResults.ContainsKey(modelingResult))
        return ClassificationModelingResults[modelingResult];
      else if (TimeSeriesPrognosisModelingResults.ContainsKey(modelingResult))
        return TimeSeriesPrognosisModelingResults[modelingResult];
      else
        throw new ArgumentException("Calculator for modeling result " + modelingResult + " not defined.");
    }

    public static IOperator CreateModelingResultEvaluator(ModelingResult modelingResult) {
      IOperator opTemplate = null;
      if (RegressionModelingResultEvaluators.ContainsKey(modelingResult))
        opTemplate = RegressionModelingResultEvaluators[modelingResult];
      else if (ClassificationModelingResultEvaluators.ContainsKey(modelingResult))
        opTemplate = ClassificationModelingResultEvaluators[modelingResult];
      else if (TimeSeriesPrognosisModelingResultEvaluators.ContainsKey(modelingResult))
        opTemplate = TimeSeriesPrognosisModelingResultEvaluators[modelingResult];
      else
        throw new ArgumentException("Evaluator for modeling result " + modelingResult + " not defined.");
      return (IOperator)opTemplate.Clone();
    }

    private static IOperator CreateEvaluator(Type evaluatorType, ModelingResult result) {
      SimpleEvaluatorBase evaluator = (SimpleEvaluatorBase)Activator.CreateInstance(evaluatorType);
      evaluator.GetVariableInfo("Values").ActualName = GetDatasetPart(result) + "Values";
      evaluator.GetVariableInfo(evaluator.OutputVariableName).ActualName = result.ToString();
      return evaluator;
    }

    private static DatasetPart GetDatasetPart(ModelingResult result) {
      if (result.ToString().StartsWith("Training")) return DatasetPart.Training;
      else if (result.ToString().StartsWith("Validation")) return DatasetPart.Validation;
      else if (result.ToString().StartsWith("Test")) return DatasetPart.Test;
      else throw new ArgumentException("Can't determine dataset part of modeling result " + result + ".");
    }

    private static Dictionary<T1, T2> CombineDictionaries<T1, T2>(
      Dictionary<T1, T2> x,
      Dictionary<T1, T2> y) {
      Dictionary<T1, T2> result = new Dictionary<T1, T2>(x);
      return x.Union(y).ToDictionary<KeyValuePair<T1, T2>, T1, T2>(p => p.Key, p => p.Value);
    }
  }
}
