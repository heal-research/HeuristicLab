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

using System.ComponentModel;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.LinearAssignment {
  [Item("LAP Assignment", "Represents a solution to the LAP.")]
  [StorableType("8F255E15-291B-44D5-BF8B-AC79264274C0")]
  public sealed class LAPAssignment : Item, INotifyPropertyChanged, IStorableContent {
    public string Filename { get; set; }

    [Storable]
    private DoubleMatrix costs;
    public DoubleMatrix Costs {
      get { return costs; }
      set {
        bool changed = (costs != value);
        costs = value;
        if (changed) OnPropertyChanged("Costs");
      }
    }

    [Storable]
    private StringArray rowNames;
    public StringArray RowNames {
      get { return rowNames; }
      set {
        bool changed = (rowNames != value);
        rowNames = value;
        if (changed) OnPropertyChanged("RowNames");
      }
    }

    [Storable]
    private StringArray columnNames;
    public StringArray ColumnNames {
      get { return columnNames; }
      set {
        bool changed = (columnNames != value);
        columnNames = value;
        if (changed) OnPropertyChanged("ColumnNames");
      }
    }

    [Storable]
    private Permutation assignment;
    public Permutation Assignment {
      get { return assignment; }
      set {
        bool changed = (assignment != value);
        assignment = value;
        if (changed) OnPropertyChanged("Assignment");
      }
    }

    [Storable]
    private DoubleValue quality;
    public DoubleValue Quality {
      get { return quality; }
      set {
        bool changed = (quality != value);
        quality = value;
        if (changed) OnPropertyChanged("Quality");
      }
    }

    [StorableConstructor]
    private LAPAssignment(StorableConstructorFlag _) : base(_) { }
    private LAPAssignment(LAPAssignment original, Cloner cloner)
      : base(original, cloner) {
      costs = cloner.Clone(original.costs);
      assignment = cloner.Clone(original.assignment);
      rowNames = cloner.Clone(original.rowNames);
      columnNames = cloner.Clone(original.columnNames);
      quality = cloner.Clone(original.quality);
    }
    public LAPAssignment(DoubleMatrix costs, Permutation assignment) {
      this.costs = costs;
      this.assignment = assignment;
    }
    public LAPAssignment(DoubleMatrix costs, Permutation assignment, DoubleValue quality)
      : this(costs, assignment) {
      this.quality = quality;
    }
    public LAPAssignment(DoubleMatrix costs, StringArray rowNames, StringArray columnNames, Permutation assignment)
      : this(costs, assignment) {
      this.rowNames = rowNames;
      this.columnNames = columnNames;
    }
    public LAPAssignment(DoubleMatrix costs, StringArray rowNames, StringArray columnNames, Permutation assignment, DoubleValue quality)
      : this(costs, rowNames, columnNames, assignment) {
      this.quality = quality;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LAPAssignment(this, cloner);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propertyName) {
      var handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
