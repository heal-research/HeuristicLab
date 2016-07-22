#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Persistence.Interfaces;

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

    [StorableConstructor]
    protected Configuration(bool isDeserializing) {
      compositeSerializerCache = new Dictionary<Type, ICompositeSerializer>();
      if (isDeserializing)
        return;
      primitiveSerializers = new Dictionary<Type, IPrimitiveSerializer>();
      compositeSerializers = new List<ICompositeSerializer>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Configuration"/> class.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="primitiveSerializers">The primitive serializers.</param>
    /// <param name="compositeSerializers">The composite serializers.</param>
    public Configuration(IFormat format,
        IEnumerable<IPrimitiveSerializer> primitiveSerializers,
        IEnumerable<ICompositeSerializer> compositeSerializers)
      : this(false) {
      this.Format = format;
      this.compositeSerializers.AddRange(compositeSerializers);
      foreach (var primitiveSerializer in primitiveSerializers) {
        if (primitiveSerializer.SerialDataType != format.SerialDataType)
          throw new ArgumentException(string.Format(
            "primitive serializer's ({0}) serialized data type ({1}) " + Environment.NewLine +
            "is not compatible with selected format's ({2}) seriali data type ({3})",
            primitiveSerializers.GetType().FullName, primitiveSerializer.SerialDataType.FullName,
            format.Name, format.SerialDataType.FullName));
        this.primitiveSerializers.Add(primitiveSerializer.SourceType, primitiveSerializer);
      }
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

    /// <summary>
    /// Copies this configuration and re-instantiates all serializers.
    /// </summary>
    /// <returns>A new <see cref="Configuration"/></returns>
    public Configuration Copy() {
      var config = new Configuration(false);
      config.Format = Format;
      foreach (var ps in primitiveSerializers)
        config.primitiveSerializers.Add(
          ps.Key,
          (IPrimitiveSerializer)Activator.CreateInstance(ps.Value.GetType()));
      foreach (var cs in compositeSerializers)
        config.compositeSerializers.Add((ICompositeSerializer)Activator.CreateInstance(cs.GetType()));
      return config;
    }

  }

}