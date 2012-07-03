using System.Globalization;
using System.Threading;

namespace SVM {
  internal static class TemporaryCulture {
    private static CultureInfo _culture;

    public static void Start() {
      _culture = Thread.CurrentThread.CurrentCulture;
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
    }

    public static void Stop() {
      Thread.CurrentThread.CurrentCulture = _culture;
    }
  }
}
