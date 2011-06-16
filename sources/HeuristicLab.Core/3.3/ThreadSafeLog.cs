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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [Item("ThreadSafeLog", "A thread-safe log for logging string messages.")]
  [StorableClass]
  public class ThreadSafeLog : Item, ILog, IStorableContent {
    protected ReaderWriterLockSlim locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

    public string Filename { get; set; }

    public override Image ItemImage {
      get { return VSImageLibrary.File; }
    }

    public IEnumerable<string> Messages {
      get {
        locker.EnterReadLock();
        try {
          return log.Messages.ToArray(); // return copy of messages
        }
        finally { locker.ExitReadLock(); }
      }
    }

    [Storable]
    protected ILog log;

    [StorableConstructor]
    protected ThreadSafeLog(bool deserializing) : base(deserializing) { }
    public ThreadSafeLog()
      : base() {
      this.log = new Log();
      RegisterLogEvents();
    }
    public ThreadSafeLog(ILog log)
      : base() {
      this.log = log;
      RegisterLogEvents();
    }
    ~ThreadSafeLog() {
      locker.Dispose();
    }

    protected ThreadSafeLog(ThreadSafeLog original, Cloner cloner)
      : base(original, cloner) {
      log = cloner.Clone(original.log);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ThreadSafeLog(this, cloner);
    }

    public virtual void Clear() {
      locker.EnterWriteLock();
      try {
        log.Clear();
      }
      finally { locker.ExitWriteLock(); }
    }

    public virtual void LogMessage(string message) {
      locker.EnterWriteLock();
      try {
        log.LogMessage(message);
      }
      finally { locker.ExitWriteLock(); }
    }

    public virtual void LogException(Exception ex) {
      locker.EnterWriteLock();
      try {
        log.LogException(ex);
      }
      finally { locker.ExitWriteLock(); }
    }

    #region Log Events
    private void RegisterLogEvents() {
      this.log.Cleared += new EventHandler(log_Cleared);
      this.log.MessageAdded += new EventHandler<EventArgs<string>>(log_MessageAdded);
      this.log.ToStringChanged += new EventHandler(log_ToStringChanged);
    }

    private void log_ToStringChanged(object sender, EventArgs e) {
      OnToStringChanged();
    }

    private void log_MessageAdded(object sender, EventArgs<string> e) {
      OnMessageAdded(e.Value);
    }

    private void log_Cleared(object sender, EventArgs e) {
      OnCleared();
    }
    #endregion

    #region Event Handler
    public event EventHandler<EventArgs<string>> MessageAdded;
    protected virtual void OnMessageAdded(string message) {
      EventHandler<EventArgs<string>> handler = MessageAdded;
      if (handler != null) handler(this, new EventArgs<string>(message));
    }
    public event EventHandler Cleared;
    protected virtual void OnCleared() {
      EventHandler handler = Cleared;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion
  }
}
