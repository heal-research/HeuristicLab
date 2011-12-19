#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Threading;
using HeuristicLab.Clients.Hive.SlaveCore.Properties;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.PluginInfrastructure.Sandboxing;

namespace HeuristicLab.Clients.Hive.SlaveCore {

  /// <summary>
  ///  Manages a single task and it's appdomain. 
  /// </summary>
  public class SlaveTask : MarshalByRefObject {
    private Executor executor;
    private AppDomain appDomain;
    private Semaphore waitForStartBeforeKillSem;
    private bool executorMonitoringRun;
    private Thread executorMonitoringThread;
    private PluginManager pluginManager;
    private ILog log;
    public Guid TaskId { get; private set; }
    public bool IsPrepared { get; private set; }

    private int coresNeeded;
    public int CoresNeeded {
      get { return coresNeeded; }
      set { this.coresNeeded = value; }
    }

    public TimeSpan ExecutionTime {
      get {
        try {
          return executor != null ? executor.ExecutionTime : TimeSpan.Zero;
        }
        catch (Exception ex) {
          EventLogManager.LogException(ex);
          return TimeSpan.Zero;
        }
      }
    }

    public SlaveTask(PluginManager pluginManager, int coresNeeded, ILog log) {
      this.pluginManager = pluginManager;
      this.coresNeeded = coresNeeded;
      this.log = log;
      waitForStartBeforeKillSem = new Semaphore(0, 1);
      executorMonitoringRun = true;
      IsPrepared = false;
    }

    public void StartJobAsync(Task task, TaskData taskData) {
      try {
        this.TaskId = task.Id;
        Prepare(task);
        StartTaskInAppDomain(taskData);
      }
      catch (Exception) {
        // make sure to clean up if something went wrong
        DisposeAppDomain();
        throw;
      }
    }

    public void PauseTask() {
      if (!IsPrepared) throw new AppDomainNotCreatedException();
      if (!executor.IsPausing && !executor.IsStopping) executor.Pause();
    }

    public void StopTask() {
      if (!IsPrepared) throw new AppDomainNotCreatedException();
      if (!executor.IsPausing && !executor.IsStopping) executor.Stop();
    }

    private void Prepare(Task task) {
      string pluginDir = Path.Combine(pluginManager.PluginTempBaseDir, task.Id.ToString());
      string configFileName;
      pluginManager.PreparePlugins(task, out configFileName);
      appDomain = CreateAppDomain(task, pluginDir, configFileName);
      IsPrepared = true;
    }

    private AppDomain CreateAppDomain(Task task, String pluginDir, string configFileName) {
      if (task.IsPrivileged) {
        appDomain = SandboxManager.CreateAndInitPrivilegedSandbox(task.Id.ToString(), pluginDir, Path.Combine(pluginDir, configFileName));
      } else {
        appDomain = SandboxManager.CreateAndInitSandbox(task.Id.ToString(), pluginDir, Path.Combine(pluginDir, configFileName));
      }
      appDomain.UnhandledException += new UnhandledExceptionEventHandler(AppDomain_UnhandledException);

      log.LogMessage("Creating AppDomain");
      executor = (Executor)appDomain.CreateInstanceAndUnwrap(typeof(Executor).Assembly.GetName().Name, typeof(Executor).FullName);

      executor.TaskId = task.Id;
      executor.CoresNeeded = task.CoresNeeded;
      executor.MemoryNeeded = task.MemoryNeeded;
      return appDomain;
    }

    private void StartTaskInAppDomain(TaskData taskData) {
      executor.Start(taskData.Data);
      waitForStartBeforeKillSem.Release();
      StartExecutorMonitoringThread();
    }

    public void DisposeAppDomain() {
      log.LogMessage(string.Format("Shutting down Appdomain for Task {0}", TaskId));
      StopExecutorMonitoringThread();

      if (executor != null) {
        try {
          executor.Dispose();
        }
        catch (Exception ex) {
          EventLogManager.LogException(ex);
        }
      }

      if (appDomain != null) {
        appDomain.UnhandledException -= new UnhandledExceptionEventHandler(AppDomain_UnhandledException);
        int repeat = Settings.Default.PluginDeletionRetries;
        while (repeat > 0) {
          try {
            waitForStartBeforeKillSem.WaitOne();
            AppDomain.Unload(appDomain);
            waitForStartBeforeKillSem.Dispose();
            repeat = 0;
          }
          catch (CannotUnloadAppDomainException) {
            log.LogMessage("Could not unload AppDomain, will try again in 1 sec.");
            Thread.Sleep(Settings.Default.PluginDeletionTimeout);
            repeat--;
            if (repeat == 0) {
              throw; // rethrow and let app crash
            }
          }
        }
      }
      pluginManager.DeletePluginsForJob(TaskId);
      GC.Collect();
    }

    private void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
      DisposeAppDomain();
      OnExceptionOccured(new Exception("Unhandled exception: " + e.ExceptionObject.ToString()));
    }

    public TaskData GetTaskData() {
      TaskData data = null;
      try {
        data = executor.GetTaskData();
      }
      catch (Exception ex) {
        EventLogManager.LogException(ex);
      }
      return data;
    }

    #region ExecutorMonitorThread
    private void StartExecutorMonitoringThread() {
      executorMonitoringThread = new Thread(MonitorExecutor);
      executorMonitoringThread.Start();
    }

    private void StopExecutorMonitoringThread() {
      if (executorMonitoringThread != null) {
        if (executorMonitoringRun) {
          executorMonitoringRun = false;
          executor.ExecutorCommandQueue.AddMessage(ExecutorMessageType.StopExecutorMonitoringThread);
        }
      }
    }

    /// <summary>
    /// Because the executor is in an appdomain and is not able to call back 
    /// (because of security -> lease time for marshall-by-ref object is 5 min),
    /// we have to poll the executor for events we have to react to (e.g. task finished...)    
    /// </summary>
    private void MonitorExecutor() {
      while (executorMonitoringRun) {
        //this call goes through the appdomain border. We have to 
        //poll so that the the lease gets renewed
        ExecutorMessage message;
        do {
          message = executor.ExecutorCommandQueue.GetMessage();
        } while (message == null);

        switch (message.MessageType) {
          case ExecutorMessageType.TaskStarted:
            OnTaskStarted();
            break;

          case ExecutorMessageType.TaskPaused:
            executorMonitoringRun = false;
            OnTaskPaused();
            DisposeAppDomain();
            break;

          case ExecutorMessageType.TaskStopped:
            executorMonitoringRun = false;
            OnTaskStopped();
            DisposeAppDomain();
            break;

          case ExecutorMessageType.TaskFailed:
            executorMonitoringRun = false;
            OnTaskFailed(new TaskFailedException(executor.CurrentExceptionStr));
            DisposeAppDomain();
            break;

          case ExecutorMessageType.StopExecutorMonitoringThread:
            executorMonitoringRun = false;
            break;

          case ExecutorMessageType.ExceptionOccured:
            executorMonitoringRun = false;
            DisposeAppDomain();
            if (executor.CurrentException != null) {
              OnExceptionOccured(executor.CurrentException);
            } else {
              OnExceptionOccured(new Exception(string.Format("Unknow exception occured in Executor for task {0}", TaskId)));
            }
            break;
        }
      }
    }
    #endregion

    public event EventHandler<EventArgs<Guid>> TaskStarted;
    private void OnTaskStarted() {
      var handler = TaskStarted;
      if (handler != null) handler(this, new EventArgs<Guid>(this.TaskId));
    }

    public event EventHandler<EventArgs<Guid>> TaskStopped;
    private void OnTaskStopped() {
      var handler = TaskStopped;
      if (handler != null) handler(this, new EventArgs<Guid>(this.TaskId));
    }

    public event EventHandler<EventArgs<Guid>> TaskPaused;
    private void OnTaskPaused() {
      var handler = TaskPaused;
      if (handler != null) handler(this, new EventArgs<Guid>(this.TaskId));
    }

    public event EventHandler<EventArgs<Guid>> TaskAborted;
    private void OnTaskAborted() {
      var handler = TaskAborted;
      if (handler != null) handler(this, new EventArgs<Guid>(this.TaskId));
    }

    public event EventHandler<EventArgs<Guid, Exception>> TaskFailed;
    private void OnTaskFailed(Exception exception) {
      var handler = TaskFailed;
      if (handler != null) handler(this, new EventArgs<Guid, Exception>(this.TaskId, exception));
    }

    public event EventHandler<EventArgs<Guid, Exception>> ExceptionOccured;
    private void OnExceptionOccured(Exception exception) {
      var handler = ExceptionOccured;
      if (handler != null) handler(this, new EventArgs<Guid, Exception>(this.TaskId, exception));
    }
  }
}
