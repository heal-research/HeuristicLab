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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.ArtificialAnt {
  /// <summary>
  /// Represents a trail of an artificial ant which can be visualized in the GUI.
  /// </summary>
  [Item("AntTrail", "Represents a trail of an artificial ant which can be visualized in the GUI.")]
  [StorableClass]
  public sealed class AntTrail : Item {
    private SymbolicExpressionTree expression;
    [Storable]
    public SymbolicExpressionTree SymbolicExpressionTree {
      get { return expression; }
      set {
        if (expression != value) {
          //if (expression != null) DeregisterSymbolicExpressionTreeEvents();
          expression = value;
          //if (expression != null) RegisterSymbolicExpressionTreeEvents();
          OnSymbolicExpressionTreeChanged();
        }
      }
    }

    public AntTrail() : base() { }
    public AntTrail(SymbolicExpressionTree expression)
      : this() {
      SymbolicExpressionTree = expression;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      AntTrail clone = new AntTrail();
      cloner.RegisterClonedObject(this, clone);
      clone.expression = (SymbolicExpressionTree)cloner.Clone(expression);
      return clone;
    }

    //#region Events
    public event EventHandler SymbolicExpressionTreeChanged;
    private void OnSymbolicExpressionTreeChanged() {
      if (SymbolicExpressionTreeChanged != null)
        SymbolicExpressionTreeChanged(this, EventArgs.Empty);
    }

    //private void RegisterSymbolicExpressionTreeEvents() {
    //  SymbolicExpressionTree.ItemChanged += new EventHandler<EventArgs<int>>(SymbolicExpressionTree_ItemChanged);
    //  SymbolicExpressionTree.Reset += new EventHandler(SymbolicExpressionTree_Reset);
    //}
    //private void DeregisterSymbolicExpressionTreeEvents() {
    //  SymbolicExpressionTree.ItemChanged -= new EventHandler<EventArgs<int>>(SymbolicExpressionTree_ItemChanged);
    //  SymbolicExpressionTree.Reset -= new EventHandler(SymbolicExpressionTree_Reset);
    //}

    //private void SymbolicExpressionTree_ItemChanged(object sender, EventArgs<int> e) {
    //  OnSymbolicExpressionTreeChanged();
    //}
    //private void SymbolicExpressionTree_Reset(object sender, EventArgs e) {
    //  OnSymbolicExpressionTreeChanged();
    //}
    //#endregion
  }
}
