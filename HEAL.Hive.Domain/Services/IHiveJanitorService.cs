using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Services {
  public interface IHiveJanitorService {

    Task CleanUpAsync();

  }
}
