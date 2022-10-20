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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Windows.Forms;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  internal partial class EditProductsView : InstallationManagerControl {
    private const string RefreshMessage = "Downloading product and plugin information...";
    private const string UploadMessage = "Uploading product and plugin information...";
    private const string DeleteProductMessage = "Deleting product...";

    private BackgroundWorker refreshProductsWorker;
    private BackgroundWorker uploadChangedProductsWorker;
    private BackgroundWorker deleteProductWorker;

    private List<DeploymentService.ProductDescription> products;
    private List<DeploymentService.PluginDescription> plugins;
    private HashSet<DeploymentService.ProductDescription> dirtyProducts;

    public EditProductsView() {
      InitializeComponent();

      productImageList.Images.Add(HeuristicLab.PluginInfrastructure.Resources.Setup_Install);
      productImageList.Images.Add(HeuristicLab.PluginInfrastructure.Resources.ArrowUp);
      pluginImageList.Images.Add(HeuristicLab.PluginInfrastructure.Resources.Plugin);

      dirtyProducts = new HashSet<DeploymentService.ProductDescription>();
      refreshProductsWorker = new BackgroundWorker();
      refreshProductsWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(refreshProductsWorker_RunWorkerCompleted);
      refreshProductsWorker.DoWork += new DoWorkEventHandler(refreshProductsWorker_DoWork);

      uploadChangedProductsWorker = new BackgroundWorker();
      uploadChangedProductsWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(uploadChangedProductsWorker_RunWorkerCompleted);
      uploadChangedProductsWorker.DoWork += new DoWorkEventHandler(uploadChangedProductsWorker_DoWork);

      deleteProductWorker = new BackgroundWorker();
      deleteProductWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(deleteProductWorker_RunWorkerCompleted);
      deleteProductWorker.DoWork += new DoWorkEventHandler(deleteProductWorker_DoWork);
    }

    #region event handlers for delete product background worker
    void deleteProductWorker_DoWork(object sender, DoWorkEventArgs e) {
      var products = (IEnumerable<DeploymentService.ProductDescription>)e.Argument;
      var adminClient = DeploymentService.AdminServiceClientFactory.CreateClient();
      // upload
      try {
        foreach (var product in products) {
          adminClient.DeleteProduct(product);
        }
        adminClient.Close();
      }
      catch (TimeoutException) {
        adminClient.Abort();
        throw;
      }
      catch (FaultException) {
        adminClient.Abort();
        throw;
      }
      catch (CommunicationException) {
        adminClient.Abort();
        throw;
      }
      // refresh      
      var updateClient = DeploymentService.UpdateServiceClientFactory.CreateClient();
      try {
        e.Result = new object[] { updateClient.GetProducts(), updateClient.GetPlugins() };
        updateClient.Close();
      }
      catch (TimeoutException) {
        updateClient.Abort();
        throw;
      }
      catch (FaultException) {
        updateClient.Abort();
        throw;
      }
      catch (CommunicationException) {
        updateClient.Abort();
        throw;
      }
    }

    void deleteProductWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      if (e.Error != null) {
        StatusView.ShowError("Connection Error",
        "There was an error while connecting to the server." + Environment.NewLine +
           "Please check your connection settings and user credentials.");
        this.products.Clear();
        this.plugins.Clear();
      } else {
        this.products = new List<DeploymentService.ProductDescription>(
  (DeploymentService.ProductDescription[])((object[])e.Result)[0]);
        this.plugins = new List<DeploymentService.PluginDescription>(
          (DeploymentService.PluginDescription[])((object[])e.Result)[1]);

        EnableControls();
      }
      UpdateProductsList();
      dirtyProducts.Clear();
      StatusView.HideProgressIndicator();
      StatusView.RemoveMessage(DeleteProductMessage);
      StatusView.UnlockUI();
    }
    #endregion

    #region event handlers for upload products background worker
    private void uploadChangedProductsWorker_DoWork(object sender, DoWorkEventArgs e) {
      var products = (IEnumerable<DeploymentService.ProductDescription>)e.Argument;
      var adminClient = DeploymentService.AdminServiceClientFactory.CreateClient();
      // upload
      try {
        foreach (var product in products) {
          adminClient.DeployProduct(product);
        }
        adminClient.Close();
      }
      catch (TimeoutException) {
        adminClient.Abort();
        throw;
      }
      catch (FaultException) {
        adminClient.Abort();
        throw;
      }
      catch (CommunicationException) {
        adminClient.Abort();
        throw;
      }
      // refresh      
      var updateClient = DeploymentService.UpdateServiceClientFactory.CreateClient();
      try {
        e.Result = new object[] { updateClient.GetProducts(), updateClient.GetPlugins() };
        updateClient.Close();
      }
      catch (TimeoutException) {
        updateClient.Abort();
        throw;
      }
      catch (FaultException) {
        updateClient.Abort();
        throw;
      }
      catch (CommunicationException) {
        updateClient.Abort();
        throw;
      }
    }

    private void uploadChangedProductsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      if (e.Error != null) {
        StatusView.ShowError("Connection Error",
        "There was an error while connecting to the server." + Environment.NewLine +
           "Please check your connection settings and user credentials.");
        this.products.Clear();
        this.plugins.Clear();
      } else {
        this.products = new List<DeploymentService.ProductDescription>(
  (DeploymentService.ProductDescription[])((object[])e.Result)[0]);
        this.plugins = new List<DeploymentService.PluginDescription>(
          (DeploymentService.PluginDescription[])((object[])e.Result)[1]);

      }
      UpdateProductsList();
      dirtyProducts.Clear();
      EnableControls();
      StatusView.HideProgressIndicator();
      StatusView.RemoveMessage(UploadMessage);
      StatusView.UnlockUI();
    }
    #endregion

    #region event handlers for refresh products background worker
    private void refreshProductsWorker_DoWork(object sender, DoWorkEventArgs e) {
      var updateClient = DeploymentService.UpdateServiceClientFactory.CreateClient();
      try {
        e.Result = new object[] { updateClient.GetProducts(), updateClient.GetPlugins() };
        updateClient.Close();
      }
      catch (TimeoutException) {
        updateClient.Abort();
        throw;
      }
      catch (FaultException) {
        updateClient.Abort();
        throw;
      }
      catch (CommunicationException) {
        updateClient.Abort();
        throw;
      }
    }

    private void refreshProductsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      if (e.Error != null) {
        StatusView.ShowError("Connection Error",
          "There was an error while connecting to the server." + Environment.NewLine +
                   "Please check your connection settings and user credentials.");
        this.products.Clear();
        this.plugins.Clear();

      } else {
        this.products = new List<DeploymentService.ProductDescription>(
          (DeploymentService.ProductDescription[])((object[])e.Result)[0]);
        this.plugins = new List<DeploymentService.PluginDescription>(
          (DeploymentService.PluginDescription[])((object[])e.Result)[1]);
      }
      UpdateProductsList();
      dirtyProducts.Clear();
      EnableControls();
      StatusView.HideProgressIndicator();
      StatusView.RemoveMessage(RefreshMessage);
      StatusView.UnlockUI();
    }
    #endregion

    private void UpdateProductsList() {
      productsListView.SelectedItems.Clear();
      productsListView.Items.Clear();
      foreach (var prodDesc in products) {
        productsListView.Items.Add(CreateListViewItem(prodDesc));
      }
      Util.ResizeColumns(productsListView.Columns.OfType<ColumnHeader>());
    }

    private void productsListBox_SelectedIndexChanged(object sender, EventArgs e) {
      bool productSelected = productsListView.SelectedItems.Count > 0;
      detailsGroupBox.Enabled = productSelected;
      UpdateProductButtons();
      if (productSelected) {
        DeploymentService.ProductDescription activeProduct = (DeploymentService.ProductDescription)((ListViewItem)productsListView.SelectedItems[0]).Tag;
        nameTextBox.Text = activeProduct.Name;
        versionTextBox.Text = activeProduct.Version.ToString();

        // populate plugins list view
        pluginListView.SuppressItemCheckedEvents = true;
        foreach (var plugin in plugins.OfType<IPluginDescription>()) {
          pluginListView.Items.Add(CreateListViewItem(plugin));
        }
        pluginListView.SuppressItemCheckedEvents = false;
        foreach (var plugin in activeProduct.Plugins) {
          pluginListView.CheckItems(FindItemsForPlugin(plugin));
        }
      } else {
        nameTextBox.Text = string.Empty;
        versionTextBox.Text = string.Empty;
        pluginListView.Items.Clear();
      }
      Util.ResizeColumns(pluginListView.Columns.OfType<ColumnHeader>());
    }

    private void UpdateProductButtons() {
      uploadButton.Enabled = dirtyProducts.Count > 0;
      if (productsListView.SelectedItems.Count > 0) {
        var selectedProduct = (DeploymentService.ProductDescription)productsListView.SelectedItems[0].Tag;
        deleteProductButton.Enabled = !dirtyProducts.Contains(selectedProduct);
      } else {
        deleteProductButton.Enabled = false;
      }
    }


    #region button event handlers
    private void newProductButton_Click(object sender, EventArgs e) {
      var newProduct = new DeploymentService.ProductDescription("New product", new Version("0.0.0.0"));
      products.Add(newProduct);
      UpdateProductsList();
      MarkProductDirty(newProduct);
    }

    private void saveButton_Click(object sender, EventArgs e) {
      StatusView.LockUI();
      StatusView.ShowProgressIndicator();
      StatusView.ShowMessage(UploadMessage);
      uploadChangedProductsWorker.RunWorkerAsync(dirtyProducts);
    }
    private void refreshButton_Click(object sender, EventArgs e) {
      StatusView.LockUI();
      StatusView.ShowProgressIndicator();
      StatusView.ShowMessage(RefreshMessage);
      refreshProductsWorker.RunWorkerAsync();
    }
    private void deleteProductButton_Click(object sender, EventArgs e) {
      StatusView.LockUI();
      StatusView.ShowProgressIndicator();
      StatusView.ShowMessage(DeleteProductMessage);
      var selectedProducts = from item in productsListView.SelectedItems.OfType<ListViewItem>()
                             select (DeploymentService.ProductDescription)item.Tag;
      deleteProductWorker.RunWorkerAsync(selectedProducts.ToList());
    }

    #endregion

    #region textbox changed event handlers
    private void nameTextBox_TextChanged(object sender, EventArgs e) {
      if (productsListView.SelectedItems.Count > 0) {
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
    }


    private void versionTextBox_TextChanged(object sender, EventArgs e) {
      if (productsListView.SelectedItems.Count > 0) {
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
    }
    #endregion


    #region plugin list view
    private void OnItemChecked(ItemCheckedEventArgs e) {
      ListViewItem activeItem = (ListViewItem)productsListView.SelectedItems[0];
      DeploymentService.ProductDescription activeProduct = (DeploymentService.ProductDescription)activeItem.Tag;
      activeProduct.Plugins = (from item in pluginListView.CheckedItems.OfType<ListViewItem>()
                               select (DeploymentService.PluginDescription)item.Tag).ToArray();
      MarkProductDirty(activeProduct);
    }

    private void listView_ItemChecked(object sender, ItemCheckedEventArgs e) {
      List<IPluginDescription> modifiedPlugins = new List<IPluginDescription>();
      if (e.Item.Checked) {
        foreach (ListViewItem item in pluginListView.SelectedItems) {
          var plugin = (IPluginDescription)item.Tag;
          // also check all dependencies
          if (!modifiedPlugins.Contains(plugin))
            modifiedPlugins.Add(plugin);
          foreach (var dep in Util.GetAllDependencies(plugin)) {
            if (!modifiedPlugins.Contains(dep))
              modifiedPlugins.Add(dep);
          }
        }
        pluginListView.CheckItems(modifiedPlugins.Select(x => FindItemsForPlugin(x).Single()));
        OnItemChecked(e);
      } else {
        foreach (ListViewItem item in pluginListView.SelectedItems) {
          var plugin = (IPluginDescription)item.Tag;
          // also uncheck all dependent plugins
          if (!modifiedPlugins.Contains(plugin))
            modifiedPlugins.Add(plugin);
          foreach (var dep in Util.GetAllDependents(plugin, plugins.Cast<IPluginDescription>())) {
            if (!modifiedPlugins.Contains(dep))
              modifiedPlugins.Add(dep);
          }

        }
        pluginListView.UncheckItems(modifiedPlugins.Select(x => FindItemsForPlugin(x).Single()));
        OnItemChecked(e);
      }
    }


    #endregion

    #region helper
    private void MarkProductDirty(HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.ProductDescription activeProduct) {
      if (!dirtyProducts.Contains(activeProduct)) {
        dirtyProducts.Add(activeProduct);
        var item = FindItemForProduct(activeProduct);
        item.ImageIndex = 1;
        UpdateProductButtons();
      }
    }
    private ListViewItem CreateListViewItem(DeploymentService.ProductDescription productDescription) {
      ListViewItem item = new ListViewItem(new string[] { productDescription.Name, productDescription.Version.ToString() });
      item.Tag = productDescription;
      item.ImageIndex = 0;
      return item;
    }

    private ListViewItem CreateListViewItem(IPluginDescription plugin) {
      ListViewItem item = new ListViewItem(new string[] { plugin.Name, plugin.Version.ToString(), 
          string.Empty, plugin.Description });
      item.Tag = plugin;
      item.ImageIndex = 0;
      item.Checked = false;
      return item;
    }

    private ListViewItem FindItemForProduct(HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.ProductDescription activeProduct) {
      return (from item in productsListView.Items.OfType<ListViewItem>()
              let product = item.Tag as DeploymentService.ProductDescription
              where product != null
              where product == activeProduct
              select item).Single();
    }
    private IEnumerable<ListViewItem> FindItemsForPlugin(IPluginDescription plugin) {
      return from item in pluginListView.Items.OfType<ListViewItem>()
             let p = item.Tag as IPluginDescription
             where p.Name == plugin.Name
             where p.Version == plugin.Version
             select item;
    }

    private void EnableControls() {
      newProductButton.Enabled = true;
      productsListView.Enabled = true;
    }
    #endregion

  }
}
