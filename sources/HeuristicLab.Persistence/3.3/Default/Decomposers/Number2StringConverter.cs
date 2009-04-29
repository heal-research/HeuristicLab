using System;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Auxiliary;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System.Text;

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

    private static readonly Dictionary<Type, MethodInfo> numberParsers;

    static Number2StringDecomposer() {
      numberParsers = new Dictionary<Type, MethodInfo>();
      foreach (var type in numberTypes) {
        numberParsers[type] = type
          .GetMethod("Parse", BindingFlags.Static | BindingFlags.Public,
                     null, new[] { typeof(string) }, null);
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
      try {
        return numberParsers[type]
          .Invoke(null,
              BindingFlags.Static | BindingFlags.PutRefDispProperty,
                    null, new[] { stringValue }, CultureInfo.InvariantCulture);
      } catch (FormatException e) {
        throw new PersistenceException("Invalid element data during number parsing.", e);
      } catch (OverflowException e) {
        throw new PersistenceException("Overflow during number parsing.", e);
      }
    }


    #region IDecomposer Members

    public int Priority {
      get { return -100; }
    }

    public IEnumerable<Tag> CreateMetaInfo(object obj) {
      yield return new Tag(Format(obj));
    }

    public IEnumerable<Tag> Decompose(object obj) {
      // numbers are composed just of meta info
      return new Tag[] { };
    }

    public object CreateInstance(Type type, IEnumerable<Tag> metaInfo) {
      var it = metaInfo.GetEnumerator();
      try {
        it.MoveNext();
        return Parse((string)it.Current.Value, type);
      } catch (InvalidOperationException e) {
        throw new PersistenceException(
          String.Format("Insufficient meta information to reconstruct number of type {0}.",
          type.VersionInvariantName()), e);
      } catch (InvalidCastException e) {
        throw new PersistenceException("Invalid meta information element type", e);
      }
    }

    public void Populate(object instance, IEnumerable<Tag> tags, Type type) {
      // numbers are composed just of meta info, no need to populate
    }

    #endregion
  }

}