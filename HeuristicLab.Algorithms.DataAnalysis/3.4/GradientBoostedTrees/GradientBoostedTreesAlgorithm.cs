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
using System.Linq;
using System.Threading;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [Item("Gradient Boosted Trees (GBT)", "Gradient boosted trees algorithm. Specific implementation of gradient boosting for regression trees. Friedman, J. \"Greedy Function Approximation: A Gradient Boosting Machine\", IMS 1999 Reitz Lecture.")]
  [StorableClass]
  [Creatable(CreatableAttribute.Categories.DataAnalysisRegression, Priority = 125)]
  public class GradientBoostedTreesAlgorithm : BasicAlgorithm {
    public override Type ProblemType {
      get { return typeof(IRegressionProblem); }
    }
    public new IRegressionProblem Problem {
      get { return (IRegressionProblem)base.Problem; }
      set { base.Problem = value; }
    }

    #region ParameterNames
    private const string IterationsParameterName = "Iterations";
    private const string MaxSizeParameterName = "Maximum Tree Size";
    private const string NuParameterName = "Nu";
    private const string RParameterName = "R";
    private const string MParameterName = "M";
    private const string SeedParameterName = "Seed";
    private const string SetSeedRandomlyParameterName = "SetSeedRandomly";
    private const string LossFunctionParameterName = "LossFunction";
    private const string UpdateIntervalParameterName = "UpdateInterval";
    private const string CreateSolutionParameterName = "CreateSolution";
    #endregion

    #region ParameterProperties
    public IFixedValueParameter<IntValue> IterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[IterationsParameterName]; }
    }
    public IFixedValueParameter<IntValue> MaxSizeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaxSizeParameterName]; }
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
    public IFixedValueParameter<IntValue> SeedParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[SeedParameterName]; }
    }
    public FixedValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (FixedValueParameter<BoolValue>)Parameters[SetSeedRandomlyParameterName]; }
    }
    public IConstrainedValueParameter<ILossFunction> LossFunctionParameter {
      get { return (IConstrainedValueParameter<ILossFunction>)Parameters[LossFunctionParameterName]; }
    }
    public IFixedValueParameter<IntValue> UpdateIntervalParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[UpdateIntervalParameterName]; }
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
    public int MaxSize {
      get { return MaxSizeParameter.Value.Value; }
      set { MaxSizeParameter.Value.Value = value; }
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
    #endregion

    #region ResultsProperties
    private double ResultsBestQuality {
      get { return ((DoubleValue)Results["Best Quality"].Value).Value; }
      set { ((DoubleValue)Results["Best Quality"].Value).Value = value; }
    }
    private DataTable ResultsQualities {
      get { return ((DataTable)Results["Qualities"].Value); }
    }
    #endregion

    [StorableConstructor]
    protected GradientBoostedTreesAlgorithm(bool deserializing) : base(deserializing) { }

    protected GradientBoostedTreesAlgorithm(GradientBoostedTreesAlgorithm original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GradientBoostedTreesAlgorithm(this, cloner);
    }

    public GradientBoostedTreesAlgorithm() {
      Problem = new RegressionProblem(); // default problem

      Parameters.Add(new FixedValueParameter<IntValue>(IterationsParameterName, "Number of iterations (set as high as possible, adjust in combination with nu, when increasing iterations also decrease nu)", new IntValue(1000)));
      Parameters.Add(new FixedValueParameter<IntValue>(SeedParameterName, "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<BoolValue>(SetSeedRandomlyParameterName, "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<IntValue>(MaxSizeParameterName, "Maximal size of the tree learned in each step (prefer smaller sizes if possible)", new IntValue(10)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(RParameterName, "Ratio of training rows selected randomly in each step (0 < R <= 1)", new DoubleValue(0.5)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(MParameterName, "Ratio of variables selected randomly in each step (0 < M <= 1)", new DoubleValue(0.5)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(NuParameterName, "Learning rate nu (step size for the gradient update, should be small 0 < nu < 0.1)", new DoubleValue(0.002)));
      Parameters.Add(new FixedValueParameter<IntValue>(UpdateIntervalParameterName, "", new IntValue(100)));
      Parameters[UpdateIntervalParameterName].Hidden = true;
      Parameters.Add(new FixedValueParameter<BoolValue>(CreateSolutionParameterName, "Flag that indicates if a solution should be produced at the end of the run", new BoolValue(true)));
      Parameters[CreateSolutionParameterName].Hidden = true;

      var lossFunctions = ApplicationManager.Manager.GetInstances<ILossFunction>();
      Parameters.Add(new ConstrainedValueParameter<ILossFunction>(LossFunctionParameterName, "The loss function", new ItemSet<ILossFunction>(lossFunctions)));
      LossFunctionParameter.Value = LossFunctionParameter.ValidValues.First(f => f.ToString().Contains("Squared")); // squared error loss is the default
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.4
      #region Backwards compatible code, remove with 3.5
      // parameter type has been changed
      var lossFunctionParam = Parameters[LossFunctionParameterName] as ConstrainedValueParameter<StringValue>;
      if (lossFunctionParam != null) {
        Parameters.Remove(LossFunctionParameterName);
        var selectedValue = lossFunctionParam.Value; // to be restored below

        var lossFunctions = ApplicationManager.Manager.GetInstances<ILossFunction>();
        Parameters.Add(new ConstrainedValueParameter<ILossFunction>(LossFunctionParameterName, "The loss function", new ItemSet<ILossFunction>(lossFunctions)));
        // try to restore selected value
        var selectedLossFunction =
          LossFunctionParameter.ValidValues.FirstOrDefault(f => f.ToString() == selectedValue.Value);
        if (selectedLossFunction != null) {
          LossFunctionParameter.Value = selectedLossFunction;
        } else {
          LossFunctionParameter.Value = LossFunctionParameter.ValidValues.First(f => f.ToString().Contains("Squared")); // default: SE
        }
      }
      #endregion
    }

    protected override void Run(CancellationToken cancellationToken) {
      // Set up the algorithm
      if (SetSeedRandomly) Seed = new System.Random().Next();

      // Set up the results display
      var iterations = new IntValue(0);
      Results.Add(new Result("Iterations", iterations));

      var table = new DataTable("Qualities");
      table.Rows.Add(new DataRow("Loss (train)"));
      table.Rows.Add(new DataRow("Loss (test)"));
      Results.Add(new Result("Qualities", table));
      var curLoss = new DoubleValue();
      Results.Add(new Result("Loss (train)", curLoss));

      // init
      var problemData = (IRegressionProblemData)Problem.ProblemData.Clone();
      var lossFunction = LossFunctionParameter.Value;
      var state = GradientBoostedTreesAlgorithmStatic.CreateGbmState(problemData, lossFunction, (uint)Seed, MaxSize, R, M, Nu);

      var updateInterval = UpdateIntervalParameter.Value.Value;
      // Loop until iteration limit reached or canceled.
      for (int i = 0; i < Iterations; i++) {
        cancellationToken.ThrowIfCancellationRequested();

        GradientBoostedTreesAlgorithmStatic.MakeStep(state);

        // iteration results
        if (i % updateInterval == 0) {
          curLoss.Value = state.GetTrainLoss();
          table.Rows["Loss (train)"].Values.Add(curLoss.Value);
          table.Rows["Loss (test)"].Values.Add(state.GetTestLoss());
          iterations.Value = i;
        }
      }

      // final results
      iterations.Value = Iterations;
      curLoss.Value = state.GetTrainLoss();
      table.Rows["Loss (train)"].Values.Add(curLoss.Value);
      table.Rows["Loss (test)"].Values.Add(state.GetTestLoss());

      // produce variable relevance
      var orderedImpacts = state.GetVariableRelevance().Select(t => new { name = t.Key, impact = t.Value }).ToList();

      var impacts = new DoubleMatrix();
      var matrix = impacts as IStringConvertibleMatrix;
      matrix.Rows = orderedImpacts.Count;
      matrix.RowNames = orderedImpacts.Select(x => x.name);
      matrix.Columns = 1;
      matrix.ColumnNames = new string[] { "Relative variable relevance" };

      int rowIdx = 0;
      foreach (var p in orderedImpacts) {
        matrix.SetValue(string.Format("{0:N2}", p.impact), rowIdx++, 0);
      }

      Results.Add(new Result("Variable relevance", impacts));
      Results.Add(new Result("Loss (test)", new DoubleValue(state.GetTestLoss())));

      // produce solution 
      if (CreateSolution) {
        var model = state.GetModel();

        // for logistic regression we produce a classification solution
        if (lossFunction is LogisticRegressionLoss) {
          var classificationModel = new DiscriminantFunctionClassificationModel(model,
            new AccuracyMaximizationThresholdCalculator());
          var classificationProblemData = new ClassificationProblemData(problemData.Dataset,
            problemData.AllowedInputVariables, problemData.TargetVariable, problemData.Transformations);
          classificationModel.RecalculateModelParameters(classificationProblemData, classificationProblemData.TrainingIndices);

          var classificationSolution = new DiscriminantFunctionClassificationSolution(classificationModel, classificationProblemData);
          Results.Add(new Result("Solution", classificationSolution));
        } else {
          // otherwise we produce a regression solution
          Results.Add(new Result("Solution", new RegressionSolution(model, problemData)));
        }
      }
    }
  }
}
