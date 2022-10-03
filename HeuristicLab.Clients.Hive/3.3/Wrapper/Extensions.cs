
using HEAL.Hive.SwaggerClient;

namespace HeuristicLab.Clients.Hive.Wrapper {
  public static class Extensions {
    public static HiveJobPermissionDTO toDto(this JobPermission permission) {
      return new HiveJobPermissionDTO {
        JobId = permission.JobId,
        GrantedUserId = permission.GrantedUserId,
        PermissionType = (PermissionType)(int)permission.Permission
      };
    }

    public static ProjectDTO toDto(this Project project) {
      return new ProjectDTO {
        Id = project.Id,
        Name = project.Name,
        Description = project.Description,
        OwnerId = project.OwnerUserId,
        ParentProjectId = project.ParentProjectId
      };
    }

    public static HiveJobDTO toDto(this Job job) {
      return new HiveJobDTO {
        Id = job.Id,
        Description = job.Description,
        HiveJobState = (HiveJobState)(int)job.State,
        Name = job.Name,
        OwnerId = job.OwnerUserId,
        ProjectId = job.ProjectId
      };
    }

    public static PluginDTO toDto(this Plugin plugin) {
      return new PluginDTO {
        Name = plugin.Name,
        OwnerId = plugin.UserId,
        Hash = plugin.Hash,
        Version = plugin.Version.ToString(),
      };
    }

    public static PluginDataDTO toDto(this PluginData pluginData) {
      return new PluginDataDTO {
        FileName = pluginData.FileName,
        PluginId = pluginData.PluginId,
        Data = pluginData.Data,
      };
    }

    public static HiveTaskDTO toDto(this Task task) {
      return new HiveTaskDTO {
        Id = task.Id,
        CoresNeeded = task.CoresNeeded,
        Command = task.Command.HasValue ? (HiveTaskCommand) ((int) task.Command + 1) : HiveTaskCommand.None,
        MemoryNeeded = task.MemoryNeeded,
        ParentTaskId = task.ParentTaskId,
        Priority = task.Priority,
        TaskState = (HiveTaskState) (int) task.State,
        HiveJobId = task.JobId
      };
    }

    public static HiveTaskDataDTO toDto(this TaskData taskData) {
      return new HiveTaskDataDTO {
        Data = taskData.Data,
        HiveTaskId = taskData.TaskId,
      };
    }

  }
}
