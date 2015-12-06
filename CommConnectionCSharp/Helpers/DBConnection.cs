using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using AMSMSService;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AMSMSService{
    public static class DBConnection {

        private static string connectionString = "Data Source=dev10\\AMCR;Initial Catalog=Adaptive;User ID=sa;Password=5erV3RPa55!";
        private static SqlConnection cnn = new SqlConnection(connectionString);
        private static bool isConnected = false;
        private static SqlDataReader dataReader;

        private static bool Connect() {
            try {
                if(!isConnected)
                cnn.Open();
                //Logger.WriteLineToFile("<Connect> Database Connection successful!");
                return isConnected = true;
                
            } catch(Exception ex) {
                Logger.WriteLineToFile(string.Format("<Connect> Database Connection failed with Error: {0}", ex.Message));
                return false;
            }
        }

        private static void Disconnect() {
            if(isConnected) {
                cnn.Close();
                isConnected = false;
                //Logger.WriteLineToFile("<Disconnect> Database Connection Closed");
            }
        }

       public static bool executeSP(String vars){
           bool executed = false;
           if(Connect()) {
               string sql = "EXEC AddInboundSMS" +  vars;
               try {
                   SqlCommand a = new SqlCommand(sql, cnn);
                   a.ExecuteNonQuery();
                   a.Dispose();
                   Logger.WriteLineToFile("<executeSP> Procedure Executed");
                   Disconnect();
                   executed = true;
               } catch(Exception Ex) {
                   Logger.WriteLineToFile(String.Format("<executeSP> An Exception occured with message; {0}", Ex.Message));
               }
           }
           return  executed;
       }

       public static void GetMessagesToBeSent() {
           if(Connect()) {
               string sql = "select Outbound_Queue.id, Outbound_Queue.message, Outbound_Queue.isBulk, Outbound_Queue.port, Outbound_Queue_Recipients.recipient from Outbound_Queue inner join Outbound_Queue_Recipients on (Outbound_Queue.id = Outbound_Queue_Recipients.Outbound_ID AND Outbound_Queue.isSMS = 1) where Outbound_Queue.processed IS NULL and Outbound_Queue.enqueued = 0 and forDispatch < GETDATE()";
               try {
                   SqlCommand b = new SqlCommand(sql, cnn);
                   dataReader = b.ExecuteReader();
                   var ids = new List<string>();
                   var last_message = string.Empty;
                   while(dataReader.Read()) {
                       if(dataReader.GetValue(1).ToString() == string.Empty) {
                           textMessage.messagestobesent.Enqueue(new textMessage { Index = dataReader.GetValue(0).ToString(), Message = last_message, Sender = dataReader.GetValue(4).ToString(), MCommPort = dataReader.GetValue(3).ToString() });
                       } else {
                           textMessage.messagestobesent.Enqueue(new textMessage { Index = dataReader.GetValue(0).ToString(), Message = dataReader.GetValue(1).ToString(), Sender = dataReader.GetValue(4).ToString(), MCommPort = dataReader.GetValue(3).ToString() });
                           last_message = dataReader.GetValue(1).ToString();
                       }
                       ids.Add(dataReader.GetValue(0).ToString());
                       
                   }
                   dataReader.Close();
                   b.Dispose();
                   foreach(var item in ids)
                       Updatequeued(item);
                   if(ids.Count() > 0)
                       Logger.WriteLineToFile("<GetMessagesToBeSent> Fetched Messages to be sent");
                   Disconnect();
               } catch(Exception Ex) {
                   Logger.WriteLineToFile(String.Format("<GetMessagesToBeSent> An Exception occured with message; {0}", Ex.Message));
               }
           }
       }

       public static void getConnectedPorts() {
           if(Connect()) {
               string sql = "select * from SMS_Modems";
               try {
                   SqlCommand c = new SqlCommand(sql, cnn);
                   dataReader = c.ExecuteReader();
                   while(dataReader.Read()) {
                       SMSModem.modems.Add(new SMSModem { id = dataReader.GetValue(0).ToString(), comPort = dataReader.GetValue(1).ToString(), name = dataReader.GetValue(2).ToString(), Number = dataReader.GetValue(3).ToString(), receive = dataReader.GetValue(4).ToString(), send = dataReader.GetValue(5).ToString() });
                   }
                   dataReader.Close();
                   c.Dispose();
                   Logger.WriteLineToFile("<getConnectedPorts> Fetched connected GSM modems");
                   Disconnect();
               } catch(Exception Ex) {
                   Logger.WriteLineToFile(String.Format("<getConnectedPorts> An Exception occured with message; {0}", Ex.Message));
               }
           }
       }

       public static void Updateprocessed(string id) {
           if(Connect()) {
               string sql = "update Outbound_Queue set processed = GETDATE() where id = "+id;
               Task.Factory.StartNew(() => {
                   try {
                       SqlCommand d = new SqlCommand(sql, cnn);
                       d.ExecuteNonQuery();
                       d.Dispose();
                       Logger.WriteLineToFile("<Updateprocessed> Message Processed");
                       Disconnect();
                   } catch(Exception Ex) {
                       Logger.WriteLineToFile(String.Format("<Updateprocessed> An Exception occured with message; {0}", Ex.Message));
                   }

               });
           }
       }

       public static void Updatequeued(string id) {
           if(Connect()) {
               string sql = "update Outbound_Queue set enqueued = 1 where id = " + id;
               try {
                   SqlCommand e = new SqlCommand(sql, cnn);
                   e.ExecuteNonQuery();
                   e.Dispose();
                   Disconnect();
               } catch(Exception Ex) {
                   Logger.WriteLineToFile(String.Format("<Updatequeued> An Exception occured with message; {0}", Ex.Message));
               }
           }
       }
    }
}
