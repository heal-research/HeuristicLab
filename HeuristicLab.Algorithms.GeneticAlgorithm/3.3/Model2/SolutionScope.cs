using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization.Model2;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.GeneticAlgorithm.Model2 {
  [Item("GA Solution Scope", "")]
  [StorableClass]
  public class SolutionScope : Scope, ISolutionScope {

    [Storable]
    private Variable fitness;
    public double Fitness {
      get { return ((DoubleValue)fitness.Value).Value; }
      set { ((DoubleValue)fitness.Value).Value = value; }
    }

    [StorableConstructor]
    protected SolutionScope(bool deserializing) : base(deserializing) { }
    protected SolutionScope(SolutionScope original, Cloner cloner)
    : base(original, cloner) {
      fitness = cloner.Clone(original.fitness);
    }
    public SolutionScope(string fitnessName) : this(fitnessName, double.NaN) { }
    public SolutionScope(string fitnessName, double fitness) {
      this.fitness = new Variable(fitnessName, new DoubleValue(fitness));
      Variables.Add(this.fitness);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SolutionScope(this, cloner);
    }
  }
}
