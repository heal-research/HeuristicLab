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

using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Clients.Access.Views {
  [View("Client View")]
  [Content(typeof(Client), true)]
  public partial class ClientView : ItemView {
    public new Client Content {
      get { return (Client)base.Content; }
      set { base.Content = value; }
    }

    public ClientView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();

      if (Content == null) {
        txtClientConfiguration.Text = string.Empty;
        txtClientType.Text = string.Empty;
        txtDescription.Text = string.Empty;
        txtMemory.Text = string.Empty;
        txtName.Text = string.Empty;
        txtNumberOfCores.Text = string.Empty;
        txtPerformanceValue.Text = string.Empty;
        txtProcessor.Text = string.Empty;
        txtTimestamp.Text = string.Empty;
        txtVersion.Text = string.Empty;
        txtId.Text = string.Empty;
        txtOS.Text = string.Empty;
      } else {
        if (Content.ClientConfiguration != null) {
          txtClientConfiguration.Text = Content.ClientConfiguration.Hash;
        }
        if (Content.ClientType != null) {
          txtClientType.Text = Content.ClientType.Name;
        }
        if (Content.OperatingSystem != null) {
          txtOS.Text = Content.OperatingSystem.Name;
        }
        txtDescription.Text = Content.Description;
        txtMemory.Text = Content.MemorySize.ToString();
        txtName.Text = Content.Name;
        txtNumberOfCores.Text = Content.NumberOfCores.ToString();
        txtPerformanceValue.Text = Content.PerformanceValue.ToString();
        txtProcessor.Text = Content.ProcessorType;
        txtTimestamp.Text = Content.Timestamp.ToShortDateString();
        txtVersion.Text = Content.HeuristicLabVersion;
        txtId.Text = Content.Id.ToString();
      }
    }

    public void StartProgressView() {
      var message = "Downloading client information. Please be patient.";
      MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().AddOperationProgressToView(this, message);
    }

    public void FinishProgressView() {
      MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().RemoveOperationProgressFromView(this);
    }
  }
}
