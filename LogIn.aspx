<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogIn.aspx.cs" Inherits="TanglewoodLMS.LogIn"%>

<asp:Content ID="LogIn" ContentPlaceHolderID="main" runat="server">
    <link rel="stylesheet" type="text/css" href="style.css" />
    <div id="loginFrame">
        <asp:Image runat="server" id="logo" src="Resources/Logo Design 1.2.png"></asp:Image>
        <asp:TextBox runat="server" id="emailInput" class="inputField"></asp:TextBox>
        <asp:TextBox runat="server" id="passwordInput" class="inputField"></asp:TextBox>
    </div>
</asp:Content>
