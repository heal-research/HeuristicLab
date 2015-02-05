using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using HeuristicLab.Algorithms.DataAnalysis;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.Instances.DataAnalysis;
using HeuristicLab.Scripting;

using LibSVM;

public class SVMClassificationCrossValidationScript : HeuristicLab.Scripting.CSharpScriptBase {
  /* Maximum degree of parallelism (specifies whether or not the grid search should be parallelized) */
  const int maximumDegreeOfParallelism = 4;

  /* Number of crossvalidation folds: */
  const int numberOfFolds = 5;

  /* Specify whether the folds should be shuffled */
  const bool shuffleFolds = false;

  /* The tunable SVM parameters:
     - "C" (penalty factor) effects the trade-off between complexity and proportion of nonseparable samples and must be selected by the user. Can have any positive value.
     - "nu" is an upper bound on the fraction of margin errors and a lower bound of the fraction of support vectors relative to the total number of training examples.
     - "degree" represents the polynomial kernel degree
     - "eps" (epsilon) determines the level of accuracy of the approximated function. It controls the width of the epsilon-insensitive zone used to fit the training data.
       With optimal values of epsilon, the parameter C has negligible effect.
     - "degree" represents the degree of the polynomial kernel
     - "kernel_type" specifies the kernel to be used: linear, polynomial, radial basis or sigmoidal.
       Valid values: svm_parameter.LINEAR, svm_parameter.POLY, svm_parameter.RBF, svm_parameter.SIGMOID
     Comment or uncomment the parameter ranges below as needed.  */

  static Dictionary<string, IEnumerable<double>> svmParameterRanges = new Dictionary<string, IEnumerable<double>> {
        { "svm_type", new List<double> {svm_parameter.NU_SVC } },
        { "kernel_type", new List<double> { svm_parameter.RBF }},
        { "C", ValueGenerator.GenerateSteps(-1m, 10, 1).Select(x => Math.Pow(2,(double)x)) },
        { "gamma", ValueGenerator.GenerateSteps(-4m, -1, 1).Select(x => Math.Pow(2,(double)x)) },
//        { "eps", ValueGenerator.GenerateSteps(-8m, -1, 1).Select(x => Math.Pow(2, (double)x)) },
        { "nu" , ValueGenerator.GenerateSteps(-10m, 0, 1m).Select(x => Math.Pow(2, (double)x)) },
//        { "degree", ValueGenerator.GenerateSteps(1m, 4, 1).Select(x => (double)x) }
  };

  static Dictionary<int, string> svmTypes = new Dictionary<int, string> {
    { svm_parameter.NU_SVC, "NU_SVC" },
    { svm_parameter.C_SVC, "C_SVC" }
  };

  static Dictionary<int, string> kernelTypes = new Dictionary<int, string> {
    { svm_parameter.LINEAR, "LINEAR" },
    { svm_parameter.POLY, "POLY" },
    { svm_parameter.RBF, "RBF" },
    { svm_parameter.SIGMOID, "SIGMOID" }
  };

  private static SupportVectorClassificationSolution SvmGridSearch(IClassificationProblemData problemData, out svm_parameter bestParameters, out int nSv, out double cvMse) {
    bestParameters = SupportVectorMachineUtil.GridSearch(out cvMse, problemData, svmParameterRanges, numberOfFolds, shuffleFolds, maximumDegreeOfParallelism);
    double trainingError, testError;
    string svmType = svmTypes[bestParameters.svm_type];
    string kernelType = kernelTypes[bestParameters.kernel_type];
    var svm_solution = SupportVectorClassification.CreateSupportVectorClassificationSolution(problemData, problemData.AllowedInputVariables, svmType, kernelType,
                       bestParameters.C, bestParameters.nu, bestParameters.gamma, bestParameters.degree, out trainingError, out testError, out nSv);
    return svm_solution;
  }

  public override void Main() {
    var variables = (Variables)vars;
    var item = variables.SingleOrDefault(x => x.Value is IClassificationProblem || x.Value is IClassificationProblemData);
    if (item.Equals(default(KeyValuePair<string, object>)))
      throw new ArgumentException("Could not find a suitable problem or problem data.");

    string name = item.Key;
    IClassificationProblemData problemData;
    if (item.Value is IClassificationProblem)
      problemData = ((IClassificationProblem)item.Value).ProblemData;
    else
      problemData = (IClassificationProblemData)item.Value;

    int nSv; // number of support vectors
    double cvMse;
    svm_parameter bestParameters;
    var bestSolution = SvmGridSearch(problemData, out bestParameters, out nSv, out cvMse);

    vars["bestSolution"] = bestSolution;
    Console.WriteLine(name + " parameters: C = {0}, g = {1:0.000}, eps = {2:0.000}, nu = {3:0.000}, degree = {4}", bestParameters.C, bestParameters.gamma, bestParameters.eps, bestParameters.nu, bestParameters.degree);
    Console.WriteLine(name + " best solution accuracy (training): " + bestSolution.TrainingAccuracy + ", accuracy (test): " + bestSolution.TestAccuracy);

    var bestParametersCollection = new ParameterCollection();
    foreach (var p in svmParameterRanges.Keys) {
      var getter = GenerateGetter(p);
      bestParametersCollection.Add(new FixedValueParameter<DoubleValue>(p, new DoubleValue(getter(bestParameters))));
    }
    vars["bestParameters"] = bestParametersCollection;
  }

  private static Func<svm_parameter, double> GenerateGetter(string field) {
    var paramExpr = Expression.Parameter(typeof(svm_parameter));
    var getterExpr = Expression.Convert(Expression.Field(paramExpr, field), typeof(double)); // cast to double
    Func<svm_parameter, double> f = Expression.Lambda<Func<svm_parameter, double>>(getterExpr, paramExpr).Compile();
    return f;
  }
}

