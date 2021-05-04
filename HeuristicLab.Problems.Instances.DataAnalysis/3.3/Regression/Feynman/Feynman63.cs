using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman63 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman63() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman63(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman63(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("II.11.20 n_rho*p_d**2*Ef/(3*kb*T) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "Pol" : "Pol_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "n_rho", "p_d", "Ef", "kb", "T", "Pol" } : new[] { "n_rho", "p_d", "Ef", "kb", "T", "Pol", "Pol_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"n_rho", "p_d", "Ef", "kb", "T"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var n_rho = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var p_d   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var Ef    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var kb    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var T     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var Pol = new List<double>();

      data.Add(n_rho);
      data.Add(p_d);
      data.Add(Ef);
      data.Add(kb);
      data.Add(T);
      data.Add(Pol);

      for (var i = 0; i < n_rho.Count; i++) {
        var res = n_rho[i] * Math.Pow(p_d[i], 2) * Ef[i] / (3 * kb[i] * T[i]);
        Pol.Add(res);
      }

      var targetNoise = GetNoisyTarget(Pol, rand);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}