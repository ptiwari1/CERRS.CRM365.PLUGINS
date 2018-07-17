using Microsoft.Xrm.Sdk;
using System;

namespace CERRS.Plugins.TraceLog
{
    public class AccountUpdateSync : IPlugin
    {        public void Execute(IServiceProvider serviceProvider)
        {
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracingService.Trace("Tracing: Execute");

            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            try
            {
                tracingService.Trace("Tracing : Try");
                if (!context.InputParameters.Contains("Target") || !(context.InputParameters["Target"] is Entity))
                    return;

                tracingService.Trace("Tracing : Inpur Parameter Count: {0}", context.InputParameters.Count.ToString());

                var entity = (Entity)context.InputParameters["Target"];

                tracingService.Trace("Tracing - Entity Attributes Count: {0}", entity.Attributes.Count.ToString());
                foreach (var attribute in entity.Attributes)
                {
                    tracingService.Trace("Tracing -  Key: {0} - Value: {1}", attribute.Key, attribute.Value);
                }
                tracingService.Trace("Tracing: End");

            }
            catch (Exception ex)
            {
                tracingService.Trace(ex.ToString());
                throw;
            }
        }
    }
}
