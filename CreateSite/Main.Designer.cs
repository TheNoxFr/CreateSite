namespace CreateSite
{
    partial class Main
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtLogin = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.BtnCiscoFile = new System.Windows.Forms.Button();
            this.BtnGenesysFile = new System.Windows.Forms.Button();
            this.rtConfig = new System.Windows.Forms.RichTextBox();
            this.treeObjects = new System.Windows.Forms.TreeView();
            this.rtLog = new System.Windows.Forms.RichTextBox();
            this.lblCodeUGS = new System.Windows.Forms.Label();
            this.lblVille = new System.Windows.Forms.Label();
            this.BtnVerifExiste = new System.Windows.Forms.Button();
            this.BtnCreer = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtServeur = new System.Windows.Forms.TextBox();
            this.gbInitButton = new System.Windows.Forms.GroupBox();
            this.gbFinalButton = new System.Windows.Forms.GroupBox();
            this.gbInitButton.SuspendLayout();
            this.gbFinalButton.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtLogin
            // 
            this.txtLogin.Location = new System.Drawing.Point(271, 16);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(100, 20);
            this.txtLogin.TabIndex = 0;
            this.txtLogin.Text = "default";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(464, 16);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(100, 20);
            this.txtPassword.TabIndex = 1;
            this.txtPassword.Text = "password";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(226, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Login :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(399, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Password :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(803, 89);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Logs";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(327, 89);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Objets";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Config Spécifique";
            // 
            // BtnCiscoFile
            // 
            this.BtnCiscoFile.Location = new System.Drawing.Point(33, 9);
            this.BtnCiscoFile.Name = "BtnCiscoFile";
            this.BtnCiscoFile.Size = new System.Drawing.Size(149, 59);
            this.BtnCiscoFile.TabIndex = 11;
            this.BtnCiscoFile.Text = "Cisco";
            this.BtnCiscoFile.UseVisualStyleBackColor = true;
            this.BtnCiscoFile.Click += new System.EventHandler(this.BtnCiscoFile_Click);
            // 
            // BtnGenesysFile
            // 
            this.BtnGenesysFile.Location = new System.Drawing.Point(226, 9);
            this.BtnGenesysFile.Name = "BtnGenesysFile";
            this.BtnGenesysFile.Size = new System.Drawing.Size(160, 59);
            this.BtnGenesysFile.TabIndex = 12;
            this.BtnGenesysFile.Text = "Genesys";
            this.BtnGenesysFile.UseVisualStyleBackColor = true;
            this.BtnGenesysFile.Click += new System.EventHandler(this.BtnGenesysFile_Click);
            // 
            // rtConfig
            // 
            this.rtConfig.BackColor = System.Drawing.Color.Silver;
            this.rtConfig.Location = new System.Drawing.Point(21, 110);
            this.rtConfig.Name = "rtConfig";
            this.rtConfig.Size = new System.Drawing.Size(273, 704);
            this.rtConfig.TabIndex = 13;
            this.rtConfig.Text = "";
            // 
            // treeObjects
            // 
            this.treeObjects.BackColor = System.Drawing.Color.Silver;
            this.treeObjects.CheckBoxes = true;
            this.treeObjects.Location = new System.Drawing.Point(330, 110);
            this.treeObjects.Name = "treeObjects";
            this.treeObjects.Size = new System.Drawing.Size(429, 704);
            this.treeObjects.TabIndex = 14;
            this.treeObjects.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeObjects_AfterCheck);
            // 
            // rtLog
            // 
            this.rtLog.BackColor = System.Drawing.Color.Silver;
            this.rtLog.Location = new System.Drawing.Point(806, 110);
            this.rtLog.Name = "rtLog";
            this.rtLog.ReadOnly = true;
            this.rtLog.Size = new System.Drawing.Size(525, 704);
            this.rtLog.TabIndex = 15;
            this.rtLog.Text = "";
            // 
            // lblCodeUGS
            // 
            this.lblCodeUGS.AutoSize = true;
            this.lblCodeUGS.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCodeUGS.ForeColor = System.Drawing.Color.Blue;
            this.lblCodeUGS.Location = new System.Drawing.Point(43, 25);
            this.lblCodeUGS.Name = "lblCodeUGS";
            this.lblCodeUGS.Size = new System.Drawing.Size(47, 37);
            this.lblCodeUGS.TabIndex = 16;
            this.lblCodeUGS.Text = "...";
            // 
            // lblVille
            // 
            this.lblVille.AutoSize = true;
            this.lblVille.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVille.ForeColor = System.Drawing.Color.Blue;
            this.lblVille.Location = new System.Drawing.Point(127, 25);
            this.lblVille.Name = "lblVille";
            this.lblVille.Size = new System.Drawing.Size(47, 37);
            this.lblVille.TabIndex = 17;
            this.lblVille.Text = "...";
            // 
            // BtnVerifExiste
            // 
            this.BtnVerifExiste.BackColor = System.Drawing.Color.Transparent;
            this.BtnVerifExiste.ForeColor = System.Drawing.SystemColors.ControlText;
            this.BtnVerifExiste.Location = new System.Drawing.Point(615, 8);
            this.BtnVerifExiste.Name = "BtnVerifExiste";
            this.BtnVerifExiste.Size = new System.Drawing.Size(125, 59);
            this.BtnVerifExiste.TabIndex = 18;
            this.BtnVerifExiste.Text = "Vérif existence";
            this.BtnVerifExiste.UseVisualStyleBackColor = false;
            this.BtnVerifExiste.Click += new System.EventHandler(this.BtnVerifExiste_Click);
            // 
            // BtnCreer
            // 
            this.BtnCreer.Enabled = false;
            this.BtnCreer.Location = new System.Drawing.Point(780, 8);
            this.BtnCreer.Name = "BtnCreer";
            this.BtnCreer.Size = new System.Drawing.Size(125, 59);
            this.BtnCreer.TabIndex = 19;
            this.BtnCreer.Text = "Créer objets";
            this.BtnCreer.UseVisualStyleBackColor = true;
            this.BtnCreer.Click += new System.EventHandler(this.BtnCreer_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "Serveur :";
            // 
            // txtServeur
            // 
            this.txtServeur.Location = new System.Drawing.Point(77, 16);
            this.txtServeur.Name = "txtServeur";
            this.txtServeur.Size = new System.Drawing.Size(134, 20);
            this.txtServeur.TabIndex = 21;
            this.txtServeur.Text = "genserv";
            // 
            // gbInitButton
            // 
            this.gbInitButton.Controls.Add(this.BtnCiscoFile);
            this.gbInitButton.Controls.Add(this.BtnGenesysFile);
            this.gbInitButton.Location = new System.Drawing.Point(300, 12);
            this.gbInitButton.Name = "gbInitButton";
            this.gbInitButton.Size = new System.Drawing.Size(1031, 74);
            this.gbInitButton.TabIndex = 22;
            this.gbInitButton.TabStop = false;
            // 
            // gbFinalButton
            // 
            this.gbFinalButton.Controls.Add(this.BtnCreer);
            this.gbFinalButton.Controls.Add(this.txtLogin);
            this.gbFinalButton.Controls.Add(this.txtServeur);
            this.gbFinalButton.Controls.Add(this.txtPassword);
            this.gbFinalButton.Controls.Add(this.label3);
            this.gbFinalButton.Controls.Add(this.label1);
            this.gbFinalButton.Controls.Add(this.label2);
            this.gbFinalButton.Controls.Add(this.BtnVerifExiste);
            this.gbFinalButton.Location = new System.Drawing.Point(300, 12);
            this.gbFinalButton.Name = "gbFinalButton";
            this.gbFinalButton.Size = new System.Drawing.Size(1031, 74);
            this.gbFinalButton.TabIndex = 23;
            this.gbFinalButton.TabStop = false;
            this.gbFinalButton.Visible = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1392, 840);
            this.Controls.Add(this.gbFinalButton);
            this.Controls.Add(this.gbInitButton);
            this.Controls.Add(this.lblVille);
            this.Controls.Add(this.lblCodeUGS);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.rtConfig);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.treeObjects);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.rtLog);
            this.Name = "Main";
            this.Text = "Création de Site";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.gbInitButton.ResumeLayout(false);
            this.gbFinalButton.ResumeLayout(false);
            this.gbFinalButton.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLogin;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button BtnCiscoFile;
        private System.Windows.Forms.Button BtnGenesysFile;
        private System.Windows.Forms.RichTextBox rtConfig;
        private System.Windows.Forms.TreeView treeObjects;
        private System.Windows.Forms.RichTextBox rtLog;
        private System.Windows.Forms.Label lblCodeUGS;
        private System.Windows.Forms.Label lblVille;
        private System.Windows.Forms.Button BtnVerifExiste;
        private System.Windows.Forms.Button BtnCreer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtServeur;
        private System.Windows.Forms.GroupBox gbInitButton;
        private System.Windows.Forms.GroupBox gbFinalButton;
    }
}

