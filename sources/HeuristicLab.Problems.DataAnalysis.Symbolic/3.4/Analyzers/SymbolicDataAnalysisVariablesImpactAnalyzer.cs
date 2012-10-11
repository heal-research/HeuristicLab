#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using HeuristicLab.Core;
using System.Collections;
using HeuristicLab.Parameters;
using HeuristicLab.Analysis;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {

    public class SymbolicDataAnalysisVariableImpactAnalyzer<T, U> : SymbolicDataAnalysisSingleObjectiveValidationAnalyzer<T, U>
        where T : class, ISymbolicDataAnalysisSingleObjectiveEvaluator<U>
        where U : class, IDataAnalysisProblemData {
        private const string EstimationLimitsParameterName = "EstimationLimits";
        private const string VariableImpactsDataTableResultName = "Variable Impacts";

        public IValueLookupParameter<DoubleLimit> EstimationLimitsParameter {
            get { return (IValueLookupParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName]; }
        }

        [StorableConstructor]
        protected SymbolicDataAnalysisVariableImpactAnalyzer(bool deserializing) : base(deserializing) { }
        protected SymbolicDataAnalysisVariableImpactAnalyzer(SymbolicDataAnalysisVariableImpactAnalyzer<T, U> original, Cloner cloner) : base(original, cloner) { }
        public override IDeepCloneable Clone(Cloner cloner) { return new SymbolicDataAnalysisVariableImpactAnalyzer<T,U>(this, cloner); }

        public SymbolicDataAnalysisVariableImpactAnalyzer() :base(){
            Parameters.Add(new ValueLookupParameter<DoubleLimit>(EstimationLimitsParameterName, "The lower and upper limit for the estimated values produced by the symbolic classification model."));
        }

        public override IOperation Apply() {
            var rows = GenerateRowsToEvaluate().ToArray();
            if (!rows.Any()) throw new ArgumentException();

            var estimationLimits = EstimationLimitsParameter.ActualValue;
            var problemData = ProblemDataParameter.ActualValue;
            var interpreter = SymbolicDataAnalysisTreeInterpreterParameter.ActualValue;
            var trees = SymbolicExpressionTreeParameter.ActualValue.ToArray();

            var originalTreeEvaluations = trees.Select(t => interpreter.GetSymbolicExpressionTreeValues(t, problemData.Dataset, rows).LimitToRange(estimationLimits.Lower,estimationLimits.Upper).ToArray()).ToArray();

            List<IList> variableValues = new List<IList>();
            foreach(var variable in problemData.AllowedInputVariables) {
                variableValues.Add(problemData.Dataset.GetDoubleValues(variable,rows).ToList());
            }
            Dictionary<string, List<double>> variableImpacts = new Dictionary<string, List<double>>();

            //calculation of variable impacts per tree
            for(int i=0; i < problemData.AllowedInputVariables.Count(); i++) {
               var variableName = problemData.AllowedInputVariables.ElementAt(i);
               var variableOrginalValues = variableValues[i];
               var variableReplacedValues = CalculateReplacementValues(problemData, variableName, rows, rows.Length).ToList();

               variableValues[i] = variableReplacedValues;
               var modifiedDataset = new Dataset(problemData.AllowedInputVariables, variableValues);

               for(int t =0; t< trees.Length; t++) {
                   var tree = trees[t];

                   var treeEvaluation = interpreter.GetSymbolicExpressionTreeValues(tree, modifiedDataset,Enumerable.Range(0,rows.Length)).LimitToRange(estimationLimits.Lower,estimationLimits.Upper);
                   OnlineCalculatorError error;
                   var regressionProblemData = (IRegressionProblemData) problemData;
                   var modifiedR2 = OnlinePearsonsRSquaredCalculator.Calculate(originalTreeEvaluations[t], treeEvaluation, out error);
                   //var modifiedR2 = OnlinePearsonsRSquaredCalculator.Calculate(problemData.Dataset.GetDoubleValues(regressionProblemData.TargetVariable,rows), treeEvaluation, out error);
                   //var originalR2 = OnlinePearsonsRSquaredCalculator.Calculate(problemData.Dataset.GetDoubleValues(regressionProblemData.TargetVariable, rows), originalTreeEvaluations[t], out error);

                   if (error != OnlineCalculatorError.None) modifiedR2 = 0.0;

                   if (!variableImpacts.ContainsKey(variableName)) variableImpacts[variableName] = new List<double>();
                   variableImpacts[variableName].Add(1 - modifiedR2);
               }              
               variableValues[i] = variableOrginalValues;
            }

            //create data table and store average impacts

            var results = ResultCollectionParameter.ActualValue;
            if (!results.ContainsKey(VariableImpactsDataTableResultName)) {
                var dataTableResult = new DataTable("Variable Impacts", "TODO");
                foreach(var variableName in variableImpacts.Keys)
                    dataTableResult.Rows.Add(new DataRow(variableName));

                results.Add(new Result("Variable Impacts",dataTableResult));
            }

            var dataTable = (DataTable)results[VariableImpactsDataTableResultName].Value;

            foreach (var pair in variableImpacts) {
                dataTable.Rows[pair.Key].Values.Add(pair.Value.Average());
            }

            return base.Apply();
        }

        protected IEnumerable<double> CalculateReplacementValues(U problemData, string variableName, IEnumerable<int> rows, int rowCount) {
            var mean = problemData.Dataset.GetDoubleValues(variableName, problemData.TrainingIndices).Average();
            return Enumerable.Repeat(mean, rowCount);
        }



    }

}
