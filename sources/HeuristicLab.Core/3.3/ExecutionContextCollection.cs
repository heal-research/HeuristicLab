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
using System.Linq;
using System.Text;
using System.Xml;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  public class ExecutionContextCollection : DeepCloneable, IList<IExecutionContext>, IExecutionContext {
    [Storable]
    private IList<IExecutionContext> contexts;

    [Storable]
    private bool parallel;
    public bool Parallel {
      get { return parallel; }
      set { parallel = value; }
    }

    public ExecutionContextCollection() {
      contexts = new List<IExecutionContext>();
      parallel = false;
    }
    public ExecutionContextCollection(IEnumerable<IExecutionContext> collection) {
      contexts = new List<IExecutionContext>(collection.Where(e => e != null));
      parallel = false;
    }
    public ExecutionContextCollection(params IExecutionContext[] list) {
      contexts = new List<IExecutionContext>(list.Where(e => e != null));
      parallel = false;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      ExecutionContextCollection clone = new ExecutionContextCollection();
      cloner.RegisterClonedObject(this, clone);
      clone.parallel = parallel;
      for (int i = 0; i < contexts.Count; i++)
        clone.contexts.Add((IExecutionContext)cloner.Clone(contexts[i]));
      return clone;
    }

    #region IList<IExecutionContext> Members
    public int IndexOf(IExecutionContext item) {
      return contexts.IndexOf(item);
    }
    public void Insert(int index, IExecutionContext item) {
      if (item != null) contexts.Insert(index, item);
    }
    public void RemoveAt(int index) {
      contexts.RemoveAt(index);
    }
    public IExecutionContext this[int index] {
      get { return contexts[index]; }
      set { if (value != null) contexts[index] = value; }
    }
    #endregion

    #region ICollection<IExecutionContext> Members
    public void Add(IExecutionContext item) {
      if (item != null) contexts.Add(item);
    }
    public void Clear() {
      contexts.Clear();
    }
    public bool Contains(IExecutionContext item) {
      return contexts.Contains(item);
    }
    public void CopyTo(IExecutionContext[] array, int arrayIndex) {
      contexts.CopyTo(array, arrayIndex);
    }
    public int Count {
      get { return contexts.Count; }
    }
    public bool IsReadOnly {
      get { return contexts.IsReadOnly; }
    }
    public bool Remove(IExecutionContext item) {
      return contexts.Remove(item);
    }
    #endregion

    #region IEnumerable<IExecutionContext> Members
    public IEnumerator<IExecutionContext> GetEnumerator() {
      return contexts.GetEnumerator();
    }
    #endregion

    #region IEnumerable Members
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return contexts.GetEnumerator();
    }
    #endregion
  }
}
