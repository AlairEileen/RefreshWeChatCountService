namespace RefreshWeChatCountService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceRefreshCountDataProjInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceRefreshCountDataInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceRefreshCountDataProjInstaller
            // 
            this.serviceRefreshCountDataProjInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceRefreshCountDataProjInstaller.Password = null;
            this.serviceRefreshCountDataProjInstaller.Username = null;
            // 
            // serviceRefreshCountDataInstaller
            // 
            this.serviceRefreshCountDataInstaller.Description = "刷新微擎及微信统计数据";
            this.serviceRefreshCountDataInstaller.DisplayName = "RefreshWe7&WeChatCountData";
            this.serviceRefreshCountDataInstaller.ServiceName = "ServiceRefreshCountData";
            this.serviceRefreshCountDataInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceRefreshCountDataProjInstaller,
            this.serviceRefreshCountDataInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceRefreshCountDataProjInstaller;
        private System.ServiceProcess.ServiceInstaller serviceRefreshCountDataInstaller;
    }
}