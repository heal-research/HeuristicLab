#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Auxiliary;
using HeuristicLab.Persistence.Core.Tokens;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core {

  /// <summary>
  /// Core hub for deserialization. Reads the serialization token stream,
  /// instantiates objects and fills in values.
  /// </summary>
  public class Deserializer {

    /// <summary>
    /// Helps in delivering the class instance and acts as proxy while
    /// the object cannot yet be instantiate.
    /// </summary>
    private class Midwife {

      public int? Id { get; private set; }
      public bool MetaMode { get; set; }
      public object Obj { get; private set; }

      private List<Tag> metaInfo;
      private List<Tag> customValues;
      private Type type;
      private ICompositeSerializer compositeSerializer;

      public Midwife(object value) {
        this.Obj = value;
      }

      public Midwife(Type type, ICompositeSerializer compositeSerializer, int? id) {
        this.type = type;
        this.compositeSerializer = compositeSerializer;
        this.Id = id;
        MetaMode = false;
        metaInfo = new List<Tag>();
        customValues = new List<Tag>();
      }

      public void CreateInstance() {
        if (Obj != null)
          throw new PersistenceException("object already instantiated");
        Obj = compositeSerializer.CreateInstance(type, metaInfo);
      }

      public void AddValue(string name, object value) {
        if (MetaMode) {
          metaInfo.Add(new Tag(name, value));
        } else {
          customValues.Add(new Tag(name, value));
        }
      }

      public void Populate() {
        compositeSerializer.Populate(Obj, customValues, type);
      }
    }

    private readonly Dictionary<int, object> id2obj;
    private readonly Dictionary<Type, object> serializerMapping;
    private readonly Stack<Midwife> parentStack;
    private readonly Dictionary<int, Type> typeIds;
    private Dictionary<Type, object> serializerInstances;

    /// <summary>
    /// Instantiates a new deserializer with the given type cache,
    /// that contains information about the serializers to use
    /// for every type and their type ids.
    /// </summary>
    /// <param name="typeCache">The type cache.</param>
    public Deserializer(
      IEnumerable<TypeMapping> typeCache) {
      id2obj = new Dictionary<int, object>();
      parentStack = new Stack<Midwife>();
      typeIds = new Dictionary<int, Type>();
      serializerMapping = new Dictionary<Type, object>();
      serializerInstances = new Dictionary<Type, object>();
      foreach (var typeMapping in typeCache) {
        AddTypeInfo(typeMapping);
      }
    }

    /// <summary>
    /// Adds additionaly type information.
    /// </summary>
    /// <param name="typeMapping">The new type mapping.</param>
    public void AddTypeInfo(TypeMapping typeMapping) {
      if (typeIds.ContainsKey(typeMapping.Id))
        return;
      try {
        Type type = TypeLoader.Load(typeMapping.TypeName);
        typeIds.Add(typeMapping.Id, type);
        Type serializerType = TypeLoader.Load(typeMapping.Serializer);
        object serializer;
        if (serializerInstances.ContainsKey(serializerType)) {
          serializer = serializerInstances[serializerType];
        } else {
          serializer = Activator.CreateInstance(serializerType, true);
          serializerInstances.Add(serializerType, serializer);
        }
        serializerMapping.Add(type, serializer);
      } catch (PersistenceException) {
        throw;
      } catch (Exception e) {
        throw new PersistenceException(string.Format(
          "Could not add type info for {0} ({1})",
          typeMapping.TypeName, typeMapping.Serializer), e);
      }
    }

    /// <summary>
    /// Process the token stream and deserialize an instantate a new object graph.
    /// </summary>
    /// <param name="tokens">The tokens.</param>
    /// <returns>A fresh object filled with fresh data.</returns>
    public object Deserialize(IEnumerable<ISerializationToken> tokens) {
      foreach (ISerializationToken token in tokens) {
        Type t = token.GetType();
        if (t == typeof(BeginToken)) {
          CompositeStartHandler((BeginToken)token);
        } else if (t == typeof(EndToken)) {
          CompositeEndHandler((EndToken)token);
        } else if (t == typeof(PrimitiveToken)) {
          PrimitiveHandler((PrimitiveToken)token);
        } else if (t == typeof(ReferenceToken)) {
          ReferenceHandler((ReferenceToken)token);
        } else if (t == typeof(NullReferenceToken)) {
          NullHandler((NullReferenceToken)token);
        } else if (t == typeof(MetaInfoBeginToken)) {
          MetaInfoBegin((MetaInfoBeginToken)token);
        } else if (t == typeof(MetaInfoEndToken)) {
          MetaInfoEnd((MetaInfoEndToken)token);
        } else if (t == typeof(TypeToken)) {
          Type((TypeToken)token);
        } else {
          throw new PersistenceException("invalid token type");
        }
      }
      return parentStack.Pop().Obj;
    }

    private void InstantiateParent() {
      if (parentStack.Count == 0)
        return;
      Midwife m = parentStack.Peek();
      if (!m.MetaMode && m.Obj == null)
        CreateInstance(m);
    }

    private void Type(TypeToken token) {
      AddTypeInfo(new TypeMapping(token.Id, token.TypeName, token.Serializer));
    }

    private void CompositeStartHandler(BeginToken token) {
      InstantiateParent();
      Type type = typeIds[(int)token.TypeId];
      try {
        parentStack.Push(new Midwife(type, (ICompositeSerializer)serializerMapping[type], token.Id));
      } catch (Exception e) {
        if (e is InvalidCastException || e is KeyNotFoundException) {
          throw new PersistenceException(String.Format(
            "Invalid composite serializer configuration for type \"{0}\".",
            type.AssemblyQualifiedName), e);
        } else {
          throw new PersistenceException(String.Format(
            "Unexpected exception while trying to compose object of type \"{0}\".",
            type.AssemblyQualifiedName), e);
        }
      }
    }

    private void CompositeEndHandler(EndToken token) {
      Type type = typeIds[(int)token.TypeId];
      Midwife midwife = parentStack.Pop();
      if (midwife.Obj == null)
        CreateInstance(midwife);
      midwife.Populate();
      SetValue(token.Name, midwife.Obj);
    }

    private void PrimitiveHandler(PrimitiveToken token) {
      Type type = typeIds[(int)token.TypeId];
      try {
        object value = ((IPrimitiveSerializer)serializerMapping[type]).Parse(token.SerialData);
        if (token.Id != null)
          id2obj[(int)token.Id] = value;
        SetValue(token.Name, value);
      } catch (Exception e) {
        if (e is InvalidCastException || e is KeyNotFoundException) {
          throw new PersistenceException(String.Format(
            "Invalid primitive serializer configuration for type \"{0}\".",
            type.AssemblyQualifiedName), e);
        } else {
          throw new PersistenceException(String.Format(
            "Unexpected exception while trying to parse object of type \"{0}\".",
            type.AssemblyQualifiedName), e);
        }
      }
    }

    private void ReferenceHandler(ReferenceToken token) {
      object referredObject = id2obj[token.Id];
      SetValue(token.Name, referredObject);
    }

    private void NullHandler(NullReferenceToken token) {
      SetValue(token.Name, null);
    }

    private void MetaInfoBegin(MetaInfoBeginToken token) {
      parentStack.Peek().MetaMode = true;
    }

    private void MetaInfoEnd(MetaInfoEndToken token) {
      Midwife m = parentStack.Peek();
      m.MetaMode = false;
      CreateInstance(m);
    }

    private void CreateInstance(Midwife m) {
      m.CreateInstance();
      if (m.Id != null)
        id2obj.Add((int)m.Id, m.Obj);
    }

    private void SetValue(string name, object value) {
      if (parentStack.Count == 0) {
        parentStack.Push(new Midwife(value));
      } else {
        Midwife m = parentStack.Peek();
        if (m.MetaMode == false && m.Obj == null) {
          CreateInstance(m);
        }
        m.AddValue(name, value);
      }
    }
  }
}