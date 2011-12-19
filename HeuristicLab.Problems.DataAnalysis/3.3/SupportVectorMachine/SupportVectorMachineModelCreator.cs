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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using SVM;

namespace HeuristicLab.Problems.DataAnalysis.SupportVectorMachine {
  /// <summary>
  /// Represents an operator that creates a support vector machine model.
  /// </summary>
  [StorableClass]
  [Item("SupportVectorMachineModelCreator", "Represents an operator that creates a support vector machine model.")]
  public sealed class SupportVectorMachineModelCreator : SingleSuccessorOperator {
    private const string DataAnalysisProblemDataParameterName = "DataAnalysisProblemData";
    private const string SvmTypeParameterName = "SvmType";
    private const string KernelTypeParameterName = "KernelType";
    private const string CostParameterName = "Cost";
    private const string NuParameterName = "Nu";
    private const string GammaParameterName = "Gamma";
    private const string EpsilonParameterName = "Epsilon";
    private const string SamplesStartParameterName = "SamplesStart";
    private const string SamplesEndParameterName = "SamplesEnd";
    private const string ModelParameterName = "SupportVectorMachineModel";

    #region parameter properties
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
    public ILookupParameter<SupportVectorMachineModel> SupportVectorMachineModelParameter {
      get { return (ILookupParameter<SupportVectorMachineModel>)Parameters[ModelParameterName]; }
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
    #endregion

    [StorableConstructor]
    private SupportVectorMachineModelCreator(bool deserializing) : base(deserializing) { }
    private SupportVectorMachineModelCreator(SupportVectorMachineModelCreator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SupportVectorMachineModelCreator(this, cloner);
    }
    public SupportVectorMachineModelCreator()
      : base() {
      StringValue nuSvrType = new StringValue("NU_SVR").AsReadOnly();
      StringValue rbfKernelType = new StringValue("RBF").AsReadOnly();
      Parameters.Add(new ValueLookupParameter<DataAnalysisProblemData>(DataAnalysisProblemDataParameterName, "The data analysis problem data to use for training."));
      Parameters.Add(new ValueLookupParameter<StringValue>(SvmTypeParameterName, "The type of SVM to use.", nuSvrType));
      Parameters.Add(new ValueLookupParameter<StringValue>(KernelTypeParameterName, "The kernel type to use for the SVM.", rbfKernelType));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(NuParameterName, "The value of the nu parameter nu-SVC, one-class SVM and nu-SVR."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(CostParameterName, "The value of the C (cost) parameter of C-SVC, epsilon-SVR and nu-SVR."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(GammaParameterName, "The value of the gamma parameter in the kernel function."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(EpsilonParameterName, "The value of the epsilon parameter for epsilon-SVR."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesStartParameterName, "The first index of the data set partition the support vector machine should use for training."));
      Parameters.Add(new ValueLookupParameter<IntValue>(SamplesEndParameterName, "The last index of the data set partition the support vector machine should use for training."));
      Parameters.Add(new LookupParameter<SupportVectorMachineModel>(ModelParameterName, "The result model generated by the SVM."));
    }

    public override IOperation Apply() {
      int start = SamplesStart.Value;
      int end = SamplesEnd.Value;
      IEnumerable<int> rows =
        Enumerable.Range(start, end - start)
        .Where(i => i < DataAnalysisProblemData.TestSamplesStart.Value || DataAnalysisProblemData.TestSamplesEnd.Value <= i);

      SupportVectorMachineModel model = TrainModel(DataAnalysisProblemData,
                             rows,
                             SvmType.Value, KernelType.Value,
                             Cost.Value, Nu.Value, Gamma.Value, Epsilon.Value);
      SupportVectorMachineModelParameter.ActualValue = model;

      return base.Apply();
    }

    private static SupportVectorMachineModel TrainModel(
      DataAnalysisProblemData problemData,
      string svmType, string kernelType,
      double cost, double nu, double gamma, double epsilon) {
      return TrainModel(problemData, problemData.TrainingIndizes, svmType, kernelType, cost, nu, gamma, epsilon);
    }

    public static SupportVectorMachineModel TrainModel(
      DataAnalysisProblemData problemData,
      IEnumerable<int> trainingIndizes,
      string svmType, string kernelType,
      double cost, double nu, double gamma, double epsilon) {
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


      SVM.Problem problem = SupportVectorMachineUtil.CreateSvmProblem(problemData, trainingIndizes);
      SVM.RangeTransform rangeTransform = SVM.RangeTransform.Compute(problem);
      SVM.Problem scaledProblem = Scaling.Scale(rangeTransform, problem);
      var model = new SupportVectorMachineModel();
      model.Model = SVM.Training.Train(scaledProblem, parameter);
      model.RangeTransform = rangeTransform;

      return model;
    }
  }
}
