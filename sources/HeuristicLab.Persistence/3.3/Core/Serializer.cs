using System.Collections.Generic;
using System.Collections;
using System;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core {

  public class Serializer : IEnumerable<ISerializationToken> {

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
            if (d != null)
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
      obj2id = new Dictionary<object, int> {{new object(), 0}};
      typeCache = new Dictionary<Type, int>();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    public IEnumerator<ISerializationToken> GetEnumerator() {
      DataMemberAccessor rootAccessor = new DataMemberAccessor(
        rootName, null, () => obj, null);
      IEnumerator<ISerializationToken> iterator = Serialize(rootAccessor);
      while (iterator.MoveNext())
        yield return iterator.Current;      
    }
    
    private IEnumerator<ISerializationToken> Serialize(DataMemberAccessor accessor) {
      object value = accessor.Get();
      if (value == null)
        return NullReferenceEnumerator(accessor.Name);
      if (obj2id.ContainsKey(value))
        return ReferenceEnumerator(accessor.Name, obj2id[value]);              
      if ( ! typeCache.ContainsKey(value.GetType()))
        typeCache.Add(value.GetType(), typeCache.Count);
      int typeId = typeCache[value.GetType()];
      int? id = null;
      if ( ! value.GetType().IsValueType) {
        id = obj2id.Count;
        obj2id.Add(value, (int)id);
      }
      IFormatter formatter = configuration.GetFormatter(value.GetType());
      if (formatter != null)
        return PrimitiveEnumerator(accessor.Name, typeId, formatter.DoFormat(value), id);
      IDecomposer decomposer = configuration.GetDecomposer(value.GetType()); 
      if (decomposer != null)
        return CompositeEnumerator(accessor.Name, decomposer.DeCompose(value), id, typeId);                  
      throw new ApplicationException(
          String.Format(
          "No suitable method for serializing values of type \"{0}\" found.",
          value.GetType().VersionInvariantName()));      
    }

    private IEnumerator<ISerializationToken> NullReferenceEnumerator(string name) {
      yield return new NullReferenceToken(name);
    }

    private IEnumerator<ISerializationToken> ReferenceEnumerator(string name, int id) {
      yield return new ReferenceToken(name, id);
    }

    private IEnumerator<ISerializationToken> PrimitiveEnumerator(string name,
        int typeId, object serializedValue, int? id) {
      yield return new PrimitiveToken(name, typeId, serializedValue, id);
    }

    private IEnumerator<ISerializationToken> CompositeEnumerator(string name,
        IEnumerable<Tag> tags, int? id, int typeId) {
      yield return new BeginToken(name, typeId, id);      
        foreach (var tag in tags) {
          IEnumerator<ISerializationToken> iterator = Serialize(            
            new DataMemberAccessor(tag.Value, tag.Name));
          while (iterator.MoveNext())
            yield return iterator.Current;
        }
      yield return new EndToken(name, typeId, id);        
    }

  }


}