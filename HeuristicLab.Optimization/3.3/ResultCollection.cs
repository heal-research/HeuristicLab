#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [StorableClass]
  [Item("ResultCollection", "Represents a collection of results.")]
  public class ResultCollection : NamedItemCollection<IResult> {
    public ResultCollection() : base() { }
    public ResultCollection(int capacity) : base(capacity) { }
    public ResultCollection(IEnumerable<IResult> collection) : base(collection) { }
    [StorableConstructor]
    protected ResultCollection(bool deserializing) : base(deserializing) { }
    protected ResultCollection(ResultCollection original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ResultCollection(this, cloner);
    }

    public static new System.Drawing.Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Object; }
    }

    public virtual void CollectResultValues(IDictionary<string, IItem> values) {
      CollectResultValues(values, string.Empty);
    }

    public virtual void CollectResultValues(IDictionary<string, IItem> values, string rootPath) {
      foreach (IResult result in this) {
        var children = GetCollectedResults(result);
        string path = string.Empty;
        if (!string.IsNullOrWhiteSpace(rootPath))
          path = rootPath + ".";
        foreach (var c in children) {
          if (string.IsNullOrEmpty(c.Key))
            values.Add(path + result.Name, c.Value);
          else values.Add(path + result.Name + "." + c.Key, c.Value);
        }
      }
    }

    protected virtual IEnumerable<KeyValuePair<string, IItem>> GetCollectedResults(IResult result) {
      if (result.Value == null) yield break;
      yield return new KeyValuePair<string, IItem>(string.Empty, result.Value);

      var resultCollection = result.Value as ResultCollection;
      if (resultCollection != null) {
        var children = new Dictionary<string, IItem>();
        resultCollection.CollectResultValues(children);
        foreach (var child in children) yield return child;
      }
    }

  }
}
