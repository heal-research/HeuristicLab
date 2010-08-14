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

using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;

namespace HeuristicLab.VS2010Wizards {
  public class ProblemWizard : IWizard {
    private bool shouldAddItem;
    private ProblemWizardForm form;

    public ProblemWizard() {
      form = new ProblemWizardForm();
    }

    public void BeforeOpeningFile(ProjectItem projectItem) {
    }

    public void ProjectFinishedGenerating(Project project) {
    }

    public void ProjectItemFinishedGenerating(ProjectItem projectItem) {
    }

    public void RunFinished() {
    }

    public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams) {
      if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
        replacementsDictionary.Add("$problemName$", form.ProblemName);
        replacementsDictionary.Add("$problemDescription$", form.ProblemDescription);
        replacementsDictionary.Add("$problemTypeImplementation$", form.ProblemTypeImplementation);
        replacementsDictionary.Add("$problemSpecificParameterProperties$", form.ProblemSpecificParameterProperties);
        replacementsDictionary.Add("$problemSpecificProperties$", form.ProblemSpecificProperties);
        replacementsDictionary.Add("$problemSpecificParameterInitializers$", form.ProblemSpecificParameterInitializers);
        replacementsDictionary.Add("$parameterProperties$", form.ParameterProperties);
        replacementsDictionary.Add("$properties$", form.Properties);
        replacementsDictionary.Add("$parameterInitializers$", form.ParameterInitializers);
        shouldAddItem = true;
      } else shouldAddItem = false;
    }

    public bool ShouldAddProjectItem(string filePath) {
      return shouldAddItem;
    }
  }
}
