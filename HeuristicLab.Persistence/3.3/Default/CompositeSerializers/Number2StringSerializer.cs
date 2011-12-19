#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Persistence.Auxiliary;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Persistence.Default.Xml.Primitive;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Default.CompositeSerializers {

  /// <summary>
  /// Serializes a primitive number type using the ToString() method and an
  /// approriate precision and parses back the generated string using
  /// the number type's Parse() method.
  /// 
  /// This serializer has Priorty below zero and is disabled by default
  /// but can be useful in generating custom serializers.
  /// </summary>
  [StorableClass]
  public sealed class Number2StringSerializer : ICompositeSerializer {

    [StorableConstructor]
    private Number2StringSerializer(bool deserializing) { }
    public Number2StringSerializer() { }

    private static readonly Dictionary<Type, IPrimitiveSerializer> numberSerializerMap;
    private static readonly List<IPrimitiveSerializer> numberSerializers = new List<IPrimitiveSerializer> {
      new Bool2XmlSerializer(),
      new Byte2XmlSerializer(),
      new SByte2XmlSerializer(),
      new Short2XmlSerializer(),
      new UShort2XmlSerializer(),
      new Int2XmlSerializer(),
      new UInt2XmlSerializer(),
      new Long2XmlSerializer(),
      new ULong2XmlSerializer(),
      new Float2XmlSerializer(),
      new Double2XmlSerializer(),
      new Decimal2XmlSerializer(),
    };

    static Number2StringSerializer() {
      numberSerializerMap = new Dictionary<Type, IPrimitiveSerializer>();
      foreach (var s in numberSerializers) {
        numberSerializerMap[s.SourceType] = s;
      }
    }

    /// <summary>
    /// Determines for every type whether the composite serializer is applicable.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>
    /// 	<c>true</c> if this instance can serialize the specified type; otherwise, <c>false</c>.
    /// </returns>
    public bool CanSerialize(Type type) {
      return numberSerializerMap.ContainsKey(type);
    }

    /// <summary>
    /// Give a reason if possibly why the given type cannot be serialized by this
    /// ICompositeSerializer.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>
    /// A string justifying why type cannot be serialized.
    /// </returns>
    public string JustifyRejection(Type type) {
      return string.Format("not a number type (one of {0})",
        string.Join(", ", numberSerializers.Select(n => n.SourceType.Name).ToArray()));
    }

    /// <summary>
    /// Formats the specified obj.
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <returns></returns>
    public string Format(object obj) {
      return ((XmlString)numberSerializerMap[obj.GetType()].Format(obj)).Data;
    }

    /// <summary>
    /// Parses the specified string value.
    /// </summary>
    /// <param name="stringValue">The string value.</param>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public object Parse(string stringValue, Type type) {
      try {
        return numberSerializerMap[type].Parse(new XmlString(stringValue));
      }
      catch (FormatException e) {
        throw new PersistenceException("Invalid element data during number parsing.", e);
      }
      catch (OverflowException e) {
        throw new PersistenceException("Overflow during number parsing.", e);
      }
    }



    /// <summary>
    /// Defines the Priorty of this composite serializer. Higher number means
    /// higher prioriy. Negative numbers are fallback serializers that are
    /// disabled by default.
    /// All default generic composite serializers have priority 100. Specializations
    /// have priority 200 so they will  be tried first. Priorities are
    /// only considered for default configurations.
    /// </summary>
    /// <value></value>
    public int Priority {
      get { return -100; }
    }

    /// <summary>
    /// Generate MetaInfo necessary for instance creation. (e.g. dimensions
    /// necessary for array creation.
    /// </summary>
    /// <param name="obj">An object.</param>
    /// <returns>An enumerable of <see cref="Tag"/>s.</returns>
    public IEnumerable<Tag> CreateMetaInfo(object obj) {
      yield return new Tag(Format(obj));
    }

    /// <summary>
    /// Decompose an object into <see cref="Tag"/>s, the tag name can be null,
    /// the order in which elements are generated is guaranteed to be
    /// the same as they will be supplied to the Populate method.
    /// </summary>
    /// <param name="obj">An object.</param>
    /// <returns>An enumerable of <see cref="Tag"/>s.</returns>
    public IEnumerable<Tag> Decompose(object obj) {
      // numbers are composed just of meta info
      return new Tag[] { };
    }

    /// <summary>
    /// Create an instance of the object using the provided meta information.
    /// </summary>
    /// <param name="type">A type.</param>
    /// <param name="metaInfo">The meta information.</param>
    /// <returns>A fresh instance of the provided type.</returns>
    public object CreateInstance(Type type, IEnumerable<Tag> metaInfo) {
      var it = metaInfo.GetEnumerator();
      try {
        it.MoveNext();
        return Parse((string)it.Current.Value, type);
      }
      catch (InvalidOperationException e) {
        throw new PersistenceException(
          String.Format("Insufficient meta information to reconstruct number of type {0}.",
          type.VersionInvariantName()), e);
      }
      catch (InvalidCastException e) {
        throw new PersistenceException("Invalid meta information element type", e);
      }
    }

    /// <summary>
    /// Fills an object with values from the previously generated <see cref="Tag"/>s
    /// in Decompose. The order in which the values are supplied is
    /// the same as they where generated. <see cref="Tag"/> names might be null.
    /// </summary>
    /// <param name="instance">An empty object instance.</param>
    /// <param name="tags">The tags.</param>
    /// <param name="type">The type.</param>
    public void Populate(object instance, IEnumerable<Tag> tags, Type type) {
      // numbers are composed just of meta info, no need to populate
    }
  }
}