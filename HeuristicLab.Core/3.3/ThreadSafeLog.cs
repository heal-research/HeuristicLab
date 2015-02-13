#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [Item("ThreadSafeLog", "A thread-safe log for logging string messages.")]
  [StorableClass]
  public sealed class ThreadSafeLog : Item, ILog, IStorableContent {
    public string Filename { get; set; }
    private ConcurrentQueue<string> messages;

    public IEnumerable<string> Messages {
      get { return messages.ToArray(); }
    }

    [Storable(Name = "messages")]
    private IEnumerable<string> StorableMessages {
      get { return Messages; }
      set { messages = new ConcurrentQueue<string>(value); }
    }

    [Storable]
    private long maxMessageCount;
    public long MaxMessageCount {
      get { return maxMessageCount; }
    }

    [StorableConstructor]
    private ThreadSafeLog(bool deserializing) : base(deserializing) { }
    private ThreadSafeLog(ThreadSafeLog original, Cloner cloner)
      : base(original, cloner) {
      this.messages = new ConcurrentQueue<string>(original.messages);
      this.maxMessageCount = original.maxMessageCount;
    }
    public ThreadSafeLog(long maxMessageCount = int.MaxValue) {
      this.messages = new ConcurrentQueue<string>();
      this.maxMessageCount = maxMessageCount;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ThreadSafeLog(this, cloner);
    }

    public void Clear() {
      messages = new ConcurrentQueue<string>();
      OnCleared();
    }

    public void LogMessage(string message) {
      var s = Log.FormatLogMessage(message);
      messages.Enqueue(s);
      CapMessages();
      OnMessageAdded(s);
    }

    public void LogException(Exception ex) {
      var s = Log.FormatException(ex);
      messages.Enqueue(s);
      CapMessages();
      OnMessageAdded(s);
    }

    private readonly object capLock = new object();
    private void CapMessages() {
      lock (capLock) {
        string s;
        while (messages.Count > maxMessageCount)
          if (!messages.TryDequeue(out s)) break;
      }
    }

    public event EventHandler<EventArgs<string>> MessageAdded;
    private void OnMessageAdded(string message) {
      var handler = MessageAdded;
      if (handler != null) handler(this, new EventArgs<string>(message));
    }

    public event EventHandler Cleared;
    private void OnCleared() {
      var handler = Cleared;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
