using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Oracle.DataAccess.Client;

namespace ConnectToOracle
{
    
    public partial class Form1 : Form
    {
        public OracleConnection conn;
        public DataSet ds;
        public Form1()
        {
            InitializeComponent();
            tabControl1.TabPages.Remove(tabPage2);
            //tabControl1.TabPages[1].Hide();
        }

        

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            
            if(ConnectButton.Text == "Отключиться")
            {
                conn.Close();
                textBox1.Text += "Отключение выполнено\r\n";
                ConnectButton.Text = "Подключиться";
                groupBox2.Enabled = false;
                tabControl1.Enabled = false;
                groupBox1.Enabled = true;
                QueryText.Clear();
                textBox1.Clear();
                dataGridView1.Hide();
                dataGridView2.Hide();
                tabControl1.TabPages.Remove(tabPage2);
            }
            else
            {
                string protocol = textBox6.Text;
                string ip = textBox5.Text;
                string port = textBox7.Text;
                string login = textBox3.Text;
                string password = textBox4.Text;
                try
                {
                    //Connection Information
                    string oracleDbConnection = "Data Source=(DESCRIPTION="
                                                                               + "(ADDRESS_LIST="
                                                                                       + "(ADDRESS="
                                                                                       + "(PROTOCOL="+protocol+")"
                                                                                       + "(HOST="+ip+")"
                                                                                       + "(PORT="+port+")"
                                                                                       + ")"
                                                                               + ")"
                                                                               + "(CONNECT_DATA="
                                                                                       + "(SERVER=DEDICATED)"
                                                                                       + "(SERVICE_NAME=xe)"
                                                                               + ")"
                                                                        + ");"
                               + "User Id="+login+";Password="+password+";";
                    //Connection to datasource, using connection parameters given above
                    conn = new OracleConnection(oracleDbConnection);
                    conn.Open();
                    textBox1.Text += "Подключение выполнено\r\n";
                    ConnectButton.Text = "Отключиться";
                    groupBox2.Enabled = true;
                    groupBox1.Enabled = false;
                    tabControl1.Enabled = true;
                    dataGridView1.Show();
                    
                    if (login == "system")
                    {
                        tabControl1.TabPages.Add(tabPage2);
                        string query = "select username, ACCOUNT_STATUS, TABLE_NAME, PRIVILEGE, grantor from (SELECT * FROM DBA_TAB_PRIVS INNER JOIN dba_users ON DBA_TAB_PRIVS.GRANTEE = DBA_USERS.USERNAME WHERE TABLE_NAME = 'FILMS' and OWNER = 'SYSTEM')";
                        OracleCommand cmd = new OracleCommand(query, conn);
                        OracleDataReader rows = cmd.ExecuteReader();

                        ds = new DataSet();
                        OracleDataAdapter adapter = new OracleDataAdapter(query, conn);
                        adapter.Fill(ds);
                        dataGridView2.DataSource = ds.Tables[0];
                        //tabControl1.TabPages[1].Show();// BringToFront();
                        dataGridView2.Show();
                    }
                    else {
                        tabControl1.TabPages.Remove(tabPage2);
                        
                    }
                }
                catch (Exception ex)
                {
                    textBox1.Text += "\r\nПодключение не выполнено: ";
                    textBox1.Text += ex;
                    string invalid = "invalid username/password";
                    if (ex.ToString().Contains(invalid))
                        MessageBox.Show("Вы не прошли авторизацию. Проверьте правильность ввода логина и пароля.");
                    invalid = "the account is locked";
                    if (ex.ToString().Contains(invalid))
                        MessageBox.Show("Вы заблокированы администратором.");
                    

                }

            }
            
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            try
            {
                string query = "SELECT * FROM system.films";
                OracleCommand cmd = new OracleCommand(query, conn);
                OracleDataReader rows = cmd.ExecuteReader();

                ds = new DataSet();
                OracleDataAdapter adapter = new OracleDataAdapter(query, conn);
                adapter.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                dataGridView1.Sort(dataGridView1.Columns["ID"], ListSortDirection.Ascending);
            }
            catch (Exception ex)
            {
                textBox1.Text += "\r\nОперация не может быть выполнена: ";
                textBox1.Text += ex;
                string invalid = "insufficient privileges";
                if (ex.ToString().Contains(invalid))
                    MessageBox.Show("У Вас недостаточно прав для выполнения этой операции");

            }

        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            try
            {
                string insert = "insert into system.films values ('','«V» значит Вендетта','2006','США','Джеймс МакТиг','132 мин','фантастика, боевик, триллер')";
                string selectall = "SELECT * FROM system.films";
                string commit = "commit";
                OracleCommand cmd = new OracleCommand(insert, conn);
                OracleDataReader rows = cmd.ExecuteReader();

                ds = new DataSet();
                OracleDataAdapter adapter = new OracleDataAdapter(selectall, conn);
                adapter.Fill(ds);
                cmd = new OracleCommand(commit, conn);
                rows = cmd.ExecuteReader();
                dataGridView1.DataSource = ds.Tables[0];
                QueryText.Clear();
            }
            catch (Exception ex)
            {
                textBox1.Text += "\r\nОперация не может быть выполнена: ";
                textBox1.Text += ex;
                string invalid = "insufficient privileges";
                if (ex.ToString().Contains(invalid))
                    MessageBox.Show("У Вас недостаточно прав для выполнения этой операции");

            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                string id = dataGridView1[dataGridView1.CurrentCellAddress.X, dataGridView1.CurrentCellAddress.Y].Value.ToString();
                string delete = "delete from system.films where id="+id;
                string selectall = "SELECT * FROM system.films";
                string commit = "commit";
                OracleCommand cmd = new OracleCommand(delete, conn);
                OracleDataReader rows = cmd.ExecuteReader();

                ds = new DataSet();
                OracleDataAdapter adapter = new OracleDataAdapter(selectall, conn);
                adapter.Fill(ds);
                cmd = new OracleCommand(commit, conn);
                rows = cmd.ExecuteReader();
                dataGridView1.DataSource = ds.Tables[0];
                QueryText.Clear();
            }
            catch (Exception ex)
            {
                textBox1.Text += "\r\nОперация не может быть выполнена: ";
                textBox1.Text += ex;
                string invalid = "insufficient privileges";
                if (ex.ToString().Contains(invalid))
                    MessageBox.Show("У Вас недостаточно прав для выполнения этой операции");
   
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void QueryButton_Click(object sender, EventArgs e)
        {
            
            try
            {

                string query = QueryText.Text;
                //string selectall = "SELECT * FROM system.films";
                string commit = "commit";
                OracleCommand cmd = new OracleCommand(query, conn);
                OracleDataReader rows = cmd.ExecuteReader();

                ds = new DataSet();
                OracleDataAdapter adapter = new OracleDataAdapter(query, conn);
                adapter.Fill(ds);
                cmd = new OracleCommand(commit, conn);
                rows = cmd.ExecuteReader();
                dataGridView1.DataSource = ds.Tables[0];
                QueryText.Clear();
               
            }
            catch (Exception ex)
            {
                textBox1.Text += "\r\nОперация не может быть выполнена: ";
                textBox1.Text += ex;
                string invalid = "insufficient privileges";
                string nontext = "OracleCommand.CommandText is invalid";
                if (ex.ToString().Contains(invalid))
                    MessageBox.Show("У Вас недостаточно прав для выполнения этой операции");
                if (ex.ToString().Contains(nontext))
                    MessageBox.Show("Ваш запрос некорректен либо пуст. Проверьте его правильность.");

            }
        }

        private void BlockButton_Click(object sender, EventArgs e)
        {
            //int user = dataGridView2.SelectedCells.Count;
           // MessageBox.Show(user.ToString());
            try {
                string user = dataGridView2[dataGridView2.CurrentCellAddress.X,dataGridView2.CurrentCellAddress.Y].Value.ToString();
                string query = "ALTER USER " + user + " ACCOUNT LOCK";
                string commit = "commit";
                OracleCommand cmd = new OracleCommand(query, conn);
                OracleDataReader rows = cmd.ExecuteReader();
                rows = cmd.ExecuteReader();
                cmd = new OracleCommand(commit, conn);
                rows = cmd.ExecuteReader();

                string queryall = "select username, ACCOUNT_STATUS, TABLE_NAME, PRIVILEGE, grantor from (SELECT * FROM DBA_TAB_PRIVS INNER JOIN dba_users ON DBA_TAB_PRIVS.GRANTEE = DBA_USERS.USERNAME WHERE TABLE_NAME = 'FILMS' and OWNER = 'SYSTEM')";
                cmd = new OracleCommand(queryall, conn);
                rows = cmd.ExecuteReader();
                ds = new DataSet();
                OracleDataAdapter adapter = new OracleDataAdapter(queryall, conn);
                adapter.Fill(ds);
                dataGridView2.DataSource = ds.Tables[0];
                
       
            }
            catch (Exception ex)
            {
                textBox1.Text += "\r\nОперация не может быть выполнена: ";
                textBox1.Text += ex;
                string invalid = "insufficient privileges";
                if (ex.ToString().Contains(invalid))
                    MessageBox.Show("У Вас недостаточно прав для выполнения этой операции");

            }
        }

        private void DefreezeButton_Click(object sender, EventArgs e)
        {
            try
            {
                string user = dataGridView2[dataGridView2.CurrentCellAddress.X, dataGridView2.CurrentCellAddress.Y].Value.ToString();
                string query = "ALTER USER " + user + " ACCOUNT UNLOCK";
                string commit = "commit";
                OracleCommand cmd = new OracleCommand(query, conn);
                OracleDataReader rows = cmd.ExecuteReader();
                rows = cmd.ExecuteReader();
                cmd = new OracleCommand(commit, conn);
                rows = cmd.ExecuteReader();

                string queryall = "select username, ACCOUNT_STATUS, TABLE_NAME, PRIVILEGE, grantor from (SELECT * FROM DBA_TAB_PRIVS INNER JOIN dba_users ON DBA_TAB_PRIVS.GRANTEE = DBA_USERS.USERNAME WHERE TABLE_NAME = 'FILMS' and OWNER = 'SYSTEM')";
                cmd = new OracleCommand(queryall, conn);
                rows = cmd.ExecuteReader();
                ds = new DataSet();
                OracleDataAdapter adapter = new OracleDataAdapter(queryall, conn);
                adapter.Fill(ds);
                dataGridView2.DataSource = ds.Tables[0];

            }
            catch (Exception ex)
            {
                textBox1.Text += "\r\nОперация не может быть выполнена: ";
                textBox1.Text += ex;
                string invalid = "insufficient privileges";
                if (ex.ToString().Contains(invalid))
                    MessageBox.Show("У Вас недостаточно прав для выполнения этой операции");

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string queryall = "select username, ACCOUNT_STATUS, TABLE_NAME, PRIVILEGE, grantor from (SELECT * FROM DBA_TAB_PRIVS INNER JOIN dba_users ON DBA_TAB_PRIVS.GRANTEE = DBA_USERS.USERNAME WHERE TABLE_NAME = 'FILMS' and OWNER = 'SYSTEM')";
                OracleCommand cmd = new OracleCommand(queryall, conn);
                //rows = cmd.ExecuteReader();
                ds = new DataSet();
                OracleDataAdapter adapter = new OracleDataAdapter(queryall, conn);
                adapter.Fill(ds);
                dataGridView2.DataSource = ds.Tables[0];

            }
            catch (Exception ex)
            {
                textBox1.Text += "\r\nОперация не может быть выполнена: ";
                textBox1.Text += ex;
                string invalid = "insufficient privileges";
                if (ex.ToString().Contains(invalid))
                    MessageBox.Show("У Вас недостаточно прав для выполнения этой операции");

            }
        }

        

        

        

        
    }
    /*public class dbConnection
    {
        OracleConnection conn;
        public Boolean getDBConnection()
        {
            try
            {
                //Connection Information
                string oracleDbConnection = "Data Source=(DESCRIPTION="
                                                                           + "(ADDRESS_LIST="
                                                                                   + "(ADDRESS="
                                                                                   + "(PROTOCOL=TCP)"
                                                                                   + "(HOST=foxwinter)"
                                                                                   + "(PORT=1521)"
                                                                                   + ")"
                                                                           + ")"
                                                                           + "(CONNECT_DATA="
                                                                                   + "(SERVER=DEDICATED)"
                                                                                   + "(SERVICE_NAME=xe)"
                                                                           + ")"
                                                                    + ");"
                           + "User Id=SYSTEM;Password=789789;";
                //Connection to datasource, using connection parameters given above
                conn = new OracleConnection(oracleDbConnection);
                conn.Open();
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

        }

    }*/
}


/*dbConnection conne = new dbConnection();
            if (conne.getDBConnection())
            {
                textBox1.Text = "OK";
                OracleCommand cmd = new OracleCommand("SELECT * FROM system.laba1", conn);
                OracleDataReader rows = cmd.ExecuteReader();

                Console.WriteLine(rows.GetName(0));
                Console.WriteLine(rows.GetName(1));
                Console.WriteLine(rows.Read());
                Console.WriteLine(rows.GetValue(0));
                Console.WriteLine(rows.GetValue(1));
            }
            else
            {
                textBox1.Text = "НЕТ";
            }
            
            //Console.ReadKey();*/