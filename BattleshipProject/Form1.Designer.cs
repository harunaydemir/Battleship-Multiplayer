namespace BattleshipProject
{
    partial class Form1
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pnlPlayer = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pnlEnemy = new System.Windows.Forms.Panel();
            this.rtbOutput = new System.Windows.Forms.RichTextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.rbClient = new System.Windows.Forms.RadioButton();
            this.rbServer = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnGonder = new System.Windows.Forms.Button();
            this.txtSohbetMesaji = new System.Windows.Forms.TextBox();
            this.rtbChat = new System.Windows.Forms.RichTextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnTorpedo = new System.Windows.Forms.Button();
            this.btnRadar = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pnlPlayer);
            this.groupBox1.Location = new System.Drawing.Point(1, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(477, 444);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Your ships:";
            // 
            // pnlPlayer
            // 
            this.pnlPlayer.Location = new System.Drawing.Point(11, 63);
            this.pnlPlayer.Name = "pnlPlayer";
            this.pnlPlayer.Size = new System.Drawing.Size(446, 330);
            this.pnlPlayer.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pnlEnemy);
            this.groupBox2.Location = new System.Drawing.Point(498, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(578, 444);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Your enemy:";
            // 
            // pnlEnemy
            // 
            this.pnlEnemy.Location = new System.Drawing.Point(16, 63);
            this.pnlEnemy.Name = "pnlEnemy";
            this.pnlEnemy.Size = new System.Drawing.Size(493, 330);
            this.pnlEnemy.TabIndex = 0;
            // 
            // rtbOutput
            // 
            this.rtbOutput.Location = new System.Drawing.Point(25, 479);
            this.rtbOutput.Name = "rtbOutput";
            this.rtbOutput.Size = new System.Drawing.Size(711, 165);
            this.rtbOutput.TabIndex = 0;
            this.rtbOutput.Text = "Output";
            this.rtbOutput.TextChanged += new System.EventHandler(this.rtbOutput_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.txtPort);
            this.groupBox3.Controls.Add(this.txtIP);
            this.groupBox3.Controls.Add(this.rbClient);
            this.groupBox3.Controls.Add(this.rbServer);
            this.groupBox3.Location = new System.Drawing.Point(1104, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(273, 274);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Bağlantı Ayarları";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(26, 178);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 39);
            this.button1.TabIndex = 6;
            this.button1.Text = "Bağlan";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 144);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Port:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(22, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "IP:";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(66, 138);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(100, 22);
            this.txtPort.TabIndex = 3;
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(66, 110);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(100, 22);
            this.txtIP.TabIndex = 2;
            // 
            // rbClient
            // 
            this.rbClient.AutoSize = true;
            this.rbClient.Location = new System.Drawing.Point(18, 60);
            this.rbClient.Name = "rbClient";
            this.rbClient.Size = new System.Drawing.Size(107, 20);
            this.rbClient.TabIndex = 1;
            this.rbClient.TabStop = true;
            this.rbClient.Text = "Play as client";
            this.rbClient.UseVisualStyleBackColor = true;
            this.rbClient.CheckedChanged += new System.EventHandler(this.rbClient_CheckedChanged);
            // 
            // rbServer
            // 
            this.rbServer.AutoSize = true;
            this.rbServer.Location = new System.Drawing.Point(18, 34);
            this.rbServer.Name = "rbServer";
            this.rbServer.Size = new System.Drawing.Size(114, 20);
            this.rbServer.TabIndex = 0;
            this.rbServer.TabStop = true;
            this.rbServer.Text = "Play as server";
            this.rbServer.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnGonder);
            this.groupBox4.Controls.Add(this.txtSohbetMesaji);
            this.groupBox4.Controls.Add(this.rtbChat);
            this.groupBox4.Location = new System.Drawing.Point(1104, 312);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(273, 225);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Sohbet";
            // 
            // btnGonder
            // 
            this.btnGonder.Location = new System.Drawing.Point(95, 192);
            this.btnGonder.Name = "btnGonder";
            this.btnGonder.Size = new System.Drawing.Size(93, 25);
            this.btnGonder.TabIndex = 2;
            this.btnGonder.Text = "Gönder";
            this.btnGonder.UseVisualStyleBackColor = true;
            this.btnGonder.Click += new System.EventHandler(this.btnGonder_Click);
            // 
            // txtSohbetMesaji
            // 
            this.txtSohbetMesaji.Location = new System.Drawing.Point(18, 164);
            this.txtSohbetMesaji.Name = "txtSohbetMesaji";
            this.txtSohbetMesaji.Size = new System.Drawing.Size(241, 22);
            this.txtSohbetMesaji.TabIndex = 1;
            // 
            // rtbChat
            // 
            this.rtbChat.Location = new System.Drawing.Point(9, 21);
            this.rtbChat.Name = "rtbChat";
            this.rtbChat.Size = new System.Drawing.Size(258, 137);
            this.rtbChat.TabIndex = 0;
            this.rtbChat.Text = "";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnTorpedo);
            this.groupBox5.Controls.Add(this.btnRadar);
            this.groupBox5.Location = new System.Drawing.Point(1098, 575);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(278, 190);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Özel Yetenekler";
            // 
            // btnTorpedo
            // 
            this.btnTorpedo.Location = new System.Drawing.Point(16, 69);
            this.btnTorpedo.Name = "btnTorpedo";
            this.btnTorpedo.Size = new System.Drawing.Size(179, 23);
            this.btnTorpedo.TabIndex = 1;
            this.btnTorpedo.Text = "Torpido Kullan (Kalan: 2)";
            this.btnTorpedo.UseVisualStyleBackColor = true;
            this.btnTorpedo.Click += new System.EventHandler(this.btnTorpedo_Click);
            // 
            // btnRadar
            // 
            this.btnRadar.Location = new System.Drawing.Point(16, 39);
            this.btnRadar.Name = "btnRadar";
            this.btnRadar.Size = new System.Drawing.Size(179, 23);
            this.btnRadar.TabIndex = 0;
            this.btnRadar.Text = "Radar Kullan (Kalan: 3)";
            this.btnRadar.UseVisualStyleBackColor = true;
            this.btnRadar.Click += new System.EventHandler(this.btnRadar_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1465, 815);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.rtbOutput);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "BattleShip v1.0";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel pnlPlayer;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel pnlEnemy;
        private System.Windows.Forms.RichTextBox rtbOutput;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbClient;
        private System.Windows.Forms.RadioButton rbServer;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnGonder;
        private System.Windows.Forms.TextBox txtSohbetMesaji;
        private System.Windows.Forms.RichTextBox rtbChat;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnTorpedo;
        private System.Windows.Forms.Button btnRadar;
    }
}

