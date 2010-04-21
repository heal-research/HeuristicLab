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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  internal partial class ProductEditor : UserControl {
    private BackgroundWorker refreshProductsWorker;
    private BackgroundWorker uploadChangedProductsWorker;
    private List<DeploymentService.ProductDescription> products;
    private List<DeploymentService.PluginDescription> plugins;
    private HashSet<DeploymentService.ProductDescription> dirtyProducts;

    public ProductEditor() {
      InitializeComponent();

      productImageList.Images.Add(HeuristicLab.PluginInfrastructure.Resources.Resources.Assembly);
      productImageList.Images.Add(HeuristicLab.PluginInfrastructure.Resources.Resources.ArrowUp);
      pluginImageList.Images.Add(HeuristicLab.PluginInfrastructure.Resources.Resources.Assembly);

      dirtyProducts = new HashSet<DeploymentService.ProductDescription>();
      refreshProductsWorker = new BackgroundWorker();
      refreshProductsWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(refreshProductsWorker_RunWorkerCompleted);
      refreshProductsWorker.DoWork += new DoWorkEventHandler(refreshProductsWorker_DoWork);

      uploadChangedProductsWorker = new BackgroundWorker();
      uploadChangedProductsWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(uploadChangedProductsWorker_RunWorkerCompleted);
      uploadChangedProductsWorker.DoWork += new DoWorkEventHandler(uploadChangedProductsWorker_DoWork);
    }

    #region event handlers for upload products background worker
    private void uploadChangedProductsWorker_DoWork(object sender, DoWorkEventArgs e) {
      var products = (IEnumerable<DeploymentService.ProductDescription>)e.Argument;
      var adminClient = DeploymentService.AdminClientFactory.CreateClient();
      try {
        foreach (var product in products) {
          adminClient.DeployProduct(product);
        }
        e.Cancel = false;
      }
      catch (FaultException) {
      }
    }

    private void uploadChangedProductsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      this.Enabled = true;
      refreshProductsWorker.RunWorkerAsync();
    }
    #endregion

    #region event handlers for refresh products background worker
    private void refreshProductsWorker_DoWork(object sender, DoWorkEventArgs e) {
      var updateClient = DeploymentService.UpdateClientFactory.CreateClient();
      try {
        e.Result = new object[] { updateClient.GetProducts(), updateClient.GetPlugins() };
      }
      catch (FaultException) {
        e.Cancel = true;
      }
    }

    private void refreshProductsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      if (!e.Cancelled && e.Result != null) {
        this.products = new List<DeploymentService.ProductDescription>(
          (DeploymentService.ProductDescription[])((object[])e.Result)[0]);
        this.plugins = new List<DeploymentService.PluginDescription>(
          (DeploymentService.PluginDescription[])((object[])e.Result)[1]);

        UpdateProductsList();
        dirtyProducts.Clear();

        Cursor = Cursors.Default;
        SetControlsEnabled(true);
      }
    }
    #endregion

    private void UpdateProductsList() {
      productsListView.Items.Clear();
      foreach (var prodDesc in products) {
        productsListView.Items.Add(CreateListViewItem(prodDesc));
      }
      foreach (ColumnHeader column in productsListView.Columns)
        column.Width = -1;
    }

    private void productsListBox_SelectedIndexChanged(object sender, EventArgs e) {
      bool productSelected = productsListView.SelectedItems.Count > 0;
      detailsGroupBox.Enabled = productSelected;
      uploadButton.Enabled = productSelected;
      if (productsListView.SelectedItems.Count == 0) {
        ClearProductDetails();
        return;
      }
      DeploymentService.ProductDescription activeProduct = (DeploymentService.ProductDescription)((ListViewItem)productsListView.SelectedItems[0]).Tag;
      UpdateProductDetails(activeProduct);
    }

    private void ClearProductDetails() {
      nameTextBox.Text = string.Empty;
      versionTextBox.Text = string.Empty;
      pluginListView.Plugins = new IPluginDescription[0];
    }

    private void UpdateProductDetails(DeploymentService.ProductDescription activeProduct) {
      nameTextBox.Text = activeProduct.Name;
      versionTextBox.Text = activeProduct.Version.ToString();

      UpdatePluginsListView();
    }

    private ListViewItem CreateListViewItem(DeploymentService.ProductDescription productDescription) {
      ListViewItem item = new ListViewItem(new string[] { productDescription.Name, productDescription.Version.ToString() });
      item.Tag = productDescription;
      item.ImageIndex = 0;
      return item;
    }

    private void SetControlsEnabled(bool enabled) {
      uploadButton.Enabled = enabled;
      refreshButton.Enabled = enabled;
      newProductButton.Enabled = enabled;
      splitContainer.Enabled = enabled;
      productsListView.Enabled = enabled;
      nameLabel.Enabled = enabled;
      nameTextBox.Enabled = enabled;
      versionLabel.Enabled = enabled;
      versionTextBox.Enabled = enabled;
      pluginListView.Enabled = enabled;
    }

    #region button event handlers
    private void newProductButton_Click(object sender, EventArgs e) {
      var newProduct = new DeploymentService.ProductDescription("New product", new Version("0.0.0.0"));
      ListViewItem item = CreateListViewItem(newProduct);
      productsListView.Items.Add(item);
      MarkProductDirty(newProduct);
    }

    private void saveButton_Click(object sender, EventArgs e) {
      this.Enabled = false;
      uploadChangedProductsWorker.RunWorkerAsync(dirtyProducts);
    }
    private void refreshButton_Click(object sender, EventArgs e) {
      SetControlsEnabled(false);
      Cursor = Cursors.AppStarting;
      refreshProductsWorker.RunWorkerAsync();
    }

    #endregion

    #region textbox changed event handlers
    private void nameTextBox_TextChanged(object sender, EventArgs e) {
      ListViewItem activeItem = (ListViewItem)productsListView.SelectedItems[0];
      DeploymentService.ProductDescription activeProduct = (DeploymentService.ProductDescription)activeItem.Tag;
      if (string.IsNullOrEmpty(nameTextBox.Name)) {
        errorProvider.SetError(nameTextBox, "Invalid value");
      } else {
        if (activeProduct.Name != nameTextBox.Text) {
          activeProduct.Name = nameTextBox.Text;
          activeItem.SubItems[0].Text = activeProduct.Name;
          errorProvider.SetError(nameTextBox, string.Empty);
          MarkProductDirty(activeProduct);
        }
      }
    }


    private void versionTextBox_TextChanged(object sender, EventArgs e) {
      ListViewItem activeItem = (ListViewItem)productsListView.SelectedItems[0];
      DeploymentService.ProductDescription activeProduct = (DeploymentService.ProductDescription)activeItem.Tag;
      try {
        var newVersion = new Version(versionTextBox.Text);
        if (activeProduct.Version != newVersion) {
          activeProduct.Version = newVersion;
          activeItem.SubItems[1].Text = versionTextBox.Text;
          errorProvider.SetError(versionTextBox, string.Empty);
          MarkProductDirty(activeProduct);
        }
      }
      catch (OverflowException) {
        errorProvider.SetError(versionTextBox, "Invalid value");
      }

      catch (ArgumentException) {
        errorProvider.SetError(versionTextBox, "Invalid value");
      }
      catch (FormatException) {
        errorProvider.SetError(versionTextBox, "Invalid value");
      }
    }
    #endregion


    #region plugin list view
    private void UpdatePluginsListView() {
      ListViewItem activeItem = (ListViewItem)productsListView.SelectedItems[0];
      DeploymentService.ProductDescription activeProduct = (DeploymentService.ProductDescription)activeItem.Tag;
      pluginListView.Plugins = plugins.OfType<IPluginDescription>();
      foreach (var plugin in activeProduct.Plugins) pluginListView.CheckPlugin(plugin);
    }

    private void pluginListView_ItemChecked(object sender, ItemCheckedEventArgs e) {
      ListViewItem activeItem = (ListViewItem)productsListView.SelectedItems[0];
      DeploymentService.ProductDescription activeProduct = (DeploymentService.ProductDescription)activeItem.Tag;
      activeProduct.Plugins = pluginListView.CheckedPlugins.Cast<DeploymentService.PluginDescription>().ToArray();
      MarkProductDirty(activeProduct);
    }
    #endregion

    private void MarkProductDirty(HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.ProductDescription activeProduct) {
      if (!dirtyProducts.Contains(activeProduct)) {
        dirtyProducts.Add(activeProduct);
        var item = FindItemForProduct(activeProduct);
        item.ImageIndex = 1;
      }
    }
    private ListViewItem FindItemForProduct(HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.ProductDescription activeProduct) {
      return (from item in productsListView.Items.OfType<ListViewItem>()
              let product = item.Tag as DeploymentService.ProductDescription
              where product != null
              where product == activeProduct
              select item).Single();
    }
  }
}
