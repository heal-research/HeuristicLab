#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.OKB.Administration {
  [View("Problem View")]
  [Content(typeof(Problem), true)]
  public partial class ProblemView : NamedOKBItemView {
    private List<Platform> platformComboBoxValues;
    private List<ProblemClass> problemClassComboBoxValues;
    private TypeSelectorDialog typeSelectorDialog;
    private byte[] data;

    public new Problem Content {
      get { return (Problem)base.Content; }
      set { base.Content = value; }
    }

    public ProblemView() {
      InitializeComponent();
    }

    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (components != null) components.Dispose();
        if (typeSelectorDialog != null) typeSelectorDialog.Dispose();
      }
      base.Dispose(disposing);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();

      platformComboBox.SelectedValueChanged -= new EventHandler(platformComboBox_SelectedValueChanged);
      if (AdministrationClient.Instance.Platforms != null)
        platformComboBoxValues = AdministrationClient.Instance.Platforms.ToList();
      platformComboBox.DataSource = platformComboBoxValues;
      platformComboBox.SelectedValueChanged += new EventHandler(platformComboBox_SelectedValueChanged);

      problemClassComboBox.SelectedValueChanged -= new EventHandler(problemClassComboBox_SelectedValueChanged);
      if (AdministrationClient.Instance.ProblemClasses != null)
        problemClassComboBoxValues = AdministrationClient.Instance.ProblemClasses.ToList();
      problemClassComboBox.DataSource = problemClassComboBoxValues;
      problemClassComboBox.SelectedValueChanged += new EventHandler(problemClassComboBox_SelectedValueChanged);

      data = null;
      dataViewHost.Content = null;
      if (Content == null) {
        platformComboBox.SelectedIndex = -1;
        problemClassComboBox.SelectedIndex = -1;
        dataTypeNameTextBox.Text = string.Empty;
        dataTypeTypeNameTextBox.Text = string.Empty;
        problemUserView.Content = null;
        problemUserView.FetchSelectedUsers = null;
      } else {
        platformComboBox.SelectedItem = platformComboBoxValues.FirstOrDefault(p => p.Id == Content.PlatformId);
        problemClassComboBox.SelectedItem = problemClassComboBoxValues.FirstOrDefault(a => a.Id == Content.ProblemClassId);
        dataTypeNameTextBox.Text = Content.DataTypeName;
        dataTypeTypeNameTextBox.Text = Content.DataTypeTypeName;
        problemUserView.Content = Access.AccessClient.Instance;
        problemUserView.FetchSelectedUsers = new Func<List<Guid>>(delegate { return AdministrationClient.GetProblemUsers(Content.Id); });
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      platformComboBox.Enabled = (Content != null) && !ReadOnly;
      problemClassComboBox.Enabled = (Content != null) && !ReadOnly;
      dataTypeGroupBox.Enabled = (Content != null) && !ReadOnly;
      problemUserView.Enabled = (problemUserView.Content != null) && !ReadOnly;
      storeUsersButton.Enabled = (problemUserView.GetCheckedUsers() != null) && !ReadOnly;
      refreshDataButton.Enabled = (Content != null) && (Content.Id != 0);
      storeDataButton.Enabled = ((data != null) || (dataViewHost.Content != null)) && !ReadOnly;
      openFileButton.Enabled = (Content != null) && (Content.Id != 0);
      saveFileButton.Enabled = (data != null) || (dataViewHost.Content != null);
      noViewAvailableLabel.Visible = dataViewHost.Content == null;

      bool isHL33Platform = platformComboBox.Text == "HeuristicLab 3.3";
      dataTypeNameTextBox.ReadOnly = isHL33Platform;
      dataTypeTypeNameTextBox.ReadOnly = isHL33Platform;
      newDataButton.Enabled = isHL33Platform && (Content.Id != 0) && !ReadOnly;
    }

    #region Content Events
    protected override void OnContentPropertyChanged(string propertyName) {
      switch (propertyName) {
        case "Id":
          SetEnabledStateOfControls();
          break;
        case "PlatformId":
          platformComboBox.SelectedItem = platformComboBoxValues.FirstOrDefault(p => p.Id == Content.PlatformId);
          SetEnabledStateOfControls();
          break;
        case "ProblemClassId":
          problemClassComboBox.SelectedItem = problemClassComboBoxValues.FirstOrDefault(a => a.Id == Content.ProblemClassId);
          break;
        case "DataTypeName":
          dataTypeNameTextBox.Text = Content.DataTypeName;
          break;
        case "DataTypeTypeName":
          dataTypeTypeNameTextBox.Text = Content.DataTypeTypeName;
          break;
      }
    }
    #endregion

    #region Control Events
    private void platformComboBox_SelectedValueChanged(object sender, System.EventArgs e) {
      if (Content != null) {
        Platform selected = platformComboBox.SelectedItem as Platform;
        if (selected != null) Content.PlatformId = selected.Id;
      }
    }
    private void problemClassComboBox_SelectedValueChanged(object sender, System.EventArgs e) {
      if (Content != null) {
        ProblemClass selected = problemClassComboBox.SelectedItem as ProblemClass;
        if (selected != null) Content.ProblemClassId = selected.Id;
      }
    }

    private void dataTypeNameTextBox_TextChanged(object sender, EventArgs e) {
      if (dataTypeNameTextBox.Text != Content.DataTypeName)
        Content.DataTypeName = dataTypeNameTextBox.Text;
    }
    private void dataTypeTypeNameTextBox_TextChanged(object sender, EventArgs e) {
      if (dataTypeTypeNameTextBox.Text != Content.DataTypeTypeName)
        Content.DataTypeTypeName = dataTypeTypeNameTextBox.Text;
    }

    private void storeUsersButton_Click(object sender, System.EventArgs e) {
      try {
        AdministrationClient.UpdateProblemUsers(Content.Id, problemUserView.GetCheckedUsers().Select(x => x.Id).ToList());
        storeUsersButton.Enabled = false;
      }
      catch (Exception ex) {
        ErrorHandling.ShowErrorDialog(this, "Store authorized users and groups failed.", ex);
      }
    }
    private void problemUserView_SelectedUsersChanged(object sender, EventArgs e) {
      storeUsersButton.Enabled = !ReadOnly;
    }


    private void refreshDataButton_Click(object sender, EventArgs e) {
      CallAsync(
        () => {
          data = null;
          dataViewHost.Content = null;
          data = AdministrationClient.GetProblemData(Content.Id);
          if (data != null) {
            using (MemoryStream stream = new MemoryStream(data)) {
              try {
                dataViewHost.Content = XmlParser.Deserialize<IContent>(stream);
              }
              catch (Exception) { }
              stream.Close();
            }
          }
        },
        "Refresh problem data failed.",
        () => SetEnabledStateOfControls()
      );
    }
    private void storeDataButton_Click(object sender, EventArgs e) {
      CallAsync(
        () => {
          if (dataViewHost.Content != null) {
            using (MemoryStream stream = new MemoryStream()) {
              XmlGenerator.Serialize(dataViewHost.Content, stream);
              stream.Close();
              data = stream.ToArray();
            }
          }
          AdministrationClient.UpdateProblemData(Content.Id, data);
        },
        "Store problem data failed.",
        null
      );
    }
    private void newDataButton_Click(object sender, EventArgs e) {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
        typeSelectorDialog.Caption = "Select Problem";
        typeSelectorDialog.TypeSelector.Caption = "Available Problems";
        typeSelectorDialog.TypeSelector.Configure(typeof(IProblem), false, true);
      }
      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          Content.DataTypeName = typeSelectorDialog.TypeSelector.SelectedType.Name;
          Content.DataTypeTypeName = typeSelectorDialog.TypeSelector.SelectedType.AssemblyQualifiedName;
          data = null;
          dataViewHost.Content = (IContent)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, "Create new problem data failed.", ex);
        }
        SetEnabledStateOfControls();
      }
    }
    private void openFileButton_Click(object sender, EventArgs e) {
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        CallAsync(
          () => {
            IContent problem = null;
            try {
              problem = XmlParser.Deserialize<IContent>(openFileDialog.FileName);
            }
            catch (Exception) { }

            if (problem != null) {
              Content.DataTypeName = problem.GetType().Name;
              Content.DataTypeTypeName = problem.GetType().AssemblyQualifiedName;
              data = null;
            } else {
              using (FileStream stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read)) {
                data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                stream.Close();
              }
            }
            dataViewHost.Content = problem;
          },
          "Save problem data into file failed.",
          () => SetEnabledStateOfControls()
        );
      }
    }
    private void saveFileButton_Click(object sender, EventArgs e) {
      if (saveFileDialog.ShowDialog(this) == DialogResult.OK) {
        CallAsync(
          () => {
            if (dataViewHost.Content != null) {
              XmlGenerator.Serialize(dataViewHost.Content, saveFileDialog.FileName);
            } else {
              using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write)) {
                stream.Write(data, 0, data.Length);
                stream.Close();
              }
            }
          },
          "Save problem data into file failed.",
          null
        );
      }
    }
    #endregion

    #region Helpers
    private void CallAsync(Action call, string errorMessage, Action continueWith) {
      BeginAsyncCall();
      call.BeginInvoke(delegate(IAsyncResult result) {
        Exception exception = null;
        try {
          call.EndInvoke(result);
        }
        catch (Exception ex) {
          exception = ex;
        }
        EndAsyncCall(errorMessage, exception, continueWith);
      }, null);
    }
    private void BeginAsyncCall() {
      if (InvokeRequired)
        Invoke(new Action(BeginAsyncCall));
      else {
        Cursor = Cursors.AppStarting;
        Enabled = false;
      }
    }
    private void EndAsyncCall(string errorMessage, Exception exception, Action continueWith) {
      if (InvokeRequired)
        Invoke(new Action<string, Exception, Action>(EndAsyncCall), errorMessage, exception, continueWith);
      else {
        Cursor = Cursors.Default;
        Enabled = true;
        if (exception != null) ErrorHandling.ShowErrorDialog(this, errorMessage, exception);
        if (continueWith != null) continueWith();
      }
    }
    #endregion
  }
}
