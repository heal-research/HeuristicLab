#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Runtime.CompilerServices;
using System.Threading;
using HeuristicLab.Algorithms.DataAnalysis.MctsSymbolicRegression.Policies;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis.MctsSymbolicRegression {
  [Item("MCTS Symbolic Regression", "Monte carlo tree search for symbolic regression. Useful mainly as a base learner in gradient boosting.")]
  [StorableClass]
  [Creatable(CreatableAttribute.Categories.DataAnalysisRegression, Priority = 250)]
  public class MctsSymbolicRegressionAlgorithm : FixedDataAnalysisAlgorithm<IRegressionProblem> {

    #region ParameterNames
    private const string IterationsParameterName = "Iterations";
    private const string MaxVariablesParameterName = "Maximum variables";
    private const string ScaleVariablesParameterName = "Scale variables";
    private const string AllowedFactorsParameterName = "Allowed factors";
    private const string ConstantOptimizationIterationsParameterName = "Iterations (constant optimization)";
    private const string PolicyParameterName = "Policy";
    private const string SeedParameterName = "Seed";
    private const string SetSeedRandomlyParameterName = "SetSeedRandomly";
    private const string UpdateIntervalParameterName = "UpdateInterval";
    private const string CreateSolutionParameterName = "CreateSolution";
    private const string PunishmentFactorParameterName = "PunishmentFactor";

    private const string VariableProductFactorName = "product(xi)";
    private const string ExpFactorName = "exp(c * product(xi))";
    private const string LogFactorName = "log(c + sum(c*product(xi))";
    private const string InvFactorName = "1 / (1 + sum(c*product(xi))";
    private const string FactorSumsName = "sum of multiple terms";
    #endregion

    #region ParameterProperties
    public IFixedValueParameter<IntValue> IterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[IterationsParameterName]; }
    }
    public IFixedValueParameter<IntValue> MaxVariableReferencesParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaxVariablesParameterName]; }
    }
    public IFixedValueParameter<BoolValue> ScaleVariablesParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[ScaleVariablesParameterName]; }
    }
    public IFixedValueParameter<IntValue> ConstantOptimizationIterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[ConstantOptimizationIterationsParameterName]; }
    }
    public IValueParameter<IPolicy> PolicyParameter {
      get { return (IValueParameter<IPolicy>)Parameters[PolicyParameterName]; }
    }
    public IFixedValueParameter<DoubleValue> PunishmentFactorParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[PunishmentFactorParameterName]; }
    }
    public IValueParameter<ICheckedItemList<StringValue>> AllowedFactorsParameter {
      get { return (IValueParameter<ICheckedItemList<StringValue>>)Parameters[AllowedFactorsParameterName]; }
    }
    public IFixedValueParameter<IntValue> SeedParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[SeedParameterName]; }
    }
    public FixedValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (FixedValueParameter<BoolValue>)Parameters[SetSeedRandomlyParameterName]; }
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
    public int MaxVariableReferences {
      get { return MaxVariableReferencesParameter.Value.Value; }
      set { MaxVariableReferencesParameter.Value.Value = value; }
    }
    public IPolicy Policy {
      get { return PolicyParameter.Value; }
      set { PolicyParameter.Value = value; }
    }
    public double PunishmentFactor {
      get { return PunishmentFactorParameter.Value.Value; }
      set { PunishmentFactorParameter.Value.Value = value; }
    }
    public ICheckedItemList<StringValue> AllowedFactors {
      get { return AllowedFactorsParameter.Value; }
    }
    public int ConstantOptimizationIterations {
      get { return ConstantOptimizationIterationsParameter.Value.Value; }
      set { ConstantOptimizationIterationsParameter.Value.Value = value; }
    }
    public bool ScaleVariables {
      get { return ScaleVariablesParameter.Value.Value; }
      set { ScaleVariablesParameter.Value.Value = value; }
    }
    public bool CreateSolution {
      get { return CreateSolutionParameter.Value.Value; }
      set { CreateSolutionParameter.Value.Value = value; }
    }
    #endregion

    [StorableConstructor]
    protected MctsSymbolicRegressionAlgorithm(bool deserializing) : base(deserializing) { }

    protected MctsSymbolicRegressionAlgorithm(MctsSymbolicRegressionAlgorithm original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MctsSymbolicRegressionAlgorithm(this, cloner);
    }

    public MctsSymbolicRegressionAlgorithm() {
      Problem = new RegressionProblem(); // default problem

      var defaultFactorsList = new CheckedItemList<StringValue>(
        new string[] { VariableProductFactorName, ExpFactorName, LogFactorName, InvFactorName, FactorSumsName }
        .Select(s => new StringValue(s).AsReadOnly())
        ).AsReadOnly();
      defaultFactorsList.SetItemCheckedState(defaultFactorsList.First(s => s.Value == FactorSumsName), false);

      Parameters.Add(new FixedValueParameter<IntValue>(IterationsParameterName,
        "Number of iterations", new IntValue(100000)));
      Parameters.Add(new FixedValueParameter<IntValue>(SeedParameterName,
        "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<BoolValue>(SetSeedRandomlyParameterName,
        "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<IntValue>(MaxVariablesParameterName,
        "Maximal number of variables references in the symbolic regression models (multiple usages of the same variable are counted)", new IntValue(5)));
      // Parameters.Add(new FixedValueParameter<DoubleValue>(CParameterName,
      //   "Balancing parameter in UCT formula (0 < c < 1000). Small values: greedy search. Large values: enumeration. Default: 1.0", new DoubleValue(1.0)));
      Parameters.Add(new ValueParameter<IPolicy>(PolicyParameterName,
        "The policy to use for selecting nodes in MCTS (e.g. Ucb)", new Ucb()));
      PolicyParameter.Hidden = true;
      Parameters.Add(new ValueParameter<ICheckedItemList<StringValue>>(AllowedFactorsParameterName,
        "Choose which expressions are allowed as factors in the model.", defaultFactorsList));

      Parameters.Add(new FixedValueParameter<IntValue>(ConstantOptimizationIterationsParameterName,
        "Number of iterations for constant optimization. A small number of iterations should be sufficient for most models. " +
        "Set to 0 to disable constants optimization.", new IntValue(10)));
      Parameters.Add(new FixedValueParameter<BoolValue>(ScaleVariablesParameterName,
        "Set to true to scale all input variables to the range [0..1]", new BoolValue(false)));
      Parameters[ScaleVariablesParameterName].Hidden = true;
      Parameters.Add(new FixedValueParameter<DoubleValue>(PunishmentFactorParameterName, "Estimations of models can be bounded. The estimation limits are calculated in the following way (lb = mean(y) - punishmentFactor*range(y), ub = mean(y) + punishmentFactor*range(y))", new DoubleValue(10)));
      Parameters[PunishmentFactorParameterName].Hidden = true;
      Parameters.Add(new FixedValueParameter<IntValue>(UpdateIntervalParameterName,
        "Number of iterations until the results are updated", new IntValue(100)));
      Parameters[UpdateIntervalParameterName].Hidden = true;
      Parameters.Add(new FixedValueParameter<BoolValue>(CreateSolutionParameterName,
        "Flag that indicates if a solution should be produced at the end of the run", new BoolValue(true)));
      Parameters[CreateSolutionParameterName].Hidden = true;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }

    protected override void Run(CancellationToken cancellationToken) {
      // Set up the algorithm
      if (SetSeedRandomly) Seed = new System.Random().Next();

      // Set up the results display
      var iterations = new IntValue(0);
      Results.Add(new Result("Iterations", iterations));

      var bestSolutionIteration = new IntValue(0);
      Results.Add(new Result("Best solution iteration", bestSolutionIteration));

      var table = new DataTable("Qualities");
      table.Rows.Add(new DataRow("Best quality"));
      table.Rows.Add(new DataRow("Current best quality"));
      table.Rows.Add(new DataRow("Average quality"));
      Results.Add(new Result("Qualities", table));

      var bestQuality = new DoubleValue();
      Results.Add(new Result("Best quality", bestQuality));

      var curQuality = new DoubleValue();
      Results.Add(new Result("Current best quality", curQuality));

      var avgQuality = new DoubleValue();
      Results.Add(new Result("Average quality", avgQuality));

      var totalRollouts = new IntValue();
      Results.Add(new Result("Total rollouts", totalRollouts));
      var effRollouts = new IntValue();
      Results.Add(new Result("Effective rollouts", effRollouts));
      var funcEvals = new IntValue();
      Results.Add(new Result("Function evaluations", funcEvals));
      var gradEvals = new IntValue();
      Results.Add(new Result("Gradient evaluations", gradEvals));


      // same as in SymbolicRegressionSingleObjectiveProblem
      var y = Problem.ProblemData.Dataset.GetDoubleValues(Problem.ProblemData.TargetVariable,
        Problem.ProblemData.TrainingIndices);
      var avgY = y.Average();
      var minY = y.Min();
      var maxY = y.Max();
      var range = maxY - minY;
      var lowerLimit = avgY - PunishmentFactor * range;
      var upperLimit = avgY + PunishmentFactor * range;

      // init
      var problemData = (IRegressionProblemData)Problem.ProblemData.Clone();
      if (!AllowedFactors.CheckedItems.Any()) throw new ArgumentException("At least on type of factor must be allowed");
      var state = MctsSymbolicRegressionStatic.CreateState(problemData, (uint)Seed, MaxVariableReferences, ScaleVariables, ConstantOptimizationIterations,
        Policy,
        lowerLimit, upperLimit,
        allowProdOfVars: AllowedFactors.CheckedItems.Any(s => s.Value.Value == VariableProductFactorName),
        allowExp: AllowedFactors.CheckedItems.Any(s => s.Value.Value == ExpFactorName),
        allowLog: AllowedFactors.CheckedItems.Any(s => s.Value.Value == LogFactorName),
        allowInv: AllowedFactors.CheckedItems.Any(s => s.Value.Value == InvFactorName),
        allowMultipleTerms: AllowedFactors.CheckedItems.Any(s => s.Value.Value == FactorSumsName)
        );

      var updateInterval = UpdateIntervalParameter.Value.Value;
      double sumQ = 0.0;
      double bestQ = 0.0;
      double curBestQ = 0.0;
      int n = 0;
      // Loop until iteration limit reached or canceled.
      for (int i = 0; i < Iterations && !state.Done; i++) {
        cancellationToken.ThrowIfCancellationRequested();

        var q = MctsSymbolicRegressionStatic.MakeStep(state);
        sumQ += q; // sum of qs in the last updateinterval iterations
        curBestQ = Math.Max(q, curBestQ); // the best q in the last updateinterval iterations
        bestQ = Math.Max(q, bestQ); // the best q overall
        n++;
        // iteration results
        if (n == updateInterval) {
          if (bestQ > bestQuality.Value) {
            bestSolutionIteration.Value = i;
          }
          bestQuality.Value = bestQ;
          curQuality.Value = curBestQ;
          avgQuality.Value = sumQ / n;
          sumQ = 0.0;
          curBestQ = 0.0;

          funcEvals.Value = state.FuncEvaluations;
          gradEvals.Value = state.GradEvaluations;
          effRollouts.Value = state.EffectiveRollouts;
          totalRollouts.Value = state.TotalRollouts;

          table.Rows["Best quality"].Values.Add(bestQuality.Value);
          table.Rows["Current best quality"].Values.Add(curQuality.Value);
          table.Rows["Average quality"].Values.Add(avgQuality.Value);
          iterations.Value += n;
          n = 0;
        }
      }

      // final results
      if (n > 0) {
        if (bestQ > bestQuality.Value) {
          bestSolutionIteration.Value = iterations.Value + n;
        }
        bestQuality.Value = bestQ;
        curQuality.Value = curBestQ;
        avgQuality.Value = sumQ / n;

        funcEvals.Value = state.FuncEvaluations;
        gradEvals.Value = state.GradEvaluations;
        effRollouts.Value = state.EffectiveRollouts;
        totalRollouts.Value = state.TotalRollouts;

        table.Rows["Best quality"].Values.Add(bestQuality.Value);
        table.Rows["Current best quality"].Values.Add(curQuality.Value);
        table.Rows["Average quality"].Values.Add(avgQuality.Value);
        iterations.Value = iterations.Value + n;

      }


      Results.Add(new Result("Best solution quality (train)", new DoubleValue(state.BestSolutionTrainingQuality)));
      Results.Add(new Result("Best solution quality (test)", new DoubleValue(state.BestSolutionTestQuality)));


      // produce solution 
      if (CreateSolution) {
        var model = state.BestModel;

        // otherwise we produce a regression solution
        Results.Add(new Result("Solution", model.CreateRegressionSolution(problemData)));
      }
    }
  }
}
