using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  public partial class RemotePluginInstaller : UserControl {
    private ListViewGroup newPluginsGroup;
    private ListViewGroup productsGroup;
    private ListViewGroup allPluginsGroup;

    public event ItemCheckedEventHandler ItemChecked;

    public RemotePluginInstaller() {
      InitializeComponent();

      imageListForRemoteItems.Images.Add(HeuristicLab.PluginInfrastructure.Resources.Resources.Assembly);
      imageListForRemoteItems.Images.Add(HeuristicLab.PluginInfrastructure.Resources.Resources.Install);

      newPluginsGroup = remotePluginsListView.Groups["newPluginsGroup"];
      productsGroup = remotePluginsListView.Groups["productsGroup"];
      allPluginsGroup = remotePluginsListView.Groups["allPluginsGroup"];
    }

    private IEnumerable<DeploymentService.ProductDescription> products;
    public IEnumerable<DeploymentService.ProductDescription> Products {
      get { return products ?? Enumerable.Empty<DeploymentService.ProductDescription>(); }
      set {
        if (value != this.products) {
          this.products = value;
          UpdateControl();
        }
      }
    }

    private IEnumerable<IPluginDescription> plugins;
    public IEnumerable<IPluginDescription> AllPlugins {
      get { return plugins ?? Enumerable.Empty<IPluginDescription>(); }
      set {
        if (value != this.plugins) {
          this.plugins = value;
          UpdateControl();
        }
      }
    }

    private IEnumerable<IPluginDescription> newPlugins;
    public IEnumerable<IPluginDescription> NewPlugins {
      get { return newPlugins ?? Enumerable.Empty<IPluginDescription>(); }
      set {
        if (value != this.newPlugins) {
          this.newPlugins = value;
          UpdateControl();
        }
      }
    }

    public IEnumerable<IPluginDescription> CheckedPlugins {
      get {
        return (from item in remotePluginsListView.Items.OfType<ListViewItem>()
                where item.Checked
                let plugin = item.Tag as IPluginDescription
                where plugin != null
                select plugin).ToList();
      }
    }

    private void UpdateControl() {
      ClearListView();
      remotePluginsListView.SuppressItemCheckedEvents = true;
      foreach (var newPlugin in NewPlugins) {
        var item = CreateListViewItem(newPlugin);
        item.Group = newPluginsGroup;
        remotePluginsListView.Items.Add(item);
      }

      foreach (var product in Products) {
        var item = CreateListViewItem(product);
        item.Group = productsGroup;
        remotePluginsListView.Items.Add(item);
      }

      foreach (var plugin in AllPlugins) {
        var item = CreateListViewItem(plugin);
        item.Group = allPluginsGroup;
        remotePluginsListView.Items.Add(item);
      }
      remotePluginsListView.SuppressItemCheckedEvents = false;
    }

    private void ClearListView() {
      List<ListViewItem> itemsToDelete = new List<ListViewItem>(remotePluginsListView.Items.OfType<ListViewItem>());
      itemsToDelete.ForEach(item => remotePluginsListView.Items.Remove(item));
    }

    private ListViewItem CreateListViewItem(DeploymentService.ProductDescription product) {
      ListViewItem item = new ListViewItem(new string[] { product.Name, product.Version.ToString(), string.Empty });
      item.Tag = product;
      return item;
    }

    private ListViewItem CreateListViewItem(IPluginDescription plugin) {
      ListViewItem item = new ListViewItem(new string[] { plugin.Name, plugin.Version.ToString(), plugin.Description });
      item.Tag = plugin;
      return item;
    }

    #region item checked event handler
    private void remotePluginsListView_ItemChecked(object sender, ItemCheckedEventArgs e) {
      foreach (ListViewItem item in remotePluginsListView.SelectedItems) {
        // dispatch by check state and type of item (product/plugin)
        IPluginDescription plugin = item.Tag as IPluginDescription;
        if (plugin != null)
          if (e.Item.Checked)
            HandlePluginChecked(plugin);
          else
            HandlePluginUnchecked(plugin);
        else {
          DeploymentService.ProductDescription product = item.Tag as DeploymentService.ProductDescription;
          if (product != null)
            if (e.Item.Checked)
              HandleProductChecked(product);
            else
              HandleProductUnchecked(product);
        }
      }
      OnItemChecked(e);
    }

    private void HandleProductUnchecked(HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.ProductDescription product) {
      // also uncheck the plugins of the product
      List<ListViewItem> modifiedItems = new List<ListViewItem>();
      modifiedItems.Add(FindItemForProduct(product));
      foreach (var plugin in product.Plugins) {
        var item = FindItemForPlugin(plugin);
        if (item != null && item.Checked)
          modifiedItems.Add(item);
      }
      remotePluginsListView.UncheckItems(modifiedItems);
    }

    private void HandleProductChecked(HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.ProductDescription product) {
      // also check all plugins of the product
      List<ListViewItem> modifiedItems = new List<ListViewItem>();
      modifiedItems.Add(FindItemForProduct(product));
      foreach (var plugin in product.Plugins) {
        var item = FindItemForPlugin(plugin);
        if (item != null && !item.Checked) {
          modifiedItems.Add(item);
        }
      }
      remotePluginsListView.CheckItems(modifiedItems);
    }

    private void HandlePluginUnchecked(IPluginDescription plugin) {
      // also uncheck all dependent plugins
      List<ListViewItem> modifiedItems = new List<ListViewItem>();
      modifiedItems.Add(FindItemForPlugin(plugin));
      var dependentPlugins = from otherPlugin in plugins
                             where otherPlugin.Dependencies.Any(dep => dep.Name == plugin.Name && dep.Version == plugin.Version)
                             select otherPlugin;
      foreach (var dependentPlugin in dependentPlugins) {
        var item = FindItemForPlugin(dependentPlugin);
        if (item != null && item.Checked) {
          modifiedItems.Add(item);
        }
      }
      // also uncheck all products containing this plugin
      var dependentProducts = from product in products
                              where product.Plugins.Any(p => p.Name == plugin.Name && p.Version == plugin.Version)
                              select product;
      foreach (var dependentProduct in dependentProducts) {
        var item = FindItemForProduct(dependentProduct);
        if (item != null && item.Checked) {
          modifiedItems.Add(item);
        }
      }
      remotePluginsListView.UncheckItems(modifiedItems);
    }

    private void HandlePluginChecked(IPluginDescription plugin) {
      // also check all dependencies
      List<ListViewItem> modifiedItems = new List<ListViewItem>();
      modifiedItems.Add(FindItemForPlugin(plugin));
      foreach (var dep in plugin.Dependencies) {
        var item = FindItemForPlugin(dep);
        if (item != null && !item.Checked) {
          modifiedItems.Add(item);
        }
      }
      remotePluginsListView.CheckItems(modifiedItems);
    }

    private void OnItemChecked(ItemCheckedEventArgs e) {
      if (ItemChecked != null) ItemChecked(this, e);
    }
    #endregion

    #region helper methods
    private ListViewItem FindItemForPlugin(IPluginDescription plugin) {
      return (from item in remotePluginsListView.Items.OfType<ListViewItem>()
              let otherPlugin = item.Tag as IPluginDescription
              where otherPlugin != null && otherPlugin.Name == plugin.Name && otherPlugin.Version == plugin.Version
              select item).SingleOrDefault();
    }

    private ListViewItem FindItemForProduct(DeploymentService.ProductDescription product) {
      return (from item in remotePluginsListView.Items.OfType<ListViewItem>()
              let otherProduct = item.Tag as DeploymentService.ProductDescription
              where otherProduct != null && otherProduct.Name == product.Name && otherProduct.Version == product.Version
              select item).SingleOrDefault();
    }

    #endregion
  }
}
