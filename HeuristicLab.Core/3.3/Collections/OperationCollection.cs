#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [StorableClass]
  public sealed class OperationCollection : DeepCloneable, IList<IOperation>, IOperation {
    [Storable]
    private IList<IOperation> operations;

    [Storable]
    private bool parallel;
    public bool Parallel {
      get { return parallel; }
      set { parallel = value; }
    }

    [StorableConstructor]
    private OperationCollection(bool deserializing) { }
    private OperationCollection(OperationCollection original, Cloner cloner)
      : base(original, cloner) {
      operations = new List<IOperation>(original.Select(x => cloner.Clone(x)));
      parallel = original.parallel;
    }
    public OperationCollection() {
      operations = new List<IOperation>();
      parallel = false;
    }
    public OperationCollection(IEnumerable<IOperation> collection) {
      operations = new List<IOperation>(collection.Where(e => e != null));
      parallel = false;
    }
    public OperationCollection(params IOperation[] list) {
      operations = new List<IOperation>(list.Where(e => e != null));
      parallel = false;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OperationCollection(this, cloner);
    }

    #region IList<IOperation> Members
    public int IndexOf(IOperation item) {
      return operations.IndexOf(item);
    }
    public void Insert(int index, IOperation item) {
      if (item != null) operations.Insert(index, item);
    }
    public void RemoveAt(int index) {
      operations.RemoveAt(index);
    }
    public IOperation this[int index] {
      get { return operations[index]; }
      set { if (value != null) operations[index] = value; }
    }
    #endregion

    #region ICollection<IOperation> Members
    public void Add(IOperation item) {
      if (item != null) operations.Add(item);
    }
    public void Clear() {
      operations.Clear();
    }
    public bool Contains(IOperation item) {
      return operations.Contains(item);
    }
    public void CopyTo(IOperation[] array, int arrayIndex) {
      operations.CopyTo(array, arrayIndex);
    }
    public int Count {
      get { return operations.Count; }
    }
    public bool IsReadOnly {
      get { return operations.IsReadOnly; }
    }
    public bool Remove(IOperation item) {
      return operations.Remove(item);
    }
    #endregion

    #region IEnumerable<IOperation> Members
    public IEnumerator<IOperation> GetEnumerator() {
      return operations.GetEnumerator();
    }
    #endregion

    #region IEnumerable Members
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return operations.GetEnumerator();
    }
    #endregion
  }
}
