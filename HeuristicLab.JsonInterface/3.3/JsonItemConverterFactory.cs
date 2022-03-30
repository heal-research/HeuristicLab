using System;
using System.Collections.Generic;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.JsonInterface {
  public static class JsonItemConverterFactory {
    private static IEnumerable<IJsonItemConverter> ConverterCache { get; set; }

    public static JsonItemConverter Create() {
      if (ConverterCache == null)
        InitCache();
      return new JsonItemConverter(ConverterCache);
    }

    private static void InitCache() {
      IList<IJsonItemConverter> cache = new List<IJsonItemConverter>();
      foreach (var converter in ApplicationManager.Manager.GetInstances<IJsonItemConverter>()) {
        cache.Add(converter);
      }
      ConverterCache = cache;
    }
  }
}
