using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman53 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman53() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman53(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman53(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("II.3.24 Pwr/(4*pi*r**2) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "flux" : "flux_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"Pwr", "r", noiseRatio == null ? "flux" : "flux_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"Pwr", "r"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var Pwr  = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var r    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var flux = new List<double>();

      data.Add(Pwr);
      data.Add(r);
      data.Add(flux);

      for (var i = 0; i < Pwr.Count; i++) {
        var res = Pwr[i] / (4 * Math.PI * Math.Pow(r[i], 2));
        flux.Add(res);
      }

      if (noiseRatio != null) {
        var flux_noise  = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * flux.StandardDeviationPop();
        flux_noise.AddRange(flux.Select(md => md + NormalDistributedRandomPolar.NextDouble(rand, 0, sigma_noise)));
        data.Remove(flux);
        data.Add(flux_noise);
      }

      return data;
    }
  }
}