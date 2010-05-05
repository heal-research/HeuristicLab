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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using System.Collections.Generic;
using HeuristicLab.Analysis;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Interfaces;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Analyzers {
  /// <summary>
  /// An operator that tracks the min avgerage and max tree size in the population.
  /// </summary>
  [Item("PopulationMinAvgMaxTreeSizeAnalyzer", "An operator that tracks the min avgerage and max tree size in the population.")]
  [StorableClass]
  public sealed class PopulationMinAvgMaxTreeSizeAnalyzer : AlgorithmOperator, ISymbolicExpressionTreePopulationAnalyzer {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string SymbolicExpressionTreeSizeParameterName = "SymbolicExpressionTreeSize";
    private const string SymbolicExpressionTreeSizesParameterName = "SymbolicExpressionTreeSizes";
    private const string ResultsParameterName = "Results";


    #region parameter properties
    public ILookupParameter<ItemArray<SymbolicExpressionTree>> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<ItemArray<SymbolicExpressionTree>>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ILookupParameter<ItemArray<DoubleValue>> SymbolicExpressionTreeSizeParameter {
      get { return (ILookupParameter<ItemArray<DoubleValue>>)Parameters[SymbolicExpressionTreeSizeParameterName]; }
    }
    public ValueLookupParameter<DataTable> SymbolicExpressionTreeSizesParameter {
      get { return (ValueLookupParameter<DataTable>)Parameters[SymbolicExpressionTreeSizesParameterName]; }
    }
    public ValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (ValueLookupParameter<VariableCollection>)Parameters[ResultsParameterName]; }
    }
    #endregion
    public PopulationMinAvgMaxTreeSizeAnalyzer()
      : base() {
      Parameters.Add(new SubScopesLookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression tree whose size should be calculated."));
      Parameters.Add(new SubScopesLookupParameter<DoubleValue>(SymbolicExpressionTreeSizeParameterName, "The tree size of the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<DataTable>(SymbolicExpressionTreeSizesParameterName, "The data table to store the tree sizes."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>(ResultsParameterName, "The results collection where the analysis values should be stored."));

      UniformSubScopesProcessor subScopesProcessor = new UniformSubScopesProcessor();
      SymbolicExpressionTreeSizeCalculator sizeCalculator = new SymbolicExpressionTreeSizeCalculator();
      PopulationMinAverageMaxValueAnalyzer valuesAnalyzer = new PopulationMinAverageMaxValueAnalyzer();
      sizeCalculator.SymbolicExpressionTreeParameter.ActualName = SymbolicExpressionTreeParameter.Name;
      sizeCalculator.SymbolicExpressionTreeSizeParameter.ActualName = SymbolicExpressionTreeSizeParameter.Name;
      valuesAnalyzer.ValueParameter.ActualName = sizeCalculator.SymbolicExpressionTreeSizeParameter.Name;
      valuesAnalyzer.ValuesParameter.ActualName = SymbolicExpressionTreeSizesParameter.Name;
      valuesAnalyzer.ResultsParameter.ActualName = ResultsParameter.Name;
      valuesAnalyzer.AverageValueParameter.ActualName = "Avg. Tree Size";
      valuesAnalyzer.MaxValueParameter.ActualName = "Max Tree Size";
      valuesAnalyzer.MinValueParameter.ActualName = "Min Tree Size";

      OperatorGraph.InitialOperator = subScopesProcessor;
      subScopesProcessor.Operator = sizeCalculator;
      subScopesProcessor.Successor = valuesAnalyzer;
      valuesAnalyzer.Successor = null;
    }
  }
}
