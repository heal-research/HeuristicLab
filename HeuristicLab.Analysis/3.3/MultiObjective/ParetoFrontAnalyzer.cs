using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  [Item("ParetoFrontAnalyzer", "Analyzer for multiobjective problems that collects and presents the current Pareto front as double matrix as well as the solution scopes that lie on the current front.")]
  [StorableClass]
  public abstract class ParetoFrontAnalyzer : SingleSuccessorOperator, IAnalyzer {
    public virtual bool EnabledByDefault {
      get { return true; }
    }

    public IScopeTreeLookupParameter<DoubleArray> QualitiesParameter {
      get { return (IScopeTreeLookupParameter<DoubleArray>)Parameters["Qualities"]; }
    }
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    [StorableConstructor]
    protected ParetoFrontAnalyzer(bool deserializing) : base(deserializing) { }
    protected ParetoFrontAnalyzer(ParetoFrontAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public ParetoFrontAnalyzer() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleArray>("Qualities", "The vector of qualities of each solution."));
      Parameters.Add(new LookupParameter<ResultCollection>("Results", "The result collection to store the front to."));
    }

    public override IOperation Apply() {
      ItemArray<DoubleArray> qualities = QualitiesParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;
      Analyze(qualities, results);
      return base.Apply();
    }

    protected abstract void Analyze(ItemArray<DoubleArray> qualities, ResultCollection results);
  }
}
