#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.ArtificialAnt {
  [Item("ArtificialAntProblem", "Represents the Artificial Ant problem.")]
  [Creatable("Problems")]
  [StorableClass]
  public sealed class ArtificialAntProblem : ParameterizedNamedItem, ISingleObjectiveProblem {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Type; }
    }

    #region Parameter Properties
    public ValueParameter<BoolValue> MaximizationParameter {
      get { return (ValueParameter<BoolValue>)Parameters["Maximization"]; }
    }
    IParameter ISingleObjectiveProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    public ValueParameter<SymbolicExpressionTreeCreator> SolutionCreatorParameter {
      get { return (ValueParameter<SymbolicExpressionTreeCreator>)Parameters["SolutionCreator"]; }
    }
    IParameter IProblem.SolutionCreatorParameter {
      get { return SolutionCreatorParameter; }
    }
    public ValueParameter<Evaluator> EvaluatorParameter {
      get { return (ValueParameter<Evaluator>)Parameters["Evaluator"]; }
    }
    IParameter IProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }
    public ValueParameter<ISymbolicExpressionGrammar> ArtificialAntExpressionGrammarParameter {
      get { return (ValueParameter<ISymbolicExpressionGrammar>)Parameters["ArtificialAntExpressionGrammar"]; }
    }
    public ValueParameter<IntValue> MaxExpressionLengthParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaxExpressionLength"]; }
    }
    public ValueParameter<IntValue> MaxExpressionDepthParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaxExpressionDepth"]; }
    }

    public OptionalValueParameter<IAntTrailVisualizer> VisualizerParameter {
      get { return (OptionalValueParameter<IAntTrailVisualizer>)Parameters["Visualizer"]; }
    }
    IParameter IProblem.VisualizerParameter {
      get { return VisualizerParameter; }
    }
    public ValueParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ValueParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    IParameter ISingleObjectiveProblem.BestKnownQualityParameter {
      get { return BestKnownQualityParameter; }
    }
    #endregion

    #region Properties
    public SymbolicExpressionTreeCreator SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
      set { SolutionCreatorParameter.Value = value; }
    }
    ISolutionCreator IProblem.SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
    }
    public Evaluator Evaluator {
      get { return EvaluatorParameter.Value; }
      set { EvaluatorParameter.Value = value; }
    }
    ISingleObjectiveEvaluator ISingleObjectiveProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    IEvaluator IProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    public ISymbolicExpressionGrammar ArtificialAntExpressionGrammar {
      get { return ArtificialAntExpressionGrammarParameter.Value; }
    }
    public IAntTrailVisualizer Visualizer {
      get { return VisualizerParameter.Value; }
      set { VisualizerParameter.Value = value; }
    }
    ISolutionsVisualizer IProblem.Visualizer {
      get { return VisualizerParameter.Value; }
    }
    public DoubleValue BestKnownQuality {
      get { return BestKnownQualityParameter.Value; }
    }
    private List<IOperator> operators;
    public IEnumerable<IOperator> Operators {
      get { return operators; }
    }
    #endregion

    public ArtificialAntProblem()
      : base() {
      SymbolicExpressionTreeCreator creator = new ProbabilisticTreeCreator();
      Evaluator evaluator = new Evaluator();
      ArtificialAntExpressionGrammar grammar = new ArtificialAntExpressionGrammar();
      BestAntTrailVisualizer visualizer = new BestAntTrailVisualizer();

      Parameters.Add(new ValueParameter<BoolValue>("Maximization", "Set to true as the Artificial Ant Problem is a maximization problem.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<SymbolicExpressionTreeCreator>("SolutionCreator", "The operator which should be used to create new artificial ant solutions.", creator));
      Parameters.Add(new ValueParameter<Evaluator>("Evaluator", "The operator which should be used to evaluate artificial ant solutions.", evaluator));
      Parameters.Add(new ValueParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this OneMax instance.", new DoubleValue(89)));
      Parameters.Add(new ValueParameter<ISymbolicExpressionGrammar>("ArtificialAntExpressionGrammar", "The grammar that should be used for artificial ant expressions.", grammar));
      Parameters.Add(new ValueParameter<IntValue>("MaxExpressionLength", "Maximal length of the expression to control the artificial ant.", new IntValue(100)));
      Parameters.Add(new ValueParameter<IntValue>("MaxExpressionDepth", "Maximal depth of the expression to control the artificial ant.", new IntValue(10)));
      Parameters.Add(new ValueParameter<IAntTrailVisualizer>("Visualizer", "The operator which should be used to visualize artificial ant solutions.", visualizer));

      creator.SymbolicExpressionTreeParameter.ActualName = "AntTrailSolution";
      evaluator.QualityParameter.ActualName = "FoodEaten";
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      ParameterizeVisualizer();

      Initialize();
    }

    [StorableConstructor]
    private ArtificialAntProblem(bool deserializing) : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      ArtificialAntProblem clone = (ArtificialAntProblem)base.Clone(cloner);
      clone.Initialize();
      return clone;
    }

    #region Events
    public event EventHandler SolutionCreatorChanged;
    private void OnSolutionCreatorChanged() {
      if (SolutionCreatorChanged != null)
        SolutionCreatorChanged(this, EventArgs.Empty);
    }
    public event EventHandler EvaluatorChanged;
    private void OnEvaluatorChanged() {
      if (EvaluatorChanged != null)
        EvaluatorChanged(this, EventArgs.Empty);
    }
    public event EventHandler VisualizerChanged;
    private void OnVisualizerChanged() {
      if (VisualizerChanged != null)
        VisualizerChanged(this, EventArgs.Empty);
    }

    public event EventHandler OperatorsChanged;
    private void OnOperatorsChanged() {
      if (OperatorsChanged != null)
        OperatorsChanged(this, EventArgs.Empty);
    }

    private void SolutionCreatorParameter_ValueChanged(object sender, EventArgs e) {
      SolutionCreator.SymbolicExpressionTreeParameter.ActualNameChanged += new EventHandler(SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged);
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      ParameterizeOperators();
      OnSolutionCreatorChanged();
    }
    private void SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeOperators();
    }
    private void EvaluatorParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      OnEvaluatorChanged();
    }
    #endregion

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      InitializeOperators();
      SolutionCreatorParameter.ValueChanged += new EventHandler(SolutionCreatorParameter_ValueChanged);
      SolutionCreator.SymbolicExpressionTreeParameter.ActualNameChanged += new EventHandler(SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged);
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
    }
    private void ParameterizeSolutionCreator() {
      SolutionCreator.SymbolicExpressionGrammarParameter.ActualName = ArtificialAntExpressionGrammarParameter.Name;
    }
    private void ParameterizeEvaluator() {
      Evaluator.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
    }
    private void InitializeOperators() {
      operators = new List<IOperator>();
      operators.Add(Evaluator);
      operators.Add(SolutionCreator);
      operators.Add(new SubtreeCrossover());
      ParameterizeOperators();
    }

    private void ParameterizeOperators() {
      foreach (ProbabilisticTreeCreator op in Operators.OfType<ProbabilisticTreeCreator>()) {
        op.MaxTreeHeightParameter.ActualName = MaxExpressionDepthParameter.Name;
        op.MaxTreeSizeParameter.ActualName = MaxExpressionLengthParameter.Name;
        op.SymbolicExpressionGrammarParameter.ActualName = ArtificialAntExpressionGrammarParameter.Name;
      }
      foreach (Evaluator op in Operators.OfType<Evaluator>()) {
        op.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      }
      foreach (SymbolicExpressionTreeCrossover op in Operators.OfType<SymbolicExpressionTreeCrossover>()) {
        op.MaxTreeHeightParameter.ActualName = MaxExpressionDepthParameter.Name;
        op.MaxTreeSizeParameter.ActualName = MaxExpressionLengthParameter.Name;
        op.SymbolicExpressionGrammarParameter.ActualName = ArtificialAntExpressionGrammarParameter.Name;
        op.ParentsParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        op.ChildParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      }
    }
    private void ParameterizeVisualizer() {
      if (Visualizer != null) {
        Visualizer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        if (Visualizer is IAntTrailVisualizer)
          ((IAntTrailVisualizer)Visualizer).SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      }
    }

    #endregion
  }
}
