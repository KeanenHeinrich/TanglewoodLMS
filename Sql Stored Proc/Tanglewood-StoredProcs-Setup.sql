
/* STORED PROCEDURE INITIALISATION */

CREATE PROCEDURE [dbo].[addClass]
	@IDX INT,
	@Class INT,
	@UpdateWho INT,
	@Year INT
AS
BEGIN
	INSERT INTO [dbo].[tbl_ClassStudent]
	(
		idxStudent,
		idxClass,
		bitEnabled,
		txtUpdateWho,
		datUpdateDate
	)
	VALUES
	(
		@IDX,
		@Class,
		1,
		@UpdateWho,
		GETDATE()
	)
	DECLARE @Count INT
	SET @Count = 0
	WHILE (@Count < 4)
	BEGIN
		INSERT INTO [dbo].[tbl_Markbook]
		(
			idxClassStudent,
			intTerm,
			intYear,
			intMark1,
			intMark2,
			intMark3,
			bitEnabled,
			txtUpdateWho,
			datUpdateDate
		)
		VALUES
		(
			(SELECT IDX FROM tbl_ClassStudent WHERE idxStudent = @IDX AND idxClass = @Class AND bitEnabled = 'true'),
			@Count,
			@Year,
			0,
			0,
			0,
			1,
			@UpdateWho,
			GETDATE()
		)
	END
END

GO

CREATE PROCEDURE [dbo].[AddToken]

	@User INT,
	@Token NVARCHAR(256),
	@Expires DATETIME
AS
BEGIN
	INSERT INTO tbl_UserTokens
	VALUES
	(
		@User,
		@Token,
		@Expires,
		NULL,
		GETDATE()
	)
END;

GO

CREATE PROCEDURE [dbo].[CheckAdmin]
	@UserIDX INT,
	@AdminTrue BIT OUTPUT
AS 
BEGIN
	SELECT @AdminTrue = bitAdmin
	FROM tbl_Users
	WHERE IDX = @UserIDX
END;

GO

CREATE PROCEDURE [dbo].[CheckDuplicate]
	@original NVARCHAR(50),
	@output BIT OUTPUT
AS
BEGIN
	IF EXISTS (SELECT txtUsername FROM tbl_Users WHERE txtUsername = @original) 
	BEGIN
		SET @output = 1
	END
	ELSE
	BEGIN
		SET @output = 0
	END
END

GO

CREATE PROCEDURE [dbo].[CheckToken]
	@inputToken NVARCHAR(256),
	@user INT OUTPUT
AS
BEGIN
	DECLARE @Expires DATETIME
	SELECT @user = idxUser, @Expires = datExpires FROM tbl_UserTokens
	WHERE @inputToken = txtToken AND datExpires >= GETDATE()
	IF @user IS NULL
		BEGIN
			SET @user = 0;
		END
END;

GO

CREATE PROCEDURE [dbo].[ClassDropdown]
	@Grade INT,
	@Subject INT
AS
BEGIN
	IF @Grade IS NULL AND @Subject IS NULL
		BEGIN
			SELECT IDX, txtClass
			FROM tbl_Classes
		END
	ELSE IF @Grade IS NULL
		BEGIN
			SELECT IDX, txtClass
			FROM tbl_Classes
			WHERE idxSubject = @Subject
		END
	ELSE IF @Subject IS NULL
		BEGIN
			SELECT IDX, txtClass
			FROM tbl_Classes
			WHERE idxGrade = @Grade
		END
	ELSE
		BEGIN
			SELECT IDX, txtClass
			FROM tbl_Classes
			WHERE @Grade = idxGrade AND @Subject = idxSubject
		END
END;

GO

CREATE PROCEDURE [dbo].[CreateAnnouncement]
	@UserID INT,
	@Message NVARCHAR(200)
AS
BEGIN
	INSERT INTO tbl_Announcements
	VALUES(
		@UserID,
		@Message,
		'False',
		GETDATE(),
		'True',
		NULL,
		GETDATE()
	);
END;

GO

CREATE PROCEDURE [dbo].[CreateStudent]
	@UpdateWho INT
AS
BEGIN
    INSERT INTO tbl_Students
	([txtName]
     ,[txtSurname]
     ,[txtGovID]
     ,[datDOB]
     ,[txtGender]
     ,[txtSex]
     ,[txtAddress]
     ,[txtParentEmail]
     ,[txtParentPhone]
     ,[txtLearningDifficulties]
     ,[txtAdditionalNotes]
     ,[datEnrolled]
     ,[bitEnabled]
     ,[txtUpdateWho]
     ,[datUpdateDate])
    VALUES (
		null,
		null,
		null,
		null,
		null,
		null,
		null,
		null,
		null,
		null,
		null,
		CAST(GETDATE() AS DATE),
		'True',
		@UpdateWho,
		GETDATE());
END;

GO

CREATE PROCEDURE [dbo].[CreateUser]
	@UpdateWho INT,
	@Created BIT OUTPUT
AS
IF NOT EXISTS (
	SELECT 1
	FROM tbl_Users
	WHERE txtUsername IS NULL
)
BEGIN
INSERT INTO [dbo].[tbl_Users]
           ([txtUsername]
           ,[txtInitials]
           ,[txtPassword]
           ,[txtName]
           ,[txtSurname]
           ,[idxSubject]
           ,[bitAdmin]
           ,[bitEnabled]
           ,[txtUpdateWho]
           ,[datUpdateDate])
     VALUES
	 (
           	null, 
			null,
			null,
			null,
			null,
			null,
			0,
			1,
			@UpdateWho,
			GETDATE()
			);
	SET @Created = 'true'
END
ELSE
BEGIN
	SET @Created = 'false'
END

GO

CREATE PROCEDURE [dbo].[DisableStudent]
    @IDX INT
AS
BEGIN
    UPDATE tbl_Students
    SET bitEnabled = 'False'
    WHERE IDX = @IDX;
END;

GO

CREATE PROCEDURE [dbo].[DisableUser]
	@UserIDX INT
AS
BEGIN
	IF (SELECT txtUsername FROM tbl_Users WHERE IDX = @UserIDX) = null
	BEGIN
		UPDATE tbl_Users
		SET txtUsername = CONCAT('blankUser', IDX)
	END
	UPDATE tbl_Users
	SET bitEnabled = 'False'
	WHERE IDX = @UserIDX
END;

GO

CREATE PROCEDURE [dbo].[DisplayAnnouncements]
AS
BEGIN
	SELECT Users.txtName, Users.txtSurname, Users.txtInitials, txtAnnouncement, datCreationDate
	FROM tbl_Announcements AS Ann
	INNER JOIN tbl_Users AS Users ON Ann.idxUser = Users.IDX
	ORDER BY Ann.IDX DESC
END;

GO

CREATE PROCEDURE [dbo].[DisplayReport]
	@Student INT,
	@Term INT,
	@Year INT
AS
BEGIN
	IF @Term = 5
	BEGIN
		SELECT txtSubject, intMark1, intMark2, intMark3, intTotalMark1, intTotalMark2, intTotalMark3
		FROM tbl_Markbook AS Mk
		INNER JOIN tbl_ClassStudent AS ClStd ON ClStd.IDX = Mk.idxClassStudent
		INNER JOIN tbl_Classes AS Cl ON Cl.IDX = Clstd.idxClass
		INNER JOIN tbl_Subjects AS Sub ON Sub.IDX = Cl.idxSubject
		INNER JOIN tbl_TotalMarks AS Tmk ON Tmk.idxSubject = Sub.IDX
		WHERE idxStudent = @Student AND Mk.intYear = @Year AND Cl.idxGrade = Tmk.idxGrade AND Cl.idxSubject = Tmk.idxSubject
		ORDER BY Sub.IDX ASC, intTerm ASC
	END
	ELSE
	BEGIN
		SELECT txtSubject, intMark1, intMark2, intMark3, intTotalMark1, intTotalMark2, intTotalMark3
		FROM tbl_Markbook AS Mk
		INNER JOIN tbl_ClassStudent AS ClStd ON ClStd.IDX = Mk.idxClassStudent
		INNER JOIN tbl_Classes AS Cl ON Cl.IDX = Clstd.idxClass
		INNER JOIN tbl_Subjects AS Sub ON Sub.IDX = Cl.idxSubject
		INNER JOIN tbl_TotalMarks AS Tmk ON Tmk.idxSubject = Sub.IDX
		WHERE idxStudent = @Student AND intTerm = @Term AND Mk.intYear = @Year AND Cl.idxGrade = Tmk.idxGrade AND Cl.idxSubject = Tmk.idxSubject
		ORDER BY Sub.IDX ASC, intTerm ASC
	END
END;

GO

CREATE PROCEDURE [dbo].[DisplayStudent]
	@Grade INT,
	@Input NVARCHAR(50)
AS
BEGIN
	SELECT * FROM tbl_Students AS Std
	INNER JOIN tbl_ClassStudent AS ClStd ON ClStd.idxStudent = Std.IDX
	INNER JOIN tbl_Classes AS Cl ON ClStd.idxClass = Cl.IDX
	WHERE idxGrade = @Grade;
END

GO

CREATE PROCEDURE [dbo].[EmptyCheck]
	@IDX INT,
	@output BIT OUTPUT
AS
BEGIN
	IF (SELECT txtUsername FROM tbl_Users WHERE @IDX = IDX) IS NULL
	BEGIN
		SET @output = 'true'
	END
	ELSE
	BEGIN
		SET @output = 'false'
	END
END

GO

CREATE PROCEDURE [dbo].[EmptyStudents]
AS
BEGIN
	SELECT DISTINCT *
	FROM tbl_Students AS Std
	LEFT JOIN tbl_ClassStudent AS Cl ON Cl.idxStudent = Std.IDX
	WHERE idxStudent IS NULL AND Std.bitEnabled = 'true'
END

GO

CREATE PROCEDURE [dbo].[FetchMarkbook]
	@Term INT,
	@Class INT,
	@Year INT
AS
BEGIN
    SELECT txtName, txtSurname, Std.IDX, intMark1, intMark2, intMark3
    FROM tbl_Markbook AS Mk
	INNER JOIN tbl_ClassStudent AS ClStd ON ClStd.IDX = Mk.idxClassStudent
	INNER JOIN tbl_Students AS Std ON Std.IDX = ClStd.idxStudent
    WHERE intTerm = @Term AND ClStd.idxClass = @Class AND intYear = @Year
END;

GO

CREATE PROCEDURE [dbo].[FetchMaxMarks]
	@Class INT,
	@Year INT
AS
BEGIN
	SELECT intTotalMark1, intTotalMark2, intTotalMark3
	FROM tbl_TotalMarks AS Mk
	INNER JOIN tbl_Grades AS Gr ON Gr.IDX = Mk.idxGrade
	INNER JOIN tbl_Classes AS Cl ON Cl.idxGrade = Gr.IDX
	WHERE @Class = Cl.IDX AND @Year = Mk.intYear
END

GO

CREATE PROCEDURE [dbo].[fetchRemark]
	@Year INT,
	@idxStudent INT
AS
BEGIN
	SELECT txtRemarks FROM tbl_Remarks
	WHERE idxStudent = @idxStudent AND @Year = intYear
END

GO

CREATE PROCEDURE [dbo].[FetchStaff]
    @SearchInput VARCHAR(50)
AS
BEGIN
	IF @SearchInput IS NULL
		BEGIN
			SELECT Users.IDX, txtName, txtSurname, txtSubject
			FROM tbl_Users AS Users
			LEFT JOIN tbl_Subjects AS Sub ON Users.idxSubject = Sub.IDX
			WHERE Users.bitEnabled = 'true'
		END
	ELSE
		BEGIN
			SELECT Users.IDX, txtName, txtSurname, txtSubject
			FROM tbl_Users AS Users
			LEFT JOIN tbl_Subjects AS Sub ON Users.idxSubject = Sub.IDX
			WHERE (txtName LIKE '%' + @SearchInput + '%' OR txtSurname LIKE '%' + @SearchInput + '%') AND Users.bitEnabled = 'true';
		END
END;

GO

CREATE PROCEDURE [dbo].[FetchStaffInfo]
	@IDX INT
AS
BEGIN
    SELECT Users.IDX, txtUsername, txtName, txtSurname, idxSubject, bitAdmin
    FROM tbl_Users AS Users
	LEFT JOIN tbl_Subjects AS Sub ON Users.idxSubject = Sub.IDX
	WHERE Users.IDX = @IDX
END;

GO

CREATE PROCEDURE [dbo].[FetchStudent]
	@Class INT,
    @SearchInput VARCHAR(50)
AS
BEGIN
	IF @Class IS NULL
		BEGIN
			SELECT Std.IDX, txtName, txtSurname, txtGrade
			FROM tbl_Students AS Std
			INNER JOIN tbl_ClassStudent AS ClStd ON Std.IDX = ClStd.idxStudent
			INNER JOIN tbl_Classes AS Cl ON Cl.IDX = ClStd.idxClass
			INNER JOIN tbl_Grades AS Gr ON Gr.IDX = Cl.idxGrade
			WHERE (txtName LIKE '%' + @SearchInput + '%' OR txtSurname LIKE '%' + @SearchInput + '%' OR txtGovID LIKE '%' + @SearchInput + '%') AND Std.bitEnabled = 'true'
			ORDER BY Std.IDX DESC
		END
	ELSE IF @Class IS NOT NULL
		BEGIN
			SELECT Std.IDX, txtName, txtSurname, txtGrade, txtClass
			FROM tbl_Students AS Std
			INNER JOIN tbl_ClassStudent AS ClStd ON Std.IDX = ClStd.idxStudent
			INNER JOIN tbl_Classes AS Cl ON Cl.IDX = ClStd.idxClass
			INNER JOIN tbl_Grades AS Gr ON Gr.IDX = Cl.idxGrade
			WHERE idxClass = @Class AND (txtName LIKE '%' + @SearchInput + '%' OR txtSurname LIKE '%' + @SearchInput + '%' OR txtGovID LIKE '%' + @SearchInput + '%') AND Std.bitEnabled = 'true'
			ORDER BY Std.IDX DESC
		END
END;

GO

CREATE PROCEDURE [dbo].[FetchStudentClasses]
	@IDX INT
AS
BEGIN
	IF @IDX IS NOT NULL
	BEGIN
	SELECT Cl.txtClass
	FROM tbl_Classes AS Cl
	INNER JOIN tbl_ClassStudent AS ClStd ON ClStd.idxClass = Cl.IDX
	INNER JOIN tbl_Students AS Std ON Std.IDX = ClStd.idxStudent
	WHERE Std.IDX = @IDX
	END
	ELSE
	BEGIN
		SELECT IDX, Cl.txtClass
		FROM tbl_Classes AS Cl
	END
END

GO

CREATE PROCEDURE [dbo].[FetchStudentDetails]
	@IDX INT
AS
BEGIN
    SELECT DISTINCT Std.IDX, Std.txtName, Std.txtSurname, Std.txtGovID, Std.datDOB, Std.txtGender, Std.datEnrolled, Std.txtAddress, Std.txtParentEmail, Std.txtParentPhone, Std.txtLearningDifficulties, Std.txtAdditionalNotes
    FROM tbl_Students AS Std
	WHERE Std.IDX = @IDX
END;

GO

CREATE PROCEDURE [dbo].[GradeDropdown]
AS
BEGIN
	SELECT IDX, txtGrade
	FROM tbl_Grades
END;

GO

CREATE PROCEDURE [dbo].[LoginValidation]
    @Username NVARCHAR(50),
	@InputPassword NVARCHAR(50),
	@returnValue BIT OUTPUT,
	@userID INT OUTPUT
AS
BEGIN
	DECLARE @Password NVARCHAR(50)
	SELECT @Password = txtPassword FROM tbl_Users
	WHERE txtUsername = @Username;

	IF @Password IS NOT NULL AND @Password = @InputPassword
		BEGIN
			SET	@returnValue = 'True';
			SELECT @userID = IDX FROM tbl_Users
			WHERE txtUsername = @Username
		END
	ELSE
		BEGIN
			SET @returnValue = 'False';
		END
END;

GO

CREATE PROCEDURE [dbo].[removeClass] 
	@IDX INT,
	@Class INT,
	@UpdateWho INT
AS
BEGIN
	UPDATE tbl_ClassStudent
	SET bitEnabled = 'false',
		txtUpdateWho = @UpdateWho,
		datUpdateDate = GETDATE()
	WHERE idxStudent = @IDX AND idxClass = @Class
	DECLARE @Count INT
	SET @Count = 0
	WHILE (@Count < 4)
	BEGIN
		UPDATE [dbo].[tbl_Markbook]
		SET bitEnabled = 'false',
			txtUpdateWho = @UpdateWho,
			datUpdateDate = GETDATE()
		WHERE idxClassStudent = (SELECT IDX FROM tbl_ClassStudent WHERE idxStudent = @IDX AND idxClass = @Class) AND intTerm = @Count
	END
END

GO

CREATE PROCEDURE [dbo].[SaltPassword]
	@InputPassword NVARCHAR(50),
	@OutputPassword NVARCHAR(50) OUTPUT
AS
BEGIN
	DECLARE @salt NVARCHAR(50)
	SELECT @salt = salt FROM tbl_Salt
	WHERE IDX = 1;
	SET @outputPassword = CONCAT(@inputPassword, @salt)
END;

GO

CREATE PROCEDURE [dbo].[StudentDropdown]
	@Grade INT,
	@Class INT
AS
BEGIN
	SELECT txtName, txtSurname, Std.IDX
	FROM tbl_Students AS Std
	INNER JOIN tbl_ClassStudent AS ClStd ON ClStd.idxStudent = Std.IDX
	INNER JOIN tbl_Classes AS Cl ON Cl.IDX = ClStd.idxClass
	WHERE idxGrade = @Grade AND idxClass = @Class
END;

GO

CREATE PROCEDURE [dbo].[StudentDropdownReport]
	@Year INT,
	@Grade INT
AS
BEGIN
	IF @Grade IS NULL AND @Year IS NULL
		BEGIN
			SELECT DISTINCT Std.IDX, txtName, txtSurname, Std.IDX
			FROM tbl_Students AS Std
		END
	ELSE IF @Grade IS NULL
		BEGIN
			SELECT DISTINCT Std.IDX, txtName, txtSurname, Std.IDX
			FROM tbl_Students AS Std
			INNER JOIN tbl_ClassStudent AS ClStd ON ClStd.idxStudent = Std.IDX
			INNER JOIN tbl_Classes AS Cl ON Cl.IDX = ClStd.idxClass
			WHERE EXISTS (SELECT IDX FROM tbl_Markbook AS Mk WHERE intYear = @Year AND Mk.idxClassStudent = ClStd.IDX)
		END
	ELSE IF @Year IS NULL
		BEGIN
			SELECT DISTINCT Std.IDX, txtName, txtSurname, Std.IDX
			FROM tbl_Students AS Std
			INNER JOIN tbl_ClassStudent AS ClStd ON ClStd.idxStudent = Std.IDX
			INNER JOIN tbl_Classes AS Cl ON Cl.IDX = ClStd.idxClass
			WHERE idxGrade = @Grade
		END
	ELSE
		BEGIN
			SELECT DISTINCT Std.IDX, txtName, txtSurname, Std.IDX
			FROM tbl_Students AS Std
			INNER JOIN tbl_ClassStudent AS ClStd ON ClStd.idxStudent = Std.IDX
			INNER JOIN tbl_Classes AS Cl ON Cl.IDX = ClStd.idxClass
			WHERE idxGrade = @Grade AND EXISTS (SELECT IDX FROM tbl_Markbook AS Mk WHERE intYear = @Year AND Mk.idxClassStudent = ClStd.IDX)
		END
END;

GO

CREATE PROCEDURE [dbo].[SubjectDropdown]
AS
BEGIN
	SELECT IDX, txtSubject
	FROM tbl_Subjects
END

GO

CREATE PROCEDURE [dbo].[UpdateMarkBook]
    @StudentID INT,
	@Class INT,
	@Year INT,
    @Mark1 INT,
	@Mark2 INT,
	@Mark3 INT,
	@UpdateWho NVARCHAR(50)
AS
BEGIN
	DECLARE @ClassStudent INT
	SELECT @ClassStudent = IDX FROM tbl_ClassStudent
	WHERE idxClass = @Class AND idxStudent = @StudentID
    UPDATE tbl_Markbook
    SET intMark1 = @Mark1, intMark2 = @Mark2, intMark3 = @Mark3, txtUpdateWho = @UpdateWho, datUpdateDate = GETDATE()
    WHERE idxClassStudent = @ClassStudent AND intYear = @Year
END;

GO

CREATE PROCEDURE [dbo].[UpdateRemark]
	@Year INT,
	@idxStudent INT,
	@Remark NVARCHAR(500),
	@User INT
AS
BEGIN
	UPDATE tbl_Remarks
	SET txtRemarks = @Remark, txtUpdateWho = @User, datUpdateDate = GETDATE()
	WHERE idxStudent = @idxStudent AND intYear = @Year
END

GO

CREATE PROCEDURE [dbo].[UpdateStudent]
	@IDX INT,
    @Name NVARCHAR(50),
    @Surname NVARCHAR(50),
	@GovID NVARCHAR(13),
    @DOB DATE,
	@Gender NVARCHAR(50),
	@Address NVARCHAR(50),
	@ParentEmail NVARCHAR(50),
	@ParentPhone NVARCHAR(50),
	@LearningDifficulties NVARCHAR(200) = NULL,
	@AdditionalNotes NVARCHAR(200) = NULL,
	@UpdateWho INT
AS
BEGIN
    UPDATE tbl_Students
    SET txtName = @Name,
        txtSurname = @Surname,
		txtGovID = @GovID,
        datDOB = @DOB,
		txtGender = @Gender,
		txtAddress = @Address,
		txtParentEmail = @ParentEmail,
		txtParentPhone = @ParentPhone,
		txtLearningDifficulties = @LearningDifficulties,
		txtAdditionalNotes = @AdditionalNotes,
		txtUpdateWho = @UpdateWho,
		datUpdateDate = GETDATE()
    WHERE IDX = @IDX;
END;

GO

CREATE PROCEDURE [dbo].[UpdateUser]
    @IDX INT,
    @Name NVARCHAR(50),
	@Username NVARCHAR(50),
	@Initials NVARCHAR(2),
	@Password NVARCHAR(256),
	@Surname NVARCHAR(50),
    @Subject INT,
	@Admin BIT,
	@UpdateWho NVARCHAR(50)
AS
BEGIN
	IF @Password IS NULL AND @Username IS NULL
	BEGIN
    UPDATE tbl_Users
    SET txtName = @Name, txtInitials = @Initials, txtSurname = @Surname, idxSubject = @Subject, bitAdmin = @Admin, txtUpdateWho = @UpdateWho, datUpdateDate = GETDATE()
    WHERE IDX = @IDX;
	END
	ELSE IF @Password IS NULL
	BEGIN
	UPDATE tbl_Users
    SET txtUsername=@Username, txtName = @Name, txtInitials = @Initials, txtSurname = @Surname, idxSubject = @Subject, bitAdmin = @Admin, txtUpdateWho = @UpdateWho, datUpdateDate = GETDATE()
    WHERE IDX = @IDX;
	END
	ELSE IF @Username IS NULL
	BEGIN
	UPDATE tbl_Users
    SET txtPassword = @Password, txtName = @Name, txtInitials = @Initials, txtSurname = @Surname, idxSubject = @Subject, bitAdmin = @Admin, txtUpdateWho = @UpdateWho, datUpdateDate = GETDATE()
    WHERE IDX = @IDX;
	END
	ELSE
	BEGIN
	UPDATE tbl_Users
    SET txtPassword = @Password, txtUsername=@Username, txtName = @Name, txtInitials = @Initials, txtSurname = @Surname, idxSubject = @Subject, bitAdmin = @Admin, txtUpdateWho = @UpdateWho, datUpdateDate = GETDATE()
    WHERE IDX = @IDX;
	END
END;