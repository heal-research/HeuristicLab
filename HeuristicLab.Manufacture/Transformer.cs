using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace ParameterTest {
  public static class Transformer {
    private static IDictionary<Type, ITypeTransformer> transformers = new Dictionary<Type, ITypeTransformer>();
    
    public static void Register(Type type, ITypeTransformer transformer) {
      if(!transformers.ContainsKey(type))
        transformers.Add(type, transformer);
    }


    public static void Register<T>(ITypeTransformer transformer) => Register(typeof(T), transformer);

    public static ITypeTransformer Get(Type type) {
      foreach (var x in transformers) {
        if (type.IsEqualTo(x.Key)) return x.Value;
      }
      return null;
    }

    internal static void SetValue(IItem item, ParameterData data) {
      Get(item.GetType()).SetValue(item, data);
    }


    static Transformer() {
      Register<IntValue>(new ValueTypeValueTransformer<IntValue, int>());
      Register<DoubleValue>(new ValueTypeValueTransformer<DoubleValue, double>());
      Register<PercentValue>(new ValueTypeValueTransformer<PercentValue, double>());
      Register<BoolValue>(new ValueTypeValueTransformer<BoolValue, bool>());
      Register<DateTimeValue>(new ValueTypeValueTransformer<DateTimeValue, DateTime>());
      Register<StringValue>(new StringValueTransformer());

      Register<IntArray>(new ValueTypeArrayTransformer<IntArray, int>());
      Register<DoubleArray>(new ValueTypeArrayTransformer<DoubleArray, double>());
      Register<PercentArray>(new ValueTypeArrayTransformer<PercentArray, double>());
      Register<BoolArray>(new ValueTypeArrayTransformer<BoolArray, bool>());

      Register<IntMatrix>(new ValueTypeMatrixTransformer<IntMatrix, int>());
      Register<DoubleMatrix>(new ValueTypeMatrixTransformer<DoubleMatrix, double>());
      Register<PercentMatrix>(new ValueTypeMatrixTransformer<PercentMatrix, double>());
      Register<BoolMatrix>(new ValueTypeMatrixTransformer<BoolMatrix, bool>());
    }

  }
}
