using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using AMSMSService;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;


namespace AMSMSService {
    public partial class AMSMSService : ServiceBase {
        #region Private Variables
        // Port settings
        //string portName = "COM1";
        int baudRate = 115200;
        int dataBits = 8;
        int readTimout = 300;
        int writeTimeout = 300;


        SerialPort portHandle = new SerialPort();
        SMSOperations msgHandle = new SMSOperations();
        ShortMessageCollection objShortMessageCollection = new ShortMessageCollection();
        #endregion

        public AMSMSService() {
            InitializeComponent();
        }

        protected override void OnStart(string[] args) {

            //msgHandle.ClosePort(portHandle);
            Logger.WriteLineToFile("<OnStart> Adaptive SMS Service succesfully started");

            //Getting Connected Ports
            DBConnection.getConnectedPorts();


            //waiting for incoming messages
            Task.Factory.StartNew(() => {
                //var CommPorts = new List<string> { "COM1", "COM2", "COM3" };
                //Parallel.ForEach(SMSOperations.CommPorts, (port) => CheckForMessages(port));
                Parallel.ForEach(SMSModem.modems, (port) => CheckForMessages(port.comPort));
            });

            

            Task.Factory.StartNew(() => {
                while(true) {
                    // check and fetch messages to be sent.
                    DBConnection.GetMessagesToBeSent();
                    // Peek at the first element.
                    textMessage result;
                    if(!textMessage.messagestobesent.TryPeek(out result)) {
                    } else {
                        CheckandSendMessages(result);
                    }

                    AutoResetEvent pause = new AutoResetEvent(false);
                    pause.WaitOne(5000, true);
                }

            });
           
            
        }

        private void CheckForMessages(string port) {
            while(true) {
                try {
                    portHandle = msgHandle.OpenPort(port, baudRate, dataBits, readTimout, writeTimeout);
                    Logger.WriteLineToFile(string.Format("<CheckForMessages> Succefully connected to port: {0}", port));
                } catch(Exception ex) {
                    Logger.WriteLineToFile(string.Format("<CheckForMessages> The application could not connect to port: {0} with Error {1}", port, ex.Message));
                    return;
                }

            
                #region receive all messages
                //Reading all messages
                try {
                    //count SMS 
                    int uCountSMS = msgHandle.CountSMSmessages(portHandle);
                    if(uCountSMS > 0) {
                        string strCommand = "AT+CMGL=\"ALL\"";
                        // If SMS exist then read SMS
                        //.............................................. Read all SMS ....................................................
                        objShortMessageCollection = msgHandle.ReadSMS(portHandle, strCommand);
                        foreach(ShortMessage msg in objShortMessageCollection) {
                            string replacement = "''";
                            Regex rgx = new Regex("\'");
                            string rez = rgx.Replace(msg.Message, replacement);

                            if(DBConnection.executeSP(string.Format("'{0}', '{1}', '{2}', '{3}'", port, msg.Sender, rez, msg.Sent))) {
                                Logger.WriteLineToFile(string.Format("<CheckForMessages> Com Port: '{0}', Sender: '{1}', Message: '{2}', Recipient: '{3}'", port, msg.Sender, msg.Message, msg.Sent));
                            } else { msgHandle.ClosePort(portHandle);  return; }
                          }

                        // delete all sms after reading
                        strCommand = "AT+CMGD=1,3";
                        if(msgHandle.DeleteMsg(portHandle, strCommand)) {
                            Logger.WriteLineToFile("<CheckForMessages> Messages succesffully deleted");
                        } else {
                            //Logger.WriteLineToFile("Failed to delete messgaes");
                        }
                    } 
                } catch(Exception ex) {
                    Logger.WriteLineToFile(string.Format("<CheckForMessages> An exception occured : {0}", ex.Message));
                }
                msgHandle.ClosePort(portHandle);
                #endregion

                AutoResetEvent pause = new AutoResetEvent(false);
                pause.WaitOne(10000, true);
            }


        }

        private bool CheckandSendMessages(textMessage msgObj) {
            // try to open the specified port first
            bool isBusy = true;
            try {
                if(msgObj.MCommPort != string.Empty) {
                    portHandle = msgHandle.OpenPort(msgObj.MCommPort, baudRate, dataBits, readTimout, writeTimeout);
                } else {
                    foreach(var m in SMSModem.modems) {
                        portHandle = msgHandle.OpenPort(m.name, baudRate, dataBits, readTimout, writeTimeout);
                        break;
                    }                
                }
                Logger.WriteLineToFile(string.Format("<CheckandSendMessages> Succefully connected to port: {0}", msgObj.MCommPort));
                isBusy = false;
            } catch(Exception ex) {
                Logger.WriteLineToFile(string.Format("<CheckandSendMessages> The application could not connect to port: {0} with Error {1}, {2}", msgObj.MCommPort, ex.Message, ex.GetType().ToString()));
                return isBusy;
            }

            #region send message
            // Sending messages
            if(msgHandle.sendMsg(portHandle, msgObj.Sender, msgObj.Message)) {
                textMessage result;
                if(textMessage.messagestobesent.TryDequeue(out result)) {
                    Logger.WriteLineToFile(string.Format("<CheckandSendMessages> MEssage sent on: {0} to: {1}", msgObj.MCommPort, msgObj.Sender));
                }
                DBConnection.Updateprocessed(msgObj.Index);
               
            } else {
                Logger.WriteLineToFile("<CheckandSendMessages> Message could not be sent");
            }
            msgHandle.ClosePort(portHandle);
            return isBusy;
            #endregion

        }

        protected override void OnStop() {
        }

        public void onDebug() {
            OnStart(null);
        }
    }
}

