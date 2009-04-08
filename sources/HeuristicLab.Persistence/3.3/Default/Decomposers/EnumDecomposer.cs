using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Collections.Generic;

namespace HeuristicLab.Persistence.Default.Decomposers {
    
  public class EnumDecomposer : IDecomposer {

    public int Priority {
      get { return 100; }
    }

    public bool CanDecompose(Type type) {
      return type.IsEnum || type == typeof(Enum);
    }

    public IEnumerable<Tag> Decompose(object obj) {      
      yield return new Tag(Enum.GetName(obj.GetType(), obj));
    }

    public object CreateInstance(Type t) {
      return null;
    }
    
    public object Populate(object instance, IEnumerable<Tag> elements, Type t) {
      IEnumerator<Tag> it = elements.GetEnumerator();
      it.MoveNext();
      return Enum.Parse(t, (string)it.Current.Value);
    }
    
  }
  
}
