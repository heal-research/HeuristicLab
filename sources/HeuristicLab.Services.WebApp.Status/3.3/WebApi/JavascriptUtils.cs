using System;

namespace HeuristicLab.Services.WebApp.Status.WebApi {
  public static class JavascriptUtils {
    public static long ToTimestamp(DateTime input) {
      var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
      var time = input.Subtract(new TimeSpan(epoch.Ticks));
      return (long)(time.Ticks / 10000);
    }
  }
}
