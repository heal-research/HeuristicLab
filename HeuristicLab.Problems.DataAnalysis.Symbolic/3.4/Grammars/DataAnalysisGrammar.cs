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
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {

  [StorableType("B91038EA-338E-40BB-99D2-738B96C66D81")]
  public abstract class DataAnalysisGrammar : SymbolicExpressionGrammar, ISymbolicDataAnalysisGrammar {
    [StorableConstructor]
    protected DataAnalysisGrammar(StorableConstructorFlag _) : base(_) { }
    protected DataAnalysisGrammar(DataAnalysisGrammar original, Cloner cloner) : base(original, cloner) { }
    protected DataAnalysisGrammar(string name, string description) : base(name, description) { }

    public void ConfigureVariableSymbols(IDataAnalysisProblemData problemData) {
      StartGrammarManipulation();

      var dataset = problemData.Dataset;
      foreach (var varSymbol in Symbols.OfType<VariableBase>()) {
        if (varSymbol.Fixed) continue;
        if (varSymbol is BinaryFactorVariable) continue;
        if (varSymbol is FactorVariable) continue;

        varSymbol.AllVariableNames = problemData.InputVariables.Select(x => x.Value).Where(x => dataset.VariableHasType<double>(x));
        varSymbol.VariableNames = problemData.AllowedInputVariables.Where(x => dataset.VariableHasType<double>(x));

      }

      foreach (var factorSymbol in Symbols.OfType<BinaryFactorVariable>()) {
        if (factorSymbol.Fixed) continue;
        factorSymbol.AllVariableNames = problemData.InputVariables.Select(x => x.Value).Where(x => dataset.VariableHasType<string>(x));
        factorSymbol.VariableNames = problemData.AllowedInputVariables.Where(x => dataset.VariableHasType<string>(x));
        factorSymbol.VariableValues = factorSymbol.VariableNames
          .ToDictionary(varName => varName, varName => dataset.GetStringValues(varName).Distinct().ToList());
      }

      foreach (var factorSymbol in Symbols.OfType<FactorVariable>()) {
        if (factorSymbol.Fixed) continue;
        factorSymbol.AllVariableNames = problemData.InputVariables.Select(x => x.Value).Where(x => dataset.VariableHasType<string>(x));
        factorSymbol.VariableNames = problemData.AllowedInputVariables.Where(x => dataset.VariableHasType<string>(x));
        factorSymbol.VariableValues = factorSymbol.VariableNames
          .ToDictionary(varName => varName,
          varName => dataset.GetStringValues(varName).Distinct()
          .Select((n, i) => Tuple.Create(n, i))
          .ToDictionary(tup => tup.Item1, tup => tup.Item2));
      }

      FinishedGrammarManipulation();
    }
  }
}
