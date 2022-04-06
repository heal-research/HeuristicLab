using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace HeuristicLab.Build {
  public class MergeConfigs : Task {
    [Required]
    public string SourceFilePath { get; set; }

    [Required]
    public string DestinationFilePath { get; set; }

    public override bool Execute() {
      Log.LogMessage("ConfigMerger: Merge started ...");
      Log.LogMessage("ConfigMerger: Merge source:      \"" + SourceFilePath + "\"");
      Log.LogMessage("ConfigMerger: Merge destination: \"" + DestinationFilePath + "\"");

      XmlDocument source = new XmlDocument();
      source.Load(SourceFilePath);
      XmlDocument destination = new XmlDocument();
      destination.Load(DestinationFilePath);

      XmlNode sourceNode;
      XmlNode destinationNode;

      #region Merge 'system.serviceModel/behaviors/*'
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/behaviors");
      if (destinationNode == null) {
        destinationNode = destination.CreateElement("behaviors");
        destination.SelectSingleNode("/configuration/system.serviceModel").AppendChild(destinationNode);
      }

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/behaviors/endpointBehaviors");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/behaviors/endpointBehaviors");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel/behaviors");

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/behaviors/serviceBehaviors");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/behaviors/serviceBehaviors");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel/behaviors");
      #endregion

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/services");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/services");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel");

      #region Merge 'system.serviceModel/bindings/*'
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/bindings");
      if (destinationNode == null) {
        destinationNode = destination.CreateElement("bindings");
        destination.SelectSingleNode("/configuration/system.serviceModel").AppendChild(destinationNode);
      }
      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/bindings/basicHttpBinding");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/bindings/basicHttpBinding");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel/bindings");

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/bindings/basicHttpContextBinding");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/bindings/basicHttpContextBinding");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel/bindings");

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/bindings/customBinding");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/bindings/customBinding");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel/bindings");

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/bindings/mexHttpBinding");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/bindings/mexHttpBinding");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel/bindings");

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/bindings/mexHttpsBinding");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/bindings/mexHttpsBinding");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel/bindings");

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/bindings/mexNamedPipeBinding");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/bindings/mexNamedPipeBinding");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel/bindings");

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/bindings/mexTcpBinding");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/bindings/mexTcpBinding");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel/bindings");

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/bindings/msmqIntegrationBinding");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/bindings/msmqIntegrationBinding");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel/bindings");

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/bindings/netMsmqBinding");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/bindings/netMsmqBinding");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel/bindings");

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/bindings/netNamedPipeBinding");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/bindings/netNamedPipeBinding");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel/bindings");

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/bindings/netPeerTcpBinding");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/bindings/netPeerTcpBinding");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel/bindings");

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/bindings/netTcpBinding");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/bindings/netTcpBinding");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel/bindings");

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/bindings/netTcpContextBinding");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/bindings/netTcpContextBinding");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel/bindings");

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/bindings/webHttpBinding");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/bindings/webHttpBinding");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel/bindings");

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/bindings/ws2007FederationHttpBinding");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/bindings/ws2007FederationHttpBinding");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel/bindings");

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/bindings/ws2007HttpBinding");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/bindings/ws2007HttpBinding");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel/bindings");

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/bindings/wsDualHttpBinding");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/bindings/wsDualHttpBinding");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel/bindings");

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/bindings/wsFederationHttpBinding");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/bindings/wsFederationHttpBinding");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel/bindings");

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/bindings/wsHttpBinding");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/bindings/wsHttpBinding");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel/bindings");

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/bindings/wsHttpContextBinding");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/bindings/wsHttpContextBinding");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel/bindings");
      #endregion

      sourceNode = source.SelectSingleNode("/configuration/system.serviceModel/client");
      destinationNode = destination.SelectSingleNode("/configuration/system.serviceModel/client");
      Merge(sourceNode, destinationNode, destination, "/configuration/system.serviceModel");

      sourceNode = source.SelectSingleNode("/configuration/configSections/sectionGroup[@name='applicationSettings']");
      destinationNode = destination.SelectSingleNode("/configuration/configSections/sectionGroup[@name='applicationSettings']");
      Merge(sourceNode, destinationNode, destination, "/configuration/configSections");

      sourceNode = source.SelectSingleNode("/configuration/configSections/sectionGroup[@name='userSettings']");
      destinationNode = destination.SelectSingleNode("/configuration/configSections/sectionGroup[@name='userSettings']");
      Merge(sourceNode, destinationNode, destination, "/configuration/configSections");

      sourceNode = source.SelectSingleNode("/configuration/applicationSettings");
      destinationNode = destination.SelectSingleNode("/configuration/applicationSettings");
      Merge(sourceNode, destinationNode, destination, "/configuration");

      sourceNode = source.SelectSingleNode("/configuration/userSettings");
      destinationNode = destination.SelectSingleNode("/configuration/userSettings");
      Merge(sourceNode, destinationNode, destination, "/configuration");

      sourceNode = source.SelectSingleNode("/configuration/connectionStrings");
      destinationNode = destination.SelectSingleNode("/configuration/connectionStrings");
      Merge(sourceNode, destinationNode, destination, "/configuration");

      sourceNode = source.SelectSingleNode("/configuration/system.data/DbProviderFactories");
      destinationNode = destination.SelectSingleNode("/configuration/system.data/DbProviderFactories");
      Merge(sourceNode, destinationNode, destination, "/configuration");

      sourceNode = source.SelectSingleNode("/configuration/system.diagnostics");
      destinationNode = destination.SelectSingleNode("/configuration/system.diagnostics");
      Merge(sourceNode, destinationNode, destination, "/configuration");

      sourceNode = source.SelectSingleNode("/configuration/oracle.manageddataaccess.client/version/dataSources");
      destinationNode = destination.SelectSingleNode("/configuration/oracle.manageddataaccess.client/version/dataSources");
      Merge(sourceNode, destinationNode, destination, "/configuration");

      sourceNode = source.SelectSingleNode("/configuration/oracle.manageddataaccess.client/version/settings");
      destinationNode = destination.SelectSingleNode("/configuration/oracle.manageddataaccess.client/version/settings");
      Merge(sourceNode, destinationNode, destination, "/configuration");

      sourceNode = source.SelectSingleNode("/configuration/oracle.manageddataaccess.client/version/LDAPsettings");
      destinationNode = destination.SelectSingleNode("/configuration/oracle.manageddataaccess.client/version/LDAPsettings");
      Merge(sourceNode, destinationNode, destination, "/configuration");

      destination.Save(DestinationFilePath);

      Log.LogMessage("ConfigMerger: Merge successfully finished!");
      return true;
    }

    private static void Merge(XmlNode source, XmlNode destination, XmlDocument document, string root) {
      try {
        if (source != null) {
          if (destination == null) {
            XmlNode newNode = document.ImportNode(source, true);
            document.SelectSingleNode(root).AppendChild(newNode);
          } else {
            foreach (XmlNode node in source.ChildNodes) {
              XmlNode newNode = document.ImportNode(node, true);
              XmlNode oldNode = destination.SelectSingleNode(BuildXPathString(newNode));
              if (oldNode != null)
                destination.ReplaceChild(newNode, oldNode);
              else
                destination.AppendChild(newNode);
            }
          }
        }
      } catch (Exception ex) {
        StringBuilder sb = new StringBuilder();
        sb.Append("Error while merging node \"").Append(source.Name).Append("\"");
        throw new Exception(sb.ToString(), ex);
      }
    }

    private static string BuildXPathString(XmlNode node) {
      StringBuilder builder = new StringBuilder();
      builder.Append(node.Name);
      if (node.Attributes.Count > 0) {
        XmlAttribute attrib = node.Attributes[0];
        builder.Append("[");
        builder.Append("@" + attrib.Name + "='" + attrib.Value + "'");
        for (int i = 1; i < node.Attributes.Count; i++) {
          attrib = node.Attributes[i];
          builder.Append(" and @" + attrib.Name + "='" + attrib.Value + "'");
        }
        builder.Append("]");
      }
      return builder.ToString();
    }
  }
}
