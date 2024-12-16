<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminPage.aspx.cs" Inherits="TanglewoodLMS.AdminPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="AdminPage" runat="server">
        <div id="validationCheck" runat="server" style="position: absolute; top: 0; left: 0; height: 100vh; width: 100vw; z-index: 1000; background-color: rgba(0, 0, 0, 0.7); display: none; justify-content: center; align-items: center;">      
            <div id="popup" class="popup">
                <asp:Label ID="popupHeading" class="popupHeading" runat="server" Text="DELETING STAFF MEMBER"></asp:Label>
                <asp:HiddenField ID="idxStore" runat="server" Value=""/>
                <asp:Label ID="popupText" runat="server"></asp:Label>
                <div id="popupButtons">
                    <asp:Button ID="confirm" runat="server" Text="DELETE" Onclick="confirm_Click"/>
                    <asp:Button ID="cancel" runat="server" Text="EXIT" Onclick="cancel_Click"/>
                </div>
            </div>
        </div>
        <div id="errorDiv" runat="server" style="position: absolute; top: 0; left: 0; height: 100vh; width: 100vw; z-index: 1000; background-color: rgba(0, 0, 0, 0.7); display: none; justify-content: center; align-items: center;">      
            <div id="errorPopup" class="popup">
                <asp:Label ID="errorHeading" class="popupHeading" runat="server" Text="INVALID ACTION"></asp:Label>
                <asp:Label ID="errorLabel" runat="server"></asp:Label>
                <asp:Button ID="errorCont" runat="server" Text="CONTINUE" Onclick="errorCont_Click"/>
            </div>
        </div>
        <link rel="stylesheet" type="text/css" href="style.css" />
        <div id="headerAllStudents">
            <asp:Label ID="header" runat="server" Text="ADMIN - STAFF MANAGEMENT"></asp:Label>
        </div>
        <div id="filterAllStaff">
            <asp:TextBox ID="searchStaff" runat="server" Placeholder="Search Staff" OnTextChanged="searchStudents_TextChanged" AutoPostBack="True"></asp:TextBox>
        </div>
        <div id="listAllStudents">
            <asp:Repeater ID="studentsRepeater" runat="server" OnItemCommand="studentsRepeater_ItemCommand">
              <ItemTemplate>
                <div id="postAllStaff" runat="server" style="box-sizing: border-box; display: flex;flex-direction: row;width: 100%; padding: 15px; background-color: whitesmoke; color: #868686; border: solid 1px; border-radius: 10px; justify-content: space-between; font-size: 25px;">
                    <asp:Label ID="studentName" runat="server" Text='<%# Eval("IDX") + " | " + Eval("txtName") + " " + Eval("txtSurname") + " | " + Eval("txtSubject") %>' />
                    <asp:ImageButton ID="expandButton" EnableTheming="false" CssClass="expandButton" ImageUrl="Resources/downArrow.png" runat="server" CommandName="Select" CommandArgument='<%# Eval("IDX") %>' />
                </div>
                  <div id="detailDisplay" runat="server" style="display: none; width: 100%; min-height: 100px;">
                      <div id="detailBackground">
                          <asp:Table ID="displayTable" runat="server" EnableTheming="false">
                              <asp:TableRow>
                                      <asp:TableCell>
                                          <asp:Label ID="nameLabel" runat="server" Text="Name:"></asp:Label>
                                      </asp:TableCell>
                                      <asp:TableCell>
                                          <asp:TextBox ID="nameBox" runat="server" Placeholder="Name" OnTextChanged="nameBox_TextChanged"></asp:TextBox>
                                      </asp:TableCell>
                                      <asp:TableCell>
                                          <asp:Label ID="surnameLabel" runat="server" Text="Surname:"></asp:Label>
                                      </asp:TableCell>
                                      <asp:TableCell>
                                          <asp:TextBox ID="surnameBox" runat="server" Placeholder="Surname" OnTextChanged="surnameBox_TextChanged"></asp:TextBox>
                                      </asp:TableCell>
                              </asp:TableRow>
                              <asp:TableRow>
                                      <asp:TableCell>
                                          <asp:Label ID="subjectLabel" runat="server" Text="Subject"></asp:Label>
                                      </asp:TableCell>
                                      <asp:TableCell>
                                          <asp:DropDownList ID="subjectList" runat="server">
                                              <asp:ListItem Text="Choose" Value="" Selected="True"></asp:ListItem>
                                          </asp:DropDownList>
                                      </asp:TableCell>
                                      <asp:TableCell>
                                          <asp:Label ID="adminLabel" runat="server" Text="Admin:"></asp:Label>
                                      </asp:TableCell>
                                      <asp:TableCell>
                                          <asp:CheckBox ID="adminCheck" runat="server" />
                                      </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                      <asp:TableCell>
                                          <asp:Label ID="usernameLabel" runat="server" Text="Username:"></asp:Label>
                                      </asp:TableCell>
                                      <asp:TableCell>
                                          <asp:TextBox ID="usernameText" runat="server" Placeholder="Enter New Username" OnTextChanged="usernameText_TextChanged" ></asp:TextBox>
                                      </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:Label ID="passwordLabel" runat="server" Text="Password:"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:TextBox ID="passwordText" runat="server" Placeholder="Enter New Password" OnTextChanged="passwordText_TextChanged" ></asp:TextBox>
                                    </asp:TableCell>
                                </asp:TableRow>
                          </asp:Table>
                          <div id="Classes2">
                            <asp:Button ID="editButton" CssClass="editButton2" runat="server" Text="SAVE EDITS" CommandName="Edit" CommandArgument='<%# Eval("IDX") %>' />
                            <asp:Button ID="deleteButton" CssClass="deleteButton2" runat="server" Text="DELETE" CommandName="Delete" CommandArgument='<%# Eval("IDX") %>' />
                          </div>
                      </div>
                  </div>
            </ItemTemplate>
          </asp:Repeater>
        </div>
        <div id="navbuttonsAllStudents">
            <asp:Button ID="exitButton" class="navMinor" runat="server" Text="Exit" OnClick="exitButton_Click" />
            <asp:Button ID="addStudent" runat="server" Text="ADD" OnClick="addStudent_Click" />
        </div>
        
    </form>
</body>
</html>
