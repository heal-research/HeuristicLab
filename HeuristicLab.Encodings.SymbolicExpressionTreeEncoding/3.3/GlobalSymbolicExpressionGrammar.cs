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

using System;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  [Item("GlobalSymbolicExpressionGrammar", "Represents a grammar that defines the syntax of symbolic expression trees.")]
  public sealed class GlobalSymbolicExpressionGrammar : DefaultSymbolicExpressionGrammar {
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

    [StorableConstructor]
    private GlobalSymbolicExpressionGrammar(bool deserializing) : base(deserializing) { }
    private GlobalSymbolicExpressionGrammar(GlobalSymbolicExpressionGrammar original, Cloner cloner)
      : base(original, cloner) {
      defunSymbol = (Defun)cloner.Clone(original.defunSymbol);
      maxFunctionArguments = original.maxFunctionArguments;
      minFunctionArguments = original.minFunctionArguments;
      maxFunctionDefinitions = original.maxFunctionDefinitions;
      minFunctionDefinitions = original.minFunctionDefinitions;
    }

    public GlobalSymbolicExpressionGrammar(ISymbolicExpressionGrammar mainBranchGrammar)
      : base(mainBranchGrammar) {
      maxFunctionArguments = 3;
      maxFunctionDefinitions = 3;

      ProgramRootSymbol programRootSymbol = Symbols.OfType<ProgramRootSymbol>().FirstOrDefault();
      if (programRootSymbol == null) {
        programRootSymbol = new ProgramRootSymbol();
        AddSymbol(programRootSymbol);
      }
      StartSymbol = programRootSymbol;

      defunSymbol = Symbols.OfType<Defun>().FirstOrDefault();
      if (defunSymbol == null) {
        defunSymbol = new Defun();
        AddSymbol(defunSymbol);
      }

      SetMinSubtreeCount(StartSymbol, minFunctionDefinitions + 1); // min number of ADF + 1 for RPB
      SetMaxSubtreeCount(StartSymbol, maxFunctionDefinitions + 1); // max number of ADF + 1 for RPB
      // defun can have only one child
      SetMinSubtreeCount(defunSymbol, 1); 
      SetMaxSubtreeCount(defunSymbol, 1);


      // the start symbol of the mainBranchGrammar is allowed as the result producing branch
      SetAllowedChild(StartSymbol, Symbols.Where(s => s.Name == mainBranchGrammar.StartSymbol.Name).First(), 0);

      // defuns are allowed as children on the same level and after RPB
      for (int i = 0; i < maxFunctionDefinitions; i++) {
        // +1 because RPB has index 0
        SetAllowedChild(StartSymbol, defunSymbol, i + 1); 
      }

      // every symbol of the mainBranchGrammar that is allowed as child of the start symbol is also allowed as direct child of defun
      foreach (var symb in mainBranchGrammar.Symbols) {
        if (mainBranchGrammar.IsAllowedChild(mainBranchGrammar.StartSymbol, symb, 0))
          SetAllowedChild(defunSymbol, Symbols.Where(s => s.Name == symb.Name).First(), 0);
      }
    }

    [Obsolete]
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
      return new GlobalSymbolicExpressionGrammar(this, cloner);
    }
  }
}
