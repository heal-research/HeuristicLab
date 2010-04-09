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
using HeuristicLab.Core;
using System.Xml;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.GeneralSymbols;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  [Item("DefaultSymbolicExpressionGrammar", "Represents a grammar that defines the syntax of symbolic expression trees.")]
  public class DefaultSymbolicExpressionGrammar : Item, ISymbolicExpressionGrammar {
    [Storable]
    private int minFunctionDefinitions;
    [Storable]
    private int maxFunctionDefinitions;
    [Storable]
    private int minFunctionArguments;
    [Storable]
    private int maxFunctionArguments;

    [Storable]
    private Dictionary<string, int> minSubTreeCount;
    [Storable]
    private Dictionary<string, int> maxSubTreeCount;
    [Storable]
    private Dictionary<string, List<HashSet<string>>> allowedFunctions;
    [Storable]
    private HashSet<Symbol> allSymbols;

    public DefaultSymbolicExpressionGrammar(int minFunctionDefinitions, int maxFunctionDefinitions, int minFunctionArguments, int maxFunctionArguments)
      : base() {
      this.minFunctionDefinitions = minFunctionDefinitions;
      this.maxFunctionDefinitions = maxFunctionDefinitions;
      this.minFunctionArguments = minFunctionArguments;
      this.maxFunctionArguments = maxFunctionArguments;
      minSubTreeCount = new Dictionary<string, int>();
      maxSubTreeCount = new Dictionary<string, int>();
      allowedFunctions = new Dictionary<string, List<HashSet<string>>>();
      allSymbols = new HashSet<Symbol>();
      cachedMinExpressionLength = new Dictionary<Symbol, int>();
      cachedMaxExpressionLength = new Dictionary<Symbol, int>();
      cachedMinExpressionDepth = new Dictionary<Symbol, int>();
      Initialize();
    }

    private void Initialize() {
      programRootSymbol = new ProgramRootSymbol();
      var defunSymbol = new Defun();
      startSymbol = new StartSymbol();
      var invokeFunctionSymbol = new InvokeFunction();

      SetMinSubTreeCount(programRootSymbol, minFunctionDefinitions + 1);
      SetMaxSubTreeCount(programRootSymbol, maxFunctionDefinitions + 1);
      SetMinSubTreeCount(startSymbol, 1);
      SetMaxSubTreeCount(startSymbol, 1);
      SetMinSubTreeCount(defunSymbol, 1);
      SetMaxSubTreeCount(defunSymbol, 1);
      SetMinSubTreeCount(invokeFunctionSymbol, minFunctionArguments);
      SetMaxSubTreeCount(invokeFunctionSymbol, maxFunctionArguments);
      AddAllowedSymbols(programRootSymbol, 0, startSymbol);
      for (int argumentIndex = 1; argumentIndex < maxFunctionDefinitions + 1; argumentIndex++) {
        AddAllowedSymbols(programRootSymbol, argumentIndex, defunSymbol);
      }
    }

    public void AddAllowedSymbols(Symbol parent, int argumentIndex, Symbol allowedChild) {
      allSymbols.Add(parent); allSymbols.Add(allowedChild);
      if (!allowedFunctions.ContainsKey(parent.Name)) {
        allowedFunctions[parent.Name] = new List<HashSet<string>>();
      }
      while (allowedFunctions[parent.Name].Count <= argumentIndex)
        allowedFunctions[parent.Name].Add(new HashSet<string>());
      allowedFunctions[parent.Name][argumentIndex].Add(allowedChild.Name);
      ClearCaches();
    }

    public void SetMaxSubTreeCount(Symbol parent, int nSubTrees) {
      maxSubTreeCount[parent.Name] = nSubTrees;
      ClearCaches();
    }

    public void SetMinSubTreeCount(Symbol parent, int nSubTrees) {
      minSubTreeCount[parent.Name] = nSubTrees;
      ClearCaches();
    }

    private void ClearCaches() {
      cachedMinExpressionLength.Clear();
      cachedMaxExpressionLength.Clear();
      cachedMinExpressionDepth.Clear();
    }

    private void symbol_ToStringChanged(object sender, EventArgs e) {
      OnToStringChanged();
    }

    #region ISymbolicExpressionGrammar Members

    private Symbol programRootSymbol;
    public Symbol ProgramRootSymbol {
      get { return programRootSymbol; }
    }

    private Symbol startSymbol;
    public Symbol StartSymbol {
      get { return startSymbol; }
    }

    public IEnumerable<Symbol> GetAllowedSymbols(Symbol parent, int argumentIndex) {
      return from name in allowedFunctions[parent.Name][argumentIndex]
             from sym in allSymbols
             where name == sym.Name
             select sym;
    }


    private Dictionary<Symbol, int> cachedMinExpressionLength;
    public int GetMinExpressionLength(Symbol start) {
      if (!cachedMinExpressionLength.ContainsKey(start)) {
        cachedMinExpressionLength[start] = int.MaxValue; // prevent infinite recursion
        cachedMinExpressionLength[start] = 1 + (from argIndex in Enumerable.Range(0, GetMinSubTreeCount(start))
                                                let minForSlot = (from symbol in GetAllowedSymbols(start, argIndex)
                                                                  select GetMinExpressionLength(symbol)).DefaultIfEmpty(0).Min()
                                                select minForSlot).DefaultIfEmpty(0).Sum();
      }
      return cachedMinExpressionLength[start];
    }

    private Dictionary<Symbol, int> cachedMaxExpressionLength;
    public int GetMaxExpressionLength(Symbol start) {
      if (!cachedMaxExpressionLength.ContainsKey(start)) {
        cachedMaxExpressionLength[start] = int.MaxValue; // prevent infinite recursion
        long sumOfMaxTrees = 1 + (from argIndex in Enumerable.Range(0, GetMaxSubTreeCount(start))
                                  let maxForSlot = (long)(from symbol in GetAllowedSymbols(start, argIndex)
                                                          select GetMaxExpressionLength(symbol)).DefaultIfEmpty(0).Max()
                                  select maxForSlot).DefaultIfEmpty(0).Sum();
        long limit = int.MaxValue;
        cachedMaxExpressionLength[start] = (int)Math.Min(sumOfMaxTrees, limit);
      }
      return cachedMaxExpressionLength[start];
    }

    private Dictionary<Symbol, int> cachedMinExpressionDepth;
    public int GetMinExpressionDepth(Symbol start) {
      if (!cachedMinExpressionDepth.ContainsKey(start)) {
        cachedMinExpressionDepth[start] = int.MaxValue; // prevent infinite recursion
        cachedMinExpressionDepth[start] = 1 + (from argIndex in Enumerable.Range(0, GetMinSubTreeCount(start))
                                               let minForSlot = (from symbol in GetAllowedSymbols(start, argIndex)
                                                                 select GetMinExpressionDepth(symbol)).DefaultIfEmpty(0).Min()
                                               select minForSlot).DefaultIfEmpty(0).Max();
      }
      return cachedMinExpressionDepth[start];
    }

    public int GetMinSubTreeCount(Symbol start) {
      return minSubTreeCount[start.Name];
    }

    public int GetMaxSubTreeCount(Symbol start) {
      return maxSubTreeCount[start.Name];
    }

    public bool IsValidExpression(SymbolicExpressionTree expression) {
      if (expression.Root.Symbol != ProgramRootSymbol) return false;
      // check dynamic symbols
      foreach (var branch in expression.Root.SubTrees) {
        foreach (var dynamicNode in branch.DynamicSymbols) {
          if (!dynamicNode.StartsWith("ARG")) {
            if (FindDefinitionOfDynamicFunction(expression.Root, dynamicNode) == null) return false;
          }
        }
      }
      return IsValidExpression(expression.Root);
    }

    #endregion
    private bool IsValidExpression(SymbolicExpressionTreeNode root) {
      if (root.SubTrees.Count < GetMinSubTreeCount(root.Symbol)) return false;
      if (root.SubTrees.Count > GetMaxSubTreeCount(root.Symbol)) return false;
      if (root.Symbol is Defun || root.Symbol is StartSymbol) {
        // check references to dynamic symbols
        if (!CheckDynamicSymbolsInBranch(root, root.SubTrees[0])) return false;
      }
      for (int i = 0; i < root.SubTrees.Count; i++) {
        if (!GetAllowedSymbols(root.Symbol, i).Contains(root.SubTrees[i].Symbol)) return false;
        if (!IsValidExpression(root.SubTrees[i])) return false;
      }
      return true;
    }

    private SymbolicExpressionTreeNode FindDefinitionOfDynamicFunction(SymbolicExpressionTreeNode root, string dynamicNode) {
      return (from node in root.SubTrees.OfType<DefunTreeNode>()
              where node.Name == dynamicNode
              select node).FirstOrDefault();
    }

    private bool CheckDynamicSymbolsInBranch(SymbolicExpressionTreeNode root, SymbolicExpressionTreeNode node) {
      var argNode = node as ArgumentTreeNode;
      var invokeNode = node as InvokeFunctionTreeNode;
      if (argNode != null) {
        if (!root.DynamicSymbols.Contains("ARG" + argNode.ArgumentIndex)) return false;
      } else if (invokeNode != null) {
        if (!root.DynamicSymbols.Contains(invokeNode.InvokedFunctionName)) return false;
        if (root.GetDynamicSymbolArgumentCount(invokeNode.InvokedFunctionName) != invokeNode.SubTrees.Count()) return false;
      }
      foreach (var subtree in node.SubTrees) {
        if (!CheckDynamicSymbolsInBranch(root, subtree)) return false;
      }
      return true;
    }

  }
}
