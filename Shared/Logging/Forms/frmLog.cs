﻿/// frmLog is a part of https://github.com/murrty/aphrodite booru downloader.
/// Licensed via GPL-3.0, if you did not receieve a license with this file; idk figure it out.
/// This code, *as-is*, should not be a part of another project; it should really only be used as reference or testing.
namespace murrty.logging;

using aphrodite;
using System.Windows.Forms;

internal partial class frmLog : Form {
    public bool IsShown {
        get; set;
    } = false;

    public frmLog() {
        InitializeComponent();

        if (!Program.DebugMode) {
            btnTestLine.Enabled = btnTestLine.Visible = false;
        }

        // The icon for the exception form.
        this.Icon = global::aphrodite.Properties.Resources.ProgramIcon;

        rtbLog.AutoScroll = controls.AutoScroll.ScrolledToBottom;
        rtbLog.LineCountChanged += (s, e) => lbLines.Text = $"Log count: {rtbLog.TotalLines}";
        rtbLog.ContextMenu = cmLog;
    }
    private void frmLog_Load(object sender, EventArgs e) {
        if (Config.ValidPoint(Config.Settings.FormSettings.frmLog_Location)) {
            this.StartPosition = FormStartPosition.Manual;
            this.Location = Config.Settings.FormSettings.frmLog_Location;
        }

        if (Config.ValidSize(Config.Settings.FormSettings.frmLog_Size)) {
            this.Size = Config.Settings.FormSettings.frmLog_Size;
        }
    }
    private void frmLog_FormClosing(object sender, FormClosingEventArgs e) {
        e.Cancel = true;
        this.Hide();
        IsShown = false;
    }

    private void mCopyText_Click(object sender, EventArgs e) {
        if (tcMain.SelectedTab == tpMainLog) {
            if (rtbLog.SelectionLength > 0) {
                rtbLog.Copy();
            }
        }
        else if (tcMain.SelectedTab == tpExceptions) {
            if (tcExceptions.SelectedTab != null) {
                Control[] ex = tcExceptions.SelectedTab.Controls.Find("TextBox", false);
                if (ex.Length > 0 && ex[0] is TextBox txt) {
                    if (txt.SelectionLength > 0) {
                        txt.Copy();
                    }
                }
            }
        }
    }
    private void btnClear_Click(object sender, EventArgs e) {
        rtbLog.Clear();
        Append("Log has been cleared");
    }
    private void btnRemoveException_Click(object sender, EventArgs e) {
        if (tcExceptions.SelectedTab != null) {
            Control[] ex = tcExceptions.SelectedTab.Controls.Find("TextBox", false);
            if (ex.Length > 0 && ex[0] is TextBox txt)
                txt.Dispose();

            int Index = tcExceptions.SelectedIndex;
            if (tcExceptions.TabCount > 1)
                tcExceptions.SelectTab(
                    tcExceptions.TabCount > 0 ? Index + 1 < tcExceptions.TabCount ?
                    Index + 1 : Index - 1 : 0);

            tcExceptions.TabPages[Index].Dispose();
            btnRemoveException.Enabled = tcExceptions.TabCount > 0;
        }
    }
    private void btnClose_Click(object sender, EventArgs e) {
        this.Hide();
        IsShown = false;
    }

    /// <summary>
    /// Appends text to the log.
    /// </summary>
    /// <param name="message">The message to append.</param>
    [System.Diagnostics.DebuggerStepThrough]
    public void Append(string message) =>
        rtbLog.InvokeIfRequired(() => rtbLog.AppendText($"[{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}] {message}", true));

    /// <summary>
    /// Appends text to the log, not including date/time of the message.
    /// </summary>
    /// <param name="message">The message to append.</param>
    [System.Diagnostics.DebuggerStepThrough]
    public void AppendNoDate(string message) =>
        rtbLog.InvokeIfRequired(() => rtbLog.AppendText(message, true));

    /// <summary>
    /// Adds a new exception to the log.
    /// </summary>
    /// <param name="type">The exception type</param>
    /// <param name="exception">The inner exception details.</param>
    [System.Diagnostics.DebuggerStepThrough]
    public void AddException(string type, string exception, DateTime occurance) {
        TabPage ExceptionPage = new($"{type} @ {occurance:yyyy/MM/dd HH:mm:ss.fff}");
        TextBox ExceptionDetails = new() {
            Multiline = true,
            ReadOnly = true,
            Name = "TextBox"
        };
        ExceptionPage.Controls.Add(ExceptionDetails);
        ExceptionDetails.Dock = DockStyle.Fill;
        ExceptionDetails.Text = exception;
        ExceptionDetails.ContextMenu = cmLog;
        ExceptionDetails.Font = rtbLog.Font;
        tcExceptions.TabPages.Add(ExceptionPage);
        tcExceptions.SelectedTab = ExceptionPage;
        btnRemoveException.Enabled = tcExceptions.TabCount > 0;
    }

    private void btnTestLine_Click(object sender, EventArgs e) {
        rtbLog.AppendLine("Hello!");
    }
}