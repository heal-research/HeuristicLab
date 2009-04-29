using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using HeuristicLab.Persistence.Auxiliary;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core.Tokens;
using HeuristicLab.Persistence.Default.Decomposers.Storable;
using System.Text;

namespace HeuristicLab.Persistence.Core {

  public class Serializer : IEnumerable<ISerializationToken> {

    class ReferenceEqualityComparer : IEqualityComparer<object> {

      public bool Equals(object a, object b) {
        return Object.ReferenceEquals(a, b);
      }

      public int GetHashCode(object obj) {
        return obj.GetHashCode();
      }

    }

    private readonly object obj;
    private readonly string rootName;
    private readonly Dictionary<object, int> obj2id;
    private readonly Dictionary<Type, int> typeCache;
    private readonly Configuration configuration;

    public List<TypeMapping> TypeCache {
      get {
        List<TypeMapping> result = new List<TypeMapping>();
        foreach (var pair in typeCache) {
          string serializer = null;
          IFormatter f = configuration.GetFormatter(pair.Key);
          if (f != null) {
            serializer = f.GetType().VersionInvariantName();
          } else {
            IDecomposer d = configuration.GetDecomposer(pair.Key);
            serializer = d.GetType().VersionInvariantName();
          }
          result.Add(new TypeMapping(pair.Value, pair.Key.VersionInvariantName(), serializer));
        }
        return result;
      }
    }

    public Serializer(object obj, Configuration configuration) :
      this(obj, configuration, "ROOT") { }

    public Serializer(object obj, Configuration configuration, string rootName) {
      this.obj = obj;
      this.rootName = rootName;
      this.configuration = configuration;
      obj2id = new Dictionary<object, int>(new ReferenceEqualityComparer()) { { new object(), 0 } };
      typeCache = new Dictionary<Type, int>();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    public IEnumerator<ISerializationToken> GetEnumerator() {
      return Serialize(new DataMemberAccessor(rootName, null, () => obj, null));
    }

    private IEnumerator<ISerializationToken> Serialize(DataMemberAccessor accessor) {
      object value = accessor.Get();
      if (value == null)
        return NullReferenceEnumerator(accessor.Name);
      Type type = value.GetType();
      if (obj2id.ContainsKey(value))
        return ReferenceEnumerator(accessor.Name, obj2id[value]);
      if (!typeCache.ContainsKey(type))
        typeCache.Add(type, typeCache.Count);
      int typeId = typeCache[type];
      int? id = null;
      if (!type.IsValueType) {
        id = obj2id.Count;
        obj2id.Add(value, (int)id);
      }
      IFormatter formatter = configuration.GetFormatter(type);
      if (formatter != null)
        return PrimitiveEnumerator(accessor.Name, typeId, formatter.Format(value), id);
      IDecomposer decomposer = configuration.GetDecomposer(type);
      if (decomposer != null)
        return CompositeEnumerator(accessor.Name, decomposer.Decompose(value), id, typeId, decomposer.CreateMetaInfo(value));
      throw new PersistenceException(
          String.Format(
          "No suitable method for serializing values of type \"{0}\" found\r\n" +
          "Formatters:\r\n{1}\r\n" +
          "Decomposers:\r\n{2}",
          value.GetType().VersionInvariantName(),
          string.Join("\r\n", configuration.Formatters.Select(f => f.GetType().VersionInvariantName()).ToArray()),
          string.Join("\r\n", configuration.Decomposers.Select(d => d.GetType().VersionInvariantName()).ToArray())
          ));

    }

    private IEnumerator<ISerializationToken> NullReferenceEnumerator(string name) {
      yield return new NullReferenceToken(name);
    }

    private IEnumerator<ISerializationToken> ReferenceEnumerator(string name, int id) {
      yield return new ReferenceToken(name, id);
    }

    private IEnumerator<ISerializationToken> PrimitiveEnumerator(string name,
        int typeId, ISerialData serializedValue, int? id) {
      yield return new PrimitiveToken(name, typeId, id, serializedValue);
    }

    private IEnumerator<ISerializationToken> CompositeEnumerator(
        string name, IEnumerable<Tag> tags, int? id, int typeId, IEnumerable<Tag> metaInfo) {
      yield return new BeginToken(name, typeId, id);
      bool first = true;
      foreach (var tag in metaInfo) {
        IEnumerator<ISerializationToken> metaIt = Serialize(new DataMemberAccessor(tag.Value, tag.Name));
        while (metaIt.MoveNext()) {
          if (first) {
            yield return new MetaInfoBeginToken();
            first = false;
          }
          yield return metaIt.Current;
        }
      }
      if (!first) {
        yield return new MetaInfoEndToken();
      }
      foreach (var tag in tags) {
        IEnumerator<ISerializationToken> it = Serialize(new DataMemberAccessor(tag.Value, tag.Name));
        while (it.MoveNext())
          yield return it.Current;
      }
      yield return new EndToken(name, typeId, id);
    }

  }


}