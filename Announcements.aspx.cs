using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TanglewoodLMS
{
    public partial class Announcements : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    Date.Text = (DateTime.Now).ToString().Substring(0, Math.Min(10, (DateTime.Now).ToString().Length));
                    loggedInUser currentUser = (loggedInUser)Session["loggedInUser"];
                    nameButton.Text = currentUser.Name + "!";
                    SqlConnection con = CreateConnection();
                    SqlDataReader reader = GetAnnouncements();
                    if (reader != null)
                    {
                        repeaterAnnouncements.DataSource = reader;
                        repeaterAnnouncements.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        public SqlDataReader GetAnnouncements()
        {
            try
            {
                SqlConnection con = CreateConnection();
                SqlCommand cmd = new SqlCommand("dbo.DisplayAnnouncements", con) { CommandType = System.Data.CommandType.StoredProcedure };
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (reader.HasRows)
                {
                    return reader;
                }
                else
                {
                    con.Close();
                    return null;
                }
            }
            catch
            {
                return null;
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

        protected void sendAnnouncement_Click(object sender, EventArgs e)
        {
            try
            {
                loggedInUser currentUser = (loggedInUser)Session["loggedInUser"];
                SqlConnection con = CreateConnection();
                using (SqlCommand cmd = new SqlCommand("dbo.CreateAnnouncement", con) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    cmd.Parameters.Add(new SqlParameter("@userID", currentUser.UserId));
                    cmd.Parameters.Add(new SqlParameter("@Message", SqlDbType.NVarChar, 200) { Value = inputAnnouncement.Text });
                    cmd.ExecuteNonQuery();
                    SqlDataReader reader = GetAnnouncements();
                    if (reader != null)
                    {
                        repeaterAnnouncements.DataSource = reader;
                        repeaterAnnouncements.DataBind();
                    }
                    con.Close();
                    inputAnnouncement.Text = "";
                }
            }
            catch
            {
                /* Error Handling */
            }
        }

        protected void logOut_Click(object sender, EventArgs e)
        {
            HttpCookie rememberMeCookie = Request.Cookies["RememberMe"];
            if (rememberMeCookie != null)
            {
                rememberMeCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(rememberMeCookie);
            }
            Session.Remove("loggedInUser");
            Response.Redirect("Test.aspx");
        }

        protected void nameButton_Click(object sender, EventArgs e)
        {
            loggedInUser currentUser = (loggedInUser)Session["loggedInUser"];
            if (currentUser.Admin == true)
            {
                Response.Redirect("AdminPage.aspx");
            }
        }

        protected void allStudents_Click(object sender, EventArgs e)
        {
            Response.Redirect("AllStudents.aspx");
        }

        protected void markbooks_Click(object sender, EventArgs e)
        {
            Response.Redirect("Markbook.aspx");
        }

        protected void reports_Click(object sender, EventArgs e)
        {
            Response.Redirect("Report.aspx");
        }
    }
}