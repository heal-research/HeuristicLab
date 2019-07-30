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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HEAL.Attic;

namespace HeuristicLab.Common {
  public abstract class ContentManager {
    public class Info {
      public Info() { }

      public Info(string filename, TimeSpan duration) {
        Filename = filename;
        Duration = duration;
      }

      public Info(string filename, SerializationInfo serInfo) {
        Filename = filename;
        Duration = serInfo.Duration;
        UnknownTypeGuids = serInfo.UnknownTypeGuids;
        NumberOfSerializedObjects = serInfo.NumberOfSerializedObjects;
        SerializedTypes = serInfo.SerializedTypes;        
      }

      public TimeSpan Duration { get; internal set; }
      public IEnumerable<Guid> UnknownTypeGuids { get; internal set; } = Enumerable.Empty<Guid>();
      public int NumberOfSerializedObjects { get; internal set; }
      public IEnumerable<Type> SerializedTypes { get; internal set; } = Enumerable.Empty<Type>();
      public string Filename { get; internal set; } = string.Empty;
    }

    private static ContentManager instance;

    public static void Initialize(ContentManager manager) {
      if (manager == null) throw new ArgumentNullException();
      if (ContentManager.instance != null) throw new InvalidOperationException("ContentManager has already been initialized.");
      ContentManager.instance = manager;
    }

    protected ContentManager() { }

    public static IStorableContent Load(string filename) {
      return Load(filename, out var _);
    }

    public static IStorableContent Load(string filename, out Info serializationInfo) {
      if (instance == null) throw new InvalidOperationException("ContentManager is not initialized.");
      IStorableContent content = instance.LoadContent(filename, out serializationInfo);
      
      if (content != null) content.Filename = filename;
      return content;
    }


    public static Task LoadAsync(string filename, Action<IStorableContent, Exception> loadingCompletedCallback) {
      return LoadAsync(filename, (content, error, info) => loadingCompletedCallback(content, error)); // drop info
    }

    public static async Task LoadAsync(string filename, Action<IStorableContent, Exception, Info> loadingCompletedCallback) {
      if (instance == null) throw new InvalidOperationException("ContentManager is not initialized.");

      Exception error = null;
      IStorableContent result = null;
      Info serializationInfo = null;
      try {
        result = await Task.Run(() => {
          var content = instance.LoadContent(filename, out serializationInfo);
          if(content!=null) content.Filename = filename;
          return content;
        });
      } catch(Exception ex) {
        error = ex;
      }
      loadingCompletedCallback(result, error, serializationInfo);
    }

    protected abstract IStorableContent LoadContent(string filename, out Info serializationInfo);

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
