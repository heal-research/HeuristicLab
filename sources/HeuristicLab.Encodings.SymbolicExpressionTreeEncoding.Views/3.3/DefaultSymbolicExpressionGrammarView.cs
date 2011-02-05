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
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  [View("Symbolic Expression Grammar View")]
  [Content(typeof(DefaultSymbolicExpressionGrammar), true)]
  public partial class DefaultSymbolicExpressionGrammarView : ItemView {
    private CheckedItemList<Symbol> symbols;

    public new DefaultSymbolicExpressionGrammar Content {
      get { return (DefaultSymbolicExpressionGrammar)base.Content; }
      set { base.Content = value; }
    }

    public DefaultSymbolicExpressionGrammarView() {
      InitializeComponent();
      symbols = new CheckedItemList<Symbol>();
      symbols.CheckedItemsChanged += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<HeuristicLab.Collections.IndexedItem<Symbol>>(symbols_CheckedItemsChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateControl();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      checkedItemListView.Enabled = Content != null;
      checkedItemListView.ReadOnly = ReadOnly;
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
        foreach (Symbol symbol in Content.Symbols) {
          if (!(symbol is ReadOnlySymbol)) {
            symbol.Changed += new EventHandler(symbol_Changed);
            symbols.Add(symbol, symbol.InitialFrequency > 0.0);
          }
        }
        checkedItemListView.Content = symbols.AsReadOnly();
      }
      SetEnabledStateOfControls();
    }


    private void symbol_Changed(object sender, EventArgs e) {
      Symbol symbol = (Symbol)sender;
      symbols.SetItemCheckedState(symbol, symbol.InitialFrequency > 0.0);
    }

    private void symbols_CheckedItemsChanged(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<HeuristicLab.Collections.IndexedItem<Symbol>> e) {
      ICheckedItemList<Symbol> checkedItemList = (ICheckedItemList<Symbol>)sender;
      foreach (var indexedItem in e.Items) {
        if (checkedItemList.ItemChecked(indexedItem.Value)) {
          indexedItem.Value.InitialFrequency = 1.0;
        } else {
          indexedItem.Value.InitialFrequency = 0.0;
        }
      }
    }
    private void ClearSymbols() {
      foreach (Symbol s in symbols)
        s.Changed -= new EventHandler(symbol_Changed);
      symbols.Clear();
    }
    #endregion
  }
}
