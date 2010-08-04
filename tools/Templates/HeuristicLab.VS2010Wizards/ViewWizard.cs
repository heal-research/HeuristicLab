using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;

namespace HeuristicLab.VS2010Wizards {
  public class ViewWizard : IWizard {
    ViewWizardForm form;
    bool shouldAddItem;

    #region IWizard Members

    public void BeforeOpeningFile(ProjectItem projectItem) {
    }

    public void ProjectFinishedGenerating(Project project) {
    }

    public void ProjectItemFinishedGenerating(ProjectItem projectItem) {
    }

    public void RunFinished() {
    }

    public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams) {
      form = new ViewWizardForm();
      if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
        replacementsDictionary.Add("$baseClass$", form.BaseClass);
        replacementsDictionary.Add("$viewContentType$", form.ViewContentType);
        replacementsDictionary.Add("$isDefaultView$", form.IsDefaultView.ToString().ToLower());
        shouldAddItem = true;
      } else shouldAddItem = false;
    }

    public bool ShouldAddProjectItem(string filePath) {
      return shouldAddItem;
    }

    #endregion
  }
}
