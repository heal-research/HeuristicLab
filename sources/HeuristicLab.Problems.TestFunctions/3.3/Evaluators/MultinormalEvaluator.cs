using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using System.Diagnostics;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.TestFunctions.Evaluators {
  [Item("MultinormalFunction", "Evaluates a random multinormal function on a given point.")]
  [StorableClass]
  public class MultinormalEvaluator : SingleObjectiveTestFunctionProblemEvaluator {
    
    private ItemList<RealVector> centers {
      get { return (ItemList<RealVector>)Parameters["Centers"].ActualValue; }
      set { Parameters["Centers"].ActualValue = value; }
    }
    private RealVector s_2s {
      get { return (RealVector)Parameters["s^2s"].ActualValue; }
      set { Parameters["s^2s"].ActualValue = value; }
    }   

    [StorableConstructor]
    public MultinormalEvaluator(bool deserializing) { }
    
    public MultinormalEvaluator() {      
      Parameters.Add(new ValueParameter<ItemList<RealVector>>("Centers", "Centers of normal distributions"));
      Parameters.Add(new ValueParameter<RealVector>("s^2s", "sigma^2 of normal distributions"));
      centers = new ItemList<RealVector>() {
          new RealVector(new double[] { -5.0, -5.0 }),
          new RealVector(new double[] {  5.0, -5.0 }),          
          new RealVector(new double[] { -5.0,  5.0 }),
          new RealVector(new double[] {  5.0,  5.0 }),
        };
      s_2s = new RealVector(new double[] { 0.2, 1, 1, 2 });        
    }
    
    private double FastFindOptimum(out RealVector bestSolution) {
      var optima = centers.Select((c, i) => new { f = EvaluateFunction(c), i }).OrderBy(v => v.f).ToList();
      if (optima.Count == 0) {
        bestSolution = new RealVector();
        return 0;
      } else {
        var best = optima.First();
        bestSolution = centers[best.i];
        return best.f;
      }
    }

    public static double N(RealVector x, RealVector x0, double s_2) {
      Debug.Assert(x.Length == x0.Length);
      double d = 0;
      for (int i = 0; i < x.Length; i++) {
        d += (x[i] - x0[i]) * (x[i] - x0[i]);
      }
      return Math.Exp(-d / (2 * s_2)) / (2 * Math.PI * s_2);
    }

    public override bool Maximization {
      get { return false; }
    }

    public override DoubleMatrix Bounds {
      get { return new DoubleMatrix(new double[,] { { -10, 10 } }); }
    }

    public override double BestKnownQuality {
      get {
        RealVector bestSolution;
        return FastFindOptimum(out bestSolution);
      }
    }

    public override int MinimumProblemSize { get { return 1; } }

    public override int MaximumProblemSize { get { return 1000; } }

    private RealVector Shorten(RealVector x, int dimensions) {
      return new RealVector(x.Take(dimensions).ToArray());      
    }

    public override RealVector GetBestKnownSolution(int dimension) {
      RealVector bestSolution;
      FastFindOptimum(out bestSolution);
      return Shorten(bestSolution, dimension);
    }

    public double Evaluate(RealVector point) {
      return EvaluateFunction(point);
    }

    protected override double EvaluateFunction(RealVector point) {
      double value = 0;
      for (int i = 0; i < centers.Count; i++) {        
        value -= N(point, Shorten(centers[i], point.Length), s_2s[i]);
      }
      return value;
    }
  }
}
