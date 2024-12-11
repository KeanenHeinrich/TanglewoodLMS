using iText.IO.Image;
using iText.IO.Source;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Std = System.Web.UI.WebControls;

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
            studentList.Items.Add(new Std.ListItem("Student", "0"));
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

        public void PopulateDropDown(string procName, Std.DropDownList dropdown, string field)
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
                                    dropdown.Items.Add(new Std.ListItem(reader[field].ToString(), reader["IDX"].ToString()));
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
                                    dropdown.Items.Add(new Std.ListItem((reader["IDX"].ToString() + " - " + reader["txtName"].ToString()) + " " + reader["txtSurname"].ToString() , reader["IDX"].ToString()));
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

        public void ClearMarkbook(Std.GridView markbook)
        {
            markbook.Controls.Clear();
        }

        public string GetPercentage(string mark, string totalMark)
        {
            int markInt = Convert.ToInt32(mark);
            int totalMarkInt = Convert.ToInt32(totalMark);
            int percentInt = ((markInt*100) / (totalMarkInt));
            string percent = percentInt.ToString();
            return percent;
        }

        public string GetFinalPercentage(string mark1, string totalMark1, string mark2, string totalMark2, string mark3, string totalMark3)
        {
            int markInt1 = Convert.ToInt32(mark1);
            int markInt2 = Convert.ToInt32(mark2);
            int markInt3 = Convert.ToInt32(mark3);
            int totalMarkInt1 = Convert.ToInt32(totalMark1);
            int totalMarkInt2 = Convert.ToInt32(totalMark2);
            int totalMarkInt3 = Convert.ToInt32(totalMark3);
            int percentInt = (((markInt1 * 100) / (totalMarkInt1)) + ((markInt2 * 100) / (totalMarkInt2)) + ((markInt3 * 100) / (totalMarkInt3)))/3;
            string percent = percentInt.ToString();
            return percent;
        }

        public string GetYearMark(string term1, string term2, string term3, string term4)
        {
            int term1Int;
            int term2Int;
            int term3Int;
            int term4Int;
            term1Int = Convert.ToInt32(term1);
            term2Int = Convert.ToInt32(term2);
            term3Int = Convert.ToInt32(term3);
            term4Int = Convert.ToInt32(term4);
            int yearInt = (term1Int + term2Int + term3Int + term4Int) / 4;
            string year = (yearInt.ToString() + "%");
            return (year);
        }

        public string GetFinalMark(Array results)
        {
            int percentageTotal = 0;
            foreach (string result in results)
            {
                percentageTotal += Convert.ToInt32(result.Substring(0, (result.Length-1)));
            }
            percentageTotal = percentageTotal / results.Length;
            return percentageTotal.ToString()+"%";
        }

        public void PopulateMarkbook()
        {
            int count = 0;
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
                    SqlConnection con = CreateConnection();
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
                                row["Assessment 1"] = GetPercentage(reader["intMark1"].ToString(), reader["intTotalMark1"].ToString()) + "%";
                                row["Assessment 2"] = GetPercentage(reader["intMark2"].ToString(), reader["intTotalMark2"].ToString()) + "%";
                                row["Assessment 3"] = GetPercentage(reader["intMark3"].ToString(), reader["intTotalMark3"].ToString()) + "%";
                                row["Final Percentage"] = GetFinalPercentage(
                                    reader["intMark1"].ToString(),
                                    reader["intTotalMark1"].ToString(),
                                    reader["intMark2"].ToString(),
                                    reader["intTotalMark2"].ToString(),
                                    reader["intMark3"].ToString(),
                                    reader["intTotalMark3"].ToString()
                                ) + "%";
                                dataTable.Rows.Add(row);
                            }
                        }
                    }
                    reportView.DataSource = dataTable;
                    reportView.DataBind();
                    SqlConnection con2 = CreateConnection();
                    using (SqlCommand cmd2 = new SqlCommand("dbo.fetchRemark", con2) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd2.Parameters.Add(new SqlParameter("@Year", yearList.SelectedValue));
                        cmd2.Parameters.Add(new SqlParameter("@idxStudent", studentList.SelectedValue));
                        using (SqlDataReader reader = cmd2.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                commentBox.Text = reader["txtRemarks"].ToString();
                                errorLabel.Text = reader["txtRemarks"].ToString();
                            }
                        }
                    }
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
                            
                            string term1 = "0";
                            string term2 = "0";
                            string term3 = "0";
                            string term4 = "0";
                            while (reader.Read())
                            {
                                DataRow row = dataTable.NewRow();
                                row = dataTable.NewRow();
                                row["Subject"] = reader["txtSubject"].ToString();
                                term1 = GetFinalPercentage(
                                    reader["intMark1"].ToString(),
                                    reader["intTotalMark1"].ToString(),
                                    reader["intMark2"].ToString(),
                                    reader["intTotalMark2"].ToString(),
                                    reader["intMark3"].ToString(),
                                    reader["intTotalMark3"].ToString()
                                );
                                row["Term 1"] = term1 + "%";
                                reader.Read();
                                term2 = GetFinalPercentage(
                                    reader["intMark1"].ToString(),
                                    reader["intTotalMark1"].ToString(),
                                    reader["intMark2"].ToString(),
                                    reader["intTotalMark2"].ToString(),
                                    reader["intMark3"].ToString(),
                                    reader["intTotalMark3"].ToString()
                                );
                                row["Term 2"] = term2 + "%";
                                reader.Read();
                                term3=  GetFinalPercentage(
                                    reader["intMark1"].ToString(),
                                    reader["intTotalMark1"].ToString(),
                                    reader["intMark2"].ToString(),
                                    reader["intTotalMark2"].ToString(),
                                    reader["intMark3"].ToString(),
                                    reader["intTotalMark3"].ToString()
                                );
                                row["Term 3"] = term3 + "%";
                                reader.Read();
                                term4 =  GetFinalPercentage(
                                    reader["intMark1"].ToString(),
                                    reader["intTotalMark1"].ToString(),
                                    reader["intMark2"].ToString(),
                                    reader["intTotalMark2"].ToString(),
                                    reader["intMark3"].ToString(),
                                    reader["intTotalMark3"].ToString()
                                );
                                row["Term 4"] = term4 + "%";
                                row["Year Mark"] = GetYearMark(
                                    term1,
                                    term2,
                                    term3,
                                    term4
                                );
                                dataTable.Rows.Add(row);
                                count++;
                            }
                        }
                    }
                    finalReportView.DataSource = dataTable;
                    finalReportView.DataBind();
                    SqlConnection con2 = CreateConnection();
                    using (SqlCommand cmd2 = new SqlCommand("dbo.fetchRemark", con2) { CommandType = CommandType.StoredProcedure })
                    {
                        cmd2.Parameters.Add(new SqlParameter("@Year", yearList.SelectedValue));
                        cmd2.Parameters.Add(new SqlParameter("@idxStudent", studentList.SelectedValue));
                        using (SqlDataReader reader = cmd2.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                commentBox.Text = reader["txtRemarks"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorLabel.Text = count + ex.ToString();
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
                yearList.Items.Add(new Std.ListItem(currentYear.ToString(), currentYear.ToString()));
            }
            
        }

        protected void downloadButton_Click(object sender, EventArgs e)
        {
            try
            {
                GridView selectedGridView = termList.SelectedValue == "5" ? finalReportView : reportView;

                if (selectedGridView.Rows.Count == 0)
                {
                    errorLabel.Text = "No data available to generate the report.";
                    return;
                }
                string fileName = "ReportCard.pdf";
                int index = studentList.SelectedItem.Text.IndexOf("-")+1;

                using (MemoryStream stream = new MemoryStream())
                {
                    PdfWriter writer = new PdfWriter(stream);
                    PdfDocument pdf = new PdfDocument(writer);
                    Document document = new Document(pdf);
                    var imageData = new iText.Layout.Element.Image(ImageDataFactory.Create("C:\\Users\\keane\\source\\repos\\TanglewoodLMS\\Resources\\Logo Design 1.2.png")).SetWidth(200).SetHeight(200).SetMarginTop(-30).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                    document.Add(imageData);
                    Paragraph title = new Paragraph()
                        .Add("Report Card")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(30)
                        .SetMarginTop(-20)
                        .SetCharacterSpacing(7)
                        .SetFontColor(ColorConstants.BLACK);
                    document.Add(title);
                    // Create a table with 2 columns
                    iText.Layout.Element.Table details = new iText.Layout.Element.Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth().SetMarginTop(20).SetTextAlignment(TextAlignment.CENTER).SetFontSize(16);
                    details.AddCell((new Cell().Add(new Paragraph($"Student: {studentList.SelectedItem.Text.Substring(index + 1)}"))).SetBorder(border:iText.Layout.Borders.Border.NO_BORDER));
                    details.AddCell(new Cell().Add(new Paragraph($"Grade: {gradeList.SelectedItem.Text}")).SetBorder(border: iText.Layout.Borders.Border.NO_BORDER)).SetFontSize(16);
                    details.AddCell(new Cell().Add(new Paragraph($"Year: {yearList.SelectedItem.Text}")).SetBorder(border: iText.Layout.Borders.Border.NO_BORDER)).SetFontSize(16);
                    if (termList.SelectedValue != "5")
                    {
                        details.AddCell(new Cell().Add(new Paragraph($"Term: {termList.SelectedValue}")).SetBorder(border: iText.Layout.Borders.Border.NO_BORDER)).SetFontSize(16);
                    }
                    document.Add(details);
                    int columnCount = selectedGridView.HeaderRow.Cells.Count;
                    iText.Layout.Element.Table table = new iText.Layout.Element.Table(columnCount, true).SetMarginTop(20);
                    foreach (TableCell headerCell in selectedGridView.HeaderRow.Cells)
                    {
                        table.AddHeaderCell(new Paragraph(headerCell.Text));
                    }
                    foreach (GridViewRow gridViewRow in selectedGridView.Rows)
                    {
                        foreach (TableCell gridViewCell in gridViewRow.Cells)
                        {
                            table.AddCell(new Paragraph(gridViewCell.Text)).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 1)).SetTextAlignment(TextAlignment.CENTER).SetFontSize(16);
                        }
                    }
                    document.Add(table);
                    iText.Layout.Element.Table finalMarks = new iText.Layout.Element.Table(UnitValue.CreatePercentArray(6)).UseAllAvailableWidth();
                    finalMarks.AddCell(new Cell().Add(new Paragraph("Final Mark:")));
                    for (int i = 0; i < columnCount; i++)
                    {
                        int counter = 0;
                        string[] values = new string[selectedGridView.Rows.Count];
                        if (i != 0)
                        {
                            foreach (TableRow row in selectedGridView.Rows)
                            {
                                values[counter] = row.Cells[i].Text;
                                counter++;
                            }
                            finalMarks.AddCell(GetFinalMark(values)).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 1)).SetTextAlignment(TextAlignment.CENTER).SetFontSize(16);
                        }
                    }
                    if (selectedGridView == finalReportView)
                    {
                        document.Add(finalMarks);
                    }
                    document.Add(new Paragraph($"Remarks:").SetFontSize(16).SetMarginTop(20));
                    document.Add(new Paragraph($"{commentBox.Text}").SetMarginTop(0));
                    Paragraph footer = new Paragraph("Generated on: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                        .SetTextAlignment(TextAlignment.RIGHT)
                        .SetFontSize(10)
                        .SetFontColor(ColorConstants.GRAY);
                    document.Add(footer);
                    document.Close();
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", $"attachment; filename={fileName}");
                    Response.BinaryWrite(stream.ToArray());
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                errorLabel.Text = "An error occurred while generating the PDF: " + ex.Message;
            }
        }

        protected void commentSubmit_Click(object sender, EventArgs e)
        {
            loggedInUser currentUser = (loggedInUser)Session["loggedInUser"];
            SqlConnection con = CreateConnection();
            using (SqlCommand cmd = new SqlCommand("dbo.UpdateRemark", con) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.Add(new SqlParameter("@Year", yearList.SelectedValue));
                cmd.Parameters.Add(new SqlParameter("@idxStudent", studentList.SelectedValue));
                cmd.Parameters.Add(new SqlParameter("@Remark", SqlDbType.NVarChar, 500) { Value = commentBox.Text });
                cmd.Parameters.Add(new SqlParameter("@User", currentUser.UserId.ToString()));
                cmd.ExecuteNonQuery();
            }
        }
    }
}