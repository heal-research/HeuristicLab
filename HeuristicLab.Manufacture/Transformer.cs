using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Manufacture {
  public static class Transformer {
    private static IDictionary<Type, ITypeTransformer> transformers = new Dictionary<Type, ITypeTransformer>();
    
    public static void Register(Type type, ITypeTransformer transformer) {
      if(!transformers.ContainsKey(type))
        transformers.Add(type, transformer);
    }


    public static void Register<T>(ITypeTransformer transformer) => Register(typeof(T), transformer);

    public static ITypeTransformer Get(Type type) {
      IList<KeyValuePair<Type, ITypeTransformer>> possibleTransformers = new List<KeyValuePair<Type, ITypeTransformer>>();
      foreach (var x in transformers) {
        if (type.IsEqualTo(x.Key))
          possibleTransformers.Add(x);
      }

      if(possibleTransformers.Count > 0) {
        ITypeTransformer nearest = possibleTransformers.First().Value;
        int nearestDistance = -1;
        foreach (var x in possibleTransformers) {
          int d = type.GetInterfaceDistance(x.Key);
          if (d != -1 && (nearestDistance == -1 || d < nearestDistance)) {
            nearestDistance = d;
            nearest = x.Value;
          }
        }
        return nearest;
      }
      return new DummyTransformer();
    }

    internal static void Inject(IItem item, ParameterData data) {
      Get(item.GetType()).Inject(item, data);
    }

    internal static ParameterData Extract(IItem item) {
      return Get(item.GetType()).Extract(item);
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

      Register(typeof(IConstrainedValueParameter<>), new ConstrainedValueParameterTransformer());
      Register(typeof(ILookupParameter), new LookupParameterTransformer());
      Register(typeof(IValueParameter), new ValueParameterTransformer());
    }

  }
}
