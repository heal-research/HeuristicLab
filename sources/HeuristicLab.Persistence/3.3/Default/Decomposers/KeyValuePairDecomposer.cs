using System;
using System.Linq;
using System.Collections.Generic;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Reflection;

namespace HeuristicLab.Persistence.Default.Decomposers {
  
  [EmptyStorableClass]
  public class KeyValuePairDecomposer : IDecomposer {

    public int Priority {
      get { return 100; }
    }


    public bool CanDecompose(Type type) {
      return type.IsGenericType &&
             type.GetGenericTypeDefinition() == 
             typeof (KeyValuePair<int, int>).GetGenericTypeDefinition();
    }

    public IEnumerable<Tag> CreateMetaInfo(object o) {
      return new Tag[] { };
    }

    public IEnumerable<Tag> Decompose(object o) {
      Type t = o.GetType();
      yield return new Tag("key", t.GetProperty("Key").GetValue(o, null));
      yield return new Tag("value", t.GetProperty("Value").GetValue(o, null));
    }

    public object CreateInstance(Type type, IEnumerable<Tag> metaInfo) {
      return Activator.CreateInstance(type, true);
    }

    public void Populate(object instance, IEnumerable<Tag> o, Type t) {
      IEnumerator<Tag> iter = o.GetEnumerator();
      iter.MoveNext();      
      t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        .Single(fi => fi.Name == "key").SetValue(instance, iter.Current.Value);
      iter.MoveNext();
      t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        .Single(fi => fi.Name == "value").SetValue(instance, iter.Current.Value);      
    }
  }
}
