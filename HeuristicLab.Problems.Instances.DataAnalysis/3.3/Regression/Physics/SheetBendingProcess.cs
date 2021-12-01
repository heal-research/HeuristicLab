using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public sealed class SheetBendingProcess : ArtificialRegressionDataDescriptor {
    public override string Name => "Sheet Bending Process F = E/(E+h) * ((σ_y * t^2) / w + (h * t^3) / (3 * R * w) - (4 * σ_y^3 * R^2) / (3 * w * E^2)) * (1 + 4 * t / w)";

    public override string Description => 
      "A full description of this instance is given in: " + Environment.NewLine +
      "Mohammad Zhian Asadzadeh, Hans-Peter Gänser, Manfred Mücke, " + Environment.NewLine +
      "\"Symbolic regression based hybrid semiparametric modelling of processes: " + Environment.NewLine +
      "An example case of a bending process\"," + Environment.NewLine +
      "Applications in Engineering Science, Volume 6, 2021, 100049, " + Environment.NewLine +
      "ISSN 2666-4968, https://doi.org/10.1016/j.apples.2021.100049. " + Environment.NewLine +
      "Function: F = E/(E+h) * ((σ_y * t^2) / w + (h * t^3) / (3 * R * w) - (4 * σ_y^3 * R^2) / (3 * w * E^2)) * (1 + 4 * t / w)" +
      "with E = 210000," + Environment.NewLine +
      "h ∈ [0.1, 5] [MPa]," + Environment.NewLine +
      "σ_y ∈ [250, 1000] [MPa]," + Environment.NewLine +
      "t ∈ [0.5, 5] [mm]," + Environment.NewLine +
      "R/t ∈ [1, 100]," + Environment.NewLine +
      "w/R ∈ [0.5, 1.5]";

   

    protected override string TargetVariable => "F";
    protected override string[] VariableNames => new string[] { "h", "sigma_y", "t", "Rt", "wR", "F" };
    protected override string[] AllowedInputVariables => new string[] { "h", "sigma_y", "t", "Rt", "wR" };

    protected override int TrainingPartitionStart => 0;
    protected override int TrainingPartitionEnd => 500;
    protected override int TestPartitionStart => 500;
    protected override int TestPartitionEnd => 1000;

    public int Seed { get; }

    public SheetBendingProcess() : this((int)DateTime.Now.Ticks) { }

    public SheetBendingProcess(int seed) {
      Seed = seed;
    }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint)Seed);

      List<double> h = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0.1, 5).ToList();
      List<double> sigmaY = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 250, 1000).ToList();
      List<double> t = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0.5, 5).ToList();
      List<double> Rt = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 100).ToList();
      List<double> wR = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0.5, 1.5).ToList();
      List<double> F = new List<double>();

      for (int i = TrainingPartitionStart; i < TestPartitionEnd; ++i) {
        F.Add(Calc(h[i], sigmaY[i], t[i], Rt[i], wR[i]));
      }

      return new List<List<double>> { h, sigmaY, t, Rt, wR, F };
    }

    private static double Calc(double h, double sigmaY, double t, double Rt, double wR) {
      var E = 210000;
      var R = Rt * t;
      var w = wR * R;
      var e = E / (E + h);
      var yieldStrength = (sigmaY * (t * t)) / w;
      var plasticHardening = (h * t * t * t) / (3 * R * w);
      var elasticity = (4 * sigmaY * sigmaY * sigmaY * R * R) / (3 * w * E * E);
      var C = 1 + 4 * (t / w);
      return e * (yieldStrength + plasticHardening - elasticity) * C;
    }
  }
}
