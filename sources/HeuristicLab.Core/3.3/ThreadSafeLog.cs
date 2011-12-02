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
using System.Linq;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [Item("ThreadSafeLog", "A thread-safe log for logging string messages.")]
  [StorableClass]
  public sealed class ThreadSafeLog : Log {
    private ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

    public override IEnumerable<string> Messages {
      get {
        locker.EnterReadLock();
        try {
          return messages.ToArray(); // return copy of messages
        } finally { locker.ExitReadLock(); }
      }
    }

    [StorableConstructor]
    private ThreadSafeLog(bool deserializing) : base(deserializing) { }
    public ThreadSafeLog(long maxMessageCount = -1)
      : base(maxMessageCount) {
    }

    private ThreadSafeLog(ThreadSafeLog original, Cloner cloner)
      : base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      locker.EnterReadLock();
      try {
        return new ThreadSafeLog(this, cloner);
      } finally { locker.ExitReadLock(); }
    }

    public override void Clear() {
      locker.EnterWriteLock();
      try {
        messages.Clear();
      } finally { locker.ExitWriteLock(); }
      OnCleared();
    }

    public override void LogMessage(string message) {
      string s = FormatLogMessage(message);
      locker.EnterWriteLock();
      try {
        messages.Add(s);
        CapMessages();
      } finally { locker.ExitWriteLock(); }
      OnMessageAdded(s);
    }

    public override void LogException(Exception ex) {
      string s = FormatException(ex);
      locker.EnterWriteLock();
      try {
        messages.Add(s);
        CapMessages();
      } finally { locker.ExitWriteLock(); }
      OnMessageAdded(s);
    }
  }
}
