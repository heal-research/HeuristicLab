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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;
using System.Collections.Generic;
using System;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.VarProMRGP {
  [Item("VarProGrammar", "")]
  [StorableType("E2FFA4F4-2301-4A28-BC6E-670A0D233262")]
  public sealed class VarProGrammar : DataAnalysisGrammar {
    [StorableConstructor]
    private VarProGrammar(StorableConstructorFlag _) : base(_) { }

    private VarProGrammar(VarProGrammar orig, Cloner cloner) : base(orig, cloner) { }
    public VarProGrammar() : base("VarProGrammar", "") {
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new VarProGrammar(this, cloner);
    }

    private void Initialize() {
      Symbol sumForLogSy = new Addition();
      sumForLogSy.Name = "SimpleAdd";
      Symbol mulSy = new Multiplication();
      Symbol simpleMulSy = new Multiplication();
      simpleMulSy.Name = "SimpleMul";
      Symbol aqSy = new AnalyticQuotient();
      Symbol logSy = new Logarithm();
      Symbol expSy = new Exponential();

      var constSy = new Constant();
      constSy.MinValue = -1;
      constSy.MaxValue = 1;

      var constOneSy = new Constant();
      constOneSy.Name = "<1.0>";
      constOneSy.MinValue = 1;
      constOneSy.MaxValue = 1;
      constOneSy.ManipulatorMu = 0.0;
      constOneSy.ManipulatorSigma = 0.0;
      constOneSy.MultiplicativeManipulatorSigma = 0.0;

      var varSy = new DataAnalysis.Symbolic.Variable();
      varSy.WeightMu = 1.0;
      varSy.WeightSigma = 0.0;

      var allSymbols = new List<Symbol>() { sumForLogSy, mulSy, simpleMulSy, aqSy, logSy, expSy, constSy, constOneSy, varSy };

      foreach (var symb in allSymbols)
        AddSymbol(symb);

      SetSubtreeCount(sumForLogSy, 2, 5);
      SetSubtreeCount(simpleMulSy, 2, 5);
      SetSubtreeCount(mulSy, 2, 4);


      foreach (var funSymb in new[] { logSy, expSy }) {
        SetSubtreeCount(funSymb, 1, 1);
      }

      SetSubtreeCount(aqSy, 2, 2);
      SetSubtreeCount(constSy, 0, 0);
      SetSubtreeCount(constOneSy, 0, 0);
      SetSubtreeCount(varSy, 0, 0);

      // allowed root symbols
      foreach (var childSy in new[] { mulSy, varSy, logSy, expSy, aqSy }) {
        AddAllowedChildSymbol(StartSymbol, childSy);
        AddAllowedChildSymbol(DefunSymbol, childSy);
      }

      // allowed under mul
      foreach (var childSy in new[] { varSy, logSy, expSy }) {
        AddAllowedChildSymbol(mulSy, childSy);
      }

      // allowed for log:
      AddAllowedChildSymbol(logSy, sumForLogSy);

      // allowed for exp:
      AddAllowedChildSymbol(expSy, simpleMulSy);

      // allowed for simpleAdd arg 0:
      AddAllowedChildSymbol(sumForLogSy, constOneSy, 0); // enforce log ( 1 + ...)

      // allowed for simpleAdd ars 1 - n:
      for (int arg = 1; arg < this.GetMaximumSubtreeCount(sumForLogSy); arg++)
        AddAllowedChildSymbol(sumForLogSy, simpleMulSy, arg);

      // allowed for simpleMul arg 0:
      AddAllowedChildSymbol(simpleMulSy, constSy, 0);

      // allowed for simpleMul ars 1 - n:
      for (int arg = 1; arg < this.GetMaximumSubtreeCount(simpleMulSy); arg++)
        AddAllowedChildSymbol(simpleMulSy, varSy, arg);

      // allowed numerators for AQ:
      foreach (var child in new[] { constOneSy, varSy, mulSy })
        AddAllowedChildSymbol(aqSy, child, 0);

      // allowed denominator for AQ:
      foreach (var child in new[] { varSy, sumForLogSy })
        AddAllowedChildSymbol(aqSy, child, 1);
    }
  }
}
