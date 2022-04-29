using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using Microsoft.Win32;
namespace Project_Backup_Restore
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
        }

        // Load ServerName //
        private void GetLoadServer(ComboBox combo)
        {
            String serverName = Environment.MachineName;
            RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey registry = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                RegistryKey instansKey = registry.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL", false);
                if (instansKey != null)
                {
                    foreach (var instanceName in instansKey.GetValueNames())
                    {
                        combo.Items.Add(serverName + "\\" + instanceName);
                    }
                }
            }
        }

        private void login_Load(object sender, EventArgs e)
        {
            txtUsername.Enabled = false;
            txtPassword.Enabled = false;
            GetLoadServer(cbServer);
        }

        private void ckAut_CheckedChanged(object sender, EventArgs e)
        {
            if (ckAut.Checked)
            {
                txtUsername.Enabled = true;
                txtPassword.Enabled = true;
            }
            else
            {
                txtUsername.Enabled = false;
                txtPassword.Enabled = false;
            }
        }

        private void btnfrThoat_Click(object sender, EventArgs e)
        {
            DialogResult ret = MessageBox.Show("Bạn có muốn thoát???", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ret == DialogResult.Yes)
            {
                foreach (Form a in Program.mainview.MdiChildren)
                {
                    a.Close();
                }
                Program.mainview.check_login2();
            }
        }

        private void btnfrDangNhap_Click(object sender, EventArgs e)
        {
            Program.serverName = cbServer.Text;
            Program.userName = txtUsername.Text;
            Program.passWord = txtPassword.Text;
            if (Program.KetNoi() == 1)
            {
                MessageBox.Show("Kết nối thành công!!!", "Successfuly", MessageBoxButtons.OK);
                txtUsername.Enabled = false;
                txtPassword.Enabled = false;
                ckAut.Enabled = false;
                cbServer.Enabled = false;
                Program.mainview.check_login();
            }
            else
            {
                MessageBox.Show("Kết nối thất bại!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
