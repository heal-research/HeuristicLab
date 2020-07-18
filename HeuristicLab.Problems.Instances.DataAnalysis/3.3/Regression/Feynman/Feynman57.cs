using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman57 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman57() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman57(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman57(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format(
          "II.6.15b 3/(4*pi*epsilon)*p_d/r**3*cos(theta)*sin(theta) | {0} samples | {1}",
          trainingSamples, noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "Ef" : "Ef_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"epsilon", "p_d", "theta", "r", noiseRatio == null ? "Ef" : "Ef_noise"}; }
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

      var Ef = new List<double>();

      data.Add(epsilon);
      data.Add(p_d);
      data.Add(theta);
      data.Add(r);
      data.Add(Ef);

      for (var i = 0; i < epsilon.Count; i++) {
        var res = 3.0 / (4 * Math.PI * epsilon[i]) * p_d[i] / Math.Pow(r[i], 3) * Math.Cos(theta[i]) * Math.Sin(theta[i]);
        Ef.Add(res);
      }

      if (noiseRatio != null) {
        var Ef_noise    = new List<double>();
        var sigma_noise = (double) noiseRatio * Ef.StandardDeviationPop();
        Ef_noise.AddRange(Ef.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(Ef);
        data.Add(Ef_noise);
      }

      return data;
    }
  }
}