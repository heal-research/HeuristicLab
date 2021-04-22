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

using System.Linq;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [StorableType("4318C6BD-E0A1-45FE-AC30-96E7F73B51FB")]
  [Item("ShapeConstraintsAnalyzer", "Analyzes the number of shape constraint violations of symbolic regression models.")]
  public class ShapeConstraintsAnalyzer : SymbolicDataAnalysisAnalyzer, ISymbolicExpressionTreeAnalyzer {
    private const string ProblemDataParameterName = "ProblemData";
    private const string ConstraintViolationsParameterName = "ConstraintViolations";
    private const string InfeasibleSolutionsParameterName = "InfeasibleSolutions";

    #region parameter properties

    public ILookupParameter<IRegressionProblemData> RegressionProblemDataParameter =>
      (ILookupParameter<IRegressionProblemData>) Parameters[ProblemDataParameterName];

    public IResultParameter<DataTable> ConstraintViolationsParameter =>
      (IResultParameter<DataTable>) Parameters[ConstraintViolationsParameterName];

    public IResultParameter<DataTable> InfeasibleSolutionsParameter =>
      (IResultParameter<DataTable>)Parameters[InfeasibleSolutionsParameterName];

    #endregion

    #region properties
    public IRegressionProblemData RegressionProblemData => RegressionProblemDataParameter.ActualValue;
    public DataTable ConstraintViolations => ConstraintViolationsParameter.ActualValue;
    public DataTable InfeasibleSolutions => InfeasibleSolutionsParameter.ActualValue;
    #endregion

    public override bool EnabledByDefault => false;

    [StorableConstructor]
    protected ShapeConstraintsAnalyzer(StorableConstructorFlag _) : base(_) { }

    protected ShapeConstraintsAnalyzer(ShapeConstraintsAnalyzer original, Cloner cloner) :
      base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ShapeConstraintsAnalyzer(this, cloner);
    }

    public ShapeConstraintsAnalyzer() {
      Parameters.Add(new LookupParameter<IRegressionProblemData>(ProblemDataParameterName,
        "The problem data of the symbolic data analysis problem."));
      Parameters.Add(new ResultParameter<DataTable>(ConstraintViolationsParameterName,
        "The number of constraint violations."));
      Parameters.Add(new ResultParameter<DataTable>(InfeasibleSolutionsParameterName,
        "The number of infeasible solutions."));


      ConstraintViolationsParameter.DefaultValue = new DataTable(ConstraintViolationsParameterName) {
        VisualProperties = {
          XAxisTitle = "Generations",
          YAxisTitle = "Constraint violations"
        }
      };

      InfeasibleSolutionsParameter.DefaultValue = new DataTable(InfeasibleSolutionsParameterName) {
        VisualProperties = {
          XAxisTitle = "Generations",
          YAxisTitle = "Infeasible solutions"
        }
      };
    }


    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public override IOperation Apply() {
      var problemData = (IShapeConstrainedRegressionProblemData)RegressionProblemData;
      var trees = SymbolicExpressionTree.ToArray();

      var results = ResultCollection;
      var constraints = problemData.ShapeConstraints.EnabledConstraints;
      var variableRanges = problemData.VariableRanges;
      var constraintViolationsTable = ConstraintViolations;
      var estimator = new IntervalArithBoundsEstimator();

      if (!constraintViolationsTable.Rows.Any())
        foreach (var constraint in constraints)
          constraintViolationsTable.Rows.Add(new DataRow(constraint.ToString()));

      foreach (var constraint in constraints) {
        var numViolations = trees.Count(tree => IntervalUtil.GetConstraintViolation(constraint, estimator, variableRanges, tree) > 0.0);
        constraintViolationsTable.Rows[constraint.ToString()].Values.Add(numViolations);
      }

      var infeasibleSolutionsDataTable = InfeasibleSolutions;
      if (infeasibleSolutionsDataTable.Rows.Count == 0)
        infeasibleSolutionsDataTable.Rows.Add(new DataRow(InfeasibleSolutionsParameterName));

      infeasibleSolutionsDataTable.Rows[InfeasibleSolutionsParameterName]
        .Values
        .Add(trees.Count(t => IntervalUtil.GetConstraintViolations(constraints, estimator, variableRanges, t).Any(x => x > 0.0)));

      return base.Apply();
    }
  }
}