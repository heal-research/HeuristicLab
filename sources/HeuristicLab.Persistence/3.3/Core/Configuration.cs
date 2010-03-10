using System;
using System.Collections.Generic;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Core {

  [StorableClass(StorableClassType.MarkedOnly)]    
  public class Configuration {

    [Storable]
    private readonly Dictionary<Type, IPrimitiveSerializer> primitiveSerializers;

    [Storable]
    private readonly List<ICompositeSerializer> compositeSerializers;
    private readonly Dictionary<Type, ICompositeSerializer> compositeSerializerCache;

    [Storable]
    public IFormat Format { get; private set; }
    
    private Configuration() {
      compositeSerializerCache = new Dictionary<Type, ICompositeSerializer>();
    }

    public Configuration(IFormat format,
        IEnumerable<IPrimitiveSerializer> primitiveSerializers,
        IEnumerable<ICompositeSerializer> compositeSerializers) {
      this.Format = format;
      this.primitiveSerializers = new Dictionary<Type, IPrimitiveSerializer>();
      foreach (IPrimitiveSerializer primitiveSerializer in primitiveSerializers) {
        if (primitiveSerializer.SerialDataType != format.SerialDataType) {
          throw new ArgumentException("All primitive serializers must have the same IFormat.");
        }
        this.primitiveSerializers.Add(primitiveSerializer.SourceType, primitiveSerializer);
      }
      this.compositeSerializers = new List<ICompositeSerializer>(compositeSerializers);
      compositeSerializerCache = new Dictionary<Type, ICompositeSerializer>();
    }

    public IEnumerable<IPrimitiveSerializer> PrimitiveSerializers {
      get { return primitiveSerializers.Values; }
    }

    public IEnumerable<ICompositeSerializer> CompositeSerializers {
      get { return compositeSerializers; }
    }

    public IPrimitiveSerializer GetPrimitiveSerializer(Type type) {
      IPrimitiveSerializer primitiveSerializer;
      primitiveSerializers.TryGetValue(type, out primitiveSerializer);
      return primitiveSerializer;
    }

    public ICompositeSerializer GetCompositeSerializer(Type type) {
      if (compositeSerializerCache.ContainsKey(type))
        return compositeSerializerCache[type];
      foreach (ICompositeSerializer d in compositeSerializers) {
        if (d.CanSerialize(type)) {
          compositeSerializerCache.Add(type, d);
          return d;
        }
      }
      compositeSerializerCache.Add(type, null);
      return null;
    }
  }

}