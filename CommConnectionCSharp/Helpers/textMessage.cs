using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace AMSMSService{
    public class textMessage {
        #region Private Variables
        private string index;
        private string status;
        private string sender;
        private string alphabet;
        private string sent;
        private string message;
        private string dispatchTime;
        private string mCommPort;
        public static ConcurrentQueue<textMessage> messagestobesent = new ConcurrentQueue<textMessage>();
        #endregion

        #region Public Properties
        public string Index {
            get { return index; }
            set { index = value; }
        }
        public string Status {
            get { return status; }
            set { status = value; }
        }
        public string Sender {
            get { return sender; }
            set { sender = value; }
        }
        public string Alphabet {
            get { return alphabet; }
            set { alphabet = value; }
        }
        public string Sent {
            get { return sent; }
            set { sent = value; }
        }
        public string Message {
            get { return message; }
            set { message = value; }
        }
        public string DispatchTime {
            get { return dispatchTime; }
            set { dispatchTime = value; }
        }

        public string MCommPort {
            get { return mCommPort; }
            set { mCommPort = value; }
        }
        #endregion
    }
}
