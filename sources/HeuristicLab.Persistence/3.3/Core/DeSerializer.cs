using System.Collections.Generic;
using System;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core.Tokens;
using HeuristicLab.Persistence.Auxiliary;
using HeuristicLab.Tracing;
using System.Reflection;

namespace HeuristicLab.Persistence.Core {

  public class TypeLoader {

    public static Type Load(string typeNameString) {
      Type type;
      try {
        type = Type.GetType(typeNameString, true);
      } catch (Exception) {
        Logger.Warn(String.Format(
          "Cannot load type \"{0}\", falling back to loading with partial name", typeNameString));
        try {
          TypeName typeName = TypeNameParser.Parse(typeNameString);
          Assembly a = Assembly.LoadWithPartialName(typeName.AssemblyName);
          type = a.GetType(typeName.ToString(false, false), true);
        } catch (Exception) {
          throw new PersistenceException(String.Format(
            "Could not load type \"{0}\"",
            typeNameString));
        }
        try {
          if (
            TypeNameParser.Parse(type.AssemblyQualifiedName).IsOlderThan(
            TypeNameParser.Parse(typeNameString)))
            throw new PersistenceException(String.Format(
              "Serialized type is newer than available type: serialized: {0}, loaded: {1}",
              typeNameString,
              type.AssemblyQualifiedName));
        } catch (PersistenceException) {
          throw;
        } catch (Exception e) {
          Logger.Warn(String.Format(
            "Could not perform version check requested type was {0} while loaded type is {1}:",
            typeNameString,
            type.AssemblyQualifiedName),
            e);
        }
      }
      return type;
    }

  }

  public class Deserializer {

    class Midwife {

      public int? Id { get; private set; }
      public bool MetaMode { get; set; }
      public object Obj { get; private set; }

      private List<Tag> metaInfo;
      private List<Tag> customValues;
      private Type type;
      private IDecomposer decomposer;

      public Midwife(object value) {
        this.Obj = value;
      }

      public Midwife(Type type, IDecomposer decomposer, int? id) {
        this.type = type;
        this.decomposer = decomposer;
        this.Id = id;
        MetaMode = false;
        metaInfo = new List<Tag>();
        customValues = new List<Tag>();
      }

      public void CreateInstance() {
        if (Obj != null)
          throw new PersistenceException("object already instantiated");
        Obj = decomposer.CreateInstance(type, metaInfo);
      }

      public void AddValue(string name, object value) {
        if (MetaMode) {
          metaInfo.Add(new Tag(name, value));
        } else {
          customValues.Add(new Tag(name, value));
        }
      }

      public void Populate() {
        decomposer.Populate(Obj, customValues, type);
      }
    }

    private readonly Dictionary<int, object> id2obj;
    private readonly Dictionary<Type, object> serializerMapping;
    private readonly Stack<Midwife> parentStack;
    private readonly Dictionary<int, Type> typeIds;

    public Deserializer(
      IEnumerable<TypeMapping> typeCache) {
      id2obj = new Dictionary<int, object>();
      parentStack = new Stack<Midwife>();
      typeIds = new Dictionary<int, Type>();
      serializerMapping = CreateSerializers(typeCache);
    }

    private Dictionary<Type, object> CreateSerializers(IEnumerable<TypeMapping> typeCache) {
      try {
        var map = new Dictionary<Type, object>();
        foreach (var typeMapping in typeCache) {
          Type type = TypeLoader.Load(typeMapping.TypeName);
          typeIds.Add(typeMapping.Id, type);
          Type serializerType = TypeLoader.Load(typeMapping.Serializer);
          map.Add(type, Activator.CreateInstance(serializerType, true));
        }
        return map;
      } catch (PersistenceException) {
        throw;
      } catch (Exception e) {
        throw new PersistenceException(
          "The serialization type cache could not be loaded.\r\n" +
          "This usualy happens when you are missing an Assembly/Plugin.", e);
      }
    }


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
        } else {
          throw new PersistenceException("invalid token type");
        }
      }
      return parentStack.Pop().Obj;
    }

    private void CompositeStartHandler(BeginToken token) {
      Type type = typeIds[(int)token.TypeId];
      IDecomposer decomposer = null;
      if (serializerMapping.ContainsKey(type))
        decomposer = serializerMapping[type] as IDecomposer;
      if (decomposer == null)
        throw new PersistenceException(String.Format(
          "No suitable method for deserialization of type \"{0}\" found.",
          type.VersionInvariantName()));
      parentStack.Push(new Midwife(type, decomposer, token.Id));
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
      object value = ((IFormatter)serializerMapping[type]).Parse(token.SerialData);
      if (token.Id != null)
        id2obj[(int)token.Id] = value;
      SetValue(token.Name, value);
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
        if (m.MetaMode == false && m.Obj == null)
          CreateInstance(m);
        m.AddValue(name, value);
      }
    }
  }
}