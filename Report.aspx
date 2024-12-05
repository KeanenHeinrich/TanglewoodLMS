<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="TanglewoodLMS.Report" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="Report" runat="server">
        <link rel="stylesheet" type="text/css" href="style.css" />
        <div id="filters">
            <div id="heading">
                <asp:Label ID="titleCard" runat="server" Text="REPORT"></asp:Label>
            </div>
            <asp:Label ID="guide" runat="server" Text="Filter reports below"></asp:Label>
            <asp:DropDownList ID="yearList" class="dropList" runat="server" OnSelectedIndexChanged="yearList_SelectedIndexChanged" AutoPostBack="True">
                <asp:ListItem Text="Year" Value="" Selected="True"></asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="gradeList" class="dropList" runat="server" OnSelectedIndexChanged="gradeList_SelectedIndexChanged" AutoPostBack="True">
                <asp:ListItem Text="Grade" Value="" Selected="True"></asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="termList" class="dropList" runat="server" OnSelectedIndexChanged="termList_SelectedIndexChanged" AutoPostBack="True">
                <asp:ListItem Text="Term" Value="" Selected="True"></asp:ListItem>
                <asp:ListItem Text="Term 1" Value="1"></asp:ListItem>
                <asp:ListItem Text="Term 2" Value="2"></asp:ListItem>
                <asp:ListItem Text="Term 3" Value="3"></asp:ListItem>
                <asp:ListItem Text="Term 4" Value="4"></asp:ListItem>
                <asp:ListItem Text="Final" Value="5"></asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="studentList" class="dropList" runat="server" OnSelectedIndexChanged="studentList_SelectedIndexChanged" AutoPostBack="True">
                <asp:ListItem Text="Student" Value="" Selected="True"></asp:ListItem>
            </asp:DropDownList>
            <asp:Button ID="exitButton" class="navMinor" Text="Exit" runat="server" OnClick="exitButton_Click" />
        </div>
        <div id="marks">
            <asp:Label ID="markHeader" runat="server" Text="No Markbook Selected"></asp:Label>
            <div id="results">        
                <asp:GridView ID="reportView" runat="server" AutoGenerateColumns="false" EnableTheming="false" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="Subject" HeaderText="Subject" />
                        <asp:BoundField DataField="Assessment 1" HeaderText="Assessment 1" />
                        <asp:BoundField DataField="Assessment 2" HeaderText="Assessment 2" />
                        <asp:BoundField DataField="Assessment 3" HeaderText="Assessment 3" />
                        <asp:BoundField DataField="Final Percentage" HeaderText="Final Total" />
                    </Columns>
                </asp:GridView>
                <asp:GridView ID="finalReportView" runat="server" AutoGenerateColumns="false" EnableTheming="false" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="Subject" HeaderText="Subject" />
                        <asp:BoundField DataField="Term 1" HeaderText="Term 1" />
                        <asp:BoundField DataField="Term 2" HeaderText="Term 2" />
                        <asp:BoundField DataField="Term 3" HeaderText="Term 3" />
                        <asp:BoundField DataField="Term 4" HeaderText="Term 4" />
                        <asp:BoundField DataField="Year Mark" HeaderText="Year Mark" />
                    </Columns>
                </asp:GridView>
            </div>
            <div id="markButtons" runat="server">
                
                <asp:Button ID="downloadButton" class="markButton" Text="DOWNLOAD" runat="server" OnClick="downloadButton_Click" />
            </div>
            <asp:Label ID="errorLabel" runat="server"></asp:Label>
        </div>
    </form>
</body>
</html>
