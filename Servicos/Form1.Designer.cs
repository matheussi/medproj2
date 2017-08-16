namespace Servicos
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblImportAgendStatus = new System.Windows.Forms.Label();
            this.cmdImportAgendIniciar = new System.Windows.Forms.Button();
            this.timerImportAgend = new System.Windows.Forms.Timer(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblExportAgendCartaoStatus = new System.Windows.Forms.Label();
            this.cmdExportAgendCartaoIniciar = new System.Windows.Forms.Button();
            this.timerExportCartaoAgend = new System.Windows.Forms.Timer(this.components);
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblExportAgendKitStatus = new System.Windows.Forms.Label();
            this.cmdExportAgendKitIniciar = new System.Windows.Forms.Button();
            this.timerExportKitAgend = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cmdProcsXPrestadores = new System.Windows.Forms.Button();
            this.tProcsPrestadores = new System.Windows.Forms.Timer(this.components);
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cmdArquivosDePagamento = new System.Windows.Forms.Button();
            this.tArquivoPagamento = new System.Windows.Forms.Timer(this.components);
            this.cmdComunicacaoEmail = new System.Windows.Forms.Button();
            this.tComunicacaoEmail = new System.Windows.Forms.Timer(this.components);
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.lblAvisosPorEmail = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblImportAgendStatus);
            this.groupBox1.Controls.Add(this.cmdImportAgendIniciar);
            this.groupBox1.Location = new System.Drawing.Point(3, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(277, 88);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Importações agendadas";
            // 
            // lblImportAgendStatus
            // 
            this.lblImportAgendStatus.AutoSize = true;
            this.lblImportAgendStatus.Location = new System.Drawing.Point(20, 42);
            this.lblImportAgendStatus.Name = "lblImportAgendStatus";
            this.lblImportAgendStatus.Size = new System.Drawing.Size(50, 13);
            this.lblImportAgendStatus.TabIndex = 1;
            this.lblImportAgendStatus.Text = "Parado...";
            // 
            // cmdImportAgendIniciar
            // 
            this.cmdImportAgendIniciar.Location = new System.Drawing.Point(194, 35);
            this.cmdImportAgendIniciar.Name = "cmdImportAgendIniciar";
            this.cmdImportAgendIniciar.Size = new System.Drawing.Size(75, 23);
            this.cmdImportAgendIniciar.TabIndex = 0;
            this.cmdImportAgendIniciar.Text = "Iniciar";
            this.cmdImportAgendIniciar.UseVisualStyleBackColor = true;
            this.cmdImportAgendIniciar.Click += new System.EventHandler(this.cmdImportAgendIniciar_Click);
            // 
            // timerImportAgend
            // 
            this.timerImportAgend.Interval = 3600000;
            this.timerImportAgend.Tick += new System.EventHandler(this.timerImportAgend_Tick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblExportAgendCartaoStatus);
            this.groupBox2.Controls.Add(this.cmdExportAgendCartaoIniciar);
            this.groupBox2.Location = new System.Drawing.Point(4, 107);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(277, 88);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Exportações agendadas - Cartão";
            // 
            // lblExportAgendCartaoStatus
            // 
            this.lblExportAgendCartaoStatus.AutoSize = true;
            this.lblExportAgendCartaoStatus.Location = new System.Drawing.Point(20, 42);
            this.lblExportAgendCartaoStatus.Name = "lblExportAgendCartaoStatus";
            this.lblExportAgendCartaoStatus.Size = new System.Drawing.Size(50, 13);
            this.lblExportAgendCartaoStatus.TabIndex = 1;
            this.lblExportAgendCartaoStatus.Text = "Parado...";
            // 
            // cmdExportAgendCartaoIniciar
            // 
            this.cmdExportAgendCartaoIniciar.Location = new System.Drawing.Point(194, 35);
            this.cmdExportAgendCartaoIniciar.Name = "cmdExportAgendCartaoIniciar";
            this.cmdExportAgendCartaoIniciar.Size = new System.Drawing.Size(75, 23);
            this.cmdExportAgendCartaoIniciar.TabIndex = 0;
            this.cmdExportAgendCartaoIniciar.Text = "Iniciar";
            this.cmdExportAgendCartaoIniciar.UseVisualStyleBackColor = true;
            this.cmdExportAgendCartaoIniciar.Click += new System.EventHandler(this.cmdExportAgendCartaoIniciar_Click);
            // 
            // timerExportCartaoAgend
            // 
            this.timerExportCartaoAgend.Interval = 1800000;
            this.timerExportCartaoAgend.Tick += new System.EventHandler(this.timerExportCartaoAgend_Tick);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblExportAgendKitStatus);
            this.groupBox3.Controls.Add(this.cmdExportAgendKitIniciar);
            this.groupBox3.Location = new System.Drawing.Point(300, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(277, 88);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Exportações agendadas - KIT";
            // 
            // lblExportAgendKitStatus
            // 
            this.lblExportAgendKitStatus.AutoSize = true;
            this.lblExportAgendKitStatus.Location = new System.Drawing.Point(20, 42);
            this.lblExportAgendKitStatus.Name = "lblExportAgendKitStatus";
            this.lblExportAgendKitStatus.Size = new System.Drawing.Size(50, 13);
            this.lblExportAgendKitStatus.TabIndex = 1;
            this.lblExportAgendKitStatus.Text = "Parado...";
            // 
            // cmdExportAgendKitIniciar
            // 
            this.cmdExportAgendKitIniciar.Location = new System.Drawing.Point(194, 35);
            this.cmdExportAgendKitIniciar.Name = "cmdExportAgendKitIniciar";
            this.cmdExportAgendKitIniciar.Size = new System.Drawing.Size(75, 23);
            this.cmdExportAgendKitIniciar.TabIndex = 0;
            this.cmdExportAgendKitIniciar.Text = "Iniciar";
            this.cmdExportAgendKitIniciar.UseVisualStyleBackColor = true;
            this.cmdExportAgendKitIniciar.Click += new System.EventHandler(this.cmdExportAgendKitIniciar_Click);
            // 
            // timerExportKitAgend
            // 
            this.timerExportKitAgend.Interval = 1800000;
            this.timerExportKitAgend.Tick += new System.EventHandler(this.timerExportKitAgend_Tick);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Aquamarine;
            this.button1.Location = new System.Drawing.Point(6, 48);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cmdProcsXPrestadores);
            this.groupBox4.Location = new System.Drawing.Point(300, 107);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(277, 88);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Procedimentos x Prestadores";
            // 
            // cmdProcsXPrestadores
            // 
            this.cmdProcsXPrestadores.Location = new System.Drawing.Point(194, 37);
            this.cmdProcsXPrestadores.Name = "cmdProcsXPrestadores";
            this.cmdProcsXPrestadores.Size = new System.Drawing.Size(75, 23);
            this.cmdProcsXPrestadores.TabIndex = 1;
            this.cmdProcsXPrestadores.Text = "Iniciar";
            this.cmdProcsXPrestadores.UseVisualStyleBackColor = true;
            this.cmdProcsXPrestadores.Click += new System.EventHandler(this.cmdProcsXPrestadores_Click);
            // 
            // tProcsPrestadores
            // 
            this.tProcsPrestadores.Enabled = true;
            this.tProcsPrestadores.Interval = 800000;
            this.tProcsPrestadores.Tick += new System.EventHandler(this.tProcsPrestadores_Tick);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cmdArquivosDePagamento);
            this.groupBox5.Location = new System.Drawing.Point(4, 210);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(277, 88);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Arquivos de pagamento";
            // 
            // cmdArquivosDePagamento
            // 
            this.cmdArquivosDePagamento.Location = new System.Drawing.Point(194, 37);
            this.cmdArquivosDePagamento.Name = "cmdArquivosDePagamento";
            this.cmdArquivosDePagamento.Size = new System.Drawing.Size(75, 23);
            this.cmdArquivosDePagamento.TabIndex = 1;
            this.cmdArquivosDePagamento.Text = "Iniciar";
            this.cmdArquivosDePagamento.UseVisualStyleBackColor = true;
            this.cmdArquivosDePagamento.Click += new System.EventHandler(this.cmdArquivosDePagamento_Click);
            // 
            // tArquivoPagamento
            // 
            this.tArquivoPagamento.Enabled = true;
            this.tArquivoPagamento.Interval = 4500000;
            this.tArquivoPagamento.Tick += new System.EventHandler(this.tArquivoPagamento_Tick);
            // 
            // cmdComunicacaoEmail
            // 
            this.cmdComunicacaoEmail.Location = new System.Drawing.Point(193, 48);
            this.cmdComunicacaoEmail.Name = "cmdComunicacaoEmail";
            this.cmdComunicacaoEmail.Size = new System.Drawing.Size(75, 23);
            this.cmdComunicacaoEmail.TabIndex = 6;
            this.cmdComunicacaoEmail.Text = "Iniciar";
            this.cmdComunicacaoEmail.UseVisualStyleBackColor = true;
            this.cmdComunicacaoEmail.Click += new System.EventHandler(this.cmdComunicacaoEmail_Click);
            // 
            // tComunicacaoEmail
            // 
            this.tComunicacaoEmail.Enabled = true;
            this.tComunicacaoEmail.Interval = 9000000;
            this.tComunicacaoEmail.Tick += new System.EventHandler(this.tComunicacaoEmail_Tick);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.lblAvisosPorEmail);
            this.groupBox6.Controls.Add(this.cmdComunicacaoEmail);
            this.groupBox6.Controls.Add(this.button1);
            this.groupBox6.Location = new System.Drawing.Point(300, 210);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(277, 90);
            this.groupBox6.TabIndex = 7;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Avisos por email";
            // 
            // lblAvisosPorEmail
            // 
            this.lblAvisosPorEmail.AutoSize = true;
            this.lblAvisosPorEmail.Location = new System.Drawing.Point(7, 29);
            this.lblAvisosPorEmail.Name = "lblAvisosPorEmail";
            this.lblAvisosPorEmail.Size = new System.Drawing.Size(22, 13);
            this.lblAvisosPorEmail.TabIndex = 7;
            this.lblAvisosPorEmail.Text = "[...]";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 312);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Processos CESA";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button cmdImportAgendIniciar;
        private System.Windows.Forms.Label lblImportAgendStatus;
        private System.Windows.Forms.Timer timerImportAgend;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblExportAgendCartaoStatus;
        private System.Windows.Forms.Button cmdExportAgendCartaoIniciar;
        private System.Windows.Forms.Timer timerExportCartaoAgend;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblExportAgendKitStatus;
        private System.Windows.Forms.Button cmdExportAgendKitIniciar;
        private System.Windows.Forms.Timer timerExportKitAgend;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button cmdProcsXPrestadores;
        private System.Windows.Forms.Timer tProcsPrestadores;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button cmdArquivosDePagamento;
        private System.Windows.Forms.Timer tArquivoPagamento;
        private System.Windows.Forms.Button cmdComunicacaoEmail;
        private System.Windows.Forms.Timer tComunicacaoEmail;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label lblAvisosPorEmail;
    }
}

