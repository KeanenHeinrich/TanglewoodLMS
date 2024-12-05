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
    public partial class Markbook : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    PopulateDropDown("dbo.GradeDropdown", gradeList, "txtGrade");
                    PopulateDropDown("dbo.SubjectDropdown", subjectList, "txtSubject");
                    PopulateDropDown("dbo.ClassDropdown", classList, "txtClass");
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
            classList.Items.Clear();
            classList.Items.Add(new ListItem("Class", "0"));
            PopulateDropDown("dbo.ClassDropdown", classList, "txtClass");
        }

        protected void subjectList_SelectedIndexChanged(object sender, EventArgs e)
        {
            classList.Items.Clear();
            classList.Items.Add(new ListItem("Class", "0"));
            PopulateDropDown("dbo.ClassDropdown", classList, "txtClass");
        }

        protected void termList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (classList.SelectedIndex != 0 && termList.SelectedIndex != 0)
            {
                PopulateMarkbook();
                CreateTitle();
            }
            else
            {
                ClearMarkbook();
            }
        }

        protected void classList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (termList.SelectedIndex != 0 && classList.SelectedIndex != 0)
            {
                PopulateMarkbook();
                CreateTitle();
            }
            else
            {
                ClearMarkbook();
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
                if (dropdown != classList)
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
                        if(gradeList.SelectedIndex != 0)
                        {
                            cmd.Parameters.Add(new SqlParameter("@Grade", gradeList.SelectedValue));
                        }
                        else
                        {
                            cmd.Parameters.Add(new SqlParameter("@Grade", DBNull.Value));
                        }

                        if (subjectList.SelectedIndex != 0)
                        {
                            cmd.Parameters.Add(new SqlParameter("@Subject", subjectList.SelectedValue));
                        }
                        else
                        {
                            cmd.Parameters.Add(new SqlParameter("@Subject", DBNull.Value));
                        }
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
            }
            catch (Exception ex)
            {
                errorLabel.Text = (ex.ToString());
            }
        }

        public void ClearMarkbook()
        {
            markbookView.DataSource = null;
            markbookView.DataBind();
        }

        public void PopulateMarkbook()
        {
            try
            {
                SqlConnection con = CreateConnection();
                using (SqlCommand cmd = new SqlCommand("dbo.FetchMarkbook", con) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    cmd.Parameters.Add(new SqlParameter("@Term", termList.SelectedValue));
                    cmd.Parameters.Add(new SqlParameter("@Class", classList.SelectedValue));
                    cmd.Parameters.Add(new SqlParameter("@Year", (DateTime.Now).ToString().Substring(0, Math.Min(4, (DateTime.Now).ToString().Length))));
                    using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader.HasRows)
                        {
                            {
                                markbookView.DataSource = reader;
                                markbookView.DataBind();
                            }
                        }
                    }
                }
                SqlConnection con2 = CreateConnection();
                using (SqlCommand cmd = new SqlCommand("dbo.FetchMaxMarks", con2) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    TemplateField field;
                    cmd.Parameters.Add(new SqlParameter("@Class", classList.SelectedValue));
                    cmd.Parameters.Add(new SqlParameter("@Year", ((DateTime.Now).ToString().Substring(0, Math.Min(4, (DateTime.Now).ToString().Length)))));
                    using (SqlDataReader reader2 = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader2.HasRows)
                        {
                            
                            reader2.Read();
                            for (int i = 0; i < 3; i++)
                            {
                                field = markbookView.Columns[(i+2)] as TemplateField;
                                switch (i)
                                {
                                    case 0: field.HeaderText = reader2["intTotalMark1"].ToString(); break;
                                    case 1: field.HeaderText = reader2["intTotalMark2"].ToString(); break;
                                    case 2: field.HeaderText = reader2["intTotalMark3"].ToString(); break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorLabel.Text = (ex.ToString());
            }
        }

        protected void editButton_Click(object sender, EventArgs e)
        {
            try
            {
                loggedInUser currentUser = (loggedInUser)Session["loggedInUser"];
                foreach (GridViewRow row in markbookView.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        using (SqlConnection con = CreateConnection())
                        {
                            using (SqlCommand cmd = new SqlCommand("dbo.UpdateMarkBook", con) { CommandType = System.Data.CommandType.StoredProcedure })
                            {
                                Label idx = (Label)row.FindControl("idx");
                                cmd.Parameters.Add(new SqlParameter("@StudentID", idx.Text));
                                cmd.Parameters.Add(new SqlParameter("@Class", classList.SelectedValue));
                                TextBox mark1 = (TextBox)row.FindControl("intMark1");
                                cmd.Parameters.Add(new SqlParameter("@Mark1", mark1.Text));
                                TextBox mark2 = (TextBox)row.FindControl("intMark2");
                                cmd.Parameters.Add(new SqlParameter("@Mark2", mark2.Text));
                                TextBox mark3 = (TextBox)row.FindControl("intMark3");
                                cmd.Parameters.Add(new SqlParameter("@Mark3", mark3.Text));
                                cmd.Parameters.Add(new SqlParameter("@Year", (DateTime.Now).ToString().Substring(0, Math.Min(4, (DateTime.Now).ToString().Length))));
                                cmd.Parameters.Add(new SqlParameter("@UpdateWho", SqlDbType.NVarChar, 50) { Value = (currentUser.UserId).ToString() });
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                ClearMarkbook();
                PopulateMarkbook();
            }
            catch( Exception ex)
            {
                errorLabel.Text = ex.ToString();
            }
        }

        protected void cancelButton_Click(object sender, EventArgs e)
        {
            PopulateMarkbook();
        }

        protected void markbookView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    TextBox intMark1 = (TextBox)e.Row.FindControl("intmark1");
                    TextBox intMark2 = (TextBox)e.Row.FindControl("intmark2");
                    TextBox intMark3 = (TextBox)e.Row.FindControl("intmark3");
                    Label FinalTotal = (Label)e.Row.FindControl("finalMark");
                    if (intMark1 != null && intMark2 != null && intMark3 != null && FinalTotal != null)
                    {
                        int mark1, mark2, mark3;
                        int.TryParse(intMark1.Text, out mark1);
                        int.TryParse(intMark2.Text, out mark2);
                        int.TryParse(intMark3.Text, out mark3);
                        int markSum = mark1 + mark2 + mark3;
                        FinalTotal.Text = markSum.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                errorLabel.Text = ex.Message;
            }
        }

        public void CreateTitle()
        {
            markHeader.Text = subjectList.SelectedItem + " Markbook - " + gradeList.SelectedItem + " - " + (DateTime.Now).ToString().Substring(0, Math.Min(4, (DateTime.Now).ToString().Length));
        }
    }
}