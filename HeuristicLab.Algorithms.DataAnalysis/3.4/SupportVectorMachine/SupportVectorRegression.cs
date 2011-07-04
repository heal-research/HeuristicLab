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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Support vector machine regression data analysis algorithm.
  /// </summary>
  [Item("Support Vector Regression", "Support vector machine regression data analysis algorithm (wrapper for libSVM).")]
  [Creatable("Data Analysis")]
  [StorableClass]
  public sealed class SupportVectorRegression : FixedDataAnalysisAlgorithm<IRegressionProblem> {
    private const string SvmTypeParameterName = "SvmType";
    private const string KernelTypeParameterName = "KernelType";
    private const string CostParameterName = "Cost";
    private const string NuParameterName = "Nu";
    private const string GammaParameterName = "Gamma";
    private const string EpsilonParameterName = "Epsilon";

    #region parameter properties
    public IValueParameter<StringValue> SvmTypeParameter {
      get { return (IValueParameter<StringValue>)Parameters[SvmTypeParameterName]; }
    }
    public IValueParameter<StringValue> KernelTypeParameter {
      get { return (IValueParameter<StringValue>)Parameters[KernelTypeParameterName]; }
    }
    public IValueParameter<DoubleValue> NuParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[NuParameterName]; }
    }
    public IValueParameter<DoubleValue> CostParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[CostParameterName]; }
    }
    public IValueParameter<DoubleValue> GammaParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[GammaParameterName]; }
    }
    public IValueParameter<DoubleValue> EpsilonParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[EpsilonParameterName]; }
    }
    #endregion
    #region properties
    public StringValue SvmType {
      get { return SvmTypeParameter.Value; }
    }
    public StringValue KernelType {
      get { return KernelTypeParameter.Value; }
    }
    public DoubleValue Nu {
      get { return NuParameter.Value; }
    }
    public DoubleValue Cost {
      get { return CostParameter.Value; }
    }
    public DoubleValue Gamma {
      get { return GammaParameter.Value; }
    }
    public DoubleValue Epsilon {
      get { return EpsilonParameter.Value; }
    }
    #endregion
    [StorableConstructor]
    private SupportVectorRegression(bool deserializing) : base(deserializing) { }
    private SupportVectorRegression(SupportVectorRegression original, Cloner cloner)
      : base(original, cloner) {
    }
    public SupportVectorRegression()
      : base() {
      Problem = new RegressionProblem();

      List<StringValue> svrTypes = (from type in new List<string> { "NU_SVR", "EPSILON_SVR" }
                                    select new StringValue(type).AsReadOnly())
                                   .ToList();
      ItemSet<StringValue> svrTypeSet = new ItemSet<StringValue>(svrTypes);
      List<StringValue> kernelTypes = (from type in new List<string> { "LINEAR", "POLY", "SIGMOID", "RBF" }
                                       select new StringValue(type).AsReadOnly())
                                   .ToList();
      ItemSet<StringValue> kernelTypeSet = new ItemSet<StringValue>(kernelTypes);
      Parameters.Add(new ConstrainedValueParameter<StringValue>(SvmTypeParameterName, "The type of SVM to use.", svrTypeSet, svrTypes[0]));
      Parameters.Add(new ConstrainedValueParameter<StringValue>(KernelTypeParameterName, "The kernel type to use for the SVM.", kernelTypeSet, kernelTypes[3]));
      Parameters.Add(new ValueParameter<DoubleValue>(NuParameterName, "The value of the nu parameter of the nu-SVR.", new DoubleValue(0.5)));
      Parameters.Add(new ValueParameter<DoubleValue>(CostParameterName, "The value of the C (cost) parameter of epsilon-SVR and nu-SVR.", new DoubleValue(1.0)));
      Parameters.Add(new ValueParameter<DoubleValue>(GammaParameterName, "The value of the gamma parameter in the kernel function.", new DoubleValue(1.0)));
      Parameters.Add(new ValueParameter<DoubleValue>(EpsilonParameterName, "The value of the epsilon parameter for epsilon-SVR.", new DoubleValue(0.1)));
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SupportVectorRegression(this, cloner);
    }

    #region support vector regression
    protected override void Run() {
      IRegressionProblemData problemData = Problem.ProblemData;
      IEnumerable<string> selectedInputVariables = problemData.AllowedInputVariables;
      var solution = CreateSupportVectorRegressionSolution(problemData, selectedInputVariables, SvmType.Value, KernelType.Value, Cost.Value, Nu.Value, Gamma.Value, Epsilon.Value);

      Results.Add(new Result("Support vector regression solution", "The support vector regression solution.", solution));
    }

    public static SupportVectorRegressionSolution CreateSupportVectorRegressionSolution(IRegressionProblemData problemData, IEnumerable<string> allowedInputVariables,
      string svmType, string kernelType, double cost, double nu, double gamma, double epsilon) {
      Dataset dataset = problemData.Dataset;
      string targetVariable = problemData.TargetVariable;
      IEnumerable<int> rows = problemData.TrainingIndizes;

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


      SVM.Problem problem = SupportVectorMachineUtil.CreateSvmProblem(dataset, targetVariable, allowedInputVariables, rows);
      SVM.RangeTransform rangeTransform = SVM.RangeTransform.Compute(problem);
      SVM.Problem scaledProblem = SVM.Scaling.Scale(rangeTransform, problem);
      var model = new SupportVectorMachineModel(SVM.Training.Train(scaledProblem, parameter), rangeTransform, targetVariable, allowedInputVariables);
      return new SupportVectorRegressionSolution(model, (IRegressionProblemData)problemData.Clone());
    }
    #endregion
  }
}
