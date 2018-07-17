using Newtonsoft.Json;

namespace CERRS.MCR.JournalInterface.Models
{
    class HICSData
    {
        [JsonProperty("applicationid")]
        public string Applicationid { get; set; }

        [JsonProperty("policyid")]
        public string Policyid { get; set; }
    }
}
