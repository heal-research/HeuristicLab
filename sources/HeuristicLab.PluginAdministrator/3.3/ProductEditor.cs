using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using PluginDeploymentService = HeuristicLab.PluginInfrastructure.Advanced.DeploymentService;
using System.ServiceModel;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.DeploymentService.AdminClient {
  public partial class ProductEditor : HeuristicLab.MainForm.WindowsForms.View {
    private BackgroundWorker refreshProductsWorker;
    private BackgroundWorker uploadChangedProductsWorker;
    private List<PluginDeploymentService.ProductDescription> products;
    private List<PluginDeploymentService.PluginDescription> plugins;
    private HashSet<PluginDeploymentService.ProductDescription> dirtyProducts;

    public ProductEditor() {
      InitializeComponent();
      Caption = "Products";

      productImageList.Images.Add(HeuristicLab.Common.Resources.VS2008ImageLibrary.Assembly);
      productImageList.Images.Add(HeuristicLab.Common.Resources.VS2008ImageLibrary.ArrowUp);
      pluginImageList.Images.Add(HeuristicLab.Common.Resources.VS2008ImageLibrary.Assembly);

      dirtyProducts = new HashSet<PluginDeploymentService.ProductDescription>();
      refreshProductsWorker = new BackgroundWorker();
      refreshProductsWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(refreshProductsWorker_RunWorkerCompleted);
      refreshProductsWorker.DoWork += new DoWorkEventHandler(refreshProductsWorker_DoWork);

      uploadChangedProductsWorker = new BackgroundWorker();
      uploadChangedProductsWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(uploadChangedProductsWorker_RunWorkerCompleted);
      uploadChangedProductsWorker.DoWork += new DoWorkEventHandler(uploadChangedProductsWorker_DoWork);
    }

    #region event handlers for upload products background worker
    private void uploadChangedProductsWorker_DoWork(object sender, DoWorkEventArgs e) {
      var products = (IEnumerable<PluginDeploymentService.ProductDescription>)e.Argument;
      var adminClient = PluginDeploymentService.AdminClientFactory.CreateClient();
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
      var updateClient = PluginDeploymentService.UpdateClientFactory.CreateClient();
      try {
        e.Result = new object[] { updateClient.GetProducts(), updateClient.GetPlugins() };
      }
      catch (FaultException) {
        e.Cancel = true;
      }
    }

    private void refreshProductsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      this.products = new List<PluginDeploymentService.ProductDescription>(
        (PluginDeploymentService.ProductDescription[])((object[])e.Result)[0]);
      this.plugins = new List<PluginDeploymentService.PluginDescription>(
        (PluginDeploymentService.PluginDescription[])((object[])e.Result)[1]);

      UpdateProductsList();
      dirtyProducts.Clear();

      Cursor = Cursors.Default;
      SetControlsEnabled(true);
    }
    #endregion

    private void UpdateProductsList() {
      productsListView.Items.Clear();
      foreach (var prodDesc in products) {
        productsListView.Items.Add(CreateListViewItem(prodDesc));
      }
    }

    private void productsListBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (productsListView.SelectedItems.Count == 0) return;
      PluginDeploymentService.ProductDescription activeProduct = (PluginDeploymentService.ProductDescription)((ListViewItem)productsListView.SelectedItems[0]).Tag;
      UpdateProductDetails(activeProduct);
    }

    private void UpdateProductDetails(PluginDeploymentService.ProductDescription activeProduct) {
      nameTextBox.Text = activeProduct.Name;
      versionTextBox.Text = activeProduct.Version.ToString();

      UpdatePluginsListView();
    }

    private ListViewItem CreateListViewItem(PluginDeploymentService.ProductDescription productDescription) {
      ListViewItem item = new ListViewItem(new string[] { productDescription.Name, productDescription.Version.ToString() });
      item.Tag = productDescription;
      item.ImageIndex = 0;
      return item;
    }

    private void SetControlsEnabled(bool enabled) {
      saveButton.Enabled = enabled;
      refreshButton.Enabled = enabled;
      newProductButton.Enabled = enabled;
      splitContainer.Enabled = enabled;
    }

    #region button event handlers
    private void newProductButton_Click(object sender, EventArgs e) {
      var newProduct = new PluginDeploymentService.ProductDescription("New product", new Version("0.0.0.0"));
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
      PluginDeploymentService.ProductDescription activeProduct = (PluginDeploymentService.ProductDescription)activeItem.Tag;
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
      PluginDeploymentService.ProductDescription activeProduct = (PluginDeploymentService.ProductDescription)activeItem.Tag;
      try {
        var newVersion = new Version(versionTextBox.Text);
        if (activeProduct.Version != newVersion) {
          activeProduct.Version = newVersion;
          activeItem.SubItems[1].Text = versionTextBox.Text;
          errorProvider.SetError(versionTextBox, string.Empty);
          MarkProductDirty(activeProduct);
        }
      }
      catch (OverflowException ex) {
        errorProvider.SetError(versionTextBox, "Invalid value");
      }

      catch (ArgumentException ex) {
        errorProvider.SetError(versionTextBox, "Invalid value");
      }
      catch (FormatException ex) {
        errorProvider.SetError(versionTextBox, "Invalid value");
      }
    }
    #endregion


    #region plugin list view
    private void UpdatePluginsListView() {
      ListViewItem activeItem = (ListViewItem)productsListView.SelectedItems[0];
      PluginDeploymentService.ProductDescription activeProduct = (PluginDeploymentService.ProductDescription)activeItem.Tag;
      pluginListView.Plugins = plugins.OfType<IPluginDescription>();
      foreach (var plugin in activeProduct.Plugins) pluginListView.CheckPlugin(plugin);
    }

    private void pluginListView_ItemChecked(object sender, ItemCheckedEventArgs e) {
      ListViewItem activeItem = (ListViewItem)productsListView.SelectedItems[0];
      PluginDeploymentService.ProductDescription activeProduct = (PluginDeploymentService.ProductDescription)activeItem.Tag;
      activeProduct.Plugins = pluginListView.CheckedPlugins.Cast<PluginDeploymentService.PluginDescription>().ToArray();
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
              let product = item.Tag as PluginDeploymentService.ProductDescription
              where product != null
              where product == activeProduct
              select item).Single();
    }
  }
}
