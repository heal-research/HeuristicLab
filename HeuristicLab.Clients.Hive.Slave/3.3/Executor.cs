#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Threading;
using HeuristicLab.Clients.Hive.SlaveCore.Properties;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Hive;

namespace HeuristicLab.Clients.Hive.SlaveCore {
  /// <summary>
  /// The executor runs in the appdomain and handles the execution of an Hive task.
  /// </summary>
  public class Executor : MarshalByRefObject, IDisposable {
    private bool wasTaskAborted = false;
    private AutoResetEvent pauseStopSem = new AutoResetEvent(false);
    private AutoResetEvent startTaskSem = new AutoResetEvent(false); // block start method call
    private AutoResetEvent taskStartedSem = new AutoResetEvent(false); // make pause or stop wait until start is finished
    private ExecutorQueue executorQueue;
    private bool taskDataInvalid = false; // if true, the jobdata is not sent when the task is failed
    private ITask task;
    private DateTime creationTime;

    public Guid TaskId { get; set; }
    public int CoresNeeded { get; set; }
    public int MemoryNeeded { get; set; }
    public bool IsStopping { get; set; }
    public bool IsPausing { get; set; }

    public Exception CurrentException;
    public String CurrentExceptionStr {
      get {
        if (CurrentException != null) {
          return CurrentException.ToString();
        } else {
          return string.Empty;
        }
      }
    }

    public ExecutorQueue ExecutorCommandQueue {
      get { return executorQueue; }
    }

    private ExecutionState ExecutionState {
      get { return task != null ? task.ExecutionState : HeuristicLab.Core.ExecutionState.Stopped; }
    }

    public TimeSpan ExecutionTime {
      get { return task != null ? task.ExecutionTime : new TimeSpan(0, 0, 0); }
    }

    public Executor() {
      IsStopping = false;
      IsPausing = false;
      executorQueue = new ExecutorQueue();
    }

    public void Start(byte[] serializedJob) {
      try {
        creationTime = DateTime.Now;
        task = PersistenceUtil.Deserialize<ITask>(serializedJob);

        RegisterJobEvents();

        task.Start();
        if (!startTaskSem.WaitOne(Settings.Default.ExecutorSemTimeouts)) {
          taskDataInvalid = true;
          throw new TimeoutException("Timeout when starting the task. TaskStarted event was not fired.");
        }
      }
      catch (Exception e) {
        this.CurrentException = e;
        taskDataInvalid = true;
        Task_TaskFailed(this, new EventArgs<Exception>(e));
      } finally {
        taskStartedSem.Set();
      }
    }

    public void Pause() {
      IsPausing = true;
      // wait until task is started. if this does not happen, the Task is null an we give up
      taskStartedSem.WaitOne(Settings.Default.ExecutorSemTimeouts);
      if (task == null) {
        CurrentException = new Exception("Pausing task " + this.TaskId + ": Task is null");
        executorQueue.AddMessage(ExecutorMessageType.ExceptionOccured);
        return;
      }

      if (task.ExecutionState == ExecutionState.Started) {
        try {
          task.Pause();
          //we need to block the pause...
          pauseStopSem.WaitOne();
        }
        catch (Exception ex) {
          CurrentException = new Exception("Error pausing task " + this.TaskId + ": " + ex.ToString());
          executorQueue.AddMessage(ExecutorMessageType.ExceptionOccured);
        }
      }
    }

    public void Stop() {
      IsStopping = true;
      // wait until task is started. if this does not happen, the Task is null an we give up
      taskStartedSem.WaitOne(Settings.Default.ExecutorSemTimeouts);
      if (task == null) {
        CurrentException = new Exception("Stopping task " + this.TaskId + ": Task is null");
        executorQueue.AddMessage(ExecutorMessageType.ExceptionOccured);
      }
      wasTaskAborted = true;

      if ((ExecutionState == ExecutionState.Started) || (ExecutionState == ExecutionState.Paused)) {
        try {
          task.Stop();
          pauseStopSem.WaitOne();
        }
        catch (Exception ex) {
          CurrentException = new Exception("Error stopping task " + this.TaskId + ": " + ex.ToString());
          executorQueue.AddMessage(ExecutorMessageType.ExceptionOccured);
        }
      }
    }

    private void RegisterJobEvents() {
      task.TaskStopped += new EventHandler(Task_TaskStopped);
      task.TaskFailed += new EventHandler(Task_TaskFailed);
      task.TaskPaused += new EventHandler(Task_TaskPaused);
      task.TaskStarted += new EventHandler(Task_TaskStarted);
    }

    private void DeregisterJobEvents() {
      task.TaskStopped -= new EventHandler(Task_TaskStopped);
      task.TaskFailed -= new EventHandler(Task_TaskFailed);
      task.TaskPaused -= new EventHandler(Task_TaskPaused);
      task.TaskStarted -= new EventHandler(Task_TaskStarted);
    }

    #region Task Events
    private void Task_TaskFailed(object sender, EventArgs e) {
      IsStopping = true;
      EventArgs<Exception> ex = (EventArgs<Exception>)e;
      CurrentException = ex.Value;
      executorQueue.AddMessage(ExecutorMessageType.TaskFailed);
    }

    private void Task_TaskStopped(object sender, EventArgs e) {
      IsStopping = true;
      if (wasTaskAborted) {
        pauseStopSem.Set();
      }
      executorQueue.AddMessage(ExecutorMessageType.TaskStopped);
    }

    private void Task_TaskPaused(object sender, EventArgs e) {
      IsPausing = true;
      pauseStopSem.Set();
      executorQueue.AddMessage(ExecutorMessageType.TaskPaused);
    }

    private void Task_TaskStarted(object sender, EventArgs e) {
      startTaskSem.Set();
      executorQueue.AddMessage(ExecutorMessageType.TaskStarted);
    }
    #endregion

    public TaskData GetTaskData() {
      if (taskDataInvalid) return null;

      if (task != null && task.ExecutionState == ExecutionState.Started) {
        throw new InvalidStateException("Task is still running");
      } else {
        TaskData taskData = new TaskData();
        if (task == null) {
          //send empty task and save exception
          taskData.Data = PersistenceUtil.Serialize(new TaskData());
          if (CurrentException == null) {
            CurrentException = new Exception("Task with id " + this.TaskId + " is null, sending empty task");
          }
        } else {
          taskData.Data = PersistenceUtil.Serialize(task);
        }
        taskData.TaskId = TaskId;
        return taskData;
      }
    }

    public void Dispose() {
      if (task != null)
        DeregisterJobEvents();
      task = null;
    }
  }
}
