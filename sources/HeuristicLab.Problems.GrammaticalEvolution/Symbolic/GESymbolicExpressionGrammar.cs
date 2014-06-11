#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
 * 
 * Author: Sabine Winkler
 */

#endregion

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.GrammaticalEvolution {
  [StorableClass]
  [Item("GESymbolicExpressionGrammar", "Represents a grammar for functional expressions for grammatical evolution.")]
  public class GESymbolicExpressionGrammar : SymbolicExpressionGrammar, ISymbolicDataAnalysisGrammar {
    [StorableConstructor]
    protected GESymbolicExpressionGrammar(bool deserializing) : base(deserializing) { }
    protected GESymbolicExpressionGrammar(GESymbolicExpressionGrammar original, Cloner cloner) : base(original, cloner) { }
    public GESymbolicExpressionGrammar()
      : base(ItemAttribute.GetName(typeof(GESymbolicExpressionGrammar)), ItemAttribute.GetDescription(typeof(GESymbolicExpressionGrammar))) {
      // empty ctor is necessary to allow creation of new GEGrammars from the GUI.
      // the problem creates a new correctly configured grammar when the grammar is set
    }
    internal GESymbolicExpressionGrammar(IEnumerable<string> variableNames, int nConstants)
      : base(ItemAttribute.GetName(typeof(GESymbolicExpressionGrammar)), ItemAttribute.GetDescription(typeof(GESymbolicExpressionGrammar))) {
      // this ctor is called by the problem as only the problem knows the allowed input variables
      Initialize(variableNames, nConstants);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new GESymbolicExpressionGrammar(this, cloner);
    }

    private void Initialize(IEnumerable<string> variableNames, int nConstants) {
      #region symbol declaration
      var add = new Addition();
      var sub = new Subtraction();
      var mul = new Multiplication();
      var div = new Division();
      var mean = new Average();
      var log = new Logarithm();
      var pow = new Power();
      var square = new Square();
      var root = new Root();
      var sqrt = new SquareRoot();
      var exp = new Exponential();

      // we use our own random number generator here because we assume 
      // that grammars are only initialized once when setting the grammar in the problem.
      // This means everytime the grammar parameter in the problem is changed
      // we initialize the constants to new values
      var rand = new MersenneTwister();
      // warm up
      for (int i = 0; i < 1000; i++) rand.NextDouble();

      var constants = new List<Constant>(nConstants);
      for (int i = 0; i < nConstants; i++) {
        var constVal = rand.NextDouble() * 20.0 - 10.0;
        var constant = new Constant();
        constant.Name = string.Format("{0:0.000}", constVal);
        constant.MinValue = constVal;
        constant.MaxValue = constVal;
        constant.ManipulatorSigma = 0.0;
        constant.ManipulatorMu = 0.0;
        constant.MultiplicativeManipulatorSigma = 0.0;
        constants.Add(constant);
      }

      var variables = new List<HeuristicLab.Problems.DataAnalysis.Symbolic.Variable>();
      foreach (var variableName in variableNames) {
        var variableSymbol = new HeuristicLab.Problems.DataAnalysis.Symbolic.Variable();
        variableSymbol.Name = variableName;
        variableSymbol.WeightManipulatorMu = 0.0;
        variableSymbol.WeightManipulatorSigma = 0.0;
        variableSymbol.WeightMu = 1.0;
        variableSymbol.WeightSigma = 0.0;
        variableSymbol.MultiplicativeWeightManipulatorSigma = 0.0;
        variableSymbol.AllVariableNames = new[] { variableName };
        variableSymbol.VariableNames = new[] { variableName };
        variables.Add(variableSymbol);
      }

      #endregion

      AddSymbol(add);
      AddSymbol(sub);
      AddSymbol(mul);
      AddSymbol(div);
      AddSymbol(mean);
      AddSymbol(log);
      AddSymbol(pow);
      AddSymbol(square);
      AddSymbol(root);
      AddSymbol(sqrt);
      AddSymbol(exp);
      constants.ForEach(AddSymbol);
      variables.ForEach(AddSymbol);

      #region subtree count configuration
      SetSubtreeCount(add, 2, 2);
      SetSubtreeCount(sub, 2, 2);
      SetSubtreeCount(mul, 2, 2);
      SetSubtreeCount(div, 2, 2);
      SetSubtreeCount(mean, 2, 2);
      SetSubtreeCount(log, 1, 1);
      SetSubtreeCount(pow, 2, 2);
      SetSubtreeCount(square, 1, 1);
      SetSubtreeCount(root, 2, 2);
      SetSubtreeCount(sqrt, 1, 1);
      SetSubtreeCount(exp, 1, 1);
      constants.ForEach((c) => SetSubtreeCount(c, 0, 0));
      variables.ForEach((v) => SetSubtreeCount(v, 0, 0));
      #endregion

      var functions = new ISymbol[] { add, sub, mul, div, mean, log, pow, root, square, sqrt };
      var terminalSymbols = variables.Concat<ISymbol>(constants);
      var allSymbols = functions.Concat(terminalSymbols);

      #region allowed child symbols configuration
      foreach (var s in allSymbols) {
        AddAllowedChildSymbol(StartSymbol, s);
      }
      foreach (var parentSymb in functions)
        foreach (var childSymb in allSymbols) {
          AddAllowedChildSymbol(parentSymb, childSymb);
        }

      #endregion
    }
  }
}
