using Eldan_Exercise_02.AppEnums;

namespace Eldan_Exercise_02
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox textBoxInput;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.ListBox listBoxChat;
        private System.Windows.Forms.ComboBox comboBoxCompany;
        private System.Windows.Forms.ComboBox comboBoxOpenAIModel;
        private System.Windows.Forms.ComboBox comboBoxGeminiModel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelCombos;

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
            textBoxInput = new TextBox();
            buttonSend = new Button();
            listBoxChat = new ListBox();
            comboBoxCompany = new ComboBox();
            comboBoxOpenAIModel = new ComboBox();
            comboBoxGeminiModel = new ComboBox();
            flowLayoutPanelCombos = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // textBoxInput
            // 
            textBoxInput.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxInput.Location = new Point(12, 410);
            textBoxInput.Name = "textBoxInput";
            textBoxInput.Size = new Size(650, 23);
            textBoxInput.TabIndex = 0;
            // 
            // buttonSend
            // 
            buttonSend.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonSend.Location = new Point(680, 410);
            buttonSend.Name = "buttonSend";
            buttonSend.Size = new Size(100, 23);
            buttonSend.TabIndex = 1;
            buttonSend.Text = "Send";
            buttonSend.UseVisualStyleBackColor = true;
            buttonSend.Click += buttonSend_Click;
            // 
            // listBoxChat
            // 
            listBoxChat.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBoxChat.DrawMode = DrawMode.OwnerDrawFixed;
            listBoxChat.FormattingEnabled = true;
            listBoxChat.ItemHeight = 15;
            listBoxChat.Location = new Point(12, 12);
            listBoxChat.Name = "listBoxChat";
            listBoxChat.Size = new Size(768, 349);
            listBoxChat.TabIndex = 2;
            listBoxChat.DrawItem += listBoxChat_DrawItem;
            // 
            // comboBoxCompany
            // 
            comboBoxCompany.DropDownStyle = ComboBoxStyle.DropDownList;
            // Ensure items are added for default selection
            comboBoxCompany.Items.Clear();
            comboBoxCompany.Items.AddRange(new object[] { "OpenAI", "Gemini" });
            comboBoxCompany.Location = new Point(3, 3);
            comboBoxCompany.Name = "comboBoxCompany";
            comboBoxCompany.Size = new Size(121, 23);
            comboBoxCompany.TabIndex = 3;
            comboBoxCompany.SelectedIndexChanged += ComboBoxModel_SelectedIndexChanged;
            // 
            // comboBoxOpenAIModel
            // 
            comboBoxOpenAIModel.DropDownStyle = ComboBoxStyle.DropDownList;
            // Update ComboBox initialization to display user-friendly names
            comboBoxOpenAIModel.Items.Clear();
            foreach (var model in Enum.GetValues(typeof(OpenAIModels)))
            {
                comboBoxOpenAIModel.Items.Add(model.ToString().Replace("_", " "));
            }
            comboBoxOpenAIModel.Location = new Point(130, 3);
            comboBoxOpenAIModel.Name = "comboBoxOpenAIModel";
            comboBoxOpenAIModel.Size = new Size(150, 23);
            comboBoxOpenAIModel.TabIndex = 4;
            // Gemini model ComboBox
            comboBoxGeminiModel.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxGeminiModel.Items.Clear();
            foreach (var model in Enum.GetValues(typeof(GeminiModels)))
            {
                comboBoxGeminiModel.Items.Add(model.ToString().Replace("_", " "));
            }
            comboBoxGeminiModel.Location = new Point(290, 3); // Ensure not overlapped
            comboBoxGeminiModel.Name = "comboBoxGeminiModel";
            comboBoxGeminiModel.Size = new Size(170, 23);
            comboBoxGeminiModel.TabIndex = 5;
            comboBoxGeminiModel.Visible = false;
            // 
            // flowLayoutPanelCombos
            // 
            flowLayoutPanelCombos.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            flowLayoutPanelCombos.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanelCombos.WrapContents = false;
            flowLayoutPanelCombos.Location = new Point(23, 370);
            flowLayoutPanelCombos.Name = "flowLayoutPanelCombos";
            flowLayoutPanelCombos.Size = new Size(500, 35);
            flowLayoutPanelCombos.TabIndex = 100;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(flowLayoutPanelCombos);
            Controls.Add(listBoxChat);
            Controls.Add(buttonSend);
            Controls.Add(textBoxInput);
            Name = "Form1";
            Text = "AI Chat";
            ResumeLayout(false);
            PerformLayout();

            // Add ComboBoxes to the FlowLayoutPanel
            flowLayoutPanelCombos.Controls.Add(comboBoxCompany);
            flowLayoutPanelCombos.Controls.Add(comboBoxOpenAIModel);
            flowLayoutPanelCombos.Controls.Add(comboBoxGeminiModel);

            // Re-add the Clear button to the FlowLayoutPanel
            Button buttonClear = new Button();
            buttonClear.Text = "Clear";
            buttonClear.Size = new Size(100, 23);
            buttonClear.TabIndex = 6;
            buttonClear.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            flowLayoutPanelCombos.Controls.Add(buttonClear);
            buttonClear.Click += buttonClear_Click;

            // Add a CheckBox for "Stream Data" before the Clear button
            CheckBox checkBoxStreamData = new CheckBox();
            checkBoxStreamData.Text = "Stream Data";
            checkBoxStreamData.AutoSize = true;
            checkBoxStreamData.TabIndex = 5;
            flowLayoutPanelCombos.Controls.Add(checkBoxStreamData);
            flowLayoutPanelCombos.Controls.SetChildIndex(checkBoxStreamData, flowLayoutPanelCombos.Controls.Count - 2);
        }
    }
}
