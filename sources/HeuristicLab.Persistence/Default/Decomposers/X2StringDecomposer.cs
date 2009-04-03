using System;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System.Text;
using System.Collections;

namespace HeuristicLab.Persistence.Default.Decomposers {

  public class Number2StringDecomposer : IDecomposer {

    private static readonly List<Type> numberTypes =
      new List<Type> {
        typeof(bool),
        typeof(byte),
        typeof(sbyte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(float),
        typeof(double),
        typeof(decimal),
      };

    private static readonly Dictionary<Type, MethodInfo> numberParser;  

    static Number2StringDecomposer() {
      numberParser = new Dictionary<Type, MethodInfo>();
      foreach ( var type in numberTypes ) {
        numberParser[type] = type
          .GetMethod("Parse", BindingFlags.Static | BindingFlags.Public,
                     null, new[] {typeof (string)}, null);          
      }
    }

    public bool CanDecompose(Type type) {
      return numberParser.ContainsKey(type);
    }

    public string SimpleDecompose(object obj) {
      if (obj.GetType() == typeof(float))        
        return ((float)obj).ToString("r", CultureInfo.InvariantCulture);
      else if (obj.GetType() == typeof(double))
        return ((double)obj).ToString("r", CultureInfo.InvariantCulture);
      else if (obj.GetType() == typeof(decimal))
        return ((decimal)obj).ToString("r", CultureInfo.InvariantCulture);
      else
        return obj.ToString();      
    }

    public IEnumerable<Tag> DeCompose(object obj) {      
      yield return new Tag(SimpleDecompose(obj));      
    }

    public object CreateInstance(Type type) {
      return null;
    }

    public object Parse(string stringValue, Type type) {
      return numberParser[type]
        .Invoke(null,
            BindingFlags.Static | BindingFlags.PutRefDispProperty,
                  null, new[] {stringValue}, CultureInfo.InvariantCulture);
    }

    public object Populate(object instance, IEnumerable<Tag> tags, Type type) {      
      foreach (Tag tag in tags)
        return Parse((string)tag.Value, type);
      throw new ApplicationException("not enough tags to re-compose number.");
    }

  }  

  public class DateTime2StringDecomposer : IDecomposer {

    public bool CanDecompose(Type type) {
      return type == typeof(DateTime);
    }

    public IEnumerable<Tag> DeCompose(object obj) {
      yield return new Tag(((DateTime)obj).Ticks);
    }

    public object CreateInstance(Type type) {
      return null;
    }

    public object Populate(object instance, IEnumerable<Tag> tags, Type type) {
      foreach (Tag tag in tags) {
        return new DateTime((long)tag.Value);
      }
      throw new ApplicationException("Not enough components to compose a bool.");
    }

  }  

  public class CompactNumberArray2StringDecomposer : IDecomposer {
    
    private static readonly Number2StringDecomposer numberDecomposer =
      new Number2StringDecomposer();    

    public bool CanDecompose(Type type) {
      return
        (type.IsArray || type == typeof (Array)) &&
        numberDecomposer.CanDecompose(type.GetElementType());
    }

    public IEnumerable<Tag> DeCompose(object obj) {
      Array a = (Array) obj;
      StringBuilder sb = new StringBuilder();
      sb.Append(a.Rank).Append(';');      
      for ( int i = 0; i<a.Rank; i++ )
        sb.Append(a.GetLength(i)).Append(';');
      for ( int i = 0; i<a.Rank; i++)
        sb.Append(a.GetLowerBound(i)).Append(';');
      foreach (var number in a) {        
        sb.Append(numberDecomposer.SimpleDecompose(number)).Append(';');
      }
      yield return new Tag("compact array", sb.ToString());
    }

    public object CreateInstance(Type type) {
      return null;
    }

    public object Populate(object instance, IEnumerable<Tag> tags, Type type) {      
      var iter = tags.GetEnumerator();
      iter.MoveNext();
      var it = ((string) iter.Current.Value).Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries).GetEnumerator();
      it.MoveNext();
      int rank = int.Parse((string) it.Current);
      it.MoveNext();
      int[] lengths = new int[rank];
      int[] lowerBounds = new int[rank];
      for (int i = 0; i < rank; i++, it.MoveNext())
        lengths[i] = int.Parse((string) it.Current);
      for (int i = 0; i < rank; i++, it.MoveNext())
        lowerBounds[i] = int.Parse((string)it.Current);
      Array a = Array.CreateInstance(type.GetElementType(), lengths, lowerBounds);
      int[] positions = (int[]) lowerBounds.Clone();
      while (it.MoveNext()) {
        a.SetValue(
          numberDecomposer.Parse((string)it.Current, type.GetElementType()),          
          positions);
        positions[0] += 1;
        for ( int i = 0; i<rank-1; i++ ) {
          if (positions[i] >= lengths[i] + lowerBounds[i]) {
            positions[i + 1] += 1;
            positions[i] = lowerBounds[i];
          } else {
            break;
          }
        }
      }
      return a;
    }
  }

  public class NumberEnumerable2StringDecomposer : IDecomposer {

    private static readonly Number2StringDecomposer numberDecomposer =
      new Number2StringDecomposer();
    
    public bool ImplementsGenericEnumerable(Type type) {
      foreach( Type iface in type.GetInterfaces() ) {
        if ( iface.IsGenericType &&
          iface.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
          numberDecomposer.CanDecompose(iface.GetGenericArguments()[0]) )
          return true;
      }
      return false;
    }

    public bool HasAddMethod(Type type) {
      return
        type.GetMethod("Add") != null &&
        type.GetMethod("Add").GetParameters().Length == 1 &&
        type.GetConstructor(
          BindingFlags.Public |
          BindingFlags.NonPublic |
          BindingFlags.Instance,
          null, Type.EmptyTypes, null) != null;      
    }

    public bool CanDecompose(Type type) {
      return
        ImplementsGenericEnumerable(type) &&
        HasAddMethod(type);
    }

    public IEnumerable<Tag> DeCompose(object obj) {
      Type elementType = obj.GetType().GetGenericArguments()[0];
      Type instantiatedGenericInterface =
        typeof (IEnumerable<>).MakeGenericType(new[] {elementType});
      MethodInfo genericGetEnumeratorMethod =
        instantiatedGenericInterface.GetMethod("GetEnumerator");
      InterfaceMapping iMap = obj.GetType().GetInterfaceMap(instantiatedGenericInterface);
      MethodInfo getEnumeratorMethod =
        iMap.TargetMethods[Array.IndexOf(iMap.InterfaceMethods, genericGetEnumeratorMethod)];
      object[] empty = new object[] {};
      object genericEnumerator = getEnumeratorMethod.Invoke(obj, empty);
      MethodInfo moveNextMethod = genericEnumerator.GetType().GetMethod("MoveNext");
      PropertyInfo currentProperty = genericEnumerator.GetType().GetProperty("Current");
      StringBuilder sb = new StringBuilder();
      while ( (bool)moveNextMethod.Invoke(genericEnumerator, empty) )
        sb.Append(
          numberDecomposer.SimpleDecompose(
            currentProperty.GetValue(genericEnumerator, null))).Append(';');
      yield return new Tag("compact enumerable", sb.ToString());
    }

    public object CreateInstance(Type type) {
      return Activator.CreateInstance(type, true);
    }

    public object Populate(object instance, IEnumerable<Tag> tags, Type type) {
      Type elementType = type.GetGenericArguments()[0];
      MethodInfo addMethod = type.GetMethod("Add");      
      var tagEnumerator = tags.GetEnumerator();
      tagEnumerator.MoveNext();
      string[] stringValues = ((string) tagEnumerator.Current.Value)
        .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
      foreach (var value in stringValues) {
        addMethod.Invoke(instance, new[] {numberDecomposer.Parse(value, elementType)});
      }      
      return instance;      
    }
    
  }

}