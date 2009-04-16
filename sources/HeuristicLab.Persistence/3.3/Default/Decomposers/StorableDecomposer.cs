using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;

namespace HeuristicLab.Persistence.Default.Decomposers {

  [EmptyStorableClass]
  public class StorableDecomposer : IDecomposer {

    public int Priority {
      get { return 200; }
    }

    public bool CanDecompose(Type type) {
      return StorableAttribute.GetStorableMembers(type, false).Count() > 0 ||
        EmptyStorableClassAttribute.IsEmptyStorable(type);

    }

    public IEnumerable<Tag> CreateMetaInfo(object o) {
      return new Tag[] { };
    }

    public IEnumerable<Tag> Decompose(object obj) {
      foreach (var mapping in StorableAttribute.GetStorableAccessors(obj)) {
        yield return new Tag(mapping.Key, mapping.Value.Get());
      }
    }

    public object CreateInstance(Type type, IEnumerable<Tag> metaInfo) {
      return Activator.CreateInstance(type, true);
    }

    public void Populate(object instance, IEnumerable<Tag> objects, Type type) {
      var memberDict = new Dictionary<string, Tag>();
      IEnumerator<Tag> iter = objects.GetEnumerator();
      while (iter.MoveNext()) {
        memberDict.Add(iter.Current.Name, iter.Current);
      }
      foreach (var mapping in StorableAttribute.GetStorableAccessors(instance)) {
        if (memberDict.ContainsKey(mapping.Key)) {
          mapping.Value.Set(memberDict[mapping.Key].Value);
        } else if (mapping.Value.DefaultValue != null) {
          mapping.Value.Set(mapping.Value.DefaultValue);
        }
      }
    }
  }
}
