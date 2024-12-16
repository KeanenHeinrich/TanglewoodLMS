<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Announcements.aspx.cs" Inherits="TanglewoodLMS.Announcements" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="Announcements" runat="server">
    <link rel="stylesheet" type="text/css" href="style.css" />
        <div id="posts">
            <div id="heading">
                <asp:Label ID="titleCard" runat="server" Text="ANNOUNCEMENTS"></asp:Label>
                <asp:Label ID="Date" runat="server" Text="DATE"></asp:Label>
            </div>
            <div id="listAnnouncements">
                <asp:Repeater ID="repeaterAnnouncements" runat="server">
                    <ItemTemplate>
                        <div class="post">
                            <div class="profile">
                                <asp:Label ID="profileInitials" runat="server"><%# Eval("txtInitials") %></asp:Label>
                            </div>
                            <div id="bodyPost">
                                <div id="fullName">
                                    <asp:Label ID="userName" runat="server"><%# Eval("txtName") %></asp:Label>
                                    <asp:Label ID="userSurname" runat="server"><%# Eval("txtSurname") %></asp:Label>
                                </div>
                                <asp:Label ID="messageAnn" Font-Size="2em" runat="server"><%# Eval("txtAnnouncement") %></asp:Label>
                                <br />
                                <asp:Label ID="timeAnn" runat="server"><%# Eval("datCreationDate") %></asp:Label>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <div id="submitAnnouncement">
                <asp:Button ID="sendAnnouncement" runat="server" Text="SEND" OnClick="sendAnnouncement_Click" />
                <asp:TextBox ID="inputAnnouncement" runat="server" TextMode="MultiLine"></asp:TextBox>
            </div>
        </div>
        <div id="navigation">
            <div id="welcome">
                <asp:Label ID="intro" runat="server" Text="Welcome, "></asp:Label>
                <asp:Button ID="nameButton" runat="server" Text="Name" OnClick="nameButton_Click" />
            </div>
            <asp:Button ID="allStudents" class="navButton" runat="server" Text="ALL STUDENTS" OnClick="allStudents_Click" />
            <asp:Button ID="markbooks" class="navButton" runat="server" Text="MARKBOOKS" OnClick="markbooks_Click" />
            <asp:Button ID="reports" class="navButton" runat="server" Text="REPORTS" OnClick="reports_Click" />
            <asp:Button ID="logOut" class="navMinor" runat="server" Text="EXIT" BorderStyle="None" OnClick="logOut_Click" />
        </div>
        <script>
            function getVpWidth() {
                const viewportWidth = window.innerWidth;
                viewportWidth = viewportWidth / 1920;
                document.documentElement.style.setProperty('--adjust', `${viewportWidth}px`);
            }
            geyVpWidth();
            window.addEventListener('resize', setViewportWidthVariable);
        </script>
    </form>
</body>
</html>
