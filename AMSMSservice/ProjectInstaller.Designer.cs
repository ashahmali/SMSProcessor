namespace AMSMSService {
    partial class AMSMSServiceInstaller {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.AMSMSServicesInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.AMSMSService = new System.ServiceProcess.ServiceInstaller();
            // 
            // AMSMSServicesInstaller
            // 
            this.AMSMSServicesInstaller.Password = null;
            this.AMSMSServicesInstaller.Username = null;
            // 
            // AMSMSService
            // 
            this.AMSMSService.Description = "Sends and Recieve SMS";
            this.AMSMSService.DisplayName = "Adaptive SMS Service";
            this.AMSMSService.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.AMSMSServicesInstaller});
            this.AMSMSService.ServiceName = "AMSMSService";
            this.AMSMSService.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.AMSMSService.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstaller1_AfterInstall);
            // 
            // AMSMSServiceInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.AMSMSService});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller AMSMSServicesInstaller;
        private System.ServiceProcess.ServiceInstaller AMSMSService;
    }
}