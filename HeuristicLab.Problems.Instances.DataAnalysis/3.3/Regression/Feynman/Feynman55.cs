using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman55 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman55() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman55(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman55(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("II.6.11 1/(4*pi*epsilon)*p_d*cos(theta)/r**2 | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "Volt" : "Volt_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"epsilon", "p_d", "theta", "r", noiseRatio == null ? "Volt" : "Volt_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"epsilon", "p_d", "theta", "r"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data    = new List<List<double>>();
      var epsilon = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var p_d     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var theta   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var r       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();

      var Volt = new List<double>();

      data.Add(epsilon);
      data.Add(p_d);
      data.Add(theta);
      data.Add(r);
      data.Add(Volt);

      for (var i = 0; i < epsilon.Count; i++) {
        var res = 1.0 / (4 * Math.PI * epsilon[i]) * p_d[i] * Math.Cos(theta[i]) / Math.Pow(r[i], 2);
        Volt.Add(res);
      }

      if (noiseRatio != null) {
        var Volt_noise  = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * Volt.StandardDeviationPop();
        Volt_noise.AddRange(Volt.Select(md => md + NormalDistributedRandomPolar.NextDouble(rand, 0, sigma_noise)));
        data.Remove(Volt);
        data.Add(Volt_noise);
      }

      return data;
    }
  }
}