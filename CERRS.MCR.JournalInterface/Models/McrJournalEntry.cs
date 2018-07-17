using Newtonsoft.Json;
using System;

namespace CERRS.MCR.JournalInterface
{
    public class McrJournalEntry
    {
        public long? InsuranceApplicationIdentifier { get; set; }
        public long? MarketplaceGroupPolicyIdentifier { get; set; }
        public string personTrackingNumber { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string JournalTypeCode { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string JournalObjectCode { get; set; }
        public string JournalObjectIdentifier { get; set; }
        public long? ApplicationVersionNumber { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string ActionCode { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string JournalDescriptionCode { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string ActorCode { get; set; }
        public string SystemCode { get; set; }
        public McrAdditionalData AdditionalData { get; set; } = new McrAdditionalData();
        [JsonProperty(Required = Required.Always)]
        public DateTime JournalDateTime { get; set; }
    }
}
