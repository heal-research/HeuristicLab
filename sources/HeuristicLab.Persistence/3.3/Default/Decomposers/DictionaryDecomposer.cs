using System;
using System.Collections;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Collections.Generic;

namespace HeuristicLab.Persistence.Default.Decomposers {

  class DictionaryAdder {

    bool keyIsSet, valueIsSet;
    object key;
    object value;
    readonly IDictionary dict;

    public DictionaryAdder(IDictionary dict) {
      this.dict = dict;
      keyIsSet = false;
      valueIsSet = false;
    }

    public void SetKey(object v) {
      key = v;
      keyIsSet = true;
      check();
    }

    public void SetValue(object v) {
      value = v;
      valueIsSet = true;
      check();
    }

    private void check() {
      if ( keyIsSet && valueIsSet )
        dict.Add(key, value);
    }

  }
  
  public class DictionaryDecomposer : IDecomposer {

    public int Priority {
      get { return 100; }
    }


    public bool CanDecompose(Type type) {
      return type.GetInterface(typeof(IDictionary).FullName) != null;        
    }

    public IEnumerable<Tag> DeCompose(object o) {
      IDictionary dict = (IDictionary)o;      
      foreach ( DictionaryEntry entry in dict) {
        yield return new Tag("key", entry.Key);
        yield return new Tag("value", entry.Value);
      }
    }

    public object CreateInstance(Type t) {
      return Activator.CreateInstance(t, true);
    }

    public object Populate(object instance, IEnumerable<Tag> o, Type t) {
      IDictionary dict = (IDictionary)instance;
      IEnumerator<Tag> iter = o.GetEnumerator();
      while (iter.MoveNext()) {
        Tag key = iter.Current;
        iter.MoveNext();
        Tag value = iter.Current;
        DictionaryAdder da = new DictionaryAdder(dict);
        key.SafeSet(da.SetKey);
        value.SafeSet(da.SetValue);        
      }
      return dict;
    }
  }

}
