using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TanglewoodLMS
{
    public partial class Report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    PopulateYear();
                    PopulateDropDown("dbo.GradeDropdown", gradeList, "txtGrade");
                    PopulateDropDown("dbo.StudentDropdownReport", studentList, "txtName");
                }
                catch (Exception ex)
                {
                    errorLabel.Text = (ex.ToString());
                }
            }
        }

        protected void exitButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("Announcements.aspx");
        }

        protected void gradeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            studentList.Items.Clear();
            studentList.Items.Add(new ListItem("Student", "0"));
            PopulateDropDown("dbo.StudentDropdownReport", studentList, "txtName");
        }
        protected void yearList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (studentList.SelectedIndex != 0 && termList.SelectedIndex != 0 && yearList.SelectedIndex != 0)
            {
                PopulateMarkbook();
                CreateTitle();
            }
            else
            {
                ClearMarkbook(reportView);
            }
        }

        protected void termList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (studentList.SelectedIndex != 0 && termList.SelectedIndex != 0 && yearList.SelectedIndex != 0)
            {
                PopulateMarkbook();
                CreateTitle();
            }
            else
            {
                ClearMarkbook(reportView);
            }
        }

        protected void studentList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (studentList.SelectedIndex != 0 && termList.SelectedIndex != 0 && yearList.SelectedIndex != 0)
            {
                PopulateMarkbook();
                CreateTitle();
            }
            else
            {
                //ClearMarkbook();
            }
        }

        public SqlConnection CreateConnection()
        {
            string constr = ConfigurationManager.ConnectionStrings["TanglewoodConnectionString"].ConnectionString;
            try
            {
                SqlConnection con = new SqlConnection(constr);
                con.Open();
                return con;
            }
            catch (Exception ex)
            {
                errorLabel.Text = (ex.ToString());
                return null;
            }
        }

        public void PopulateDropDown(string procName, DropDownList dropdown, string field)
        {
            try
            {
                SqlConnection con = CreateConnection();
                if (dropdown != studentList)
                {
                    using (SqlCommand cmd = new SqlCommand(procName, con) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    dropdown.Items.Add(new ListItem(reader[field].ToString(), reader["IDX"].ToString()));
                                }
                            }
                            con.Close();
                        }
                    }
                }
                else
                {
                    using (SqlCommand cmd = new SqlCommand(procName, con) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        if (gradeList.SelectedIndex != 0)
                        {
                            cmd.Parameters.Add(new SqlParameter("@Grade", gradeList.SelectedValue));
                        }
                        else
                        {
                            cmd.Parameters.Add(new SqlParameter("@Grade", DBNull.Value));
                        }
                        if (yearList.SelectedIndex != 0)
                        {
                            cmd.Parameters.Add(new SqlParameter("@Year", yearList.SelectedValue));
                        }
                        else
                        {
                            cmd.Parameters.Add(new SqlParameter("@Year", DBNull.Value));
                        }
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    dropdown.Items.Add(new ListItem((reader["IDX"].ToString() + " - " + reader["txtName"].ToString()) + " " + reader["txtSurname"].ToString() , reader["IDX"].ToString()));
                                }
                            }
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorLabel.Text = (ex.ToString());
            }
        }

        public void ClearMarkbook(GridView markbook)
        {
            markbook.Controls.Clear();
        }

        public string GetPercentage(string mark, string totalMark)
        {
            int markInt = Convert.ToInt32(mark);
            int totalMarkInt = Convert.ToInt32(totalMark);
            if(mark != null || markInt != 0)
            {
                int percentInt = (markInt / totalMarkInt) * 100;
                string percent = percentInt.ToString() + "%";
                return percent;
            }
            return "?%";
        }

        public string GetFinalPercentage(string mark1, string totalMark1, string mark2, string totalMark2, string mark3, string totalMark3)
        {
            int markInt1 = Convert.ToInt32(mark1);
            int markInt2 = Convert.ToInt32(mark2);
            int markInt3 = Convert.ToInt32(mark3);
            int totalMarkInt1 = Convert.ToInt32(totalMark1);
            int totalMarkInt2 = Convert.ToInt32(totalMark2);
            int totalMarkInt3 = Convert.ToInt32(totalMark3);
            if (mark1 != null || markInt1 != 0 || mark2 != null || markInt2 != 0 || mark3 != null || markInt3 != 0)
            {
                int percentInt = ((markInt1 + markInt2 + markInt3) / (totalMarkInt1 + totalMarkInt2 + totalMarkInt3)) * 100;
                string percent = percentInt.ToString() + "%";
                return percent;
            }
            return "?%";
        }

        public string GetYearMark(string term1, string term2, string term3, string term4)
        {
            return null;
        }

        public void PopulateMarkbook()
        {
            try
            {
                if (termList.SelectedValue != "5")
                {
                    ClearMarkbook(finalReportView);
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("Subject", typeof(string));
                    dataTable.Columns.Add("Assessment 1", typeof(string));
                    dataTable.Columns.Add("Assessment 2", typeof(string));
                    dataTable.Columns.Add("Assessment 3", typeof(string));
                    dataTable.Columns.Add("Final Percentage", typeof(string));
                    using (SqlConnection con = CreateConnection())
                    using (SqlCommand cmd = new SqlCommand("dbo.DisplayReport", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@Term", termList.SelectedValue);
                        cmd.Parameters.AddWithValue("@Student", studentList.SelectedValue);
                        cmd.Parameters.AddWithValue("@Year", yearList.SelectedValue);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = dataTable.NewRow();
                                row["Subject"] = reader["txtSubject"].ToString();
                                row["Assessment 1"] = GetPercentage(reader["intMark1"].ToString(), reader["intTotalMark1"].ToString());
                                row["Assessment 2"] = GetPercentage(reader["intMark2"].ToString(), reader["intTotalMark2"].ToString());
                                row["Assessment 3"] = GetPercentage(reader["intMark3"].ToString(), reader["intTotalMark3"].ToString());
                                row["Final Percentage"] = GetFinalPercentage(
                                    reader["intMark1"].ToString(),
                                    reader["intTotalMark1"].ToString(),
                                    reader["intMark2"].ToString(),
                                    reader["intTotalMark2"].ToString(),
                                    reader["intMark3"].ToString(),
                                    reader["intTotalMark3"].ToString()
                                );

                                dataTable.Rows.Add(row);
                            }
                        }
                    }
                    reportView.DataSource = dataTable;
                    reportView.DataBind();
                }
                else
                {
                    ClearMarkbook(reportView);
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("Subject", typeof(string));
                    dataTable.Columns.Add("Term 1", typeof(string));
                    dataTable.Columns.Add("Term 2", typeof(string));
                    dataTable.Columns.Add("Term 3", typeof(string));
                    dataTable.Columns.Add("Term 4", typeof(string));
                    dataTable.Columns.Add("Year Mark", typeof(string));
                    using (SqlConnection con = CreateConnection())
                    using (SqlCommand cmd = new SqlCommand("dbo.DisplayReport", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@Term", termList.SelectedValue);
                        cmd.Parameters.AddWithValue("@Student", studentList.SelectedValue);
                        cmd.Parameters.AddWithValue("@Year", yearList.SelectedValue);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = dataTable.NewRow();
                                row["Subject"] = reader["txtSubject"].ToString();
                                row["Assessment 1"] = GetPercentage(reader["intMark1"].ToString(), reader["intTotalMark1"].ToString());
                                row["Assessment 2"] = GetPercentage(reader["intMark2"].ToString(), reader["intTotalMark2"].ToString());
                                row["Assessment 3"] = GetPercentage(reader["intMark3"].ToString(), reader["intTotalMark3"].ToString());
                                row["Final Percentage"] = GetFinalPercentage(
                                    reader["intMark1"].ToString(),
                                    reader["intTotalMark1"].ToString(),
                                    reader["intMark2"].ToString(),
                                    reader["intTotalMark2"].ToString(),
                                    reader["intMark3"].ToString(),
                                    reader["intTotalMark3"].ToString()
                                );

                                dataTable.Rows.Add(row);
                            }
                        }
                    }
                    reportView.DataSource = dataTable;
                    reportView.DataBind();
                }
            }
            catch (Exception ex)
            {
                errorLabel.Text = ex.ToString();
            }
        }

        public void CreateTitle()
        {
            markHeader.Text = "ID " + studentList.SelectedItem + " - " + gradeList.SelectedItem + " - " + termList.SelectedItem + " - " + yearList.SelectedValue;
        }

        public void PopulateYear()
        {
            for (int currentYear = Convert.ToInt32((DateTime.Now).ToString().Substring(0, Math.Min(4, (DateTime.Now).ToString().Length))); currentYear > 2023; currentYear--)
            {
                yearList.Items.Add(new ListItem(currentYear.ToString(), currentYear.ToString()));
            }
            
        }

        protected void downloadButton_Click(object sender, EventArgs e)
        {

        }
    }
}