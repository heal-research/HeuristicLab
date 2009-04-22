using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Collections.Generic;
using HeuristicLab.Persistence.Default.Decomposers.Storable;

namespace HeuristicLab.Persistence.Default.Decomposers {

  [EmptyStorableClass]
  public class EnumDecomposer : IDecomposer {

    public int Priority {
      get { return 100; }
    }

    public bool CanDecompose(Type type) {
      return type.IsEnum || type == typeof(Enum);
    }

    public IEnumerable<Tag> CreateMetaInfo(object obj) {
      yield return new Tag(Enum.GetName(obj.GetType(), obj));
    }

    public IEnumerable<Tag> Decompose(object obj) {
      return new Tag[] { };
    }

    public object CreateInstance(Type t, IEnumerable<Tag> metaInfo) {
      IEnumerator<Tag> it = metaInfo.GetEnumerator();
      it.MoveNext();
      return Enum.Parse(t, (string)it.Current.Value);
    }

    public void Populate(object instance, IEnumerable<Tag> elements, Type t) {
    }
  }
}
