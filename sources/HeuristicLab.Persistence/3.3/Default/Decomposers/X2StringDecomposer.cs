using System;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System.Text;

namespace HeuristicLab.Persistence.Default.Decomposers {

  public class Number2StringConverter {    

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

    private static readonly Dictionary<Type, MethodInfo> numberParsers;  

    static Number2StringConverter() {
      numberParsers = new Dictionary<Type, MethodInfo>();
      foreach ( var type in numberTypes ) {
        numberParsers[type] = type
          .GetMethod("Parse", BindingFlags.Static | BindingFlags.Public,
                     null, new[] {typeof (string)}, null);          
      }
    }

    public bool CanDecompose(Type type) {
      return numberParsers.ContainsKey(type);
    }

    public string Format(object obj) {
      if (obj.GetType() == typeof(float))        
        return ((float)obj).ToString("r", CultureInfo.InvariantCulture);
      if (obj.GetType() == typeof(double))
        return ((double)obj).ToString("r", CultureInfo.InvariantCulture);
      if (obj.GetType() == typeof(decimal))
        return ((decimal)obj).ToString("r", CultureInfo.InvariantCulture);
      return obj.ToString();
    }
    
    public object Parse(string stringValue, Type type) {
      return numberParsers[type]
        .Invoke(null,
            BindingFlags.Static | BindingFlags.PutRefDispProperty,
                  null, new[] {stringValue}, CultureInfo.InvariantCulture);
    }

  }  

  public class DateTime2StringDecomposer : IDecomposer {

    public int Priority {
      get { return 100; }
    }


    public bool CanDecompose(Type type) {
      return type == typeof(DateTime);
    }

    public IEnumerable<Tag> Decompose(object obj) {
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

    public int Priority {
      get { return 200; }
    }
    
    private static readonly Number2StringConverter numberConverter =
      new Number2StringConverter();    

    public bool CanDecompose(Type type) {
      return
        (type.IsArray || type == typeof (Array)) &&
        numberConverter.CanDecompose(type.GetElementType());
    }

    public IEnumerable<Tag> Decompose(object obj) {
      Array a = (Array) obj;
      int[] lengths = new int[a.Rank];
      int[] lowerBounds = new int[a.Rank];
      StringBuilder sb = new StringBuilder();
      sb.Append(a.Rank).Append(';');
      for (int i = 0; i < a.Rank; i++) {
        sb.Append(a.GetLength(i)).Append(';');
        lengths[i] = a.GetLength(i);
      }
      for (int i = 0; i < a.Rank; i++) {
        sb.Append(a.GetLowerBound(i)).Append(';');
        lowerBounds[i] = a.GetLowerBound(i);
      }
      int[] positions = (int[])lowerBounds.Clone();
      while (positions[a.Rank - 1] < lengths[a.Rank - 1] + lowerBounds[a.Rank - 1]) {
        sb.Append(numberConverter.Format(a.GetValue(positions))).Append(';');
        positions[0] += 1;
        for (int i = 0; i < a.Rank - 1; i++) {
          if (positions[i] >= lengths[i] + lowerBounds[i]) {
            positions[i] = lowerBounds[i];
            positions[i + 1] += 1;
          } else {
            break;
          }
        }
      }
      yield return new Tag("compact array", sb.ToString());
    }

    public object CreateInstance(Type type) {
      return null;
    }

    public object Populate(object instance, IEnumerable<Tag> tags, Type type) {      
      var tagIter = tags.GetEnumerator();
      tagIter.MoveNext();
      var valueIter = ((string) tagIter.Current.Value)
        .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
        .GetEnumerator();
      valueIter.MoveNext();
      int rank = int.Parse((string) valueIter.Current);      
      int[] lengths = new int[rank];
      int[] lowerBounds = new int[rank];      
      for (int i = 0; i < rank; i++) {
        valueIter.MoveNext();
        lengths[i] = int.Parse((string) valueIter.Current);        
      }      
      for (int i = 0; i < rank; i++) {
        valueIter.MoveNext();
        lowerBounds[i] = int.Parse((string) valueIter.Current);        
      }
      Type elementType = type.GetElementType();
      Array a = Array.CreateInstance(elementType, lengths, lowerBounds);
      int[] positions = (int[]) lowerBounds.Clone();
      while (valueIter.MoveNext()) {
        a.SetValue(
          numberConverter.Parse((string)valueIter.Current, elementType),          
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

    public int Priority {
      get { return 200; }
    }

    private static readonly Number2StringConverter numberConverter =
      new Number2StringConverter();
    
    private static readonly Dictionary<Type, Type> interfaceCache = new Dictionary<Type, Type>();

    public Type GetGenericEnumerableInterface(Type type) {
      if (interfaceCache.ContainsKey(type))
        return interfaceCache[type];
      foreach (Type iface in type.GetInterfaces()) {
        if (iface.IsGenericType &&
          iface.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
          numberConverter.CanDecompose(iface.GetGenericArguments()[0])) {
          interfaceCache.Add(type, iface);
          return iface;
        }
      }
      interfaceCache.Add(type, null);
      return null;
    }
    
    public bool ImplementsGenericEnumerable(Type type) {
      return GetGenericEnumerableInterface(type) != null;
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

    public IEnumerable<Tag> Decompose(object obj) {
      Type type = obj.GetType();
      Type enumerable = GetGenericEnumerableInterface(type);      
      InterfaceMapping iMap = obj.GetType().GetInterfaceMap(enumerable);      
      MethodInfo getEnumeratorMethod =
        iMap.TargetMethods[
        Array.IndexOf(
          iMap.InterfaceMethods,
          enumerable.GetMethod("GetEnumerator"))];
      object[] empty = new object[] {};
      object genericEnumerator = getEnumeratorMethod.Invoke(obj, empty);
      MethodInfo moveNextMethod = genericEnumerator.GetType().GetMethod("MoveNext");
      PropertyInfo currentProperty = genericEnumerator.GetType().GetProperty("Current");
      StringBuilder sb = new StringBuilder();
      while ( (bool)moveNextMethod.Invoke(genericEnumerator, empty) )
        sb.Append(
          numberConverter.Format(
            currentProperty.GetValue(genericEnumerator, null))).Append(';');
      yield return new Tag("compact enumerable", sb.ToString());
    }

    public object CreateInstance(Type type) {
      return Activator.CreateInstance(type, true);
    }

    public object Populate(object instance, IEnumerable<Tag> tags, Type type) {
      Type enumerable = GetGenericEnumerableInterface(type);
      Type elementType = enumerable.GetGenericArguments()[0];      
      MethodInfo addMethod = type.GetMethod("Add");      
      var tagEnumerator = tags.GetEnumerator();
      tagEnumerator.MoveNext();
      string[] stringValues = ((string) tagEnumerator.Current.Value)
        .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
      foreach (var value in stringValues) {
        addMethod.Invoke(instance, new[] {numberConverter.Parse(value, elementType)});
      }      
      return instance;      
    }
    
  }

}