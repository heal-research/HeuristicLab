using System;
using System.Collections;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Collections.Generic;

namespace HeuristicLab.Persistence.Default.Decomposers {  
  
  [EmptyStorableClass]
  public class DictionaryDecomposer : IDecomposer {

    public int Priority {
      get { return 100; }
    }


    public bool CanDecompose(Type type) {
      return type.GetInterface(typeof(IDictionary).FullName) != null;        
    }

    public IEnumerable<Tag> CreateMetaInfo(object o) {
      return new Tag[] { };
    }

    public IEnumerable<Tag> Decompose(object o) {
      IDictionary dict = (IDictionary)o;      
      foreach ( DictionaryEntry entry in dict) {
        yield return new Tag("key", entry.Key);
        yield return new Tag("value", entry.Value);
      }
    }

    public object CreateInstance(Type t, IEnumerable<Tag> metaInfo) {
      return Activator.CreateInstance(t, true);
    }

    public void Populate(object instance, IEnumerable<Tag> o, Type t) {
      IDictionary dict = (IDictionary)instance;
      IEnumerator<Tag> iter = o.GetEnumerator();
      while (iter.MoveNext()) {
        Tag key = iter.Current;
        iter.MoveNext();
        Tag value = iter.Current;
        dict.Add(key.Value, value.Value);
      }      
    }
  }

}
