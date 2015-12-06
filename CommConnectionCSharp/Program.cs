using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.IO;
using System.IO.Ports;

namespace AMSMSService {
    class Program {
        static void Main(string[] args) {

            //#region Private Variables
            //// Port settings
            //string portName = "COM1";
            //int baudRate = 115200;
            //int dataBits = 8;
            //int readTimout = 300;
            //int writeTimeout = 300;


            //SerialPort portHandle = new SerialPort();
            //SMSOperations msgHandle = new SMSOperations();
            //ShortMessageCollection objShortMessageCollection = new ShortMessageCollection();
            //#endregion

            //try {
            //    portHandle = msgHandle.OpenPort(portName, baudRate, dataBits, readTimout, writeTimeout);
            //} catch(Exception ex) {
            //    Console.WriteLine("The application could not connect to port: {0} with Error {1}", portName, ex.Message);
            //    Console.ReadLine();
            //    return;
            //}


            
            //#region send message
            //// Sending messages
            //if(msgHandle.sendMsg(portHandle, "07423298887", "Testing message")) {
            //    Console.WriteLine("message sent successfully");
            //} else {
            //    Console.WriteLine("message could not be sent");
            //}
            //#endregion

            //#region receive all messages
            ////Reading all messages
            //try {
            //    //count SMS 
            //    int uCountSMS = msgHandle.CountSMSmessages(portHandle);
            //    if(uCountSMS > 0) {
            //        string strCommand = "AT+CMGL=\"ALL\"";
            //        // If SMS exist then read SMS
            //        //.............................................. Read all SMS ....................................................
            //        objShortMessageCollection = msgHandle.ReadSMS(portHandle, strCommand);
            //        foreach(ShortMessage msg in objShortMessageCollection) {
            //            Console.WriteLine("Message Index: {0}\n Message Recieved at: {1}\n Message Sender: {2}\n Message: {3}\n\n\n", msg.Index, msg.Sent, msg.Sender, msg.Message);
            //        }
            //    } else {
            //        Console.WriteLine("No Message in inbox");
            //    }
            //} catch(Exception ex) {
            //    Console.WriteLine("An exception occured : {0}", ex.Message);
            //}
            //#endregion

            //#region Delete messages 
            //// delete messages
            //try {
            //    //count SMS 
            //    int uCountSMS = msgHandle.CountSMSmessages(portHandle);
            //    if(uCountSMS > 0) {
            //        string strCommand = "AT+CMGD=1,3";
            //        if(msgHandle.DeleteMsg(portHandle, strCommand)) {
            //            Console.WriteLine("MEssages succesffully deleted");
            //        } else {
            //            Console.WriteLine("Failed to delete messgaes");
            //        }
            //    } else {
            //        Console.WriteLine("No messages to delete");
            //    }

            //} catch(Exception ex) {
            //    Console.WriteLine("An exception occured : {0}", ex.Message);
            //}
            //#endregion

            //Console.ReadLine();
            //msgHandle.ClosePort(portHandle);
            
            
        }
    }
}
