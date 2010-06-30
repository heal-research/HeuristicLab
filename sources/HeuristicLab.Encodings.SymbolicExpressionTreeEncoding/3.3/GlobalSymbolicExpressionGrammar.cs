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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using System.Xml;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  [Item("GlobalSymbolicExpressionGrammar", "Represents a grammar that defines the syntax of symbolic expression trees.")]
  public class GlobalSymbolicExpressionGrammar : DefaultSymbolicExpressionGrammar {
    [Storable]
    private int minFunctionDefinitions;
    public int MinFunctionDefinitions {
      get { return minFunctionDefinitions; }
      set { 
        minFunctionDefinitions = value;
        UpdateAdfConstraints();
      }
    }
    [Storable]
    private int maxFunctionDefinitions;
    public int MaxFunctionDefinitions {
      get { return maxFunctionDefinitions; }
      set { 
        maxFunctionDefinitions = value;
        UpdateAdfConstraints();
      }
    }
    [Storable]
    private int minFunctionArguments;
    public int MinFunctionArguments {
      get { return minFunctionArguments; }
      set { 
        minFunctionArguments = value;
      }
    }
    [Storable]
    private int maxFunctionArguments;
    public int MaxFunctionArguments {
      get { return maxFunctionArguments; }
      set { 
        maxFunctionArguments = value;
      }
    }

    [Storable]
    private Defun defunSymbol;

    public GlobalSymbolicExpressionGrammar() : base() { } // empty constructor for cloning

    public GlobalSymbolicExpressionGrammar(ISymbolicExpressionGrammar mainBranchGrammar )
      : base() {
      maxFunctionArguments = 3;
      maxFunctionDefinitions = 3;
      Initialize(mainBranchGrammar);
    }

    private void Initialize(ISymbolicExpressionGrammar mainBranchGrammar) {
      base.Clear();

      // remove the start symbol of the default grammar
      RemoveSymbol(StartSymbol);

      StartSymbol = new ProgramRootSymbol();
      defunSymbol = new Defun();
      AddSymbol(StartSymbol);
      AddSymbol(defunSymbol);

      SetMinSubtreeCount(StartSymbol, minFunctionDefinitions + 1);
      SetMaxSubtreeCount(StartSymbol, maxFunctionDefinitions + 1);
      SetMinSubtreeCount(defunSymbol, 1);
      SetMaxSubtreeCount(defunSymbol, 1);

      // ADF branches maxFunctionDefinitions 
      for (int argumentIndex = 1; argumentIndex < maxFunctionDefinitions + 1; argumentIndex++) {
        SetAllowedChild(StartSymbol, defunSymbol, argumentIndex);
      }

      if (mainBranchGrammar != null) {
        // copy symbols from mainBranchGrammar
        foreach (var symb in mainBranchGrammar.Symbols) {
          AddSymbol(symb);
          SetMinSubtreeCount(symb, mainBranchGrammar.GetMinSubtreeCount(symb));
          SetMaxSubtreeCount(symb, mainBranchGrammar.GetMaxSubtreeCount(symb));
        }

        // the start symbol of the mainBranchGrammar is allowed as the result producing branch
        SetAllowedChild(StartSymbol, mainBranchGrammar.StartSymbol, 0);

        // copy syntax constraints from mainBranchGrammar
        foreach (var parent in mainBranchGrammar.Symbols) {
          for (int i = 0; i < mainBranchGrammar.GetMaxSubtreeCount(parent); i++) {
            foreach (var child in mainBranchGrammar.Symbols) {
              if (mainBranchGrammar.IsAllowedChild(parent, child, i)) {
                SetAllowedChild(parent, child, i);
              }
            }
          }
        }

        // every symbol of the mainBranchGrammar that is allowed as child of the start symbol is also allowed as direct child of defun
        foreach (var symb in mainBranchGrammar.Symbols) {
          if (mainBranchGrammar.IsAllowedChild(mainBranchGrammar.StartSymbol, symb, 0))
            SetAllowedChild(defunSymbol, symb, 0);
        }
      }
    }
    private void UpdateAdfConstraints() {
      SetMinSubtreeCount(StartSymbol, minFunctionDefinitions + 1);
      SetMaxSubtreeCount(StartSymbol, maxFunctionDefinitions + 1);
      
      // ADF branches maxFunctionDefinitions 
      for (int argumentIndex = 1; argumentIndex < maxFunctionDefinitions + 1; argumentIndex++) {
        SetAllowedChild(StartSymbol, defunSymbol, argumentIndex);
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      GlobalSymbolicExpressionGrammar clone = (GlobalSymbolicExpressionGrammar)base.Clone(cloner);
      clone.defunSymbol = defunSymbol;
      clone.maxFunctionArguments = maxFunctionArguments;
      clone.maxFunctionDefinitions = maxFunctionDefinitions;
      clone.minFunctionArguments = minFunctionArguments;
      clone.minFunctionDefinitions = minFunctionDefinitions;
      return clone;
    }
  }
}
