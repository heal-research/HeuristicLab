#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.Instances;


namespace HeuristicLab.Problems.GeneticProgramming.BasicSymbolicRegression {
  [Item("Koza-style Symbolic Regression", "An implementation of symbolic regression without bells-and-whistles. Use \"Symbolic Regression Problem (single-objective)\" if you want to use all features.")]
  [Creatable(CreatableAttribute.Categories.GeneticProgrammingProblems, Priority = 900)]
  [StorableClass]
  public sealed class Problem : SymbolicExpressionTreeProblem, IRegressionProblem, IProblemInstanceConsumer<IRegressionProblemData>, IProblemInstanceExporter<IRegressionProblemData> {

    #region parameter names
    private const string ProblemDataParameterName = "ProblemData";
    #endregion

    #region Parameter Properties
    IParameter IDataAnalysisProblem.ProblemDataParameter { get { return ProblemDataParameter; } }

    public IValueParameter<IRegressionProblemData> ProblemDataParameter {
      get { return (IValueParameter<IRegressionProblemData>)Parameters[ProblemDataParameterName]; }
    }
    #endregion

    #region Properties
    public IRegressionProblemData ProblemData {
      get { return ProblemDataParameter.Value; }
      set { ProblemDataParameter.Value = value; }
    }
    IDataAnalysisProblemData IDataAnalysisProblem.ProblemData { get { return ProblemData; } }
    #endregion

    public event EventHandler ProblemDataChanged;

    public override bool Maximization {
      get { return true; }
    }

    #region item cloning and persistence
    // persistence
    [StorableConstructor]
    private Problem(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    // cloning 
    private Problem(Problem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new Problem(this, cloner); }
    #endregion

    public Problem()
      : base() {
      Parameters.Add(new ValueParameter<IRegressionProblemData>(ProblemDataParameterName, "The data for the regression problem", new RegressionProblemData()));

      var g = new SimpleSymbolicExpressionGrammar(); // empty grammar is replaced in UpdateGrammar()
      base.Encoding = new SymbolicExpressionTreeEncoding(g, 100, 17);

      UpdateGrammar();
      RegisterEventHandlers();
    }


    public override double Evaluate(ISymbolicExpressionTree tree, IRandom random) {
      // Doesn't use classes from HeuristicLab.Problems.DataAnalysis.Symbolic to make sure that the implementation can be fully understood easily.
      // HeuristicLab.Problems.DataAnalysis.Symbolic would already provide all the necessary functionality (esp. interpreter) but at a much higher complexity.
      // Another argument is that we don't need a reference to HeuristicLab.Problems.DataAnalysis.Symbolic

      var problemData = ProblemData;
      var rows = ProblemData.TrainingIndices.ToArray();
      var target = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, rows);
      var predicted = Interpret(tree, problemData.Dataset, rows);

      OnlineCalculatorError errorState;
      var r = OnlinePearsonsRCalculator.Calculate(target, predicted, out errorState);
      if (errorState != OnlineCalculatorError.None) r = 0;
      return r * r;
    }

    private IEnumerable<double> Interpret(ISymbolicExpressionTree tree, IDataset dataset, IEnumerable<int> rows) {
      // skip programRoot and startSymbol
      return InterpretRec(tree.Root.GetSubtree(0).GetSubtree(0), dataset, rows);
    }

    private IEnumerable<double> InterpretRec(ISymbolicExpressionTreeNode node, IDataset dataset, IEnumerable<int> rows) {
      Func<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode, Func<double, double, double>, IEnumerable<double>> binaryEval =
        (left, right, f) => InterpretRec(left, dataset, rows).Zip(InterpretRec(right, dataset, rows), f);

      switch (node.Symbol.Name) {
        case "+": return binaryEval(node.GetSubtree(0), node.GetSubtree(1), (x, y) => x + y);
        case "*": return binaryEval(node.GetSubtree(0), node.GetSubtree(1), (x, y) => x * y);
        case "-": return binaryEval(node.GetSubtree(0), node.GetSubtree(1), (x, y) => x - y);
        case "%": return binaryEval(node.GetSubtree(0), node.GetSubtree(1), (x, y) => y.IsAlmost(0.0) ? 0.0 : x / y); // protected division
        default: {
            double erc;
            if (double.TryParse(node.Symbol.Name, out erc)) {
              return rows.Select(_ => erc);
            } else {
              // assume that this is a variable name
              return dataset.GetDoubleValues(node.Symbol.Name, rows);
            }
          }
      }
    }


    #region events
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

    private void OnProblemDataChanged() {
      UpdateGrammar();

      var handler = ProblemDataChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    private void UpdateGrammar() {
      // whenever ProblemData is changed we create a new grammar with the necessary symbols
      var g = new SimpleSymbolicExpressionGrammar();
      g.AddSymbols(new[] { "+", "*", "%", "-" }, 2, 2); // % is protected division 1/0 := 0

      foreach (var variableName in ProblemData.AllowedInputVariables)
        g.AddTerminalSymbol(variableName);

      // generate ephemeral random consts in the range [-10..+10[ (2*number of variables)
      var rand = new System.Random();
      for (int i = 0; i < ProblemData.AllowedInputVariables.Count() * 2; i++) {
        string newErcSy;
        do {
          newErcSy = string.Format("{0:F2}", rand.NextDouble() * 20 - 10);
        } while (g.Symbols.Any(sy => sy.Name == newErcSy)); // it might happen that we generate the same constant twice
        g.AddTerminalSymbol(newErcSy);
      }

      Encoding.Grammar = g;
    }
    #endregion

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
