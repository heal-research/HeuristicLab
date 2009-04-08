using System;
using System.Linq;
using System.Collections.Generic;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Reflection;

namespace HeuristicLab.Persistence.Default.Decomposers {
  
  public class KeyValuePairDecomposer : IDecomposer {

    public int Priority {
      get { return 100; }
    }


    public bool CanDecompose(Type type) {
      return type.IsGenericType &&
             type.GetGenericTypeDefinition() == 
             typeof (KeyValuePair<int, int>).GetGenericTypeDefinition();
    }

    public IEnumerable<Tag> DeCompose(object o) {      
      Type t = o.GetType();
      yield return new Tag("key", t.GetProperty("Key").GetValue(o, null));
      yield return new Tag("value", t.GetProperty("Value").GetValue(o, null));
    }

    public object CreateInstance(Type type) {
      return Activator.CreateInstance(type, true);
    }

    public object Populate(object instance, IEnumerable<Tag> o, Type t) {
      IEnumerator<Tag> iter = o.GetEnumerator();
      iter.MoveNext();
      FieldInfo keyFieldInfo =
        t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        .Single(fi => fi.Name == "key");
      FieldInfo valueFieldInfo = 
        t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        .Single(fi => fi.Name == "value");
      iter.Current.SafeSet(value => keyFieldInfo.SetValue(instance, value));      
      iter.MoveNext();
      iter.Current.SafeSet(value => valueFieldInfo.SetValue(instance, value));
      return instance;
    }    
  }

}
