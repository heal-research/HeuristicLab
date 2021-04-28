using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman100 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman100() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman100(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman100(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("III.21.20 -rho_c_0*q*A_vec/m | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "j" : "j_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"rho_c_0", "q", "A_vec", "m", noiseRatio == null ? "j" : "j_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"rho_c_0", "q", "A_vec", "m"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data    = new List<List<double>>();
      var rho_c_0 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var q       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var A_vec   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var m       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var j = new List<double>();

      data.Add(rho_c_0);
      data.Add(q);
      data.Add(A_vec);
      data.Add(m);
      data.Add(j);

      for (var i = 0; i < rho_c_0.Count; i++) {
        var res = -rho_c_0[i] * q[i] * A_vec[i] / m[i];
        j.Add(res);
      }

      if (noiseRatio != null) {
        var j_noise     = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * j.StandardDeviationPop();
        j_noise.AddRange(j.Select(md => md + NormalDistributedRandomPolar.NextDouble(rand, 0, sigma_noise)));
        data.Remove(j);
        data.Add(j_noise);
      }

      return data;
    }
  }
}