using System;
using System.Collections;
using System.Reflection;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Collections.Generic;

namespace HeuristicLab.Persistence.Default.Decomposers {

  public class EnumerableCache {
    readonly List<object> values;
    int nSet;
    int count;
    readonly object enumerable;
    readonly MethodInfo addMethod;

    public EnumerableCache(object enumerable, MethodInfo addMethod) {      
      values = new List<object>();
      this.enumerable = enumerable;
      this.addMethod = addMethod;
      count = -1;
    }

    public Setter GetNextSetter() {      
      int index = values.Count;      
      values.Add(new object());
      return v => Set(index, v);
    }

    private void Set(int index, object value) {      
      values[index] = value;
      nSet += 1;
      if (count >= 0 && nSet >= count)
        Fill();
    }

    public void Terminate() {
      count = values.Count;      
      if (nSet >= count)
        Fill();
    }

    private void Fill() {      
      foreach ( object v in values ) {
        addMethod.Invoke(enumerable, new[] {v});
      }
    }

  }

  public class EnumerableDecomposer : IDecomposer {

    public int Priority {
      get { return 100; }
    }


    public bool CanDecompose(Type type) {
      return
        type.GetInterface(typeof(IEnumerable).FullName) != null &&
        type.GetMethod("Add") != null &&
        type.GetMethod("Add").GetParameters().Length == 1 &&
        type.GetConstructor(
          BindingFlags.Public |
          BindingFlags.NonPublic |
          BindingFlags.Instance,
          null, Type.EmptyTypes, null) != null;
    }

    public IEnumerable<Tag> Decompose(object obj) {
      foreach (object o in (IEnumerable)obj) {
        yield return new Tag(null, o);
      }
    }

    public object CreateInstance(Type type) {
      return Activator.CreateInstance(type, true);
    }

    public object Populate(object instance, IEnumerable<Tag> tags, Type type) {
      MethodInfo addMethod = type.GetMethod("Add");
      EnumerableCache cache = new EnumerableCache(instance, addMethod);
      foreach (var tag in tags) {
        tag.SafeSet(cache.GetNextSetter());
      }
      cache.Terminate();
      return instance;
    }

  }  

}
