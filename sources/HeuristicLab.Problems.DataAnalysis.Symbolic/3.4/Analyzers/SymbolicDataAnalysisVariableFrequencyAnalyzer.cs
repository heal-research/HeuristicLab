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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// Calculates the accumulated frequencies of variable-symbols over all trees in the population.
  /// </summary>
  [Item("SymbolicDataAnalysisVariableFrequencyAnalyzer", "Calculates the accumulated frequencies of variable-symbols over all trees in the population.")]
  [StorableClass]
  public sealed class SymbolicDataAnalysisVariableFrequencyAnalyzer : SymbolicDataAnalysisAnalyzer {
    private const string VariableFrequenciesParameterName = "VariableFrequencies";
    private const string AggregateLaggedVariablesParameterName = "AggregateLaggedVariables";
    private const string VariableImpactsParameterName = "VariableImpacts";

    #region parameter properties
    public ILookupParameter<DataTable> VariableFrequenciesParameter {
      get { return (ILookupParameter<DataTable>)Parameters[VariableFrequenciesParameterName]; }
    }
    public ILookupParameter<DoubleMatrix> VariableImpactsParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters[VariableImpactsParameterName]; }
    }
    public IValueLookupParameter<BoolValue> AggregateLaggedVariablesParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters[AggregateLaggedVariablesParameterName]; }
    }
    #endregion
    #region properties
    public BoolValue AggregateLaggedVariables {
      get { return AggregateLaggedVariablesParameter.ActualValue; }
      set { AggregateLaggedVariablesParameter.Value = value; }
    }
    #endregion
    [StorableConstructor]
    private SymbolicDataAnalysisVariableFrequencyAnalyzer(bool deserializing) : base(deserializing) { }
    private SymbolicDataAnalysisVariableFrequencyAnalyzer(SymbolicDataAnalysisVariableFrequencyAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }
    public SymbolicDataAnalysisVariableFrequencyAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<DataTable>(VariableFrequenciesParameterName, "The relative variable reference frequencies aggregated over all trees in the population."));
      Parameters.Add(new LookupParameter<DoubleMatrix>(VariableImpactsParameterName, "The relative variable relevance calculated as the average relative variable frequency over the whole run."));
      Parameters.Add(new ValueLookupParameter<BoolValue>(AggregateLaggedVariablesParameterName, "Switch that determines whether all references to a variable should be aggregated regardless of time-offsets. Turn off to analyze all variable references with different time offsets separately.", new BoolValue(true)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisVariableFrequencyAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      ItemArray<ISymbolicExpressionTree> expressions = SymbolicExpressionTreeParameter.ActualValue;
      ResultCollection results = ResultCollection;
      DataTable datatable;
      if (VariableFrequenciesParameter.ActualValue == null) {
        datatable = new DataTable("Variable frequencies", "Relative frequency of variable references aggregated over the whole population.");
        datatable.VisualProperties.XAxisTitle = "Generation";
        datatable.VisualProperties.YAxisTitle = "Relative Variable Frequency";
        VariableFrequenciesParameter.ActualValue = datatable;
        results.Add(new Result("Variable frequencies", "Relative frequency of variable references aggregated over the whole population.", datatable));
        results.Add(new Result("Variable impacts", "The relative variable relevance calculated as the average relative variable frequency over the whole run.", new DoubleMatrix()));
      }

      datatable = VariableFrequenciesParameter.ActualValue;
      // all rows must have the same number of values so we can just take the first
      int numberOfValues = datatable.Rows.Select(r => r.Values.Count).DefaultIfEmpty().First();

      foreach (var pair in SymbolicDataAnalysisVariableFrequencyAnalyzer.CalculateVariableFrequencies(expressions, AggregateLaggedVariables.Value)) {
        if (!datatable.Rows.ContainsKey(pair.Key)) {
          // initialize a new row for the variable and pad with zeros
          DataRow row = new DataRow(pair.Key, "", Enumerable.Repeat(0.0, numberOfValues));
          row.VisualProperties.StartIndexZero = true;
          datatable.Rows.Add(row);
        }
        datatable.Rows[pair.Key].Values.Add(Math.Round(pair.Value, 3));
      }

      // add a zero for each data row that was not modified in the previous loop 
      foreach (var row in datatable.Rows.Where(r => r.Values.Count != numberOfValues + 1))
        row.Values.Add(0.0);

      // update variable impacts matrix
      var orderedImpacts = (from row in datatable.Rows
                            select new { Name = row.Name, Impact = Math.Round(datatable.Rows[row.Name].Values.Average(), 3) })
                           .OrderByDescending(p => p.Impact)
                           .ToList();
      var impacts = new DoubleMatrix();
      var matrix = impacts as IStringConvertibleMatrix;
      matrix.Rows = orderedImpacts.Count;
      matrix.RowNames = orderedImpacts.Select(x => x.Name);
      matrix.Columns = 1;
      matrix.ColumnNames = new string[] { "Relative variable relevance" };
      int i = 0;
      foreach (var p in orderedImpacts) {
        matrix.SetValue(p.Impact.ToString(), i++, 0);
      }

      VariableImpactsParameter.ActualValue = impacts;
      results["Variable impacts"].Value = impacts;
      return base.Apply();
    }

    public static IEnumerable<KeyValuePair<string, double>> CalculateVariableFrequencies(IEnumerable<ISymbolicExpressionTree> trees, bool aggregateLaggedVariables = true) {

      var variableFrequencies = trees
        .AsParallel()
        .SelectMany(t => GetVariableReferences(t, aggregateLaggedVariables))
        .GroupBy(pair => pair.Key, pair => pair.Value)
        .ToDictionary(g => g.Key, g => (double)g.Sum());

      double totalNumberOfSymbols = variableFrequencies.Values.Sum();

      foreach (var pair in variableFrequencies.OrderBy(p => p.Key, new NaturalStringComparer()))
        yield return new KeyValuePair<string, double>(pair.Key, pair.Value / totalNumberOfSymbols);
    }

    private static IEnumerable<KeyValuePair<string, int>> GetVariableReferences(ISymbolicExpressionTree tree, bool aggregateLaggedVariables = true) {
      Dictionary<string, int> references = new Dictionary<string, int>();
      if (aggregateLaggedVariables) {
        tree.Root.ForEachNodePrefix(node => {
          if (node.Symbol is Variable) {
            var varNode = node as VariableTreeNode;
            IncReferenceCount(references, varNode.VariableName);
          } else if (node.Symbol is VariableCondition) {
            var varCondNode = node as VariableConditionTreeNode;
            IncReferenceCount(references, varCondNode.VariableName);
          }
        });
      } else {
        GetVariableReferences(references, tree.Root, 0);
      }
      return references;
    }

    private static void GetVariableReferences(Dictionary<string, int> references, ISymbolicExpressionTreeNode node, int currentLag) {
      if (node.Symbol is LaggedVariable) {
        var laggedVarNode = node as LaggedVariableTreeNode;
        IncReferenceCount(references, laggedVarNode.VariableName, currentLag + laggedVarNode.Lag);
      } else if (node.Symbol is Variable) {
        var varNode = node as VariableTreeNode;
        IncReferenceCount(references, varNode.VariableName, currentLag);
      } else if (node.Symbol is VariableCondition) {
        var varCondNode = node as VariableConditionTreeNode;
        IncReferenceCount(references, varCondNode.VariableName, currentLag);
        GetVariableReferences(references, node.GetSubtree(0), currentLag);
        GetVariableReferences(references, node.GetSubtree(1), currentLag);
      } else if (node.Symbol is Integral) {
        var laggedNode = node as LaggedTreeNode;
        for (int l = laggedNode.Lag; l <= 0; l++) {
          GetVariableReferences(references, node.GetSubtree(0), currentLag + l);
        }
      } else if (node.Symbol is Derivative) {
        for (int l = -4; l <= 0; l++) {
          GetVariableReferences(references, node.GetSubtree(0), currentLag + l);
        }
      } else if (node.Symbol is TimeLag) {
        var laggedNode = node as LaggedTreeNode;
        GetVariableReferences(references, node.GetSubtree(0), currentLag + laggedNode.Lag);
      } else {
        foreach (var subtree in node.Subtrees) {
          GetVariableReferences(references, subtree, currentLag);
        }
      }
    }

    private static void IncReferenceCount(Dictionary<string, int> references, string variableName, int timeLag = 0) {
      string referenceId = variableName +
        (timeLag == 0 ? "" : timeLag < 0 ? "(t" + timeLag + ")" : "(t+" + timeLag + ")");
      if (references.ContainsKey(referenceId)) {
        references[referenceId]++;
      } else {
        references[referenceId] = 1;
      }
    }
  }
}
