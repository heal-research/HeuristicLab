#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.MainForm;

namespace HeuristicLab.Clients.Hive {
  public class RefreshableJob : IHiveItem, IDeepCloneable, IContent, IComparable<RefreshableJob> {
    private JobResultPoller jobResultPoller;
    private ConcurrentTaskDownloader<ItemTask> jobDownloader;
    private object locker = new object();
    private object downloadFinishedLocker = new object();

    public bool IsProgressing { get; set; }

    private Job job;
    public Job Job {
      get { return job; }
      set {
        if (value != job) {
          if (value == null)
            throw new ArgumentNullException();

          if (job != null) DergisterJobEvents();
          job = value;
          if (job != null) {
            RegisterJobEvents();
            job_PropertyChanged(job, new PropertyChangedEventArgs("Id"));
          }
          OnJobChanged();
          OnToStringChanged(this, EventArgs.Empty);
          job_ItemImageChanged(this, EventArgs.Empty);
        }
      }
    }

    private ItemCollection<HiveTask> hiveTasks;
    public ItemCollection<HiveTask> HiveTasks {
      get { return hiveTasks; }
      set {
        if (hiveTasks != value) {
          if (hiveTasks != null) DeregisterHiveJobsEvents();
          hiveTasks = value;
          if (hiveTasks != null) RegisterHiveJobsEvents();
          OnHiveTasksChanged();
        }
      }
    }

    private ExecutionState executionState;
    public ExecutionState ExecutionState {
      get { return executionState; }
      internal set {
        if (executionState != value) {
          executionState = value;
          OnExecutionStateChanged();
        }
      }
    }

    private TimeSpan executionTime;
    public TimeSpan ExecutionTime {
      get { return executionTime; }
      internal set {
        if (executionTime != value) {
          executionTime = value;
          OnExecutionTimeChanged();
        }
      }
    }

    private bool refreshAutomatically;
    public bool RefreshAutomatically {
      get { return refreshAutomatically; }
      set {
        lock (locker) {
          if (refreshAutomatically != value) {
            refreshAutomatically = value;
            OnRefreshAutomaticallyChanged();
          }
          if (RefreshAutomatically) {
            if (this.HiveTasks != null && this.HiveTasks.Count > 0 && (jobResultPoller == null || !jobResultPoller.IsPolling)) {
              StartResultPolling();
            }
          } else {
            StopResultPolling();
          }
        }
      }
    }

    // indicates if download button is enabled
    private bool isDownloadable = true;
    public bool IsDownloadable {
      get { return isDownloadable; }
      set {
        if (value != isDownloadable) {
          isDownloadable = value;
          OnIsDownloadableChanged();
        }
      }
    }

    // if true, all control buttons should be enabled. otherwise disabled
    private bool isControllable = true;
    public bool IsControllable {
      get { return isControllable; }
      private set {
        if (value != isControllable) {
          isControllable = value;
          OnIsControllableChanged();
          if (this.hiveTasks != null) {
            foreach (var hiveJob in this.hiveTasks) {
              hiveJob.IsControllable = value;
            }
          }
        }
      }
    }

    // indicates if a user is allowed to share this experiment
    private bool isSharable = true;
    public bool IsSharable {
      get { return isSharable; }
      private set {
        if (value != isSharable) {
          isSharable = value;
          OnIsSharableChanged();
        }
      }
    }

    // may execute jobs with privileged permissions on slaves
    private bool isAllowedPrivileged = true;
    public bool IsAllowedPrivileged {
      get { return isAllowedPrivileged; }
      set {
        if (value != isAllowedPrivileged) {
          isAllowedPrivileged = value;
          OnIsAllowedPrivilegedChanged();
        }
      }
    }

    private Progress progress;
    public Progress Progress {
      get { return progress; }
      set {
        this.progress = value;
        OnIsProgressingChanged();
      }
    }


    private ThreadSafeLog log;
    public ILog Log {
      get { return log; }
    }

    public StateLogListList StateLogList {
      get { return new StateLogListList(this.GetAllHiveTasks().Select(x => x.StateLog)); }
    }

    #region Constructors and Cloning
    public RefreshableJob() {
      this.refreshAutomatically = false;
      this.Job = new Job();
      this.log = new ThreadSafeLog();
      this.jobDownloader = new ConcurrentTaskDownloader<ItemTask>(Settings.Default.MaxParallelDownloads, Settings.Default.MaxParallelDownloads);
      this.jobDownloader.ExceptionOccured += new EventHandler<EventArgs<Exception>>(jobDownloader_ExceptionOccured);
      this.HiveTasks = new ItemCollection<HiveTask>();
    }
    public RefreshableJob(Job hiveExperiment) {
      this.refreshAutomatically = true;
      this.Job = hiveExperiment;
      this.log = new ThreadSafeLog();
      this.jobDownloader = new ConcurrentTaskDownloader<ItemTask>(Settings.Default.MaxParallelDownloads, Settings.Default.MaxParallelDownloads);
      this.jobDownloader.ExceptionOccured += new EventHandler<EventArgs<Exception>>(jobDownloader_ExceptionOccured);
      this.HiveTasks = new ItemCollection<HiveTask>();
    }
    protected RefreshableJob(RefreshableJob original, Cloner cloner) {
      cloner.RegisterClonedObject(original, this);
      this.Job = cloner.Clone(original.Job);
      this.IsControllable = original.IsControllable;
      this.log = cloner.Clone(original.log);
      this.RefreshAutomatically = false; // do not start results polling automatically
      this.jobDownloader = new ConcurrentTaskDownloader<ItemTask>(Settings.Default.MaxParallelDownloads, Settings.Default.MaxParallelDownloads);
      this.jobDownloader.ExceptionOccured += new EventHandler<EventArgs<Exception>>(jobDownloader_ExceptionOccured);
      this.HiveTasks = cloner.Clone(original.HiveTasks);
      this.ExecutionTime = original.ExecutionTime;
      this.ExecutionState = original.ExecutionState;
    }
    public IDeepCloneable Clone(Cloner cloner) {
      return new RefreshableJob(this, cloner);
    }
    public object Clone() {
      return this.Clone(new Cloner());
    }
    #endregion

    #region JobResultPoller Events
    public void StartResultPolling() {
      if (jobResultPoller == null) {
        jobResultPoller = new JobResultPoller(job.Id, Settings.Default.ResultPollingInterval);
        RegisterResultPollingEvents();
        jobResultPoller.AutoResumeOnException = false;
      }

      if (!jobResultPoller.IsPolling) {
        jobResultPoller.Start();
      }
    }

    public void StopResultPolling() {
      if (jobResultPoller != null && jobResultPoller.IsPolling) {
        jobResultPoller.Stop();
      }
    }

    private void RegisterResultPollingEvents() {
      jobResultPoller.ExceptionOccured += new EventHandler<EventArgs<Exception>>(jobResultPoller_ExceptionOccured);
      jobResultPoller.JobResultsReceived += new EventHandler<EventArgs<IEnumerable<LightweightTask>>>(jobResultPoller_JobResultReceived);
      jobResultPoller.IsPollingChanged += new EventHandler(jobResultPoller_IsPollingChanged);
    }
    private void DeregisterResultPollingEvents() {
      jobResultPoller.ExceptionOccured -= new EventHandler<EventArgs<Exception>>(jobResultPoller_ExceptionOccured);
      jobResultPoller.JobResultsReceived -= new EventHandler<EventArgs<IEnumerable<LightweightTask>>>(jobResultPoller_JobResultReceived);
      jobResultPoller.IsPollingChanged -= new EventHandler(jobResultPoller_IsPollingChanged);
    }
    private void jobResultPoller_IsPollingChanged(object sender, EventArgs e) {
      if (this.refreshAutomatically != jobResultPoller.IsPolling) {
        this.refreshAutomatically = jobResultPoller.IsPolling;
        OnRefreshAutomaticallyChanged();
      }
    }

    private void jobResultPoller_JobResultReceived(object sender, EventArgs<IEnumerable<LightweightTask>> e) {
      foreach (LightweightTask lightweightTask in e.Value) {
        HiveTask hiveTask = GetHiveTaskById(lightweightTask.Id);
        if (hiveTask != null) {
          // lastJobDataUpdate equals DateTime.MinValue right after it was uploaded. When the first results are polled, this value is updated
          if (hiveTask.Task.State == TaskState.Offline && lightweightTask.State != TaskState.Finished && lightweightTask.State != TaskState.Failed && lightweightTask.State != TaskState.Aborted) {
            hiveTask.Task.LastTaskDataUpdate = lightweightTask.LastTaskDataUpdate;
          }

          hiveTask.UpdateFromLightweightJob(lightweightTask);

          if (!hiveTask.IsFinishedTaskDownloaded && !hiveTask.IsDownloading && hiveTask.Task.LastTaskDataUpdate < lightweightTask.LastTaskDataUpdate) {
            log.LogMessage(string.Format("Downloading task {0}", lightweightTask.Id));
            hiveTask.IsDownloading = true;
            jobDownloader.DownloadTaskData(hiveTask.Task, (localJob, itemJob) => {
              lock (downloadFinishedLocker) {
                log.LogMessage(string.Format("Finished downloading task {0}", localJob.Id));
                HiveTask localHiveTask = GetHiveTaskById(localJob.Id);

                if (itemJob == null) {
                  // something bad happened to this task. bad task, BAAAD task!
                  localHiveTask.IsDownloading = false;
                } else {
                  // if the task is paused, download but don't integrate into parent optimizer (to avoid Prepare) 
                  if (localJob.State == TaskState.Paused) {
                    localHiveTask.ItemTask = itemJob;
                  } else {
                    if (localJob.ParentTaskId.HasValue) {
                      HiveTask parentHiveTask = GetHiveTaskById(localJob.ParentTaskId.Value);
                      parentHiveTask.IntegrateChild(itemJob, localJob.Id);
                    } else {
                      localHiveTask.ItemTask = itemJob;
                    }
                  }
                  localHiveTask.IsDownloading = false;
                  localHiveTask.Task.LastTaskDataUpdate = lightweightTask.LastTaskDataUpdate;
                }
              }
            });
          }
        }
      }
      GC.Collect(); // force GC, because .NET is too lazy here (deserialization takes a lot of memory)
      if (AllJobsFinished()) {
        this.ExecutionState = Core.ExecutionState.Stopped;
        StopResultPolling();
      }
      UpdateTotalExecutionTime();
      UpdateStatistics();
      OnStateLogListChanged();
      OnTaskReceived();
    }

    public HiveTask GetHiveTaskById(Guid jobId) {
      foreach (HiveTask t in this.HiveTasks) {
        var hj = t.GetHiveTaskByTaskId(jobId);
        if (hj != null)
          return hj;
      }
      return null;
    }

    private void UpdateStatistics() {
      var jobs = this.GetAllHiveTasks();
      job.JobCount = jobs.Count();
      job.CalculatingCount = jobs.Count(j => j.Task.State == TaskState.Calculating);
      job.FinishedCount = jobs.Count(j => j.Task.State == TaskState.Finished);
      OnJobStatisticsChanged();
    }

    public bool AllJobsFinished() {
      return this.GetAllHiveTasks().All(j => (j.Task.State == TaskState.Finished
                                                   || j.Task.State == TaskState.Aborted
                                                   || j.Task.State == TaskState.Failed)
                                                   && !j.IsDownloading);
    }

    private void jobResultPoller_ExceptionOccured(object sender, EventArgs<Exception> e) {
      OnExceptionOccured(sender, e.Value);
    }
    private void jobDownloader_ExceptionOccured(object sender, EventArgs<Exception> e) {
      OnExceptionOccured(sender, e.Value);
    }
    public void UpdateTotalExecutionTime() {
      this.ExecutionTime = TimeSpan.FromMilliseconds(this.GetAllHiveTasks().Sum(x => x.Task.ExecutionTime.TotalMilliseconds));
    }
    #endregion

    #region Job Events
    private void RegisterJobEvents() {
      job.ToStringChanged += new EventHandler(OnToStringChanged);
      job.PropertyChanged += new PropertyChangedEventHandler(job_PropertyChanged);
      job.ItemImageChanged += new EventHandler(job_ItemImageChanged);
      job.ModifiedChanged += new EventHandler(job_ModifiedChanged);
    }

    private void DergisterJobEvents() {
      job.ToStringChanged -= new EventHandler(OnToStringChanged);
      job.PropertyChanged -= new PropertyChangedEventHandler(job_PropertyChanged);
      job.ItemImageChanged -= new EventHandler(job_ItemImageChanged);
      job.ModifiedChanged -= new EventHandler(job_ModifiedChanged);
    }
    #endregion

    #region Event Handler
    public event EventHandler RefreshAutomaticallyChanged;
    private void OnRefreshAutomaticallyChanged() {
      var handler = RefreshAutomaticallyChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler JobChanged;
    private void OnJobChanged() {
      var handler = JobChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler ModifiedChanged;
    private void job_ModifiedChanged(object sender, EventArgs e) {
      var handler = ModifiedChanged;
      if (handler != null) handler(sender, e);
    }

    public event EventHandler ItemImageChanged;
    private void job_ItemImageChanged(object sender, EventArgs e) {
      var handler = ItemImageChanged;
      if (handler != null) handler(this, e);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void job_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      this.IsSharable = job.Permission == Permission.Full;
      this.IsControllable = job.Permission == Permission.Full;

      var handler = PropertyChanged;
      if (handler != null) handler(sender, e);
    }

    public event EventHandler ToStringChanged;
    private void OnToStringChanged(object sender, EventArgs e) {
      var handler = ToStringChanged;
      if (handler != null) handler(this, e);
    }

    public event EventHandler IsDownloadableChanged;
    private void OnIsDownloadableChanged() {
      var handler = IsDownloadableChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler IsControllableChanged;
    private void OnIsControllableChanged() {
      var handler = IsControllableChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler IsSharableChanged;
    private void OnIsSharableChanged() {
      var handler = IsSharableChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler IsAllowedPrivilegedChanged;
    private void OnIsAllowedPrivilegedChanged() {
      var handler = IsAllowedPrivilegedChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler JobStatisticsChanged;
    private void OnJobStatisticsChanged() {
      var handler = JobStatisticsChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler<EventArgs<Exception>> ExceptionOccured;
    private void OnExceptionOccured(object sender, Exception exception) {
      log.LogException(exception);
      var handler = ExceptionOccured;
      if (handler != null) handler(sender, new EventArgs<Exception>(exception));
    }

    public event EventHandler StateLogListChanged;
    private void OnStateLogListChanged() {
      var handler = StateLogListChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler ExecutionTimeChanged;
    protected virtual void OnExecutionTimeChanged() {
      var handler = ExecutionTimeChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler ExecutionStateChanged;
    protected virtual void OnExecutionStateChanged() {
      var handler = ExecutionStateChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler TaskReceived;
    protected virtual void OnTaskReceived() {
      var handler = TaskReceived;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler IsProgressingChanged;
    private void OnIsProgressingChanged() {
      var handler = IsProgressingChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion

    #region HiveTasks Events
    private void RegisterHiveJobsEvents() {
      this.hiveTasks.ItemsAdded += new CollectionItemsChangedEventHandler<HiveTask>(hivetasks_ItemsAdded);
      this.hiveTasks.ItemsRemoved += new CollectionItemsChangedEventHandler<HiveTask>(hiveTasks_ItemsRemoved);
      this.hiveTasks.CollectionReset += new CollectionItemsChangedEventHandler<HiveTask>(hiveTasks_CollectionReset);
    }

    private void DeregisterHiveJobsEvents() {
      this.hiveTasks.ItemsAdded -= new CollectionItemsChangedEventHandler<HiveTask>(hivetasks_ItemsAdded);
      this.hiveTasks.ItemsRemoved -= new CollectionItemsChangedEventHandler<HiveTask>(hiveTasks_ItemsRemoved);
      this.hiveTasks.CollectionReset -= new CollectionItemsChangedEventHandler<HiveTask>(hiveTasks_CollectionReset);
    }

    private void hiveTasks_CollectionReset(object sender, CollectionItemsChangedEventArgs<HiveTask> e) {
      foreach (var item in e.Items) {
        item.StateLogChanged -= new EventHandler(item_StateLogChanged);
      }
      OnHiveTasksReset(e);
    }

    private void hiveTasks_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<HiveTask> e) {
      foreach (var item in e.Items) {
        item.StateLogChanged -= new EventHandler(item_StateLogChanged);
      }
      OnHiveTasksRemoved(e);
    }

    private void hivetasks_ItemsAdded(object sender, CollectionItemsChangedEventArgs<HiveTask> e) {
      foreach (var item in e.Items) {
        item.StateLogChanged += new EventHandler(item_StateLogChanged);
        item.IsControllable = this.IsControllable;
      }
      OnHiveTasksAdded(e);
    }

    private void item_StateLogChanged(object sender, EventArgs e) {
      OnStateLogListChanged();
    }
    #endregion

    public event EventHandler HiveTasksChanged;
    protected virtual void OnHiveTasksChanged() {
      StopResultPolling();
      if (this.HiveTasks != null && this.HiveTasks.Count > 0 && this.GetAllHiveTasks().All(x => x.Task.Id != Guid.Empty)) {
        if (IsFinished()) {
          this.ExecutionState = Core.ExecutionState.Stopped;
          this.RefreshAutomatically = false;
          if (jobResultPoller != null) DeregisterResultPollingEvents();
        } else {
          this.RefreshAutomatically = true;
        }
      }

      var handler = HiveTasksChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler Loaded;
    public virtual void OnLoaded() {
      this.UpdateTotalExecutionTime();
      this.OnStateLogListChanged();

      if (this.ExecutionState != ExecutionState.Stopped) {
        this.RefreshAutomatically = true;
      }

      var handler = Loaded;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler<CollectionItemsChangedEventArgs<HiveTask>> HiveTasksAdded;
    private void OnHiveTasksAdded(CollectionItemsChangedEventArgs<HiveTask> e) {
      var handler = HiveTasksAdded;
      if (handler != null) handler(this, e);
    }

    public event EventHandler<CollectionItemsChangedEventArgs<HiveTask>> HiveTasksRemoved;
    private void OnHiveTasksRemoved(CollectionItemsChangedEventArgs<HiveTask> e) {
      var handler = HiveTasksRemoved;
      if (handler != null) handler(this, e);
    }

    public event EventHandler<CollectionItemsChangedEventArgs<HiveTask>> HiveTasksReset;
    private void OnHiveTasksReset(CollectionItemsChangedEventArgs<HiveTask> e) {
      var handler = HiveTasksReset;
      if (handler != null) handler(this, e);
    }

    public Guid Id {
      get { return job.Id; }
      set { job.Id = value; }
    }
    public bool Modified {
      get { return job.Modified; }
    }
    public void Store() {
      job.Store();
    }
    public string ItemDescription {
      get { return job.ItemDescription; }
    }
    public Image ItemImage {
      get { return job.ItemImage; }
    }
    public string ItemName {
      get { return job.ItemName; }
    }
    public Version ItemVersion {
      get { return job.ItemVersion; }
    }

    public override string ToString() {
      return string.Format("{0} {1}", Job.DateCreated.ToString("MM.dd.yyyy HH:mm"), Job.ToString());
    }

    public bool IsFinished() {
      return HiveTasks != null
        && HiveTasks.All(x => x.Task.DateFinished.HasValue && x.Task.DateCreated.HasValue);
    }

    public IEnumerable<HiveTask> GetAllHiveTasks() {
      if (hiveTasks == null) return Enumerable.Empty<HiveTask>();

      var tasks = new List<HiveTask>();
      foreach (HiveTask task in HiveTasks) {
        tasks.AddRange(task.GetAllHiveTasks());
      }
      return tasks;
    }

    public int CompareTo(RefreshableJob other) {
      return this.ToString().CompareTo(other.ToString());
    }
  }
}
