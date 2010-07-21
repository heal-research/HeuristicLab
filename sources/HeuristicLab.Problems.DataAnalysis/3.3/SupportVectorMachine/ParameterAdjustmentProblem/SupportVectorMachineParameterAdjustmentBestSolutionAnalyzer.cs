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

using System;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Evaluators;

namespace HeuristicLab.Problems.DataAnalysis.SupportVectorMachine.ParameterAdjustmentProblem {
  [Item("SupportVectorMachineParameterAdjustmentBestSolutionAnalyzer", "Collects the parameters and the quality on training and test of the best solution for the SVM parameter adjustment problem.")]
  [StorableClass]
  public class SupportVectorMachineParameterAdjustmentBestSolutionAnalyzer : SingleSuccessorOperator, IAnalyzer {
    private const string ParameterVectorParameterName = "ParameterVector";
    private const string DataAnalysisProblemDataParameterName = "DataAnalysisProblemData";
    private const string SvmTypeParameterName = "SvmType";
    private const string KernelTypeParameterName = "KernelType";
    private const string QualityParameterName = "Quality";
    private const string BestSolutionParameterName = "BestSolution";
    private const string BestSolutionQualityParameterName = "BestSolutionQuality";
    private const string ResultsParameterName = "Results";
    private const string BestSolutionResultName = "Best solution (cross-validation)";
    private const string BestSolutionTrainingMse = "Best solution mean squared error (training)";
    private const string BestSolutionTestMse = "Best solution mean squared error (test)";
    private const string BestSolutionNu = "Best nu (cross-validation)";
    private const string BestSolutionCost = "Best cost (cross-validation)";
    private const string BestSolutionGamma = "Best gamma (cross-validation)";


    #region parameter properties
    public ILookupParameter<ItemArray<RealVector>> ParameterVectorParameter {
      get { return (ILookupParameter<ItemArray<RealVector>>)Parameters["ParameterVector"]; }
    }
    public IValueLookupParameter<DataAnalysisProblemData> DataAnalysisProblemDataParameter {
      get { return (IValueLookupParameter<DataAnalysisProblemData>)Parameters[DataAnalysisProblemDataParameterName]; }
    }
    public IValueLookupParameter<StringValue> SvmTypeParameter {
      get { return (IValueLookupParameter<StringValue>)Parameters[SvmTypeParameterName]; }
    }
    public IValueLookupParameter<StringValue> KernelTypeParameter {
      get { return (IValueLookupParameter<StringValue>)Parameters[KernelTypeParameterName]; }
    }
    public ILookupParameter<ItemArray<DoubleValue>> QualityParameter {
      get { return (ILookupParameter<ItemArray<DoubleValue>>)Parameters[QualityParameterName]; }
    }
    #endregion
    #region properties
    public DataAnalysisProblemData DataAnalysisProblemData {
      get { return DataAnalysisProblemDataParameter.ActualValue; }
    }
    public StringValue SvmType {
      get { return SvmTypeParameter.Value; }
    }
    public StringValue KernelType {
      get { return KernelTypeParameter.Value; }
    }
    public ILookupParameter<SupportVectorMachineModel> BestSolutionParameter {
      get { return (ILookupParameter<SupportVectorMachineModel>)Parameters[BestSolutionParameterName]; }
    }
    public ILookupParameter<DoubleValue> BestSolutionQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BestSolutionQualityParameterName]; }
    }
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }

    #endregion

    public SupportVectorMachineParameterAdjustmentBestSolutionAnalyzer()
      : base() {
      StringValue nuSvrType = new StringValue("NU_SVR").AsReadOnly();
      StringValue rbfKernelType = new StringValue("RBF").AsReadOnly();
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>(ParameterVectorParameterName, "The parameters for the SVM encoded as a real vector."));
      Parameters.Add(new ValueLookupParameter<DataAnalysisProblemData>(DataAnalysisProblemDataParameterName, "The data analysis problem data to use for training."));
      Parameters.Add(new ValueLookupParameter<StringValue>(SvmTypeParameterName, "The type of SVM to use.", nuSvrType));
      Parameters.Add(new ValueLookupParameter<StringValue>(KernelTypeParameterName, "The kernel type to use for the SVM.", rbfKernelType));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(QualityParameterName, "The cross validation quality reached with the given parameters."));
      Parameters.Add(new LookupParameter<SupportVectorMachineModel>(BestSolutionParameterName, "The best support vector solution."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestSolutionQualityParameterName, "The quality of the best support vector model."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection where the best support vector solution should be stored."));
    }

    public override IOperation Apply() {
      var points = ParameterVectorParameter.ActualValue;
      var qualities = QualityParameter.ActualValue;
      var bestPoint = points[0];
      var bestQuality = qualities[0].Value;
      for (int i = 1; i < points.Length; i++) {
        if (bestQuality > qualities[i].Value) {
          bestQuality = qualities[i].Value;
          bestPoint = points[i];
        }
      }
      ResultCollection results = ResultsParameter.ActualValue;
      double nu = bestPoint[0];
      double cost = Math.Pow(2, bestPoint[1]);
      double gamma = Math.Pow(2, bestPoint[2]);
      DataAnalysisProblemData problemData = DataAnalysisProblemData;

      SupportVectorMachineModel bestModel = BestSolutionParameter.ActualValue;
      if (bestModel == null) {
        bestModel = SupportVectorMachineModelCreator.TrainModel(DataAnalysisProblemData,
          DataAnalysisProblemData.TrainingSamplesStart.Value, DataAnalysisProblemData.TrainingSamplesEnd.Value,
          SvmType.Value, KernelType.Value, cost, nu, gamma, 0.0);
        BestSolutionParameter.ActualValue = bestModel;
        BestSolutionQualityParameter.ActualValue = new DoubleValue(bestQuality);
        results.Add(new Result(BestSolutionResultName, bestModel));
        #region calculate R2,MSE,Rel Error
        double[] trainingValues = problemData.Dataset.GetVariableValues(
          problemData.TargetVariable.Value,
          problemData.TrainingSamplesStart.Value,
          problemData.TrainingSamplesEnd.Value);
        double[] testValues = problemData.Dataset.GetVariableValues(
          problemData.TargetVariable.Value,
          problemData.TestSamplesStart.Value,
          problemData.TestSamplesEnd.Value);
        double[] estimatedTrainingValues = bestModel.GetEstimatedValues(problemData, problemData.TrainingSamplesStart.Value, problemData.TrainingSamplesEnd.Value)
          .ToArray();
        double[] estimatedTestValues = bestModel.GetEstimatedValues(problemData, problemData.TestSamplesStart.Value, problemData.TestSamplesEnd.Value)
          .ToArray();
        double trainingMse = SimpleMSEEvaluator.Calculate(trainingValues, estimatedTrainingValues);
        double testMse = SimpleMSEEvaluator.Calculate(testValues, estimatedTestValues);
        results.Add(new Result(BestSolutionTrainingMse, new DoubleValue(trainingMse)));
        results.Add(new Result(BestSolutionTestMse, new DoubleValue(testMse)));
        results.Add(new Result(BestSolutionNu, new DoubleValue(nu)));
        results.Add(new Result(BestSolutionCost, new DoubleValue(cost)));
        results.Add(new Result(BestSolutionGamma, new DoubleValue(gamma)));
        #endregion
      } else {
        if (BestSolutionQualityParameter.ActualValue.Value > bestQuality) {
          bestModel = SupportVectorMachineModelCreator.TrainModel(DataAnalysisProblemData,
            DataAnalysisProblemData.TrainingSamplesStart.Value, DataAnalysisProblemData.TrainingSamplesEnd.Value,
            SvmType.Value, KernelType.Value, cost, nu, gamma, 0.0);
          BestSolutionParameter.ActualValue = bestModel;
          BestSolutionQualityParameter.ActualValue = new DoubleValue(bestQuality);
          results[BestSolutionResultName].Value = bestModel;
          #region calculate R2,MSE,Rel Error
          double[] trainingValues = problemData.Dataset.GetVariableValues(
            problemData.TargetVariable.Value,
            problemData.TrainingSamplesStart.Value,
            problemData.TrainingSamplesEnd.Value);
          double[] testValues = problemData.Dataset.GetVariableValues(
            problemData.TargetVariable.Value,
            problemData.TestSamplesStart.Value,
            problemData.TestSamplesEnd.Value);
          double[] estimatedTrainingValues = bestModel.GetEstimatedValues(problemData, problemData.TrainingSamplesStart.Value, problemData.TrainingSamplesEnd.Value)
            .ToArray();
          double[] estimatedTestValues = bestModel.GetEstimatedValues(problemData, problemData.TestSamplesStart.Value, problemData.TestSamplesEnd.Value)
            .ToArray();
          double trainingMse = SimpleMSEEvaluator.Calculate(trainingValues, estimatedTrainingValues);
          double testMse = SimpleMSEEvaluator.Calculate(testValues, estimatedTestValues);
          results[BestSolutionTrainingMse].Value = new DoubleValue(trainingMse);
          results[BestSolutionTestMse].Value = new DoubleValue(testMse);
          results[BestSolutionNu].Value = new DoubleValue(nu);
          results[BestSolutionCost].Value = new DoubleValue(cost);
          results[BestSolutionGamma].Value = new DoubleValue(gamma);
          #endregion
        }
      }

      return base.Apply();
    }
  }
}
