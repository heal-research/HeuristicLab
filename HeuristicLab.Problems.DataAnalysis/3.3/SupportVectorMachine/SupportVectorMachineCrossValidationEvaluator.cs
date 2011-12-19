#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using SVM;

namespace HeuristicLab.Problems.DataAnalysis.SupportVectorMachine {
  /// <summary>
  /// Represents an operator that performs SVM cross validation with the given parameters.
  /// </summary>
  [StorableClass]
  [Item("SupportVectorMachineCrossValidationEvaluator", "Represents an operator that performs SVM cross validation with the given parameters.")]
  public class SupportVectorMachineCrossValidationEvaluator : SingleSuccessorOperator, ISingleObjectiveEvaluator {
    private const string RandomParameterName = "Random";
    private const string DataAnalysisProblemDataParameterName = "DataAnalysisProblemData";
    private const string SvmTypeParameterName = "SvmType";
    private const string KernelTypeParameterName = "KernelType";
    private const string CostParameterName = "Cost";
    private const string NuParameterName = "Nu";
    private const string GammaParameterName = "Gamma";
    private const string EpsilonParameterName = "Epsilon";
    private const string SamplesStartParameterName = "SamplesStart";
    private const string SamplesEndParameterName = "SamplesEnd";
    private const string ActualSamplesParameterName = "ActualSamples";
    private const string NumberOfFoldsParameterName = "NumberOfFolds";
    private const string QualityParameterName = "Quality";

    #region parameter properties
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters[RandomParameterName]; }
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
    public IValueLookupParameter<DoubleValue> NuParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[NuParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> CostParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[CostParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> GammaParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[GammaParameterName]; }
    }
    public IValueLookupParameter<DoubleValue> EpsilonParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters[EpsilonParameterName]; }
    }
    public IValueLookupParameter<IntValue> SamplesStartParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[SamplesStartParameterName]; }
    }
    public IValueLookupParameter<IntValue> SamplesEndParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[SamplesEndParameterName]; }
    }
    public IValueLookupParameter<PercentValue> ActualSamplesParameter {
      get { return (IValueLookupParameter<PercentValue>)Parameters[ActualSamplesParameterName]; }
    }
    public IValueLookupParameter<IntValue> NumberOfFoldsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[NumberOfFoldsParameterName]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }
    #endregion
    #region properties
    public DataAnalysisProblemData DataAnalysisProblemData {
      get { return DataAnalysisProblemDataParameter.ActualValue; }
    }
    public StringValue SvmType {
      get { return SvmTypeParameter.ActualValue; }
    }
    public StringValue KernelType {
      get { return KernelTypeParameter.ActualValue; }
    }
    public DoubleValue Nu {
      get { return NuParameter.ActualValue; }
    }
    public DoubleValue Cost {
      get { return CostParameter.ActualValue; }
    }
    public DoubleValue Gamma {
      get { return GammaParameter.ActualValue; }
    }
    public DoubleValue Epsilon {
      get { return EpsilonParameter.ActualValue; }
    }
    public IntValue SamplesStart {
      get { return SamplesStartParameter.ActualValue; }
    }
    public IntValue SamplesEnd {
      get { return SamplesEndParameter.ActualValue; }
    }
    public IntValue NumberOfFolds {
      get { return NumberOfFoldsParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    protected SupportVectorMachineCrossValidationEvaluator(bool deserializing) : base(deserializing) { }

    protected SupportVectorMachineCrossValidationEvaluator(SupportVectorMachineCrossValidationEvaluator original,
      Cloner cloner)
      : base(original, cloner) { }
    public SupportVectorMachineCrossValidationEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>(RandomParameterName, "The random generator to use."));
      Parameters.Add(new ValueLookupParameter<DataAnalysisProblemData>(DataAnalysisProblemDataParameterName, "The data analysis problem data to use for training."));
      Parameters.Add(new ValueLookupParameter<StringValue>(SvmTypeParameterName, "The type of SVM to use."));
      Parameters.Add(new ValueLookupParameter<StringValue>(KernelTypeParameterName, "The kernel type to use for the SVM."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(NuParameterName, "The value of the nu parameter nu-SVC, one-class SVM and nu-SVR."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(CostParameterName, "The value of the C (cost) parameter of C-SVC, epsilon-SVR and nu-SVR."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(GammaParameterName, "The value of the gamma parameter in the kernel function."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(EpsilonParameterName, "The value of the epsilon parameter for epsilon-SVR."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesStartParameterName, "The first index of the data set partition the support vector machine should use for training."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesEndParameterName, "The last index of the data set partition the support vector machine should use for training."));
      Parameters.Add(new ValueLookupParameter<PercentValue>(ActualSamplesParameterName, "The percentage of the training set that should be acutally used for cross-validation (samples are picked randomly from the training set)."));
      Parameters.Add(new ValueLookupParameter<IntValue>(NumberOfFoldsParameterName, "The number of folds to use for cross-validation."));
      Parameters.Add(new LookupParameter<DoubleValue>(QualityParameterName, "The cross validation quality reached with the given parameters."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SupportVectorMachineCrossValidationEvaluator(this, cloner);
    }

    public override IOperation Apply() {
      double reductionRatio = 1.0; // TODO: make parameter
      if (ActualSamplesParameter.ActualValue != null)
        reductionRatio = ActualSamplesParameter.ActualValue.Value;
      IEnumerable<int> rows =
        Enumerable.Range(SamplesStart.Value, SamplesEnd.Value - SamplesStart.Value)
        .Where(i => i < DataAnalysisProblemData.TestSamplesStart.Value || DataAnalysisProblemData.TestSamplesEnd.Value <= i);

      // create a new DataAnalysisProblemData instance
      DataAnalysisProblemData reducedProblemData = (DataAnalysisProblemData)DataAnalysisProblemData.Clone();
      reducedProblemData.Dataset =
        CreateReducedDataset(RandomParameter.ActualValue, reducedProblemData.Dataset, rows, reductionRatio);
      reducedProblemData.TrainingSamplesStart.Value = 0;
      reducedProblemData.TrainingSamplesEnd.Value = reducedProblemData.Dataset.Rows;
      reducedProblemData.TestSamplesStart.Value = reducedProblemData.Dataset.Rows;
      reducedProblemData.TestSamplesEnd.Value = reducedProblemData.Dataset.Rows;
      reducedProblemData.ValidationPercentage.Value = 0;

      double quality = PerformCrossValidation(reducedProblemData,
                             SvmType.Value, KernelType.Value,
                             Cost.Value, Nu.Value, Gamma.Value, Epsilon.Value, NumberOfFolds.Value);

      QualityParameter.ActualValue = new DoubleValue(quality);
      return base.Apply();
    }

    private Dataset CreateReducedDataset(IRandom random, Dataset dataset, IEnumerable<int> rowIndices, double reductionRatio) {

      // must not make a fink:
      // => select n rows randomly from start..end
      // => sort the selected rows by index
      // => move rows to beginning of partition (start)

      // all possible rowIndexes from start..end
      int[] rowIndexArr = rowIndices.ToArray();
      int n = (int)Math.Max(1.0, rowIndexArr.Length * reductionRatio);

      // knuth shuffle
      for (int i = rowIndexArr.Length - 1; i > 0; i--) {
        int j = random.Next(0, i);
        // swap
        int tmp = rowIndexArr[i];
        rowIndexArr[i] = rowIndexArr[j];
        rowIndexArr[j] = tmp;
      }

      // take the first n indexes (selected n rowIndexes from start..end)
      // now order by index
      int[] orderedRandomIndexes =
        rowIndexArr.Take(n)
        .OrderBy(x => x)
        .ToArray();

      // now build a dataset containing only rows from orderedRandomIndexes 
      double[,] reducedData = new double[n, dataset.Columns];
      for (int i = 0; i < n; i++) {
        for (int column = 0; column < dataset.Columns; column++) {
          reducedData[i, column] = dataset[orderedRandomIndexes[i], column];
        }
      }
      return new Dataset(dataset.VariableNames, reducedData);
    }

    private static double PerformCrossValidation(
      DataAnalysisProblemData problemData,
      string svmType, string kernelType,
      double cost, double nu, double gamma, double epsilon,
      int nFolds) {
      return PerformCrossValidation(problemData, problemData.TrainingIndizes, svmType, kernelType, cost, nu, gamma, epsilon, nFolds);
    }

    public static double PerformCrossValidation(
      DataAnalysisProblemData problemData,
      IEnumerable<int> rowIndices,
      string svmType, string kernelType,
      double cost, double nu, double gamma, double epsilon,
      int nFolds) {
      int targetVariableIndex = problemData.Dataset.GetVariableIndex(problemData.TargetVariable.Value);

      //extract SVM parameters from scope and set them
      SVM.Parameter parameter = new SVM.Parameter();
      parameter.SvmType = (SVM.SvmType)Enum.Parse(typeof(SVM.SvmType), svmType, true);
      parameter.KernelType = (SVM.KernelType)Enum.Parse(typeof(SVM.KernelType), kernelType, true);
      parameter.C = cost;
      parameter.Nu = nu;
      parameter.Gamma = gamma;
      parameter.P = epsilon;
      parameter.CacheSize = 500;
      parameter.Probability = false;


      SVM.Problem problem = SupportVectorMachineUtil.CreateSvmProblem(problemData, rowIndices);
      SVM.RangeTransform rangeTransform = SVM.RangeTransform.Compute(problem);
      SVM.Problem scaledProblem = Scaling.Scale(rangeTransform, problem);

      return SVM.Training.PerformCrossValidation(scaledProblem, parameter, nFolds, false);
    }
  }
}
