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
using HeuristicLab.Scripting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Tests {
  public static class ScriptingUtils {
    public const string ScriptsDirectory = @"Scripts\";
    public const string ScriptSourcesDirectory = @"Test Resources\Script Sources\";
    public const string ScriptFileExtension = ".hl";
    public const string ScriptSourceFileExtension = ".cs";

    public static void RunScript(CSharpScript s) {
      Exception ex = null;
      s.ScriptExecutionFinished += (sender, e) => { ex = e.Value; };
      s.Execute();
      Assert.IsNull(ex);
    }

    public static T GetVariable<T>(CSharpScript a, string resultName) {
      return (T)a.VariableStore[resultName];
    }
  }
}
