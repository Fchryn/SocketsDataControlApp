namespace SocketsDataControl
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.txtOprID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCavUse = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtTotalRun = new System.Windows.Forms.TextBox();
            this.btnPlus = new System.Windows.Forms.Button();
            this.txtCavRun1 = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnAddStatus = new System.Windows.Forms.Button();
            this.pictureBoxSocket = new System.Windows.Forms.PictureBox();
            this.lblDateTime = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.combBoxSocketPN = new System.Windows.Forms.ComboBox();
            this.cmbStatus1 = new System.Windows.Forms.ComboBox();
            this.txtCavRun2 = new System.Windows.Forms.TextBox();
            this.txtCavRun7 = new System.Windows.Forms.TextBox();
            this.txtCavRun6 = new System.Windows.Forms.TextBox();
            this.txtCavRun5 = new System.Windows.Forms.TextBox();
            this.txtCavRun4 = new System.Windows.Forms.TextBox();
            this.txtCavRun3 = new System.Windows.Forms.TextBox();
            this.txtCavRun8 = new System.Windows.Forms.TextBox();
            this.cmbStatus2 = new System.Windows.Forms.ComboBox();
            this.cmbStatus4 = new System.Windows.Forms.ComboBox();
            this.cmbStatus3 = new System.Windows.Forms.ComboBox();
            this.cmbStatus8 = new System.Windows.Forms.ComboBox();
            this.cmbStatus7 = new System.Windows.Forms.ComboBox();
            this.cmbStatus6 = new System.Windows.Forms.ComboBox();
            this.cmbStatus5 = new System.Windows.Forms.ComboBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.BtnAdmin = new System.Windows.Forms.Button();
            this.BtnLogout = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSocket)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(501, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(258, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "Sockets Data Control";
            // 
            // txtOprID
            // 
            this.txtOprID.Location = new System.Drawing.Point(112, 100);
            this.txtOprID.Multiline = true;
            this.txtOprID.Name = "txtOprID";
            this.txtOprID.Size = new System.Drawing.Size(121, 35);
            this.txtOprID.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(4, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Operator ID";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(236, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Cavity Use";
            // 
            // txtCavUse
            // 
            this.txtCavUse.Location = new System.Drawing.Point(333, 100);
            this.txtCavUse.Multiline = true;
            this.txtCavUse.Name = "txtCavUse";
            this.txtCavUse.Size = new System.Drawing.Size(82, 35);
            this.txtCavUse.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(4, 169);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 20);
            this.label4.TabIndex = 6;
            this.label4.Text = "Sockets PN";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(9, 229);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 20);
            this.label5.TabIndex = 8;
            this.label5.Text = "Output Qty";
            // 
            // txtOutput
            // 
            this.txtOutput.Location = new System.Drawing.Point(112, 226);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(82, 35);
            this.txtOutput.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(5, 287);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 20);
            this.label6.TabIndex = 10;
            this.label6.Text = "Total Qty IC";
            // 
            // txtTotalRun
            // 
            this.txtTotalRun.Location = new System.Drawing.Point(112, 280);
            this.txtTotalRun.Multiline = true;
            this.txtTotalRun.Name = "txtTotalRun";
            this.txtTotalRun.Size = new System.Drawing.Size(168, 35);
            this.txtTotalRun.TabIndex = 9;
            // 
            // btnPlus
            // 
            this.btnPlus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPlus.Location = new System.Drawing.Point(200, 218);
            this.btnPlus.Name = "btnPlus";
            this.btnPlus.Size = new System.Drawing.Size(80, 55);
            this.btnPlus.TabIndex = 11;
            this.btnPlus.Text = "+";
            this.btnPlus.UseVisualStyleBackColor = true;
            // 
            // txtCavRun1
            // 
            this.txtCavRun1.Location = new System.Drawing.Point(426, 100);
            this.txtCavRun1.Multiline = true;
            this.txtCavRun1.Name = "txtCavRun1";
            this.txtCavRun1.Size = new System.Drawing.Size(100, 35);
            this.txtCavRun1.TabIndex = 12;
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(161, 435);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(101, 55);
            this.btnSave.TabIndex = 28;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnAddStatus
            // 
            this.btnAddStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddStatus.Location = new System.Drawing.Point(29, 429);
            this.btnAddStatus.Name = "btnAddStatus";
            this.btnAddStatus.Size = new System.Drawing.Size(113, 69);
            this.btnAddStatus.TabIndex = 29;
            this.btnAddStatus.Text = "Add Status";
            this.btnAddStatus.UseVisualStyleBackColor = true;
            // 
            // pictureBoxSocket
            // 
            this.pictureBoxSocket.Location = new System.Drawing.Point(426, 229);
            this.pictureBoxSocket.Name = "pictureBoxSocket";
            this.pictureBoxSocket.Size = new System.Drawing.Size(844, 283);
            this.pictureBoxSocket.TabIndex = 30;
            this.pictureBoxSocket.TabStop = false;
            // 
            // lblDateTime
            // 
            this.lblDateTime.AutoSize = true;
            this.lblDateTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDateTime.Location = new System.Drawing.Point(1046, 35);
            this.lblDateTime.Name = "lblDateTime";
            this.lblDateTime.Size = new System.Drawing.Size(53, 20);
            this.lblDateTime.TabIndex = 31;
            this.lblDateTime.Text = "Timer";
            // 
            // combBoxSocketPN
            // 
            this.combBoxSocketPN.FormattingEnabled = true;
            this.combBoxSocketPN.Location = new System.Drawing.Point(112, 166);
            this.combBoxSocketPN.Name = "combBoxSocketPN";
            this.combBoxSocketPN.Size = new System.Drawing.Size(262, 28);
            this.combBoxSocketPN.TabIndex = 32;
            // 
            // cmbStatus1
            // 
            this.cmbStatus1.FormattingEnabled = true;
            this.cmbStatus1.Location = new System.Drawing.Point(426, 156);
            this.cmbStatus1.Name = "cmbStatus1";
            this.cmbStatus1.Size = new System.Drawing.Size(100, 28);
            this.cmbStatus1.TabIndex = 33;
            // 
            // txtCavRun2
            // 
            this.txtCavRun2.Location = new System.Drawing.Point(532, 100);
            this.txtCavRun2.Multiline = true;
            this.txtCavRun2.Name = "txtCavRun2";
            this.txtCavRun2.Size = new System.Drawing.Size(100, 35);
            this.txtCavRun2.TabIndex = 13;
            // 
            // txtCavRun7
            // 
            this.txtCavRun7.Location = new System.Drawing.Point(1063, 100);
            this.txtCavRun7.Multiline = true;
            this.txtCavRun7.Name = "txtCavRun7";
            this.txtCavRun7.Size = new System.Drawing.Size(100, 35);
            this.txtCavRun7.TabIndex = 18;
            // 
            // txtCavRun6
            // 
            this.txtCavRun6.Location = new System.Drawing.Point(957, 100);
            this.txtCavRun6.Multiline = true;
            this.txtCavRun6.Name = "txtCavRun6";
            this.txtCavRun6.Size = new System.Drawing.Size(100, 35);
            this.txtCavRun6.TabIndex = 17;
            // 
            // txtCavRun5
            // 
            this.txtCavRun5.Location = new System.Drawing.Point(851, 100);
            this.txtCavRun5.Multiline = true;
            this.txtCavRun5.Name = "txtCavRun5";
            this.txtCavRun5.Size = new System.Drawing.Size(100, 35);
            this.txtCavRun5.TabIndex = 16;
            // 
            // txtCavRun4
            // 
            this.txtCavRun4.Location = new System.Drawing.Point(744, 100);
            this.txtCavRun4.Multiline = true;
            this.txtCavRun4.Name = "txtCavRun4";
            this.txtCavRun4.Size = new System.Drawing.Size(100, 35);
            this.txtCavRun4.TabIndex = 15;
            // 
            // txtCavRun3
            // 
            this.txtCavRun3.Location = new System.Drawing.Point(638, 100);
            this.txtCavRun3.Multiline = true;
            this.txtCavRun3.Name = "txtCavRun3";
            this.txtCavRun3.Size = new System.Drawing.Size(100, 35);
            this.txtCavRun3.TabIndex = 14;
            // 
            // txtCavRun8
            // 
            this.txtCavRun8.Location = new System.Drawing.Point(1169, 100);
            this.txtCavRun8.Multiline = true;
            this.txtCavRun8.Name = "txtCavRun8";
            this.txtCavRun8.Size = new System.Drawing.Size(100, 35);
            this.txtCavRun8.TabIndex = 19;
            // 
            // cmbStatus2
            // 
            this.cmbStatus2.FormattingEnabled = true;
            this.cmbStatus2.Location = new System.Drawing.Point(532, 156);
            this.cmbStatus2.Name = "cmbStatus2";
            this.cmbStatus2.Size = new System.Drawing.Size(100, 28);
            this.cmbStatus2.TabIndex = 34;
            // 
            // cmbStatus4
            // 
            this.cmbStatus4.FormattingEnabled = true;
            this.cmbStatus4.Location = new System.Drawing.Point(744, 156);
            this.cmbStatus4.Name = "cmbStatus4";
            this.cmbStatus4.Size = new System.Drawing.Size(100, 28);
            this.cmbStatus4.TabIndex = 36;
            // 
            // cmbStatus3
            // 
            this.cmbStatus3.FormattingEnabled = true;
            this.cmbStatus3.Location = new System.Drawing.Point(638, 156);
            this.cmbStatus3.Name = "cmbStatus3";
            this.cmbStatus3.Size = new System.Drawing.Size(100, 28);
            this.cmbStatus3.TabIndex = 35;
            // 
            // cmbStatus8
            // 
            this.cmbStatus8.FormattingEnabled = true;
            this.cmbStatus8.Location = new System.Drawing.Point(1169, 156);
            this.cmbStatus8.Name = "cmbStatus8";
            this.cmbStatus8.Size = new System.Drawing.Size(100, 28);
            this.cmbStatus8.TabIndex = 40;
            // 
            // cmbStatus7
            // 
            this.cmbStatus7.FormattingEnabled = true;
            this.cmbStatus7.Location = new System.Drawing.Point(1063, 156);
            this.cmbStatus7.Name = "cmbStatus7";
            this.cmbStatus7.Size = new System.Drawing.Size(100, 28);
            this.cmbStatus7.TabIndex = 39;
            // 
            // cmbStatus6
            // 
            this.cmbStatus6.FormattingEnabled = true;
            this.cmbStatus6.Location = new System.Drawing.Point(957, 156);
            this.cmbStatus6.Name = "cmbStatus6";
            this.cmbStatus6.Size = new System.Drawing.Size(100, 28);
            this.cmbStatus6.TabIndex = 38;
            // 
            // cmbStatus5
            // 
            this.cmbStatus5.FormattingEnabled = true;
            this.cmbStatus5.Location = new System.Drawing.Point(851, 156);
            this.cmbStatus5.Name = "cmbStatus5";
            this.cmbStatus5.Size = new System.Drawing.Size(100, 28);
            this.cmbStatus5.TabIndex = 37;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(29, 360);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(366, 54);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 41;
            this.pictureBox2.TabStop = false;
            // 
            // BtnAdmin
            // 
            this.BtnAdmin.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnAdmin.Location = new System.Drawing.Point(282, 421);
            this.BtnAdmin.Name = "BtnAdmin";
            this.BtnAdmin.Size = new System.Drawing.Size(113, 42);
            this.BtnAdmin.TabIndex = 42;
            this.BtnAdmin.Text = "Admin";
            this.BtnAdmin.UseVisualStyleBackColor = true;
            this.BtnAdmin.Click += new System.EventHandler(this.BtnAdmin_Click_1);
            // 
            // BtnLogout
            // 
            this.BtnLogout.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnLogout.Location = new System.Drawing.Point(282, 462);
            this.BtnLogout.Name = "BtnLogout";
            this.BtnLogout.Size = new System.Drawing.Size(113, 42);
            this.BtnLogout.TabIndex = 43;
            this.BtnLogout.Text = "Logout";
            this.BtnLogout.UseVisualStyleBackColor = true;
            this.BtnLogout.Click += new System.EventHandler(this.BtnLogout_Click_1);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1311, 538);
            this.Controls.Add(this.BtnLogout);
            this.Controls.Add(this.BtnAdmin);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.cmbStatus8);
            this.Controls.Add(this.cmbStatus7);
            this.Controls.Add(this.cmbStatus6);
            this.Controls.Add(this.cmbStatus5);
            this.Controls.Add(this.cmbStatus4);
            this.Controls.Add(this.cmbStatus3);
            this.Controls.Add(this.cmbStatus2);
            this.Controls.Add(this.cmbStatus1);
            this.Controls.Add(this.combBoxSocketPN);
            this.Controls.Add(this.lblDateTime);
            this.Controls.Add(this.pictureBoxSocket);
            this.Controls.Add(this.btnAddStatus);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtCavRun8);
            this.Controls.Add(this.txtCavRun7);
            this.Controls.Add(this.txtCavRun6);
            this.Controls.Add(this.txtCavRun5);
            this.Controls.Add(this.txtCavRun4);
            this.Controls.Add(this.txtCavRun3);
            this.Controls.Add(this.txtCavRun2);
            this.Controls.Add(this.txtCavRun1);
            this.Controls.Add(this.btnPlus);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtTotalRun);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtCavUse);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtOprID);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sockets Data Control";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSocket)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtOprID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCavUse;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtTotalRun;
        private System.Windows.Forms.Button btnPlus;
        private System.Windows.Forms.TextBox txtCavRun1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnAddStatus;
        private System.Windows.Forms.PictureBox pictureBoxSocket;
        private System.Windows.Forms.Label lblDateTime;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ComboBox combBoxSocketPN;
        private System.Windows.Forms.ComboBox cmbStatus1;
        private System.Windows.Forms.TextBox txtCavRun2;
        private System.Windows.Forms.TextBox txtCavRun7;
        private System.Windows.Forms.TextBox txtCavRun6;
        private System.Windows.Forms.TextBox txtCavRun5;
        private System.Windows.Forms.TextBox txtCavRun4;
        private System.Windows.Forms.TextBox txtCavRun3;
        private System.Windows.Forms.TextBox txtCavRun8;
        private System.Windows.Forms.ComboBox cmbStatus2;
        private System.Windows.Forms.ComboBox cmbStatus4;
        private System.Windows.Forms.ComboBox cmbStatus3;
        private System.Windows.Forms.ComboBox cmbStatus8;
        private System.Windows.Forms.ComboBox cmbStatus7;
        private System.Windows.Forms.ComboBox cmbStatus6;
        private System.Windows.Forms.ComboBox cmbStatus5;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button BtnAdmin;
        private System.Windows.Forms.Button BtnLogout;
    }
}

