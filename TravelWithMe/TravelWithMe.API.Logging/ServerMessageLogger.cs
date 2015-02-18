using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using TravelWithMe.Logging.Helper;
using System.ServiceModel;
using System.Collections.ObjectModel;

namespace TravelWithMe.API.Logging
{
    public class ServerMessageLogger : Attribute, IDispatchMessageInspector, IServiceBehavior, IOperationBehavior
    {
        private string Source = "ServerMessageLogger";

        #region IDispatchMessageInspector Members

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            try
            {
                Log log = MessageInspectorHelper.BuildLogRequest(ref request, channel);
                return log;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, Source, "AfterReceiveRequest", Severity.Normal);
                return null;
            }
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (correlationState != null)
            {
                var log = (Log)correlationState;
                try
                {
                    MessageInspectorHelper.LogRequestResponse(ref reply, log);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex, Source, "BeforeSendReply", Severity.Normal);
                }
            }
        }

        #endregion

        #region IOperationBehavior Members

        void IOperationBehavior.AddBindingParameters(OperationDescription operationDescription,
                                                     BindingParameterCollection bindingParameters)
        {
        }

        void IOperationBehavior.ApplyClientBehavior(OperationDescription operationDescription,
                                                    ClientOperation clientOperation)
        {
        }

        void IOperationBehavior.ApplyDispatchBehavior(OperationDescription operationDescription,
                                                      DispatchOperation dispatchOperation)
        {
        }

        void IOperationBehavior.Validate(OperationDescription operationDescription)
        {
        }

        #endregion

        #region IServiceBehavior Members

        void IServiceBehavior.AddBindingParameters(ServiceDescription serviceDescription,
                                                   ServiceHostBase serviceHostBase,
                                                   Collection<ServiceEndpoint> endpoints,
                                                   BindingParameterCollection bindingParameters)
        {
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription serviceDescription,
                                                    ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher chDisp in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher epDisp in chDisp.Endpoints)
                {
                    epDisp.DispatchRuntime.MessageInspectors.Add(this);
                }
            }
        }

        void IServiceBehavior.Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        #endregion
    }
}
