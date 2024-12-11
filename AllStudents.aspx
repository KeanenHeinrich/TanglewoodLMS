<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AllStudents.aspx.cs" Inherits="TanglewoodLMS.AllStudents" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="AllStudents" runat="server">
        <div id="validationCheck" runat="server" style="position: absolute; top: 0; left: 0; height: 100vh; width: 100vw; z-index: 1000; background-color: rgba(0, 0, 0, 0.7); display: none; justify-content: center; align-items: center;">      
            <div id="popup" class="popup">
                <asp:Label ID="popupHeading" class="popupHeading" runat="server" Text="DELETING STUDENT"></asp:Label>
                <asp:HiddenField ID="idxStore" runat="server" Value=""/>
                <asp:Label ID="popupText" runat="server"></asp:Label>
                <div id="popupButtons">
                    <asp:Button ID="confirm" runat="server" Text="DELETE" Onclick="confirm_Click"/>
                    <asp:Button ID="cancel" runat="server" Text="CANCEL" Onclick="cancel_Click"/>
                </div>
            </div>
        </div>
        <div id="editClasses" runat="server" style="position: absolute; top: 0; left: 0; height: 100vh; width: 100vw; z-index: 1000; background-color: rgba(0, 0, 0, 0.7); display: none; justify-content: center; align-items: center;">      
            <div id="popup2" class="popup">
                <asp:Label ID="popupHeading2" class="popupHeading" runat="server" Text="Assign Classes"></asp:Label>
                <asp:HiddenField ID="idxStore2" runat="server" Value="" />
                <br />
                <div id="container" style="overflow-y: scroll; height: 100%; width:100%;">
                <asp:Repeater ID="classAssignRepeater" runat="server">
                    <ItemTemplate>
                        <div style="height: 30px; margin-bottom: 10px;">
                            <asp:Label id="itemClass" runat="server"><%# Eval("txtClass") %></asp:Label>
                            <asp:Button ID="addClass" CssClass="addRemove" runat="server" Text="Assign" Visible="false" />
                            <asp:Button ID="removeClass" CssClass="addRemove" runat="server" Text="Unassign" />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                </div>
                <div id="buttonHolder">
                    <asp:Button ID="cancelChanges" runat="server" Text="EXIT" OnClick="cancelChanges_Click" />
                </div>
            </div>
        </div>
        <link rel="stylesheet" type="text/css" href="style.css" />
        <div id="headerAllStudents">
            <asp:Label ID="header" runat="server" Text="ADMIN - STUDENT MANAGEMENT"></asp:Label>
        </div>
        <div id="filterAllStudents">
            <asp:TextBox ID="searchStudents" runat="server" Placeholder="Search Students" OnTextChanged="searchStudents_TextChanged" AutoPostBack="True"></asp:TextBox>
            <div id="emptyDiv">
                <asp:Label ID="showNewLabel" runat="server" Text="Show Classless Students"></asp:Label>
                <asp:CheckBox ID="showNew" runat="server" AutoPostBack="true" OnCheckedChanged="showNew_CheckedChanged" />
            </div>
            <asp:DropDownList ID="gradeStudent" runat="server" OnSelectedIndexChanged="gradeStudent_SelectedIndexChanged" AutoPostBack="True">
                <asp:ListItem Text="Grade" Value="" Selected="True"></asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList ID="classStudent" runat="server" OnSelectedIndexChanged="classStudent_SelectedIndexChanged" AutoPostBack="True">
                <asp:ListItem Text="Class" Value="" Selected="True"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <div id="listAllStudents">
            <asp:Repeater ID="studentsRepeater" runat="server" OnItemCommand="studentsRepeater_ItemCommand">
              <ItemTemplate>
                <div id="postAllStudents" runat="server" style="box-sizing: border-box; display: flex;flex-direction: row;width: 100%; padding: 15px; background-color: whitesmoke; color: #868686; border: solid 1px; border-radius: 10px; justify-content: space-between; font-size: 25px;">
                    <asp:Label ID="studentName" runat="server" Text='<%# Eval("IDX") + " | " + Eval("txtName") + " " + Eval("txtSurname") %>' />
                    <asp:ImageButton ID="expandButton" EnableTheming="false" CssClass="expandButton" ImageUrl="Resources/downArrow.png" runat="server" CommandName="Select" CommandArgument='<%# Eval("IDX") %>' />
                </div>
                  <div id="detailDisplay" runat="server" style="display: none; width: 100%; min-height: 550px;">
                      <div id="detailBackground">
                          <asp:Table ID="displayTable" runat="server" EnableTheming="false">
                              <asp:TableRow>
                                  <asp:TableCell>
                                      <asp:Label ID="nameLabel" runat="server" Text="Name:"></asp:Label>
                                  </asp:TableCell>
                                  <asp:TableCell>
                                      <asp:TextBox ID="nameBox" runat="server" Placeholder="Name"></asp:TextBox>
                                  </asp:TableCell>
                                  <asp:TableCell>
                                      <asp:Label ID="surnameLabel" runat="server" Text="Surname:"></asp:Label>
                                  </asp:TableCell>
                                  <asp:TableCell>
                                      <asp:TextBox ID="surnameBox" runat="server" Placeholder="Surname"></asp:TextBox>
                                  </asp:TableCell>
                              </asp:TableRow>
                              <asp:TableRow>
                                  <asp:TableCell>
                                      <asp:Label ID="idLabel" runat="server" Text="Student ID:"></asp:Label>
                                  </asp:TableCell>
                                  <asp:TableCell>
                                      <asp:TextBox ID="ID" runat="server" Placeholder="Student ID"></asp:TextBox>
                                  </asp:TableCell>
                                  <asp:TableCell>
                                      <asp:Label ID="govLabel" runat="server" Text="Government ID:"></asp:Label>
                                  </asp:TableCell>
                                  <asp:TableCell>
                                      <asp:TextBox ID="govBox" runat="server" Placeholder="Government ID"></asp:TextBox>
                                  </asp:TableCell>
                              </asp:TableRow>
                              <asp:TableRow>
                                  <asp:TableCell>
                                      <asp:Label ID="enrollLabel" runat="server" Text="Date Enrolled:"></asp:Label>
                                  </asp:TableCell>
                                  <asp:TableCell>
                                      <asp:TextBox ID="enrollBox" runat="server" Placeholder="Enrollment Date"></asp:TextBox>
                                  </asp:TableCell>
                                  <asp:TableCell>
                                      <asp:Label ID="addressLabel" runat="server" Text="Home Address:"></asp:Label>
                                  </asp:TableCell>
                                  <asp:TableCell>
                                      <asp:TextBox ID="addressBox" runat="server" Placeholder="Home Address"></asp:TextBox>
                                  </asp:TableCell>
                              </asp:TableRow>
                              <asp:TableRow>
                                  <asp:TableCell>
                                      <asp:Label ID="genderLabel" runat="server" Text="Gender:"></asp:Label>
                                  </asp:TableCell>
                                  <asp:TableCell>
                                      <asp:TextBox ID="genderBox" runat="server" Placeholder="Gender"></asp:TextBox>
                                  </asp:TableCell>
                                  <asp:TableCell>
                                      <asp:Label ID="dobLabel" runat="server" Text="Date of Birth:"></asp:Label>
                                  </asp:TableCell>
                                  <asp:TableCell>
                                      <asp:TextBox ID="dobBox" runat="server" Placeholder="Date of Birth"></asp:TextBox>
                                  </asp:TableCell>
                              </asp:TableRow>
                              <asp:TableRow>
                                  <asp:TableCell>
                                      <asp:Label ID="emailLabel" runat="server" Text="Parent Email:"></asp:Label>
                                  </asp:TableCell>
                                  <asp:TableCell>
                                      <asp:TextBox ID="emailBox" runat="server" Placeholder="Parent Email"></asp:TextBox>
                                  </asp:TableCell>
                                  <asp:TableCell>
                                      <asp:Label ID="phoneLabel" runat="server" Text="Parent Phone Number:"></asp:Label>
                                  </asp:TableCell>
                                  <asp:TableCell>
                                      <asp:TextBox ID="phoneBox" runat="server" Placeholder="Parent Phone Number"></asp:TextBox>
                                  </asp:TableCell>
                              </asp:TableRow>
                              <asp:TableRow>
                                  <asp:TableCell>
                                      <asp:Label ID="diffLabel" runat="server" Text="Learning Difficulties:"></asp:Label>
                                  </asp:TableCell>
                                  <asp:TableCell>
                                      <asp:TextBox ID="diffBox" runat="server" Placeholder="Learning Difficulties"></asp:TextBox>
                                  </asp:TableCell>
                              </asp:TableRow>
                          </asp:Table>
                          <asp:TextBox ID="noteBox" runat="server" Placeholder="Additional Notes" TextMode="MultiLine" style="min-height: 20%; margin-top: 1%;"></asp:TextBox>
                          <div id="studentClasses" style="display: flex; flex-direction: column; align-items: center; margin: 1%;">
                          <asp:Label ID="classList" runat="server" Text="Assigned Classes" Font-Size="20pt"></asp:Label>
                              <br />
                            <asp:Repeater ID="classRepeater" runat="server">
                                <ItemTemplate>
                                    <asp:Label id="currentClass" runat="server"><%# Eval("txtClass") %></asp:Label>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Button CssClass="classChange" runat="server" Text="Assign Classes" CommandName="Class" CommandArgument='<%# Eval("IDX") %>' />
                          </div>
                          <div id="Classes">
                            <asp:Button ID="editButton" CssClass="editButton" runat="server" Text="SAVE EDITS" CommandName="Edit" CommandArgument='<%# Eval("IDX") %>' />
                            <asp:Button ID="deleteButton" CssClass="deleteButton" runat="server" Text="DELETE" CommandName="Delete" CommandArgument='<%# Eval("IDX") %>' />
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
