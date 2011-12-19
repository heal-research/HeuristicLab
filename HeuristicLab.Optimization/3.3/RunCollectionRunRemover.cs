using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [Item("RunCollection Run Remover", "Removes all currently visible runs. Use the filtering tab to selectively remove runs.")]
  [StorableClass]
  public class RunCollectionRunRemover : ParameterizedNamedItem, IRunCollectionModifier {

    #region Construction & Cloning
    [StorableConstructor]
    protected RunCollectionRunRemover(bool deserializing) : base(deserializing) { }
    protected RunCollectionRunRemover(RunCollectionRunRemover original, Cloner cloner) : base(original, cloner) {}
    public RunCollectionRunRemover() {}
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RunCollectionRunRemover(this, cloner);
    }
    #endregion

    #region IRunCollectionModifier Members
    public void Modify(List<IRun> runs) {
      runs.Clear();
    }
    #endregion

  }
}
