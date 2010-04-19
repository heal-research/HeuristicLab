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
using System.Reflection;

namespace HeuristicLab.Common {
  public abstract class StorableContent : Content, IStorableContent {
    public StorableContent()
      : base() {
      this.filename = string.Empty;
      this.saveEnabled = true;
    }
    public StorableContent(bool saveEnabled)
      : this() {
      this.saveEnabled = saveEnabled;
      //NOTE: important do not call propagate changes, because derived objects are not constructed
    }
    public StorableContent(bool saveEnabled, string filename)
      : this(saveEnabled) {
      this.Filename = filename;
    }

    private string filename;
    public string Filename {
      get { return this.filename; }
      set {
        if (this.filename != value) {
          this.filename = value;
          this.OnFilenameChanged();
        }
      }
    }

    private bool saveEnabled;
    public virtual bool SaveEnabled {
      get { return this.saveEnabled; }
      set {
        if (this.saveEnabled != value) {
          this.saveEnabled = value;
          this.PropagateSaveEnabledChanges();
          this.OnSaveEnabledChanged();
        }
      }
    }

    protected void PropagateSaveEnabledChanges() {
      Type type = this.GetType();
      foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Default)) {
        if (typeof(IStorableContent).IsAssignableFrom(fieldInfo.GetType())) {
          IStorableContent storableContent = (IStorableContent)fieldInfo.GetValue(this);
          storableContent.SaveEnabled = this.saveEnabled;
        }
      }
    }

    protected abstract void Save();
    void IStorableContent.Save() {
      if (this.SaveEnabled) {
        this.OnSaveOperationStarted();
        this.Save();
        this.OnSaveOperationFinished(null);
      }
    }
    public void Save(string filename) {
      this.Filename = filename;
      ((IStorableContent)this).Save();
    }

    protected virtual void SaveAsnychronous() {
      ThreadPool.QueueUserWorkItem(
        new WaitCallback(delegate(object arg) {
        try { this.Save(); }
        catch (Exception ex) {
          this.OnSaveOperationFinished(ex);
        }
      })
      );
    }
    void IStorableContent.SaveAsynchronous() {
      this.OnSaveOperationStarted();
      this.SaveAsnychronous();
      this.OnSaveOperationFinished(null);
    }
    public void SaveAsynchronous(string filename) {
      this.Filename = filename;
      ((IStorableContent)this).SaveAsynchronous();
    }

    public event EventHandler FilenameChanged;
    protected virtual void OnFilenameChanged() {
      EventHandler handler = FilenameChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler SaveEnabledChanged;
    protected virtual void OnSaveEnabledChanged() {
      EventHandler handler = SaveEnabledChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler SaveOperationStarted;
    protected virtual void OnSaveOperationStarted() {
      EventHandler handler = SaveOperationStarted;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler<EventArgs<Exception>> SaveOperationFinished;
    protected virtual void OnSaveOperationFinished(Exception ex) {
      EventHandler<EventArgs<Exception>> handler = SaveOperationFinished;
      if (handler != null) handler(this, new EventArgs<Exception>(ex));
    }

  }
}
