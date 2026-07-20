/*
    03_relationship_coverage.sql
    Muc dich: kiem tra bang co data nhung data co noi duoc sang bang cha hay khong.

    MissingParentRows > 0:
      - Co ban ghi mo coi, hoac
      - Khoa lien ket trong schema/du lieu dang dung mot quy uoc khac.

    Script chi SELECT, khong cap nhat du lieu.
*/

SET NOCOUNT ON;

SELECT *
FROM
(
    SELECT
        N'Dang ky -> Sinh vien' AS RelationshipName,
        COUNT_BIG(*) AS ChildRows,
        SUM(CASE WHEN s.StudentID IS NULL THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END) AS MissingParentRows
    FROM dbo.psc_StudentScheduleStudyUnits AS x
    LEFT JOIN dbo.psc_Students AS s ON s.StudentID = x.StudentID

    UNION ALL

    SELECT
        N'Dang ky -> Lop hoc phan',
        COUNT_BIG(*),
        SUM(CASE WHEN p.ScheduleStudyUnitID IS NULL THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END)
    FROM dbo.psc_StudentScheduleStudyUnits AS x
    LEFT JOIN dbo.psc_ScheduleStudyUnits AS p
        ON p.ScheduleStudyUnitID = x.ScheduleStudyUnitID

    UNION ALL

    SELECT
        N'Lop hoc phan -> Hoc phan',
        COUNT_BIG(*),
        SUM(CASE WHEN p.StudyUnitID IS NULL THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END)
    FROM dbo.psc_ScheduleStudyUnits AS x
    LEFT JOIN dbo.psc_StudyUnits AS p ON p.StudyUnitID = x.StudyUnitID

    UNION ALL

    SELECT
        N'Hoc phan -> Mon hoc',
        COUNT_BIG(*),
        SUM(CASE WHEN p.CurriculumID IS NULL THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END)
    FROM dbo.psc_StudyUnits AS x
    LEFT JOIN dbo.psc_Curriculums AS p ON p.CurriculumID = x.CurriculumID

    UNION ALL

    SELECT
        N'Sinh vien CTDT -> Sinh vien',
        COUNT_BIG(*),
        SUM(CASE WHEN p.StudentID IS NULL THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END)
    FROM dbo.psc1_StudentStudyPrograms AS x
    LEFT JOIN dbo.psc_Students AS p ON p.StudentID = x.StudentID

    UNION ALL

    SELECT
        N'Sinh vien CTDT -> CTDT',
        COUNT_BIG(*),
        SUM(CASE WHEN p.StudyProgramID IS NULL THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END)
    FROM dbo.psc1_StudentStudyPrograms AS x
    LEFT JOIN dbo.psc1_StudyPrograms AS p ON p.StudyProgramID = x.StudyProgramID

    UNION ALL

    SELECT
        N'CTDT -> Nganh',
        COUNT_BIG(*),
        SUM(CASE WHEN p.OlogyID IS NULL THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END)
    FROM dbo.psc1_StudyPrograms AS x
    LEFT JOIN dbo.psc_Ologies AS p ON p.OlogyID = x.OlogyID

    UNION ALL

    SELECT
        N'Mon trong CTDT -> CTDT',
        COUNT_BIG(*),
        SUM(CASE WHEN p.StudyProgramID IS NULL THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END)
    FROM dbo.psc1_CurriculumStudyPrograms AS x
    LEFT JOIN dbo.psc1_StudyPrograms AS p ON p.StudyProgramID = x.StudyProgramID

    UNION ALL

    SELECT
        N'Mon trong CTDT -> Mon hoc',
        COUNT_BIG(*),
        SUM(CASE WHEN p.CurriculumID IS NULL THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END)
    FROM dbo.psc1_CurriculumStudyPrograms AS x
    LEFT JOIN dbo.psc_Curriculums AS p ON p.CurriculumID = x.CurriculumID

    UNION ALL

    SELECT
        N'Lich tuan -> Lop hoc phan',
        COUNT_BIG(*),
        SUM(CASE WHEN p.ScheduleStudyUnitID IS NULL THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END)
    FROM dbo.psc_WeekSchedules AS x
    LEFT JOIN dbo.psc_ScheduleStudyUnits AS p
        ON p.ScheduleStudyUnitID = x.ScheduleStudyUnitID

    UNION ALL

    SELECT
        N'Lich tuan -> Tiet hoc',
        COUNT_BIG(*),
        SUM(CASE WHEN x.PeriodID IS NOT NULL AND p.PeriodID IS NULL THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END)
    FROM dbo.psc_WeekSchedules AS x
    LEFT JOIN dbo.psc_Periods AS p ON p.PeriodID = x.PeriodID

    UNION ALL

    SELECT
        N'Lich tuan -> Phong hoc',
        COUNT_BIG(*),
        SUM(CASE WHEN x.RoomID IS NOT NULL AND p.RoomID IS NULL THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END)
    FROM dbo.psc_WeekSchedules AS x
    LEFT JOIN dbo.psc_Rooms AS p ON p.RoomID = x.RoomID

    UNION ALL

    SELECT
        N'Lich Sch -> Lop hoc phan',
        COUNT_BIG(*),
        SUM(CASE WHEN p.ScheduleStudyUnitID IS NULL THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END)
    FROM dbo.psc_Sch_WeekScheduleStudyUnits AS x
    LEFT JOIN dbo.psc_ScheduleStudyUnits AS p
        ON p.ScheduleStudyUnitID = x.ScheduleStudyUnitID

    UNION ALL

    SELECT
        N'Lich Sch -> Khung lich tuan',
        COUNT_BIG(*),
        SUM(CASE WHEN p.WeekScheduleID IS NULL THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END)
    FROM dbo.psc_Sch_WeekScheduleStudyUnits AS x
    LEFT JOIN dbo.psc_Sch_WeekSchedules AS p
        ON p.WeekScheduleID = x.WeekScheduleID

    UNION ALL

    SELECT
        N'Giang vien LHP -> Lop hoc phan',
        COUNT_BIG(*),
        SUM(CASE WHEN p.ScheduleStudyUnitID IS NULL THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END)
    FROM dbo.psc_ProfessorScheduleStudyUnits AS x
    LEFT JOIN dbo.psc_ScheduleStudyUnits AS p
        ON p.ScheduleStudyUnitID = x.ScheduleStudyUnitID

    UNION ALL

    SELECT
        N'Giang vien LHP -> Giang vien',
        COUNT_BIG(*),
        SUM(CASE WHEN p.ProfessorID IS NULL THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END)
    FROM dbo.psc_ProfessorScheduleStudyUnits AS x
    LEFT JOIN dbo.psc_Professors AS p ON p.ProfessorID = x.ProfessorID

    UNION ALL

    SELECT
        N'Tai khoan mapping -> Tai khoan',
        COUNT_BIG(*),
        SUM(CASE WHEN p.AccountID IS NULL THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END)
    FROM dbo.psc_AccountMappings AS x
    LEFT JOIN dbo.psc_Accounts AS p ON p.AccountID = x.AccountID

    UNION ALL

    SELECT
        N'Giao dich -> Tai khoan',
        COUNT_BIG(*),
        SUM(CASE WHEN p.AccountID IS NULL THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END)
    FROM dbo.psc_Transactions AS x
    LEFT JOIN dbo.psc_Accounts AS p ON p.AccountID = x.AccountID

    UNION ALL

    SELECT
        N'Giao dich -> Hoa don',
        COUNT_BIG(*),
        SUM(CASE WHEN x.BillID IS NOT NULL AND p.BillID IS NULL THEN CONVERT(bigint, 1) ELSE CONVERT(bigint, 0) END)
    FROM dbo.psc_Transactions AS x
    LEFT JOIN dbo.psc_Bills AS p ON p.BillID = x.BillID
) AS coverage
ORDER BY
    CASE WHEN MissingParentRows > 0 THEN 0 ELSE 1 END,
    MissingParentRows DESC,
    RelationshipName;

