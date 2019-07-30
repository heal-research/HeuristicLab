#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;

namespace HeuristicLab.Clients.Hive.SlaveCore {
  public class SlaveStatusInfo {
    private static object coreLock = new object();
    private static int tasksFetched;  // number of fetched jobs
    private static int tasksStarted;  // number of started jobs
    private static int tasksFinished; // everything went fine
    private static int tasksAborted;  // server sent abort
    private static int tasksFailed;   // tasks that failed in the sandbox
    private static int usedCores;    // number of cores currently used

    public static DateTime LoginTime { get; set; }

    public static int UsedCores {
      get { return usedCores; }
    }

    public static int TasksStarted {
      get { return tasksStarted; }
    }

    public static int TasksFinished {
      get { return tasksFinished; }
    }

    public static int TasksAborted {
      get { return tasksAborted; }
    }

    public static int TasksFetched {
      get { return tasksFetched; }
    }

    public static int TasksFailed {
      get { return tasksFailed; }
    }

    public static void IncrementTasksStarted() {
      Interlocked.Increment(ref tasksStarted);
    }

    public static void IncrementTasksFinished() {
      Interlocked.Increment(ref tasksFinished);
    }

    public static void IncrementTasksFailed() {
      Interlocked.Increment(ref tasksFailed);
    }

    public static void IncrementTasksAborted() {
      Interlocked.Increment(ref tasksAborted);
    }

    public static void IncrementTasksFetched() {
      Interlocked.Increment(ref tasksFetched);
    }

    public static void IncrementUsedCores(int val) {
      Interlocked.Add(ref usedCores, val);
    }

    public static void DecrementUsedCores(int val) {
      Interlocked.Add(ref usedCores, -val);
    }
  }
}
