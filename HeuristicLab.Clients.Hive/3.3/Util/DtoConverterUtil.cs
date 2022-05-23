using HEAL.Hive.Domain.DTOs.Entities;

namespace HeuristicLab.Clients.Hive.Util {
  public static class DtoConverterUtil {

    public static PluginDTO convertToNewDto(Plugin plugin) {
      return new PluginDTO {
        Hash = plugin.Hash,
        Name = plugin.Name,
        Version = plugin.Version
      };
    }

    public static PluginDataDTO convertToNewDto(PluginData pluginData) {
      return new PluginDataDTO {
        FileName = pluginData.FileName,
        PluginId = pluginData.PluginId,
        Data = pluginData.Data
      };
    }

    public static HiveJobDTO convertToNewDto(Job job) {
      return new HiveJobDTO {
        ProjectId = job.ProjectId,
        Name = job.Name,
        Description = job.Description,
        OwnerId = job.OwnerUserId
      };
    }

    public static Plugin convertFromNewDto(PluginDTO plugin) {
      return new Plugin {
        Id = plugin.Id,
        DateCreated = plugin.CreatedAt,
        Hash = plugin.Hash,
        Name = plugin.Name,
        Version = plugin.Version
      };
    }

    public static HiveTaskDTO convertToNewDto(Task task) {
      return new HiveTaskDTO {
        FinishWhenChildJobsFinished = task.FinishWhenChildJobsFinished,
        HiveJobId = task.JobId,
        MemoryNeeded = task.MemoryNeeded,
        CoresNeeded = task.CoresNeeded,
        ParentTaskId = task.ParentTaskId,
        RequiredPlugins = task.PluginsNeededIds,
        TaskState = HEAL.Hive.Domain.Enums.HiveTaskState.Waiting
      };
    }

    public static HiveTaskDataDTO convertToNewDto(TaskData taskdata) {
      return new HiveTaskDataDTO {
        HiveTaskId = taskdata.TaskId,
        Data = taskdata.Data
      };
    }
  }
}
