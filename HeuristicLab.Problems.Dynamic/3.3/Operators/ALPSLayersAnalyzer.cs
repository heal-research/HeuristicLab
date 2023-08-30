using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.Dynamic.Operators {
  [Item("ALPS Single-objective basic Analyzer", "Calls the script's Analyze method to be able to write into the results collection.")]
  [StorableType("D5930EC6-BF72-4A0C-848C-F28FB767D5DE")]
  public class AlpsLayersAnalyzer : BasicSingleObjectiveAnalyzer /*, ILayerAnalyzer*/ {
    public string LayerParameterName = "Layer";
    public IValueLookupParameter<IntValue> LayerParameter => (IValueLookupParameter<IntValue>)Parameters[LayerParameterName];

    public int Layer => LayerParameter.ActualValue?.Value ?? -1;

    public Action<Individual[], double[], ResultCollection, ResultCollection, int, IRandom> AnalyzeFunc;

    public AlpsLayersAnalyzer() {
      Parameters.Add(new ValueLookupParameter<IntValue>(LayerParameterName, "speculative Lookup") { ActualName = LayerParameterName });
    }

    public AlpsLayersAnalyzer(AlpsLayersAnalyzer original, Cloner cloner) : base(original, cloner) { }


    [StorableConstructor]
    public AlpsLayersAnalyzer(StorableConstructorFlag _) : base(_) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlpsLayersAnalyzer(this, cloner);
    }

    public static string LayerResultName(int layerNumber) {
      return "layerResults" + layerNumber;
    }
    public override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      var layer = Layer;
      if (layer == -1) return;
      var rname = LayerResultName(layer);
      if (!results.ContainsKey(rname)) results.AddOrUpdateResult(rname, new ResultCollection());
      var res = (ResultCollection)results[rname].Value;
      AnalyzeFunc(individuals, qualities, results, res, layer, random);
    }
  }
}
