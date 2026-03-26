namespace WindFormEldan_01
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox textBoxInput;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.ListBox listBoxChat;
        private System.Windows.Forms.ComboBox comboBoxModel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.textBoxInput = new System.Windows.Forms.TextBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.listBoxChat = new System.Windows.Forms.ListBox();
            this.comboBoxModel = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // textBoxInput
            // 
            this.textBoxInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxInput.Location = new System.Drawing.Point(12, 410);
            this.textBoxInput.Name = "textBoxInput";
            this.textBoxInput.Size = new System.Drawing.Size(650, 23);
            this.textBoxInput.TabIndex = 0;
            // 
            // buttonSend
            // 
            this.buttonSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSend.Location = new System.Drawing.Point(680, 410);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(100, 23);
            this.buttonSend.TabIndex = 1;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // listBoxChat
            // 
            this.listBoxChat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxChat.FormattingEnabled = true;
            this.listBoxChat.ItemHeight = 15;
            this.listBoxChat.Location = new System.Drawing.Point(12, 12);
            this.listBoxChat.Name = "listBoxChat";
            this.listBoxChat.Size = new System.Drawing.Size(768, 362);
            this.listBoxChat.TabIndex = 2;
            this.listBoxChat.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxChat.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxChat_DrawItem);
            // 
            // comboBoxModel
            // 
            this.comboBoxModel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBoxModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxModel.Items.AddRange(new object[] { "OpenAI", "Gemini" });
            this.comboBoxModel.Location = new System.Drawing.Point(12, 380);
            this.comboBoxModel.Name = "comboBoxModel";
            this.comboBoxModel.Size = new System.Drawing.Size(121, 23);
            this.comboBoxModel.TabIndex = 3;
            this.comboBoxModel.SelectedIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.comboBoxModel);
            this.Controls.Add(this.listBoxChat);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.textBoxInput);
            this.Name = "Form1";
            this.Text = "AI Chat";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
