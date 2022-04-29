using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Project_Backup_Restore
{
    public partial class MainView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MainView()
        {
            InitializeComponent();
        }

        // Kiểm tra form có mở không //
        private Form CheckExists(Type ftype)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f.GetType() == ftype)
                    return f;
            }
            return null;
        }

        private void btnDangNhap_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExists(typeof(login));
            if (frm != null) frm.Activate();
            else
            {
                login f = new login();
                f.MdiParent = this;
                f.Show();
            }
        }

        private void btnChucNang_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frmv = this.CheckExists(typeof(viewData));
            if (frmv != null) frmv.Activate();
            else
            {
                viewData v = new viewData();
                v.MdiParent = this;
                v.Show();
            }
        }

        private void MainView_Load(object sender, EventArgs e)
        {
            ribDanhMuc.Visible = false;
        }

        public void check_login ()
        {
            ribDanhMuc.Visible = true;
        }
        public void check_login2 ()
        {
            ribDanhMuc.Visible = false;
        }
    }
}
