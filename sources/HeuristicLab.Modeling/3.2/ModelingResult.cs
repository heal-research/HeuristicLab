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


namespace HeuristicLab.Modeling {
  public enum ModelingResult {
    TrainingMeanSquaredError,
    ValidationMeanSquaredError,
    TestMeanSquaredError,

    TrainingNormalizedMeanSquaredError,
    ValidationNormalizedMeanSquaredError,
    TestNormalizedMeanSquaredError,

    TrainingMeanAbsolutePercentageError,
    ValidationMeanAbsolutePercentageError,
    TestMeanAbsolutePercentageError,

    TrainingMeanAbsolutePercentageOfRangeError,
    ValidationMeanAbsolutePercentageOfRangeError,
    TestMeanAbsolutePercentageOfRangeError,

    TrainingCoefficientOfDetermination,
    ValidationCoefficientOfDetermination,
    TestCoefficientOfDetermination,

    TrainingPearsonsCorrelationCoefficient,   //MK 10.12.09 should be removed as soon as actual ModelAnalyzer files are not needed anymore
    TrainingPearsonCorrelationCoefficient,
    ValidationPearsonCorrelationCoefficient,
    TestPearsonCorrelationCoefficient,

    TrainingStablePearsonsCorrelationCoefficient, //MK 10.12.09 should be removed as soon as actual ModelAnalyzer files are not needed anymore
    TrainingStablePearsonCorrelationCoefficient,
    ValidationStablePearsonCorrelationCoefficient,
    TestStablePearsonCorrelationCoefficient,

    TrainingSpearmansRankCorrelationCoefficient,
    ValidationSpearmansRankCorrelationCoefficient,
    TestSpearmansRankCorrelationCoefficient,

    TrainingVarianceAccountedFor,
    ValidationVarianceAccountedFor,
    TestVarianceAccountedFor,

    TrainingAccuracy,
    ValidationAccuracy,
    TestAccuracy,

    TrainingTheilInequality,
    ValidationTheilInequality,
    TestTheilInequality,

    TrainingDirectionalSymmetry,
    ValidationDirectionalSymmetry,
    TestDirectionalSymmetry,

    TrainingWeightedDirectionalSymmetry,
    ValidationWeightedDirectionalSymmetry,
    TestWeightedDirectionalSymmetry,

    VariableQualityImpact,
    VariableEvaluationImpact,
    VariableNodeImpact,
    RelativeFrequencyVariableImpact,
  }
}
