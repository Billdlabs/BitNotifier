using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLNotifications
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = Properties.Settings.Default.currency;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            //notifications.Instance.MtGoxApiKey = txtKey.Text;
            //notifications.Instance.MtGoxSecret = txtSecret.Text;
            //notifications.Instance.RefreshOrderHistory();

            //update the settings
            Properties.Settings.Default.currency = comboBox1.SelectedIndex;
            Properties.Settings.Default.Save();
            notifications.Instance.CurrencyNumber = Properties.Settings.Default.currency;
            notifications.Instance.UpdateCurrency();
            MessageBox.Show("Changes will be displayed with the next update from Mt. Gox");
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
