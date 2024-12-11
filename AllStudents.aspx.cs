using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace TanglewoodLMS
{
    public partial class AllStudents : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateDropDown("dbo.GradeDropdown", gradeStudent, "txtGrade");
                PopulateDropDown("dbo.ClassDropdown", classStudent, "txtClass");
                GetStudents();
            }
            SqlConnection con = CreateConnection();
            using (SqlCommand cmd = new SqlCommand("FetchStudentClasses", con) { CommandType = System.Data.CommandType.StoredProcedure })
            {
                cmd.Parameters.Add(new SqlParameter("@IDX", DBNull.Value));
                using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    classAssignRepeater.DataSource = reader;
                    classAssignRepeater.DataBind();
                }
            }
        }

        public void PopulateDropDown(string procName, DropDownList dropdown, string field)
        {
            try
            {
                SqlConnection con = CreateConnection();
                if (dropdown != classStudent)
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
                        if (gradeStudent.SelectedIndex != 0)
                        {
                            cmd.Parameters.Add(new SqlParameter("@Grade", gradeStudent.SelectedValue));
                        }
                        else
                        {
                            cmd.Parameters.Add(new SqlParameter("@Grade", DBNull.Value));
                        }
                        cmd.Parameters.Add(new SqlParameter("@Subject", DBNull.Value));
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
                
            }
        }

        public void PopulateDropDownRecord(string procName, HtmlGenericControl holder, string field)
        {
            try
            {
                DropDownList dropdownSub = holder.FindControl("Subject") as DropDownList;
                SqlConnection con = CreateConnection();
                if (field == "txtSubject")
                {
                    using (SqlCommand cmd = new SqlCommand(procName, con) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    dropdownSub.Items.Add(new ListItem(reader[field].ToString(), reader["IDX"].ToString()));
                                }
                            }
                            con.Close();
                        }
                    }
                }
                else
                {
                    DropDownList dropdown = holder.FindControl("Class") as DropDownList;
                    using (SqlCommand cmd = new SqlCommand(procName, con) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        if (dropdownSub.SelectedIndex != 0)
                        {
                            cmd.Parameters.Add(new SqlParameter("@Subject", dropdownSub.SelectedValue));
                        }
                        else
                        {
                            cmd.Parameters.Add(new SqlParameter("@Subject", DBNull.Value));
                        }
                        cmd.Parameters.Add(new SqlParameter("@Grade", DBNull.Value));
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
                return null;
            }
        }

        public void GetStudents()
        {
            studentsRepeater.DataSource = null;
            studentsRepeater.DataBind();
            SqlConnection con = CreateConnection();
            using (SqlCommand cmd = new SqlCommand("dbo.FetchStudent", con) { CommandType = System.Data.CommandType.StoredProcedure })
            {
                if (classStudent.SelectedValue != "")
                {
                    cmd.Parameters.Add(new SqlParameter("@Class", classStudent.SelectedValue));
                }
                else
                {
                    cmd.Parameters.Add(new SqlParameter("@Class", DBNull.Value));
                }
                cmd.Parameters.Add(new SqlParameter("@SearchInput", SqlDbType.NVarChar, 50) { Value = searchStudents.Text });
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (reader.HasRows)
                {
                    studentsRepeater.DataSource = reader;
                    studentsRepeater.DataBind();
                }
                con.Close();
            }
        }

        public void GetStudentDetails(HtmlGenericControl record, string IDX)
        {
            SqlConnection con = CreateConnection();
            SqlCommand cmd = new SqlCommand("dbo.FetchStudentDetails", con) { CommandType = System.Data.CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IDX", IDX));
            using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    TextBox nameBox = record.FindControl("nameBox") as TextBox;
                    nameBox.Text = reader["txtName"].ToString();
                    TextBox surnameBox = record.FindControl("surnameBox") as TextBox;
                    surnameBox.Text = reader["txtSurname"].ToString();
                    TextBox ID = record.FindControl("ID") as TextBox;
                    ID.Text = reader["IDX"].ToString();
                    TextBox govBox = record.FindControl("govBox") as TextBox;
                    govBox.Text = reader["txtGovID"].ToString();
                    TextBox enrollBox = record.FindControl("enrollBox") as TextBox;
                    enrollBox.Text = reader["datEnrolled"].ToString();
                    TextBox addressBox = record.FindControl("addressBox") as TextBox;
                    addressBox.Text = reader["txtAddress"].ToString();
                    TextBox genderBox = record.FindControl("genderBox") as TextBox;
                    genderBox.Text = reader["txtGender"].ToString();
                    TextBox dobBox = record.FindControl("dobBox") as TextBox;
                    dobBox.Text = reader["datDOB"].ToString();
                    TextBox emailBox = record.FindControl("emailBox") as TextBox;
                    emailBox.Text = reader["txtParentEmail"].ToString();
                    TextBox phoneBox = record.FindControl("phoneBox") as TextBox;
                    phoneBox.Text = reader["txtParentPhone"].ToString();
                    TextBox diffBox = record.FindControl("diffBox") as TextBox;
                    diffBox.Text = reader["txtLearningDifficulties"].ToString();
                    TextBox noteBox = record.FindControl("noteBox") as TextBox;
                    noteBox.Text = reader["txtAdditionalNotes"].ToString();
                }
            }
        }

        public void EditStudentDetails(HtmlGenericControl record, string IDX)
        {
            SqlConnection con = CreateConnection();
            SqlCommand cmd = new SqlCommand("dbo.UpdateStudent", con) { CommandType = System.Data.CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IDX", IDX));
            TextBox nameBox = record.FindControl("nameBox") as TextBox;
            cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 50) { Value = nameBox.Text });
            TextBox surnameBox = record.FindControl("surnameBox") as TextBox;
            cmd.Parameters.Add(new SqlParameter("@Surname", SqlDbType.NVarChar, 50) { Value = surnameBox.Text });
            TextBox govBox = record.FindControl("govBox") as TextBox;
            cmd.Parameters.Add(new SqlParameter("@GovID", SqlDbType.NVarChar, 13) { Value = govBox.Text });
            TextBox addressBox = record.FindControl("addressBox") as TextBox;
            cmd.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar, 50) { Value = addressBox.Text });
            TextBox genderBox = record.FindControl("genderBox") as TextBox;
            cmd.Parameters.Add(new SqlParameter("@Gender", SqlDbType.NVarChar, 50) { Value = genderBox.Text });
            TextBox dobBox = record.FindControl("dobBox") as TextBox;
            cmd.Parameters.Add(new SqlParameter("@DOB", SqlDbType.DateTime) { Value = dobBox.Text });
            TextBox emailBox = record.FindControl("emailBox") as TextBox;
            cmd.Parameters.Add(new SqlParameter("@ParentEmail", SqlDbType.NVarChar, 50) { Value = emailBox.Text });
            TextBox phoneBox = record.FindControl("phoneBox") as TextBox;
            cmd.Parameters.Add(new SqlParameter("@ParentPhone", SqlDbType.NVarChar, 50) { Value = phoneBox.Text });
            TextBox diffBox = record.FindControl("diffBox") as TextBox;
            cmd.Parameters.Add(new SqlParameter("@LearningDifficulties", SqlDbType.NVarChar, 200) { Value = diffBox.Text });
            TextBox noteBox = record.FindControl("noteBox") as TextBox;
            cmd.Parameters.Add(new SqlParameter("@AdditionalNotes", SqlDbType.NVarChar, 50) { Value = noteBox.Text });
            loggedInUser currentUser = (loggedInUser)Session["loggedInUser"];
            cmd.Parameters.Add(new SqlParameter("@UpdateWho", currentUser.UserId));
            cmd.ExecuteNonQuery();
        }

        protected void gradeStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            showNew.Checked = false;
            classStudent.Items.Clear();
            classStudent.Items.Add(new ListItem("Class", "0"));
            PopulateDropDown("dbo.ClassDropdown", classStudent, "txtClass");
        }

        protected void classStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            showNew.Checked = false;
            GetStudents();
        }

        protected void searchStudents_TextChanged(object sender, EventArgs e)
        {
            showNew.Checked = false;
            GetStudents();
        }

        public void GetStudentClasses(HtmlGenericControl record, string IDX)
        {
            Repeater classes = record.FindControl("classRepeater") as Repeater;
            SqlConnection con = CreateConnection();
            SqlCommand cmd = new SqlCommand("dbo.FetchStudentClasses", con) { CommandType = System.Data.CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IDX", IDX));
            using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
            {
                if (reader.HasRows)
                {
                    classes.DataSource = reader;
                    classes.DataBind();
                }
            }
        }

        protected void studentsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                RepeaterItem item = e.Item;
                var detailDisplay = item.FindControl("detailDisplay") as HtmlGenericControl;
                string displayValue = detailDisplay.Style["display"];
                foreach (RepeaterItem repeaterItem in studentsRepeater.Items)
                {
                    var display = repeaterItem.FindControl("detailDisplay") as HtmlGenericControl;
                    display.Style["display"] = "none";
                }
                if (detailDisplay != null)
                {
                    if (displayValue == "none")
                    {
                        detailDisplay.Style["display"] = "flex";
                        GetStudentDetails(detailDisplay, e.CommandArgument.ToString());
                        GetStudentClasses(detailDisplay, e.CommandArgument.ToString());
                    }
                    else
                    {
                        detailDisplay.Style["display"] = "none";
                    }
                }
                else
                {
                }
            }
            else if (e.CommandName == "Delete")
            {
                validationCheck.Style["display"] = "flex";
                idxStore.Value = e.CommandArgument.ToString();
                popupText.Text = "Are you sure you wish to delete Student ID: " + e.CommandArgument.ToString();
            }
            else if(e.CommandName == "Class")
            {
                editClasses.Style["display"] = "flex";
                idxStore2.Value = e.CommandArgument.ToString();
                SqlConnection con = CreateConnection();
                using (SqlCommand cmd = new SqlCommand("FetchStudentClasses", con) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    cmd.Parameters.Add(new SqlParameter("@IDX", DBNull.Value));
                    using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        classAssignRepeater.DataSource = reader;
                        classAssignRepeater.DataBind();
                    }
                }
                SqlConnection con2 = CreateConnection();
                using (SqlCommand cmd = new SqlCommand("FetchStudentClasses", con2) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    cmd.Parameters.Add(new SqlParameter("@IDX", e.CommandArgument.ToString()));
                    using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            foreach (RepeaterItem repeaterItem in classAssignRepeater.Items)
                            {
                                Label item = repeaterItem.FindControl("itemClass") as Label;
                                if ( item.Text == reader["txtClass"].ToString())
                                {
                                    Button button = repeaterItem.FindControl("addClass") as Button;
                                    button.Visible = true;
                                    Button otherButton = repeaterItem.FindControl("removeClass") as Button;
                                    otherButton.Visible = false;
                                }
                            }
                        }
                    }
                }
            }
            else if (e.CommandName == "Edit")
            {
                RepeaterItem item = e.Item;
                var detailDisplay = item.FindControl("detailDisplay") as HtmlGenericControl;
                EditStudentDetails(detailDisplay, e.CommandArgument.ToString());
                showNew.Checked = false;
                GetStudents();
            }
        }

        protected void cancel_Click(object sender, EventArgs e)
        {
            idxStore.Value = "";
            validationCheck.Style["display"] = "none";
        }

        protected void confirm_Click(object sender, EventArgs e)
        {
            SqlConnection con = CreateConnection();
            SqlCommand cmd = new SqlCommand("dbo.DisableStudent", con) { CommandType = System.Data.CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IDX", idxStore.Value));
            cmd.ExecuteNonQuery();
            idxStore.Value = "";
            validationCheck.Style["display"] = "none";
            studentsRepeater.DataSource = null;
            studentsRepeater.DataBind();
            if (showNew.Checked)
            {
                SqlConnection con2 = CreateConnection();
                using (SqlCommand cmd2 = new SqlCommand("dbo.EmptyStudents", con) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    using (SqlDataReader reader = cmd2.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            studentsRepeater.DataSource = reader;
                            studentsRepeater.DataBind();
                        }
                    }
                }
            }
            else
            {
                GetStudents();
            }
        }

        protected void confirm_Click1(object sender, EventArgs e)
        {
            SqlConnection con = CreateConnection();
            SqlCommand cmd = new SqlCommand("dbo.CreateStudent", con) { CommandType = System.Data.CommandType.StoredProcedure };
        }
        
        protected void addStudent_Click(object sender, EventArgs e)
        {
            SqlConnection con = CreateConnection();
            SqlCommand cmd = new SqlCommand("dbo.CreateStudent", con) { CommandType = System.Data.CommandType.StoredProcedure };
            loggedInUser currentUser = (loggedInUser)Session["loggedInUser"];
            cmd.Parameters.Add(new SqlParameter("@UpdateWho", currentUser.UserId));
            cmd.ExecuteNonQuery();
        }

        protected void cancelChanges_Click(object sender, EventArgs e)
        {
            editClasses.Style["display"] = "none";
        }

        protected void showNew_CheckedChanged(object sender, EventArgs e)
        {
            if (showNew.Checked)
            {
                gradeStudent.SelectedIndex = 0;
                classStudent.SelectedIndex = 0;
                searchStudents.Text = "";
                SqlConnection con = CreateConnection();
                using (SqlCommand cmd = new SqlCommand("dbo.EmptyStudents", con) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    using (SqlDataReader reader = cmd.ExecuteReader()) 
                    {
                        if (reader.HasRows)
                        {
                            studentsRepeater.DataSource = reader;
                            studentsRepeater.DataBind();
                        }
                    }
                }
            }
            else
            {
                gradeStudent.SelectedIndex = 0;
                classStudent.SelectedIndex = 0;
                searchStudents.Text = "";
                GetStudents();
            }
        }

        protected void exitButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("Announcements.aspx");
        }
    }
}