using System.Collections.Generic;
using System;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Interfaces.Tokens;

namespace HeuristicLab.Persistence.Core {  

  public class ParentReference {}  

  class CompositeObject {

    public object Obj { get; private set; }
    public List<Tag> customValues;

    public CompositeObject(object obj) {
      Obj = obj;
      customValues = new List<Tag>();
    }

    public void AddValue(string name, object value, List<Thunk> finalFixes) {
      Tag t = new Tag(name, value) {globalFinalFixes = finalFixes};
      customValues.Add(t);
    }

    public Setter GetSetterForLastAddedValue(string name) {            
      Tag t = customValues[customValues.Count - 1];      
      return value => t.Value = value;
    }
  }

  public delegate void Thunk();

  public class Deserializer {
    
    private readonly Dictionary<int, object> id2obj;
    private readonly Dictionary<Type, object> serializerMapping;    
    private readonly Stack<CompositeObject> parentStack;    
    private readonly Dictionary<int, Type> typeIds;    
    private List<Thunk> finalFixes;

    public Deserializer(
      IEnumerable<TypeMapping> typeCache) {      
      id2obj = new Dictionary<int, object>();
      parentStack = new Stack<CompositeObject>();
      typeIds = new Dictionary<int, Type>();
      serializerMapping = CreateSerializers(typeCache);
    }

    private Dictionary<Type, object> CreateSerializers(IEnumerable<TypeMapping> typeCache) {
      var map = new Dictionary<Type, object>();
      foreach (var typeMapping in typeCache) {
        Type type = Type.GetType(typeMapping.TypeName, true);
        typeIds.Add(typeMapping.Id, type);
        if (typeMapping.Serializer != null) {
          Type serializerType = Type.GetType(typeMapping.Serializer);
          map.Add(type, Activator.CreateInstance(serializerType, true));
        }
      }
      return map;
    }

    public object Deserialize(IEnumerable<ISerializationToken> tokens) {
      finalFixes = new List<Thunk>();      
      foreach (ISerializationToken token in tokens) {
        Type t = token.GetType();
        if ( t == typeof(BeginToken) ) {
          CompositeStartHandler((BeginToken)token);
        } else if ( t == typeof(EndToken) ) {
          CompositeEndHandler((EndToken) token);
        } else if ( t == typeof(PrimitiveToken) ) {
          PrimitiveHandler((PrimitiveToken) token);
        } else if ( t == typeof(ReferenceToken) ) {
          ReferenceHandler((ReferenceToken) token);
        } else if (t == typeof(NullReferenceToken)) {
          NullHandler((NullReferenceToken)token);
        } else {
          throw new ApplicationException("invalid token type");
        }
      }
      foreach (Thunk fix in finalFixes) {
        fix();
      }
      return parentStack.Pop().Obj;
    }

    private void CompositeStartHandler(BeginToken token) {      
      Type type = typeIds[(int)token.TypeId];
      IDecomposer decomposer = null;
      if ( serializerMapping.ContainsKey(type) )
        decomposer = serializerMapping[type] as IDecomposer;      
      if (decomposer == null)
        throw new ApplicationException(String.Format(
          "No suitable method for deserialization of type \"{0}\" found.",
          type.VersionInvariantName()));
      object instance = 
        decomposer.CreateInstance(type) ??
        new ParentReference();
      parentStack.Push(new CompositeObject(instance));              
      if ( token.Id != null )
        id2obj.Add((int)token.Id, instance);
    }

    private void CompositeEndHandler(EndToken token) {      
      Type type = typeIds[(int)token.TypeId];
      IDecomposer decomposer = null;
      if (serializerMapping.ContainsKey(type))
        decomposer = serializerMapping[type] as IDecomposer;            
      if (decomposer == null)
        throw new ApplicationException(String.Format(
          "No suitable method for deserialization of type \"{0}\" found.",
          type.VersionInvariantName()));
      CompositeObject customComposite = parentStack.Pop();
      object deserializedObject =          
        decomposer.Populate(customComposite.Obj, customComposite.customValues, type);
      if ( token.Id != null )
        id2obj[(int)token.Id] = deserializedObject;        
      SetValue(token.Name, deserializedObject);          
    }

    private void PrimitiveHandler(PrimitiveToken token) {      
      Type type = typeIds[(int)token.TypeId];
      object value = ((IFormatter) serializerMapping[type]).Parse(token.SerialData);
      if ( token.Id != null )      
        id2obj[(int)token.Id] = value;
      SetValue(token.Name, value);
    }

    private void ReferenceHandler(ReferenceToken token) {      
      object referredObject = id2obj[token.Id];
      SetValue(token.Name, referredObject);      
      if (referredObject is ParentReference) {
        Setter set = parentStack.Peek().GetSetterForLastAddedValue(token.Name);                
        finalFixes.Add(() => set(id2obj[token.Id]));
      } 
    }

    private void NullHandler(NullReferenceToken token) {      
      SetValue(token.Name, null);
    }    

    private void SetValue(string name, object value) {
      if (parentStack.Count == 0) {        
        parentStack.Push(new CompositeObject(value));
      } else {        
        parentStack.Peek().AddValue(name, value, finalFixes);        
      }
    }
  }
}