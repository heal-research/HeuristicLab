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

namespace HeuristicLab.Common {
  public abstract class ContentManager {
    protected ContentManager() {
    }

    private static ContentManager instance;
    public static ContentManager Instance {
      get { return instance; }
    }
    public static void CreateInstance<T>() where T : ContentManager {
      if (instance != null)
        throw new InvalidOperationException("ContentManager was already created.");
      instance = Activator.CreateInstance<T>();
    }

    public static void Save(IStorableContent content) {
      content.Save();
    }
    public static void Save(IStorableContent content, string filename) {
      content.Save(filename);
    }

    protected abstract void Load(string filename, bool flag);
    public static void Load(string filename) {
      if (instance == null)
        throw new InvalidOperationException("ContentManager must be created before access is allowed.");

      Exception ex = null;
      instance.OnLoadOperationStarted();
      try {
        instance.Load(filename, false);
      }
      catch (Exception e) {
        ex = e;
      }
      instance.OnLoadOperationFinished(ex);
    }

    public static void LoadAsynchronous(string filename) {
      if (instance == null)
        throw new InvalidOperationException("ContentManager must be created before access is allowed.");

      ThreadPool.QueueUserWorkItem(
        new WaitCallback(delegate(object arg) {
        Load(filename);
      })
      );
    }
    

    public event EventHandler LoadOperationStarted;
    protected virtual void OnLoadOperationStarted() {
      EventHandler handler = LoadOperationStarted;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    public event EventHandler<EventArgs<Exception>> LoadOperationFinished;
    protected virtual void OnLoadOperationFinished(Exception e) {
      EventHandler<EventArgs<Exception>> handler = LoadOperationFinished;
      if (handler != null)
        handler(this, new EventArgs<Exception>(e));
    }
  }
}
