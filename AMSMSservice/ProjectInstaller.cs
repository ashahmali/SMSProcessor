using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;


namespace AMSMSService {
    [ObfuscationAttribute(Exclude = true)]
    [RunInstaller(true)]
    public partial class AMSMSServiceInstaller : System.Configuration.Install.Installer {
        public AMSMSServiceInstaller() {
            InitializeComponent();
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e) {
           
        }
    }
}
