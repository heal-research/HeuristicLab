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
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  [View("Symbolic Expression Grammar View")]
  [Content(typeof(ISymbolicExpressionGrammar), false)]
  public partial class SymbolicExpressionGrammarView : NamedItemView {
    private CheckedItemList<ISymbol> symbols;

    public new ISymbolicExpressionGrammar Content {
      get { return (ISymbolicExpressionGrammar)base.Content; }
      set { base.Content = value; }
    }

    public override bool ReadOnly {
      get {
        if ((Content != null) && Content.ReadOnly) return true;
        return base.ReadOnly;
      }
      set {
        if ((Content != null) && Content.ReadOnly) base.ReadOnly = true;
        else base.ReadOnly = value;
      }
    }

    public SymbolicExpressionGrammarView() {
      InitializeComponent();
      symbols = new CheckedItemList<ISymbol>();
      symbols.CheckedItemsChanged += new CollectionItemsChangedEventHandler<IndexedItem<ISymbol>>(symbols_CheckedItemsChanged);
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ReadOnlyChanged += new EventHandler(Content_ReadOnlyChanged);
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ReadOnlyChanged -= new EventHandler(Content_ReadOnlyChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateControl();
    }

    private void Content_ReadOnlyChanged(object sender, EventArgs e) {
      ReadOnly = Content.ReadOnly;
    }

    #region content event handlers
    private void Content_Changed(object sender, EventArgs e) {
      UpdateControl();
    }
    #endregion

    #region helpers
    private void UpdateControl() {
      if (Content == null) {
        ClearSymbols();
        checkedItemListView.Content = symbols.AsReadOnly();
      } else {
        ClearSymbols();
        foreach (ISymbol symbol in Content.Symbols) {
          if (!(symbol is IReadOnlySymbol)) {
            symbol.Changed += new EventHandler(symbol_Changed);
            symbols.Add(symbol, symbol.Enabled);
          }
        }
        checkedItemListView.Content = symbols.AsReadOnly();
      }
      SetEnabledStateOfControls();
    }


    private void symbol_Changed(object sender, EventArgs e) {
      ISymbol symbol = (ISymbol)sender;
      symbols.SetItemCheckedState(symbol, symbol.Enabled);
    }

    private void symbols_CheckedItemsChanged(object sender, CollectionItemsChangedEventArgs<IndexedItem<ISymbol>> e) {
      ICheckedItemList<ISymbol> checkedItemList = (ICheckedItemList<ISymbol>)sender;
      foreach (var indexedItem in e.Items)
        indexedItem.Value.Enabled = checkedItemList.ItemChecked(indexedItem.Value);
    }
    private void ClearSymbols() {
      foreach (Symbol s in symbols)
        s.Changed -= new EventHandler(symbol_Changed);
      symbols.Clear();
    }
    #endregion
  }
}
