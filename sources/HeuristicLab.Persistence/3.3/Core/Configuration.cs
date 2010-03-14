using System;
using System.Collections.Generic;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Core {

  /// <summary>
  /// Defines the set of primitive and composite serializers that are to be used
  /// for a certain seraial format. The configuration can be obtained from the
  /// <c>ConfigurationService</c>.
  /// </summary>
  [StorableClass]    
  public class Configuration {

    [Storable]
    private readonly Dictionary<Type, IPrimitiveSerializer> primitiveSerializers;

    [Storable]
    private readonly List<ICompositeSerializer> compositeSerializers;
    private readonly Dictionary<Type, ICompositeSerializer> compositeSerializerCache;

    /// <summary>
    /// Gets the format.
    /// </summary>
    /// <value>The format.</value>
    [Storable]
    public IFormat Format { get; private set; }
    
    private Configuration() {
      compositeSerializerCache = new Dictionary<Type, ICompositeSerializer>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Configuration"/> class.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="primitiveSerializers">The primitive serializers.</param>
    /// <param name="compositeSerializers">The composite serializers.</param>
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

    /// <summary>
    /// Gets the primitive serializers.
    /// </summary>
    /// <value>The primitive serializers.</value>
    public IEnumerable<IPrimitiveSerializer> PrimitiveSerializers {
      get { return primitiveSerializers.Values; }
    }

    /// <summary>
    /// Gets the composite serializers.
    /// </summary>
    /// <value>An enumerable of composite serializers.</value>
    public IEnumerable<ICompositeSerializer> CompositeSerializers {
      get { return compositeSerializers; }
    }

    /// <summary>
    /// Gets the primitive serializer.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The appropriate primitive serializer for the type.</returns>
    public IPrimitiveSerializer GetPrimitiveSerializer(Type type) {
      IPrimitiveSerializer primitiveSerializer;
      primitiveSerializers.TryGetValue(type, out primitiveSerializer);
      return primitiveSerializer;
    }

    /// <summary>
    /// Gets the composite serializer for a given type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The first matching composite serializer for the type.</returns>
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