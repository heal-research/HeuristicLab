namespace HeuristicLab.Algorithms.DataAnalysis {
  public static class DoubleArrayExtensions {
    public static bool ContainsNanInf(this double[,] array) {
      foreach (var entry in array) {
        if (double.IsNaN(entry) || double.IsInfinity(entry)) {
          return true;
        }
      }
      return false;
    }
  }
}
