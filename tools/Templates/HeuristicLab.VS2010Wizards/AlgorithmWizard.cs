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
  public class AlgorithmWizard : IWizard {
    private bool shouldAddItem;
    private AlgorithmWizardForm form;

    public AlgorithmWizard() {
      form = new AlgorithmWizardForm();
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
        replacementsDictionary.Add("$algorithmName$", form.AlgorithmName);
        replacementsDictionary.Add("$algorithmDescription$", form.AlgorithmDescription);
        string problemType = @"public override Type ProblemType {
      get { return typeof(IMultiObjectiveProblem); }
    }
    public new IMultiObjectiveProblem Problem {
      get { return (IMultiObjectiveProblem)base.Problem; }
      set { base.Problem = value; }
    }";
        replacementsDictionary.Add("$problemType$", problemType);
        if (!form.IsMultiObjective)
          replacementsDictionary["$problemType$"] = problemType.Replace("Multi", "Single");
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
