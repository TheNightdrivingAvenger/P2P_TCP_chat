namespace lab3GUI
{
    partial class UsernameDia
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
            this.UsernameLabel = new System.Windows.Forms.Label();
            this.UsernameInputField = new System.Windows.Forms.TextBox();
            this.SubmitUsernameButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // UsernameLabel
            // 
            this.UsernameLabel.AutoSize = true;
            this.UsernameLabel.Location = new System.Drawing.Point(13, 13);
            this.UsernameLabel.Name = "UsernameLabel";
            this.UsernameLabel.Size = new System.Drawing.Size(366, 17);
            this.UsernameLabel.TabIndex = 0;
            this.UsernameLabel.Text = "Please, enter your username (255 characters maximum):";
            // 
            // UsernameInputField
            // 
            this.UsernameInputField.Location = new System.Drawing.Point(13, 43);
            this.UsernameInputField.MaxLength = 255;
            this.UsernameInputField.Name = "UsernameInputField";
            this.UsernameInputField.Size = new System.Drawing.Size(408, 22);
            this.UsernameInputField.TabIndex = 1;
            this.UsernameInputField.TextChanged += new System.EventHandler(this.UsernameInputField_TextChanged);
            // 
            // SubmitUsernameButton
            // 
            this.SubmitUsernameButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.SubmitUsernameButton.Enabled = false;
            this.SubmitUsernameButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SubmitUsernameButton.Location = new System.Drawing.Point(113, 103);
            this.SubmitUsernameButton.Name = "SubmitUsernameButton";
            this.SubmitUsernameButton.Size = new System.Drawing.Size(200, 60);
            this.SubmitUsernameButton.TabIndex = 2;
            this.SubmitUsernameButton.Text = "Submit";
            this.SubmitUsernameButton.UseVisualStyleBackColor = true;
            // 
            // UsernameDia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 178);
            this.Controls.Add(this.SubmitUsernameButton);
            this.Controls.Add(this.UsernameInputField);
            this.Controls.Add(this.UsernameLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UsernameDia";
            this.Text = "Choose your username";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label UsernameLabel;
        private System.Windows.Forms.Button SubmitUsernameButton;
        internal System.Windows.Forms.TextBox UsernameInputField;
    }
}