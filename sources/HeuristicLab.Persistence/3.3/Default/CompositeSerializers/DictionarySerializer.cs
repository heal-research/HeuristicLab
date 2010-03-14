using System;
using System.Collections;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Collections.Generic;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Persistence.Auxiliary;

namespace HeuristicLab.Persistence.Default.CompositeSerializers {

  [StorableClass]
  public class DictionarySerializer : ICompositeSerializer {

    public int Priority {
      get { return 100; }
    }


    public bool CanSerialize(Type type) {
      return ReflectionTools.HasDefaultConstructor(type) &&
        type.GetInterface(typeof(IDictionary).FullName) != null;
    }

    public string JustifyRejection(Type type) {
      if (!ReflectionTools.HasDefaultConstructor(type))
        return "no default constructor";
      return "interface IDictionary not implemented";
    }

    public IEnumerable<Tag> CreateMetaInfo(object o) {
      return new Tag[] { };
    }

    public IEnumerable<Tag> Decompose(object o) {
      IDictionary dict = (IDictionary)o;
      foreach (DictionaryEntry entry in dict) {
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
      try {
        while (iter.MoveNext()) {
          Tag key = iter.Current;
          iter.MoveNext();
          Tag value = iter.Current;
          dict.Add(key.Value, value.Value);
        }
      } catch (InvalidOperationException e) {
        throw new PersistenceException("Dictionaries must contain an even number of elements (key+value).", e);
      } catch (NotSupportedException e) {
        throw new PersistenceException("The serialized dictionary type was read-only or had a fixed size and cannot be deserialized.", e);
      } catch (ArgumentNullException e) {
        throw new PersistenceException("Dictionary key was null.", e);
      } catch (ArgumentException e) {
        throw new PersistenceException("Duplicate dictionary key.", e);
      }
    }
  }

}
