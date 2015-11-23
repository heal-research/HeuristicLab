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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.ParameterOptimization {
  [Item("Parameter Optimization Problem", "A base class for other problems for the optimization of a parameter vector.")]
  [StorableClass]
  public abstract class ParameterOptimizationProblem : SingleObjectiveHeuristicOptimizationProblem<IParameterVectorEvaluator, IRealVectorCreator>, IStorableContent {
    public string Filename { get; set; }
    private const string ProblemSizeParameterName = "ProblemSize";
    private const string BoundsParameterName = "Bounds";
    private const string ParameterNamesParameterName = "ParameterNames";


    #region parameters
    public IFixedValueParameter<IntValue> ProblemSizeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[ProblemSizeParameterName]; }
    }
    public IValueParameter<DoubleMatrix> BoundsParameter {
      get { return (IValueParameter<DoubleMatrix>)Parameters[BoundsParameterName]; }
    }
    public IValueParameter<StringArray> ParameterNamesParameter {
      get { return (IValueParameter<StringArray>)Parameters[ParameterNamesParameterName]; }
    }
    #endregion

    #region properties
    public int ProblemSize {
      get { return ProblemSizeParameter.Value.Value; }
      set { ProblemSizeParameter.Value.Value = value; }
    }
    public DoubleMatrix Bounds {
      get { return BoundsParameter.Value; }
      set { BoundsParameter.Value = value; }
    }
    public StringArray ParameterNames {
      get { return ParameterNamesParameter.Value; }
      set { ParameterNamesParameter.Value = value; }
    }
    #endregion


    [Storable]
    protected StdDevStrategyVectorCreator strategyVectorCreator;
    [Storable]
    protected StdDevStrategyVectorCrossover strategyVectorCrossover;
    [Storable]
    protected StdDevStrategyVectorManipulator strategyVectorManipulator;

    [StorableConstructor]
    protected ParameterOptimizationProblem(bool deserializing) : base(deserializing) { }
    protected ParameterOptimizationProblem(ParameterOptimizationProblem original, Cloner cloner)
      : base(original, cloner) {
      strategyVectorCreator = cloner.Clone(original.strategyVectorCreator);
      strategyVectorCrossover = cloner.Clone(original.strategyVectorCrossover);
      strategyVectorManipulator = cloner.Clone(original.strategyVectorManipulator);
      RegisterEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    protected ParameterOptimizationProblem(IParameterVectorEvaluator evaluator)
      : base(evaluator, new UniformRandomRealVectorCreator()) {
      Parameters.Add(new FixedValueParameter<IntValue>(ProblemSizeParameterName, "The dimension of the parameter vector that is to be optimized.", new IntValue(1)));
      Parameters.Add(new ValueParameter<DoubleMatrix>(BoundsParameterName, "The bounds for each dimension of the parameter vector. If the number of bounds is smaller than the problem size then the bounds are reused in a cyclic manner.", new DoubleMatrix(new double[,] { { 0, 100 } }, new string[] { "LowerBound", "UpperBound" })));
      Parameters.Add(new ValueParameter<StringArray>(ParameterNamesParameterName, "The element names which are used to calculate the quality of a parameter vector.", new StringArray(new string[] { "Parameter0" })));

      SolutionCreator.LengthParameter.ActualName = "ProblemSize";

      Operators.AddRange(ApplicationManager.Manager.GetInstances<IRealVectorOperator>());

      strategyVectorCreator = new StdDevStrategyVectorCreator();
      strategyVectorCreator.LengthParameter.ActualName = ProblemSizeParameter.Name;
      strategyVectorCrossover = new StdDevStrategyVectorCrossover();
      strategyVectorManipulator = new StdDevStrategyVectorManipulator();
      strategyVectorManipulator.LearningRateParameter.Value = new DoubleValue(0.5);
      strategyVectorManipulator.GeneralLearningRateParameter.Value = new DoubleValue(0.5);

      Operators.Add(strategyVectorCreator);
      Operators.Add(strategyVectorCrossover);
      Operators.Add(strategyVectorManipulator);
      Operators.Add(new BestSolutionAnalyzer());
      Operators.Add(new BestSolutionsAnalyzer());
      UpdateParameters();
      UpdateStrategyVectorBounds();

      RegisterEventHandlers();
    }

    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      UpdateParameters();
    }

    private void RegisterEventHandlers() {
      Bounds.ToStringChanged += Bounds_ToStringChanged;
      ProblemSizeParameter.Value.ValueChanged += ProblemSize_Changed;
      ParameterNames.Reset += ParameterNames_Reset;
    }

    private void UpdateParameters() {
      Evaluator.ParameterVectorParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
      Evaluator.ParameterNamesParameter.ActualName = ParameterNamesParameter.Name;

      foreach (var bestSolutionAnalyzer in Operators.OfType<BestSolutionAnalyzer>()) {
        bestSolutionAnalyzer.ParameterVectorParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
        bestSolutionAnalyzer.ParameterNamesParameter.ActualName = ParameterNamesParameter.Name;
      }
      Bounds = new DoubleMatrix(ProblemSize, 2);
      Bounds.RowNames = ParameterNames;
      for (int i = 0; i < Bounds.Rows; i++) {
        Bounds[i, 0] = 0.0;
        Bounds[i, 1] = 100.0;
      }

      foreach (var op in Operators.OfType<IRealVectorManipulator>())
        op.RealVectorParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
    }

    private void Bounds_ToStringChanged(object sender, EventArgs e) {
      if (Bounds.Columns != 2 || Bounds.Rows < 1)
        Bounds = new DoubleMatrix(1, 2);
      UpdateStrategyVectorBounds();
    }
    protected virtual void UpdateStrategyVectorBounds() {
      DoubleMatrix strategyBounds = (DoubleMatrix)Bounds.Clone();
      for (int i = 0; i < strategyBounds.Rows; i++) {
        if (strategyBounds[i, 0] < 0) strategyBounds[i, 0] = 0;
        strategyBounds[i, 1] = 0.1 * (Bounds[i, 1] - Bounds[i, 0]);
      }
      strategyVectorCreator.BoundsParameter.Value = strategyBounds;
    }

    protected virtual void ProblemSize_Changed(object sender, EventArgs e) {
      if (ParameterNames.Length != ProblemSize)
        ((IStringConvertibleArray)ParameterNames).Length = ProblemSize;
      for (int i = 0; i < ParameterNames.Length; i++) {
        if (string.IsNullOrEmpty(ParameterNames[i])) ParameterNames[i] = "Parameter" + i;
      }

      strategyVectorManipulator.GeneralLearningRateParameter.Value = new DoubleValue(1.0 / Math.Sqrt(2 * ProblemSize));
      strategyVectorManipulator.LearningRateParameter.Value = new DoubleValue(1.0 / Math.Sqrt(2 * Math.Sqrt(ProblemSize)));
    }

    protected virtual void ParameterNames_Reset(object sender, EventArgs e) {
      ProblemSize = ParameterNames.Length;
    }
  }
}
