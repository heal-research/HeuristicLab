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
using System.Xml;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  public class ExecutionContextCollection : DeepCloneable, IList<ExecutionContext> {
    [Storable]
    private IList<ExecutionContext> contexts;

    [Storable]
    private bool parallel;
    public bool Parallel {
      get { return parallel; }
      set { parallel = value; }
    }

    public ExecutionContextCollection() {
      contexts = new List<ExecutionContext>();
      parallel = false;
    }
    public ExecutionContextCollection(IEnumerable<ExecutionContext> collection) {
      contexts = new List<ExecutionContext>(collection);
      parallel = false;
    }
    public ExecutionContextCollection(params ExecutionContext[] list) {
      contexts = new List<ExecutionContext>(list);
      parallel = false;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      ExecutionContextCollection clone = new ExecutionContextCollection();
      cloner.RegisterClonedObject(this, clone);
      clone.parallel = parallel;
      for (int i = 0; i < contexts.Count; i++)
        clone.contexts.Add((ExecutionContext)cloner.Clone(contexts[i]));
      return clone;
    }

    #region IList<ExecutionContext> Members
    public int IndexOf(ExecutionContext item) {
      return contexts.IndexOf(item);
    }
    public void Insert(int index, ExecutionContext item) {
      contexts.Insert(index, item);
    }
    public void RemoveAt(int index) {
      contexts.RemoveAt(index);
    }
    public ExecutionContext this[int index] {
      get { return contexts[index]; }
      set { contexts[index] = value; }
    }
    #endregion

    #region ICollection<ExecutionContext> Members
    public void Add(ExecutionContext item) {
      contexts.Add(item);
    }
    public void Clear() {
      contexts.Clear();
    }
    public bool Contains(ExecutionContext item) {
      return contexts.Contains(item);
    }
    public void CopyTo(ExecutionContext[] array, int arrayIndex) {
      contexts.CopyTo(array, arrayIndex);
    }
    public int Count {
      get { return contexts.Count; }
    }
    public bool IsReadOnly {
      get { return contexts.IsReadOnly; }
    }
    public bool Remove(ExecutionContext item) {
      return contexts.Remove(item);
    }
    #endregion

    #region IEnumerable<ExecutionContext> Members
    public IEnumerator<ExecutionContext> GetEnumerator() {
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
