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
  /// Support vector machine classification data analysis algorithm.
  /// </summary>
  [Item("Support Vector Classification (SVM)", "Support vector machine classification data analysis algorithm (wrapper for libSVM).")]
  [Creatable(CreatableAttribute.Categories.DataAnalysisClassification, Priority = 110)]
  [StorableClass]
  public sealed class SupportVectorClassification : FixedDataAnalysisAlgorithm<IClassificationProblem> {
    private const string SvmTypeParameterName = "SvmType";
    private const string KernelTypeParameterName = "KernelType";
    private const string CostParameterName = "Cost";
    private const string NuParameterName = "Nu";
    private const string GammaParameterName = "Gamma";
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
    public IntValue Degree {
      get { return DegreeParameter.Value; }
    }
    public bool CreateSolution {
      get { return CreateSolutionParameter.Value.Value; }
      set { CreateSolutionParameter.Value.Value = value; }
    }
    #endregion
    [StorableConstructor]
    private SupportVectorClassification(bool deserializing) : base(deserializing) { }
    private SupportVectorClassification(SupportVectorClassification original, Cloner cloner)
      : base(original, cloner) {
    }
    public SupportVectorClassification()
      : base() {
      Problem = new ClassificationProblem();

      List<StringValue> svrTypes = (from type in new List<string> { "NU_SVC", "C_SVC" }
                                    select new StringValue(type).AsReadOnly())
                                   .ToList();
      ItemSet<StringValue> svrTypeSet = new ItemSet<StringValue>(svrTypes);
      List<StringValue> kernelTypes = (from type in new List<string> { "LINEAR", "POLY", "SIGMOID", "RBF" }
                                       select new StringValue(type).AsReadOnly())
                                   .ToList();
      ItemSet<StringValue> kernelTypeSet = new ItemSet<StringValue>(kernelTypes);
      Parameters.Add(new ConstrainedValueParameter<StringValue>(SvmTypeParameterName, "The type of SVM to use.", svrTypeSet, svrTypes[0]));
      Parameters.Add(new ConstrainedValueParameter<StringValue>(KernelTypeParameterName, "The kernel type to use for the SVM.", kernelTypeSet, kernelTypes[3]));
      Parameters.Add(new ValueParameter<DoubleValue>(NuParameterName, "The value of the nu parameter nu-SVC.", new DoubleValue(0.5)));
      Parameters.Add(new ValueParameter<DoubleValue>(CostParameterName, "The value of the C (cost) parameter of C-SVC.", new DoubleValue(1.0)));
      Parameters.Add(new ValueParameter<DoubleValue>(GammaParameterName, "The value of the gamma parameter in the kernel function.", new DoubleValue(1.0)));
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
        Parameters.Add(new FixedValueParameter<BoolValue>(CreateSolutionParameterName,
          "Flag that indicates if a solution should be produced at the end of the run", new BoolValue(true)));
        Parameters[CreateSolutionParameterName].Hidden = true;
      }
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SupportVectorClassification(this, cloner);
    }

    #region support vector classification
    protected override void Run() {
      IClassificationProblemData problemData = Problem.ProblemData;
      IEnumerable<string> selectedInputVariables = problemData.AllowedInputVariables;
      int nSv;
      ISupportVectorMachineModel model;

      Run(problemData, selectedInputVariables, GetSvmType(SvmType.Value), GetKernelType(KernelType.Value), Cost.Value, Nu.Value, Gamma.Value, Degree.Value, out model, out nSv);

      if (CreateSolution) {
        var solution = new SupportVectorClassificationSolution((SupportVectorMachineModel)model, (IClassificationProblemData)problemData.Clone());
        Results.Add(new Result("Support vector classification solution", "The support vector classification solution.",
          solution));
      }

      {
        // calculate classification metrics
        // calculate regression model metrics 
        var ds = problemData.Dataset;
        var trainRows = problemData.TrainingIndices;
        var testRows = problemData.TestIndices;
        var yTrain = ds.GetDoubleValues(problemData.TargetVariable, trainRows);
        var yTest = ds.GetDoubleValues(problemData.TargetVariable, testRows);
        var yPredTrain = model.GetEstimatedClassValues(ds, trainRows);
        var yPredTest = model.GetEstimatedClassValues(ds, testRows);

        OnlineCalculatorError error;
        var trainAccuracy = OnlineAccuracyCalculator.Calculate(yPredTrain, yTrain, out error);
        if (error != OnlineCalculatorError.None) trainAccuracy = double.MaxValue;
        var testAccuracy = OnlineAccuracyCalculator.Calculate(yPredTest, yTest, out error);
        if (error != OnlineCalculatorError.None) testAccuracy = double.MaxValue;

        Results.Add(new Result("Accuracy (training)", "The mean of squared errors of the SVR solution on the training partition.", new DoubleValue(trainAccuracy)));
        Results.Add(new Result("Accuracy (test)", "The mean of squared errors of the SVR solution on the test partition.", new DoubleValue(testAccuracy)));

        Results.Add(new Result("Number of support vectors", "The number of support vectors of the SVR solution.",
          new IntValue(nSv)));
      }
    }

    public static SupportVectorClassificationSolution CreateSupportVectorClassificationSolution(IClassificationProblemData problemData, IEnumerable<string> allowedInputVariables,
      string svmType, string kernelType, double cost, double nu, double gamma, int degree, out double trainingAccuracy, out double testAccuracy, out int nSv) {
      return CreateSupportVectorClassificationSolution(problemData, allowedInputVariables, GetSvmType(svmType), GetKernelType(kernelType), cost, nu, gamma, degree,
        out trainingAccuracy, out testAccuracy, out nSv);
    }

    // BackwardsCompatibility3.4
    #region Backwards compatible code, remove with 3.5
    public static SupportVectorClassificationSolution CreateSupportVectorClassificationSolution(IClassificationProblemData problemData, IEnumerable<string> allowedInputVariables,
      int svmType, int kernelType, double cost, double nu, double gamma, int degree, out double trainingAccuracy, out double testAccuracy, out int nSv) {

      ISupportVectorMachineModel model;
      Run(problemData, allowedInputVariables, svmType, kernelType, cost, nu, gamma, degree, out model, out nSv);
      var solution = new SupportVectorClassificationSolution((SupportVectorMachineModel)model, (IClassificationProblemData)problemData.Clone());

      trainingAccuracy = solution.TrainingAccuracy;
      testAccuracy = solution.TestAccuracy;

      return solution;
    }

    #endregion

    public static void Run(IClassificationProblemData problemData, IEnumerable<string> allowedInputVariables,
      int svmType, int kernelType, double cost, double nu, double gamma, int degree,
      out ISupportVectorMachineModel model, out int nSv) {
      var dataset = problemData.Dataset;
      string targetVariable = problemData.TargetVariable;
      IEnumerable<int> rows = problemData.TrainingIndices;

      svm_parameter parameter = new svm_parameter {
        svm_type = svmType,
        kernel_type = kernelType,
        C = cost,
        nu = nu,
        gamma = gamma,
        cache_size = 500,
        probability = 0,
        eps = 0.001,
        degree = degree,
        shrinking = 1,
        coef0 = 0
      };

      var weightLabels = new List<int>();
      var weights = new List<double>();
      foreach (double c in problemData.ClassValues) {
        double wSum = 0.0;
        foreach (double otherClass in problemData.ClassValues) {
          if (!c.IsAlmost(otherClass)) {
            wSum += problemData.GetClassificationPenalty(c, otherClass);
          }
        }
        weightLabels.Add((int)c);
        weights.Add(wSum);
      }
      parameter.weight_label = weightLabels.ToArray();
      parameter.weight = weights.ToArray();

      svm_problem problem = SupportVectorMachineUtil.CreateSvmProblem(dataset, targetVariable, allowedInputVariables, rows);
      RangeTransform rangeTransform = RangeTransform.Compute(problem);
      svm_problem scaledProblem = rangeTransform.Scale(problem);
      var svmModel = svm.svm_train(scaledProblem, parameter);
      nSv = svmModel.SV.Length;

      model = new SupportVectorMachineModel(svmModel, rangeTransform, targetVariable, allowedInputVariables, problemData.ClassValues);
    }

    private static int GetSvmType(string svmType) {
      if (svmType == "NU_SVC") return svm_parameter.NU_SVC;
      if (svmType == "C_SVC") return svm_parameter.C_SVC;
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
