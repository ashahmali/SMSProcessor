using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMSMSService{
    
    public class SMSModem {
        public string id { get; set; }
        public string comPort { get; set; }
        public string name { get; set; }
        public string Number { get; set; }
        public string receive { get; set; }
        public string send { get; set; }

        public static List<SMSModem> modems = new List<SMSModem>();
    }
}
