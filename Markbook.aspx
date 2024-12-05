<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Markbook.aspx.cs" Inherits="TanglewoodLMS.Markbook" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="Markbook" runat="server">
        <link rel="stylesheet" type="text/css" href="style.css" />
        <div id="filters">
            <div id="heading">
                <asp:Label ID="titleCard" runat="server" Text="MARKBOOKS"></asp:Label>
            </div>
            <asp:Label ID="guide" runat="server" Text="Filter markbook below"></asp:Label>
            <asp:DropDownList ID="gradeList" class="dropList" runat="server" OnSelectedIndexChanged="gradeList_SelectedIndexChanged" AutoPostBack="True">
                <asp:ListItem Text="Grade" Value="" Selected="True"></asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="subjectList" class="dropList" runat="server" OnSelectedIndexChanged="subjectList_SelectedIndexChanged" AutoPostBack="True">
                <asp:ListItem Text="Subject" Value="" Selected="True"></asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="termList" class="dropList" runat="server" OnSelectedIndexChanged="termList_SelectedIndexChanged" AutoPostBack="True">
                <asp:ListItem Text="Term" Value="" Selected="True"></asp:ListItem>
                <asp:ListItem Text="Term 1" Value="1"></asp:ListItem>
                <asp:ListItem Text="Term 2" Value="2"></asp:ListItem>
                <asp:ListItem Text="Term 3" Value="3"></asp:ListItem>
                <asp:ListItem Text="Term 4" Value="4"></asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="classList" class="dropList" runat="server" OnSelectedIndexChanged="classList_SelectedIndexChanged" AutoPostBack="True">
                <asp:ListItem Text="Class" Value="" Selected="True"></asp:ListItem>
            </asp:DropDownList>
            <asp:Button ID="exitButton" class="navMinor" Text="Exit" runat="server" OnClick="exitButton_Click" />
        </div>
        <div id="marks">
            <asp:Label ID="markHeader" runat="server" Text="No Markbook Selected"></asp:Label>
            <div id="results">        
                <asp:GridView ID="markbookView" runat="server" AutoGenerateColumns="false" EnableTheming="false" GridLines="None" OnRowDataBound="markbookView_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="Name">
                            <ItemTemplate>
                                <asp:Label ID="txtName" runat="server" Text='<%# Eval("txtName") + " " + Eval("txtSurname") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="IDX">
                            <ItemTemplate>
                                <asp:Label ID="idx" runat="server" Text='<%# Eval("IDX") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total1">
                            <ItemTemplate>
                                <asp:TextBox ID="intMark1" class="tableEdit" runat="server" Text='<%# Eval("intMark1") %>'></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total2">
                            <ItemTemplate>
                                <asp:TextBox ID="intmark2" class="tableEdit" runat="server" Text='<%# Eval("intMark2") %>'></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total3">
                            <ItemTemplate>
                                <asp:TextBox ID="intmark3" class="tableEdit" runat="server" Text='<%# Eval("intMark3") %>'></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Final Total">
                            <ItemTemplate>
                                <asp:Label ID="finalMark" runat="server"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
            <div id="markButtons" runat="server">
                <asp:Button ID="editButton" class="markButton" Text="EDIT" runat="server" OnClick="editButton_Click" />
                <asp:Button ID="cancelButton" class="markButton" Text="CANCEL" runat="server" OnClick="cancelButton_Click" />
            </div>
            <asp:Label ID="errorLabel" runat="server"></asp:Label>
        </div>
    </form>
</body>
</html>
