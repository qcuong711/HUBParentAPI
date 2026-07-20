/*
    02_student_data_check.sql
    Muc dich: kiem tra du lieu thuc te cho MOT sinh vien.

    Cach dung:
      1. Sua @StudentID.
      2. De @YearStudy/@TermID = NULL de tu tim hoc ky bao phu ngay hien tai.
      3. Neu khong tim thay hoc ky hien tai, dien truc tiep @YearStudy va @TermID.

    Script chi SELECT, khong cap nhat du lieu.
*/

SET NOCOUNT ON;

DECLARE @StudentID varchar(20) = 'THAY_MA_SINH_VIEN';
DECLARE @YearStudy varchar(20) = NULL; -- Vi du: '2025-2026'
DECLARE @TermID varchar(20) = NULL;    -- Vi du: 'HK01'

IF @StudentID = 'THAY_MA_SINH_VIEN'
BEGIN
    RAISERROR(N'Hay sua bien @StudentID truoc khi chay script.', 16, 1);
    RETURN;
END;

-- Tu dong nhan dien hoc ky hien tai neu chua truyen du.
IF @YearStudy IS NULL OR @TermID IS NULL
BEGIN
    SELECT TOP (1)
        @YearStudy = t.YearStudy,
        @TermID = t.TermID
    FROM dbo.psc_Terms AS t
    WHERE CAST(GETDATE() AS date)
          BETWEEN CAST(t.BeginDate AS date) AND CAST(t.EndDate AS date)
    ORDER BY t.BeginDate DESC, t.OrderTerm DESC;
END;

SELECT
    @StudentID AS StudentID,
    @YearStudy AS SelectedYearStudy,
    @TermID AS SelectedTermID,
    CASE
        WHEN @YearStudy IS NULL OR @TermID IS NULL
            THEN N'Khong tim thay hoc ky hien tai; hay gan @YearStudy va @TermID thu cong.'
        ELSE N'Da xac dinh hoc ky can kiem tra.'
    END AS CheckMessage;

/* 1. Kiem tra sinh vien co ton tai. */
SELECT
    s.StudentID,
    s.LastName,
    s.MiddleName,
    s.FirstName,
    s.StudentTypeID,
    s.StudyStatusID,
    s.FeeByStudyUnit,
    s.IncomeFeeObjectID
FROM dbo.psc_Students AS s
WHERE s.StudentID = @StudentID;

/* 2. Hoc phan/lop hoc phan sinh vien da dang ky trong hoc ky. */
SELECT
    sssu.StudentID,
    su.YearStudy,
    su.TermID,
    sssu.ScheduleStudyUnitID,
    ssu.ScheduleStudyUnitAlias,
    ssu.StudyUnitID,
    su.StudyUnitAlias,
    su.CurriculumID,
    c.CurriculumName,
    c.Credits,
    c.TheoryCredits,
    c.PracticeCredits,
    sssu.RegistDate,
    sssu.RegistStatus,
    sssu.RegistType,
    sssu.RegistBy,
    sssu.FeeStatus,
    sssu.FeeType,
    sssu.IsFee,
    sssu.KhongTinhPhi,
    sssu.TransactionID,
    su.FeeRate AS StudyUnitFeeRate,
    ssu.FeeRate AS ScheduleStudyUnitFeeRate,
    ssu.StartDate,
    ssu.EndDate,
    ssu.Status AS ScheduleStatus
FROM dbo.psc_StudentScheduleStudyUnits AS sssu
INNER JOIN dbo.psc_ScheduleStudyUnits AS ssu
    ON ssu.ScheduleStudyUnitID = sssu.ScheduleStudyUnitID
INNER JOIN dbo.psc_StudyUnits AS su
    ON su.StudyUnitID = ssu.StudyUnitID
LEFT JOIN dbo.psc_Curriculums AS c
    ON c.CurriculumID = su.CurriculumID
WHERE sssu.StudentID = @StudentID
  AND (@YearStudy IS NULL OR su.YearStudy = @YearStudy)
  AND (@TermID IS NULL OR su.TermID = @TermID)
ORDER BY su.YearStudy, su.TermID, c.CurriculumName, sssu.ScheduleStudyUnitID;

/* 3. Chuong trinh dao tao sinh vien da dang ky. */
SELECT
    ssp.StudentID,
    ssp.StudyProgramID,
    sp.StudyProgramName,
    sp.StudyProgramEngName,
    sp.StudyTypeID,
    sp.GraduateLevelID,
    sp.CourseID,
    sp.OlogyID,
    o.OlogyName,
    ssp.SpecializationID,
    ssp.Type,
    ssp.StudyStatus,
    sp.Credits AS ProgramCredits,
    sp.SelectionCredits,
    sp.BeginDate,
    sp.Status AS ProgramStatus
FROM dbo.psc1_StudentStudyPrograms AS ssp
INNER JOIN dbo.psc1_StudyPrograms AS sp
    ON sp.StudyProgramID = ssp.StudyProgramID
LEFT JOIN dbo.psc_Ologies AS o
    ON o.OlogyID = sp.OlogyID
WHERE ssp.StudentID = @StudentID
ORDER BY ssp.StudyProgramID;

/* 4. Toan bo mon thuoc chuong trinh dao tao cua sinh vien. */
SELECT
    ssp.StudentID,
    sp.StudyProgramID,
    sp.StudyProgramName,
    csp.CurriculumStudyProgramID,
    csp.CurriculumID,
    c.CurriculumName,
    c.Credits,
    csp.CurriculumType,
    csp.StudyPartID,
    csp.SpecializationID,
    csp.RankIndex,
    csp.IsNotStudied,
    plan.SemesterID,
    sem.SemesterName,
    plan.StudiedSemesterID
FROM dbo.psc1_StudentStudyPrograms AS ssp
INNER JOIN dbo.psc1_StudyPrograms AS sp
    ON sp.StudyProgramID = ssp.StudyProgramID
INNER JOIN dbo.psc1_CurriculumStudyPrograms AS csp
    ON csp.StudyProgramID = sp.StudyProgramID
LEFT JOIN dbo.psc_Curriculums AS c
    ON c.CurriculumID = csp.CurriculumID
LEFT JOIN dbo.psc1_StudyPlans AS plan
    ON plan.CurriculumStudyProgramID = csp.CurriculumStudyProgramID
LEFT JOIN dbo.psc_Semesters AS sem
    ON sem.SemesterID = plan.SemesterID
WHERE ssp.StudentID = @StudentID
ORDER BY sp.StudyProgramID, sem.Rank, csp.RankIndex, c.CurriculumName;

/* 5. Ke hoach dao tao theo nam hoc/hoc ky. */
SELECT
    ssp.StudentID,
    p.StudyProgramID,
    p.YearStudy,
    p.TermID,
    p.CurriculumID,
    c.CurriculumName,
    c.Credits,
    p.IsLocked,
    p.Note
FROM dbo.psc1_StudentStudyPrograms AS ssp
INNER JOIN dbo.psc_StudyPlanByTerms AS p
    ON p.StudyProgramID = ssp.StudyProgramID
LEFT JOIN dbo.psc_Curriculums AS c
    ON c.CurriculumID = p.CurriculumID
WHERE ssp.StudentID = @StudentID
  AND (@YearStudy IS NULL OR p.YearStudy = @YearStudy)
  AND (@TermID IS NULL OR p.TermID = @TermID)
ORDER BY p.YearStudy, p.TermID, c.CurriculumName;

/* 6. Trang thai hoc tap/chuyen lop/chuyen CTDT theo hoc ky. */
SELECT *
FROM dbo.psc_StudentStudyStatus
WHERE StudentID = @StudentID
  AND (@YearStudy IS NULL OR YearStudy = @YearStudy)
  AND (@TermID IS NULL OR TermID = @TermID)
ORDER BY YearStudy, TermID, UpdateDate DESC;

/* 7. Cau hinh don gia tin chi phu hop voi CTDT cua sinh vien. */
SELECT DISTINCT
    ssp.StudentID,
    sp.StudyProgramID,
    cf.ID AS CreditFeeID,
    cf.GraduateLevelID,
    cf.StudyTypeID,
    cf.YearStudy,
    cf.TermID,
    cf.Year,
    cf.StudyUnitTypeID,
    cf.CreditFee,
    cf.SecondaryCreditFee,
    cf.GDQP,
    cf.GDTC,
    cf.LoaiSinhVien
FROM dbo.psc1_StudentStudyPrograms AS ssp
INNER JOIN dbo.psc1_StudyPrograms AS sp
    ON sp.StudyProgramID = ssp.StudyProgramID
INNER JOIN dbo.psc_CreditFees AS cf
    ON cf.GraduateLevelID = sp.GraduateLevelID
   AND cf.StudyTypeID = sp.StudyTypeID
WHERE ssp.StudentID = @StudentID
  AND (@YearStudy IS NULL OR cf.YearStudy = @YearStudy)
  AND (@TermID IS NULL OR cf.TermID = @TermID)
ORDER BY cf.YearStudy, cf.TermID, cf.StudyUnitTypeID;

/* 8. Giao dich gan voi tung dang ky hoc phan.
      Schema khong khai bao FK cho TransactionID, nen hien thi ca hai nguon ung vien. */
SELECT
    sssu.StudentID,
    su.YearStudy,
    su.TermID,
    sssu.ScheduleStudyUnitID,
    c.CurriculumName,
    sssu.TransactionID,
    st.Amount AS StudentTransactionAmount,
    st.FeeRateID AS StudentTransactionFeeRateID,
    st.FeeDetail AS StudentTransactionFeeDetail,
    tr.AccountID,
    tr.Amount AS AccountTransactionAmount,
    tr.AmountEx AS AccountTransactionAmountEx,
    tr.FeeDetail AS AccountTransactionFeeDetail,
    tr.Descriptions AS AccountTransactionDescription,
    tr.BillID
FROM dbo.psc_StudentScheduleStudyUnits AS sssu
INNER JOIN dbo.psc_ScheduleStudyUnits AS ssu
    ON ssu.ScheduleStudyUnitID = sssu.ScheduleStudyUnitID
INNER JOIN dbo.psc_StudyUnits AS su
    ON su.StudyUnitID = ssu.StudyUnitID
LEFT JOIN dbo.psc_Curriculums AS c
    ON c.CurriculumID = su.CurriculumID
LEFT JOIN dbo.psc_StudentTransactions AS st
    ON st.TransactionID = sssu.TransactionID
   AND st.StudentID = sssu.StudentID
LEFT JOIN dbo.psc_Transactions AS tr
    ON tr.TransactionID = sssu.TransactionID
WHERE sssu.StudentID = @StudentID
  AND (@YearStudy IS NULL OR su.YearStudy = @YearStudy)
  AND (@TermID IS NULL OR su.TermID = @TermID)
ORDER BY su.YearStudy, su.TermID, sssu.ScheduleStudyUnitID;

/* 9. Toan bo giao dich tai khoan cua sinh vien (phan he ke toan). */
SELECT
    am.PersonID AS StudentID,
    a.AccountID,
    a.AccountNumber,
    a.AccountName,
    tr.TransactionID,
    tr.FeeDetail,
    tr.FeeDetailTypeID,
    tr.Amount,
    tr.AmountEx,
    tr.Descriptions,
    tr.UpdateDate,
    tr.BillID,
    b.BillNumber,
    b.BillDate,
    b.Amount AS BillAmount
FROM dbo.psc_AccountMappings AS am
INNER JOIN dbo.psc_Accounts AS a
    ON a.AccountID = am.AccountID
LEFT JOIN dbo.psc_Transactions AS tr
    ON tr.AccountID = a.AccountID
LEFT JOIN dbo.psc_Bills AS b
    ON b.BillID = tr.BillID
WHERE am.PersonID = @StudentID
ORDER BY tr.UpdateDate DESC, tr.TransactionID DESC;

/* 10. TKB - phan he psc_WeekSchedules (lien ket truc tiep LHP). */
SELECT
    sssu.StudentID,
    su.YearStudy,
    su.TermID,
    ssu.ScheduleStudyUnitID,
    ssu.ScheduleStudyUnitAlias,
    c.CurriculumName,
    ws.Year,
    ws.Week,
    ws.DayOfWeek,
    ws.PeriodID,
    p.PeriodName,
    p.BeginTime,
    p.EndTime,
    ws.NumberOfPeriods,
    ws.RoomID,
    r.RoomName,
    psu.ProfessorID,
    LTRIM(RTRIM(
        COALESCE(pr.LastName, N'') + N' ' +
        COALESCE(pr.MiddleName, N'') + N' ' +
        COALESCE(pr.FirstName, N'')
    )) AS ProfessorName,
    ws.Status,
    ws.UnitContent
FROM dbo.psc_StudentScheduleStudyUnits AS sssu
INNER JOIN dbo.psc_ScheduleStudyUnits AS ssu
    ON ssu.ScheduleStudyUnitID = sssu.ScheduleStudyUnitID
INNER JOIN dbo.psc_StudyUnits AS su
    ON su.StudyUnitID = ssu.StudyUnitID
LEFT JOIN dbo.psc_Curriculums AS c
    ON c.CurriculumID = su.CurriculumID
INNER JOIN dbo.psc_WeekSchedules AS ws
    ON ws.ScheduleStudyUnitID = sssu.ScheduleStudyUnitID
LEFT JOIN dbo.psc_Periods AS p
    ON p.PeriodID = ws.PeriodID
LEFT JOIN dbo.psc_Rooms AS r
    ON r.RoomID = ws.RoomID
LEFT JOIN dbo.psc_ProfessorScheduleStudyUnits AS psu
    ON psu.ScheduleStudyUnitID = sssu.ScheduleStudyUnitID
LEFT JOIN dbo.psc_Professors AS pr
    ON pr.ProfessorID = psu.ProfessorID
WHERE sssu.StudentID = @StudentID
  AND (@YearStudy IS NULL OR su.YearStudy = @YearStudy)
  AND (@TermID IS NULL OR su.TermID = @TermID)
ORDER BY ws.Year, ws.Week, ws.DayOfWeek, ws.PeriodID, c.CurriculumName;

/* 11. TKB - phan he moi psc_Sch_*.
       Chay de so sanh phan he nao dang co du lieu thuc te. */
SELECT
    sssu.StudentID,
    su.YearStudy,
    su.TermID,
    sssu.ScheduleStudyUnitID,
    c.CurriculumName,
    sws.Year,
    sws.Week,
    sws.DayOfWeek,
    sws.PeriodID,
    p.PeriodName,
    sws.BeginTime,
    sws.EndTime,
    sws.NumberOfPeriods,
    sws.RoomID,
    r.RoomName,
    sws.Status,
    swsu.UnitContent,
    swsu.StudyTimeID
FROM dbo.psc_StudentScheduleStudyUnits AS sssu
INNER JOIN dbo.psc_ScheduleStudyUnits AS ssu
    ON ssu.ScheduleStudyUnitID = sssu.ScheduleStudyUnitID
INNER JOIN dbo.psc_StudyUnits AS su
    ON su.StudyUnitID = ssu.StudyUnitID
LEFT JOIN dbo.psc_Curriculums AS c
    ON c.CurriculumID = su.CurriculumID
INNER JOIN dbo.psc_Sch_WeekScheduleStudyUnits AS swsu
    ON swsu.ScheduleStudyUnitID = sssu.ScheduleStudyUnitID
INNER JOIN dbo.psc_Sch_WeekSchedules AS sws
    ON sws.WeekScheduleID = swsu.WeekScheduleID
LEFT JOIN dbo.psc_Periods AS p
    ON p.PeriodID = sws.PeriodID
LEFT JOIN dbo.psc_Rooms AS r
    ON r.RoomID = sws.RoomID
WHERE sssu.StudentID = @StudentID
  AND (@YearStudy IS NULL OR su.YearStudy = @YearStudy)
  AND (@TermID IS NULL OR su.TermID = @TermID)
ORDER BY sws.Year, sws.Week, sws.DayOfWeek, sws.PeriodID, c.CurriculumName;

