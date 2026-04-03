using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Eldan_Exercise_02.AppEnums;

namespace Eldan_Exercise_02
{
  public partial class Form1 : Form
  {
    private List<ChatMessage> chatHistory = new List<ChatMessage>();

    public Form1()
    {
      InitializeComponent();
      comboBoxCompany.DrawMode = DrawMode.Normal;
      listBoxChat.DrawMode = DrawMode.OwnerDrawFixed;
      listBoxChat.DrawItem += listBoxChat_DrawItem;
      comboBoxCompany.SelectedIndexChanged += ComboBoxModel_SelectedIndexChanged;

      // Populate ComboBox with enum values
      comboBoxCompany.Items.Clear();
      comboBoxCompany.Items.AddRange(new object[] { "OpenAI", "Gemini" });
      comboBoxCompany.SelectedIndex = 0;
      comboBoxOpenAIModel.SelectedIndex = 0;
      comboBoxGeminiModel.SelectedIndex = 0;

      ComboBoxModel_SelectedIndexChanged(comboBoxCompany, EventArgs.Empty);

      comboBoxOpenAIModel.DrawMode = DrawMode.Normal;

      comboBoxGeminiModel.DrawMode = DrawMode.Normal;
    }

    private void ComboBoxModel_SelectedIndexChanged(object sender, EventArgs e)
    {
      var selected = comboBoxCompany.SelectedItem as string;
      bool isOpenAI = selected == "OpenAI";
      bool isGemini = selected == "Gemini";
      comboBoxOpenAIModel.Visible = isOpenAI;
      comboBoxGeminiModel.Visible = isGemini;

      // Set the background color based on the selected company
      Color backColor = isOpenAI ? Color.FromArgb(255, 230, 230) : Color.FromArgb(230, 240, 255);
      comboBoxCompany.BackColor = backColor;
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

    // Modify the buttonSend_Click method to send the entire chat history to the AI server
    private async void buttonSend_Click(object sender, EventArgs e)
    {
      string userMessage = textBoxInput.Text.Trim();
      if (string.IsNullOrEmpty(userMessage))
      {
        return;
      }

      chatHistory.Add(new ChatMessage { Sender = "You: ", Text = userMessage, IsAI = false });
      RefreshChat();
      textBoxInput.Clear();

      string aiResponse;
      string aiPrefix;
      var selected = comboBoxCompany.SelectedItem;
      bool isGemini = false;
      if (selected.ToString() == CompanyType.Gemini.ToString())
      {
        isGemini = true;
      }

      Color aiBackColor;
      try
      {
        // Combine the entire chat history into a single string
        string fullConversation = string.Join("\n", chatHistory.ConvertAll(msg => $"{msg.Sender} {msg.Text}"));

        if (isGemini)
        {
          // Retrieve the selected Gemini model
          GeminiModels selectedGeminiEnum = (GeminiModels)Enum.Parse(typeof(GeminiModels), comboBoxGeminiModel.SelectedItem.ToString());
          string selectedGeminiModel = EnumDisplayNameHelper.GetEnumDisplayName(selectedGeminiEnum);
          aiResponse = await Gemini_SDK.Call(fullConversation, selectedGeminiModel);
          aiPrefix = "Gemeni: ";
          aiBackColor = Color.FromArgb(230, 240, 255); // light blue
        }
        else
        {
          // Ensure the SelectedItem is not null before parsing
          if (comboBoxOpenAIModel.SelectedItem != null)
          {
            // Retrieve the selected OpenAI model
            OpenAIModels selectedOpenAIEnum = (OpenAIModels)Enum.Parse(typeof(OpenAIModels), comboBoxOpenAIModel.SelectedItem.ToString());
            string selectedModel = EnumDisplayNameHelper.GetEnumDisplayName(selectedOpenAIEnum);
            var openAiSdk = new OpenAI_SDK(selectedModel);
            aiResponse = await openAiSdk.Call(fullConversation);
            aiPrefix = "ChatGpt: ";
            aiBackColor = Color.FromArgb(255, 230, 230); // light red
          }
          else
          {
            // Handle the case where no item is selected
            MessageBox.Show("Please select an OpenAI model.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return; // Exit the method if no model is selected
          }
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

    private void comboBoxOpenAIModel_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    // Add the Click event handler for the Clear button
    private void buttonClear_Click(object sender, EventArgs e)
    {
        chatHistory.Clear();
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
