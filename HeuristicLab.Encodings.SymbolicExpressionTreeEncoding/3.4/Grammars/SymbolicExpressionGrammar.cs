#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Fossil;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableType("925BDEE9-0D46-48DB-BA1B-DCE82C0480B3")]
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

    private readonly ISymbolicExpressionTreeGrammar emptyGrammar;
    #endregion

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      foreach (ISymbol symbol in symbols.Values)
        RegisterSymbolEvents(symbol);
    }

    [StorableConstructor]
    protected SymbolicExpressionGrammar(StorableConstructorFlag _) : base(_) {
      emptyGrammar = new EmptySymbolicExpressionTreeGrammar(this);
    }
    protected SymbolicExpressionGrammar(SymbolicExpressionGrammar original, Cloner cloner)
      : base(original, cloner) {
      emptyGrammar = new EmptySymbolicExpressionTreeGrammar(this);

      foreach (ISymbol symbol in symbols.Values)
        RegisterSymbolEvents(symbol);

      programRootSymbol = cloner.Clone(original.programRootSymbol);
      startSymbol = cloner.Clone(original.StartSymbol);
      defunSymbol = cloner.Clone(original.defunSymbol);

      maximumFunctionArguments = original.maximumFunctionArguments;
      minimumFunctionArguments = original.minimumFunctionArguments;
      maximumFunctionDefinitions = original.maximumFunctionDefinitions;
      minimumFunctionDefinitions = original.minimumFunctionDefinitions;
    }

    protected SymbolicExpressionGrammar(string name, string description)
      : base(name, description) {
      emptyGrammar = new EmptySymbolicExpressionTreeGrammar(this);

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

    public ISymbolicExpressionTreeGrammar CreateExpressionTreeGrammar() {
      if (MaximumFunctionDefinitions == 0) return emptyGrammar;
      else return new SymbolicExpressionTreeGrammar(this);
    }

    public override sealed void AddSymbol(ISymbol symbol) {
      if (ReadOnly) throw new InvalidOperationException();
      base.AddSymbol(symbol);
      RegisterSymbolEvents(symbol);
      OnChanged();
    }
    public override sealed void RemoveSymbol(ISymbol symbol) {
      if (ReadOnly) throw new InvalidOperationException();
      DeregisterSymbolEvents(symbol);
      base.RemoveSymbol(symbol);
      OnChanged();
    }

    public event EventHandler ReadOnlyChanged;
    protected virtual void OnReadOnlyChanged() {
      var handler = ReadOnlyChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }

    #region IStatefulItem methods
    void IStatefulItem.InitializeState() {
      ReadOnly = false;
    }
    void IStatefulItem.ClearState() {
      ReadOnly = false;
    }
    #endregion

    public sealed override void AddAllowedChildSymbol(ISymbol parent, ISymbol child) {
      if (ReadOnly) throw new InvalidOperationException();
      base.AddAllowedChildSymbol(parent, child);
    }
    public sealed override void AddAllowedChildSymbol(ISymbol parent, ISymbol child, int argumentIndex) {
      if (ReadOnly) throw new InvalidOperationException();
      base.AddAllowedChildSymbol(parent, child, argumentIndex);
    }
    public sealed override void RemoveAllowedChildSymbol(ISymbol parent, ISymbol child) {
      if (ReadOnly) throw new InvalidOperationException();
      base.RemoveAllowedChildSymbol(parent, child);
    }
    public sealed override void RemoveAllowedChildSymbol(ISymbol parent, ISymbol child, int argumentIndex) {
      if (ReadOnly) throw new InvalidOperationException();
      base.RemoveAllowedChildSymbol(parent, child, argumentIndex);
    }

    public sealed override void SetSubtreeCount(ISymbol symbol, int minimumSubtreeCount, int maximumSubtreeCount) {
      if (ReadOnly) throw new InvalidOperationException();
      base.SetSubtreeCount(symbol, minimumSubtreeCount, maximumSubtreeCount);
    }

    private bool suppressEvents = false;
    public void StartGrammarManipulation() {
      suppressEvents = true;
    }
    public void FinishedGrammarManipulation() {
      suppressEvents = false;
      OnChanged();
    }

    protected sealed override void OnChanged() {
      if (suppressEvents) return;
      base.OnChanged();
    }

    #region symbol events
    private void RegisterSymbolEvents(ISymbol symbol) {
      foreach (var s in symbol.Flatten()) {
        s.NameChanging += new EventHandler<CancelEventArgs<string>>(Symbol_NameChanging);
        s.NameChanged += new EventHandler(Symbol_NameChanged);

        var groupSymbol = s as GroupSymbol;
        if (groupSymbol != null) RegisterGroupSymbolEvents(groupSymbol);
        else s.Changed += new EventHandler(Symbol_Changed);
      }
    }
    private void DeregisterSymbolEvents(ISymbol symbol) {
      foreach (var s in symbol.Flatten()) {
        s.NameChanging -= new EventHandler<CancelEventArgs<string>>(Symbol_NameChanging);
        s.NameChanged -= new EventHandler(Symbol_NameChanged);

        var groupSymbol = s as GroupSymbol;
        if (groupSymbol != null) DeregisterGroupSymbolEvents(groupSymbol);
        else s.Changed -= new EventHandler(Symbol_Changed);
      }
    }

    private void RegisterGroupSymbolEvents(GroupSymbol groupSymbol) {
      groupSymbol.Changed += new EventHandler(GroupSymbol_Changed);
      groupSymbol.SymbolsCollection.ItemsAdded += new Collections.CollectionItemsChangedEventHandler<ISymbol>(GroupSymbol_ItemsAdded);
      groupSymbol.SymbolsCollection.ItemsRemoved += new Collections.CollectionItemsChangedEventHandler<ISymbol>(GroupSymbol_ItemsRemoved);
      groupSymbol.SymbolsCollection.CollectionReset += new Collections.CollectionItemsChangedEventHandler<ISymbol>(GroupSymbol_CollectionReset);
    }
    private void DeregisterGroupSymbolEvents(GroupSymbol groupSymbol) {
      groupSymbol.Changed -= new EventHandler(GroupSymbol_Changed);
      groupSymbol.SymbolsCollection.ItemsAdded -= new Collections.CollectionItemsChangedEventHandler<ISymbol>(GroupSymbol_ItemsAdded);
      groupSymbol.SymbolsCollection.ItemsRemoved -= new Collections.CollectionItemsChangedEventHandler<ISymbol>(GroupSymbol_ItemsRemoved);
      groupSymbol.SymbolsCollection.CollectionReset -= new Collections.CollectionItemsChangedEventHandler<ISymbol>(GroupSymbol_CollectionReset);
    }

    private void Symbol_Changed(object sender, EventArgs e) {
      ClearCaches();
      OnChanged();
    }

    private void GroupSymbol_Changed(object sender, EventArgs e) {
      GroupSymbol groupSymbol = (GroupSymbol)sender;
      foreach (ISymbol symbol in groupSymbol.Flatten())
        symbol.Enabled = groupSymbol.Enabled;

      ClearCaches();
      OnChanged();
    }

    private void Symbol_NameChanging(object sender, CancelEventArgs<string> e) {
      if (symbols.ContainsKey(e.Value)) e.Cancel = true;
    }
    private void Symbol_NameChanged(object sender, EventArgs e) {
      ISymbol symbol = (ISymbol)sender;
      string oldName = symbols.First(x => x.Value == symbol).Key;
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
      OnChanged();
    }

    private void GroupSymbol_ItemsAdded(object sender, CollectionItemsChangedEventArgs<ISymbol> e) {
      foreach (ISymbol symbol in e.Items)
        if (!ContainsSymbol(symbol))
          AddSymbol(symbol);
      OnChanged();
    }
    private void GroupSymbol_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<ISymbol> e) {
      foreach (ISymbol symbol in e.Items)
        if (ContainsSymbol(symbol))
          RemoveSymbol(symbol);
      OnChanged();
    }
    private void GroupSymbol_CollectionReset(object sender, CollectionItemsChangedEventArgs<ISymbol> e) {
      foreach (ISymbol symbol in e.Items)
        if (!ContainsSymbol(symbol))
          AddSymbol(symbol);
      foreach (ISymbol symbol in e.OldItems)
        if (ContainsSymbol(symbol))
          RemoveSymbol(symbol);
      OnChanged();
    }
    #endregion
  }
}
