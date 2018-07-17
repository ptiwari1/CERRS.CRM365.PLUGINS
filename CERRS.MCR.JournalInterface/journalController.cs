using CERRS.MCR.JournalInterface.Models;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;

namespace CERRS.MCR.JournalInterface
{
    public class journalController
    {
        public IOrganizationService XrmOrgService { get; set; }
        public bool getHICSAppandPolicyData { get; set; }
        public journalController(IOrganizationService orgService)
        {
            XrmOrgService = orgService;
            getHICSAppandPolicyData = true;
        }

        public JournalEntry WriteJournalEntry(JournalEntry jEntry)
        {
            MCR objMCR = new MCR(XrmOrgService);
            if (jEntry.McrJentry.InsuranceApplicationIdentifier == null)
            {
                if (getHICSAppandPolicyData == true)
                {
                    GetApplicationOrPolicy(jEntry, objMCR);
                }
            }

            if (jEntry.McrJentry.InsuranceApplicationIdentifier == null)
            {
                jEntry.McrJentry.InsuranceApplicationIdentifier = 42;
            }
            jEntry.JournalRequest = CreateRequest(jEntry.McrJentry);
            jEntry.McrURI = objMCR.mcrURI;
            jEntry.PostError = false;
            jEntry.PostResponse = objMCR.postToMCR(jEntry.JournalRequest, jEntry.JournalResource);
            if (jEntry.PostResponse.IndexOf("errorCode") > 0)
            {
                jEntry.PostError = true;
            }
            return jEntry;
        }

        private void GetApplicationOrPolicy(JournalEntry jentry, MCR objMCR)
        {
            XRMController controller = new XRMController(XrmOrgService);
            try
            {
                HICSData hicsData = objMCR.getERR1095Data("v1/HICSData/", jentry.HicsCase);
                if (hicsData != null)
                {
                    jentry.McrJentry.InsuranceApplicationIdentifier = Convert.ToInt32(hicsData.Applicationid);
                    jentry.McrJentry.JournalObjectIdentifier = hicsData.Policyid;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private string CreateRequest(McrJournalEntry mcrJ)
        {
            var JsonSerializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                DateFormatString = "yyyy-MM-ddTHH:mm:sszzz"
            };
            var json = JsonConvert.SerializeObject(mcrJ, JsonSerializerSettings);
            return json;
        }

    }
}
