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
  [Item("OneMax", "Represents a OneMax Problem.")]
  [Creatable("Problems")]
  [StorableClass]
  public sealed class OneMax : ParameterizedNamedItem, ISingleObjectiveProblem {
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
    #endregion

    #region Properties
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
    private List<IBinaryVectorOperator> operators;
    public IEnumerable<IOperator> Operators {
      get { return operators.Cast<IOperator>(); }
    }
    #endregion

    public OneMax()
      : base() {
      RandomBinaryVectorCreator creator = new RandomBinaryVectorCreator();
      OneMaxEvaluator evaluator = new OneMaxEvaluator();

      Parameters.Add(new ValueParameter<BoolValue>("Maximization", "Set to true as the OneMax Problem is a maximization problem.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<IntValue>("Length", "The length of the BinaryVector.", new IntValue(5)));
      Parameters.Add(new ValueParameter<IBinaryVectorCreator>("SolutionCreator", "The operator which should be used to create new OneMax solutions.", creator));
      Parameters.Add(new ValueParameter<IOneMaxEvaluator>("Evaluator", "The operator which should be used to evaluate OneMax solutions.", evaluator));

      creator.BinaryVectorParameter.ActualName = "OneMaxSolution";
      evaluator.QualityParameter.ActualName = "NumberOfOnes";
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();

      Initialize();
    }

    [StorableConstructor]
    private OneMax(bool deserializing) : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      OneMax clone = (OneMax)base.Clone(cloner);
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
    public event EventHandler OperatorsChanged;
    private void OnOperatorsChanged() {
      if (OperatorsChanged != null)
        OperatorsChanged(this, EventArgs.Empty);
    }

    private void SolutionCreatorParameter_ValueChanged(object sender, EventArgs e) {
      SolutionCreator.BinaryVectorParameter.ActualNameChanged += new EventHandler(SolutionCreator_PermutationParameter_ActualNameChanged);
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
      SolutionCreatorParameter.ValueChanged += new EventHandler(SolutionCreatorParameter_ValueChanged);
      SolutionCreator.BinaryVectorParameter.ActualNameChanged += new EventHandler(SolutionCreator_PermutationParameter_ActualNameChanged);
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
    }
    private void ParameterizeSolutionCreator() {
      SolutionCreator.LengthParameter.Value = LengthParameter.Value;
    }
    private void ParameterizeEvaluator() {
      if (Evaluator is OneMaxEvaluator)
        ((OneMaxEvaluator)Evaluator).BinaryVectorParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
    }
    private void InitializeOperators() {
      operators = new List<IBinaryVectorOperator>();
      if (ApplicationManager.Manager != null) {
        operators.AddRange(ApplicationManager.Manager.GetInstances<IBinaryVectorOperator>());
        ParameterizeOperators();
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
    }
    #endregion
  }
}
