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
using System.Drawing;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.OneMax {
  [Item("OneMax Problem", "Represents a OneMax Problem.")]
  [Creatable("Problems")]
  [StorableClass]
  public sealed class OneMaxProblem : ParameterizedNamedItem, ISingleObjectiveProblem, IStorableContent {
    public string Filename { get; set; }

    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Type; }
    }

    #region Parameter Properties
    public ValueParameter<BoolValue> MaximizationParameter {
      get { return (ValueParameter<BoolValue>)Parameters["Maximization"]; }
    }
    IParameter ISingleObjectiveProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    public ValueParameter<IntValue> LengthParameter {
      get { return (ValueParameter<IntValue>)Parameters["Length"]; }
    }
    public ValueParameter<IBinaryVectorCreator> SolutionCreatorParameter {
      get { return (ValueParameter<IBinaryVectorCreator>)Parameters["SolutionCreator"]; }
    }
    IParameter IProblem.SolutionCreatorParameter {
      get { return SolutionCreatorParameter; }
    }
    public ValueParameter<IOneMaxEvaluator> EvaluatorParameter {
      get { return (ValueParameter<IOneMaxEvaluator>)Parameters["Evaluator"]; }
    }
    IParameter IProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }
    public ValueParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ValueParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    IParameter ISingleObjectiveProblem.BestKnownQualityParameter {
      get { return BestKnownQualityParameter; }
    }
    #endregion

    #region Properties
    public IntValue Length {
      get { return LengthParameter.Value; }
      set { LengthParameter.Value = value; }
    }
    public IBinaryVectorCreator SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
      set { SolutionCreatorParameter.Value = value; }
    }
    ISolutionCreator IProblem.SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
    }
    public IOneMaxEvaluator Evaluator {
      get { return EvaluatorParameter.Value; }
      set { EvaluatorParameter.Value = value; }
    }
    ISingleObjectiveEvaluator ISingleObjectiveProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    IEvaluator IProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    public DoubleValue BestKnownQuality {
      get { return BestKnownQualityParameter.Value; }
    }
    public IEnumerable<IOperator> Operators {
      get { return operators.Cast<IOperator>(); }
    }
    private BestOneMaxSolutionAnalyzer BestOneMaxSolutionAnalyzer {
      get { return operators.OfType<BestOneMaxSolutionAnalyzer>().FirstOrDefault(); }
    }
    #endregion

    [Storable]
    private List<IOperator> operators;

    [StorableConstructor]
    private OneMaxProblem(bool deserializing) : base(deserializing) { }
    private OneMaxProblem(OneMaxProblem original, Cloner cloner)
      : base(original, cloner) {
      operators = original.operators.Select(x => (IOperator)cloner.Clone(x)).ToList();
      AttachEventHandlers();
    }
    public OneMaxProblem()
      : base() {
      RandomBinaryVectorCreator creator = new RandomBinaryVectorCreator();
      OneMaxEvaluator evaluator = new OneMaxEvaluator();

      Parameters.Add(new ValueParameter<BoolValue>("Maximization", "Set to true as the OneMax Problem is a maximization problem.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<IntValue>("Length", "The length of the BinaryVector.", new IntValue(5)));
      Parameters.Add(new ValueParameter<IBinaryVectorCreator>("SolutionCreator", "The operator which should be used to create new OneMax solutions.", creator));
      Parameters.Add(new ValueParameter<IOneMaxEvaluator>("Evaluator", "The operator which should be used to evaluate OneMax solutions.", evaluator));
      Parameters.Add(new ValueParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this OneMax instance.", new DoubleValue(5)));

      creator.BinaryVectorParameter.ActualName = "OneMaxSolution";
      evaluator.QualityParameter.ActualName = "NumberOfOnes";
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();

      InitializeOperators();
      AttachEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OneMaxProblem(this, cloner);
    }

    #region Events
    public event EventHandler SolutionCreatorChanged;
    private void OnSolutionCreatorChanged() {
      EventHandler handler = SolutionCreatorChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler EvaluatorChanged;
    private void OnEvaluatorChanged() {
      EventHandler handler = EvaluatorChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler OperatorsChanged;
    private void OnOperatorsChanged() {
      EventHandler handler = OperatorsChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Reset;
    private void OnReset() {
      EventHandler handler = Reset;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    private void SolutionCreatorParameter_ValueChanged(object sender, EventArgs e) {
      SolutionCreator.BinaryVectorParameter.ActualNameChanged += new EventHandler(SolutionCreator_BinaryVectorParameter_ActualNameChanged);
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
      ParameterizeOperators();
      OnSolutionCreatorChanged();
    }
    private void SolutionCreator_BinaryVectorParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
      ParameterizeOperators();
    }
    private void EvaluatorParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
      OnEvaluatorChanged();
    }
    void LengthParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeSolutionCreator();
      LengthParameter.Value.ValueChanged += new EventHandler(Length_ValueChanged);
      BestKnownQualityParameter.Value.Value = Length.Value;
    }
    void Length_ValueChanged(object sender, EventArgs e) {
      BestKnownQualityParameter.Value.Value = Length.Value;
    }
    void BestKnownQualityParameter_ValueChanged(object sender, EventArgs e) {
      BestKnownQualityParameter.Value.Value = Length.Value;
    }
    void OneBitflipMoveParameter_ActualNameChanged(object sender, EventArgs e) {
      string name = ((ILookupParameter<OneBitflipMove>)sender).ActualName;
      foreach (IOneBitflipMoveOperator op in Operators.OfType<IOneBitflipMoveOperator>()) {
        op.OneBitflipMoveParameter.ActualName = name;
      }
    }
    #endregion

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code (remove with 3.4)
      if (operators == null) InitializeOperators();
      #endregion
      AttachEventHandlers();
    }

    private void AttachEventHandlers() {
      SolutionCreatorParameter.ValueChanged += new EventHandler(SolutionCreatorParameter_ValueChanged);
      SolutionCreator.BinaryVectorParameter.ActualNameChanged += new EventHandler(SolutionCreator_BinaryVectorParameter_ActualNameChanged);
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
      LengthParameter.ValueChanged += new EventHandler(LengthParameter_ValueChanged);
      LengthParameter.Value.ValueChanged += new EventHandler(Length_ValueChanged);
      BestKnownQualityParameter.ValueChanged += new EventHandler(BestKnownQualityParameter_ValueChanged);
    }

    private void ParameterizeSolutionCreator() {
      SolutionCreator.LengthParameter.ActualName = LengthParameter.Name;
    }
    private void ParameterizeEvaluator() {
      if (Evaluator is OneMaxEvaluator)
        ((OneMaxEvaluator)Evaluator).BinaryVectorParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
    }
    private void ParameterizeAnalyzer() {
      BestOneMaxSolutionAnalyzer.MaximizationParameter.ActualName = MaximizationParameter.Name;
      BestOneMaxSolutionAnalyzer.BestKnownQualityParameter.ActualName = BestKnownQualityParameter.Name;
      BestOneMaxSolutionAnalyzer.BinaryVectorParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
      BestOneMaxSolutionAnalyzer.ResultsParameter.ActualName = "Results";
    }
    private void InitializeOperators() {
      operators = new List<IOperator>();
      operators.Add(new BestOneMaxSolutionAnalyzer());
      ParameterizeAnalyzer();
      foreach (IBinaryVectorOperator op in ApplicationManager.Manager.GetInstances<IBinaryVectorOperator>()) {
        if (!(op is ISingleObjectiveMoveEvaluator) || (op is IOneMaxMoveEvaluator)) {
          operators.Add(op);
        }
      }
      ParameterizeOperators();
      InitializeMoveGenerators();
    }
    private void InitializeMoveGenerators() {
      foreach (IOneBitflipMoveOperator op in Operators.OfType<IOneBitflipMoveOperator>()) {
        if (op is IMoveGenerator) {
          op.OneBitflipMoveParameter.ActualNameChanged += new EventHandler(OneBitflipMoveParameter_ActualNameChanged);
        }
      }
    }
    private void ParameterizeOperators() {
      foreach (IBinaryVectorCrossover op in Operators.OfType<IBinaryVectorCrossover>()) {
        op.ParentsParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
        op.ChildParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
      }
      foreach (IBinaryVectorManipulator op in Operators.OfType<IBinaryVectorManipulator>()) {
        op.BinaryVectorParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
      }
      foreach (IBinaryVectorMoveOperator op in Operators.OfType<IBinaryVectorMoveOperator>()) {
        op.BinaryVectorParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
      }
    }
    #endregion
  }
}
