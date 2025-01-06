namespace WinFrm_Verification
{
    partial class frmCredentialsChk
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblCompanyCode = new Label();
            lblEmployeeCode = new Label();
            lblLicenceKey = new Label();
            txtCompanyCode = new TextBox();
            txtEmployeeCode = new TextBox();
            txtLicenceKey = new TextBox();
            btnSubmit = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            lblMessage = new Label();
            SuspendLayout();
            // 
            // lblCompanyCode
            // 
            lblCompanyCode.AutoSize = true;
            lblCompanyCode.Location = new Point(32, 38);
            lblCompanyCode.Name = "lblCompanyCode";
            lblCompanyCode.Size = new Size(90, 15);
            lblCompanyCode.TabIndex = 0;
            lblCompanyCode.Text = "Company Code";
            // 
            // lblEmployeeCode
            // 
            lblEmployeeCode.AutoSize = true;
            lblEmployeeCode.Location = new Point(32, 69);
            lblEmployeeCode.Name = "lblEmployeeCode";
            lblEmployeeCode.Size = new Size(90, 15);
            lblEmployeeCode.TabIndex = 1;
            lblEmployeeCode.Text = "Employee Code";
            // 
            // lblLicenceKey
            // 
            lblLicenceKey.AutoSize = true;
            lblLicenceKey.Location = new Point(32, 99);
            lblLicenceKey.Name = "lblLicenceKey";
            lblLicenceKey.Size = new Size(69, 15);
            lblLicenceKey.TabIndex = 2;
            lblLicenceKey.Text = "Licence Key";
            // 
            // txtCompanyCode
            // 
            txtCompanyCode.Location = new Point(142, 35);
            txtCompanyCode.Name = "txtCompanyCode";
            txtCompanyCode.PlaceholderText = "Enter company code";
            txtCompanyCode.Size = new Size(306, 23);
            txtCompanyCode.TabIndex = 3;
            // 
            // txtEmployeeCode
            // 
            txtEmployeeCode.Location = new Point(142, 66);
            txtEmployeeCode.Name = "txtEmployeeCode";
            txtEmployeeCode.PlaceholderText = "Enter employee code";
            txtEmployeeCode.Size = new Size(306, 23);
            txtEmployeeCode.TabIndex = 4;
            // 
            // txtLicenceKey
            // 
            txtLicenceKey.Location = new Point(142, 95);
            txtLicenceKey.Name = "txtLicenceKey";
            txtLicenceKey.PlaceholderText = "Enter license key";
            txtLicenceKey.Size = new Size(306, 23);
            txtLicenceKey.TabIndex = 5;
            // 
            // btnSubmit
            // 
            btnSubmit.Location = new Point(373, 134);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new Size(75, 23);
            btnSubmit.TabIndex = 6;
            btnSubmit.Text = "&Submit";
            btnSubmit.UseVisualStyleBackColor = true;
            btnSubmit.Click += btnSubmit_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label1.ForeColor = Color.Red;
            label1.Location = new Point(23, 39);
            label1.Name = "label1";
            label1.Size = new Size(12, 15);
            label1.TabIndex = 7;
            label1.Text = "*";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label2.ForeColor = Color.Red;
            label2.Location = new Point(23, 70);
            label2.Name = "label2";
            label2.Size = new Size(12, 15);
            label2.TabIndex = 8;
            label2.Text = "*";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label3.ForeColor = Color.Red;
            label3.Location = new Point(23, 100);
            label3.Name = "label3";
            label3.Size = new Size(12, 15);
            label3.TabIndex = 9;
            label3.Text = "*";
            // 
            // lblMessage
            // 
            lblMessage.AutoSize = true;
            lblMessage.ForeColor = Color.Red;
            lblMessage.Location = new Point(28, 156);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0, 15);
            lblMessage.TabIndex = 10;
            lblMessage.Visible = false;
            // 
            // frmCredentialsChk
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(485, 185);
            Controls.Add(lblMessage);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnSubmit);
            Controls.Add(txtLicenceKey);
            Controls.Add(txtEmployeeCode);
            Controls.Add(txtCompanyCode);
            Controls.Add(lblLicenceKey);
            Controls.Add(lblEmployeeCode);
            Controls.Add(lblCompanyCode);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "frmCredentialsChk";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "::: Verify Credentials :::";
            FormClosing += frmCredentialsChk_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblCompanyCode;
        private Label lblEmployeeCode;
        private Label lblLicenceKey;
        private TextBox txtCompanyCode;
        private TextBox txtEmployeeCode;
        private TextBox txtLicenceKey;
        private Button btnSubmit;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label lblMessage;
    }
}
