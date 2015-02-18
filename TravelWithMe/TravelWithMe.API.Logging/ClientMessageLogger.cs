using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using TravelWithMe.Logging.Helper;

namespace TravelWithMe.API.Logging
{
    public class ClientMessageLogger : Attribute, IClientMessageInspector, IEndpointBehavior, IOperationBehavior
    {
        private string Source = "ClientMessageLogger";

        #region IClientMessageInspector Members

        void IClientMessageInspector.AfterReceiveReply(ref Message reply, object correlationState)
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
                    Logger.LogException(ex, Source, "AfterReceiveReply", Severity.Normal);
                }
            }
        }

        object IClientMessageInspector.BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            try
            {
                Log log = MessageInspectorHelper.BuildLogRequest(ref request, channel);
                log.Name = "API|" + log.Name;
                return log;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, Source, "BeforeSendRequest", Severity.Normal);
                return null;
            }
        }

        #endregion

        #region IEndpointBehavior Members

        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint,
                                                    BindingParameterCollection bindingParameters)
        {
        }

        void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(this);
        }

        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
        {
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
    }
}
