
namespace HeuristicLab.Problems.Instances {
  /// <summary>
  /// Describes instances of the Probabilistic Traveling Salesman Problem (PTSP).
  /// </summary>
  public class PTSPData : TSPData {
    /// <summary>
    /// The probabilities for each of the cities to appear in a route.
    /// </summary>
    public double[] Probabilities { get; set; }
  }
}
