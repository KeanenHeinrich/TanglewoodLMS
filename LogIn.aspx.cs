using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Security.Cryptography;
using System.Web.UI.WebControls;
using System.Text;
using static System.Console;
using System.Data;
using System.Drawing;

namespace TanglewoodLMS
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                SqlConnection con = CreateConnection();
                if (Request.Cookies["RememberMe"] != null)
                {
                    HttpCookie rememberMeCookie = Request.Cookies["RememberMe"];
                    if (!string.IsNullOrEmpty(rememberMeCookie.Value))
                    {
                        int userID = ValidateRememberMeToken(rememberMeCookie);
                        if (userID != 0)
                        {
                            loggedInUser details = GetUserDetails(userID);
                            Session["loggedInUser"] = new loggedInUser(userID, details.Name, details.Surname, details.Admin);
                            loggedInUser currentUser = (loggedInUser)Session["loggedInUser"];
                            con.Close();
                            Response.Redirect("Announcements.aspx");
                        }
                        else
                        {
                            rememberMeCookie.Expires = DateTime.Now.AddDays(-1);
                            Response.Cookies.Add(rememberMeCookie);

                        }
                    }
                    else
                    {
                        rememberMeCookie.Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies.Add(rememberMeCookie);
                    }
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                errorLabel.Text = (ex.ToString());
            }
        }

        protected void loginButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = CreateConnection())
                {
                    using (SqlCommand saltcmd = new SqlCommand("dbo.SaltPassword", con) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        try
                        {
                            saltcmd.Parameters.Add(new SqlParameter("@InputPassword", SqlDbType.NVarChar, 50) { Value = passwordInput.Text });
                            saltcmd.Parameters.Add(new SqlParameter("@OutputPassword", SqlDbType.NVarChar, 50) { Direction = ParameterDirection.Output });
                            saltcmd.ExecuteNonQuery();
                            string saltedPassword = (string)saltcmd.Parameters["@OutputPassword"].Value;
                            using (SqlCommand cmd = new SqlCommand("dbo.LoginValidation", con) { CommandType = System.Data.CommandType.StoredProcedure })
                            {
                                try
                                {
                                    cmd.Parameters.Add(new SqlParameter("@Username", usernameInput.Text));
                                    cmd.Parameters.Add(new SqlParameter("@InputPassword", hashPassword(saltedPassword)));
                                    cmd.Parameters.Add(new SqlParameter("@returnValue", SqlDbType.Bit) { Direction = ParameterDirection.Output });
                                    cmd.Parameters.Add(new SqlParameter("@userID", SqlDbType.Int) { Direction = ParameterDirection.Output });
                                    cmd.ExecuteNonQuery();
                                    bool isValidUser = (bool)cmd.Parameters["@returnValue"].Value;
                                    errorLabel.Text = Convert.ToString(isValidUser);
                                    if (isValidUser)
                                    {
                                        int userID = (int)cmd.Parameters["@userID"].Value;
                                        loggedInUser details = GetUserDetails(userID);
                                        if (details != null)
                                        {
                                            Session["loggedInUser"] = new loggedInUser(userID, details.Name, details.Surname, details.Admin);
                                            loggedInUser currentUser = (loggedInUser)Session["loggedInUser"];
                                            if (rememberMeBox.Checked)
                                            {
                                                StoreRememberMeToken(currentUser.UserId);
                                            }
                                            con.Close();
                                            Response.Redirect("Announcements.aspx");
                                        }
                                        else
                                        {
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    errorLabel.Text = (ex.ToString());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            errorLabel.Text = (ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorLabel.Text = (ex.ToString());
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

        public string GenerateRandomToken()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[32];
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public void SetRememberMeCookie(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                HttpCookie rememberMeCookie = new HttpCookie("RememberMe")
                {
                    Value = token,
                    Expires = DateTime.Now.AddDays(30),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };
                Response.Cookies.Add(rememberMeCookie);
            }
        }

        public int ValidateRememberMeToken(HttpCookie rememberMeCookie)
        {
            try
            {
                SqlConnection con = CreateConnection();
                using (SqlCommand cmd = new SqlCommand("dbo.CheckToken", con) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    try
                    {
                        cmd.Parameters.Add(new SqlParameter("@InputToken", SqlDbType.NVarChar, 256) { Value = rememberMeCookie.Value });
                        cmd.Parameters.Add(new SqlParameter("@user", SqlDbType.Int) { Direction = ParameterDirection.Output });
                        cmd.ExecuteNonQuery();
                        int userID = (int)cmd.Parameters["@user"].Value;
                        con.Close();
                        return userID;
                    }
                    catch (Exception ex)
                    {
                        con.Close();
                        errorLabel.Text = (ex.ToString());
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                errorLabel.Text = (ex.ToString());
                return 0;
            }
        }   

        public loggedInUser GetUserDetails(int userID)
        {
            try
            {
                SqlConnection con = CreateConnection();
                if (con != null)
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.FetchStaffInfo", con) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        try
                        {
                            cmd.Parameters.Add(new SqlParameter("@IDX", userID));
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                reader.Read();
                                return new loggedInUser(
                                userID,
                                reader["txtName"].ToString(),
                                reader["txtSurname"].ToString(),
                                Convert.ToBoolean(reader["bitAdmin"]));
                            }
                        }
                        catch (Exception ex)
                        {
                            con.Close();
                            errorLabel.Text = (ex.ToString());
                            return null;
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                errorLabel.Text = (ex.ToString());
                return null;
            }
        }

        public void StoreRememberMeToken(int userId)
        {
            try
            {
                SqlConnection con = CreateConnection();
                string token = GenerateRandomToken();
                SetRememberMeCookie(token);
                using (SqlCommand cmd = new SqlCommand("dbo.AddToken", con) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    try
                    {
                        cmd.Parameters.Add(new SqlParameter("@User", userId));
                        cmd.Parameters.Add(new SqlParameter("@Token", token));
                        cmd.Parameters.Add(new SqlParameter("@Expires", DateTime.Now.AddDays(30)));
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    catch (Exception ex)
                    {
                        con.Close();
                        errorLabel.Text = (ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                errorLabel.Text = (ex.ToString());
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
            catch
            {
                return null;
            }
        }
    }
}