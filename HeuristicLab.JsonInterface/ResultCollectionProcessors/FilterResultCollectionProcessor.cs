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

namespace HeuristicLab.JsonInterface {
  [StorableType("4AF9EF94-2EF3-4F54-8B8D-942454CF8DDF")]
  public class FilterResultCollectionProcessor : ParameterizedNamedItem, IResultCollectionProcessor {

    #region Constants
    private const string FilterStringParameterName = "Filters";
    #endregion

    #region Parameter Properties
    public IFixedValueParameter<StringValue> FilterStringParameter =>
      (IFixedValueParameter<StringValue>)Parameters[FilterStringParameterName];
    #endregion

    #region Constructors & Cloning
    [StorableConstructor]
    protected FilterResultCollectionProcessor(StorableConstructorFlag _) : base(_) { }
    public FilterResultCollectionProcessor() {
      Parameters.Add(new FixedValueParameter<StringValue>(FilterStringParameterName, "", new StringValue()));
    }
    public FilterResultCollectionProcessor(FilterResultCollectionProcessor original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new FilterResultCollectionProcessor(this, cloner);
    }
    #endregion

    public void Apply(IObservableDictionary<string, IItem> results) {
      var filters = FilterStringParameter.Value.Value.Split(';');
      
      
      /*
      foreach (var filter in FilterStringParameter.Value) {
        if (results.ContainsKey(filter.Value))
          results.Remove(filter.Value);
      }*/
    }
  }
}
