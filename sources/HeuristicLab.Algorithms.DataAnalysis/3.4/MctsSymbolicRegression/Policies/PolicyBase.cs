using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis.MctsSymbolicRegression.Policies {
  [StorableClass]
  internal abstract class PolicyBase : Item, IParameterizedItem, IPolicy {
    [Storable]
    public IKeyedItemCollection<string, IParameter> Parameters { get; private set; }

    [StorableConstructor]
    private PolicyBase(bool deserializing) : base(deserializing) { }
    protected PolicyBase(PolicyBase original, Cloner cloner)
      : base(original, cloner) {
      Parameters = cloner.Clone(original.Parameters);
    }
    protected PolicyBase()
      : base() {
      Parameters = new ParameterCollection();
    }

    public void CollectParameterValues(IDictionary<string, IItem> values) {
      foreach (var p in Parameters) values.Add(this.ItemName + "." + p.Name, p.ActualValue);
    }

    public abstract int Select(IEnumerable<IActionStatistics> actions, IRandom random);
    public abstract void Update(IActionStatistics action, double q);
    public abstract IActionStatistics CreateActionStatistics();
  }
}
