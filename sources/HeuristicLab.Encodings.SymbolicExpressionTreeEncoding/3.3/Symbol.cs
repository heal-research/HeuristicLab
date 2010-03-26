#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Core;
using System.Linq;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  [Item("Symbol", "Represents a symbol in a symbolic function tree.")]
  public abstract class Symbol : Item {
    private List<List<Symbol>> allowedSubFunctions = new List<List<Symbol>>();
    private int minArity = -1;
    private int maxArity = -1;
    private double tickets = 1.0;
    private IOperator initializer;
    private IOperator manipulator;
    private int minTreeHeight = -1;
    private int minTreeSize = -1;

    private string name;
    public virtual string Name {
      get { return name; }
      set {
        if (string.IsNullOrEmpty(value)) throw new ArgumentException();
        if (value != name) {
          name = value;
        }
      }
    }

    protected Symbol() {
      name = this.GetType().Name;
    }

    public int MinSubTrees {
      get {
        return minArity;
      }
      protected internal set {
        if (value < 0) throw new ArgumentException();
        if (minArity != value) {
          minArity = value;
          while (minArity > allowedSubFunctions.Count) allowedSubFunctions.Add(new List<Symbol>());
          ResetCachedValues();
        }
      }
    }

    public int MaxSubTrees {
      get {
        return maxArity;
      }
      protected internal set {
        if (value < 0) throw new ArgumentException();
        if (value < minArity) throw new ArgumentException();
        if (value != maxArity) {
          maxArity = value;
          while (allowedSubFunctions.Count > maxArity) allowedSubFunctions.RemoveAt(allowedSubFunctions.Count - 1);
          while (maxArity > allowedSubFunctions.Count) {
            if (allowedSubFunctions.Count > 0) {
              // copy the list of allowed sub-functions from the previous slot
              allowedSubFunctions.Add(new List<Symbol>(allowedSubFunctions[allowedSubFunctions.Count - 1]));
            } else {
              // add empty list
              allowedSubFunctions.Add(new List<Symbol>());
            }
          }
          ResetCachedValues();
        }
      }
    }


    public int MinTreeSize {
      get {
        if (minTreeSize <= 0) {
          RecalculateMinimalTreeSize();
        }
        // Debug.Assert(minTreeSize > 0);
        return minTreeSize;
      }
    }

    public int MinTreeHeight {
      get {
        if (minTreeHeight <= 0) {
          RecalculateMinimalTreeHeight();
        }
        // Debug.Assert(minTreeHeight > 0);
        return minTreeHeight;
      }
    }

    public double Tickets {
      get { return tickets; }
      set {
        if (value < 0.0) throw new ArgumentException("Number of tickets must be positive");
        if (value != tickets) {
          tickets = value;
        }
      }
    }

    public IOperator Initializer {
      get { return initializer; }
      set {
        if (initializer != value) {
          initializer = value;
        }
      }
    }

    public IOperator Manipulator {
      get { return manipulator; }
      set {
        if (manipulator != value) {
          manipulator = value;
        }
      }
    }

    public virtual SymbolicExpressionTreeNode CreateTreeNode() {
      return new SymbolicExpressionTreeNode(this);
    }

    public IEnumerable<Symbol> GetAllowedSubFunctions(int index) {
      if (index < 0 || index > MaxSubTrees) throw new ArgumentException("Index outside of allowed range. index = " + index);
      return allowedSubFunctions[index];
    }

    public void AddAllowedSubFunction(Symbol symbol, int index) {
      if (index < 0 || index > MaxSubTrees) throw new ArgumentException("Index outside of allowed range. index = " + index);
      if (allowedSubFunctions[index] == null) {
        allowedSubFunctions[index] = new List<Symbol>();
      }
      if (!allowedSubFunctions[index].Contains(symbol)) {
        allowedSubFunctions[index].Add(symbol);
      }
      ResetCachedValues();
    }
    public void RemoveAllowedSubFunction(Symbol symbol, int index) {
      if (index < 0 || index > MaxSubTrees) throw new ArgumentException("Index outside of allowed range. index = " + index);
      if (allowedSubFunctions[index].Contains(symbol)) {
        allowedSubFunctions[index].Remove(symbol);
        ResetCachedValues();
      }
    }

    private void ResetCachedValues() {
      minTreeHeight = -1;
      minTreeSize = -1;
    }

    public bool IsAllowedSubFunction(Symbol symbol, int index) {
      return GetAllowedSubFunctions(index).Contains(symbol);
    }

    private void RecalculateMinimalTreeSize() {
      if (MinSubTrees == 0) minTreeSize = 1;
      else {
        minTreeSize = int.MaxValue; // prevent infinite recursion        
        minTreeSize = 1 + (from slot in Enumerable.Range(0, MinSubTrees)
                           let minForSlot = (from function in GetAllowedSubFunctions(slot)
                                             where function != this
                                             select function.MinTreeSize).DefaultIfEmpty(0).Min()
                           select minForSlot).Sum();
      }
    }

    private void RecalculateMinimalTreeHeight() {
      if (MinSubTrees == 0) minTreeHeight = 1;
      else {
        minTreeHeight = int.MaxValue;
        minTreeHeight = 1 + (from slot in Enumerable.Range(0, MinSubTrees)
                             let minForSlot = (from function in GetAllowedSubFunctions(slot)
                                               where function != this
                                               select function.MinTreeHeight).DefaultIfEmpty(0).Min()
                             select minForSlot).Max();
      }
    }

    public override string ToString() {
      return name;
    }
  }
}
