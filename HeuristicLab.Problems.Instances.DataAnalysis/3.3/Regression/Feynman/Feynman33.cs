using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman33 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman33() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman33(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman33(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format(
          "I.32.17 (1/2*epsilon*c*Ef**2)*(8*pi*r**2/3)*(omega**4/(omega**2-omega_0**2)**2) | {0}",
            noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "Pwr" : "Pwr_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"epsilon", "c", "Ef", "r", "omega", "omega_0", noiseRatio == null ? "Pwr" : "Pwr_noise"}; }
    }

    protected override string[] AllowedInputVariables {
      get { return new[] {"epsilon", "c", "Ef", "r", "omega", "omega_0"}; }
    }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data    = new List<List<double>>();
      var epsilon = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var c       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var Ef      = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var r       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var omega   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var omega_0 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 3, 5).ToList();

      var Pwr = new List<double>();

      data.Add(epsilon);
      data.Add(c);
      data.Add(Ef);
      data.Add(r);
      data.Add(omega);
      data.Add(omega_0);
      data.Add(Pwr);

      for (var i = 0; i < epsilon.Count; i++) {
        var res = 1.0 / 2 * epsilon[i] * c[i] * Math.Pow(Ef[i], 2) * (8 * Math.PI * Math.Pow(r[i], 2) / 3) *
                  (Math.Pow(omega[i], 4) / Math.Pow(Math.Pow(omega[i], 2) - Math.Pow(omega_0[i], 2), 2));
        Pwr.Add(res);
      }

      if (noiseRatio != null) {
        var Pwr_noise   = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * Pwr.StandardDeviationPop();
        Pwr_noise.AddRange(Pwr.Select(md => md + NormalDistributedRandomPolar.NextDouble(rand, 0, sigma_noise)));
        data.Remove(Pwr);
        data.Add(Pwr_noise);
      }

      return data;
    }
  }
}