using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab3GUI
{
    public partial class UsernameDia : Form
    {
        public UsernameDia()
        {
            InitializeComponent();
        }

        private void UsernameInputField_TextChanged(object sender, EventArgs e)
        {
            if (UsernameInputField.Text.Length > 0)
            {
                SubmitUsernameButton.Enabled = true;
            } else
            {
                SubmitUsernameButton.Enabled = false;
            }
        }
    }
}
