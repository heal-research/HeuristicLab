using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Manufacture {
  public static class JsonItemConverter {
    private static IDictionary<Type, IJsonItemConverter> transformers = new Dictionary<Type, IJsonItemConverter>();
    
    public static void Register(Type type, IJsonItemConverter transformer) { // TODO: explizit
      if (!transformers.ContainsKey(type))
        transformers.Add(type, transformer);
    }


    public static void Register<T>(IJsonItemConverter transformer) => Register(typeof(T), transformer);

    public static IEnumerable<IJsonItemConverter> Get(Type type) { 
      IList<KeyValuePair<Type, IJsonItemConverter>> possibleTransformers = new List<KeyValuePair<Type, IJsonItemConverter>>();


      foreach (var x in transformers) {
        if (type.IsEqualTo(x.Key))
          possibleTransformers.Add(x);
      }


      if(possibleTransformers.Count > 0) {
        IJsonItemConverter nearest = possibleTransformers.First().Value;
        int nearestDistance = -1;
        foreach (var x in possibleTransformers) {
          int d = type.GetInterfaceDistance(x.Key);
          if (d != -1 && (nearestDistance == -1 || d < nearestDistance)) {
            nearestDistance = d;
            nearest = x.Value;
          }
        }
        return new IJsonItemConverter[] { nearest };
        /*
        return possibleTransformers.OrderBy(x => {
          return type.GetInterfaceDistance(x.Key);
        }).Select(x => x.Value);
        */
      }
      return new IJsonItemConverter[] { new DummyConverter() };
    }

    internal static void Inject(IItem item, Component data) {
      IEnumerable<IJsonItemConverter> arr = Get(item.GetType());
      foreach (var transformer in arr)
        transformer.Inject(item, data);
    }

    internal static Component Extract(IItem item) {
      IEnumerable<IJsonItemConverter> arr = Get(item.GetType());
      Component data = new Component();
      foreach (var transformer in arr)
        Component.Merge(data, transformer.Extract(item));
      return data;
    }


    static JsonItemConverter() {
      Register<IntValue>(new ValueTypeValueConverter<IntValue, int>());
      Register<DoubleValue>(new ValueTypeValueConverter<DoubleValue, double>());
      Register<PercentValue>(new ValueTypeValueConverter<PercentValue, double>());
      Register<BoolValue>(new ValueTypeValueConverter<BoolValue, bool>());
      Register<DateTimeValue>(new ValueTypeValueConverter<DateTimeValue, DateTime>());
      Register<StringValue>(new StringValueConverter());

      Register<IntArray>(new ValueTypeArrayConverter<IntArray, int>());
      Register<DoubleArray>(new ValueTypeArrayConverter<DoubleArray, double>());
      Register<PercentArray>(new ValueTypeArrayConverter<PercentArray, double>());
      Register<BoolArray>(new ValueTypeArrayConverter<BoolArray, bool>());

      Register<IntMatrix>(new ValueTypeMatrixConverter<IntMatrix, int>());
      Register<DoubleMatrix>(new ValueTypeMatrixConverter<DoubleMatrix, double>());
      Register<PercentMatrix>(new ValueTypeMatrixConverter<PercentMatrix, double>());
      Register<BoolMatrix>(new ValueTypeMatrixConverter<BoolMatrix, bool>());

      Register(typeof(IConstrainedValueParameter<>), new ConstrainedValueParameterConverter());
      Register(typeof(ILookupParameter), new LookupParameterConverter());
      Register(typeof(IValueParameter), new ValueParameterConverter());
      Register(typeof(IParameterizedItem), new ParameterizedItemConverter());
      Register(typeof(ICheckedMultiOperator<>), new MultiCheckedOperatorConverter());
      Register(typeof(EnumValue<>), new EnumTypeConverter())
    }

  }
}
