#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 * and the BEACON Center for the Study of Evolution in Action.
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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.DataAnalysis.MctsSymbolicRegression {
  [Item("Gradient Boosting Machine Regression (GBM)",
    "Gradient boosting for any regression base learner (e.g. MCTS symbolic regression)")]
  [StorableClass]
  [Creatable(CreatableAttribute.Categories.DataAnalysisRegression, Priority = 350)]
  public class GradientBoostingRegressionAlgorithm : BasicAlgorithm {
    public override Type ProblemType {
      get { return typeof(IRegressionProblem); }
    }

    public new IRegressionProblem Problem {
      get { return (IRegressionProblem)base.Problem; }
      set { base.Problem = value; }
    }

    #region ParameterNames

    private const string IterationsParameterName = "Iterations";
    private const string NuParameterName = "Nu";
    private const string MParameterName = "M";
    private const string RParameterName = "R";
    private const string RegressionAlgorithmParameterName = "RegressionAlgorithm";
    private const string SeedParameterName = "Seed";
    private const string SetSeedRandomlyParameterName = "SetSeedRandomly";
    private const string CreateSolutionParameterName = "CreateSolution";
    private const string RegressionAlgorithmSolutionResultParameterName = "RegressionAlgorithmResult";

    #endregion

    #region ParameterProperties

    public IFixedValueParameter<IntValue> IterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[IterationsParameterName]; }
    }

    public IFixedValueParameter<DoubleValue> NuParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[NuParameterName]; }
    }

    public IFixedValueParameter<DoubleValue> RParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[RParameterName]; }
    }

    public IFixedValueParameter<DoubleValue> MParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[MParameterName]; }
    }

    // regression algorithms are currently: DataAnalysisAlgorithms, BasicAlgorithms and engine algorithms with no common interface
    public IConstrainedValueParameter<IAlgorithm> RegressionAlgorithmParameter {
      get { return (IConstrainedValueParameter<IAlgorithm>)Parameters[RegressionAlgorithmParameterName]; }
    }

    public IFixedValueParameter<StringValue> RegressionAlgorithmSolutionResultParameter {
      get { return (IFixedValueParameter<StringValue>)Parameters[RegressionAlgorithmSolutionResultParameterName]; }
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

    public int Seed {
      get { return SeedParameter.Value.Value; }
      set { SeedParameter.Value.Value = value; }
    }

    public bool SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value.Value; }
      set { SetSeedRandomlyParameter.Value.Value = value; }
    }

    public double Nu {
      get { return NuParameter.Value.Value; }
      set { NuParameter.Value.Value = value; }
    }

    public double R {
      get { return RParameter.Value.Value; }
      set { RParameter.Value.Value = value; }
    }

    public double M {
      get { return MParameter.Value.Value; }
      set { MParameter.Value.Value = value; }
    }

    public bool CreateSolution {
      get { return CreateSolutionParameter.Value.Value; }
      set { CreateSolutionParameter.Value.Value = value; }
    }

    public IAlgorithm RegressionAlgorithm {
      get { return RegressionAlgorithmParameter.Value; }
    }

    public string RegressionAlgorithmResult {
      get { return RegressionAlgorithmSolutionResultParameter.Value.Value; }
      set { RegressionAlgorithmSolutionResultParameter.Value.Value = value; }
    }

    #endregion

    [StorableConstructor]
    protected GradientBoostingRegressionAlgorithm(bool deserializing)
      : base(deserializing) {
    }

    protected GradientBoostingRegressionAlgorithm(GradientBoostingRegressionAlgorithm original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GradientBoostingRegressionAlgorithm(this, cloner);
    }

    public GradientBoostingRegressionAlgorithm() {
      Problem = new RegressionProblem(); // default problem
      var mctsSymbReg = new MctsSymbolicRegressionAlgorithm();
      // var sgp = CreateSGP();
      var regressionAlgs = new ItemSet<IAlgorithm>(new IAlgorithm[] {
        new LinearRegression(), new RandomForestRegression(), new NearestNeighbourRegression(),
        // sgp, 
        mctsSymbReg
      });
      foreach (var alg in regressionAlgs) alg.Prepare();


      Parameters.Add(new FixedValueParameter<IntValue>(IterationsParameterName,
        "Number of iterations", new IntValue(100)));
      Parameters.Add(new FixedValueParameter<IntValue>(SeedParameterName,
        "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<BoolValue>(SetSeedRandomlyParameterName,
        "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(NuParameterName,
        "The learning rate nu when updating predictions in GBM (0 < nu <= 1)", new DoubleValue(0.5)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(RParameterName,
        "The fraction of rows that are sampled randomly for the base learner in each iteration (0 < r <= 1)",
        new DoubleValue(1)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(MParameterName,
        "The fraction of variables that are sampled randomly for the base learner in each iteration (0 < m <= 1)",
        new DoubleValue(0.5)));
      Parameters.Add(new ConstrainedValueParameter<IAlgorithm>(RegressionAlgorithmParameterName,
        "The regression algorithm to use as a base learner", regressionAlgs, mctsSymbReg));
      Parameters.Add(new FixedValueParameter<StringValue>(RegressionAlgorithmSolutionResultParameterName,
        "The name of the solution produced by the regression algorithm", new StringValue("Solution")));
      Parameters[RegressionAlgorithmSolutionResultParameterName].Hidden = true;
      Parameters.Add(new FixedValueParameter<BoolValue>(CreateSolutionParameterName,
        "Flag that indicates if a solution should be produced at the end of the run", new BoolValue(true)));
      Parameters[CreateSolutionParameterName].Hidden = true;
    }

    protected override void Run(CancellationToken cancellationToken) {
      // Set up the algorithm
      if (SetSeedRandomly) Seed = new System.Random().Next();
      var rand = new MersenneTwister((uint)Seed);

      // Set up the results display
      var iterations = new IntValue(0);
      Results.Add(new Result("Iterations", iterations));

      var table = new DataTable("Qualities");
      table.Rows.Add(new DataRow("Loss (train)"));
      table.Rows.Add(new DataRow("Loss (test)"));
      Results.Add(new Result("Qualities", table));
      var curLoss = new DoubleValue();
      var curTestLoss = new DoubleValue();
      Results.Add(new Result("Loss (train)", curLoss));
      Results.Add(new Result("Loss (test)", curTestLoss));
      var runCollection = new RunCollection();
      Results.Add(new Result("Runs", runCollection));

      // init
      var problemData = Problem.ProblemData;
      var targetVarName = Problem.ProblemData.TargetVariable;
      var modifiableDataset = new ModifiableDataset(
        problemData.Dataset.VariableNames,
        problemData.Dataset.VariableNames.Select(v => problemData.Dataset.GetDoubleValues(v).ToList()));

      var trainingRows = problemData.TrainingIndices;
      var testRows = problemData.TestIndices;
      var yPred = new double[trainingRows.Count()];
      var yPredTest = new double[testRows.Count()];
      var y = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, problemData.TrainingIndices).ToArray();
      var curY = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, problemData.TrainingIndices).ToArray();

      var yTest = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, problemData.TestIndices).ToArray();
      var curYTest = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, problemData.TestIndices).ToArray();
      var nu = Nu;
      var mVars = (int)Math.Ceiling(M * problemData.AllowedInputVariables.Count());
      var rRows = (int)Math.Ceiling(R * problemData.TrainingIndices.Count());
      var alg = RegressionAlgorithm;
      List<IRegressionModel> models = new List<IRegressionModel>();
      try {

        // Loop until iteration limit reached or canceled.
        for (int i = 0; i < Iterations; i++) {
          cancellationToken.ThrowIfCancellationRequested();

          modifiableDataset.RemoveVariable(targetVarName);
          modifiableDataset.AddVariable(targetVarName, curY.Concat(curYTest));

          SampleTrainingData(rand, modifiableDataset, rRows, problemData.Dataset, curY, problemData.TargetVariable, problemData.TrainingIndices); // all training indices from the original problem data are allowed 
          var modifiableProblemData = new RegressionProblemData(modifiableDataset,
            problemData.AllowedInputVariables.SampleRandomWithoutRepetition(rand, mVars),
            problemData.TargetVariable);
          modifiableProblemData.TrainingPartition.Start = 0;
          modifiableProblemData.TrainingPartition.End = rRows;
          modifiableProblemData.TestPartition.Start = problemData.TestPartition.Start;
          modifiableProblemData.TestPartition.End = problemData.TestPartition.End;

          if (!TrySetProblemData(alg, modifiableProblemData))
            throw new NotSupportedException("The algorithm cannot be used with GBM.");

          IRegressionModel model;
          IRun run;
          // try to find a model. The algorithm might fail to produce a model. In this case we just retry until the iterations are exhausted
          if (TryExecute(alg, RegressionAlgorithmResult, out model, out run)) {
            int row = 0;
            // update predictions for training and test
            // update new targets (in the case of squared error loss we simply use negative residuals)
            foreach (var pred in model.GetEstimatedValues(problemData.Dataset, trainingRows)) {
              yPred[row] = yPred[row] + nu * pred;
              curY[row] = y[row] - yPred[row];
              row++;
            }
            row = 0;
            foreach (var pred in model.GetEstimatedValues(problemData.Dataset, testRows)) {
              yPredTest[row] = yPredTest[row] + nu * pred;
              curYTest[row] = yTest[row] - yPredTest[row];
              row++;
            }
            // determine quality
            OnlineCalculatorError error;
            var trainR = OnlinePearsonsRCalculator.Calculate(yPred, y, out error);
            var testR = OnlinePearsonsRCalculator.Calculate(yPredTest, yTest, out error);

            // iteration results
            curLoss.Value = error == OnlineCalculatorError.None ? trainR * trainR : 0.0;
            curTestLoss.Value = error == OnlineCalculatorError.None ? testR * testR : 0.0;

            models.Add(model);


          }

          runCollection.Add(run);
          table.Rows["Loss (train)"].Values.Add(curLoss.Value);
          table.Rows["Loss (test)"].Values.Add(curTestLoss.Value);
          iterations.Value = i + 1;
        }

        // produce solution 
        if (CreateSolution) {
          // when all our models are symbolic models we can easily combine them to a single model
          if (models.All(m => m is ISymbolicRegressionModel)) {
            Results.Add(new Result("Solution", CreateSymbolicSolution(models, Nu, (IRegressionProblemData)problemData.Clone())));
          }
          // just produce an ensemble solution for now (TODO: correct scaling or linear regression for ensemble model weights)
          Results.Add(new Result("EnsembleSolution", new RegressionEnsembleSolution(models, (IRegressionProblemData)problemData.Clone())));
        }
      } finally {
        // reset everything
        alg.Prepare(true);
      }
    }

    // this is probably slow as hell
    private void SampleTrainingData(MersenneTwister rand, ModifiableDataset ds, int rRows,
      IDataset sourceDs, double[] curTarget, string targetVarName, IEnumerable<int> trainingIndices) {
      var selectedRows = trainingIndices.SampleRandomWithoutRepetition(rand, rRows).ToArray();
      int t = 0;
      object[] srcRow = new object[ds.Columns];
      var varNames = ds.DoubleVariables.ToArray();
      foreach (var r in selectedRows) {
        // take all values from the original dataset
        for (int c = 0; c < srcRow.Length; c++) {
          var col = sourceDs.GetReadOnlyDoubleValues(varNames[c]);
          srcRow[c] = col[r];
        }
        ds.ReplaceRow(t, srcRow);
        // but use the updated target values
        ds.SetVariableValue(curTarget[r], targetVarName, t);
        t++;
      }
    }

    private static ISymbolicRegressionSolution CreateSymbolicSolution(List<IRegressionModel> models, double nu, IRegressionProblemData problemData) {
      var symbModels = models.OfType<ISymbolicRegressionModel>();
      var lowerLimit = symbModels.Min(m => m.LowerEstimationLimit);
      var upperLimit = symbModels.Max(m => m.UpperEstimationLimit);
      var interpreter = new SymbolicDataAnalysisExpressionTreeLinearInterpreter();
      var progRootNode = new ProgramRootSymbol().CreateTreeNode();
      var startNode = new StartSymbol().CreateTreeNode();

      var addNode = new Addition().CreateTreeNode();
      var mulNode = new Multiplication().CreateTreeNode();
      var scaleNode = (ConstantTreeNode)new Constant().CreateTreeNode(); // all models are scaled using the same nu
      scaleNode.Value = nu;

      foreach (var m in symbModels) {
        var relevantPart = m.SymbolicExpressionTree.Root.GetSubtree(0).GetSubtree(0); // skip root and start
        addNode.AddSubtree((ISymbolicExpressionTreeNode)relevantPart.Clone());
      }

      mulNode.AddSubtree(addNode);
      mulNode.AddSubtree(scaleNode);
      startNode.AddSubtree(mulNode);
      progRootNode.AddSubtree(startNode);
      var t = new SymbolicExpressionTree(progRootNode);
      var combinedModel = new SymbolicRegressionModel(t, interpreter, lowerLimit, upperLimit);
      var sol = new SymbolicRegressionSolution(combinedModel, problemData);
      return sol;
    }

    private static bool TrySetProblemData(IAlgorithm alg, IRegressionProblemData problemData) {
      var prob = alg.Problem as IRegressionProblem;
      // there is already a problem and it is compatible -> just set problem data
      if (prob != null) {
        prob.ProblemDataParameter.Value = problemData;
        return true;
      } else if (alg.Problem != null) {
        // a problem is set and it is not compatible
        return false;
      } else {
        try {
          // we try to use a symbolic regression problem (works for simple regression algs and GP)
          alg.Problem = new SymbolicRegressionSingleObjectiveProblem();
        } catch (Exception) {
          return false;
        }
        return true;
      }
    }

    private static bool TryExecute(IAlgorithm alg, string regressionAlgorithmResultName, out IRegressionModel model, out IRun run) {
      model = null;
      using (var wh = new AutoResetEvent(false)) {
        EventHandler<EventArgs<Exception>> handler = (sender, args) => wh.Set();
        EventHandler handler2 = (sender, args) => wh.Set();
        alg.ExceptionOccurred += handler;
        alg.Stopped += handler2;
        try {
          alg.Prepare();
          alg.Start();
          wh.WaitOne();

          run = alg.Runs.Last();
          var sols = alg.Results.Select(r => r.Value).OfType<IRegressionSolution>();
          if (!sols.Any()) return false;
          var sol = sols.First();
          if (sols.Skip(1).Any()) {
            // more than one solution => use regressionAlgorithmResult
            if (alg.Results.ContainsKey(regressionAlgorithmResultName)) {
              sol = (IRegressionSolution)alg.Results[regressionAlgorithmResultName].Value;
            }
          }
          var symbRegSol = sol as SymbolicRegressionSolution;
          // only accept symb reg solutions that do not hit the estimation limits
          // NaN evaluations would not be critical but are problematic if we want to combine all symbolic models into a single symbolic model 
          if (symbRegSol == null ||
            (symbRegSol.TrainingLowerEstimationLimitHits == 0 && symbRegSol.TrainingUpperEstimationLimitHits == 0 &&
             symbRegSol.TestLowerEstimationLimitHits == 0 && symbRegSol.TestUpperEstimationLimitHits == 0) &&
            symbRegSol.TrainingNaNEvaluations == 0 && symbRegSol.TestNaNEvaluations == 0) {
            model = sol.Model;
          }
        } finally {
          alg.ExceptionOccurred -= handler;
          alg.Stopped -= handler2;
        }
      }
      return model != null;
    }
  }
}
