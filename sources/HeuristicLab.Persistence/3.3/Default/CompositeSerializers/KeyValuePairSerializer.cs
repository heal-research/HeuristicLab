using System;
using System.Linq;
using System.Collections.Generic;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Reflection;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Default.CompositeSerializers {

  [StorableClass(StorableClassType.Empty)]
  public class KeyValuePairSerializer : ICompositeSerializer {

    public int Priority {
      get { return 100; }
    }

    private static readonly Type genericKeyValuePairType =
      typeof(KeyValuePair<int, int>).GetGenericTypeDefinition();

    public bool CanSerialize(Type type) {
      return type.IsGenericType &&
             type.GetGenericTypeDefinition() == genericKeyValuePairType;             
    }

    public string JustifyRejection(Type type) {
      if (!type.IsGenericType)
        return "not even generic";      
      return "not generic KeyValuePair<,>";
    }

    public IEnumerable<Tag> CreateMetaInfo(object o) {
      return new Tag[] { };
    }

    public IEnumerable<Tag> Decompose(object o) {
      Type t = o.GetType();
      Tag key, value;
      try {
        key = new Tag("key", t.GetProperty("Key").GetValue(o, null));
      } catch (Exception e) {
        throw new PersistenceException("Exception caught during KeyValuePair decomposition", e);
      }
      yield return key;
      try {
        value = new Tag("value", t.GetProperty("Value").GetValue(o, null));
      } catch (Exception e) {
        throw new PersistenceException("Exception caught during KeyValuePair decomposition", e);
      }
      yield return value;
    }

    public object CreateInstance(Type type, IEnumerable<Tag> metaInfo) {
      return Activator.CreateInstance(type, true);
    }

    public void Populate(object instance, IEnumerable<Tag> o, Type t) {
      IEnumerator<Tag> iter = o.GetEnumerator();
      try {
        iter.MoveNext();
        t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
          .Single(fi => fi.Name == "key").SetValue(instance, iter.Current.Value);
        iter.MoveNext();
        t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
          .Single(fi => fi.Name == "value").SetValue(instance, iter.Current.Value);
      } catch (InvalidOperationException e) {
        throw new PersistenceException("Not enough components to populate KeyValuePair instance", e);
      } catch (Exception e) {
        throw new PersistenceException("Exception caught during KeyValuePair reconstruction", e);
      }
    }
  }
}
