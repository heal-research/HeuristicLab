#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimization.Views {
  [View("MultiEncoding View")]
  [Content(typeof(MultiEncoding), IsDefaultView = true)]
  public sealed partial class MultiEncodingView : ParameterizedNamedItemView {

    public new MultiEncoding Content {
      get { return (MultiEncoding)base.Content; }
      set { base.Content = value; }
    }

    public MultiEncodingView() {
      InitializeComponent();
      addEncodingButton.Text = string.Empty;
      addEncodingButton.Image = VSImageLibrary.Add;
      removeEncodingButton.Text = string.Empty;
      removeEncodingButton.Image = VSImageLibrary.Remove;
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.EncodingsChanged -= ContentOnEncodingsChanged;
    }

    protected override void RegisterContentEvents() {
      Content.EncodingsChanged += ContentOnEncodingsChanged;
      base.RegisterContentEvents();
    }

    private void ContentOnEncodingsChanged(object sender, EventArgs eventArgs) {
      RecreateEncodingsList();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      encodingDetailViewHost.Content = null;
      RecreateEncodingsList();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      addEncodingButton.Enabled = !Locked && !ReadOnly && Content != null;
      removeEncodingButton.Enabled = !Locked && !ReadOnly && Content != null
                                     && encodingsListView.SelectedItems.Count > 0;
    }

    private void addEncodingButton_Click(object sender, EventArgs e) {
      using (var dialog = new CreateNewSingleEncodingDialog()) {
        dialog.ForbiddenNames = Content.Encodings.Select(x => x.Name).Concat(new[] { Content.Name });
        if (dialog.ShowDialog() == DialogResult.OK) {
          IEncoding encoding;
          try {
            encoding = (IEncoding)Activator.CreateInstance(dialog.EncodingType, dialog.EncodingName);
          } catch (MissingMethodException mmex) {
            PluginInfrastructure.ErrorHandling.ShowErrorDialog("The encoding must have a constructor that takes the name as a single string argument", mmex);
            return;
          } catch (TargetInvocationException tiex) {
            PluginInfrastructure.ErrorHandling.ShowErrorDialog("The encoding could not be created due to an error in the constructor.", tiex);
            return;
          } catch (MethodAccessException maex) {
            PluginInfrastructure.ErrorHandling.ShowErrorDialog("The encoding's string constructor is not public.", maex);
            return;
          }
          Content.Add(encoding);
        }
      }
    }

    private void removeEncodingButton_Click(object sender, EventArgs e) {
      var encoding = GetSelectedEncoding();
      var success = Content.Remove(encoding);
      encodingDetailViewHost.Content = null;
      if (!success) MessageBox.Show("Could not remove encoding.", "Error while removing encoding.", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void encodingsListView_SelectedIndexChanged(object sender, EventArgs e) {
      if (encodingsListView.SelectedItems.Count > 0) {
        var encoding = GetSelectedEncoding();
        encodingDetailViewHost.Content = encoding;
      } else encodingDetailViewHost.Content = null;
      SetEnabledStateOfControls();
    }

    private IEncoding GetSelectedEncoding() {
      var selected = encodingsListView.SelectedItems[0];
      var encodingName = selected.Text;
      var encoding = Content.Encodings.Single(x => x.Name == encodingName);
      return encoding;
    }

    private void RecreateEncodingsList() {
      encodingsListView.SelectedIndices.Clear();
      encodingsListView.Items.Clear();
      if (encodingsListView.SmallImageList != null)
        foreach (Image img in encodingsListView.SmallImageList.Images) img.Dispose();
      encodingsListView.SmallImageList = new ImageList();
      if (Content != null) {
        foreach (var enc in Content.Encodings)
          CreateListViewItem(enc);
      }
    }

    private void CreateListViewItem(IEncoding enc) {
      encodingsListView.SmallImageList.Images.Add(enc.ItemImage);
      var item = new ListViewItem() {
        ImageIndex = encodingsListView.SmallImageList.Images.Count - 1,
        Text = enc.Name
      };
      item.SubItems.Add(new ListViewItem.ListViewSubItem() {
        Text = enc.GetType().Name
      });
      encodingsListView.Items.Add(item);
    }
  }
}
