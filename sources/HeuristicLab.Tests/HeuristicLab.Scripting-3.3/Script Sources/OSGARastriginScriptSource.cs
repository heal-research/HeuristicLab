using System;
using System.Linq;

public class OSGARastriginScript : HeuristicLab.Scripting.CSharpScriptBase {
  public override void Main() {
    int N = vars.N.Value;
    double minX = vars.minX.Value;
    double maxX = vars.maxX.Value;
    int seed = vars.seed.Value;
    var rand = new Random(seed);

    var result = OSGA(rand, popSize: 1000, iterations: 5000, maxSelPres: 1000, mutationRate: 0.1,
         obj: Rastrigin,
      // x ~(i.i.d) U(0,1) * 10.24 - 5.12
         creator: () => Enumerable.Range(1, N).Select(_ => rand.NextDouble() * (maxX - minX) + minX).ToArray(),
      // random parent selection
         selector: (f) => rand.Next(f.Length),
      // single point crossover
         crossover: (p0, p1, cand) => {
           int cut = rand.Next(cand.Length);
           Array.Copy(p0, cand, cut);
           Array.Copy(p1, cut, cand, cut, cand.Length - cut);
         },
      // single point manipulation
         manipulator: (p) => p[rand.Next(N)] = rand.NextDouble() * (maxX - minX) + minX,
         output: true
      );

    double bestFit = result.Item2;
    vars.bestFitness = bestFit;
    Console.WriteLine("best fitness: {0}", bestFit);
  }

  private double Rastrigin(double[] x) {
    return 10.0 * x.Length + x.Sum(xi => xi * xi - 10.0 * Math.Cos(2.0 * Math.PI * xi));
  }

  private Tuple<T, double> OSGA<T>(Random rand, int popSize, int iterations, double maxSelPres, double mutationRate, Func<T, double> obj,
      Func<T> creator, Func<double[], int> selector, Action<T, T, T> crossover, Action<T> manipulator,
      bool output = false)
      where T : ICloneable {
    // generate random pop
    T[] pop = Enumerable.Range(0, popSize).Select(_ => creator()).ToArray();
    // evaluate initial pop
    double[] fit = pop.Select(p => obj(p)).ToArray();

    // arrays for next pop (clone current pop because solutions are reused)
    T[] popNew = pop.Select(pi => (T)pi.Clone()).ToArray();
    double[] fitNew = new double[popSize];

    var bestSolution = (T)pop.First().Clone(); // take a random solution (don't care)
    double bestFit = fit.First();

    // run generations
    double curSelPres = 0;
    for (int g = 0; g < iterations && curSelPres < maxSelPres; g++) {
      // keep the first element as elite
      int i = 1;
      int genEvals = 0;
      do {
        var p1Idx = selector(fit);
        var p2Idx = selector(fit);

        var p1 = pop[p1Idx];
        var p2 = pop[p2Idx];
        var f1 = fit[p1Idx];
        var f2 = fit[p2Idx];

        // generate candidate solution (reuse old solutions)
        crossover(p1, p2, popNew[i]);
        // optional mutation
        if (rand.NextDouble() < mutationRate) {
          manipulator(popNew[i]);
        }

        double f = obj(popNew[i]);
        genEvals++;
        // if child is better than best (strict offspring selection)
        if (f < Math.Min(f1, f2)) {
          // update best fitness
          if (f < bestFit) {
            bestFit = f;
            bestSolution = (T)popNew[i].Clone(); // overall best
          }

          // keep
          fitNew[i] = f;
          i++;
        }

        curSelPres = genEvals / (double)popSize;
      } while (i < popNew.Length && curSelPres < maxSelPres);

      Console.WriteLine("generation {0} obj {1:0.000} sel. pres. {2:###.0}", g, bestFit, curSelPres);

      // swap 
      var tmpPop = pop;
      var tmpFit = fit;
      pop = popNew;
      fit = fitNew;
      popNew = tmpPop;
      fitNew = tmpFit;

      // keep elite
      popNew[0] = (T)bestSolution.Clone();
      fitNew[0] = bestFit;
    }

    return Tuple.Create(bestSolution, bestFit);
  }
}