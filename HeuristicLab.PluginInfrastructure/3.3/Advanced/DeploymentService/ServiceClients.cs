//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HeuristicLab.PluginInfrastructure.Advanced.DeploymentService
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PluginDescription", Namespace="http://schemas.datacontract.org/2004/07/HeuristicLab.Services.Deployment", IsReference=true)]
    public partial class PluginDescription : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string ContactEmailField;
        
        private string ContactNameField;
        
        private HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.PluginDescription[] DependenciesField;
        
        private string LicenseTextField;
        
        private string NameField;
        
        private System.Version VersionField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ContactEmail
        {
            get
            {
                return this.ContactEmailField;
            }
            set
            {
                this.ContactEmailField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ContactName
        {
            get
            {
                return this.ContactNameField;
            }
            set
            {
                this.ContactNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.PluginDescription[] Dependencies
        {
            get
            {
                return this.DependenciesField;
            }
            set
            {
                this.DependenciesField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LicenseText
        {
            get
            {
                return this.LicenseTextField;
            }
            set
            {
                this.LicenseTextField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name
        {
            get
            {
                return this.NameField;
            }
            set
            {
                this.NameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Version Version
        {
            get
            {
                return this.VersionField;
            }
            set
            {
                this.VersionField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ProductDescription", Namespace="http://schemas.datacontract.org/2004/07/HeuristicLab.Services.Deployment")]
    public partial class ProductDescription : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string NameField;
        
        private HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.PluginDescription[] PluginsField;
        
        private System.Version VersionField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name
        {
            get
            {
                return this.NameField;
            }
            set
            {
                this.NameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.PluginDescription[] Plugins
        {
            get
            {
                return this.PluginsField;
            }
            set
            {
                this.PluginsField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Version Version
        {
            get
            {
                return this.VersionField;
            }
            set
            {
                this.VersionField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.IUpdateService")]
    public interface IUpdateService
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpdateService/GetPlugin", ReplyAction="http://tempuri.org/IUpdateService/GetPluginResponse")]
        byte[] GetPlugin(HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.PluginDescription description);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpdateService/GetProducts", ReplyAction="http://tempuri.org/IUpdateService/GetProductsResponse")]
        HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.ProductDescription[] GetProducts();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUpdateService/GetPlugins", ReplyAction="http://tempuri.org/IUpdateService/GetPluginsResponse")]
        HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.PluginDescription[] GetPlugins();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IUpdateServiceChannel : HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.IUpdateService, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class UpdateServiceClient : System.ServiceModel.ClientBase<HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.IUpdateService>, HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.IUpdateService
    {
        
        public UpdateServiceClient()
        {
        }
        
        public UpdateServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public UpdateServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public UpdateServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public UpdateServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public byte[] GetPlugin(HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.PluginDescription description)
        {
            return base.Channel.GetPlugin(description);
        }
        
        public HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.ProductDescription[] GetProducts()
        {
            return base.Channel.GetProducts();
        }
        
        public HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.PluginDescription[] GetPlugins()
        {
            return base.Channel.GetPlugins();
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.IAdminService")]
    public interface IAdminService
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAdminService/DeployProduct", ReplyAction="http://tempuri.org/IAdminService/DeployProductResponse")]
        void DeployProduct(HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.ProductDescription product);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAdminService/DeleteProduct", ReplyAction="http://tempuri.org/IAdminService/DeleteProductResponse")]
        void DeleteProduct(HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.ProductDescription product);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAdminService/DeployPlugin", ReplyAction="http://tempuri.org/IAdminService/DeployPluginResponse")]
        void DeployPlugin(HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.PluginDescription plugin, byte[] zipFile);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAdminServiceChannel : HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.IAdminService, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AdminServiceClient : System.ServiceModel.ClientBase<HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.IAdminService>, HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.IAdminService
    {
        
        public AdminServiceClient()
        {
        }
        
        public AdminServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public AdminServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public AdminServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public AdminServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public void DeployProduct(HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.ProductDescription product)
        {
            base.Channel.DeployProduct(product);
        }
        
        public void DeleteProduct(HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.ProductDescription product)
        {
            base.Channel.DeleteProduct(product);
        }
        
        public void DeployPlugin(HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.PluginDescription plugin, byte[] zipFile)
        {
            base.Channel.DeployPlugin(plugin, zipFile);
        }
    }
}
