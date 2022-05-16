using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Services.Statistics {
  public interface IStatisticsGeneratorService {

    Task CalculateStatisticsAsync();

  }
}
