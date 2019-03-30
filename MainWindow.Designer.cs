namespace lab3GUI
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
            this.MessagesBox = new System.Windows.Forms.TextBox();
            this.InputBox = new System.Windows.Forms.TextBox();
            this.SendButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // MessagesBox
            // 
            this.MessagesBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.MessagesBox.Location = new System.Drawing.Point(0, 0);
            this.MessagesBox.MinimumSize = new System.Drawing.Size(10, 10);
            this.MessagesBox.Multiline = true;
            this.MessagesBox.Name = "MessagesBox";
            this.MessagesBox.ReadOnly = true;
            this.MessagesBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.MessagesBox.Size = new System.Drawing.Size(505, 365);
            this.MessagesBox.TabIndex = 0;
            this.MessagesBox.TabStop = false;
            // 
            // InputBox
            // 
            this.InputBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.InputBox.Location = new System.Drawing.Point(0, 365);
            this.InputBox.MaxLength = 1024;
            this.InputBox.MinimumSize = new System.Drawing.Size(10, 10);
            this.InputBox.Multiline = true;
            this.InputBox.Name = "InputBox";
            this.InputBox.Size = new System.Drawing.Size(505, 91);
            this.InputBox.TabIndex = 1;
            // 
            // SendButton
            // 
            this.SendButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.SendButton.Enabled = false;
            this.SendButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SendButton.Location = new System.Drawing.Point(0, 455);
            this.SendButton.MinimumSize = new System.Drawing.Size(10, 10);
            this.SendButton.Name = "SendButton";
            this.SendButton.Size = new System.Drawing.Size(505, 41);
            this.SendButton.TabIndex = 2;
            this.SendButton.Text = "SEND";
            this.SendButton.UseVisualStyleBackColor = true;
            this.SendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 496);
            this.Controls.Add(this.SendButton);
            this.Controls.Add(this.InputBox);
            this.Controls.Add(this.MessagesBox);
            this.Name = "MainForm";
            this.Text = "Chat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox MessagesBox;
        private System.Windows.Forms.TextBox InputBox;
        private System.Windows.Forms.Button SendButton;
    }
}

