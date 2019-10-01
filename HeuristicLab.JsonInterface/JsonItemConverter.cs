using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.JsonInterface {
  public static class JsonItemConverter {

    private struct ConverterPriorityContainer {
      public IJsonItemConverter Converter { get; set; }
      public int Priority { get; set; }
    }

    private static IDictionary<Type, ConverterPriorityContainer> transformers = new Dictionary<Type, ConverterPriorityContainer>();
    
    public static void Register(Type type, IJsonItemConverter converter, int priority) {
      if (!transformers.ContainsKey(type))
        transformers.Add(type, new ConverterPriorityContainer() { Converter = converter, Priority = priority });
    }

    public static void Register<T>(IJsonItemConverter converter, int priority) => Register(typeof(T), converter, priority);

    public static IJsonItemConverter Get(Type type) { 
      IList<ConverterPriorityContainer> possibleConverters = new List<ConverterPriorityContainer>();

      foreach (var x in transformers)
        if (type.IsEqualTo(x.Key))
          possibleConverters.Add(x.Value);

      if(possibleConverters.Count > 0) {
        ConverterPriorityContainer best = possibleConverters.First();
        foreach (var x in possibleConverters) {
          if (x.Priority > best.Priority)
            best = x;
        }
        return best.Converter;
      }
      return new DummyConverter();
    }

    internal static void Inject(IItem item, JsonItem data) =>
      Get(item.GetType()).Inject(item, data);

    internal static JsonItem Extract(IItem item) => 
      Get(item.GetType()).Extract(item);


    static JsonItemConverter() {
      Register<IntValue>(new ValueTypeValueConverter<IntValue, int>(), 1);
      Register<DoubleValue>(new ValueTypeValueConverter<DoubleValue, double>(), 1);
      Register<PercentValue>(new ValueTypeValueConverter<PercentValue, double>(), 1);
      Register<BoolValue>(new ValueTypeValueConverter<BoolValue, bool>(), 1);
      Register<DateTimeValue>(new ValueTypeValueConverter<DateTimeValue, DateTime>(), 1);
      Register<StringValue>(new StringValueConverter(), 1);

      Register<IntArray>(new ValueTypeArrayConverter<IntArray, int>(), 1);
      Register<DoubleArray>(new ValueTypeArrayConverter<DoubleArray, double>(), 1);
      Register<PercentArray>(new ValueTypeArrayConverter<PercentArray, double>(), 1);
      Register<BoolArray>(new ValueTypeArrayConverter<BoolArray, bool>(), 1);

      Register<IntMatrix>(new ValueTypeMatrixConverter<IntMatrix, int>(), 1);
      Register<DoubleMatrix>(new ValueTypeMatrixConverter<DoubleMatrix, double>(), 1);
      Register<PercentMatrix>(new ValueTypeMatrixConverter<PercentMatrix, double>(), 1);
      Register<BoolMatrix>(new ValueTypeMatrixConverter<BoolMatrix, bool>(), 1);

      Register(typeof(IConstrainedValueParameter<>), new ConstrainedValueParameterConverter(), 1);
      Register(typeof(ILookupParameter), new LookupParameterConverter(), 1);
      Register(typeof(IValueParameter), new ValueParameterConverter(), 1);
      Register(typeof(IParameterizedItem), new ParameterizedItemConverter(), 1);
      Register(typeof(ICheckedMultiOperator<>), new MultiCheckedOperatorConverter(), 2);
      Register(typeof(EnumValue<>), new EnumTypeConverter(), 1);
    }

  }
}
