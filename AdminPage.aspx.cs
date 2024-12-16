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
            try
            {
                if (!IsPostBack)
                {
                    GetStaff();
                }
            }
            catch
            {

            }
        }

        public SqlConnection CreateConnection()
        {
            string constr = ConfigurationManager.ConnectionStrings["TanglewoodConnectionString"].ConnectionString;
            SqlConnection con = new SqlConnection(constr);
            con.Open();
            return con;
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
            if(nameBox.Text != "")
            {
                if (surnameBox.Text != "")
                {
                    cmd.Parameters.Add(new SqlParameter("@Initials", SqlDbType.NVarChar, 2) { Value = nameBox.Text.Substring(0, 1) + surnameBox.Text.Substring(0, 1) });
                }
                else
                {
                    cmd.Parameters.Add(new SqlParameter("@Initials", SqlDbType.NVarChar, 2) { Value = nameBox.Text.Substring(0, 1)});
                }
            }
            else
            {
                if (surnameBox.Text != "")
                {
                    cmd.Parameters.Add(new SqlParameter("@Initials", SqlDbType.NVarChar, 2) { Value = surnameBox.Text.Substring(0, 1) });
                }
                else
                {
                    cmd.Parameters.Add(new SqlParameter("@Initials", SqlDbType.NVarChar, 2) { Value = DBNull.Value });
                }
            }
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
                        catch (Exception ex)
                        {
                            errorDiv.Style["display"] = "flex";
                            errorLabel.Text = ex.ToString();
                        }
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
            try
            {
                GetStaff();
            }
            catch (Exception ex)
            {
                errorDiv.Style["display"] = "flex";
                errorLabel.Text = ex.ToString();
            }
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

        public bool checkDuplicate(HtmlGenericControl record, string text)
        {
            SqlConnection con = CreateConnection();
            SqlCommand cmd = new SqlCommand("dbo.CheckDuplicate", con) { CommandType = System.Data.CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@original", SqlDbType.NVarChar, 50) { Value = text });
            cmd.Parameters.Add(new SqlParameter("@output", SqlDbType.Bit) { Direction = ParameterDirection.Output });
            cmd.ExecuteNonQuery();
            bool output = (bool)cmd.Parameters["@output"].Value;
            return output;
        }

        protected void studentsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
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
                            checkEmpty(detailDisplay, e.CommandArgument.ToString());
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
                    TextBox textbox = (TextBox)detailDisplay.FindControl("usernameText");
                    if (textbox.Text != null)
                    {
                        if (!checkDuplicate(detailDisplay, textbox.Text))
                        {
                            EditStaffDetails(detailDisplay, e.CommandArgument.ToString());
                            GetStaff();
                        }
                        else
                        {
                            errorDiv.Style["display"] = "flex";
                            errorLabel.Text = "A Staff record with the input Username already exists.\nPlease choose a new username.";
                        }
                    }
                    else
                    {
                        errorDiv.Style["display"] = "flex";
                        errorLabel.Text = "No value in username.";
                    }
                }
            }
            catch(Exception ex)
            {
                errorDiv.Style["display"] = "flex";
                errorLabel.Text = ex.ToString();
            }
        }

        protected void cancel_Click(object sender, EventArgs e)
        {
            try
            {
                idxStore.Value = "";
                validationCheck.Style["display"] = "none";
            }
            catch (Exception ex)
            {
                errorDiv.Style["display"] = "flex";
                errorLabel.Text = ex.ToString();
            }
        }

        protected void confirm_Click(object sender, EventArgs e)
        {
            try
            {
                SqlConnection con = CreateConnection();
                SqlCommand cmd = new SqlCommand("dbo.DisableUser", con) { CommandType = System.Data.CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@UserIDX", idxStore.Value));
                cmd.ExecuteNonQuery();
                idxStore.Value = "";
                validationCheck.Style["display"] = "none";
                GetStaff();
            }
            catch (Exception ex)
            {
                errorDiv.Style["display"] = "flex";
                errorLabel.Text = ex.ToString();
            }
        }

        protected void addStudent_Click(object sender, EventArgs e)
        {
            try
            {
                SqlConnection con = CreateConnection();
                SqlCommand cmd = new SqlCommand("dbo.CreateUser", con) { CommandType = System.Data.CommandType.StoredProcedure };
                loggedInUser currentUser = (loggedInUser)Session["loggedInUser"];
                cmd.Parameters.Add(new SqlParameter("@UpdateWho", currentUser.UserId));
                SqlParameter createdParam = cmd.Parameters.Add(new SqlParameter("@Created", SqlDbType.Bit) { Direction = ParameterDirection.Output });
                cmd.ExecuteNonQuery();
                bool created = Convert.ToBoolean(createdParam.Value);
                if (created == false)
                {
                    errorDiv.Style["display"] = "flex";
                    errorLabel.Text = "An empty staff record already exists.\nAssign it a username and password before creating a new one";
                }
                GetStaff();
            }
            catch (Exception ex)
            {
                errorDiv.Style["display"] = "flex";
                errorLabel.Text = ex.ToString();
            }
        }

        public void checkEmpty(HtmlGenericControl record, string IDX)
        {
            SqlConnection con = CreateConnection();
            SqlCommand cmd = new SqlCommand("dbo.EmptyCheck", con) { CommandType = System.Data.CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@IDX", IDX));
            SqlParameter createdParam = cmd.Parameters.Add(new SqlParameter("@output", SqlDbType.Bit) { Direction = ParameterDirection.Output });
            cmd.ExecuteNonQuery();
            bool empty = Convert.ToBoolean(createdParam.Value);
            if (empty)
            {
                Button deleteButton = (Button)record.FindControl("deleteButton");
                deleteButton.Visible = false;
            }
            else
            {
                Button deleteButton = (Button)record.FindControl("deleteButton");
                deleteButton.Visible = true;
            }
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

        protected void errorCont_Click(object sender, EventArgs e)
        {
            try
            {
                errorDiv.Style["display"] = "none";
            }
            catch
            {
            }
        }

        protected void nameBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TextBox textbox = (TextBox)sender;
                if (textbox.Text.Length > 50)
                {
                    errorLabel.Text = "Name field entry too long.\nPlease ensure a maximum length of 50 characters.";
                    errorDiv.Style["display"] = "none";
                    textbox.Text = "";
                }
            }
            catch (Exception ex)
            {
                errorDiv.Style["display"] = "flex";
                errorLabel.Text = ex.ToString();
            }
        }

        protected void surnameBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TextBox textbox = (TextBox)sender;
                if (textbox.Text.Length > 50)
                {
                    errorLabel.Text = "Surname field entry too long.\nPlease ensure a maximum length of 50 characters.";
                    errorDiv.Style["display"] = "none";
                    textbox.Text = "";
                }
            }
            catch (Exception ex)
            {
                errorDiv.Style["display"] = "flex";
                errorLabel.Text = ex.ToString();
            }
        }

        protected void usernameText_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TextBox textbox = (TextBox)sender;
                if (textbox.Text.Length > 50)
                {
                    errorLabel.Text = "Username field entry too long.\nPlease ensure a maximum length of 50 characters.";
                    errorDiv.Style["display"] = "none";
                    textbox.Text = "";
                }
            }
            catch(Exception ex)
            {
                errorDiv.Style["display"] = "flex";
                errorLabel.Text = ex.ToString();
            }
        }

        protected void passwordText_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TextBox textbox = (TextBox)sender;
                if (textbox.Text.Length > 50)
                {
                    errorLabel.Text = "Password field entry too long.\nPlease ensure a maximum length of 50 characters.";
                    errorDiv.Style["display"] = "none";
                    textbox.Text = "";
                }
            }
            catch (Exception ex)
            {
                errorDiv.Style["display"] = "flex";
                errorLabel.Text = ex.ToString();
            }
        }
    }
}