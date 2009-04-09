#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2009 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using Microsoft.VisualStudio.TemplateWizard;
using EnvDTE;

namespace HeuristicLab.Tools.NewPluginWizard {
  public class NewPluginWizard : IWizard {
    public void BeforeOpeningFile(ProjectItem projectItem) {
    }

    public void ProjectFinishedGenerating(Project project) {
    }

    public void ProjectItemFinishedGenerating(ProjectItem projectItem) {
    }

    public void RunFinished() {
    }

    public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams) {
      try {
        MessageBox.Show("Init");
        using (NewPluginWizardForm form = new NewPluginWizardForm()) {
          if (form.ShowDialog() == DialogResult.OK) {
            MessageBox.Show(form.Settings.PluginName + "\n" + form.Settings.PluginVersion);
            replacementsDictionary.Add("$PluginName$", form.Settings.PluginName);
            replacementsDictionary.Add("$PluginVersion$", form.Settings.PluginVersion);
          }
        }
        MessageBox.Show("Done");
      }
      catch (Exception ex) {
        MessageBox.Show(ex.ToString());
      }
    }

    public bool ShouldAddProjectItem(string filePath) {
      return true;
    }
  }
}
