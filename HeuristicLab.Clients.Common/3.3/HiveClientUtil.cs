using HeuristicLab.Clients.Common.Properties;

namespace HeuristicLab.Clients.Common {
  public class HiveClientUtil {

    public static int GetHiveVersion() {
      return Settings.Default.UseNewHive ? 2 : 1;
    }
    
  }
}
