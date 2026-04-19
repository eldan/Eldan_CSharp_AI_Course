using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Eldan_Exercise_03.AppEnums;

namespace Eldan_Exercise_03
{
  public partial class Form1 : Form
  {
    private List<ChatMessage> chatHistory = new List<ChatMessage>();
    private DateTime lastUIUpdate = DateTime.MinValue;
    private const int UI_UPDATE_THROTTLE_MS = 50;

    public Form1()
    {
      InitializeComponent();
      comboBoxCompany.DrawMode = DrawMode.Normal;
      listBoxChat.DrawMode = DrawMode.OwnerDrawVariable;
      listBoxChat.DrawItem += listBoxChat_DrawItem;
      listBoxChat.MeasureItem += listBoxChat_MeasureItem;
      comboBoxCompany.SelectedIndexChanged += ComboBoxModel_SelectedIndexChanged;

      // Populate ComboBox with enum values
      comboBoxCompany.Items.Clear();
      comboBoxCompany.Items.AddRange(new object[] { "OpenAI", "Gemini" });
      comboBoxCompany.SelectedIndex = 0;
    }

    private void ComboBoxModel_SelectedIndexChanged(object sender, EventArgs e)
    {
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
      
      string text = msg?.ToString() ?? listBoxChat.Items[e.Index].ToString();
      using (SolidBrush b = new SolidBrush(foreColor))
        e.Graphics.DrawString(text, e.Font, b, e.Bounds, StringFormat.GenericDefault);
      
      e.DrawFocusRectangle();
    }

    private void listBoxChat_MeasureItem(object sender, MeasureItemEventArgs e)
    {
      if (e.Index < 0) return;
      var msg = listBoxChat.Items[e.Index] as ChatMessage;
      string text = msg?.ToString() ?? listBoxChat.Items[e.Index].ToString();

      SizeF textSize = e.Graphics.MeasureString(text, listBoxChat.Font, listBoxChat.Width - 10);
      e.ItemHeight = (int)Math.Ceiling(textSize.Height) + 4;
    }

    private void RefreshChatAll()
    {
      listBoxChat.Items.Clear();
      foreach (var msg in chatHistory)
      {
        listBoxChat.Items.Add(msg);
      }
      listBoxChat.TopIndex = Math.Max(0, listBoxChat.Items.Count - 1);
    }

    private void RefreshChat()
    {
      listBoxChat.Items.Add(chatHistory[chatHistory.Count - 1]);
      listBoxChat.TopIndex = Math.Max(0, listBoxChat.Items.Count - 1);
    }

    private void UpdateLastMessageThrottled()
    {
      if (DateTime.Now - lastUIUpdate < TimeSpan.FromMilliseconds(UI_UPDATE_THROTTLE_MS))
      {
        return;
      }

      lastUIUpdate = DateTime.Now;
      if (listBoxChat.Items.Count > 0)
      {
        int lastIndex = listBoxChat.Items.Count - 1;
        // Clear and re-add to force height recalculation
        var lastItem = listBoxChat.Items[lastIndex];
        listBoxChat.Items.RemoveAt(lastIndex);
        listBoxChat.Items.Insert(lastIndex, lastItem);
        listBoxChat.TopIndex = Math.Max(0, listBoxChat.Items.Count - 1);
      }
    }

    private async void buttonSend_Click(object sender, EventArgs e)
    {
      string userMessage = textBoxInput.Text.Trim();
      if (string.IsNullOrEmpty(userMessage))
      {
        return;
      }

      chatHistory.Add(new ChatMessage { Sender = "You: ", Text = userMessage, IsAI = false });
      RefreshChatAll();
      textBoxInput.Clear();
      buttonSend.Enabled = false;

      try
      {
        await HandleStreamingResponse();
      }
      catch (Exception ex)
      {
        var selected = comboBoxCompany.SelectedItem;
        bool isGemini = selected.ToString() == CompanyType.Gemini.ToString();
        string aiPrefix = isGemini ? "Gemeni: " : "ChatGpt: ";
        Color aiBackColor = isGemini ? Color.FromArgb(230, 240, 255) : Color.FromArgb(255, 230, 230);
        string errorMessage = "Error: " + ex.Message;
        chatHistory.Add(new ChatMessage { Sender = aiPrefix.Trim(), Text = errorMessage, IsAI = true, AIBackColor = aiBackColor });
        RefreshChat();
      }
      finally
      {
        buttonSend.Enabled = true;
      }
    }

    private async Task HandleStreamingResponse()
    {
      var selected = comboBoxCompany.SelectedItem;
      bool isGemini = selected.ToString() == CompanyType.Gemini.ToString();
      string aiPrefix = isGemini ? "Gemeni: " : "ChatGpt: ";
      Color aiBackColor = isGemini ? Color.FromArgb(230, 240, 255) : Color.FromArgb(255, 230, 230);

      ChatMessage aiMessage = new ChatMessage { Sender = aiPrefix.Trim(), Text = "", IsAI = true, AIBackColor = aiBackColor };
      chatHistory.Add(aiMessage);
      RefreshChatAll();

      string fullConversation = string.Join("\n", chatHistory.ConvertAll(msg => $"{msg.Sender} {msg.Text}"));

      if (isGemini)
      {
        var geminiSdk = new Gemini_SDK("gemini-2.5-flash", "Respond normally");

        await foreach (var chunk in geminiSdk.CallStream(fullConversation))
        {
          aiMessage.Text += chunk;
          UpdateLastMessageThrottled();
        }
      }
      else
      {
        var openAiSdk = new OpenAI_SDK_Response("gpt-5.2", "Respond normally");

        await foreach (var chunk in openAiSdk.CallStream(fullConversation))
        {
          aiMessage.Text += chunk;
          UpdateLastMessageThrottled();
        }
      }

      UpdateLastMessageThrottled();
    }

    private void comboBoxOpenAIModel_SelectedIndexChanged(object sender, EventArgs e)
    {
    }

    private void buttonClear_Click(object sender, EventArgs e)
    {
      chatHistory.Clear();
      RefreshChatAll();
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
