#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.QuadraticAssignment {
  [Item("QAP Assignment", "Represents a solution to the QAP.")]
  [StorableClass]
  public sealed class QAPAssignment : Item, INotifyPropertyChanged {

    [Storable]
    private DoubleMatrix distances;
    public DoubleMatrix Distances {
      get { return distances; }
      set {
        bool changed = (distances != value);
        distances = value;
        if (changed) OnPropertyChanged("Distances");
      }
    }

    [Storable]
    private DoubleMatrix weights;
    public DoubleMatrix Weights {
      get { return weights; }
      set {
        bool changed = (weights != value);
        weights = value;
        if (changed) OnPropertyChanged("Weights");
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
    private QAPAssignment(bool deserializing) : base(deserializing) { }
    private QAPAssignment(QAPAssignment original, Cloner cloner)
      : base(original, cloner) {
      distances = cloner.Clone(original.distances);
      weights = cloner.Clone(original.weights);
      assignment = cloner.Clone(original.assignment);
      quality = cloner.Clone(original.quality);
    }
    public QAPAssignment(DoubleMatrix weights, Permutation assignment) {
      this.weights = weights;
      this.assignment = assignment;
    }
    public QAPAssignment(DoubleMatrix weights, Permutation assignment, DoubleValue quality)
      : this(weights, assignment) {
      this.quality = quality;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QAPAssignment(this, cloner);
    }


    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propertyName) {
      var handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
