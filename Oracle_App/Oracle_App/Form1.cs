﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;

namespace Oracle_App
{
    public partial class Form1 : Form
    {
        // Instance connection object
        OracleConnection con = null;
        public Form1()
        {
            this.setConnection();
            InitializeComponent();
        }

        private void setConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["HrDB"].ConnectionString;
            con = new OracleConnection(connectionString); // Oracle connection object
            try
            {
                con.Open();
            }
            catch(Exception exp)
            {

            }
        }

        private void updateDataGrid()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "Select Employee_ID, Last_Name, Email, Job_ID, Hire_Date from employees"; // Sql statement
            cmd.CommandType = CommandType.Text; // Type of Sql statement
            OracleDataReader dr = cmd.ExecuteReader(); // Execute sql statement
            DataTable dt = new DataTable(); // Data table object
            dt.Load(dr); // Save executed sql statement
            dataGridView1.DataSource = dt.DefaultView;
            dr.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.updateDataGrid();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            con.Close();
        }

        private void IAD(String sql_stm, int state) // insert + update + delete
        {
            String msg = "";
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = sql_stm;
            cmd.CommandType = CommandType.Text;

            switch (state)
            {
                case 0:
                    cmd.Parameters.Add("EMPLOYEE_ID", OracleDbType.Int32, 6).Value = Int32.Parse(Employee_ID_textbox.Text);
                    cmd.Parameters.Add("LAST_NAME", OracleDbType.Varchar2, 25).Value = Last_Name_textbox.Text;
                    cmd.Parameters.Add("EMAIL", OracleDbType.Varchar2, 25).Value = Email_textbox.Text;
                    cmd.Parameters.Add("JOB_ID", OracleDbType.Varchar2, 10).Value = Job_ID_textbox.Text;
                    //cmd.Parameters.Add("HIRE_DATE", OracleDbType.Date, 7).Value = Hire_Date_picker.Text;
                    cmd.Parameters.Add("HIRE_DATE", OracleDbType.Date, 7).Value = Hire_Date_picker.Value.ToShortDateString();
                    msg = "Inserted Successfully";
                    break;
                case 1:
                    cmd.Parameters.Add("EMPLOYEE_ID", OracleDbType.Int32, 6).Value = Int32.Parse(Employee_ID_textbox.Text);
                    cmd.Parameters.Add("LAST_NAME", OracleDbType.Varchar2, 25).Value = Last_Name_textbox.Text;
                    cmd.Parameters.Add("EMAIL", OracleDbType.Varchar2, 25).Value = Email_textbox.Text;
                    cmd.Parameters.Add("JOB_ID", OracleDbType.Varchar2, 10).Value = Job_ID_textbox.Text;
                    cmd.Parameters.Add("HIRE_DATE", OracleDbType.Date, 7).Value = Hire_Date_picker.Value.ToShortDateString();
                    msg = "Updated Successfully";
                    break;
                case 2:
                    cmd.Parameters.Add("EMPLOYEE_ID", OracleDbType.Int32, 6).Value = Int32.Parse(Employee_ID_textbox.Text);
                    msg = "Deleted Successfully";
                    break;
            }

            try
            {
                int n = cmd.ExecuteNonQuery();
                if (n > 0)
                {
                    MessageBox.Show(msg);
                    this.updateDataGrid();
                }
            }
            catch (Exception exp)
            {

                throw;
            }
        }

        private void Add_button_Click(object sender, EventArgs e)
        {
            String sql = "Insert into EMPLOYEES(EMPLOYEE_ID, LAST_NAME, EMAIL, JOB_ID, HIRE_DATE)" +
                "Values(:EMPLOYEE_ID, :LAST_NAME, :EMAIL, :JOB_ID, :HIRE_DATE)";
            //String sql = "Insert into EMPLOYEES(EMPLOYEE_ID, LAST_NAME, EMAIL, JOB_ID)" +
            //    "Values(:EMPLOYEE_ID, :LAST_NAME, :EMAIL, :JOB_ID)";
            this.IAD(sql, 0); // insert
            Add_button.Enabled = false;
            Update_button.Enabled = true;
            Delete_button.Enabled = true;
        }

        private void Update_button_Click(object sender, EventArgs e)
        {
            String sql = "Update EMPLOYEES " +
                "set LAST_NAME = :LAST_NAME," +
                "JOB_ID = :JOB_ID," +
                "EMAIL = :EMAIL," +
                "HIRE_DATE = :HIRE_DATE " +
                "Where EMPLOYEE_ID = :EMPLOYEE_ID";

            //String sql = "Update EMPLOYEES " +
            //    "set LAST_NAME = :LAST_NAME," +
            //    "JOB_ID = :JOB_ID," +
            //    "EMAIL = :EMAIL " +
            //    "Where EMPLOYEE_ID = :EMPLOYEE_ID";
            this.IAD(sql, 1); // update
        }

        private void Delete_button_Click(object sender, EventArgs e)
        {
            String sql = "Delete from EMPLOYEES "+
                "where EMPLOYEE_ID = :EMPLOYEE_ID";
            this.IAD(sql, 2); // delete
        }

        private void ClearTextBoxes()
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is TextBox)
                        (control as TextBox).Clear();
                    else
                        func(control.Controls);
            };

            func(Controls);
        }

        private void Reset_button_Click(object sender, EventArgs e)
        {
            ClearTextBoxes();
            Hire_Date_picker.Text = null;
            Add_button.Enabled = true;
            Update_button.Enabled = false;
            Delete_button.Enabled = false;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            if (index != -1) // Nhan vao header khong tinh
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[index];
                Employee_ID_textbox.Text = selectedRow.Cells[0].Value.ToString();
                Last_Name_textbox.Text = selectedRow.Cells[1].Value.ToString();
                Email_textbox.Text = selectedRow.Cells[2].Value.ToString();
                Job_ID_textbox.Text = selectedRow.Cells[3].Value.ToString();
                Hire_Date_picker.Text = selectedRow.Cells[4].Value.ToString();
                //Hire_Date_picker.Value = DateTime.Parse(selectedRow.Cells[4].Value.ToString());

                Add_button.Enabled = false;
                Update_button.Enabled = true;
                Delete_button.Enabled = true;
            }
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            ClearTextBoxes();
            Hire_Date_picker.Text = null;
            Add_button.Enabled = true;
            Update_button.Enabled = false;
            Delete_button.Enabled = false;
        }
    }
}
