using System;
using System.Collections.Generic;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.JsonInterface {
  public static class JsonItemConverterFactory {
    private static IDictionary<Type, IJsonItemConverter> ConverterCache { get; set; }

    public static JsonItemConverter Create() {
      if (ConverterCache == null)
        InitCache();
      return new JsonItemConverter(ConverterCache);
    }

    private static void InitCache() {
      ConverterCache = new Dictionary<Type, IJsonItemConverter>();
      foreach (var converter in ApplicationManager.Manager.GetInstances<IJsonItemConverter>()) {
        ConverterCache.Add(converter.ConvertableType, converter);
      }
    }
  }
}
