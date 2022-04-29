using DevExpress.Skins;
using DevExpress.UserSkins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace Project_Backup_Restore
{
    static class Program
    {
        public static SqlConnection conn = new SqlConnection();
        public static String connstr;
        public static String serverName = string.Empty;
        public static String userName = string.Empty;
        public static String passWord = string.Empty;
        public static String databasename = string.Empty;
        public static String localsave = string.Empty;
        public static String filePath = string.Empty;
        public static MainView mainview;
        public static SqlDataReader myReader;

        // Read Data Sql //
        public static SqlDataReader ExecSqlDataReader(String strlenh)
        {
            SqlDataReader myreader;
            SqlCommand sqlcmd = new SqlCommand(strlenh, Program.conn);
            sqlcmd.CommandType = CommandType.Text;
            try
            {
                myreader = sqlcmd.ExecuteReader();
                return myreader;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        
        public static int KetNoi()
        {
            if (Program.conn != null && Program.conn.State == ConnectionState.Open)
                Program.conn.Close();
            try
            {
                Program.connstr = "Data Source=" + Program.serverName + ";User ID=" +
                    Program.userName + ";password=" + Program.passWord;
                Program.conn.ConnectionString = Program.connstr;
                Program.conn.Open();
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu.\n Bạn xem lại username và password.\n" + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mainview = new MainView();
            Application.Run(mainview);
        }
    }
}
