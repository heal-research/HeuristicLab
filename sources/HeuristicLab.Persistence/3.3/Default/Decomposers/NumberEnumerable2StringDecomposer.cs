using System;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System.Text;
using HeuristicLab.Persistence.Default.Decomposers.Storable;

namespace HeuristicLab.Persistence.Default.Decomposers {

  [EmptyStorableClass]
  public class NumberEnumerable2StringDecomposer : IDecomposer {

    public int Priority {
      get { return 200; }
    }

    private static readonly Number2StringDecomposer numberConverter =
      new Number2StringDecomposer();

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

    public IEnumerable<Tag> CreateMetaInfo(object o) {
      return new Tag[] { };
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
      object[] empty = new object[] { };
      object genericEnumerator = getEnumeratorMethod.Invoke(obj, empty);
      MethodInfo moveNextMethod = genericEnumerator.GetType().GetMethod("MoveNext");
      PropertyInfo currentProperty = genericEnumerator.GetType().GetProperty("Current");
      StringBuilder sb = new StringBuilder();
      while ((bool)moveNextMethod.Invoke(genericEnumerator, empty))
        sb.Append(
          numberConverter.Format(
            currentProperty.GetValue(genericEnumerator, null))).Append(';');
      yield return new Tag("compact enumerable", sb.ToString());
    }

    public object CreateInstance(Type type, IEnumerable<Tag> metaInfo) {
      return Activator.CreateInstance(type, true);
    }

    public void Populate(object instance, IEnumerable<Tag> tags, Type type) {
      Type enumerable = GetGenericEnumerableInterface(type);
      Type elementType = enumerable.GetGenericArguments()[0];
      MethodInfo addMethod = type.GetMethod("Add");
      try {
        var tagEnumerator = tags.GetEnumerator();
        tagEnumerator.MoveNext();
        string[] stringValues = ((string)tagEnumerator.Current.Value)
          .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var value in stringValues) {
          addMethod.Invoke(instance, new[] { numberConverter.Parse(value, elementType) });
        }
      } catch (InvalidOperationException e) {
        throw new PersistenceException("Insufficient element data to reconstruct number enumerable", e);
      } catch (InvalidCastException e) {
        throw new PersistenceException("Invalid element data during reconstruction of number enumerable", e);
      }
    }
  }
}