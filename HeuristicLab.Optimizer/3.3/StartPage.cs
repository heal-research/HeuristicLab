#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.Persistence.Default.Xml;

namespace HeuristicLab.Optimizer {
  [View("Start Page")]
  public partial class StartPage : HeuristicLab.MainForm.WindowsForms.View {
    public StartPage() {
      InitializeComponent();
    }

    protected override void OnInitialized(EventArgs e) {
      base.OnInitialized(e);
      Assembly assembly = Assembly.GetExecutingAssembly();
      AssemblyFileVersionAttribute version = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).
                                             Cast<AssemblyFileVersionAttribute>().FirstOrDefault();
      titleLabel.Text = "HeuristicLab Optimizer";
      if (version != null) titleLabel.Text += " " + version.Version;

      try {
        using (Stream stream = assembly.GetManifestResourceStream(typeof(StartPage), "Documents.FirstSteps.rtf"))
          firstStepsRichTextBox.LoadFile(stream, RichTextBoxStreamType.RichText);
      }
      catch (Exception) { }

      samplesListView.Enabled = false;
      showStartPageCheckBox.Checked = Properties.Settings.Default.ShowStartPage;

      ThreadPool.QueueUserWorkItem(new WaitCallback(LoadSamples));
    }

    protected override void OnClosing(FormClosingEventArgs e) {
      base.OnClosing(e);
      if (e.CloseReason == CloseReason.UserClosing) {
        e.Cancel = true;
        this.Hide();
      }
    }

    private void LoadSamples(object state) {
      Assembly assembly = Assembly.GetExecutingAssembly();
      var samples = assembly.GetManifestResourceNames().Where(x => x.EndsWith(".hl"));
      int count = samples.Count();
      string path = Path.GetTempFileName();

      foreach (string name in samples) {
        try {
          using (Stream stream = assembly.GetManifestResourceStream(name)) {
            WriteStreamToTempFile(stream, path);
            INamedItem item = XmlParser.Deserialize<INamedItem>(path);
            OnSampleLoaded(item, loadingProgressBar.Maximum / count);
          }
        }
        catch (Exception) { }
      }
      OnAllSamplesLoaded();
    }
    private void OnSampleLoaded(INamedItem sample, int progress) {
      if (InvokeRequired)
        Invoke(new Action<INamedItem, int>(OnSampleLoaded), sample, progress);
      else {
        ListViewItem item = new ListViewItem(new string[] { sample.Name, sample.Description });
        item.ToolTipText = sample.ItemName + ": " + sample.ItemDescription;
        samplesListView.SmallImageList.Images.Add(sample.ItemImage);
        item.ImageIndex = samplesListView.SmallImageList.Images.Count - 1;
        item.Tag = sample;
        samplesListView.Items.Add(item);
        loadingProgressBar.Value += progress;
      }
    }
    private void OnAllSamplesLoaded() {
      if (InvokeRequired)
        Invoke(new Action(OnAllSamplesLoaded));
      else {
        samplesListView.Enabled = samplesListView.Items.Count > 0;
        if (samplesListView.Items.Count > 0) {
          for (int i = 0; i < samplesListView.Columns.Count; i++)
            samplesListView.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
        loadingPanel.Visible = false;
      }
    }

    private void firstStepsRichTextBox_LinkClicked(object sender, LinkClickedEventArgs e) {
      System.Diagnostics.Process.Start(e.LinkText);
    }

    private void samplesListView_DoubleClick(object sender, EventArgs e) {
      if (samplesListView.SelectedItems.Count == 1)
        MainFormManager.MainForm.ShowContent((IContent)((IItem)samplesListView.SelectedItems[0].Tag).Clone());
    }
    private void samplesListView_ItemDrag(object sender, ItemDragEventArgs e) {
      ListViewItem listViewItem = (ListViewItem)e.Item;
      IItem item = (IItem)listViewItem.Tag;
      DataObject data = new DataObject();
      data.SetData(HeuristicLab.Common.Constants.DragDropDataFormat, item);
      DragDropEffects result = DoDragDrop(data, DragDropEffects.Copy);
    }

    private void showStartPageCheckBox_CheckedChanged(object sender, EventArgs e) {
      Properties.Settings.Default.ShowStartPage = showStartPageCheckBox.Checked;
      Properties.Settings.Default.Save();
    }

    #region Helpers
    private void WriteStreamToTempFile(Stream stream, string path) {
      using (FileStream output = new FileStream(path, FileMode.Create, FileAccess.Write)) {
        int cnt = 0;
        byte[] buffer = new byte[32 * 1024];
        while ((cnt = stream.Read(buffer, 0, buffer.Length)) != 0)
          output.Write(buffer, 0, cnt);
      }
    }
    #endregion
  }
}
