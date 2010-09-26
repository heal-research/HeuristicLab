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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Core {
  [Item("Log", "A log for logging string messages.")]
  [StorableClass]
  public class Log : Item, ILog, IStorableContent {
    public string Filename { get; set; }

    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.File; }
    }

    [Storable]
    protected IList<string> messages;
    public virtual IEnumerable<string> Messages {
      get { return messages; }
    }

    public Log()
      : base() {
      messages = new List<string>();
    }
    [StorableConstructor]
    protected Log(bool deserializing) : base(deserializing) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      Log clone = (Log)base.Clone(cloner);
      clone.messages = new List<string>(messages);
      return clone;
    }

    public virtual void Clear() {
      messages.Clear();
      OnCleared();
    }
    public virtual void LogMessage(string message) {
      string s = DateTime.Now.ToString() + "\t" + message;
      messages.Add(s);
      OnMessageAdded(s);
    }
    public virtual void LogException(Exception ex) {
      string s = DateTime.Now.ToString() + "\t" + "Exception occurred:" + Environment.NewLine + ErrorHandling.BuildErrorMessage(ex);
      messages.Add(s);
      OnMessageAdded(s);
    }

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
  }
}
