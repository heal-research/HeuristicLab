using System;
using System.Collections;
using System.Reflection;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Collections.Generic;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Persistence.Auxiliary;

namespace HeuristicLab.Persistence.Default.CompositeSerializers {

  [StorableClass(StorableClassType.Empty)]
  public class EnumerableSerializer : ICompositeSerializer {

    public int Priority {
      get { return 100; }
    }


    public bool CanSerialize(Type type) {
      return
        ReflectionTools.HasDefaultConstructor(type) &&
        type.GetInterface(typeof(IEnumerable).FullName) != null &&
        type.GetMethod("Add") != null &&
        type.GetMethod("Add").GetParameters().Length == 1;
    }

    public string JustifyRejection(Type type) {
      if (!ReflectionTools.HasDefaultConstructor(type))
        return "no default constructor";
      if (type.GetInterface(typeof(IEnumerable).FullName) == null)
        return "interface IEnumerable not implemented";
      if (type.GetMethod("Add") == null)
        return "no 'Add()' method";      
      return "no 'Add()' method with one argument";
    }

    public IEnumerable<Tag> CreateMetaInfo(object o) {
      return new Tag[] { };
    }

    public IEnumerable<Tag> Decompose(object obj) {
      foreach (object o in (IEnumerable)obj) {
        yield return new Tag(o);
      }
    }

    public object CreateInstance(Type type, IEnumerable<Tag> metaInfo) {
      return Activator.CreateInstance(type, true);
    }

    public void Populate(object instance, IEnumerable<Tag> tags, Type type) {
      MethodInfo addMethod = type.GetMethod("Add");
      try {
        foreach (var tag in tags)
          addMethod.Invoke(instance, new[] { tag.Value });
      } catch (Exception e) {
        throw new PersistenceException("Exception caught while trying to populate enumerable.", e);
      }
    }
  }
}