using Newtonsoft.Json;
using System;

namespace CERRS.MCR.JournalInterface
{
    public class McrAdditionalData
    {
        [JsonProperty("Disposition Date Time")]
        public DateTime? DispositionDateTime { get; set; }

        [JsonProperty("Dispute Source ID")]
        public string DisputeSourceId { get; set; }

        [JsonProperty("Dispute Type")]
        public string DisputeType { get; set; }

        [JsonProperty("Dispute Disposition Description")]
        public string DisputeDispositionDescription { get; set; }

        [JsonProperty("Dispute Disposition Code")]
        public string DisputeDispositionCode { get; set; }

        [JsonProperty("Final Case Disposition")]
        public string FinalCaseDisposition { get; set; }

        [JsonProperty("Telephone Number")]
        public string TelephoneNumber { get; set; }

        [JsonProperty("Call Type")]
        public string CallType { get; set; }

        [JsonProperty("Task ID")]
        public string TaskId { get; set; }

        [JsonProperty("Appeal Status")]
        public string AppealStatus { get; set; }

        [JsonProperty("HICS Case")]
        public string HicsCase { get; set; }

        [JsonProperty("HICS Case Open Date")]
        public DateTime? HicsCaseOpenDate { get; set; }

        [JsonProperty("Case Type")]
        public string CaseType { get; set; }

        [JsonProperty("Coverage Year")]
        public string CoverageYear { get; set; }

        [JsonProperty("Plan Name")]
        public string PlanName { get; set; }

        [JsonProperty("Notice Type")]
        public string NoticeType { get; set; }

        [JsonProperty("Address")]
        public string Address { get; set; }

        [JsonProperty("Call Disposition")]
        public string CallDisposition { get; set; }

        [JsonProperty("Data Source System")]
        public string DataSourceSystem { get; set; }

        [JsonProperty("Notes")]
        public string Notes { get; set; }

        [JsonProperty("DMI Reference Code")]
        public string DmiReferenceCode { get; set; }

        [JsonProperty("Version Number")]
        public string VersionNumber { get; set; }
    }
}
