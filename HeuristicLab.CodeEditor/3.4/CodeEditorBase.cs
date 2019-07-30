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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Common;
using ICSharpCode.NRefactory.TypeSystem;

namespace HeuristicLab.CodeEditor {
  public class CodeEditorBase : UserControl {
    public virtual string UserCode { get; set; }

    public virtual string Prefix { get; set; }

    public virtual string Suffix { get; set; }

    public virtual bool ReadOnly { get; set; }

    public virtual bool Locked { get; set; }

    public virtual void AddAssemblies(IEnumerable<Assembly> assemblies) { }

    public virtual Task AddAssembliesAsync(IEnumerable<Assembly> assemblies) {
      return null;
    }

    public virtual void AddAssembly(Assembly assembly) { }

    public virtual void RemoveAssembly(Assembly assembly) { }

    public virtual void RemoveAssemblies(IEnumerable<Assembly> assemblies) { }

    public virtual void ScrollAfterPrefix() { }

    public virtual void ScrollToPosition(int line, int column) { }

    public virtual void ClearEditHistory() { }

    public virtual void ShowCompileErrors(CompilerErrorCollection compileErrors) { }

    #region Events
    public event EventHandler TextEditorTextChanged;
    protected virtual void OnTextEditorTextChanged() {
      var handler = TextEditorTextChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler<EventArgs<IEnumerable<Assembly>>> AssembliesLoading;
    protected virtual void OnAssembliesLoading(IEnumerable<Assembly> args) {
      var handler = AssembliesLoading;
      if (handler != null) handler(this, new EventArgs<IEnumerable<Assembly>>(args));
    }

    public event EventHandler<EventArgs<IEnumerable<Assembly>>> AssembliesLoaded;
    protected virtual void OnAssembliesLoaded(IEnumerable<Assembly> args) {
      var handler = AssembliesLoaded;
      if (handler != null) handler(this, new EventArgs<IEnumerable<Assembly>>(args));
    }

    public event EventHandler<EventArgs<IEnumerable<IUnresolvedAssembly>>> InternalAssembliesLoaded;
    protected virtual void OnInternalAssembliesLoaded(IEnumerable<IUnresolvedAssembly> args) {
      var handler = InternalAssembliesLoaded;
      if (handler != null) handler(this, new EventArgs<IEnumerable<IUnresolvedAssembly>>(args));
    }

    public event EventHandler<EventArgs<IEnumerable<Assembly>>> AssembliesUnloading;
    protected virtual void OnAssembliesUnloading(IEnumerable<Assembly> args) {
      var handler = AssembliesUnloading;
      if (handler != null) handler(this, new EventArgs<IEnumerable<Assembly>>(args));
    }

    public event EventHandler<EventArgs<IEnumerable<Assembly>>> AssembliesUnloaded;
    protected virtual void OnAssembliesUnloaded(IEnumerable<Assembly> args) {
      var handler = AssembliesUnloaded;
      if (handler != null) handler(this, new EventArgs<IEnumerable<Assembly>>(args));
    }

    public event EventHandler<EventArgs<IEnumerable<IUnresolvedAssembly>>> InternalAssembliesUnloaded;
    protected virtual void OnInternalAssembliesUnloaded(IEnumerable<IUnresolvedAssembly> args) {
      var handler = InternalAssembliesUnloaded;
      if (handler != null) handler(this, new EventArgs<IEnumerable<IUnresolvedAssembly>>(args));
    }
    #endregion
  }
}
