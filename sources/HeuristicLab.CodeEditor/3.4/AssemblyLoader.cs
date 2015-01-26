#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HeuristicLab.Common;
using ICSharpCode.NRefactory.Documentation;
using ICSharpCode.NRefactory.TypeSystem;

namespace HeuristicLab.CodeEditor {
  internal class AssemblyLoader {
    private readonly HashSet<IUnresolvedAssembly> assemblySet = new HashSet<IUnresolvedAssembly>();
    private readonly object locker = new object();

    public void AddAssembly(Assembly assembly) {
      var assemblies = new[] { assembly };
      OnAssembliesLoading(assemblies);
      var unresolvedAssembly = Load(assembly);
      lock (locker)
        assemblySet.Add(unresolvedAssembly);
      OnInternalAssembliesLoaded(new[] { unresolvedAssembly });
      OnAssembliesLoaded(assemblies);
    }

    public void AddAssemblies(IEnumerable<Assembly> assemblies) {
      var a = assemblies.ToArray();
      OnAssembliesLoading(a);
      var ua = new IUnresolvedAssembly[a.Length];
      Parallel.For(0, a.Length, i => ua[i] = Load(a[i]));

      lock (locker)
        foreach (var asm in ua)
          assemblySet.Add(asm);

      OnInternalAssembliesLoaded(ua);
      OnAssembliesLoaded(a);
    }

    public async Task AddAssembliesAsync(IEnumerable<Assembly> assemblies) {
      await Task.Run(() => AddAssemblies(assemblies));
    }

    public void RemoveAssembly(Assembly assembly) {
      var assemblies = new[] { assembly };
      OnAssembliesUnloading(assemblies);
      IUnresolvedAssembly unresolvedAssembly;
      lock (locker) {
        unresolvedAssembly = assemblySet.SingleOrDefault(x => x.FullAssemblyName == assembly.FullName);
        assemblySet.Remove(unresolvedAssembly);
      }
      OnInternalAssembliesUnloaded(new[] { unresolvedAssembly });
      OnAssembliesUnloaded(assemblies);
    }

    #region Loading Helper
    private IUnresolvedAssembly Load(Assembly assembly) {
      var loader = new CecilLoader {
        DocumentationProvider = GetXmlDocumentation(assembly.Location)
      };
      return loader.LoadAssemblyFile(assembly.Location);
    }
    #endregion

    #region XML Documentation Helpers
    private static readonly List<string> xmlDocLookupDirectories = new List<string> {
      Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0"),
      System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()
    };

    private static IDocumentationProvider GetXmlDocumentation(string assemblyLocation) {
      if (!File.Exists(assemblyLocation)) return null;

      string xmlDocFileName = Path.GetFileNameWithoutExtension(assemblyLocation) + ".xml";

      foreach (var dir in xmlDocLookupDirectories) {
        string xmlDocFileLocation = Path.Combine(dir, xmlDocFileName);
        if (File.Exists(xmlDocFileLocation)) return new XmlDocumentationProvider(xmlDocFileLocation);
      }

      return null;
    }
    #endregion

    #region Events
    public event EventHandler<EventArgs<IEnumerable<Assembly>>> AssembliesLoading;
    private void OnAssembliesLoading(IEnumerable<Assembly> args) {
      var handler = AssembliesLoading;
      if (handler != null) handler(this, new EventArgs<IEnumerable<Assembly>>(args));
    }

    public event EventHandler<EventArgs<IEnumerable<Assembly>>> AssembliesLoaded;
    private void OnAssembliesLoaded(IEnumerable<Assembly> args) {
      var handler = AssembliesLoaded;
      if (handler != null) handler(this, new EventArgs<IEnumerable<Assembly>>(args));
    }

    public event EventHandler<EventArgs<IEnumerable<IUnresolvedAssembly>>> InternalAssembliesLoaded;
    private void OnInternalAssembliesLoaded(IEnumerable<IUnresolvedAssembly> args) {
      var handler = InternalAssembliesLoaded;
      if (handler != null) handler(this, new EventArgs<IEnumerable<IUnresolvedAssembly>>(args));
    }

    public event EventHandler<EventArgs<IEnumerable<Assembly>>> AssembliesUnloading;
    private void OnAssembliesUnloading(IEnumerable<Assembly> args) {
      var handler = AssembliesUnloading;
      if (handler != null) handler(this, new EventArgs<IEnumerable<Assembly>>(args));
    }

    public event EventHandler<EventArgs<IEnumerable<Assembly>>> AssembliesUnloaded;
    private void OnAssembliesUnloaded(IEnumerable<Assembly> args) {
      var handler = AssembliesUnloaded;
      if (handler != null) handler(this, new EventArgs<IEnumerable<Assembly>>(args));
    }

    public event EventHandler<EventArgs<IEnumerable<IUnresolvedAssembly>>> InternalAssembliesUnloaded;
    private void OnInternalAssembliesUnloaded(IEnumerable<IUnresolvedAssembly> args) {
      var handler = InternalAssembliesUnloaded;
      if (handler != null) handler(this, new EventArgs<IEnumerable<IUnresolvedAssembly>>(args));
    }
    #endregion
  }
}
