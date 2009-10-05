using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Collections.ObjectModel;

namespace HeuristicLab.BackgroundProcessing {
  /// <summary>
  /// Extends the BackgroundWorker to make it easier to follow progress and automatically
  /// registers with the WorkerMonitor.
  /// </summary>
  public class ObservableBackgroundWorker : BackgroundWorker, INotifyPropertyChanged {
    public string Name { get; private set; }
    public int Progress { get; private set; }

    /// <summary>
    /// Indicate whether the worker is actually doing something. In contrast
    /// to IsBusy this shows that the worker is about to execute or leave DoWork
    /// while IsBusy shows wheter the worker has been "started" with this.RunWorkerAsync.
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Notification about worker has actually started working i.e.
    /// has been assigned a ThreadPool thread.
    /// </summary>
    public event EventHandler WorkerStarted;

    /// <summary>
    /// Unsynchronized version of RunWorkerCompleted so you don't
    /// have to wait for the GUI thread to be notified.
    /// </summary>
    public event EventHandler WorkerStopped;

    public event PropertyChangedEventHandler PropertyChanged;

    public ObservableBackgroundWorker(string name, WorkerMonitor monitor) {
      Name = name;
      monitor.RegisterWorker(this);
      IsRunning = false;
    }

    public ObservableBackgroundWorker(string name) : this(name, WorkerMonitor.Default) { }

    protected override void OnProgressChanged(ProgressChangedEventArgs e) {
      Progress = e.ProgressPercentage;
      base.OnProgressChanged(e);
      OnPropertyChanged("Progress");
    }

    protected override void OnDoWork(DoWorkEventArgs e) {
      IsRunning = true;
      try {
        OnPropertyChanged("IsRunning");
        OnWorkerStarted();
        base.OnDoWork(e);
      } finally {
        IsRunning = false;
        OnWorkerStopped();
        OnPropertyChanged("IsRunning");
      }
    }

    protected void OnWorkerStarted() {
      if (WorkerStarted != null)
        WorkerStarted(this, new EventArgs());
    }

    protected void OnWorkerStopped() {
      if (WorkerStopped != null)
        WorkerStopped(this, new EventArgs());
    }

    protected void OnPropertyChanged(string name) {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(name));
    }

    public new void CancelAsync() {
      base.CancelAsync();
      OnPropertyChanged("CancellationPending");
    }
  }
}
