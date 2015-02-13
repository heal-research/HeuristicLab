using System;
using System.Collections.Generic;
using System.Linq;

using HeuristicLab.Algorithms.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.Instances.DataAnalysis;
using HeuristicLab.Random;
using HeuristicLab.Scripting;

public class RFRegressionCrossValidationScript : HeuristicLab.Scripting.CSharpScriptBase {
  /* Maximum degree of parallelism (specifies whether or not the grid search should be parallelized) */
  const int maximumDegreeOfParallelism = 4;
  /* Number of crossvalidation folds: */
  const int numberOfFolds = 3;
  /* Specify whether the crossvalidation folds should be shuffled */
  const bool shuffleFolds = true;

  /* The tunable Random Forest parameters:
     - "n" (number of trees). In the random forests literature, this is referred to as the ntree parameter.
       Larger number of trees produce more stable models and covariate importance estimates, but require more memory and a longer run time.
       For small datasets, 50 trees may be sufficient. For larger datasets, 500 or more may be required. Please consult the random forests
       literature for extensive discussion of this parameter (e.g. Cutler et al., 2007; Strobl et al., 2007; Strobl et al., 2008).

     - "r" The ratio of the training set that will be used in the construction of individual trees (0<r<=1). Should be adjusted depending on
       the noise level in the dataset in the range from 0.66 (low noise) to 0.05 (high noise). This parameter should be adjusted to achieve
       good generalization error.

     - "m" The ratio of features that will be used in the construction of individual trees (0<m<=1)
  */
  static Dictionary<string, IEnumerable<double>> randomForestParameterRanges = new Dictionary<string, IEnumerable<double>> {
    { "N", ValueGenerator.GenerateSteps(5m, 10, 1).Select(x => Math.Pow(2,(double)x)) },
    { "R", ValueGenerator.GenerateSteps(0.05m, 0.66m, 0.05m).Select(x => (double)x) },
    { "M", ValueGenerator.GenerateSteps(0.1m, 1, 0.1m).Select(x => (double)x) }
  };

  private static RandomForestRegressionSolution GridSearchWithCrossvalidation(IRegressionProblemData problemData, out RFParameter bestParameters, int seed = 3141519) {
    double rmsError, outOfBagRmsError, avgRelError, outOfBagAvgRelError;
    bestParameters = RandomForestUtil.GridSearch(problemData, numberOfFolds, shuffleFolds, randomForestParameterRanges, seed, maximumDegreeOfParallelism);
    var model = RandomForestModel.CreateRegressionModel(problemData, problemData.TrainingIndices, bestParameters.N, bestParameters.R, bestParameters.M, seed, out rmsError, out outOfBagRmsError, out avgRelError, out outOfBagAvgRelError);
    return (RandomForestRegressionSolution)model.CreateRegressionSolution(problemData);
  }

  private static RandomForestRegressionSolution GridSearch(IRegressionProblemData problemData, out RFParameter bestParameters, int seed = 3141519) {
    double rmsError, outOfBagRmsError, avgRelError, outOfBagAvgRelError;
    var random = new MersenneTwister();
    bestParameters = RandomForestUtil.GridSearch(problemData, randomForestParameterRanges, seed, maximumDegreeOfParallelism);
    var model = RandomForestModel.CreateRegressionModel(problemData, problemData.TrainingIndices, bestParameters.N, bestParameters.R, bestParameters.M, seed,
                                                        out rmsError, out outOfBagRmsError, out avgRelError, out outOfBagAvgRelError);
    return (RandomForestRegressionSolution)model.CreateRegressionSolution(problemData);
  }

  public override void Main() {
    var variables = (Variables)vars;
    var item = variables.SingleOrDefault(x => x.Value is IRegressionProblem || x.Value is IRegressionProblemData);
    if (item.Equals(default(KeyValuePair<string, object>)))
      throw new ArgumentException("Could not find a suitable problem or problem data.");

    string name = item.Key;
    IRegressionProblemData problemData;
    if (item.Value is IRegressionProblem)
      problemData = ((IRegressionProblem)item.Value).ProblemData;
    else
      problemData = (IRegressionProblemData)item.Value;

    var bestParameters = new RFParameter();
    var bestSolution = GridSearch(problemData, out bestParameters);
    vars["bestSolution"] = bestSolution;
    vars["bestParameters"] = bestParameters;

    Console.WriteLine("R2 (training): " + bestSolution.TrainingRSquared + ", R2 (test): " + bestSolution.TestRSquared);
    Console.WriteLine("Model parameters: n = {0}, r = {1}, m = {2}", bestParameters.N, bestParameters.R, bestParameters.M);
  }
}
