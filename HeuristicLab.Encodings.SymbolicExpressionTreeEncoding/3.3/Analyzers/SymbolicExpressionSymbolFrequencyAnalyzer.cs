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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Interfaces;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Analyzers {
  /// <summary>
  /// An operator that tracks the frequencies of distinc symbols.
  /// </summary>
  [Item("SymbolicExpressionSymbolFrequencyAnalyzer", "An operator that tracks frequencies of symbols.")]
  [StorableClass]
  [NonDiscoverableType]
  public class SymbolicExpressionSymbolFrequencyAnalyzer : SingleSuccessorOperator, ISymbolicExpressionTreeAnalyzer {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string ResultsParameterName = "Results";
    private const string SymbolFrequenciesParameterName = "SymbolFrequencies";

    #region parameter properties
    public ScopeTreeLookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ScopeTreeLookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ILookupParameter<DataTable> SymbolFrequenciesParameter {
      get { return (ILookupParameter<DataTable>)Parameters[SymbolFrequenciesParameterName]; }
    }
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }
    #endregion
    #region properties
    public DataTable SymbolFrequencies {
      get { return SymbolFrequenciesParameter.ActualValue; }
      set { SymbolFrequenciesParameter.ActualValue = value; }
    }
    #endregion

    [StorableConstructor]
    protected SymbolicExpressionSymbolFrequencyAnalyzer(bool deserializing) : base(deserializing) { }
    protected SymbolicExpressionSymbolFrequencyAnalyzer(SymbolicExpressionSymbolFrequencyAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public SymbolicExpressionSymbolFrequencyAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new ValueLookupParameter<DataTable>(SymbolFrequenciesParameterName, "The data table to store the symbol frequencies."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection where the best symbolic regression solution should be stored."));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicExpressionSymbolFrequencyAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      ItemArray<SymbolicExpressionTree> expressions = SymbolicExpressionTreeParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;

      if (SymbolFrequencies == null) {
        SymbolFrequencies = new DataTable("Symbol frequencies", "Relative frequency of symbols aggregated over the whole population.");
        SymbolFrequencies.VisualProperties.YAxisTitle = "Relative Symbol Frequency";
        results.Add(new Result("Symbol frequencies", SymbolFrequencies));
      }

      // all rows must have the same number of values so we can just take the first
      int numberOfValues = SymbolFrequencies.Rows.Select(r => r.Values.Count).DefaultIfEmpty().First();

      foreach (var pair in SymbolicExpressionSymbolFrequencyAnalyzer.CalculateSymbolFrequencies(expressions)) {
        if (!SymbolFrequencies.Rows.ContainsKey(pair.Key)) {
          // initialize a new row for the symbol and pad with zeros
          DataRow row = new DataRow(pair.Key, "", Enumerable.Repeat(0.0, numberOfValues));
          row.VisualProperties.StartIndexZero = true;
          SymbolFrequencies.Rows.Add(row);
        }
        SymbolFrequencies.Rows[pair.Key].Values.Add(pair.Value);
      }

      // add a zero for each data row that was not modified in the previous loop 
      foreach (var row in SymbolFrequencies.Rows.Where(r => r.Values.Count != numberOfValues + 1))
        row.Values.Add(0.0);

      return base.Apply();
    }

    public static IEnumerable<KeyValuePair<string, double>> CalculateSymbolFrequencies(IEnumerable<SymbolicExpressionTree> trees) {
      Dictionary<string, double> symbolFrequencies = new Dictionary<string, double>();
      int totalNumberOfSymbols = 0;

      foreach (var tree in trees) {
        foreach (var node in tree.IterateNodesPrefix()) {
          if (symbolFrequencies.ContainsKey(node.Symbol.Name)) symbolFrequencies[node.Symbol.Name] += 1;
          else symbolFrequencies.Add(node.Symbol.Name, 1);
          totalNumberOfSymbols++;
        }
      }

      foreach (var pair in symbolFrequencies)
        yield return new KeyValuePair<string, double>(pair.Key, pair.Value / totalNumberOfSymbols);
    }
  }
}
