using Microsoft.Ajax.Utilities;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace TanglewoodLMS
{
    public partial class AdminPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetStaff();
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

        public void GetStaff()
        {
            SqlConnection con = CreateConnection();
            using (SqlCommand cmd = new SqlCommand("dbo.FetchStaff", con) { CommandType = System.Data.CommandType.StoredProcedure })
            {
                if (searchStaff.Text != "")
                {
                    cmd.Parameters.Add(new SqlParameter("@SearchInput", searchStaff.Text));
                }
                else
                {
                    cmd.Parameters.Add(new SqlParameter("@SearchInput", DBNull.Value));
                }
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (reader.HasRows)
                {
                    studentsRepeater.DataSource = reader;
                    studentsRepeater.DataBind();
                }
                con.Close();
            }
        }

        public void GetStaffDetails(HtmlGenericControl record, string IDX)
        {
            SqlConnection con = CreateConnection();
            SqlCommand cmd = new SqlCommand("dbo.FetchStaffInfo", con) { CommandType = System.Data.CommandType.StoredProcedure };
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
                    DropDownList subjectList = record.FindControl("subjectList") as DropDownList;
                    if (reader["idxSubject"] != DBNull.Value)
                    {
                        subjectList.SelectedValue = reader["idxSubject"].ToString();
                    }
                    CheckBox admin = record.FindControl("adminCheck") as CheckBox;
                    admin.Checked = Convert.ToBoolean(reader["bitAdmin"].ToString());
                }
            }
        }

        public void EditStaffDetails(HtmlGenericControl record, string IDX)
        {
            SqlConnection con = CreateConnection();
            SqlCommand cmd = new SqlCommand("dbo.UpdateUser", con) { CommandType = System.Data.CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IDX", IDX));
            TextBox nameBox = record.FindControl("nameBox") as TextBox;
            cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 50) { Value = nameBox.Text });
            TextBox surnameBox = record.FindControl("surnameBox") as TextBox;
            cmd.Parameters.Add(new SqlParameter("@Surname", SqlDbType.NVarChar, 50) { Value = surnameBox.Text });
            DropDownList subjectList = record.FindControl("subjectList") as DropDownList;
            cmd.Parameters.Add(new SqlParameter("@Subject", subjectList.SelectedValue ));
            CheckBox admin = record.FindControl("adminCheck") as CheckBox;
            cmd.Parameters.Add(new SqlParameter("@Admin", SqlDbType.Bit) { Value = admin.Checked });
            TextBox password = record.FindControl("passwordText") as TextBox;

            if (password.Text != "")
            {
                using (SqlConnection con2 = CreateConnection())
                {
                    using (SqlCommand saltcmd = new SqlCommand("dbo.SaltPassword", con2) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        try
                        {
                            saltcmd.Parameters.Add(new SqlParameter("@InputPassword", SqlDbType.NVarChar, 50) { Value = password.Text });
                            saltcmd.Parameters.Add(new SqlParameter("@OutputPassword", SqlDbType.NVarChar, 50) { Direction = ParameterDirection.Output });
                            saltcmd.ExecuteNonQuery();
                            string saltedPassword = (string)saltcmd.Parameters["@OutputPassword"].Value;
                            cmd.Parameters.Add(new SqlParameter("@Password", hashPassword(saltedPassword)));
                        }
                        catch { }
                    }
                }
            }
            else
            {
                cmd.Parameters.Add(new SqlParameter("@Password", DBNull.Value));
            }
            TextBox username = record.FindControl("usernameText") as TextBox;
            if (username.Text != "")
            {
                cmd.Parameters.Add(new SqlParameter("@Username", username.Text));
            }
            else
            {
                cmd.Parameters.Add(new SqlParameter("@Username", DBNull.Value));
            }
            loggedInUser currentUser = (loggedInUser)Session["loggedInUser"];
            cmd.Parameters.Add(new SqlParameter("@UpdateWho", currentUser.UserId));
            cmd.ExecuteNonQuery();
        }

        protected void searchStudents_TextChanged(object sender, EventArgs e)
        {
            GetStaff();
        }

        public void PopulateSubjects(HtmlGenericControl record)
        {
            DropDownList dropdown = record.FindControl("subjectList") as DropDownList;
            dropdown.Items.Clear();
            dropdown.Items.Add(new ListItem("Choose", "0"));
            SqlConnection con = CreateConnection();
            SqlCommand cmd = new SqlCommand("dbo.SubjectDropdown", con) { CommandType = System.Data.CommandType.StoredProcedure };
            using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
            {
                while (reader.Read())
                {
                    dropdown.Items.Add(new ListItem(reader["txtSubject"].ToString(), reader["IDX"].ToString()));
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
                        PopulateSubjects(detailDisplay);
                        GetStaffDetails(detailDisplay, e.CommandArgument.ToString());
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
                popupText.Text = "Are you sure you wish to delete User ID: " + e.CommandArgument.ToString();
            }
            else if (e.CommandName == "Edit")
            {
                RepeaterItem item = e.Item;
                var detailDisplay = item.FindControl("detailDisplay") as HtmlGenericControl;
                EditStaffDetails(detailDisplay, e.CommandArgument.ToString());
                GetStaff();
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
            SqlCommand cmd = new SqlCommand("dbo.DisableUser", con) { CommandType = System.Data.CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@UserIDX", idxStore.Value));
            cmd.ExecuteNonQuery();
            idxStore.Value = "";
            validationCheck.Style["display"] = "none";
            GetStaff();
        }

        protected void addStudent_Click(object sender, EventArgs e)
        {
            SqlConnection con = CreateConnection();
            SqlCommand cmd = new SqlCommand("dbo.CreateUser", con) { CommandType = System.Data.CommandType.StoredProcedure };
            loggedInUser currentUser = (loggedInUser)Session["loggedInUser"];
            cmd.Parameters.Add(new SqlParameter("@UpdateWho", currentUser.UserId));
            cmd.ExecuteNonQuery();
            GetStaff();
        }

        public string hashPassword(string Password)
        {
            SHA256 hash = SHA256.Create();
            var passwordBytes = Encoding.Default.GetBytes(Password);
            var hashedPassword = hash.ComputeHash(passwordBytes);
            var passString = Convert.ToBase64String(hashedPassword);
            return passString;
        }

        protected void exitButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("Announcements.aspx");
        }
    }
}