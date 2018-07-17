using CERRS.MCR.JournalInterface;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CERRS.Plugins.MCRJournalOnPhCallCreate
{
    public partial class MCRJournalOnPhCallCreate : BasePlugin
    {
        StringBuilder sw;        
        public MCRJournalOnPhCallCreate(string unsecureConfig, string secureConfig) : base(unsecureConfig, secureConfig)
        {
            // Register for any specific events by instantiating a new instance of the 'PluginEvent' class and registering it
            base.RegisteredEvents.Add(new PluginEvent()
            {
                Stage = eStage.PostOperation,
                MessageName = MessageNames.Create,
                EntityName = EntityNames.phonecall,
                PluginAction = ExecutePluginLogic
            });
        }
        public void ExecutePluginLogic(IServiceProvider serviceProvider)
        {
            // Use a 'using' statement to dispose of the service context properly
            // To use a specific early bound entity replace the 'Entity' below with the appropriate class type
            var tracingservice = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
           
            using (var localContext = new LocalPluginContext<Entity>(serviceProvider))
            {
                IOrganizationService service = localContext.OrganizationService;
                tracingservice.Trace("Tracing: ExecutePluginLogic");

                try
                {
                    tracingservice.Trace("Tracing ExecutePluginLogic : Try");
                    XRMController controller = new XRMController(service);

                    if (controller.IsMCRActive() == true)
                    {
                        journalController jcontrol = new journalController(service);
                        //gets the plugin target image which only contains updated or filled in fields.
                        Entity phonecall = localContext.TargetEntity;
                        JournalEntry journal = CreateJournalEntry(service, phonecall);
                        tracingservice.Trace("ExecutePluginLogic : " + journal.ToString());
                        jcontrol.WriteJournalEntry(journal);
                        bool success = controller.logJournalWrite(journal);
                    }
                    tracingservice.Trace("Tracing ExecutePluginLogic : End");
                }
                catch (Exception e)
                {
                    XRMController controller = new XRMController(service);
                    bool success = controller.logJournalWrite(e.Message + e.StackTrace + sw.ToString());
                    tracingservice.Trace(e.ToString());
                    localContext.Trace(e.StackTrace);
                    localContext.Trace(e.Message);
                }
            }
        }

        private JournalEntry CreateJournalEntry(IOrganizationService service, Entity phonecall)
        {           
            //string mcrRequest="";
            JournalEntry jEntry = new JournalEntry();
            jEntry.McrJentry = new McrJournalEntry
            {
                AdditionalData = new McrAdditionalData()
            };
            // EntityReference regadingObjId = phonecall.GetAttributeValue<EntityReference>("regardingobjectid");
            Entity activity = service.Retrieve("activitypointer", phonecall.GetAttributeValue<Guid>("activityid"), new ColumnSet("regardingobjectid"));
            EntityReference regardingobject = activity.GetAttributeValue<EntityReference>("regardingobjectid");
            string hicsCase = regardingobject.Name ?? string.Empty;
            string homephone = null;
            bool IncomingCall = false;
            //sw.AppendFormat("--Incoming Argument--{0}--{1}", phonecall,service);
            IncomingCall = phonecall.GetAttributeValue<Boolean>("directioncode");
            int boolInt = IncomingCall ? 1 : 0;
            sw.AppendFormat("--IncomingCall Check--{0}", boolInt);

            if (regardingobject.LogicalName == "incident")
            {
                Entity callCase = service.Retrieve("incident", regardingobject.Id, new ColumnSet("contactid"));

                if (callCase.GetAttributeValue<EntityReference>("contactid") != null)
                {
                    Entity consumer = service.Retrieve("contact", callCase.GetAttributeValue<EntityReference>("contactid").Id, new ColumnSet("telephone2"));
                    homephone = consumer.GetAttributeValue<string>("telephone2");
                }
            }
            sw.AppendFormat("--OK upTo HomePhone--{0}", homephone);
            var ActId = activity.GetAttributeValue<Guid>("activityid");
            sw.AppendFormat("--Check up To ActID --{0}", ActId);
            jEntry.JournalResource = "v3/journalSubmittal/PhoneCall/" + ActId + "/" + ActId;            
            jEntry.McrJentry.JournalTypeCode = "COMMUNICATION";
            jEntry.McrJentry.JournalObjectCode = "PHONE_CALL";           
            sw.AppendFormat("--Check up To actionCode --{0}", hicsCase);
            jEntry.HicsCase = hicsCase;
            jEntry.McrJentry.JournalDescriptionCode = (IncomingCall) ? "OUTBOUND_CALL" : "INBOUND_CALL";
            jEntry.McrJentry.ActionCode = (IncomingCall) ? "PLACED" : "RECEIVED";
            jEntry.McrJentry.ActorCode = "ERR_1095A_WORKER";           
            sw.AppendFormat("--Check upto actorCode--{0}", hicsCase);
            jEntry.McrJentry.AdditionalData.HicsCase = hicsCase;
            jEntry.McrJentry.AdditionalData.TelephoneNumber = string.IsNullOrWhiteSpace(homephone) ? null : homephone;           
            jEntry.McrJentry.JournalDateTime = DateTime.Now;
            sw.AppendFormat("--Finished jEntry--{0}", homephone);
            jEntry.CrmCase = regardingobject.Id;
            return jEntry;
        }

        public string GetOptionsetText(Entity entity, IOrganizationService service, string optionsetName, int optionsetValue)
        {
            string optionsetSelectedText = string.Empty;
            try
            {
                RetrieveOptionSetRequest retrieveOptionSetRequest = new RetrieveOptionSetRequest { Name = optionsetName };
                RetrieveOptionSetResponse retrieveOptionSetResponse = (RetrieveOptionSetResponse)service.Execute(retrieveOptionSetRequest);
                OptionSetMetadata retrievedOptionSetMetadata = (OptionSetMetadata)retrieveOptionSetResponse.OptionSetMetadata;
                OptionMetadata[] optionList = retrievedOptionSetMetadata.Options.ToArray();
                foreach (OptionMetadata optionMetadata in optionList)
                {
                    if (optionMetadata.Value == optionsetValue)
                    {
                        optionsetSelectedText = optionMetadata.Label.UserLocalizedLabel.Label.ToString();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return optionsetSelectedText;
        }
    }
}
