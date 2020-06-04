using System;
using System.Collections.Generic;
using HeuristicLab.Services.Hive.DataAccess;
using HeuristicLab.Services.Hive.DataAccess.Interfaces;


namespace HeuristicLab.Services.Hive {
  public abstract class TaskScheduler : ITaskScheduler {
    protected IPersistenceManager PersistenceManager => ServiceLocator.Instance.PersistenceManager;

    public IEnumerable<Guid> Schedule(Slave slave, int count = 1) {
      return PersistenceManager.UseTransactionAndSubmit(() => {
        var ids = ScheduleInternal(slave, count);
        foreach (var id in ids) AssignTask(slave, id);
        return ids;
      });
    }

    protected abstract IReadOnlyList<Guid> ScheduleInternal(Slave slave, int count);

    private void AssignTask(Slave slave, Guid taskId) {
      const TaskState transferring = TaskState.Transferring;
      DateTime now = DateTime.Now;

      var pm = PersistenceManager;
      var taskDao = pm.TaskDao;
      var stateLogDao = pm.StateLogDao;

      var task = taskDao.GetById(taskId);
      task.State = transferring;
      task.LastHeartbeat = now;

      stateLogDao.Save(new StateLog {
        State = transferring,
        DateTime = now,
        TaskId = taskId,
        SlaveId = slave.ResourceId,
        UserId = null,
        Exception = null
      });
    }
  }
}
