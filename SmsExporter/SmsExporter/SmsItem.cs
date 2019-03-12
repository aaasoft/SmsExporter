using System;
using System.Collections.Generic;
using System.Text;

namespace SmsExporter
{
    public class SmsItem
    {
        public enum SmsItemType
        {
            Recv = 1,
            Send = 2
        }
        public string Id { get; set; }
        public string Address { get; set; }
        public DateTime Date { get; set; }
        public SmsItemType ItemType { get; set; }
        public string Body { get; set; }
    }
}
