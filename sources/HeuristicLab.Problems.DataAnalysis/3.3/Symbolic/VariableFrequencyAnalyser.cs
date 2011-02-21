#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [Item("VariableFrequencyAnalyser", "Calculates the accumulated frequencies of variable-symbols over the whole population.")]
  [StorableClass]
  public abstract class VariableFrequencyAnalyser : SingleSuccessorOperator {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string DataAnalysisProblemDataParameterName = "DataAnalysisProblemData";
    private const string VariableFrequenciesParameterName = "VariableFrequencies";

    #region parameter properties
    public ILookupParameter<DataAnalysisProblemData> DataAnalysisProblemDataParameter {
      get { return (ILookupParameter<DataAnalysisProblemData>)Parameters[DataAnalysisProblemDataParameterName]; }
    }
    public ILookupParameter<ItemArray<SymbolicExpressionTree>> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<ItemArray<SymbolicExpressionTree>>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ILookupParameter<DoubleMatrix> VariableFrequenciesParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters[VariableFrequenciesParameterName]; }
    }
    #endregion
    #region properties
    public DataAnalysisProblemData DataAnalysisProblemData {
      get { return DataAnalysisProblemDataParameter.ActualValue; }
    }
    public ItemArray<SymbolicExpressionTree> SymbolicExpressionTrees {
      get { return SymbolicExpressionTreeParameter.ActualValue; }
    }
    public DoubleMatrix VariableFrequencies {
      get { return VariableFrequenciesParameter.ActualValue; }
      set { VariableFrequenciesParameter.ActualValue = value; }
    }
    #endregion
    [StorableConstructor]
    protected VariableFrequencyAnalyser(bool deserializing) : base(deserializing) { }
    protected VariableFrequencyAnalyser(VariableFrequencyAnalyser original, Cloner cloner)
      : base(original, cloner) {
    }
    public VariableFrequencyAnalyser()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees that should be analyzed."));
      Parameters.Add(new LookupParameter<DataAnalysisProblemData>(DataAnalysisProblemDataParameterName, "The problem data on which the for which the symbolic expression tree is a solution."));
      Parameters.Add(new LookupParameter<DoubleMatrix>(VariableFrequenciesParameterName, "The relative variable reference frequencies aggregated over the whole population."));
    }

    public override IOperation Apply() {
      var inputVariables = DataAnalysisProblemData.InputVariables.CheckedItems.Select(x => x.Value.Value);
      if (VariableFrequencies == null) {
        VariableFrequencies = new DoubleMatrix(0, 1, inputVariables);
      }
      ((IStringConvertibleMatrix)VariableFrequencies).Rows = VariableFrequencies.Rows + 1;
      int lastRowIndex = VariableFrequencies.Rows - 1;
      var columnNames = VariableFrequencies.ColumnNames.ToList();
      foreach (var pair in CalculateVariableFrequencies(SymbolicExpressionTrees, inputVariables)) {
        int columnIndex = columnNames.IndexOf(pair.Key);
        VariableFrequencies[lastRowIndex, columnIndex] = pair.Value;
      }
      return base.Apply();
    }

    public static IEnumerable<KeyValuePair<string, double>> CalculateVariableFrequencies(IEnumerable<SymbolicExpressionTree> trees, IEnumerable<string> inputVariables) {
      Dictionary<string, double> variableReferencesSum = new Dictionary<string, double>();
      Dictionary<string, double> variableFrequencies = new Dictionary<string, double>();
      foreach (var inputVariable in inputVariables)
        variableReferencesSum[inputVariable] = 0.0;
      foreach (var tree in trees) {
        var variableReferences = GetVariableReferenceCount(tree, inputVariables);
        foreach (var pair in variableReferences) {
          variableReferencesSum[pair.Key] += pair.Value;
        }
      }
      double totalVariableReferences = variableReferencesSum.Values.Sum();
      foreach (string inputVariable in inputVariables) {
        double relFreq = variableReferencesSum[inputVariable] / totalVariableReferences;
        variableFrequencies.Add(inputVariable, relFreq);
      }
      return variableFrequencies;
    }

    private static IEnumerable<KeyValuePair<string, int>> GetVariableReferenceCount(SymbolicExpressionTree tree, IEnumerable<string> inputVariables) {
      Dictionary<string, int> references = new Dictionary<string, int>();
      var variableNames = from node in tree.IterateNodesPrefix().OfType<VariableTreeNode>()
                          select node.VariableName;
      var variableNamesInConditions = from node in tree.IterateNodesPrefix().OfType<VariableConditionTreeNode>()
                                      select node.VariableName;

      foreach (var variableName in variableNames.Concat(variableNamesInConditions)) {
        if (!references.ContainsKey(variableName)) {
          references[variableName] = 1;
        } else {
          references[variableName] += 1;
        }
      }
      return references;
    }
  }
}
