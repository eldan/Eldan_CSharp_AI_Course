using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindFormEldan_01
{
    public partial class Form1 : Form
    {
        private List<ChatMessage> chatHistory = new List<ChatMessage>();
        private OpenAI_SDK openAiSdk = new OpenAI_SDK("gpt-3.5-turbo");

        public Form1()
        {
            InitializeComponent();
            comboBoxModel.DrawMode = DrawMode.OwnerDrawFixed;
            comboBoxModel.DrawItem += ComboBoxModel_DrawItem;
            listBoxChat.DrawMode = DrawMode.OwnerDrawFixed;
            listBoxChat.DrawItem += listBoxChat_DrawItem;
        }

        private void ComboBoxModel_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            var cb = sender as ComboBox;
            string text = cb.Items[e.Index].ToString();
            Color backColor = SystemColors.Window;
            Color foreColor = SystemColors.WindowText;
            if (text == "OpenAI")
            {
                backColor = Color.FromArgb(255, 230, 230); // light red
            }
            else if (text == "Gemini")
            {
                backColor = Color.FromArgb(230, 240, 255); // light blue
            }
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                backColor = ControlPaint.LightLight(backColor);
            }
            using (SolidBrush b = new SolidBrush(backColor))
                e.Graphics.FillRectangle(b, e.Bounds);
            using (SolidBrush b = new SolidBrush(foreColor))
                e.Graphics.DrawString(text, e.Font, b, e.Bounds);
            e.DrawFocusRectangle();
        }

        private void listBoxChat_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            var msg = listBoxChat.Items[e.Index] as ChatMessage;
            Color backColor = SystemColors.Window;
            Color foreColor = SystemColors.WindowText;
            if (msg != null && msg.IsAI)
            {
                backColor = msg.AIBackColor;
            }
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                backColor = ControlPaint.LightLight(backColor);
            }
            using (SolidBrush b = new SolidBrush(backColor))
                e.Graphics.FillRectangle(b, e.Bounds);
            using (SolidBrush b = new SolidBrush(foreColor))
                e.Graphics.DrawString(msg?.ToString() ?? listBoxChat.Items[e.Index].ToString(), e.Font, b, e.Bounds);
            e.DrawFocusRectangle();
        }

        private void RefreshChat()
        {
            listBoxChat.Items.Clear();
            foreach (var msg in chatHistory)
            {
                listBoxChat.Items.Add(msg);
            }
        }

        private async void buttonSend_Click(object sender, EventArgs e)
        {
            string userMessage = textBoxInput.Text.Trim();
            if (string.IsNullOrEmpty(userMessage))
            {
                return;
            }

            chatHistory.Add(new ChatMessage { Sender = "You", Text = userMessage, IsAI = false });
            RefreshChat();
            textBoxInput.Clear();

            string aiResponse;
            string aiPrefix;
            bool isGemini = comboBoxModel.SelectedItem?.ToString() == "Gemini";
            Color aiBackColor;
            try
            {
                if (isGemini)
                {
                    aiResponse = await Gemini_SDK.Call(userMessage);
                    aiPrefix = "Gemeni: ";
                    aiBackColor = Color.FromArgb(230, 240, 255); // light blue
                }
                else
                {
                    aiResponse = await openAiSdk.Call(userMessage);
                    aiPrefix = "ChatGpt: ";
                    aiBackColor = Color.FromArgb(255, 230, 230); // light red
                }
            }
            catch (Exception ex)
            {
                aiResponse = "Error: " + ex.Message;
                aiPrefix = isGemini ? "Gemeni: " : "ChatGpt: ";
                aiBackColor = isGemini ? Color.FromArgb(230, 240, 255) : Color.FromArgb(255, 230, 230);
            }

            chatHistory.Add(new ChatMessage { Sender = aiPrefix.Trim(), Text = aiResponse, IsAI = true, AIBackColor = aiBackColor });
            RefreshChat();
        }
    }

    public class ChatMessage
    {
        public string Sender { get; set; }
        public string Text { get; set; }
        public bool IsAI { get; set; }
        public Color AIBackColor { get; set; } = Color.White;
        public override string ToString() => $"{Sender} {Text}";
    }
}
