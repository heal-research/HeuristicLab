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
using HeuristicLab.Encodings.Permutation;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.TSP {
  [Item("TSP", "Represents a symmetric Traveling Salesman Problem.")]
  [Creatable("Problems")]
  [StorableClass]
  public sealed class TSP : ParameterizedNamedItem, ISingleObjectiveProblem {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Type; }
    }

    #region Parameter Properties
    public ValueParameter<BoolData> MaximizationParameter {
      get { return (ValueParameter<BoolData>)Parameters["Maximization"]; }
    }
    IParameter ISingleObjectiveProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    public ValueParameter<DoubleMatrixData> CoordinatesParameter {
      get { return (ValueParameter<DoubleMatrixData>)Parameters["Coordinates"]; }
    }
    public ValueParameter<IPermutationCreator> SolutionCreatorParameter {
      get { return (ValueParameter<IPermutationCreator>)Parameters["SolutionCreator"]; }
    }
    IParameter IProblem.SolutionCreatorParameter {
      get { return SolutionCreatorParameter; }
    }
    public ValueParameter<ITSPEvaluator> EvaluatorParameter {
      get { return (ValueParameter<ITSPEvaluator>)Parameters["Evaluator"]; }
    }
    IParameter IProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }
    public OptionalValueParameter<DoubleData> BestKnownQualityParameter {
      get { return (OptionalValueParameter<DoubleData>)Parameters["BestKnownQuality"]; }
    }
    #endregion

    #region Properties
    public DoubleMatrixData Coordinates {
      get { return CoordinatesParameter.Value; }
      set { CoordinatesParameter.Value = value; }
    }
    public IPermutationCreator SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
      set { SolutionCreatorParameter.Value = value; }
    }
    ISolutionCreator IProblem.SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
    }
    public ITSPEvaluator Evaluator {
      get { return EvaluatorParameter.Value; }
      set { EvaluatorParameter.Value = value; }
    }
    ISingleObjectiveEvaluator ISingleObjectiveProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    IEvaluator IProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    public DoubleData BestKnownQuality {
      get { return BestKnownQualityParameter.Value; }
      set { BestKnownQualityParameter.Value = value; }
    }
    private List<IPermutationOperator> operators;
    public IEnumerable<IOperator> Operators {
      get { return operators.Cast<IOperator>(); }
    }
    #endregion

    public TSP()
      : base() {
      RandomPermutationCreator creator = new RandomPermutationCreator();
      TSPRoundedEuclideanPathEvaluator evaluator = new TSPRoundedEuclideanPathEvaluator();

      Parameters.Add(new ValueParameter<BoolData>("Maximization", "Set to false as the Traveling Salesman Problem is a minimization problem.", new BoolData(false)));
      Parameters.Add(new ValueParameter<DoubleMatrixData>("Coordinates", "The x- and y-Coordinates of the cities.", new DoubleMatrixData(0, 0)));
      Parameters.Add(new ValueParameter<IPermutationCreator>("SolutionCreator", "The operator which should be used to create new TSP solutions.", creator));
      Parameters.Add(new ValueParameter<ITSPEvaluator>("Evaluator", "The operator which should be used to evaluate TSP solutions.", evaluator));
      Parameters.Add(new OptionalValueParameter<DoubleData>("BestKnownQuality", "The quality of the best known solution of this TSP instance."));

      creator.PermutationParameter.ActualName = "TSPTour";
      evaluator.QualityParameter.ActualName = "TSPTourLength";
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();

      Initialize();
    }
    [StorableConstructor]
    private TSP(bool deserializing) : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      TSP clone = (TSP)base.Clone(cloner);
      clone.Initialize();
      return clone;
    }

    public void ImportFromTSPLIB(string filename) {
      TSPLIBParser parser = new TSPLIBParser(filename);
      parser.Parse();
      Coordinates = new DoubleMatrixData(parser.Vertices);
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
    public event EventHandler OperatorsChanged;
    private void OnOperatorsChanged() {
      if (OperatorsChanged != null)
        OperatorsChanged(this, EventArgs.Empty);
    }

    private void CoordinatesParameter_ValueChanged(object sender, EventArgs e) {
      Coordinates.ItemChanged += new EventHandler<EventArgs<int, int>>(Coordinates_ItemChanged);
      Coordinates.Reset += new EventHandler(Coordinates_Reset);
      ParameterizeSolutionCreator();
    }
    private void Coordinates_ItemChanged(object sender, EventArgs<int, int> e) {
    }
    private void Coordinates_Reset(object sender, EventArgs e) {
      ParameterizeSolutionCreator();
    }
    private void SolutionCreatorParameter_ValueChanged(object sender, EventArgs e) {
      SolutionCreator.PermutationParameter.ActualNameChanged += new EventHandler(SolutionCreator_PermutationParameter_ActualNameChanged);
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      ParameterizeOperators();
      OnSolutionCreatorChanged();
    }
    private void SolutionCreator_PermutationParameter_ActualNameChanged(object sender, EventArgs e) {
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
      CoordinatesParameter.ValueChanged += new EventHandler(CoordinatesParameter_ValueChanged);
      Coordinates.ItemChanged += new EventHandler<EventArgs<int, int>>(Coordinates_ItemChanged);
      Coordinates.Reset += new EventHandler(Coordinates_Reset);
      SolutionCreatorParameter.ValueChanged += new EventHandler(SolutionCreatorParameter_ValueChanged);
      SolutionCreator.PermutationParameter.ActualNameChanged += new EventHandler(SolutionCreator_PermutationParameter_ActualNameChanged);
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
    }
    private void ParameterizeSolutionCreator() {
      SolutionCreator.LengthParameter.Value = new IntData(Coordinates.Rows);
    }
    private void ParameterizeEvaluator() {
      if (Evaluator is ITSPPathEvaluator)
        ((ITSPPathEvaluator)Evaluator).PermutationParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
      if (Evaluator is ITSPCoordinatesPathEvaluator)
        ((ITSPCoordinatesPathEvaluator)Evaluator).CoordinatesParameter.ActualName = CoordinatesParameter.Name;
    }
    private void InitializeOperators() {
      operators = new List<IPermutationOperator>();
      if (ApplicationManager.Manager != null) {
        operators.AddRange(ApplicationManager.Manager.GetInstances<IPermutationOperator>());
        ParameterizeOperators();
      }
    }
    private void ParameterizeOperators() {
      foreach (IPermutationCrossover op in Operators.OfType<IPermutationCrossover>()) {
        op.ParentsParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
        op.ChildParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
      }
      foreach (IPermutationManipulator op in Operators.OfType<IPermutationManipulator>()) {
        op.PermutationParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
      }
    }
    #endregion
  }
}
