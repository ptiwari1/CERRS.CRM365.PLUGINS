using CERRS.MCR.JournalInterface.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace CERRS.MCR.JournalInterface
{
    public class XRMController
    {
        public IOrganizationService orgservice { get; set; }
        public XRMController(IOrganizationService service)
        {
            orgservice = service;

        }
        public bool IsMCRActive()
        {
            bool isActive = false;

            QueryExpression isActiveQuery = new QueryExpression();
            isActiveQuery.EntityName = "cog_cogsetting";
            isActiveQuery.ColumnSet = new ColumnSet("cog_stringvalue");

            ConditionExpression isActiveCond = new ConditionExpression();
            isActiveCond.EntityName = "cog_cogsetting";
            isActiveCond.AttributeName = "cog_name";
            isActiveCond.Operator = ConditionOperator.Equal;
            isActiveCond.Values.Add("MCREnable");
            FilterExpression isActiveFilter = new FilterExpression();
            isActiveFilter.Conditions.Add(isActiveCond);
            isActiveQuery.Criteria.AddFilter(isActiveFilter);

            EntityCollection cogSetting = orgservice.RetrieveMultiple(isActiveQuery);

            if (cogSetting.Entities.Count() > 0)
            {
                string stringValue = cogSetting.Entities[0].GetAttributeValue<string>("cog_stringvalue");
                isActive = Convert.ToBoolean(stringValue);
            }

            return isActive;
        }

        public string getMCRURIString()
        {
            string stringValue = "";

            QueryExpression connStringQuery = new QueryExpression();
            connStringQuery.EntityName = "cog_cogsetting";
            connStringQuery.ColumnSet = new ColumnSet("cog_stringvalue");

            ConditionExpression isActiveCond = new ConditionExpression();
            isActiveCond.EntityName = "cog_cogsetting";
            isActiveCond.AttributeName = "cog_name";
            isActiveCond.Operator = ConditionOperator.Equal;
            isActiveCond.Values.Add("Mvc6ServiceBaseUrl");
            FilterExpression isActiveFilter = new FilterExpression();
            isActiveFilter.Conditions.Add(isActiveCond);
            connStringQuery.Criteria.AddFilter(isActiveFilter);

            EntityCollection cogSetting = orgservice.RetrieveMultiple(connStringQuery);

            if (cogSetting.Entities.Count() > 0)
            {
                return cogSetting.Entities[0].GetAttributeValue<string>("cog_stringvalue");
            }
            else
            {
                return stringValue;
            }
        }
        public bool logJournalWrite(JournalEntry journal)
        {

            bool success = false;
            try
            {
                Entity mcrJournalLog = new Entity("cog_mcrlog");
                mcrJournalLog["cog_mcrtransactionresource"] = journal.JournalResource.Substring(0, 100);
                mcrJournalLog["cog_mcruri"] = journal.McrURI.ToString();
                mcrJournalLog["cog_transactiondate"] = DateTime.Now; //.ToString("MM/dd/yyyy HH:mm:ss"));
                mcrJournalLog["cog_mcrrequestbody"] = journal.JournalRequest;
                mcrJournalLog["cog_mcrresponsebody"] = journal.PostResponse;
                mcrJournalLog["cog_errorreceived"] = journal.PostError;
                if (journal.CrmCase != Guid.Empty || journal.CrmCase == null)
                {
                    mcrJournalLog["cog_caseid"] = new EntityReference("cog_case", journal.CrmCase);
                }

                if (journal.PostError == false && journal.JournalResource.Contains("v3/journalSubmittal/PhoneCall/"))
                {
                    mcrJournalLog["cog_mcrresponsebody"] = @"{""result"":""" + "SUCCESS" + @"""}";
                }

                orgservice.Create(mcrJournalLog);
                success = true;
            }
            catch (Exception e)
            {
                success = false;
            }
            return success;
        }
        public bool logJournalWrite(string Message)
        {

            bool success = false;
            try
            {
                Entity mcrJournalLog = new Entity("cog_mcrlog");               
                mcrJournalLog["cog_transactiondate"] = DateTime.Now;                
                mcrJournalLog["cog_mcrresponsebody"] = Message;
                mcrJournalLog["cog_errorreceived"] = true;
                orgservice.Create(mcrJournalLog);
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }

        public string RetrieveSettingValue(string setting)
        {
            string stringValue = "";

            QueryExpression connStringQuery = new QueryExpression();
            connStringQuery.EntityName = "cog_cogsetting";
            connStringQuery.ColumnSet = new ColumnSet("cog_stringvalue");

            ConditionExpression isActiveCond = new ConditionExpression();
            isActiveCond.EntityName = "cog_cogsetting";
            isActiveCond.AttributeName = "cog_name";
            isActiveCond.Operator = ConditionOperator.Equal;
            isActiveCond.Values.Add(setting);
            FilterExpression isActiveFilter = new FilterExpression();
            isActiveFilter.Conditions.Add(isActiveCond);
            connStringQuery.Criteria.AddFilter(isActiveFilter);

            EntityCollection cogSetting = orgservice.RetrieveMultiple(connStringQuery);

            if (cogSetting.Entities.Count() > 0)
            {
                stringValue = cogSetting.Entities[0].GetAttributeValue<string>("cog_stringvalue");
            }
            return stringValue;
        }


    }
}
