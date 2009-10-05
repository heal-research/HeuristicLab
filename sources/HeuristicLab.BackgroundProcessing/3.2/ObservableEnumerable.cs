using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;

namespace HeuristicLab.BackgroundProcessing {
  /// <summary>
  /// Combines <code>IEnumerable&gt;T&lt;"/></code> and <code>INotifyCollectionChanged</code> into
  /// a single interface.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IObservableEnumerable<T> : IEnumerable<T>, INotifyCollectionChanged { }
}
