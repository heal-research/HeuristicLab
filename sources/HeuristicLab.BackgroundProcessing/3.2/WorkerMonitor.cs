using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;

namespace HeuristicLab.BackgroundProcessing {


  /// <summary>
  /// Provides a list of all currently running or pending ObservableBackgroundWorkers.
  /// </summary>
  public class WorkerMonitor : ObservableEnumerable<ObservableBackgroundWorker> {

    public static WorkerMonitor Default = new WorkerMonitor();

    public event ThreadExceptionEventHandler ThreadException;
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    private List<ObservableBackgroundWorker> BackgroundWorkers;
    private ReaderWriterLockSlim workerLock;

    public WorkerMonitor() {
      BackgroundWorkers = new List<ObservableBackgroundWorker>();
      workerLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
    }

    internal void RegisterWorker(ObservableBackgroundWorker worker) {
      worker.RunWorkerCompleted += worker_RunWorkerCompleted;
      worker.WorkerStopped += worker_WorkerStopped;
      try {
        workerLock.EnterUpgradeableReadLock();
        try {
          workerLock.EnterWriteLock();
          BackgroundWorkers.Add(worker);
        } finally {
          workerLock.ExitWriteLock();
        }
        int index = BackgroundWorkers.Count - 1;
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(
          NotifyCollectionChangedAction.Add, worker, index));
      } finally {
        workerLock.ExitUpgradeableReadLock();
      }
    }

    void worker_WorkerStopped(object sender, EventArgs e) {
      ObservableBackgroundWorker worker = sender as ObservableBackgroundWorker;
      try {
        workerLock.EnterUpgradeableReadLock();
        int index = BackgroundWorkers.IndexOf(worker);
        try {
          workerLock.EnterWriteLock();
          BackgroundWorkers.RemoveAt(index);
        } finally {
          workerLock.ExitWriteLock();
        }
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(
          NotifyCollectionChangedAction.Remove, worker, index));
      } finally {
        workerLock.ExitUpgradeableReadLock();
      }
    }

    private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      ObservableBackgroundWorker worker = sender as ObservableBackgroundWorker;
      if (e.Error != null) {
        OnThreadException(new Exception(worker.Name, e.Error));
      }
    }

    protected void OnThreadException(Exception x) {
      if (ThreadException != null)
        ThreadException(this, new ThreadExceptionEventArgs(x));
    }

    public IEnumerator<ObservableBackgroundWorker> GetEnumerator() {
      try {
        workerLock.EnterReadLock();
        IList<ObservableBackgroundWorker> copy = BackgroundWorkers.ToList();
        return copy.GetEnumerator();
      } finally {
        workerLock.ExitReadLock();
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    protected void OnCollectionChanged(NotifyCollectionChangedEventArgs args) {
      if (CollectionChanged != null)
        CollectionChanged(this, args);
    }

    public void CancelAll() {
      List<ObservableBackgroundWorker> cancelableWorkers = GetCancelableWorkers();
      lock (cancelableWorkers) {
        foreach (var worker in cancelableWorkers.ToList()) {
          worker.WorkerStopped += (sender, args) => {
            lock (cancelableWorkers) {
              cancelableWorkers.Remove((ObservableBackgroundWorker)sender);
              Monitor.Pulse(cancelableWorkers);
            }
          };
          worker.CancelAsync();
          if (!worker.IsRunning)
            cancelableWorkers.Remove(worker);
        }
        while (cancelableWorkers.Count > 0)
          Monitor.Wait(cancelableWorkers);
      }
    }

    private List<ObservableBackgroundWorker> GetCancelableWorkers() {
      try {
        workerLock.EnterReadLock();
        return BackgroundWorkers.Where(w => w.WorkerSupportsCancellation).ToList();
      } finally {
        workerLock.ExitReadLock();
      }
    }
  }
}
