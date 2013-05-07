#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public sealed class OneMaxProblem : SingleObjectiveHeuristicOptimizationProblem<IOneMaxEvaluator, IBinaryVectorCreator>, IStorableContent {
    public string Filename { get; set; }

    #region Parameter Properties
    public ValueParameter<IntValue> LengthParameter {
      get { return (ValueParameter<IntValue>)Parameters["Length"]; }
    }
    #endregion

    #region Properties
    public IntValue Length {
      get { return LengthParameter.Value; }
      set { LengthParameter.Value = value; }
    }
    private BestOneMaxSolutionAnalyzer BestOneMaxSolutionAnalyzer {
      get { return Operators.OfType<BestOneMaxSolutionAnalyzer>().FirstOrDefault(); }
    }
    #endregion

    // BackwardsCompatibility3.3
    #region Backwards compatible code, remove with 3.4
    [Obsolete]
    [Storable(Name = "operators")]
    private IEnumerable<IOperator> oldOperators {
      get { return null; }
      set {
        if (value != null && value.Any())
          Operators.AddRange(value);
      }
    }
    #endregion

    [StorableConstructor]
    private OneMaxProblem(bool deserializing) : base(deserializing) { }
    private OneMaxProblem(OneMaxProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    public OneMaxProblem()
      : base(new OneMaxEvaluator(), new RandomBinaryVectorCreator()) {
      Parameters.Add(new ValueParameter<IntValue>("Length", "The length of the BinaryVector.", new IntValue(5)));

      Maximization.Value = true;
      MaximizationParameter.Hidden = true;
      BestKnownQuality = new DoubleValue(5);

      SolutionCreator.BinaryVectorParameter.ActualName = "OneMaxSolution";
      Evaluator.QualityParameter.ActualName = "NumberOfOnes";
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();

      InitializeOperators();
      RegisterEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OneMaxProblem(this, cloner);
    }

    #region Events
    protected override void OnSolutionCreatorChanged() {
      base.OnSolutionCreatorChanged();
      SolutionCreator.BinaryVectorParameter.ActualNameChanged += new EventHandler(SolutionCreator_BinaryVectorParameter_ActualNameChanged);
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
      ParameterizeOperators();
    }
    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
    }
    private void SolutionCreator_BinaryVectorParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
      ParameterizeOperators();
    }
    private void LengthParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeSolutionCreator();
      LengthParameter.Value.ValueChanged += new EventHandler(Length_ValueChanged);
      BestKnownQualityParameter.Value.Value = Length.Value;
    }
    private void Length_ValueChanged(object sender, EventArgs e) {
      BestKnownQualityParameter.Value.Value = Length.Value;
    }
    private void BestKnownQualityParameter_ValueChanged(object sender, EventArgs e) {
      BestKnownQualityParameter.Value.Value = Length.Value;
    }
    private void OneBitflipMoveParameter_ActualNameChanged(object sender, EventArgs e) {
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
      if (Operators.Count == 0) InitializeOperators();
      #endregion
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      SolutionCreator.BinaryVectorParameter.ActualNameChanged += new EventHandler(SolutionCreator_BinaryVectorParameter_ActualNameChanged);
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
      Operators.Add(new BestOneMaxSolutionAnalyzer());
      ParameterizeAnalyzer();
      foreach (IBinaryVectorOperator op in ApplicationManager.Manager.GetInstances<IBinaryVectorOperator>()) {
        if (!(op is ISingleObjectiveMoveEvaluator) || (op is IOneMaxMoveEvaluator)) {
          Operators.Add(op);
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
      foreach (var op in Operators.OfType<IBinaryVectorMultiNeighborhoodShakingOperator>())
        op.BinaryVectorParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
    }
    #endregion
  }
}
