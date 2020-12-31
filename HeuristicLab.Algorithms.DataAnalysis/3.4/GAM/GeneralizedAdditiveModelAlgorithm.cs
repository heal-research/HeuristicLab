#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [Item("Generalized Additive Model (GAM)", "Generalized additive model using uni-variate penalized regression splines as base learner.")]
  [StorableType("98A887E7-73DD-4602-BD6C-2F6B9E6FBBC5")]
  [Creatable(CreatableAttribute.Categories.DataAnalysisRegression, Priority = 600)]
  public sealed class GeneralizedAdditiveModelAlgorithm : FixedDataAnalysisAlgorithm<IRegressionProblem> {
    #region ParameterNames

    private const string IterationsParameterName = "Iterations";
    private const string LambdaParameterName = "Lambda";
    private const string SeedParameterName = "Seed";
    private const string SetSeedRandomlyParameterName = "SetSeedRandomly";
    private const string CreateSolutionParameterName = "CreateSolution";
    #endregion

    #region ParameterProperties

    public IFixedValueParameter<IntValue> IterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[IterationsParameterName]; }
    }

    public IFixedValueParameter<DoubleValue> LambdaParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[LambdaParameterName]; }
    }

    public IFixedValueParameter<IntValue> SeedParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[SeedParameterName]; }
    }

    public FixedValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (FixedValueParameter<BoolValue>)Parameters[SetSeedRandomlyParameterName]; }
    }

    public IFixedValueParameter<BoolValue> CreateSolutionParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[CreateSolutionParameterName]; }
    }

    #endregion

    #region Properties

    public int Iterations {
      get { return IterationsParameter.Value.Value; }
      set { IterationsParameter.Value.Value = value; }
    }

    public double Lambda {
      get { return LambdaParameter.Value.Value; }
      set { LambdaParameter.Value.Value = value; }
    }

    public int Seed {
      get { return SeedParameter.Value.Value; }
      set { SeedParameter.Value.Value = value; }
    }

    public bool SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value.Value; }
      set { SetSeedRandomlyParameter.Value.Value = value; }
    }

    public bool CreateSolution {
      get { return CreateSolutionParameter.Value.Value; }
      set { CreateSolutionParameter.Value.Value = value; }
    }

    #endregion

    [StorableConstructor]
    private GeneralizedAdditiveModelAlgorithm(StorableConstructorFlag deserializing)
      : base(deserializing) {
    }

    private GeneralizedAdditiveModelAlgorithm(GeneralizedAdditiveModelAlgorithm original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GeneralizedAdditiveModelAlgorithm(this, cloner);
    }

    public GeneralizedAdditiveModelAlgorithm() {
      Problem = new RegressionProblem(); // default problem

      Parameters.Add(new FixedValueParameter<IntValue>(IterationsParameterName,
        "Number of iterations. Try a large value and check convergence of the error over iterations. Usually, only a few iterations (e.g. 10) are needed for convergence.", new IntValue(10)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(LambdaParameterName,
        "The penalty parameter for the penalized regression splines. Set to a value between -8 (weak smoothing) and 8 (strong smooting). Usually, a value between -4 and 4 should be fine", new DoubleValue(3)));
      Parameters.Add(new FixedValueParameter<IntValue>(SeedParameterName,
        "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<BoolValue>(SetSeedRandomlyParameterName,
        "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<BoolValue>(CreateSolutionParameterName,
        "Flag that indicates if a solution should be produced at the end of the run", new BoolValue(true)));
      Parameters[CreateSolutionParameterName].Hidden = true;
    }

    protected override void Run(CancellationToken cancellationToken) {
      // Set up the algorithm
      if (SetSeedRandomly) Seed = new System.Random().Next();
      var rand = new MersenneTwister((uint)Seed);

      // calculates a GAM model using univariate non-linear functions 
      // using backfitting algorithm (see The Elements of Statistical Learning page 298)

      // init
      var problemData = Problem.ProblemData;
      var ds = problemData.Dataset;
      var trainRows = problemData.TrainingIndices;
      var testRows = problemData.TestIndices;
      var avgY = problemData.TargetVariableTrainingValues.Average();
      var inputVars = problemData.AllowedInputVariables.ToArray();

      int nTerms = inputVars.Length;

      #region init results
      // Set up the results display
      var iterations = new IntValue(0);
      Results.Add(new Result("Iterations", iterations));

      var table = new DataTable("Qualities");
      var rmseRow = new DataRow("RMSE (train)");
      var rmseRowTest = new DataRow("RMSE (test)");
      table.Rows.Add(rmseRow);
      table.Rows.Add(rmseRowTest);
      Results.Add(new Result("Qualities", table));
      var curRMSE = new DoubleValue();
      var curRMSETest = new DoubleValue();
      Results.Add(new Result("RMSE (train)", curRMSE));
      Results.Add(new Result("RMSE (test)", curRMSETest));

      // calculate table with residual contributions of each term
      var rssTable = new DoubleMatrix(nTerms, 1, new string[] { "RSS" }, inputVars);
      Results.Add(new Result("RSS Values", rssTable));
      #endregion

      // start with a set of constant models = 0
      IRegressionModel[] f = new IRegressionModel[nTerms];
      for (int i = 0; i < f.Length; i++) {
        f[i] = new ConstantModel(0.0, problemData.TargetVariable);
      }
      // init res which contains the current residual vector
      double[] res = problemData.TargetVariableTrainingValues.Select(yi => yi - avgY).ToArray();
      double[] resTest = problemData.TargetVariableTestValues.Select(yi => yi - avgY).ToArray();

      curRMSE.Value = res.StandardDeviation();
      curRMSETest.Value = resTest.StandardDeviation();
      rmseRow.Values.Add(res.StandardDeviation()); 
      rmseRowTest.Values.Add(resTest.StandardDeviation());


      double lambda = Lambda;
      var idx = Enumerable.Range(0, nTerms).ToArray();

      // Loop until iteration limit reached or canceled.
      for (int i = 0; i < Iterations && !cancellationToken.IsCancellationRequested; i++) {
        // shuffle order of terms in each iteration to remove bias on earlier terms
        idx.ShuffleInPlace(rand);
        foreach (var inputIdx in idx) {
          var inputVar = inputVars[inputIdx];
          // first remove the effect of the previous model for the inputIdx (by adding the output of the current model to the residual)
          AddInPlace(res, f[inputIdx].GetEstimatedValues(ds, trainRows));
          AddInPlace(resTest, f[inputIdx].GetEstimatedValues(ds, testRows));

          rssTable[inputIdx, 0] = res.Variance();
          f[inputIdx] = RegressSpline(problemData, inputVar, res, lambda);

          SubtractInPlace(res, f[inputIdx].GetEstimatedValues(ds, trainRows));
          SubtractInPlace(resTest, f[inputIdx].GetEstimatedValues(ds, testRows));
        }

        curRMSE.Value = res.StandardDeviation(); 
        curRMSETest.Value = resTest.StandardDeviation();
        rmseRow.Values.Add(curRMSE.Value);
        rmseRowTest.Values.Add(curRMSETest.Value);
        iterations.Value = i;
      }

      // produce solution 
      if (CreateSolution) {
        var model = new RegressionEnsembleModel(f.Concat(new[] { new ConstantModel(avgY, problemData.TargetVariable) }));
        model.AverageModelEstimates = false;
        var solution = model.CreateRegressionSolution((IRegressionProblemData)problemData.Clone());
        Results.Add(new Result("Ensemble solution", solution));
      }
    }

    private IRegressionModel RegressSpline(IRegressionProblemData problemData, string inputVar, double[] target, double lambda) {
      var x = problemData.Dataset.GetDoubleValues(inputVar, problemData.TrainingIndices).ToArray();
      var y = (double[])target.Clone();
      int info;
      alglib.spline1dinterpolant s;
      alglib.spline1dfitreport rep;
      int numKnots = (int)Math.Min(50, 3 * Math.Sqrt(x.Length)); // heuristic for number of knots  (Elements of Statistical Learning)

      alglib.spline1dfitpenalized(x, y, numKnots, lambda, out info, out s, out rep);

      return new Spline1dModel(s.innerobj, problemData.TargetVariable, inputVar);
    }


    private static void AddInPlace(double[] a, IEnumerable<double> enumerable) {
      int i = 0;
      foreach (var elem in enumerable) {
        a[i] += elem;
        i++;
      }
    }

    private static void SubtractInPlace(double[] a, IEnumerable<double> enumerable) {
      int i = 0;
      foreach (var elem in enumerable) {
        a[i] -= elem;
        i++;
      }
    }
  }
}
