/*
    01_table_row_counts.sql
    Muc dich: kiem tra bang ton tai hay khong va dem CHINH XAC so dong.
    Chi doc du lieu, khong thay doi du lieu nghiep vu.
*/

SET NOCOUNT ON;

IF OBJECT_ID('tempdb..#TargetTables') IS NOT NULL DROP TABLE #TargetTables;
IF OBJECT_ID('tempdb..#TableCheck') IS NOT NULL DROP TABLE #TableCheck;

CREATE TABLE #TargetTables
(
    CategoryName nvarchar(100) NOT NULL,
    SchemaName sysname NOT NULL,
    TableName sysname NOT NULL
);

INSERT INTO #TargetTables (CategoryName, SchemaName, TableName)
VALUES
    -- Dang ky hoc phan
    (N'01. Dang ky hoc phan', N'dbo', N'psc_StudentScheduleStudyUnits'),
    (N'01. Dang ky hoc phan', N'dbo', N'psc_ScheduleStudyUnits'),
    (N'01. Dang ky hoc phan', N'dbo', N'psc_StudyUnits'),
    (N'01. Dang ky hoc phan', N'dbo', N'psc_Curriculums'),
    (N'01. Dang ky hoc phan', N'dbo', N'psc_StudentStudyUnits'),
    (N'01. Dang ky hoc phan', N'dbo', N'psc_StudentEnrolls'),

    -- Chuong trinh dao tao
    (N'02. Chuong trinh dao tao', N'dbo', N'psc1_StudentStudyPrograms'),
    (N'02. Chuong trinh dao tao', N'dbo', N'psc1_StudyPrograms'),
    (N'02. Chuong trinh dao tao', N'dbo', N'psc_Ologies'),
    (N'02. Chuong trinh dao tao', N'dbo', N'psc1_CurriculumStudyPrograms'),
    (N'02. Chuong trinh dao tao', N'dbo', N'psc1_StudyPlans'),
    (N'02. Chuong trinh dao tao', N'dbo', N'psc_StudyPlanByTerms'),
    (N'02. Chuong trinh dao tao', N'dbo', N'psc_StudentStudyStatus'),
    (N'02. Chuong trinh dao tao', N'dbo', N'psc_Semesters'),

    -- Hoc phi
    (N'03. Hoc phi', N'dbo', N'psc_CreditFees'),
    (N'03. Hoc phi', N'dbo', N'psc_CurriculumFeeCredits'),
    (N'03. Hoc phi', N'dbo', N'psc_FeeAttended'),
    (N'03. Hoc phi', N'dbo', N'psc_FeeRates'),
    (N'03. Hoc phi', N'dbo', N'psc_StudentTransactions'),
    (N'03. Hoc phi', N'dbo', N'psc_Transactions'),
    (N'03. Hoc phi', N'dbo', N'psc_Accounts'),
    (N'03. Hoc phi', N'dbo', N'psc_AccountMappings'),
    (N'03. Hoc phi', N'dbo', N'psc_Payments'),
    (N'03. Hoc phi', N'dbo', N'psc_Bills'),
    (N'03. Hoc phi', N'dbo', N'psc_StudentFeeAttended'),
    (N'03. Hoc phi', N'dbo', N'psc_StudentFeeOthers'),
    (N'03. Hoc phi', N'dbo', N'psc_FeeObjects'),
    (N'03. Hoc phi', N'dbo', N'psc_FeeTypes'),

    -- Thoi khoa bieu: ca phan he cu va phan he psc_Sch_*
    (N'04. Thoi khoa bieu', N'dbo', N'psc_WeekSchedules'),
    (N'04. Thoi khoa bieu', N'dbo', N'psc_Periods'),
    (N'04. Thoi khoa bieu', N'dbo', N'psc_Rooms'),
    (N'04. Thoi khoa bieu', N'dbo', N'psc_ProfessorScheduleStudyUnits'),
    (N'04. Thoi khoa bieu', N'dbo', N'psc_Professors'),
    (N'04. Thoi khoa bieu', N'dbo', N'psc_Terms'),
    (N'04. Thoi khoa bieu', N'dbo', N'psc_Sch_WeekSchedules'),
    (N'04. Thoi khoa bieu', N'dbo', N'psc_Sch_WeekScheduleStudyUnits');

CREATE TABLE #TableCheck
(
    CategoryName nvarchar(100) NOT NULL,
    FullTableName nvarchar(517) NOT NULL,
    TableExists bit NOT NULL,
    RowCount bigint NULL,
    DataStatus nvarchar(30) NOT NULL,
    ErrorMessage nvarchar(4000) NULL
);

DECLARE
    @CategoryName nvarchar(100),
    @SchemaName sysname,
    @TableName sysname,
    @FullTableName nvarchar(517),
    @Sql nvarchar(max),
    @RowCount bigint;

DECLARE table_cursor CURSOR LOCAL FAST_FORWARD FOR
    SELECT CategoryName, SchemaName, TableName
    FROM #TargetTables
    ORDER BY CategoryName, TableName;

OPEN table_cursor;
FETCH NEXT FROM table_cursor INTO @CategoryName, @SchemaName, @TableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @FullTableName = QUOTENAME(@SchemaName) + N'.' + QUOTENAME(@TableName);

    IF OBJECT_ID(@FullTableName, N'U') IS NULL
    BEGIN
        INSERT INTO #TableCheck
            (CategoryName, FullTableName, TableExists, RowCount, DataStatus, ErrorMessage)
        VALUES
            (@CategoryName, @FullTableName, 0, NULL, N'KHONG TON TAI', NULL);
    END
    ELSE
    BEGIN
        BEGIN TRY
            SET @RowCount = NULL;
            SET @Sql = N'SELECT @Result = COUNT_BIG(*) FROM ' + @FullTableName + N';';

            EXEC sys.sp_executesql
                @Sql,
                N'@Result bigint OUTPUT',
                @Result = @RowCount OUTPUT;

            INSERT INTO #TableCheck
                (CategoryName, FullTableName, TableExists, RowCount, DataStatus, ErrorMessage)
            VALUES
                (
                    @CategoryName,
                    @FullTableName,
                    1,
                    @RowCount,
                    CASE WHEN @RowCount = 0 THEN N'RONG' ELSE N'CO DU LIEU' END,
                    NULL
                );
        END TRY
        BEGIN CATCH
            INSERT INTO #TableCheck
                (CategoryName, FullTableName, TableExists, RowCount, DataStatus, ErrorMessage)
            VALUES
                (@CategoryName, @FullTableName, 1, NULL, N'LOI KHI DEM', ERROR_MESSAGE());
        END CATCH;
    END;

    FETCH NEXT FROM table_cursor INTO @CategoryName, @SchemaName, @TableName;
END;

CLOSE table_cursor;
DEALLOCATE table_cursor;

-- Ket qua chi tiet.
SELECT
    CategoryName AS N'Nhom du lieu',
    FullTableName AS N'Ten bang',
    TableExists AS N'Bang ton tai',
    RowCount AS N'So dong',
    DataStatus AS N'Trang thai',
    ErrorMessage AS N'Loi'
FROM #TableCheck
ORDER BY CategoryName, FullTableName;

-- Tong hop nhanh theo nhom.
SELECT
    CategoryName AS N'Nhom du lieu',
    COUNT(*) AS N'Tong so bang can kiem tra',
    SUM(CASE WHEN TableExists = 1 THEN 1 ELSE 0 END) AS N'So bang ton tai',
    SUM(CASE WHEN DataStatus = N'CO DU LIEU' THEN 1 ELSE 0 END) AS N'So bang co du lieu',
    SUM(CASE WHEN DataStatus = N'RONG' THEN 1 ELSE 0 END) AS N'So bang rong',
    SUM(CASE WHEN DataStatus = N'KHONG TON TAI' THEN 1 ELSE 0 END) AS N'So bang khong ton tai',
    SUM(COALESCE(RowCount, 0)) AS N'Tong so dong'
FROM #TableCheck
GROUP BY CategoryName
ORDER BY CategoryName;

