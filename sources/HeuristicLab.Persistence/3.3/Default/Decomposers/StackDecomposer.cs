using System;
using System.Collections;
using System.Reflection;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Collections.Generic;
using HeuristicLab.Persistence.Default.Decomposers.Storable;
using System.IO;

namespace HeuristicLab.Persistence.Default.Decomposers {

  [EmptyStorableClass]
  public class StackDecomposer : IDecomposer {

    public int Priority {
      get { return 100; }
    }


    public bool CanDecompose(Type type) {      
      return type == typeof(Stack) ||
        type.IsGenericType &&
        type.GetGenericTypeDefinition() == typeof(Stack<>);
    }

    public IEnumerable<Tag> CreateMetaInfo(object o) {
      return new Tag[] { };
    }

    public IEnumerable<Tag> Decompose(object obj) {
      MethodInfo addMethod = obj.GetType().GetMethod("Push");
      object reverseStack = Activator.CreateInstance(obj.GetType(), true);
      foreach (object o in (IEnumerable)obj) {
        addMethod.Invoke(reverseStack, new[] { o });
      }
      foreach (object o in (IEnumerable)reverseStack) {
        yield return new Tag(null, o);
      }
    }

    public object CreateInstance(Type type, IEnumerable<Tag> metaInfo) {
      return Activator.CreateInstance(type, true);
    }

    public void Populate(object instance, IEnumerable<Tag> tags, Type type) {            
      MethodInfo addMethod = type.GetMethod("Push");
      try {
        foreach (var tag in tags)
          addMethod.Invoke(instance, new[] { tag.Value });        
      } catch (Exception e) {
        throw new PersistenceException("Exception caught while trying to populate enumerable.", e);
      }
    }
  }
}
