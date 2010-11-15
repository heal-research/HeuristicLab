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
using HeuristicLab.Analysis;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("External Evaluation Problem", "A problem that is evaluated in a different process.")]
  [Creatable("Problems")]
  [StorableClass]
  public sealed class ExternalEvaluationProblem : ParameterizedNamedItem, ISingleObjectiveProblem, IStorableContent {
    public string Filename { get; set; }

    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Type; }
    }

    public new ParameterCollection Parameters {
      get { return base.Parameters; }
    }
    IKeyedItemCollection<string, IParameter> IParameterizedItem.Parameters {
      get { return Parameters; }
    }

    #region Parameters
    public IValueParameter<IEvaluationServiceClient> ClientParameter {
      get { return (IValueParameter<IEvaluationServiceClient>)Parameters["Client"]; }
    }
    public IValueParameter<IExternalEvaluationProblemEvaluator> EvaluatorParameter {
      get { return (IValueParameter<IExternalEvaluationProblemEvaluator>)Parameters["Evaluator"]; }
    }
    public ValueParameter<BoolValue> MaximizationParameter {
      get { return (ValueParameter<BoolValue>)Parameters["Maximization"]; }
    }
    IParameter ISingleObjectiveProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    public ValueParameter<ISolutionCreator> SolutionCreatorParameter {
      get { return (ValueParameter<ISolutionCreator>)Parameters["SolutionCreator"]; }
    }
    IParameter IProblem.SolutionCreatorParameter {
      get { return SolutionCreatorParameter; }
    }
    IParameter IProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }
    public OptionalValueParameter<DoubleValue> BestKnownQualityParameter {
      get { return (OptionalValueParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    IParameter ISingleObjectiveProblem.BestKnownQualityParameter {
      get { return BestKnownQualityParameter; }
    }
    public OptionalValueParameter<IScope> BestKnownSolutionParameter {
      get { return (OptionalValueParameter<IScope>)Parameters["BestKnownSolution"]; }
    }
    public ValueParameter<ItemList<IOperator>> OperatorsParameter {
      get { return (ValueParameter<ItemList<IOperator>>)Parameters["Operators"]; }
    }
    #endregion

    #region Properties
    public BoolValue Maximization {
      get { return MaximizationParameter.Value; }
      set { MaximizationParameter.Value = value; }
    }
    public ISolutionCreator SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
      set { SolutionCreatorParameter.Value = value; }
    }
    ISolutionCreator IProblem.SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
    }
    public IExternalEvaluationProblemEvaluator Evaluator {
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
      set { BestKnownQualityParameter.Value = value; }
    }
    public IEnumerable<IOperator> Operators {
      get { return OperatorsParameter.Value; }
    }
    private BestScopeSolutionAnalyzer BestScopeSolutionAnalyzer {
      get { return OperatorsParameter.Value.OfType<BestScopeSolutionAnalyzer>().FirstOrDefault(); }
    }
    #endregion

    [StorableConstructor]
    private ExternalEvaluationProblem(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserializationHook() {
      AttachEventHandlers();
    }

    private ExternalEvaluationProblem(ExternalEvaluationProblem original, Cloner cloner)
      : base(original, cloner) {
      AttachEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExternalEvaluationProblem(this, cloner);
    }
    public ExternalEvaluationProblem()
      : base() {
      ExternalEvaluator evaluator = new ExternalEvaluator();
      UserDefinedSolutionCreator solutionCreator = new UserDefinedSolutionCreator();

      Parameters.Add(new ValueParameter<IEvaluationServiceClient>("Client", "The client that is used to communicate with the external application.", new EvaluationServiceClient()));
      Parameters.Add(new ValueParameter<IExternalEvaluationProblemEvaluator>("Evaluator", "The evaluator that collects the values to exchange.", evaluator));
      Parameters.Add(new ValueParameter<ISolutionCreator>("SolutionCreator", "An operator to create the solution components.", solutionCreator));
      Parameters.Add(new ValueParameter<BoolValue>("Maximization", "Set to false as most test functions are minimization problems.", new BoolValue(false)));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this problem."));
      Parameters.Add(new OptionalValueParameter<IScope>("BestKnownSolution", "The best known solution for this external evaluation problem."));
      Parameters.Add(new ValueParameter<ItemList<IOperator>>("Operators", "The operators that are passed to the algorithm.", new ItemList<IOperator>()));

      InitializeOperators();
      AttachEventHandlers();
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
      OnSolutionCreatorChanged();
    }
    private void EvaluatorParameter_ValueChanged(object sender, EventArgs e) {
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      ParameterizeOperators();
      OnEvaluatorChanged();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeOperators();
    }
    private void OperatorsParameter_ValueChanged(object sender, EventArgs e) {
      OnOperatorsChanged();
    }
    private void OperatorsParameter_Value_ItemsAdded(object sender, EventArgs e) {
      OnOperatorsChanged();
    }
    private void OperatorsParameter_Value_ItemsRemoved(object sender, EventArgs e) {
      OnOperatorsChanged();
    }
    private void OperatorsParameter_Value_CollectionReset(object sender, EventArgs e) {
      OnOperatorsChanged();
    }
    #endregion

    #region Helper
    private void AttachEventHandlers() {
      SolutionCreatorParameter.ValueChanged += new EventHandler(SolutionCreatorParameter_ValueChanged);
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      OperatorsParameter.ValueChanged += new EventHandler(OperatorsParameter_ValueChanged);
      OperatorsParameter.Value.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(OperatorsParameter_Value_ItemsAdded);
      OperatorsParameter.Value.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(OperatorsParameter_Value_ItemsRemoved);
      OperatorsParameter.Value.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(OperatorsParameter_Value_CollectionReset);
    }
    private void InitializeOperators() {
      ItemList<IOperator> operators = OperatorsParameter.Value;
      operators.Add(new BestScopeSolutionAnalyzer());
      ParameterizeAnalyzers();
    }
    private void ParameterizeAnalyzers() {
      BestScopeSolutionAnalyzer.ResultsParameter.ActualName = "Results";
      BestScopeSolutionAnalyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
      BestScopeSolutionAnalyzer.BestKnownQualityParameter.ActualName = BestKnownQualityParameter.Name;
      BestScopeSolutionAnalyzer.BestKnownSolutionParameter.ActualName = BestKnownSolutionParameter.Name;
      BestScopeSolutionAnalyzer.MaximizationParameter.ActualName = MaximizationParameter.Name;
    }
    private void ParameterizeEvaluator() {
      Evaluator.ClientParameter.ActualName = ClientParameter.Name;
    }
    private void ParameterizeOperators() {
      // This is a best effort approach to wiring
      string qualityName = Evaluator.QualityParameter.ActualName;
      foreach (IOperator op in OperatorsParameter.Value) {
        foreach (ILookupParameter<DoubleValue> param in op.Parameters.OfType<ILookupParameter<DoubleValue>>()) {
          if (param.Name.Equals("Quality")) param.ActualName = qualityName;
        }
        foreach (IScopeTreeLookupParameter<DoubleValue> param in op.Parameters.OfType<IScopeTreeLookupParameter<DoubleValue>>()) {
          if (param.Name.Equals("Quality")) param.ActualName = qualityName;
        }
      }
    }
    #endregion
  }
}
