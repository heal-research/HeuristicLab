using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.LinearLinkageEncoding.Creators {
  [Item("Max Group-size Linear Linkage Creator", "Creates a random linear linkage LLE encoded solution with a given maximum number of items per group.")]
  [StorableClass]
  public class MaxGroupSizeLinearLinkageCreator: LinearLinkageCreator {

    public IValueLookupParameter<IntValue> MaxGroupSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaxGroupSize"]; }
    }

    public IntValue MaxGroupSize {
      get { return MaxGroupSizeParameter.Value; }
      set { MaxGroupSizeParameter.Value = value; }
    }

    [StorableConstructor]
    private MaxGroupSizeLinearLinkageCreator(bool deserializing) : base(deserializing) { }
    private MaxGroupSizeLinearLinkageCreator(MaxGroupSizeLinearLinkageCreator original, Cloner cloner) : base(original, cloner) { }

    public MaxGroupSizeLinearLinkageCreator() {
      Parameters.Add(new ValueLookupParameter<IntValue>("MaxGroupSize", "The maximum number items in every group.", new IntValue(4)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MaxGroupSizeLinearLinkageCreator(this, cloner);
    }

    public static LinearLinkage Apply(IRandom random, int length, int maxGroupSize) {
      var groups = Enumerable.Range(0, length).Select(x => new { Item = x, Group = random.Next(length)})
        .GroupBy(x => x.Group)
        .SelectMany(x => 
           x.Select((y, i) => new {SubGroup = i%maxGroupSize, y.Item})
            .GroupBy(y=>y.SubGroup)
            .Select(y=>
              y.Select(z=>z.Item).ToList()));
      return LinearLinkage.FromGroups(length, groups);
    }

    protected override LinearLinkage Create(IRandom random, int length) {
      var maxGroupSize = MaxGroupSizeParameter.ActualValue.Value;
      return Apply(random, length, maxGroupSize);
    }
  }
}
