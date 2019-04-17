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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [StorableType("AB0C6A73-C432-46FD-AE3B-9841EAB2478C")]
  [Creatable(CreatableAttribute.Categories.Problems, Priority = 95)]
  [Item("Test Function (multi-objective)", "Test functions with real valued inputs and multiple objectives.")]
  public class MultiObjectiveTestFunctionProblem : MultiObjectiveProblem<RealVectorEncoding, RealVector>, IProblemInstanceConsumer<MOTFData> {

    #region Parameter Properties
    public IFixedValueParameter<IntValue> ProblemSizeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["ProblemSize"]; }
    }
    public IFixedValueParameter<IntValue> ObjectivesParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Objectives"]; }
    }
    public IValueParameter<DoubleMatrix> BoundsParameter {
      get { return (IValueParameter<DoubleMatrix>)Parameters["Bounds"]; }
    }
    public IValueParameter<IMultiObjectiveTestFunction> TestFunctionParameter {
      get { return (IValueParameter<IMultiObjectiveTestFunction>)Parameters["TestFunction"]; }
    }
    public IValueParameter<DoubleArray> ReferencePointParameter {
      get { return (IValueParameter<DoubleArray>)Parameters["ReferencePoint"]; }
    }
    public OptionalValueParameter<DoubleMatrix> BestKnownFrontParameter {
      get { return (OptionalValueParameter<DoubleMatrix>)Parameters["BestKnownFront"]; }
    }

    #endregion

    #region Properties
    public override bool[] Maximization {
      get {
        //necessary because of virtual member call in base ctor
        if (!Parameters.ContainsKey("TestFunction")) return new bool[0];
        return TestFunction.Maximization(Objectives).ToArray();
      }
    }

    public int ProblemSize {
      get { return ProblemSizeParameter.Value.Value; }
      set { ProblemSizeParameter.Value.Value = value; }
    }
    public int Objectives {
      get { return ObjectivesParameter.Value.Value; }
      set { ObjectivesParameter.Value.Value = value; }
    }
    public DoubleMatrix Bounds {
      get { return BoundsParameter.Value; }
      set { BoundsParameter.Value = value; }
    }
    public IMultiObjectiveTestFunction TestFunction {
      get { return TestFunctionParameter.Value; }
      set { TestFunctionParameter.Value = value; }
    }
    public DoubleArray ReferencePoint {
      get { return ReferencePointParameter.Value; }
      set { ReferencePointParameter.Value = value; }
    }
    public DoubleMatrix BestKnownFront {
      get { return BestKnownFrontParameter.Value; }
      set { BestKnownFrontParameter.Value = value; }
    }
    #endregion

    [StorableConstructor]
    protected MultiObjectiveTestFunctionProblem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    protected MultiObjectiveTestFunctionProblem(MultiObjectiveTestFunctionProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveTestFunctionProblem(this, cloner);
    }

    public MultiObjectiveTestFunctionProblem()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>("ProblemSize", "The dimensionality of the problem instance (number of variables in the function).", new IntValue(2)));
      Parameters.Add(new FixedValueParameter<IntValue>("Objectives", "The dimensionality of the solution vector (number of objectives).", new IntValue(2)));
      Parameters.Add(new ValueParameter<DoubleMatrix>("Bounds", "The bounds of the solution given as either one line for all variables or a line for each variable. The first column specifies lower bound, the second upper bound.", new DoubleMatrix(new double[,] { { -4, 4 } })));
      Parameters.Add(new ValueParameter<DoubleArray>("ReferencePoint", "The reference point used for hypervolume calculation."));
      Parameters.Add(new ValueParameter<IMultiObjectiveTestFunction>("TestFunction", "The function that is to be optimized.", new Fonseca()));
      Parameters.Add(new OptionalValueParameter<DoubleMatrix>("BestKnownFront", "The currently best known Pareto front"));

      Encoding.LengthParameter = ProblemSizeParameter;
      Encoding.BoundsParameter = BoundsParameter;
      BestKnownFrontParameter.Hidden = true;

      UpdateParameterValues();
      InitializeOperators();
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      TestFunctionParameter.ValueChanged += TestFunctionParameterOnValueChanged;
      ProblemSizeParameter.Value.ValueChanged += ProblemSizeOnValueChanged;
      ObjectivesParameter.Value.ValueChanged += ObjectivesOnValueChanged;
    }


    public override void Analyze(RealVector[] solutions, double[][] qualities, ResultCollection results, IRandom random) {
      base.Analyze(solutions, qualities, results, random);
      if (results.ContainsKey("Pareto Front")) {
        ((DoubleMatrix)results["Pareto Front"].Value).SortableView = true;
      }
    }

    /// <summary>
    /// Checks whether a given solution violates the contraints of this function.
    /// </summary>
    /// <param name="individual"></param>
    /// <returns>a double array that holds the distances that describe how much every contraint is violated (0 is not violated). If the current TestFunction does not have constraints an array of length 0 is returned</returns>
    public double[] CheckContraints(RealVector individual) {
      var constrainedTestFunction = (IConstrainedTestFunction)TestFunction;
      if (constrainedTestFunction != null) {
        return constrainedTestFunction.CheckConstraints(individual, Objectives);
      }
      return new double[0];
    }

    public override double[] Evaluate(RealVector solution, IRandom random) {
      return TestFunction.Evaluate(solution, Objectives);
    }


    public void Load(MOTFData data) {
      TestFunction = data.TestFunction;
    }

    #region Events
    private void UpdateParameterValues() {
      MaximizationParameter.Value = (BoolArray)new BoolArray(TestFunction.Maximization(Objectives)).AsReadOnly();

      var front = TestFunction.OptimalParetoFront(Objectives);
      if (front != null) {
        BestKnownFrontParameter.Value = (DoubleMatrix)Utilities.ToMatrix(front).AsReadOnly();
      } else BestKnownFrontParameter.Value = null;


      BoundsParameter.Value = new DoubleMatrix(TestFunction.Bounds(Objectives));
      ReferencePointParameter.Value = new DoubleArray(TestFunction.ReferencePoint(Objectives));
    }

    protected override void OnEncodingChanged() {
      base.OnEncodingChanged();
      UpdateParameterValues();
      ParameterizeAnalyzers();
    }
    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      UpdateParameterValues();
      ParameterizeAnalyzers();
    }

    private void TestFunctionParameterOnValueChanged(object sender, EventArgs eventArgs) {
      ProblemSize = Math.Max(TestFunction.MinimumSolutionLength, Math.Min(ProblemSize, TestFunction.MaximumSolutionLength));
      Objectives = Math.Max(TestFunction.MinimumObjectives, Math.Min(Objectives, TestFunction.MaximumObjectives));
      ReferencePointParameter.ActualValue = new DoubleArray(TestFunction.ReferencePoint(Objectives));
      ParameterizeAnalyzers();
      UpdateParameterValues();
      OnReset();
    }

    private void ProblemSizeOnValueChanged(object sender, EventArgs eventArgs) {
      ProblemSize = Math.Min(TestFunction.MaximumSolutionLength, Math.Max(TestFunction.MinimumSolutionLength, ProblemSize));
      UpdateParameterValues();
    }

    private void ObjectivesOnValueChanged(object sender, EventArgs eventArgs) {
      Objectives = Math.Min(TestFunction.MaximumObjectives, Math.Max(TestFunction.MinimumObjectives, Objectives));
      UpdateParameterValues();
    }

    #endregion

    #region Helpers
    private void InitializeOperators() {
      Operators.Add(new CrowdingAnalyzer());
      Operators.Add(new GenerationalDistanceAnalyzer());
      Operators.Add(new InvertedGenerationalDistanceAnalyzer());
      Operators.Add(new HypervolumeAnalyzer());
      Operators.Add(new SpacingAnalyzer());
      Operators.Add(new ScatterPlotAnalyzer());

      ParameterizeAnalyzers();
    }

    private IEnumerable<IMultiObjectiveTestFunctionAnalyzer> Analyzers {
      get { return Operators.OfType<IMultiObjectiveTestFunctionAnalyzer>(); }
    }

    private void ParameterizeAnalyzers() {
      foreach (var analyzer in Analyzers) {
        analyzer.ResultsParameter.ActualName = "Results";
        analyzer.QualitiesParameter.ActualName = Evaluator.QualitiesParameter.ActualName;
        analyzer.TestFunctionParameter.ActualName = TestFunctionParameter.Name;
        analyzer.BestKnownFrontParameter.ActualName = BestKnownFrontParameter.Name;

        var crowdingAnalyzer = analyzer as CrowdingAnalyzer;
        if (crowdingAnalyzer != null) {
          crowdingAnalyzer.BoundsParameter.ActualName = BoundsParameter.Name;
        }

        var scatterPlotAnalyzer = analyzer as ScatterPlotAnalyzer;
        if (scatterPlotAnalyzer != null) {
          scatterPlotAnalyzer.IndividualsParameter.ActualName = Encoding.Name;
        }
      }
    }

    #endregion
  }
}

