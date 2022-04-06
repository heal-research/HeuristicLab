using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Parameters;

namespace HeuristicLab.Optimization {

  [StorableType("4AF9EF94-2EF3-4F54-8B8D-942454CF8DDF")]
  public class FilterResultCollectionProcessor : ParameterizedNamedItem, IRunCollectionModifier {

    #region Constants
    private const string FilterStringParameterName = "Filters";
    #endregion

    #region Parameter Properties
    public IFixedValueParameter<ItemList<StringValue>> FilterStringParameter =>
      (IFixedValueParameter<ItemList<StringValue>>)Parameters[FilterStringParameterName];
    #endregion

    #region Constructors & Cloning
    [StorableConstructor]
    protected FilterResultCollectionProcessor(StorableConstructorFlag _) : base(_) { }
    public FilterResultCollectionProcessor() {
      Parameters.Add(new FixedValueParameter<ItemList<StringValue>>(
        FilterStringParameterName, 
        "All string values, which match result names are passed.", 
        new ItemList<StringValue>()));
    }
    public FilterResultCollectionProcessor(FilterResultCollectionProcessor original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new FilterResultCollectionProcessor(this, cloner);
    }
    #endregion

    public void Modify(List<IRun> runs) {
      foreach(var run in runs) {
        foreach(var filter in FilterStringParameter.Value) {
          if(!run.Results.ContainsKey(filter.Value))
            run.Results.Remove(filter.Value);
        }
      }
    }
  }
}
