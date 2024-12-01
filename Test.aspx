<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="TanglewoodLMS.Test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="LogIn" runat="server">
    <link rel="stylesheet" type="text/css" href="style.css" />
        <div id="loginFrame" runat="server">
            <asp:Image runat="server" id="logo" src="Resources/Logo Design 1.2.png"></asp:Image>
            <asp:TextBox runat="server" id="usernameInput" class="inputField" placeholder="USERNAME"></asp:TextBox>
            <asp:TextBox runat="server" ID="passwordInput" class="inputField" placeholder="PASSWORD" TextMode="Password"></asp:TextBox>
            <div id="rememberMeContainer" runat="server">
                <asp:CheckBox ID="rememberMeBox" runat="server" AutoPostBack="True" Text="Remember Me" />
            </div>
            <asp:Button ID="loginButton" class="submitButton" runat="server" Text="LOGIN" OnClick="loginButton_Click" />
            <asp:Label ID="errorLabel" class="responseLabel" runat="server"></asp:Label>
        </div>
        <div>&nbsp;</div>
    </form>
</body>
</html>
