#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using LibSVM;

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
    private const string DegreeParameterName = "Degree";

    #region parameter properties
    public IConstrainedValueParameter<StringValue> SvmTypeParameter {
      get { return (IConstrainedValueParameter<StringValue>)Parameters[SvmTypeParameterName]; }
    }
    public IConstrainedValueParameter<StringValue> KernelTypeParameter {
      get { return (IConstrainedValueParameter<StringValue>)Parameters[KernelTypeParameterName]; }
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
    public IValueParameter<IntValue> DegreeParameter {
      get { return (IValueParameter<IntValue>)Parameters[DegreeParameterName]; }
    }
    #endregion
    #region properties
    public StringValue SvmType {
      get { return SvmTypeParameter.Value; }
      set { SvmTypeParameter.Value = value; }
    }
    public StringValue KernelType {
      get { return KernelTypeParameter.Value; }
      set { KernelTypeParameter.Value = value; }
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
    public IntValue Degree {
      get { return DegreeParameter.Value; }
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
      Parameters.Add(new ValueParameter<IntValue>(DegreeParameterName, "The degree parameter for the polynomial kernel function.", new IntValue(3)));
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      #region backwards compatibility (change with 3.4)
      if (!Parameters.ContainsKey(DegreeParameterName))
        Parameters.Add(new ValueParameter<IntValue>(DegreeParameterName, "The degree parameter for the polynomial kernel function.", new IntValue(3)));
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SupportVectorRegression(this, cloner);
    }

    #region support vector regression
    protected override void Run() {
      IRegressionProblemData problemData = Problem.ProblemData;
      IEnumerable<string> selectedInputVariables = problemData.AllowedInputVariables;
      double trainR2, testR2;
      int nSv;
      var solution = CreateSupportVectorRegressionSolution(problemData, selectedInputVariables, SvmType.Value,
        KernelType.Value, Cost.Value, Nu.Value, Gamma.Value, Epsilon.Value, Degree.Value,
        out trainR2, out testR2, out nSv);

      Results.Add(new Result("Support vector regression solution", "The support vector regression solution.", solution));
      Results.Add(new Result("Training R²", "The Pearson's R² of the SVR solution on the training partition.", new DoubleValue(trainR2)));
      Results.Add(new Result("Test R²", "The Pearson's R² of the SVR solution on the test partition.", new DoubleValue(testR2)));
      Results.Add(new Result("Number of support vectors", "The number of support vectors of the SVR solution.", new IntValue(nSv)));
    }

    public static SupportVectorRegressionSolution CreateSupportVectorRegressionSolution(IRegressionProblemData problemData, IEnumerable<string> allowedInputVariables,
      string svmType, string kernelType, double cost, double nu, double gamma, double epsilon, int degree,
      out double trainingR2, out double testR2, out int nSv) {
      var dataset = problemData.Dataset;
      string targetVariable = problemData.TargetVariable;
      IEnumerable<int> rows = problemData.TrainingIndices;

      //extract SVM parameters from scope and set them
      svm_parameter parameter = new svm_parameter();
      parameter.svm_type = GetSvmType(svmType);
      parameter.kernel_type = GetKernelType(kernelType);
      parameter.C = cost;
      parameter.nu = nu;
      parameter.gamma = gamma;
      parameter.p = epsilon;
      parameter.cache_size = 500;
      parameter.probability = 0;
      parameter.eps = 0.001;
      parameter.degree = degree;
      parameter.shrinking = 1;
      parameter.coef0 = 0;



      svm_problem problem = SupportVectorMachineUtil.CreateSvmProblem(dataset, targetVariable, allowedInputVariables, rows);
      RangeTransform rangeTransform = RangeTransform.Compute(problem);
      svm_problem scaledProblem = rangeTransform.Scale(problem);
      var svmModel = svm.svm_train(scaledProblem, parameter);
      nSv = svmModel.SV.Length;
      var model = new SupportVectorMachineModel(svmModel, rangeTransform, targetVariable, allowedInputVariables);
      var solution = new SupportVectorRegressionSolution(model, (IRegressionProblemData)problemData.Clone());
      trainingR2 = solution.TrainingRSquared;
      testR2 = solution.TestRSquared;
      return solution;
    }

    private static int GetSvmType(string svmType) {
      if (svmType == "NU_SVR") return svm_parameter.NU_SVR;
      if (svmType == "EPSILON_SVR") return svm_parameter.EPSILON_SVR;
      throw new ArgumentException("Unknown SVM type");
    }

    private static int GetKernelType(string kernelType) {
      if (kernelType == "LINEAR") return svm_parameter.LINEAR;
      if (kernelType == "POLY") return svm_parameter.POLY;
      if (kernelType == "SIGMOID") return svm_parameter.SIGMOID;
      if (kernelType == "RBF") return svm_parameter.RBF;
      throw new ArgumentException("Unknown kernel type");
    }
    #endregion
  }
}
