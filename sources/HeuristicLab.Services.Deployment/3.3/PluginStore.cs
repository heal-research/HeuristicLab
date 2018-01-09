#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using HeuristicLab.Services.Deployment.DataAccess;

namespace HeuristicLab.Services.Deployment {
  public class PluginStore {

    public PluginStore() {
    }

    #region context creating members
    public IEnumerable<ProductDescription> Products {
      get {
        using (var ctx = new DeploymentDataContext()) {
          return (from p in ctx.Products
                  let plugins = from pair in ctx.ProductPlugins
                                from plugin in ctx.Plugins
                                where pair.ProductId == p.Id
                                where plugin.Id == pair.PluginId
                                select plugin
                  select MakeProductDescription(ctx, p, plugins.ToList())).ToList();
        }
      }
    }

    public IEnumerable<PluginDescription> Plugins {
      get {
        using (var ctx = new DeploymentDataContext()) {
          return (from plugin in ctx.Plugins
                  select MakePluginDescription(ctx, plugin)).ToList();
        }
      }
    }

    public byte[] PluginFile(PluginDescription pluginDescription) {
      using (var ctx = new DeploymentDataContext()) {
        return GetExistingPlugin(ctx, pluginDescription.Name, pluginDescription.Version).PluginPackage.Data.ToArray();
      }
    }

    public void Persist(PluginDescription pluginDescription, byte[] pluginPackage) {
      using (var ctx = new DeploymentDataContext()) {
        try {
          using (var transaction = new TransactionScope()) {
            Plugin pluginEntity = InsertOrUpdatePlugin(ctx, pluginDescription);
            if (pluginEntity.PluginPackage == null) {
              // insert
              pluginEntity.PluginPackage = MakePluginPackage(pluginEntity, pluginPackage);
            } else {
              // update
              pluginEntity.PluginPackage.Data = pluginPackage;
            }
            ctx.SubmitChanges();
            transaction.Complete();
          }
        }
        catch (SqlException ex) {
          throw new ArgumentException("Something went wrong while trying to persist plugin", ex);
        }
        catch (InvalidOperationException ex) {
          throw new ArgumentException("Something went wrong while trying to persist plugin", ex);
        }
      }
    }

    public void Persist(ProductDescription productDescription) {
      using (var ctx = new DeploymentDataContext()) {
        try {
          using (var transaction = new TransactionScope()) {
            foreach (var plugin in productDescription.Plugins) {
              var pluginEntity = GetExistingPlugin(ctx, plugin.Name, plugin.Version);
              UpdatePlugin(ctx, pluginEntity, plugin);
            }
            InsertOrUpdateProduct(ctx, productDescription);
            ctx.SubmitChanges();
            transaction.Complete();
          }
        }
        catch (SqlException ex) {
          throw new ArgumentException("Something went wrong while trying to persist product", ex);
        }
        catch (InvalidOperationException ex) {
          throw new ArgumentException("Something went wrong while trying to persist product", ex);
        }
      }
    }
    public void Delete(ProductDescription productDescription) {
      using (var ctx = new DeploymentDataContext()) {
        try {
          using (var transaction = new TransactionScope()) {
            var productEntity = GetExistingProduct(ctx, productDescription.Name, productDescription.Version);

            DeleteProductPlugins(ctx, productEntity);
            ctx.Products.DeleteOnSubmit(productEntity);

            ctx.SubmitChanges();
            transaction.Complete();
          }
        }
        catch (SqlException ex) {
          throw new ArgumentException("Something went wrong while trying to delete product", ex);
        }
        catch (InvalidOperationException ex) {
          throw new ArgumentException("Something went wrong while trying to delete product", ex);
        }
      }
    }

    #endregion

    #region insert/update/delete product
    private void InsertOrUpdateProduct(DeploymentDataContext ctx, ProductDescription product) {
      var productEntity = (from p in ctx.Products
                           where p.Name == product.Name
                           where p.Version == product.Version.ToString()
                           select p).FirstOrDefault() ?? MakeProductFromDescription(product);

      if (productEntity.Id <= 0) {
        ctx.Products.InsertOnSubmit(productEntity);
        ctx.SubmitChanges();
      }

      DeleteProductPlugins(ctx, productEntity);

      foreach (var plugin in product.Plugins) {
        var existingPlugin = GetExistingPlugin(ctx, plugin.Name, plugin.Version);
        ProductPlugin prodPlugin = new ProductPlugin();
        prodPlugin.PluginId = existingPlugin.Id;
        prodPlugin.ProductId = productEntity.Id;
        ctx.ProductPlugins.InsertOnSubmit(prodPlugin);
      }
    }

    private void DeleteProductPlugins(DeploymentDataContext ctx, Product productEntity) {
      var oldPlugins = (from p in ctx.ProductPlugins
                        where p.ProductId == productEntity.Id
                        select p).ToList();
      ctx.ProductPlugins.DeleteAllOnSubmit(oldPlugins);
      ctx.SubmitChanges();
    }
    #endregion

    #region insert/update plugins
    private Plugin InsertOrUpdatePlugin(DeploymentDataContext ctx, PluginDescription pluginDescription) {
      var pluginEntity = (from p in ctx.Plugins
                          where p.Name == pluginDescription.Name
                          where p.Version == pluginDescription.Version.ToString()
                          select p).FirstOrDefault() ?? MakePluginFromDescription(pluginDescription);

      if (pluginEntity.Id <= 0) {
        ctx.Plugins.InsertOnSubmit(pluginEntity);
        ctx.SubmitChanges();
      }

      UpdatePlugin(ctx, pluginEntity, pluginDescription);
      return pluginEntity;
    }

    private void UpdatePlugin(DeploymentDataContext ctx, Plugin pluginEntity, PluginDescription pluginDescription) {
      // update plugin data
      pluginEntity.License = pluginDescription.LicenseText;
      pluginEntity.ContactName = pluginDescription.ContactName;
      pluginEntity.ContactEmail = pluginDescription.ContactEmail;

      // delete cached entry
      if (pluginDescriptions.ContainsKey(pluginEntity.Id)) pluginDescriptions.Remove(pluginEntity.Id);

      DeleteOldDependencies(ctx, pluginEntity);

      foreach (var dependency in pluginDescription.Dependencies) {
        var dependencyEntity = GetExistingPlugin(ctx, dependency.Name, dependency.Version);
        Dependency d = new Dependency();
        d.PluginId = pluginEntity.Id;
        d.DependencyId = dependencyEntity.Id;
        ctx.Dependencies.InsertOnSubmit(d);
      }
    }



    private void DeleteOldDependencies(DeploymentDataContext ctx, Plugin pluginEntity) {
      var oldDependencies = (from dep in ctx.Dependencies
                             where dep.PluginId == pluginEntity.Id
                             select dep).ToList();

      ctx.Dependencies.DeleteAllOnSubmit(oldDependencies);
      ctx.SubmitChanges();
    }
    #endregion

    #region product <-> productDescription transformation
    private ProductDescription MakeProductDescription(DeploymentDataContext ctx, Product p, IEnumerable<Plugin> plugins) {
      var desc = new ProductDescription(p.Name, new Version(p.Version), from plugin in plugins
                                                                        select MakePluginDescription(ctx, plugin));
      return desc;
    }
    private Product MakeProductFromDescription(ProductDescription desc) {
      var product = new Product();
      product.Name = desc.Name;
      product.Version = desc.Version.ToString();
      return product;
    }
    #endregion

    #region plugin <-> pluginDescription transformation
    // cache for plugin descriptions
    private Dictionary<long, PluginDescription> pluginDescriptions = new Dictionary<long, PluginDescription>();
    private PluginDescription MakePluginDescription(DeploymentDataContext ctx, Plugin plugin) {
      if (!pluginDescriptions.ContainsKey(plugin.Id)) {
        // no cached description -> create new
        var desc = new PluginDescription(plugin.Name, new Version(plugin.Version));
        pluginDescriptions[plugin.Id] = desc; // and add to cache

        // fill remaining properties of plugin description
        desc.Dependencies = new List<PluginDescription>(from dep in GetDependencies(ctx, plugin) select MakePluginDescription(ctx, dep));
        desc.ContactEmail = plugin.ContactEmail ?? string.Empty;
        desc.ContactName = plugin.ContactName ?? string.Empty;
        desc.LicenseText = plugin.License ?? string.Empty;
      }
      return pluginDescriptions[plugin.Id];
    }

    private Plugin MakePluginFromDescription(PluginDescription pluginDescription) {
      var plugin = new Plugin();
      plugin.Name = pluginDescription.Name;
      plugin.Version = pluginDescription.Version.ToString();
      plugin.ContactName = pluginDescription.ContactName;
      plugin.ContactEmail = pluginDescription.ContactEmail;
      plugin.License = pluginDescription.LicenseText;
      return plugin;
    }

    private PluginPackage MakePluginPackage(Plugin plugin, byte[] pluginPackage) {
      var package = new PluginPackage();
      package.Data = pluginPackage;
      package.PluginId = plugin.Id;
      return package;
    }

    #endregion

    #region helper queries
    private Plugin GetExistingPlugin(DeploymentDataContext ctx, string name, Version version) {
      return (from p in ctx.Plugins
              where p.Name == name
              where p.Version == version.ToString()
              select p).Single();
    }

    private Product GetExistingProduct(DeploymentDataContext ctx, string name, Version version) {
      return (from p in ctx.Products
              where p.Name == name
              where p.Version == version.ToString()
              select p).Single();
    }

    private IEnumerable<Plugin> GetDependencies(DeploymentDataContext ctx, Plugin plugin) {
      return from pair in ctx.Dependencies
             from dependency in ctx.Plugins
             where pair.PluginId == plugin.Id
             where pair.DependencyId == dependency.Id
             select dependency;
    }
    #endregion
  }
}
