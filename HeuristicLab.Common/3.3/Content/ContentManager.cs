#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Common {
  public abstract class ContentManager {
    private static ContentManager instance;

    public static void Initialize(ContentManager manager) {
      if (manager == null) throw new ArgumentNullException();
      if (ContentManager.instance != null) throw new InvalidOperationException("ContentManager has already been initialized.");
      ContentManager.instance = manager;
    }

    protected ContentManager() { }

    public static IStorableContent Load(string filename) {
      if (instance == null) throw new InvalidOperationException("ContentManager is not initialized.");
      IStorableContent content = instance.LoadContent(filename);
      content.Filename = filename;
      return content;
    }
    public static void LoadAsync(string filename, Action<IStorableContent, Exception> loadingCompletedCallback) {
      if (instance == null) throw new InvalidOperationException("ContentManager is not initialized.");
      var func = new Func<string, IStorableContent>(instance.LoadContent);
      func.BeginInvoke(filename, delegate (IAsyncResult result) {
        Exception error = null;
        IStorableContent content = null;
        try {
          content = func.EndInvoke(result);
          content.Filename = filename;
        } catch (Exception ex) {
          error = ex;
        }
        loadingCompletedCallback(content, error);
      }, null);
    }
    protected abstract IStorableContent LoadContent(string filename);

    public static void Save(IStorableContent content, string filename, bool compressed, CancellationToken cancellationToken = default(CancellationToken)) {
      if (instance == null) throw new InvalidOperationException("ContentManager is not initialized.");
      instance.SaveContent(content, filename, compressed, cancellationToken);
      content.Filename = filename;
    }
    public static void SaveAsync(IStorableContent content, string filename, bool compressed, Action<IStorableContent, Exception> savingCompletedCallback, CancellationToken cancellationToken = default(CancellationToken)) {
      if (instance == null) throw new InvalidOperationException("ContentManager is not initialized.");
      var action = new Action<IStorableContent, string, bool, CancellationToken>(instance.SaveContent);
      action.BeginInvoke(content, filename, compressed, cancellationToken, delegate (IAsyncResult result) {
        Exception error = null;
        try {
          action.EndInvoke(result);
          content.Filename = filename;
        } catch (Exception ex) {
          error = ex;
        }
        savingCompletedCallback(content, error);
      }, null);

    }
    protected abstract void SaveContent(IStorableContent content, string filename, bool compressed, CancellationToken cancellationToken);
  }
}
