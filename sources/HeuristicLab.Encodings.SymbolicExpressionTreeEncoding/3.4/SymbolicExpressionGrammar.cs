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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Collections.Generic;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  public abstract class SymbolicExpressionGrammar : SymbolicExpressionGrammarBase, ISymbolicExpressionGrammar {
    #region fields & properties
    [Storable(DefaultValue = false)]
    private bool readOnly;
    public bool ReadOnly {
      get { return readOnly; }
      set {
        if (readOnly != value) {
          readOnly = value;
          OnReadOnlyChanged();
        }
      }
    }

    [Storable]
    private int minimumFunctionDefinitions;
    public int MinimumFunctionDefinitions {
      get { return minimumFunctionDefinitions; }
      set {
        minimumFunctionDefinitions = value;
        UpdateAdfConstraints();
      }
    }
    [Storable]
    private int maximumFunctionDefinitions;
    public int MaximumFunctionDefinitions {
      get { return maximumFunctionDefinitions; }
      set {
        maximumFunctionDefinitions = value;
        UpdateAdfConstraints();
      }
    }
    [Storable]
    private int minimumFunctionArguments;
    public int MinimumFunctionArguments {
      get { return minimumFunctionArguments; }
      set { minimumFunctionArguments = value; }
    }
    [Storable]
    private int maximumFunctionArguments;
    public int MaximumFunctionArguments {
      get { return maximumFunctionArguments; }
      set { maximumFunctionArguments = value; }
    }

    private ProgramRootSymbol programRootSymbol;
    public ProgramRootSymbol ProgramRootSymbol {
      get { return programRootSymbol; }
    }
    ISymbol ISymbolicExpressionGrammar.ProgramRootSymbol {
      get { return ProgramRootSymbol; }
    }
    [Storable(Name = "ProgramRootSymbol")]
    private ISymbol StorableProgramRootSymbol {
      get { return programRootSymbol; }
      set { programRootSymbol = (ProgramRootSymbol)value; }
    }

    private StartSymbol startSymbol;
    public StartSymbol StartSymbol {
      get { return startSymbol; }
    }
    ISymbol ISymbolicExpressionGrammar.StartSymbol {
      get { return StartSymbol; }
    }
    [Storable(Name = "StartSymbol")]
    private ISymbol StorableStartSymbol {
      get { return startSymbol; }
      set { startSymbol = (StartSymbol)value; }
    }

    private Defun defunSymbol;
    protected Defun DefunSymbol {
      get { return defunSymbol; }
    }
    [Storable(Name = "DefunSymbol")]
    private ISymbol StorableDefunSymbol {
      get { return defunSymbol; }
      set { defunSymbol = (Defun)value; }
    }
    #endregion

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      foreach (ISymbol symbol in symbols.Values)
        RegisterSymbolEvents(symbol);
    }
    [StorableConstructor]
    protected SymbolicExpressionGrammar(bool deserializing) : base(deserializing) { }
    protected SymbolicExpressionGrammar(SymbolicExpressionGrammar original, Cloner cloner)
      : base(original, cloner) {
      foreach (ISymbol symbol in Symbols)
        RegisterSymbolEvents(symbol);

      programRootSymbol = cloner.Clone(original.programRootSymbol);
      startSymbol = cloner.Clone(original.StartSymbol);
      defunSymbol = cloner.Clone(original.defunSymbol);

      maximumFunctionArguments = original.maximumFunctionArguments;
      minimumFunctionArguments = original.minimumFunctionArguments;
      maximumFunctionDefinitions = original.maximumFunctionDefinitions;
      minimumFunctionDefinitions = original.minimumFunctionDefinitions;
    }

    public SymbolicExpressionGrammar(string name, string description)
      : base(name, description) {
      programRootSymbol = new ProgramRootSymbol();
      AddSymbol(programRootSymbol);
      SetSubtreeCount(programRootSymbol, 1, 1);

      startSymbol = new StartSymbol();
      AddSymbol(startSymbol);
      SetSubtreeCount(startSymbol, 1, 1);

      defunSymbol = new Defun();
      AddSymbol(defunSymbol);
      SetSubtreeCount(defunSymbol, 1, 1);

      AddAllowedChildSymbol(programRootSymbol, startSymbol, 0);
      UpdateAdfConstraints();
    }

    private void UpdateAdfConstraints() {
      SetSubtreeCount(programRootSymbol, minimumFunctionDefinitions + 1, maximumFunctionDefinitions + 1);

      // ADF branches maxFunctionDefinitions 
      for (int argumentIndex = 1; argumentIndex < maximumFunctionDefinitions + 1; argumentIndex++) {
        RemoveAllowedChildSymbol(programRootSymbol, defunSymbol, argumentIndex);
        AddAllowedChildSymbol(programRootSymbol, defunSymbol, argumentIndex);
      }
    }

    protected override void AddSymbol(ISymbol symbol) {
      base.AddSymbol(symbol);
      RegisterSymbolEvents(symbol);
    }
    protected override void RemoveSymbol(ISymbol symbol) {
      DeregisterSymbolEvents(symbol);
      base.RemoveSymbol(symbol);
    }

    public event EventHandler ReadOnlyChanged;
    protected virtual void OnReadOnlyChanged() {
      var handler = ReadOnlyChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }

    #region IStatefulItem
    void IStatefulItem.InitializeState() { }
    void IStatefulItem.ClearState() {
      ReadOnly = false;
    }
    #endregion

    #region symbol events
    protected virtual void RegisterSymbolEvents(ISymbol symbol) {
      symbol.NameChanging += new EventHandler<CancelEventArgs<string>>(Symbol_NameChanging);
      symbol.NameChanged += new EventHandler(Symbol_NameChanged);
    }
    protected virtual void DeregisterSymbolEvents(ISymbol symbol) {
      symbol.NameChanging -= new EventHandler<CancelEventArgs<string>>(Symbol_NameChanging);
      symbol.NameChanged -= new EventHandler(Symbol_NameChanged);
    }

    private void Symbol_NameChanging(object sender, CancelEventArgs<string> e) {
      if (symbols.ContainsKey(e.Value)) e.Cancel = true;
    }
    private void Symbol_NameChanged(object sender, EventArgs e) {
      ISymbol symbol = (ISymbol)sender;
      string oldName = symbols.Where(x => x.Value == symbol).First().Key;
      string newName = symbol.Name;

      symbols.Remove(oldName);
      symbols.Add(newName, symbol);

      var subtreeCount = symbolSubtreeCount[oldName];
      symbolSubtreeCount.Remove(oldName);
      symbolSubtreeCount.Add(newName, subtreeCount);

      List<string> allowedChilds;
      if (allowedChildSymbols.TryGetValue(oldName, out allowedChilds)) {
        allowedChildSymbols.Remove(oldName);
        allowedChildSymbols.Add(newName, allowedChilds);
      }

      for (int i = 0; i < GetMaximumSubtreeCount(symbol); i++) {
        if (allowedChildSymbolsPerIndex.TryGetValue(Tuple.Create(oldName, i), out allowedChilds)) {
          allowedChildSymbolsPerIndex.Remove(Tuple.Create(oldName, i));
          allowedChildSymbolsPerIndex.Add(Tuple.Create(newName, i), allowedChilds);
        }
      }

      foreach (var parent in Symbols) {
        if (allowedChildSymbols.TryGetValue(parent.Name, out allowedChilds))
          if (allowedChilds.Remove(oldName))
            allowedChilds.Add(newName);

        for (int i = 0; i < GetMaximumSubtreeCount(parent); i++) {
          if (allowedChildSymbolsPerIndex.TryGetValue(Tuple.Create(parent.Name, i), out allowedChilds))
            if (allowedChilds.Remove(oldName)) allowedChilds.Add(newName);
        }
      }

      ClearCaches();
    }
    #endregion
  }
}
