using System;

namespace CERRS.MCR.JournalInterface
{
    public class JournalEntry
    {
        public string JournalRequest { get; set; }
        public string PostResponse { get; set; }
        public Boolean PostError { get; set; }
        public Uri McrURI { get; set; }
        public string JournalResource { get; set; }
        public string HicsCase { get; set; }
        public Guid CrmCase { get; set; }
        public McrJournalEntry McrJentry { get; set; }
    }
}
