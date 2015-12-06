using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace AMSMSService {
    public class ServiceHelper {

        private class MessageOutputter {
            private IList<string> msgList = new System.Collections.Generic.List<string>();
            private string msgPrefix;
            private bool isConsoleAvailable;


            public bool IsConsoleAvailable {
                get { return isConsoleAvailable; }
            }

            public MessageOutputter(string msgPrefix) {
                this.msgPrefix = msgPrefix;

                // TODO: dla mono wstawiam tak, ale to tymczasowo
                try {
#if WINDOWS
                    // On Win7, when project type is "Windows Application" following line will not raise exception
                    // but on Win2k3/WinXP exception will be raised
                    this.isConsoleAvailable = string.IsNullOrEmpty(Console.Title) == false;
#else
                    this.isConsoleAvailable = true;
#endif
                } catch {
                    this.isConsoleAvailable = false;
                }
                System.Diagnostics.Trace.WriteLine(string.Format("IsConsoleAvailable: {0}", this.isConsoleAvailable));
            }

            public void AddMessage(string msg) {
                msg = string.Format("{0}{1}", this.msgPrefix, msg);

                System.Diagnostics.Trace.WriteLine(msg);
                if(this.isConsoleAvailable) {
                    // UWAGA: Aby ponizsze zadzialalo to usluga musi byc typu "Console Application" a nie "Windows Application"
                    Console.WriteLine(msg);
                } else
                    this.msgList.Add(msg);
            }

            public void Flush() {
                if(this.isConsoleAvailable) {
                    foreach(string msg in msgList)
                        Console.WriteLine(msg);
                } else {
                    string fullMsg = string.Empty;
                    foreach(string msg in msgList)
                        fullMsg = fullMsg + (fullMsg.Length > 0 ? Environment.NewLine : string.Empty) + msg;
                    //System.Diagnostics.Trace.WriteLine(string.Format("[Flush] UserInteractive mode: {0}", Environment.UserInteractive));
                    if(Environment.UserInteractive)
                        MessageBox.Show(fullMsg);
                    else
                        System.Diagnostics.Trace.WriteLine(string.Format("[Flush] {0}", fullMsg));
                }

                msgList.Clear();
            }


        }

        private static void DisplayServiceAppHelp(MessageOutputter msgOutputter, string svcName, string svcLocation) {
            msgOutputter.AddMessage("Command line parameters");
            msgOutputter.AddMessage("  -install\t- install service");
            msgOutputter.AddMessage("  -uninstall\t- uninstall service");
            msgOutputter.AddMessage("  -console\t- run service in console/debug (interactive) mode");
            msgOutputter.AddMessage("  -setup\t- open configuration window");
            msgOutputter.AddMessage(string.Empty);
        }

        public static void ServiceAppMainEntry<AppServiceType>(string[] args, string serviceName, string svcLocation) where AppServiceType : AppServiceBase, new() {
            MessageOutputter msgOutputter = new MessageOutputter(string.Empty);
            try {
                msgOutputter.AddMessage("************************* App Service Tool *************************");
                msgOutputter.AddMessage(string.Format("Service name: {0}", serviceName, svcLocation));
                msgOutputter.AddMessage(string.Format("Service location: {0}", svcLocation));
                msgOutputter.AddMessage(string.Format("UserInteractive mode: {0}", Environment.UserInteractive));
                msgOutputter.AddMessage(string.Format("IsConsoleAvailable: {0}", msgOutputter.IsConsoleAvailable));

                msgOutputter.AddMessage(string.Empty);

                if(args.Length > 0) {
                    switch(args[0].ToLower()) {

                        case "-c":
                        case "-d":
                        case "-debug":
                        case "-console":
                        //if (servicesToRun.Length != 1)
                        //    throw new Exception("Could not start service in console/debug (interactive) mode - servicesToRun parameter should contain only one service instance.");

                        if(msgOutputter.IsConsoleAvailable) {
                            msgOutputter.AddMessage("Starting service in console/debug (interactive) mode...");
                            AppServiceType serviceToRun = new AppServiceType();
                            serviceToRun.RunInConsoleMode();
                        } else
                            msgOutputter.AddMessage("Could not run in console mode. Console not available - you should change application type to \"Console Application\".");

                        break;
                        case "-i":
                        case "-install":
                        msgOutputter.AddMessage("Attempt to install service...");
                        try {
                            msgOutputter.AddMessage(string.Format("Service name: {0}; assembly: {1}", serviceName, svcLocation));

                            msgOutputter.AddMessage("Running AssemblyInstaller.Install() method...");
                            AssemblyInstaller installer = new AssemblyInstaller();
                            //installer.Path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                            installer.Path = svcLocation;
                            installer.UseNewContext = true;
                            installer.Install(null);
                            installer.Commit(null);
                            msgOutputter.AddMessage("DONE.");

                        } catch(Exception ex) {
                            msgOutputter.AddMessage("Error installing service: " + ex.Message);
                        }

                        break;
                        case "-u":
                        case "-uninstall":
                        msgOutputter.AddMessage("Attempt to uninstall service...");

                        try {
                            //*
                            msgOutputter.AddMessage(string.Format("Service name: {0}; assembly: {1}", serviceName, svcLocation));

                            ServiceController controller = new ServiceController(serviceName);
                            if(controller.Status != ServiceControllerStatus.Stopped) {
                                msgOutputter.AddMessage(string.Format("Trying to stop service: {0}", serviceName));
                                controller.Stop();
                            }

                            msgOutputter.AddMessage("Running AssemblyInstaller.Uninstall() method...");
                            AssemblyInstaller installer = new AssemblyInstaller();
                            //installer.Path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                            installer.Path = svcLocation;
                            installer.UseNewContext = true;
                            installer.Uninstall(null);
                        } catch(Exception ex) {
                            msgOutputter.AddMessage("Error uninstalling service: " + ex.Message);
                        }

                        break;
                        default:
                        msgOutputter.AddMessage("Unrecognized command line parameter(s).");
                        DisplayServiceAppHelp(msgOutputter, serviceName, svcLocation);
                        break;
                    }
                } else
                    if(Environment.UserInteractive) DisplayServiceAppHelp(msgOutputter, serviceName, svcLocation);

                if(Environment.UserInteractive == false) {
                    msgOutputter.AddMessage("Creating service instance and registering in SCM...");
                    AppServiceType serviceToRun = new AppServiceType();
                    ServiceBase.Run(serviceToRun);
                }
            } catch(Exception ex) {
                msgOutputter.AddMessage("Error: " + ex.Message);
            }

            msgOutputter.Flush();
        }
    }
}
