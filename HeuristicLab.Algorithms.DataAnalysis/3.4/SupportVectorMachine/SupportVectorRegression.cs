#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Fossil;
using HeuristicLab.Problems.DataAnalysis;
using LibSVM;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Support vector machine regression data analysis algorithm.
  /// </summary>
  [Item("Support Vector Regression (SVM)", "Support vector machine regression data analysis algorithm (wrapper for libSVM).")]
  [Creatable(CreatableAttribute.Categories.DataAnalysisRegression, Priority = 110)]
  [StorableType("645A21E5-EF07-46BF-AA04-A616165F0EF4")]
  public sealed class SupportVectorRegression : FixedDataAnalysisAlgorithm<IRegressionProblem> {
    private const string SvmTypeParameterName = "SvmType";
    private const string KernelTypeParameterName = "KernelType";
    private const string CostParameterName = "Cost";
    private const string NuParameterName = "Nu";
    private const string GammaParameterName = "Gamma";
    private const string EpsilonParameterName = "Epsilon";
    private const string DegreeParameterName = "Degree";
    private const string CreateSolutionParameterName = "CreateSolution";

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
    public IFixedValueParameter<BoolValue> CreateSolutionParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[CreateSolutionParameterName]; }
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
    public bool CreateSolution {
      get { return CreateSolutionParameter.Value.Value; }
      set { CreateSolutionParameter.Value.Value = value; }
    }
    #endregion
    [StorableConstructor]
    private SupportVectorRegression(StorableConstructorFlag _) : base(_) { }
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
      Parameters.Add(new FixedValueParameter<BoolValue>(CreateSolutionParameterName, "Flag that indicates if a solution should be produced at the end of the run", new BoolValue(true)));
      Parameters[CreateSolutionParameterName].Hidden = true;
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      #region backwards compatibility (change with 3.4)

      if (!Parameters.ContainsKey(DegreeParameterName)) {
        Parameters.Add(new ValueParameter<IntValue>(DegreeParameterName,
          "The degree parameter for the polynomial kernel function.", new IntValue(3)));
      }
      if (!Parameters.ContainsKey(CreateSolutionParameterName)) {
        Parameters.Add(new FixedValueParameter<BoolValue>(CreateSolutionParameterName, "Flag that indicates if a solution should be produced at the end of the run", new BoolValue(true)));
        Parameters[CreateSolutionParameterName].Hidden = true;
      }
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SupportVectorRegression(this, cloner);
    }

    #region support vector regression
    protected override void Run(CancellationToken cancellationToken) {
      IRegressionProblemData problemData = Problem.ProblemData;
      IEnumerable<string> selectedInputVariables = problemData.AllowedInputVariables;
      int nSv;
      ISupportVectorMachineModel model;
      Run(problemData, selectedInputVariables, SvmType.Value, KernelType.Value, Cost.Value, Nu.Value, Gamma.Value, Epsilon.Value, Degree.Value, out model, out nSv);

      if (CreateSolution) {
        var solution = new SupportVectorRegressionSolution((SupportVectorMachineModel)model, (IRegressionProblemData)problemData.Clone());
        Results.Add(new Result("Support vector regression solution", "The support vector regression solution.", solution));
      }

      Results.Add(new Result("Number of support vectors", "The number of support vectors of the SVR solution.", new IntValue(nSv)));


      {
        // calculate regression model metrics 
        var ds = problemData.Dataset;
        var trainRows = problemData.TrainingIndices;
        var testRows = problemData.TestIndices;
        var yTrain = ds.GetDoubleValues(problemData.TargetVariable, trainRows);
        var yTest = ds.GetDoubleValues(problemData.TargetVariable, testRows);
        var yPredTrain = model.GetEstimatedValues(ds, trainRows).ToArray();
        var yPredTest = model.GetEstimatedValues(ds, testRows).ToArray();

        OnlineCalculatorError error;
        var trainMse = OnlineMeanSquaredErrorCalculator.Calculate(yPredTrain, yTrain, out error);
        if (error != OnlineCalculatorError.None) trainMse = double.MaxValue;
        var testMse = OnlineMeanSquaredErrorCalculator.Calculate(yPredTest, yTest, out error);
        if (error != OnlineCalculatorError.None) testMse = double.MaxValue;

        Results.Add(new Result("Mean squared error (training)", "The mean of squared errors of the SVR solution on the training partition.", new DoubleValue(trainMse)));
        Results.Add(new Result("Mean squared error (test)", "The mean of squared errors of the SVR solution on the test partition.", new DoubleValue(testMse)));


        var trainMae = OnlineMeanAbsoluteErrorCalculator.Calculate(yPredTrain, yTrain, out error);
        if (error != OnlineCalculatorError.None) trainMae = double.MaxValue;
        var testMae = OnlineMeanAbsoluteErrorCalculator.Calculate(yPredTest, yTest, out error);
        if (error != OnlineCalculatorError.None) testMae = double.MaxValue;

        Results.Add(new Result("Mean absolute error (training)", "The mean of absolute errors of the SVR solution on the training partition.", new DoubleValue(trainMae)));
        Results.Add(new Result("Mean absolute error (test)", "The mean of absolute errors of the SVR solution on the test partition.", new DoubleValue(testMae)));


        var trainRelErr = OnlineMeanAbsolutePercentageErrorCalculator.Calculate(yPredTrain, yTrain, out error);
        if (error != OnlineCalculatorError.None) trainRelErr = double.MaxValue;
        var testRelErr = OnlineMeanAbsolutePercentageErrorCalculator.Calculate(yPredTest, yTest, out error);
        if (error != OnlineCalculatorError.None) testRelErr = double.MaxValue;

        Results.Add(new Result("Average relative error (training)", "The mean of relative errors of the SVR solution on the training partition.", new DoubleValue(trainRelErr)));
        Results.Add(new Result("Average relative error (test)", "The mean of relative errors of the SVR solution on the test partition.", new DoubleValue(testRelErr)));
      }
    }

    // BackwardsCompatibility3.4
    #region Backwards compatible code, remove with 3.5
    // for compatibility with old API
    public static SupportVectorRegressionSolution CreateSupportVectorRegressionSolution(
      IRegressionProblemData problemData, IEnumerable<string> allowedInputVariables,
      string svmType, string kernelType, double cost, double nu, double gamma, double epsilon, int degree,
      out double trainingR2, out double testR2, out int nSv) {
      ISupportVectorMachineModel model;
      Run(problemData, allowedInputVariables, svmType, kernelType, cost, nu, gamma, epsilon, degree, out model, out nSv);

      var solution = new SupportVectorRegressionSolution((SupportVectorMachineModel)model, (IRegressionProblemData)problemData.Clone());
      trainingR2 = solution.TrainingRSquared;
      testR2 = solution.TestRSquared;
      return solution;
    }
    #endregion

    public static void Run(IRegressionProblemData problemData, IEnumerable<string> allowedInputVariables,
      string svmType, string kernelType, double cost, double nu, double gamma, double epsilon, int degree,
      out ISupportVectorMachineModel model, out int nSv) {
      var dataset = problemData.Dataset;
      string targetVariable = problemData.TargetVariable;
      IEnumerable<int> rows = problemData.TrainingIndices;

      svm_parameter parameter = new svm_parameter {
        svm_type = GetSvmType(svmType),
        kernel_type = GetKernelType(kernelType),
        C = cost,
        nu = nu,
        gamma = gamma,
        p = epsilon,
        cache_size = 500,
        probability = 0,
        eps = 0.001,
        degree = degree,
        shrinking = 1,
        coef0 = 0
      };

      svm_problem problem = SupportVectorMachineUtil.CreateSvmProblem(dataset, targetVariable, allowedInputVariables, rows);
      RangeTransform rangeTransform = RangeTransform.Compute(problem);
      svm_problem scaledProblem = rangeTransform.Scale(problem);
      var svmModel = svm.svm_train(scaledProblem, parameter);
      nSv = svmModel.SV.Length;

      model = new SupportVectorMachineModel(svmModel, rangeTransform, targetVariable, allowedInputVariables);
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
