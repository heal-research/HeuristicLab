#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Threading;

namespace HeuristicLab.Problems.ExternalEvaluation.Service {
  internal class BlockingQueue<T> {
    private readonly Queue<T> queue = new Queue<T>();
    private readonly int maxSize;
    private bool closing;
    public BlockingQueue(int maxSize) { this.maxSize = maxSize; }
    
    public void Enqueue(T item) {
      lock (queue) {
        while (queue.Count >= maxSize) {
          Monitor.Wait(queue);
        }
        if (closing) throw new QueueClosedException();
        queue.Enqueue(item);
        if (queue.Count == 1) {
          // wake up any blocked dequeue
          Monitor.PulseAll(queue);
        }
      }
    }
    public T Dequeue() {
      lock (queue) {
        while (queue.Count == 0) {
          Monitor.Wait(queue);
        }
        if (closing) throw new QueueClosedException();
        T item = queue.Dequeue();
        if (queue.Count == maxSize - 1) {
          // wake up any blocked enqueue
          Monitor.PulseAll(queue);
        }
        return item;
      }
    }
    public void Close() {
      lock (queue) {
        closing = true;
        Monitor.PulseAll(queue);
      }
    }
    public bool TryDequeue(out T value) {
      lock (queue) {
        while (queue.Count == 0) {
          if (closing) {
            value = default(T);
            return false;
          }
          Monitor.Wait(queue);
        }
        value = queue.Dequeue();
        if (queue.Count == maxSize - 1) {
          // wake up any blocked enqueue
          Monitor.PulseAll(queue);
        }
        return true;
      }
    }

    public void Clear() {
      lock (queue) {
        queue.Clear();
      }
    }
  }

  internal class QueueClosedException : Exception {
    public QueueClosedException() : base() { }
    public QueueClosedException(string message) : base(message) { }
    public QueueClosedException(string message, Exception innerException) : base(message, innerException) { }
  }
}
