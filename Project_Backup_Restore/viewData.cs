using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;

namespace Project_Backup_Restore
{
    public partial class viewData : Form
    {
        public static int number;
        public static DateTime dateBackup;
        public static int numberBackupNew;

        public viewData()
        {
            InitializeComponent();
        }

        private void viewData_Load(object sender, EventArgs e)
        {
            try
            {
                btnSaoLuu.Enabled = false;
                btnPhucHoi.Enabled = false;
                ckTime.Enabled = false;

                if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
                SqlCommand cmd = new SqlCommand("select name from sys.databases where (database_id>=5) AND (NOT(name LIKE N'ReportServer%'))", Program.conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                da.Fill(table);
                //Gan du lieu
                dagDatabase.DataSource = table;
                Program.conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi không load được databases!!!" + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dagDatabase_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {

                int index = e.RowIndex;
                DataGridViewRow selectedRow = dagDatabase.Rows[index];
                Program.databasename = selectedRow.Cells[0].Value.ToString();
                txtTen.Text = Program.databasename;

                if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
                // Kiểm tra Device đã tồn tại chưa //
                String t = @"select name from sys.backup_devices where name='DEVICE_" + Program.databasename + "'";
                Program.myReader = Program.ExecSqlDataReader(t);
                if (Program.myReader == null) return;
                Program.myReader.Read();
                try
                {
                    String name = Program.myReader.GetString(0);
                    btnDevice.Enabled = false;
                    btnSaoLuu.Enabled = true;
                }
                catch (Exception)
                { 
                    btnDevice.Enabled = true;
                    btnSaoLuu.Enabled = false;
                    btnPhucHoi.Enabled = false;
                    ckTime.Enabled = false;
                }
                Program.myReader.Close();

                /////////////////Load file Backups///////////////////////

                SqlCommand cmd1 = new SqlCommand(@"SELECT position, name, backup_start_date, user_name FROM msdb.dbo.backupset
                         Where database_name = '" + Program.databasename + @"' AND type = 'D' AND backup_set_id >=
                                     (SELECT MAX(backup_set_id) FROM msdb.dbo.backupset
                                      WHERE media_set_id = 
                                                       (SELECT MAX(media_set_id) 
                                                        FROM msdb.dbo.backupset 
                                                        WHERE database_name = '" + Program.databasename + @"' AND type = 'D')
                                      AND position = 1)
                         ORDER BY position DESC", Program.conn);
                DataTable tablefile = new DataTable();
                SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                da1.Fill(tablefile);
                dagFile.DataSource = tablefile;
                Program.conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiễn thị!." + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSaoLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (ckXoaHet.Checked == false)
                {
                    String cmd = @"BACKUP DATABASE [" + Program.databasename + "] TO  [DEVICE_" + Program.databasename + "] WITH NOFORMAT, NOINIT,  NAME = N'" + Program.databasename + "-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10";
                    if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
                    SqlCommand command = new SqlCommand(cmd, Program.conn);
                    command.ExecuteNonQuery();
                    MessageBox.Show("Database backup done successfuly");
                    Program.conn.Close();
                }
                else
                {
                    String cmdz = @"BACKUP DATABASE [" + Program.databasename + "] TO  [DEVICE_" + Program.databasename + "] WITH NOFORMAT, INIT,  NAME = N'" + Program.databasename + "-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10";
                    if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
                    SqlCommand commandz = new SqlCommand(cmdz, Program.conn);
                    commandz.ExecuteNonQuery();
                    MessageBox.Show("Database backup done successfuly");
                    Program.conn.Close();
                }
                // Loading lại dataGridView sau khi Backup //
                SqlCommand cma = new SqlCommand(@"SELECT position, name, backup_start_date, user_name FROM msdb.dbo.backupset
                         Where database_name = '" + Program.databasename + @"' AND type = 'D' AND backup_set_id >=
                                     (SELECT MAX(backup_set_id) FROM msdb.dbo.backupset
                                      WHERE media_set_id = 
                                                       (SELECT MAX(media_set_id) 
                                                        FROM msdb.dbo.backupset 
                                                        WHERE database_name = '" + Program.databasename + @"' AND type = 'D')
                                      AND position = 1)
                         ORDER BY position DESC", Program.conn);
                DataTable tablefile = new DataTable();
                SqlDataAdapter da1 = new SqlDataAdapter(cma);
                da1.Fill(tablefile);
                dagFile.DataSource = tablefile;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể Backup!!!." + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPhucHoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (ckTime.Checked == false)
                {
                    if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
                    string str1 = string.Format("ALTER DATABASE [" + Program.databasename + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
                    SqlCommand cmd1 = new SqlCommand(str1, Program.conn);
                    cmd1.ExecuteNonQuery();

                    string str2 = @"USE [master] RESTORE DATABASE [" + Program.databasename + @"] FROM [DEVICE_" + Program.databasename + "] WITH FILE =" + number + ", NOUNLOAD, REPLACE, STATS = 5";
                    SqlCommand cmd2 = new SqlCommand(str2, Program.conn);
                    cmd2.ExecuteNonQuery();

                    string str3 = string.Format("ALTER DATABASE [" + Program.databasename + "] SET MULTI_USER");
                    SqlCommand cmd3 = new SqlCommand(str3, Program.conn);
                    cmd3.ExecuteNonQuery();

                    Program.conn.Close();
                    MessageBox.Show("Database Restore done successfuly");
                }
                else
                {
                    if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
                    String tai = dTPDate.Value.ToString();
                    DateTime stopat = DateTime.Parse(tai);
                    DateTime timeNow = DateTime.Now;
                    //if (stopat.Year > timeNow.Year || (stopat.Year==timeNow.Year && stopat.Month>timeNow.Month))
                    if (stopat >= timeNow || stopat < dateBackup)
                    {
                        MessageBox.Show("Thời gian chọn vượt quá hiện tại hoặc trước bản backup gần nhất", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        //  //
                        string st1 = string.Format("ALTER DATABASE [" + Program.databasename + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
                        SqlCommand cm1 = new SqlCommand(st1, Program.conn);
                        cm1.ExecuteNonQuery();


                        String log = @"BACKUP LOG [" + Program.databasename + @"] TO  [DEVICE_" + Program.databasename + @"] WITH NOFORMAT, NOINIT,  NAME = N'QLVT_DATHANG-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10";
                        if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
                        SqlCommand cmd = new SqlCommand(log, Program.conn);
                        cmd.ExecuteNonQuery();
                        String backup_time = @"USE [master] RESTORE DATABASE [" + Program.databasename + @"] FROM  [DEVICE_" + Program.databasename + @"] WITH  FILE = " + numberBackupNew + @",  NORECOVERY,  REPLACE
                                               RESTORE LOG [" + Program.databasename + @"] FROM  [DEVICE_" + Program.databasename + @"] WITH  FILE = " + (numberBackupNew + 1) + @", STOPAT = '" + stopat.Year + "-" + stopat.Month + "-" + stopat.Day + " " + stopat.Hour + ":" + stopat.Minute + ":" + stopat.Second + "'";
                        SqlCommand cmd2 = new SqlCommand(backup_time, Program.conn);
                        cmd2.ExecuteNonQuery();

                        string st2 = string.Format("ALTER DATABASE [" + Program.databasename + "] SET MULTI_USER");
                        SqlCommand cm2 = new SqlCommand(st2, Program.conn);
                        cm2.ExecuteNonQuery();

                        MessageBox.Show("Database backup done successfuly");
                        Program.conn.Close();
                    }
                    Program.conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể Restore!!!." + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDevice_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                FolderBrowserDialog dlg = new FolderBrowserDialog();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Program.filePath = dlg.SelectedPath;
                }
                // String cmd1 = "USE master";
                String cmd2 = @"USE master EXEC master.dbo.sp_addumpdevice  @devtype = N'disk', @logicalname = N'DEVICE_" + Program.databasename + @"', @physicalname = N'" + Program.filePath + @"\DEVICE_" + Program.databasename + ".bak'";
                Program.conn.Open();
                // SqlCommand comm1 = new SqlCommand(cmd1, Program.conn);
                // comm1.ExecuteNonQuery();
                SqlCommand comm2 = new SqlCommand(cmd2, Program.conn);
                comm2.ExecuteNonQuery();
                Program.conn.Close();
                btnDevice.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo Device!!!." + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dagFile_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int index = e.RowIndex;
                DataGridViewRow selectedRow = dagFile.Rows[index];
                txtSo.Text = selectedRow.Cells[1].Value.ToString();
                number = int.Parse(selectedRow.Cells[1].Value.ToString());
                btnPhucHoi.Enabled = true;
                ckTime.Enabled = true;
                
            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi lấy số thứ tự bản backup!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ckTime_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (ckTime.Checked)
                {
                    if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
                    String k = @"select position, backup_start_date from msdb.dbo.backupset 
                          WHERE backup_set_id = ( select MAX(backup_set_id) from msdb.dbo.backupset
                                                  WHERE database_name = '" + Program.databasename + @"' AND type = 'D' )";
                    Program.myReader = Program.ExecSqlDataReader(k);
                    if (Program.myReader == null) return;
                    Program.myReader.Read();
                    dateBackup = Program.myReader.GetDateTime(1);
                    numberBackupNew = Program.myReader.GetInt32(0);

                    Program.myReader.Close();
                }
            }
            catch (Exception) { }
        }

        private void btnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult ret = MessageBox.Show("Bạn có muốn thoát???", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ret == DialogResult.Yes)
            {
                Close();

            }
        }
    }
}
