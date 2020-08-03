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
using System.Drawing;
using System.Linq;
using System.Threading;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Interpreters;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;

namespace HeuristicLab.Problems.VehicleRouting {
  [Item("Vehicle Routing Problem (VRP)", "Represents a Vehicle Routing Problem.")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 110)]
  [StorableType("95137523-AE3B-4638-958C-E86829D54CE3")]
  public sealed class VehicleRoutingProblem : SingleObjectiveProblem<IVRPEncoding, IVRPEncodedSolution>, IProblemInstanceConsumer<IVRPData> {

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Type; }
    }

    #region Parameter Properties
    public ValueParameter<IVRPProblemInstance> ProblemInstanceParameter {
      get { return (ValueParameter<IVRPProblemInstance>)Parameters["ProblemInstance"]; }
    }
    public OptionalValueParameter<VRPSolution> BestKnownSolutionParameter {
      get { return (OptionalValueParameter<VRPSolution>)Parameters["BestKnownSolution"]; }
    }
    #endregion

    #region Properties
    public IVRPProblemInstance ProblemInstance {
      get { return ProblemInstanceParameter.Value; }
      set { ProblemInstanceParameter.Value = value; }
    }

    public VRPSolution BestKnownSolution {
      get { return BestKnownSolutionParameter.Value; }
      set { BestKnownSolutionParameter.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private VehicleRoutingProblem(StorableConstructorFlag _) : base(_) { }
    public VehicleRoutingProblem()
      : base(new PotvinEncoding()) {
      EncodingParameter.Hidden = false;

      Parameters.Add(new ValueParameter<IVRPProblemInstance>("ProblemInstance", "The VRP problem instance"));
      Parameters.Add(new OptionalValueParameter<VRPSolution>("BestKnownSolution", "The best known solution of this VRP instance."));

      InitializeRandomVRPInstance();
      InitializeOperators();

      AttachEventHandlers();
      AttachProblemInstanceEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      cloner.Clone(ProblemInstance);
      return new VehicleRoutingProblem(this, cloner);
    }

    private VehicleRoutingProblem(VehicleRoutingProblem original, Cloner cloner)
      : base(original, cloner) {
      this.AttachEventHandlers();
      this.AttachProblemInstanceEventHandlers();
    }

    public override void Evaluate(ISingleObjectiveSolutionContext<IVRPEncodedSolution> solutionContext, IRandom random, CancellationToken cancellationToken) {
      solutionContext.EvaluationResult = ProblemInstance.Evaluate(solutionContext.EncodedSolution);
    }
    public override ISingleObjectiveEvaluationResult Evaluate(IVRPEncodedSolution solution, IRandom random, CancellationToken cancellationToken) {
      return ProblemInstance.Evaluate(solution);
    }

    public override void Analyze(ISingleObjectiveSolutionContext<IVRPEncodedSolution>[] solutionContexts, ResultCollection results, IRandom random) {
      base.Analyze(solutionContexts, results, random);
      var evaluations = solutionContexts.Select(x => (VRPEvaluation)x.EvaluationResult);

      var bestInPop = evaluations.Select((x, index) => new { index, Eval = x }).OrderBy(x => x.Eval.Quality).First();
      IResult bestSolutionResult;
      if (!results.TryGetValue("Best VRP Solution", out bestSolutionResult) || !(bestSolutionResult.Value is VRPSolution)) {
        var best = new VRPSolution(ProblemInstance, solutionContexts[bestInPop.index].EncodedSolution, (VRPEvaluation)bestInPop.Eval.Clone());
        if (bestSolutionResult != null)
          bestSolutionResult.Value = best;
        else results.Add(bestSolutionResult = new Result("Best VRP Solution", best));
      }

      var bestSolution = (VRPSolution)bestSolutionResult.Value;
      if (bestSolution == null || bestInPop.Eval.Quality < bestSolution.Evaluation.Quality) {
        var best = new VRPSolution(ProblemInstance,
          (IVRPEncodedSolution)solutionContexts[bestInPop.index].EncodedSolution.Clone(),
          (VRPEvaluation)bestInPop.Eval.Clone());
        bestSolutionResult.Value = best;
      };

      var bestValidInPop = evaluations.Select((x, index) => new { index, Eval = x }).Where(x => x.Eval.IsFeasible).OrderBy(x => x.Eval.Quality).FirstOrDefault();
      IResult bestValidSolutionResult;
      if (!results.TryGetValue("Best Valid VRP Solution", out bestValidSolutionResult) || !(bestValidSolutionResult.Value is VRPSolution)) {
        var bestValid = new VRPSolution(ProblemInstance, solutionContexts[bestValidInPop.index].EncodedSolution, (VRPEvaluation)bestValidInPop.Eval.Clone());
        if (bestValidSolutionResult != null)
          bestValidSolutionResult.Value = bestValid;
        else results.Add(bestValidSolutionResult = new Result("Best Valid VRP Solution", bestValid));
      }

      if (bestValidInPop != null) {
        var bestValidSolution = (VRPSolution)bestValidSolutionResult.Value;
        if (bestValidSolution == null || bestValidInPop.Eval.Quality < bestValidSolution.Evaluation.Quality) {
          var best = new VRPSolution(ProblemInstance,
            (IVRPEncodedSolution)solutionContexts[bestValidInPop.index].EncodedSolution.Clone(),
            (VRPEvaluation)bestValidInPop.Eval.Clone());
          bestValidSolutionResult.Value = best;
        };
      }
    }

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      AttachEventHandlers();
      AttachProblemInstanceEventHandlers();
    }

    [Storable(OldName = "operators")]
    private List<IOperator> StorableOperators {
      set { Operators.AddRange(value); }
    }

    private void AttachEventHandlers() {
      ProblemInstanceParameter.ValueChanged += new EventHandler(ProblemInstanceParameter_ValueChanged);
      BestKnownSolutionParameter.ValueChanged += new EventHandler(BestKnownSolutionParameter_ValueChanged);
    }

    private void AttachProblemInstanceEventHandlers() {
      if (ProblemInstance != null) {
        ProblemInstance.EvaluationChanged += new EventHandler(ProblemInstance_EvaluationChanged);
      }
    }

    private void EvalBestKnownSolution() {
      if (BestKnownSolution == null) return;
      try {
        //call evaluator
        var evaluation = ProblemInstance.Evaluate(BestKnownSolution.Solution);
        BestKnownQuality = evaluation.Quality;
        BestKnownSolution.Evaluation = evaluation;
      } catch {
        BestKnownQuality = double.NaN;
        BestKnownSolution = null;
      }
    }

    void BestKnownSolutionParameter_ValueChanged(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }

    void ProblemInstance_EvaluationChanged(object sender, EventArgs e) {
      BestKnownQuality = double.NaN;
      if (BestKnownSolution != null) {
        // the tour is not valid if there are more vehicles in it than allowed
        if (ProblemInstance.Vehicles.Value < BestKnownSolution.Solution.GetTours().Count) {
          BestKnownSolution = null;
        } else EvalBestKnownSolution();
      }
    }

    void ProblemInstanceParameter_ValueChanged(object sender, EventArgs e) {
      InitializeOperators();
      AttachProblemInstanceEventHandlers();

      OnOperatorsChanged();
    }

    public void SetProblemInstance(IVRPProblemInstance instance) {
      ProblemInstanceParameter.ValueChanged -= new EventHandler(ProblemInstanceParameter_ValueChanged);

      ProblemInstance = instance;
      AttachProblemInstanceEventHandlers();

      ProblemInstanceParameter.ValueChanged += new EventHandler(ProblemInstanceParameter_ValueChanged);
    }

    private void InitializeOperators() {
      Encoding.FilterOperators(ProblemInstance);

      Operators.Add(new VRPSimilarityCalculator());
      Operators.Add(new QualitySimilarityCalculator());
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));
      //Operators.AddRange(ProblemInstance.Operators.OfType<IAnalyzer>());
    }

    protected override void ParameterizeOperators() {
      base.ParameterizeOperators();
      Parameterize();
    }

    private void Parameterize() {
      foreach (ISolutionSimilarityCalculator op in Operators.OfType<ISolutionSimilarityCalculator>()) {
        op.SolutionVariableName = Encoding.Name;
        op.QualityVariableName = Evaluator.QualityParameter.ActualName;
        var calc = op as VRPSimilarityCalculator;
        if (calc != null) calc.ProblemInstance = ProblemInstance;
      }
    }

    #endregion

    private void InitializeRandomVRPInstance() {
      System.Random rand = new System.Random();

      CVRPTWProblemInstance problem = new CVRPTWProblemInstance();
      int cities = 100;

      problem.Coordinates = new DoubleMatrix(cities + 1, 2);
      problem.Demand = new DoubleArray(cities + 1);
      problem.DueTime = new DoubleArray(cities + 1);
      problem.ReadyTime = new DoubleArray(cities + 1);
      problem.ServiceTime = new DoubleArray(cities + 1);

      problem.Vehicles.Value = 100;
      problem.Capacity.Value = 200;

      for (int i = 0; i <= cities; i++) {
        problem.Coordinates[i, 0] = rand.Next(0, 100);
        problem.Coordinates[i, 1] = rand.Next(0, 100);

        if (i == 0) {
          problem.Demand[i] = 0;
          problem.DueTime[i] = Int16.MaxValue;
          problem.ReadyTime[i] = 0;
          problem.ServiceTime[i] = 0;
        } else {
          problem.Demand[i] = rand.Next(10, 50);
          problem.DueTime[i] = rand.Next((int)Math.Ceiling(problem.GetDistance(0, i, null)), 1200);
          problem.ReadyTime[i] = problem.DueTime[i] - rand.Next(0, 100);
          problem.ServiceTime[i] = 90;
        }
      }

      this.ProblemInstance = problem;
    }

    public void ImportSolution(string solutionFileName) {
      SolutionParser parser = new SolutionParser(solutionFileName);
      parser.Parse();

      HeuristicLab.Problems.VehicleRouting.Encodings.Potvin.PotvinEncodedSolution encoding = new Encodings.Potvin.PotvinEncodedSolution(ProblemInstance);

      int cities = 0;
      foreach (List<int> route in parser.Routes) {
        Tour tour = new Tour();
        tour.Stops.AddRange(route);
        cities += tour.Stops.Count;

        encoding.Tours.Add(tour);
      }

      if (cities != ProblemInstance.Coordinates.Rows - 1)
        ErrorHandling.ShowErrorDialog(new Exception("The optimal solution does not seem to correspond with the problem data"));
      else {
        VRPSolution solution = new VRPSolution(ProblemInstance, encoding, ProblemInstance.Evaluate(encoding));
        BestKnownSolutionParameter.Value = solution;
      }
    }

    #region Instance Consuming
    public void Load(IVRPData data, IVRPDataInterpreter interpreter) {
      VRPInstanceDescription instance = interpreter.Interpret(data);

      Name = instance.Name;
      Description = instance.Description;

      BestKnownQuality = double.NaN;
      BestKnownSolution = null;

      if (ProblemInstance != null && instance.ProblemInstance != null &&
        instance.ProblemInstance.GetType() == ProblemInstance.GetType())
        SetProblemInstance(instance.ProblemInstance);
      else
        ProblemInstance = instance.ProblemInstance;

      OnReset();

      if (instance.BestKnownQuality != null) {
        BestKnownQuality = instance.BestKnownQuality ?? double.NaN;
      }

      if (instance.BestKnownSolution != null) {
        VRPSolution solution = new VRPSolution(ProblemInstance, instance.BestKnownSolution, ProblemInstance.Evaluate(instance.BestKnownSolution));
        BestKnownSolution = solution;
      }
    }
    #endregion

    #region IProblemInstanceConsumer<VRPData> Members

    public void Load(IVRPData data) {
      var interpreterDataType = data.GetType();
      var interpreterType = typeof(IVRPDataInterpreter<>).MakeGenericType(interpreterDataType);

      var interpreters = ApplicationManager.Manager.GetTypes(interpreterType);

      var concreteInterpreter = interpreters.Single(t => GetInterpreterDataType(t) == interpreterDataType);

      Load(data, (IVRPDataInterpreter)Activator.CreateInstance(concreteInterpreter));
    }

    private Type GetInterpreterDataType(Type type) {
      var parentInterfaces = type.BaseType.GetInterfaces();
      var interfaces = type.GetInterfaces().Except(parentInterfaces);

      var interpreterInterface = interfaces.Single(i => typeof(IVRPDataInterpreter).IsAssignableFrom(i));
      return interpreterInterface.GetGenericArguments()[0];
    }
    #endregion
  }
}
