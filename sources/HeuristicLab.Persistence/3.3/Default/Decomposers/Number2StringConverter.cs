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
      return numberParsers[type]
        .Invoke(null,
            BindingFlags.Static | BindingFlags.PutRefDispProperty,
                  null, new[] { stringValue }, CultureInfo.InvariantCulture);
    }

  }

}