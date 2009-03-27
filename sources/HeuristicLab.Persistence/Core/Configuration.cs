using System;
using System.Collections.Generic;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core {

  public class Configuration {

    [Storable]
    private readonly Dictionary<Type, IFormatter> formatters;
    [Storable]
    private readonly List<IDecomposer> decomposers;
    private readonly Dictionary<Type, IDecomposer> decomposerCache;
    [Storable]
    public readonly IFormat Format;

    private Configuration() {
      decomposerCache = new Dictionary<Type, IDecomposer>();
    }

    public Configuration(Dictionary<Type, IFormatter> formatters, IEnumerable<IDecomposer> decomposers) {      
      this.formatters = new Dictionary<Type, IFormatter>();
      foreach ( var pair in formatters ) {
        if (Format == null) {
          Format = pair.Value.Format;
        } else if (pair.Value.Format != Format ) {
          throw new ArgumentException("All formatters must have the same IFormat.");
        }
        this.formatters.Add(pair.Key, pair.Value);
      }
      this.decomposers = new List<IDecomposer>(decomposers);
      decomposerCache = new Dictionary<Type, IDecomposer>();      
    }

    public IEnumerable<IFormatter> Formatters {
      get { return formatters.Values; }
    }

    public IEnumerable<IDecomposer> Decomposers {
      get { return decomposers; }
    }

    public IFormatter GetFormatter(Type type) {      
      IFormatter formatter;
      formatters.TryGetValue(type, out formatter);
      return formatter;
    }

    public IDecomposer GetDecomposer(Type type) {
      if (decomposerCache.ContainsKey(type))
        return decomposerCache[type];      
      foreach (IDecomposer d in decomposers) {
        if (d.CanDecompose(type)) {
          decomposerCache.Add(type, d);
          return d;
        }
      }
      decomposerCache.Add(type, null);
      return null;
    }    
  }  
  
}