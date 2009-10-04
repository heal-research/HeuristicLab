using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using System.Collections;
using System.ComponentModel;
using System.Windows.Threading;

namespace HeuristicLab.BackgroundProcessing {
  /// <summary>
  /// Takes an <code>ObservableEnumerable</code> and transfers all events to the
  /// specified Dispatcher. It also keeps a cache of the collection to ensure
  /// the Dispatcher sees a consistent state.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class DispatchedView<T> : ObservableEnumerable<T> where T : class {
    private List<T> cache;
    private ObservableEnumerable<T> source;
    private Dispatcher Dispatcher;
    private DispatcherPriority Priority;

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public DispatchedView(ObservableEnumerable<T> source, Dispatcher dispatcher, DispatcherPriority priority) {
      cache = source.ToList();
      source.CollectionChanged += source_CollectionChanged;
      this.source = source;
      Dispatcher = dispatcher;
      Priority = priority;
    }

    void source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
      Dispatcher.BeginInvoke(new Action(() => {
        if (e.Action == NotifyCollectionChangedAction.Add) {
          cache.InsertRange(e.NewStartingIndex, e.NewItems.Cast<T>());
        } else if (e.Action == NotifyCollectionChangedAction.Remove) {
          cache.RemoveRange(e.OldStartingIndex, e.OldItems.Count);
        } else {
          cache = source.ToList();
        }
        OnCollectionChanged(e);
      }), Priority, null);
    }

    protected void OnCollectionChanged(NotifyCollectionChangedEventArgs args) {
      if (CollectionChanged != null)
        CollectionChanged(this, args);
    }

    public IEnumerator<T> GetEnumerator() {
      return cache.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
}
