using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using HeuristicLab.Persistence.Auxiliary;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core.Tokens;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Text;
using System.Reflection;
using System.IO;

namespace HeuristicLab.Persistence.Core {

  /// <summary>
  /// The core hub for serialization. This class transforms an object graph
  /// into a tree and later into a stream of serialization tokens using
  /// the given configuration.
  /// 
  /// <para>Primitive serializers directly format an object to a serializable type.</para>
  /// 
  /// <para>Composite serializers decompose an object into other object that are then
  /// recursively serialized.</para>  
  /// 
  /// A constructed serializer is enumerable and continuously analyses
  /// and traverses the object graph while the enumerator is iterated
  /// </summary>  
  public class Serializer : IEnumerable<ISerializationToken> {

    private class ReferenceEqualityComparer : IEqualityComparer<object> {

      public new bool Equals(object a, object b) {
        return Object.ReferenceEquals(a, b);
      }

      public int GetHashCode(object obj) {
        if (obj == null)
          return 0;
        return obj.GetHashCode();
      }

    }

    private readonly object obj;
    private readonly string rootName;
    private readonly Dictionary<object, int> obj2id;
    private readonly Dictionary<Type, int> typeCache;
    private readonly Configuration configuration;
    private readonly bool isTestRun;
    private readonly List<Exception> exceptions;

    public bool InterleaveTypeInformation { get; set; }

    /// <summary>
    /// Contains a mapping of type id to type and serializer.
    /// </summary>
    public List<TypeMapping> TypeCache {
      get {
        BuildTypeCache();
        return externalTypeCache;
      }
    }
    
    /// <summary>
    /// Contains a list of files (mostly assemblies) that are
    /// necessary to deserialize the object graph again.    
    /// </summary>
    public List<string> RequiredFiles {
      get {
        BuildTypeCache();
        return requiredFiles;
      }
    }

    private List<TypeMapping> externalTypeCache;
    private List<string> requiredFiles;
    private void BuildTypeCache() {
      externalTypeCache = new List<TypeMapping>();
      Dictionary<Assembly, bool> assemblies = new Dictionary<Assembly, bool>();
      foreach (var pair in typeCache) {
        string serializer = null;
        IPrimitiveSerializer f = configuration.GetPrimitiveSerializer(pair.Key);
        if (f != null) {
          serializer = f.GetType().AssemblyQualifiedName;
          assemblies[f.GetType().Assembly] = true;
        } else {
          ICompositeSerializer d = configuration.GetCompositeSerializer(pair.Key);
          serializer = d.GetType().AssemblyQualifiedName;
          assemblies[d.GetType().Assembly] = true;
        }
        externalTypeCache.Add(new TypeMapping(pair.Value, pair.Key.AssemblyQualifiedName, serializer));
        assemblies[pair.Key.Assembly] = true;
      }
      Dictionary<string, bool> files = new Dictionary<string, bool>();
      foreach (Assembly a in assemblies.Keys) {
        files[a.CodeBase] = true;
      }
      requiredFiles = new List<string>(files.Keys);
    }
    
    public Serializer(object obj, Configuration configuration) :
      this(obj, configuration, "ROOT") { }

    public Serializer(object obj, Configuration configuration, string rootName)
      : this(obj, configuration, rootName, false) { }

    /// <param name="isTestRun">Try to complete the whole object graph,
    /// don't stop at the first exception</param>
    public Serializer(object obj, Configuration configuration, string rootName, bool isTestRun) {
      this.InterleaveTypeInformation = false;
      this.obj = obj;
      this.rootName = rootName;
      this.configuration = configuration;
      obj2id = new Dictionary<object, int>(new ReferenceEqualityComparer()) { { new object(), 0 } };
      typeCache = new Dictionary<Type, int>();
      this.isTestRun = isTestRun;
      this.exceptions = new List<Exception>();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    public IEnumerator<ISerializationToken> GetEnumerator() {
      var enumerator = Serialize(new DataMemberAccessor(rootName, null, () => obj, null));
      if (isTestRun) {
        return AddExceptionCompiler(enumerator);
      } else {
        return enumerator;
      }
    }

    private IEnumerator<ISerializationToken> AddExceptionCompiler(IEnumerator<ISerializationToken> enumerator) {
      while (enumerator.MoveNext())
        yield return enumerator.Current;
      if (exceptions.Count == 1)
        throw exceptions[0];
      if (exceptions.Count > 1)
        throw new PersistenceException("Multiple exceptions during serialization", exceptions);
    }

    private IEnumerator<ISerializationToken> Serialize(DataMemberAccessor accessor) {
      object value = accessor.Get();
      if (value == null)
        return NullReferenceEnumerator(accessor.Name);
      Type type = value.GetType();
      if (obj2id.ContainsKey(value))
        return ReferenceEnumerator(accessor.Name, obj2id[value]);
      bool emitTypeInfo = false;
      if (!typeCache.ContainsKey(type)) {
        typeCache.Add(type, typeCache.Count);
        emitTypeInfo = InterleaveTypeInformation;
      }
      int typeId = typeCache[type];
      int? id = null;
      if (!type.IsValueType) {
        id = obj2id.Count;
        obj2id.Add(value, (int)id);
      }
      try {
        IPrimitiveSerializer primitiveSerializer = configuration.GetPrimitiveSerializer(type);
        if (primitiveSerializer != null)
          return PrimitiveEnumerator(
            accessor.Name,
            typeId,
            primitiveSerializer.Format(value),
            id,
            emitTypeInfo);
        ICompositeSerializer compositeSerializer = configuration.GetCompositeSerializer(type);
        if (compositeSerializer != null)
          return CompositeEnumerator(
            accessor.Name,
            compositeSerializer.Decompose(value),
            id,
            typeId,
            compositeSerializer.CreateMetaInfo(value),
            emitTypeInfo);
        throw CreatePersistenceException(type);
      } catch (Exception x) {
        if (isTestRun) {
          exceptions.Add(x);
          return new List<ISerializationToken>().GetEnumerator();
        } else {
          throw x;
        }
      }
    }

    private PersistenceException CreatePersistenceException(Type type) {
      StringBuilder sb = new StringBuilder();
      sb.Append("Could not determine how to serialize a value of type \"")
        .Append(type.VersionInvariantName())
        .AppendLine("\"");
      sb.AppendLine("No registered primitive serializer for this type:");
      foreach (var ps in configuration.PrimitiveSerializers)
        sb.Append(ps.SourceType.VersionInvariantName())
          .Append(" ---- (")
          .Append(ps.GetType().VersionInvariantName())
          .AppendLine(")");          
      sb.AppendLine("Rejected by all composite serializers:");
      foreach (var cs in configuration.CompositeSerializers)
        sb.Append("\"")
          .Append(cs.JustifyRejection(type))
          .Append("\" ---- (")
          .Append(cs.GetType().VersionInvariantName())
          .AppendLine(")");
      return new PersistenceException(sb.ToString());              
    }

    private IEnumerator<ISerializationToken> NullReferenceEnumerator(string name) {
      yield return new NullReferenceToken(name);
    }

    private IEnumerator<ISerializationToken> ReferenceEnumerator(string name, int id) {
      yield return new ReferenceToken(name, id);
    }

    private IEnumerator<ISerializationToken> PrimitiveEnumerator(string name,
        int typeId, ISerialData serializedValue, int? id, bool emitTypeInfo) {
      if (emitTypeInfo) {
        var mapping = TypeCache[typeId];
        yield return new TypeToken(mapping.Id, mapping.TypeName, mapping.Serializer);
      }
      yield return new PrimitiveToken(name, typeId, id, serializedValue);
    }

    private IEnumerator<ISerializationToken> CompositeEnumerator(
        string name, IEnumerable<Tag> tags, int? id, int typeId, IEnumerable<Tag> metaInfo,
        bool emitTypeInfo) {
      if (emitTypeInfo) {
        var mapping = TypeCache[typeId];
        yield return new TypeToken(mapping.Id, mapping.TypeName, mapping.Serializer);
      }
      yield return new BeginToken(name, typeId, id);
      bool first = true;
      if (metaInfo != null) {
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
      }
      if (!first) {
        yield return new MetaInfoEndToken();
      }
      if (tags != null) {
        foreach (var tag in tags) {
          IEnumerator<ISerializationToken> it = Serialize(new DataMemberAccessor(tag.Value, tag.Name));
          while (it.MoveNext())
            yield return it.Current;
        }
      }
      yield return new EndToken(name, typeId, id);
    }

  }


}