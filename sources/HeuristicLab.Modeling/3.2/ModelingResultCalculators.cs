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

    public static readonly Dictionary<ModelingResult, Func<double[,], double>> Mapping;

    static ModelingResultCalculators() {
      Mapping = new Dictionary<ModelingResult, Func<double[,], double>>();

      //Mean squared errors
      Mapping[ModelingResult.TrainingMeanSquaredError] = SimpleMSEEvaluator.Calculate;
      Mapping[ModelingResult.ValidationMeanSquaredError] = SimpleMSEEvaluator.Calculate;
      Mapping[ModelingResult.TestMeanSquaredError] = SimpleMSEEvaluator.Calculate;

      //Normalized mean squared errors
      Mapping[ModelingResult.TrainingNormalizedMeanSquaredError] = SimpleNMSEEvaluator.Calculate;
      Mapping[ModelingResult.ValidationNormalizedMeanSquaredError] = SimpleNMSEEvaluator.Calculate;
      Mapping[ModelingResult.TestNormalizedMeanSquaredError] = SimpleNMSEEvaluator.Calculate;

      //Mean absolute percentage error
      Mapping[ModelingResult.TrainingMeanAbsolutePercentageError] = SimpleMeanAbsolutePercentageErrorEvaluator.Calculate;
      Mapping[ModelingResult.ValidationMeanAbsolutePercentageError] = SimpleMeanAbsolutePercentageErrorEvaluator.Calculate;
      Mapping[ModelingResult.TestMeanAbsolutePercentageError] = SimpleMeanAbsolutePercentageErrorEvaluator.Calculate;

      //Mean absolute percentage of range error
      Mapping[ModelingResult.TrainingMeanAbsolutePercentageOfRangeError] = SimpleMeanAbsolutePercentageOfRangeErrorEvaluator.Calculate;
      Mapping[ModelingResult.ValidationMeanAbsolutePercentageOfRangeError] = SimpleMeanAbsolutePercentageOfRangeErrorEvaluator.Calculate;
      Mapping[ModelingResult.TestMeanAbsolutePercentageOfRangeError] = SimpleMeanAbsolutePercentageOfRangeErrorEvaluator.Calculate;

      //Coefficient of determination
      Mapping[ModelingResult.TrainingCoefficientOfDetermination] = SimpleR2Evaluator.Calculate;
      Mapping[ModelingResult.ValidationCoefficientOfDetermination] = SimpleR2Evaluator.Calculate;
      Mapping[ModelingResult.TestCoefficientOfDetermination] = SimpleR2Evaluator.Calculate;

      //Variance accounted for
      Mapping[ModelingResult.TrainingVarianceAccountedFor] = SimpleVarianceAccountedForEvaluator.Calculate;
      Mapping[ModelingResult.ValidationVarianceAccountedFor] = SimpleVarianceAccountedForEvaluator.Calculate;
      Mapping[ModelingResult.TestVarianceAccountedFor] = SimpleVarianceAccountedForEvaluator.Calculate;

      //Accuracy
      Mapping[ModelingResult.TrainingAccuracy] = SimpleAccuracyEvaluator.Calculate;
      Mapping[ModelingResult.ValidationAccuracy] = SimpleAccuracyEvaluator.Calculate;
      Mapping[ModelingResult.TestAccuracy] = SimpleAccuracyEvaluator.Calculate;

      //Theil inequality
      Mapping[ModelingResult.TrainingTheilInequality] = SimpleTheilInequalityCoefficientEvaluator.Calculate;
      Mapping[ModelingResult.ValidationTheilInequality] = SimpleTheilInequalityCoefficientEvaluator.Calculate;
      Mapping[ModelingResult.TestTheilInequality] = SimpleTheilInequalityCoefficientEvaluator.Calculate;

      //Directional symmetry
      Mapping[ModelingResult.TrainingDirectionalSymmetry] = SimpleDirectionalSymmetryEvaluator.Calculate;
      Mapping[ModelingResult.ValidationDirectionalSymmetry] = SimpleDirectionalSymmetryEvaluator.Calculate;
      Mapping[ModelingResult.TestDirectionalSymmetry] = SimpleDirectionalSymmetryEvaluator.Calculate;

      //Weighted directional symmetry
      Mapping[ModelingResult.TrainingWeightedDirectionalSymmetry] = SimpleWeightedDirectionalSymmetryEvaluator.Calculate;
      Mapping[ModelingResult.ValidationWeightedDirectionalSymmetry] = SimpleWeightedDirectionalSymmetryEvaluator.Calculate;
      Mapping[ModelingResult.TestWeightedDirectionalSymmetry] = SimpleWeightedDirectionalSymmetryEvaluator.Calculate;
    }
  }
}
