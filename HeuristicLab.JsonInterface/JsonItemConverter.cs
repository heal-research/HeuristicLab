using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.PluginInfrastructure;
using HEAL.Attic;
using System.Collections;

namespace HeuristicLab.JsonInterface {
  /// <summary>
  /// Static class for handling json converters.
  /// </summary>
  public static class JsonItemConverter {

    private struct ConverterPriorityContainer {
      public IJsonItemConverter Converter { get; set; }
      public int Priority { get; set; }
    }

    private static IDictionary<Type, ConverterPriorityContainer> Converters { get; set; } 
      = new Dictionary<Type, ConverterPriorityContainer>();
    
    /// <summary>
    /// Register a converter for a given type and priority.
    /// </summary>
    /// <param name="type">The type for which the converter will be selected.</param>
    /// <param name="converter">The implemented converter.</param>
    /// <param name="priority">The priority for the converter selection (when multiple converter match for a given type). Higher is better.</param>
    public static void Register(Type type, IJsonItemConverter converter, int priority) {
      if (!Converters.ContainsKey(type))
        Converters.Add(type, new ConverterPriorityContainer() { Converter = converter, Priority = priority });
    }


    public static void Register(string atticGuid, IJsonItemConverter converter, int priority) =>
      Register(new Guid(atticGuid), converter, priority);

    public static void Register(Guid atticGuid, IJsonItemConverter converter, int priority) {
      if (Mapper.StaticCache.TryGetType(atticGuid, out Type type)) {
        Register(type, converter, priority);
      }
    }

    public static void Register<T>(IJsonItemConverter converter, int priority) => 
      Register(typeof(T), converter, priority);

    /// <summary>
    /// Deregister a converter (same object has to be already registered).
    /// </summary>
    /// <param name="converter">Converter to deregister.</param>
    public static void Deregister(IJsonItemConverter converter) {
      var types = 
        Converters
        .Where(x => x.Value.Converter.GetHashCode() == converter.GetHashCode())
        .Select(x => x.Key);
      foreach (var x in types)
        Converters.Remove(x);
    }

    /// <summary>
    /// Get a converter for a specific type.
    /// </summary>
    /// <param name="type">The type for which the converter will be selected.</param>
    /// <returns>An IJsonItemConverter object.</returns>
    public static IJsonItemConverter Get(Type type) { 
      IList<ConverterPriorityContainer> possibleConverters = new List<ConverterPriorityContainer>();
      
      foreach (var x in Converters)
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


    /// <summary>
    /// Static constructor for default converter configuration.
    /// </summary>
    static JsonItemConverter() {
      Register<IntValue>(new ValueTypeValueConverter<IntValue, int>(), 1);
      Register<DoubleValue>(new ValueTypeValueConverter<DoubleValue, double>(), 1);
      Register<PercentValue>(new ValueTypeValueConverter<PercentValue, double>(), 2);
      Register<BoolValue>(new ValueTypeValueConverter<BoolValue, bool>(), 1);
      Register<DateTimeValue>(new ValueTypeValueConverter<DateTimeValue, DateTime>(), 1);
      Register<StringValue>(new StringValueConverter(), 1);

      Register<IntArray>(new ValueTypeArrayConverter<IntArray, int>(), 1);
      Register<DoubleArray>(new ValueTypeArrayConverter<DoubleArray, double>(), 1);
      Register<PercentArray>(new ValueTypeArrayConverter<PercentArray, double>(), 2);
      Register<BoolArray>(new ValueTypeArrayConverter<BoolArray, bool>(), 1);

      Register<IntMatrix>(new ValueTypeMatrixConverter<IntMatrix, int>(), 1);
      Register<DoubleMatrix>(new ValueTypeMatrixConverter<DoubleMatrix, double>(), 1);
      Register<PercentMatrix>(new ValueTypeMatrixConverter<PercentMatrix, double>(), 2);
      Register<BoolMatrix>(new ValueTypeMatrixConverter<BoolMatrix, bool>(), 1);

      Register<DoubleRange>(new DoubleRangeConverter(), 1);
      Register<IntRange>(new IntRangeConverter(), 1);

      Register(typeof(EnumValue<>), new EnumTypeConverter(), 1);
      
      Register<IValueParameter>(new ValueParameterConverter(), 2);
      Register<IParameterizedItem>(new ParameterizedItemConverter(), 2);
      Register<ILookupParameter>(new LookupParameterConverter(), 3);
      Register<IValueLookupParameter>(new ValueLookupParameterConverter(), 4);

      Register(typeof(IConstrainedValueParameter<>), new ConstrainedValueParameterConverter(), 3);
      Register(typeof(ICheckedMultiOperator<>), new MultiCheckedOperatorConverter(), 3);
      
      Register("EE612297-B1AF-42D2-BF21-AF9A2D42791C", new RegressionProblemDataConverter(), 20);

      // ISymbol
      /*
      Register("25137f88-66b9-48d7-a2bd-60190082e044", 
        new ConfigurableConverter()
        .Primitive("InitialFrequency", ElementType.Property, new object[] { 0.0, 1.0 })
        .Primitive("Enabled", ElementType.Property, true)
        .Primitive("MinimumArity", ElementType.Property, 0)
        .Primitive("MaximumArity", ElementType.Property, 10),
        5);
      */
      // SymbolicRegressionProblem
      /*
      Register("7DDCF683-96FC-4F70-BF4F-FE3A0B0DE6E0",
        new ConfigurableConverter()
        .
        10);
      */

      // Dataset
      /*
      Register("49F4D145-50D7-4497-8D8A-D190CD556CC8",
        new ConfigurableConverter() //TODO: set guid to enable "inheritance"?
        .PrimitiveEnumerable("storableData", ElementType.Field, new double[,] { }),
        5);
      */
      // ItemCollection<>
      /*
      Register("0BD4F01E-2D52-4E41-AEF8-611F10856D90",
        new ConfigurableConverter()
        .This((o, t) => {
          dynamic itemList = o as dynamic;
          IList<JsonItem> jsonItems = new List<JsonItem>();
          foreach (var obj in itemList) {
            IItem item = obj as IItem;
            jsonItems.Add(Extract(item));
          }
          return jsonItems;
        }),
        5);
      */
      // IItemList<>
      /*
      Register("466747d2-6a73-495b-ac68-7b0199d0f830",
        new ConfigurableConverter()
        .This((o, t) => {
          dynamic itemList = o as dynamic;
          IList<JsonItem> jsonItems = new List<JsonItem>();
          foreach (var obj in itemList) {
            IItem item = obj as IItem;
            jsonItems.Add(Extract(item));
          }
          return jsonItems;
        }),
        6);
      */
      // ICheckedItemList<>
      /*
      Register("ba4a82ca-92eb-47a1-95a7-f41f6ef470f4",
        new ConfigurableConverter()
        .This((o,t) => {
          dynamic itemList = o as dynamic;
          IList<JsonItem> jsonItems = new List<JsonItem>();
          int count = 0;
          foreach(var obj in itemList) {
            IItem item = obj as IItem;
            JsonItem checkedStatus = new JsonItem() {
              Name = "Checked",
              Value = itemList.ItemChecked(obj),
              Range = new object[] { false, true }
            };
            JsonItem value = Extract(item);

            jsonItems.Add(new JsonItem() {
              Name = item.ItemName + count++,
              Parameters = new JsonItem[] {
                checkedStatus, value
              },
              Type = item.GetType().AssemblyQualifiedName
            });
          }
          return jsonItems;
        }),
        6);
      */
      //ISymbolicExpressionGrammar - "1f6afcbe-b309-44e2-8d35-2d33eaeb9649"
      //ISymbolicExpressionGrammarBase - "119f5e94-bc7d-42fc-a48b-ac0230115ef2"
      /*
      Register("119f5e94-bc7d-42fc-a48b-ac0230115ef2",
        new ConfigurableConverter()
        .Enumerable("Symbols", ElementType.Property, (o,t) => new JsonItem[] { Extract(o as IItem) }),
        10);
      */
    }
  }
}
