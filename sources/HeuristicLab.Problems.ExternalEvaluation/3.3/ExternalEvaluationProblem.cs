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
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("External Evaluation Problem", "A problem that is evaluated in a different process.")]
  [StorableClass]
  public class ExternalEvaluationProblem : ParameterizedNamedItem, ISingleObjectiveProblem {
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
    private IValueParameter<IExternalEvaluationDriver> DriverParameter {
      get { return (IValueParameter<IExternalEvaluationDriver>)Parameters["Driver"]; }
    }
    private IValueParameter<IExternalEvaluationProblemEvaluator> EvaluatorParameter {
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
    private BestExternalEvaluationSolutionAnalyzer BestExternalEvaluationSolutionAnalyzer {
      get { return OperatorsParameter.Value.OfType<BestExternalEvaluationSolutionAnalyzer>().FirstOrDefault(); }
    }
    #endregion

    [StorableConstructor]
    protected ExternalEvaluationProblem(bool deserializing) : base(deserializing) { }
    public ExternalEvaluationProblem()
      : base() {
      ExternalEvaluator evaluator = new ExternalEvaluator();
      ExternalEvaluationSolutionCreator solutionCreator = new ExternalEvaluationSolutionCreator();

      Parameters.Add(new ValueParameter<IExternalEvaluationDriver>("Driver", "The communication driver that is used to exchange data with the external process."));
      Parameters.Add(new ValueParameter<IExternalEvaluationProblemEvaluator>("Evaluator", "The evaluator that collects the values to exchange.", evaluator));
      Parameters.Add(new ValueParameter<IOperator>("SolutionCreator", "An operator to create the solution components.", solutionCreator));
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
      OnEvaluatorChanged();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      // TODO: Following code is not yet approved behavior, but it's the best effort I can make here regarding wiring
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

    #region Helpers
    private void InitializeOperators() {
      ItemList<IOperator> operators = OperatorsParameter.Value;
      operators.Add(new BestExternalEvaluationSolutionAnalyzer());
      ParameterizeAnalyzers();
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AttachEventHandlers() {
      SolutionCreatorParameter.ValueChanged += new EventHandler(SolutionCreatorParameter_ValueChanged);
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      OperatorsParameter.ValueChanged += new System.EventHandler(OperatorsParameter_ValueChanged);
      OperatorsParameter.Value.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(OperatorsParameter_Value_ItemsAdded);
      OperatorsParameter.Value.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(OperatorsParameter_Value_ItemsRemoved);
      OperatorsParameter.Value.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(OperatorsParameter_Value_CollectionReset);
    }
    private void ParameterizeAnalyzers() {
      BestExternalEvaluationSolutionAnalyzer.ResultsParameter.ActualName = "Results";
      BestExternalEvaluationSolutionAnalyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
      BestExternalEvaluationSolutionAnalyzer.BestKnownQualityParameter.ActualName = BestKnownQualityParameter.Name;
      BestExternalEvaluationSolutionAnalyzer.BestKnownSolutionParameter.ActualName = BestKnownSolutionParameter.Name;
      BestExternalEvaluationSolutionAnalyzer.MaximizationParameter.ActualName = MaximizationParameter.Name;
    }
    private void ParameterizeEvaluator() {
      Evaluator.DriverParameter.ActualName = DriverParameter.Name;
    }
    #endregion
  }
}
