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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {

  [Item("RunCollection Group Creater", "Regroups existing runs according to equal values in GroupBy and prefixes them according to their value in Prefix.")]
  [StorableClass]
  public class RunCollectionGroupCreater : ParameterizedNamedItem, IRunCollectionModifier {
    
    public ValueParameter<ItemCollection<StringValue>> GroupByParameter {
      get { return (ValueParameter<ItemCollection<StringValue>>)Parameters["GroupBy"]; }
    }

    public ValueParameter<StringValue> PrefixParameter {
      get { return (ValueParameter<StringValue>)Parameters["Prefix"]; }
    }

    private IEnumerable<string> GroupBy { get { return GroupByParameter.Value.Select(v => v.Value); } }
    private string Prefix { get { return PrefixParameter.Value.Value; } }

    #region Construction & Cloning
    [StorableConstructor]
    protected RunCollectionGroupCreater(bool deserializing) : base(deserializing) { }
    protected RunCollectionGroupCreater(RunCollectionGroupCreater original, Cloner cloner) : base(original, cloner) { }
    public RunCollectionGroupCreater() {
      Parameters.Add(new ValueParameter<ItemCollection<StringValue>>("GroupBy", "The variable that has to be the same for all members of a group.",
        new ItemCollection<StringValue>(new[] { new StringValue("Problem Name") })));
      Parameters.Add(new ValueParameter<StringValue>("Prefix", "The distinguishing prefix values for the individual runs.",
        new StringValue("Algorithm Name")));      
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RunCollectionGroupCreater(this, cloner);
    }
    #endregion    

    private static string GetStringValue(string name, IRun r) {
      IItem item;
      r.Results.TryGetValue(name, out item);
      if (item != null)
        return item.ToString();
      r.Parameters.TryGetValue(name, out item);
      return item != null ? item.ToString() : "<none>";
    }

    private static string GetValues(IEnumerable<string> names, IRun r) {
      return string.Join("/", names.Select(n => GetStringValue(n, r)));
    }

    public void Modify(List<IRun> runs) {
      var groups = runs.GroupBy(r => GetValues(GroupBy, r).ToString()).ToList();
      runs.Clear();
      foreach (var group in groups) {
        var run = new Run { Name = string.Format(group.Key) };
        foreach (var r in group) {
          var prefix = GetStringValue(Prefix, r);
          foreach (var result in r.Results) {
            InsertNew(run.Results, prefix, result.Key, result.Value);
          }
          foreach (var parameter in r.Parameters) {
            InsertNew(run.Parameters, prefix, parameter.Key, parameter.Value);
          }
        }
        runs.Add(run);
      }
    }

    private static void InsertNew(IDictionary<string, IItem> dict, string prefix, string key, IItem value) {
      if (prefix == null)
        prefix = "<null>";
      var n = 0;
      var name = string.Format("{0}.{1}", prefix, key);
      while (dict.ContainsKey(name)) {
        name = string.Format("{0}_{1}.{2}", prefix, ++n, key);
      }
      dict.Add(name, value);      
    }
  }
}
