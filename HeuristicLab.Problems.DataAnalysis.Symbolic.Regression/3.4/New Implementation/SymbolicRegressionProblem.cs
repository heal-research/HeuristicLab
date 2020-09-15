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
using System.Threading;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("Symbolic Regression Problem", "Represents a symbolic regression problem (single-objective).")]
  [StorableType("01A2E13B-30F4-42DA-A57D-0D5B01A3FDF8")]
  [Creatable(CreatableAttribute.Categories.GeneticProgrammingProblems, Priority = 100)]
  public class SymbolicRegressionProblem : SymbolicExpressionTreeProblem, IRegressionProblem {
    #region Parameter properties
    //TODO remove private setter to transform to readonly auto properties
    [Storable] public IValueParameter<IRegressionProblemData> ProblemDataParameter { get; private set; }
    [Storable] public IValueParameter<ISymbolicDataAnalysisGrammar> GrammarParameter { get; private set; }
    [Storable] public IValueParameter<IntValue> MaximumTreeLengthParameter { get; private set; }
    [Storable] public IValueParameter<IntValue> MaximumTreeDepthParameter { get; private set; }
    #endregion

    #region Properties
    IDataAnalysisProblemData IDataAnalysisProblem.ProblemData {
      get { return ProblemData; }
    }
    public IRegressionProblemData ProblemData {
      get { return ProblemDataParameter.Value; }
      set { ProblemDataParameter.Value = value; }
    }
    public ISymbolicDataAnalysisGrammar Grammar {
      get { return GrammarParameter.Value; }
      set { GrammarParameter.Value = value; }
    }
    public int MaximumTreeLength {
      get { return MaximumTreeLengthParameter.Value.Value; }
      set { MaximumTreeLengthParameter.Value.Value = value; }
    }

    public int MaximumTreeDepth {
      get { return MaximumTreeDepthParameter.Value.Value; }
      set { MaximumTreeDepthParameter.Value.Value = value; }
    }
    #endregion


    #region Serialization
    [StorableConstructor]
    protected SymbolicRegressionProblem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }
    #endregion

    #region Cloning
    protected SymbolicRegressionProblem(SymbolicRegressionProblem original, Cloner cloner) : base(original, cloner) {
      ProblemDataParameter = cloner.Clone(original.ProblemDataParameter);
      GrammarParameter = cloner.Clone(original.GrammarParameter);
      MaximumTreeLengthParameter = cloner.Clone(original.MaximumTreeLengthParameter);
      MaximumTreeDepthParameter = cloner.Clone(original.MaximumTreeDepthParameter);
      RegisterEventHandlers();

    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionProblem(this, cloner);
    }
    #endregion

    public SymbolicRegressionProblem() : base() {
      ProblemDataParameter = new ValueParameter<IRegressionProblemData>("ProblemData", "The data set, target variable and input variables of the regression problem.", new RegressionProblemData());
      GrammarParameter = new ReferenceParameter<ISymbolicDataAnalysisGrammar, ISymbolicExpressionGrammar>("Grammar", "The grammar that should be used for symbolic expression tree.", Encoding.GrammarParameter);
      MaximumTreeLengthParameter = new ReferenceParameter<IntValue>("Maximum Tree Length", "Maximal length of the symbolic expression.", Encoding.TreeLengthParameter);
      MaximumTreeDepthParameter = new ReferenceParameter<IntValue>("Maximum Tree Depth", "Maximal depth of the symbolic expression.", Encoding.TreeDepthParameter);

      Encoding.GrammarParameter.ReadOnly = false;
      var grammar = new TypeCoherentExpressionGrammar();
      grammar.ConfigureAsDefaultRegressionGrammar();
      Grammar = grammar;

      //Parameters.Add(GrammarParameter);
      Parameters.Add(ProblemDataParameter);
      Parameters.Add(MaximumTreeLengthParameter);
      Parameters.Add(MaximumTreeDepthParameter);

      RegisterEventHandlers();
    }


    public override ISingleObjectiveEvaluationResult Evaluate(ISymbolicExpressionTree solution, IRandom random, CancellationToken cancellationToken) {
      var quality = 0.0;

      //IEnumerable<double> estimatedValues = interpreter.GetSymbolicExpressionTreeValues(solution, problemData.Dataset, rows);
      //IEnumerable<double> targetValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, rows);

      //if (LinearScaling) estimatedValues = LinearScaling.ScaleEstimatedValues(estimatedValues, targetValues, lowerEstimationLimit, upperEstimationLimit, ProblemData.Dataset.Rows);

      //quality = OnlineMeanSquaredErrorCalculator.Calculate(targetValues, estimatedValues, out OnlineCalculatorError errorState);
      //if (errorState != OnlineCalculatorError.None) quality = double.NaN;

      var result = new SingleObjectiveEvaluationResult(quality);
      return result;
    }

    public override void Analyze(ISingleObjectiveSolutionContext<ISymbolicExpressionTree>[] solutionContexts, IRandom random) {

    }


    private void RegisterEventHandlers() {
      ProblemDataParameter.ValueChanged += new EventHandler(ProblemDataParameter_ValueChanged);
      if (ProblemDataParameter.Value != null) ProblemDataParameter.Value.Changed += new EventHandler(ProblemData_Changed);
    }

    private void ProblemDataParameter_ValueChanged(object sender, EventArgs e) {
      ProblemDataParameter.Value.Changed += new EventHandler(ProblemData_Changed);
      OnProblemDataChanged();
      OnReset();
    }

    private void ProblemData_Changed(object sender, EventArgs e) {
      OnReset();
    }

    public event EventHandler ProblemDataChanged;
    protected virtual void OnProblemDataChanged() {
      ProblemDataChanged?.Invoke(this, EventArgs.Empty);
    }

    #region Import & Export
    public void Load(IRegressionProblemData data) {
      Name = data.Name;
      Description = data.Description;
      ProblemData = data;
    }

    public IRegressionProblemData Export() {
      return ProblemData;
    }
    #endregion
  }
}
