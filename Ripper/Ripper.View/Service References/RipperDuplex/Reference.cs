﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.34014
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ripper.View.RipperDuplex {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.ripper.com.cn", ConfigurationName="RipperDuplex.BeerInventoryService", CallbackContract=typeof(Ripper.View.RipperDuplex.BeerInventoryServiceCallback), SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface BeerInventoryService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.ripper.com.cn/BeerInventoryService/Register", ReplyAction="http://www.ripper.com.cn/BeerInventoryService/RegisterResponse")]
        void Register(string clientId);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://www.ripper.com.cn/BeerInventoryService/SendCmd")]
        void SendCmd(string cmd);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://www.ripper.com.cn/BeerInventoryService/UnRegister")]
        void UnRegister(string clientId);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://www.ripper.com.cn/BeerInventoryService/HeartBreak")]
        void HeartBreak(string clientId);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface BeerInventoryServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://www.ripper.com.cn/BeerInventoryService/ReceiveCmd")]
        void ReceiveCmd(string cmd);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface BeerInventoryServiceChannel : Ripper.View.RipperDuplex.BeerInventoryService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class BeerInventoryServiceClient : System.ServiceModel.DuplexClientBase<Ripper.View.RipperDuplex.BeerInventoryService>, Ripper.View.RipperDuplex.BeerInventoryService {
        
        public BeerInventoryServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public BeerInventoryServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public BeerInventoryServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public BeerInventoryServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public BeerInventoryServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void Register(string clientId) {
            base.Channel.Register(clientId);
        }
        
        public void SendCmd(string cmd) {
            base.Channel.SendCmd(cmd);
        }
        
        public void UnRegister(string clientId) {
            base.Channel.UnRegister(clientId);
        }
        
        public void HeartBreak(string clientId) {
            base.Channel.HeartBreak(clientId);
        }
    }
}
