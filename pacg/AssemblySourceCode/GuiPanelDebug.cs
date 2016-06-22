using System;
using System.IO;
using UnityEngine;

public class GuiPanelDebug : GuiPanel
{
    private int bufferSize = 20;
    [Tooltip("reference to the command display text field on this panel")]
    public GuiLabel Command;
    private TouchScreenKeyboard keyboard;
    [Tooltip("reference to the output display text field on this panel")]
    public GuiLabel Output;
    [Tooltip("reference to the parent panel (the options panel)")]
    public GuiPanel Parent;
    private RingBuffer previousCommands;
    [Tooltip("reference to the prompt text field on this panel")]
    public GuiLabel Prompt;

    public override void Clear()
    {
        this.Command.Clear();
        this.Output.Clear();
    }

    private string GetDebugFilePath() => 
        GameDirectory.GetHistoryPath();

    private void HandlePreviousCommands(bool upArrow, bool downArrow)
    {
        if ((this.previousCommands == null) && (upArrow || downArrow))
        {
            this.Command.Text = string.Empty;
        }
        else if (downArrow)
        {
            this.Command.Text = this.previousCommands.IncrementAndRead();
        }
        else if (upArrow)
        {
            this.Command.Text = this.previousCommands.DecrementAndRead();
        }
    }

    private void InputLoopDesktop()
    {
        if (Input.inputString != null)
        {
            for (int i = 0; i < Input.inputString.Length; i++)
            {
                char ch = Input.inputString[i];
                switch (ch)
                {
                    case '\b':
                        if (this.Command.Text.Length > 0)
                        {
                            this.Command.Text = this.Command.Text.Substring(0, this.Command.Text.Length - 1);
                            if (this.previousCommands != null)
                            {
                                this.previousCommands.ResetPointer();
                            }
                        }
                        break;

                    case '\n':
                    case '\r':
                        this.Output.Text = DebugParser.Execute(this.Command.Text);
                        if (this.Command.Text.Length > 0)
                        {
                            if (this.previousCommands == null)
                            {
                                this.previousCommands = new RingBuffer(this.bufferSize);
                            }
                            this.previousCommands.Save(this.Command.Text);
                        }
                        this.Command.Clear();
                        break;

                    default:
                        this.Command.Text = this.Command.Text + ch;
                        if (this.previousCommands != null)
                        {
                            this.previousCommands.ResetPointer();
                        }
                        break;
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            this.Parent.Show(false);
        }
    }

    private void InputLoopMobile()
    {
        if (this.keyboard != null)
        {
            if (this.keyboard.done)
            {
                if (!this.keyboard.wasCanceled)
                {
                    if (!string.IsNullOrEmpty(this.Command.Text))
                    {
                        this.Output.Text = DebugParser.Execute(this.Command.Text);
                        if (this.Command.Text.Length > 0)
                        {
                            if (this.previousCommands == null)
                            {
                                this.previousCommands = new RingBuffer(this.bufferSize);
                            }
                            this.previousCommands.Save(this.Command.Text);
                        }
                        this.Command.Clear();
                    }
                }
                else
                {
                    this.Command.Clear();
                }
            }
            else if (this.keyboard.active)
            {
                this.Command.Text = this.keyboard.text;
            }
        }
    }

    private void LoadCommands()
    {
        this.previousCommands = new RingBuffer(this.bufferSize);
        this.previousCommands.head = -1;
        this.previousCommands.tail = -1;
        this.previousCommands.pointer = -1;
        string debugFilePath = this.GetDebugFilePath();
        if (File.Exists(debugFilePath))
        {
            using (StreamReader reader = new StreamReader(debugFilePath))
            {
                this.previousCommands.head = int.Parse(reader.ReadLine());
                this.previousCommands.tail = int.Parse(reader.ReadLine());
                for (int i = 0; i < this.previousCommands.data.Length; i++)
                {
                    this.previousCommands.data[i] = reader.ReadLine();
                }
            }
        }
    }

    private void OnCloseButtonPushed()
    {
        this.Show(false);
    }

    private void OnDestroy()
    {
        this.SaveCommands();
    }

    private void SaveCommands()
    {
        if ((this.previousCommands != null) && (this.previousCommands.data != null))
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(this.GetDebugFilePath()))
                {
                    writer.WriteLine(this.previousCommands.head);
                    writer.WriteLine(this.previousCommands.tail);
                    for (int i = 0; i < this.previousCommands.data.Length; i++)
                    {
                        writer.WriteLine(this.previousCommands.data[i]);
                    }
                }
            }
            catch (Exception exception)
            {
                if (exception.GetType() == typeof(AccessViolationException))
                {
                    Debug.Log("File Access Violation Exception for Debug txt saving!");
                }
                else
                {
                    Debug.Log(exception.StackTrace);
                }
            }
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.Clear();
        if (isVisible)
        {
            this.Prompt.Text = ">>";
            if (Device.GetIsIphone() || Device.GetIsAndroid())
            {
                TouchScreenKeyboard.hideInput = false;
                this.keyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.Default, false, false, false, false, string.Empty);
            }
        }
    }

    public void ShowPreviousCommand()
    {
        this.HandlePreviousCommands(true, false);
    }

    protected override void Start()
    {
        base.Start();
        this.LoadCommands();
    }

    private void Update()
    {
        if (Device.GetIsIphone() || Device.GetIsAndroid())
        {
            this.InputLoopMobile();
        }
        else
        {
            this.InputLoopDesktop();
        }
    }

    private class RingBuffer
    {
        public string[] data;
        public int head;
        private bool m_cycling;
        private int m_size;
        public int pointer;
        public int tail;

        public RingBuffer(int size)
        {
            this.m_size = size;
            this.data = new string[this.m_size];
            this.pointer = -1;
            this.tail = -1;
            this.head = -1;
        }

        public void Clear()
        {
            this.data = new string[this.m_size];
            this.tail = -1;
            this.head = -1;
            this.pointer = -1;
        }

        public string DecrementAndRead()
        {
            if ((this.head != -1) || (this.tail != -1))
            {
                if (this.pointer == this.tail)
                {
                    return this.data[this.pointer];
                }
                if (this.pointer == 0)
                {
                    this.pointer = this.m_size - 1;
                }
                else if (this.pointer == -1)
                {
                    this.pointer = this.head;
                }
                else
                {
                    this.pointer--;
                }
                if (((this.pointer >= 0) && (this.pointer <= (this.m_size - 1))) && ((this.data[this.pointer] != null) && (this.data[this.pointer].Length > 0)))
                {
                    return this.data[this.pointer];
                }
                this.pointer++;
            }
            return string.Empty;
        }

        public string IncrementAndRead()
        {
            if ((this.head != -1) || (this.tail != -1))
            {
                if (this.pointer == this.head)
                {
                    this.pointer = -1;
                    return string.Empty;
                }
                if (this.pointer == -1)
                {
                    return string.Empty;
                }
                if (this.pointer == (this.m_size - 1))
                {
                    this.pointer = 0;
                }
                else
                {
                    this.pointer++;
                }
                if (((this.pointer >= 0) && (this.pointer <= (this.m_size - 1))) && ((this.data[this.pointer] != null) && (this.data[this.pointer].Length > 0)))
                {
                    return this.data[this.pointer];
                }
                this.pointer--;
            }
            return string.Empty;
        }

        public void ResetPointer()
        {
            this.pointer = -1;
        }

        public void Save(string str)
        {
            if (this.head == -1)
            {
                this.head = 0;
            }
            else if (((this.head + 1) < this.m_size) && !this.m_cycling)
            {
                this.head++;
            }
            else
            {
                if ((this.head + 1) >= this.m_size)
                {
                    this.head = 0;
                }
                else
                {
                    this.head++;
                }
                this.tail++;
                if (this.tail == this.m_size)
                {
                    this.tail = 0;
                }
                this.m_cycling = true;
            }
            if (this.tail == -1)
            {
                this.tail = 0;
            }
            this.pointer = -1;
            this.data[this.head] = str;
        }
    }
}

