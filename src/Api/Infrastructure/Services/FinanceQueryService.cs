using System.Data;
using System.Data.Common;
using Api.Application;
using Api.Application.Dtos;
using Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Services;

public class FinanceQueryService : IFinanceQueryService
{
    private readonly AppDbContext _db;

    public FinanceQueryService(AppDbContext db)
    {
        _db = db;
    }

    public Task<IReadOnlyList<TuitionTermSummaryDto>> GetTuitionTermsAsync(
        string studentCode,
        string? yearStudy,
        string? termId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT d.StudentId,
                   d.YearStudy,
                   d.TermID,
                   SUM(ISNULL(d.SumAmountCurr, 0)) AS TuitionAmount,
                   SUM(ISNULL(d.SumPaidAmount, 0)) AS PaidAmount,
                   SUM(ISNULL(d.SumPaidLeft, 0)) AS OffsetAmount,
                   SUM(ISNULL(d.SumAmountEx, 0) + ISNULL(d.SumAmountExDiff, 0)) AS DiscountAmount,
                   CASE
                       WHEN SUM(ISNULL(d.SumAmountCurr, 0)
                              - ISNULL(d.SumPaidAmount, 0)
                              - ISNULL(d.SumPaidLeft, 0)
                              - ISNULL(d.SumAmountEx, 0)
                              - ISNULL(d.SumAmountExDiff, 0)) > 0
                       THEN SUM(ISNULL(d.SumAmountCurr, 0)
                              - ISNULL(d.SumPaidAmount, 0)
                              - ISNULL(d.SumPaidLeft, 0)
                              - ISNULL(d.SumAmountEx, 0)
                              - ISNULL(d.SumAmountExDiff, 0))
                       ELSE 0
                   END AS RemainingAmount
            FROM [AccountsFee].[dbo].[tblTransactionDebts] d WITH (NOLOCK)
            INNER JOIN [AccountsFee].[dbo].[tblFeeDetailTypes] ft WITH (NOLOCK)
                ON ft.FeeDetailTypeID = d.FeeDetailTypeID
               AND ISNULL(ft.IsFeeSemester, 0) = 1
            WHERE d.StudentId = @StudentCode
              AND d.Locked <> 1
              AND (@YearStudy IS NULL OR d.YearStudy = @YearStudy)
              AND (@TermID IS NULL OR d.TermID = @TermID)
            GROUP BY d.StudentId, d.YearStudy, d.TermID
            ORDER BY d.YearStudy DESC, d.TermID DESC;
            """;

        return QueryAsync(
            sql,
            command => AddCommonParameters(command, studentCode, yearStudy, termId),
            reader =>
            {
                var remaining = GetDecimal(reader, "RemainingAmount");
                return new TuitionTermSummaryDto
                {
                    StudentID = GetString(reader, "StudentId")!,
                    YearStudy = GetString(reader, "YearStudy"),
                    TermID = GetString(reader, "TermID"),
                    TuitionAmount = GetDecimal(reader, "TuitionAmount"),
                    PaidAmount = GetDecimal(reader, "PaidAmount"),
                    OffsetAmount = GetDecimal(reader, "OffsetAmount"),
                    DiscountAmount = GetDecimal(reader, "DiscountAmount"),
                    RemainingAmount = remaining,
                    IsFullyPaid = remaining <= 0
                };
            },
            cancellationToken);
    }

    public Task<IReadOnlyList<TuitionCourseDetailDto>> GetTuitionCoursesAsync(
        string studentCode,
        string? yearStudy,
        string? termId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT d.TransactionID,
                   d.StudentId,
                   d.YearStudy,
                   d.TermID,
                   d.ScheduleStudyUnitID,
                   su.StudyUnitID,
                   c.CurriculumID,
                   c.CurriculumName,
                   COALESCE(NULLIF(ISNULL(d.CreditsThe, 0) + ISNULL(d.CreditsPra, 0), 0), c.Credits) AS Credits,
                   d.FeeDetailTypeID,
                   ft.FeeDetailTypeName,
                   d.Descriptions,
                   ISNULL(d.SumAmountCurr, 0) AS TuitionAmount,
                   ISNULL(d.SumPaidAmount, 0) AS PaidAmount,
                   ISNULL(d.SumPaidLeft, 0) AS OffsetAmount,
                   ISNULL(d.SumAmountEx, 0) + ISNULL(d.SumAmountExDiff, 0) AS DiscountAmount,
                   CASE
                       WHEN ISNULL(d.SumAmountCurr, 0)
                            - ISNULL(d.SumPaidAmount, 0)
                            - ISNULL(d.SumPaidLeft, 0)
                            - ISNULL(d.SumAmountEx, 0)
                            - ISNULL(d.SumAmountExDiff, 0) > 0
                       THEN ISNULL(d.SumAmountCurr, 0)
                            - ISNULL(d.SumPaidAmount, 0)
                            - ISNULL(d.SumPaidLeft, 0)
                            - ISNULL(d.SumAmountEx, 0)
                            - ISNULL(d.SumAmountExDiff, 0)
                       ELSE 0
                   END AS RemainingAmount
            FROM [AccountsFee].[dbo].[tblTransactionDebts] d WITH (NOLOCK)
            INNER JOIN [AccountsFee].[dbo].[tblFeeDetailTypes] ft WITH (NOLOCK)
                ON ft.FeeDetailTypeID = d.FeeDetailTypeID
               AND ISNULL(ft.IsFeeSemester, 0) = 1
            LEFT JOIN dbo.psc_ScheduleStudyUnits s WITH (NOLOCK)
                ON s.ScheduleStudyUnitID = d.ScheduleStudyUnitID
            LEFT JOIN dbo.psc_StudyUnits su WITH (NOLOCK)
                ON su.StudyUnitID = s.StudyUnitID
            LEFT JOIN dbo.psc_Curriculums c WITH (NOLOCK)
                ON c.CurriculumID = su.CurriculumID
            WHERE d.StudentId = @StudentCode
              AND d.Locked <> 1
              AND (@YearStudy IS NULL OR d.YearStudy = @YearStudy)
              AND (@TermID IS NULL OR d.TermID = @TermID)
            ORDER BY d.YearStudy DESC, d.TermID DESC, c.CurriculumName, d.TransactionID;
            """;

        return QueryAsync(
            sql,
            command => AddCommonParameters(command, studentCode, yearStudy, termId),
            reader =>
            {
                var remaining = GetDecimal(reader, "RemainingAmount");
                var scheduleStudyUnitId = GetString(reader, "ScheduleStudyUnitID");
                return new TuitionCourseDetailDto
                {
                    TransactionID = GetInt64(reader, "TransactionID"),
                    StudentID = GetString(reader, "StudentId")!,
                    YearStudy = GetString(reader, "YearStudy"),
                    TermID = GetString(reader, "TermID"),
                    ScheduleStudyUnitID = scheduleStudyUnitId,
                    StudyUnitID = GetString(reader, "StudyUnitID"),
                    CurriculumID = GetString(reader, "CurriculumID"),
                    CurriculumName = GetString(reader, "CurriculumName"),
                    Credits = GetNullableDecimal(reader, "Credits"),
                    FeeDetailTypeID = GetNullableInt32(reader, "FeeDetailTypeID"),
                    FeeDetailTypeName = GetString(reader, "FeeDetailTypeName"),
                    Description = GetString(reader, "Descriptions"),
                    TuitionAmount = GetDecimal(reader, "TuitionAmount"),
                    PaidAmount = GetDecimal(reader, "PaidAmount"),
                    OffsetAmount = GetDecimal(reader, "OffsetAmount"),
                    DiscountAmount = GetDecimal(reader, "DiscountAmount"),
                    RemainingAmount = remaining,
                    IsTermLevel = string.IsNullOrWhiteSpace(scheduleStudyUnitId),
                    IsFullyPaid = remaining <= 0
                };
            },
            cancellationToken);
    }

    public Task<IReadOnlyList<OtherFeeDto>> GetOtherFeesAsync(
        string studentCode,
        string? yearStudy,
        string? termId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT d.TransactionID,
                   d.StudentId,
                   d.YearStudy,
                   d.TermID,
                   d.FeeDetailTypeID,
                   ft.FeeDetailTypeName,
                   d.Descriptions,
                   ISNULL(d.SumAmountCurr, 0) AS FeeAmount,
                   ISNULL(d.SumPaidAmount, 0) AS PaidAmount,
                   ISNULL(d.SumPaidLeft, 0) AS OffsetAmount,
                   ISNULL(d.SumAmountEx, 0) + ISNULL(d.SumAmountExDiff, 0) AS DiscountAmount,
                   CASE
                       WHEN ISNULL(d.SumAmountCurr, 0)
                            - ISNULL(d.SumPaidAmount, 0)
                            - ISNULL(d.SumPaidLeft, 0)
                            - ISNULL(d.SumAmountEx, 0)
                            - ISNULL(d.SumAmountExDiff, 0) > 0
                       THEN ISNULL(d.SumAmountCurr, 0)
                            - ISNULL(d.SumPaidAmount, 0)
                            - ISNULL(d.SumPaidLeft, 0)
                            - ISNULL(d.SumAmountEx, 0)
                            - ISNULL(d.SumAmountExDiff, 0)
                       ELSE 0
                   END AS RemainingAmount
            FROM [AccountsFee].[dbo].[tblTransactionDebts] d WITH (NOLOCK)
            INNER JOIN [AccountsFee].[dbo].[tblFeeDetailTypes] ft WITH (NOLOCK)
                ON ft.FeeDetailTypeID = d.FeeDetailTypeID
               AND ISNULL(ft.IsFeeSemester, 0) = 0
               AND ISNULL(ft.LePhiNgoaiTruong, 0) = 0
            WHERE d.StudentId = @StudentCode
              AND d.Locked <> 1
              AND (@YearStudy IS NULL OR d.YearStudy = @YearStudy)
              AND (@TermID IS NULL OR d.TermID = @TermID)
            ORDER BY d.YearStudy DESC, d.TermID DESC, ft.OrderNumber, d.TransactionID;
            """;

        return QueryAsync(
            sql,
            command => AddCommonParameters(command, studentCode, yearStudy, termId),
            reader =>
            {
                var remaining = GetDecimal(reader, "RemainingAmount");
                return new OtherFeeDto
                {
                    TransactionID = GetInt64(reader, "TransactionID"),
                    StudentID = GetString(reader, "StudentId")!,
                    YearStudy = GetString(reader, "YearStudy"),
                    TermID = GetString(reader, "TermID"),
                    FeeDetailTypeID = GetNullableInt32(reader, "FeeDetailTypeID"),
                    FeeDetailTypeName = GetString(reader, "FeeDetailTypeName"),
                    Description = GetString(reader, "Descriptions"),
                    FeeAmount = GetDecimal(reader, "FeeAmount"),
                    PaidAmount = GetDecimal(reader, "PaidAmount"),
                    OffsetAmount = GetDecimal(reader, "OffsetAmount"),
                    DiscountAmount = GetDecimal(reader, "DiscountAmount"),
                    RemainingAmount = remaining,
                    IsFullyPaid = remaining <= 0
                };
            },
            cancellationToken);
    }

    public Task<IReadOnlyList<PaymentReceiptDto>> GetPaymentsAsync(
        string studentCode,
        string? yearStudy,
        string? termId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT b.BillID,
                   b.StudentId,
                   b.BillNumber,
                   b.BillSeries,
                   b.BillDate,
                   b.YearStudy,
                   b.TermID,
                   ISNULL(b.Amount, 0) AS Amount,
                   ISNULL(b.AmountEx, 0) + ISNULL(b.AmountExDiff, 0) AS DiscountAmount,
                   b.HinhThucThanhToan AS PaymentMethodID,
                   b.Descriptions,
                   b.NoiDungThu,
                   b.SoHoaDonEinvoice,
                   b.LinkChuyenDoi
            FROM [AccountsFee].[dbo].[tblBills] b WITH (NOLOCK)
            WHERE b.StudentId = @StudentCode
              AND ISNULL(b.BillTypeID, 0) NOT IN (5, 7, 8)
              AND b.BillIDRut IS NULL
              AND NOT EXISTS
                  (SELECT 1
                   FROM [AccountsFee].[dbo].[tblBillNumberDels] deleted WITH (NOLOCK)
                   WHERE deleted.BillID = b.BillID)
              AND (@YearStudy IS NULL OR b.YearStudy = @YearStudy)
              AND (@TermID IS NULL OR b.TermID = @TermID)
            ORDER BY b.BillDate DESC, b.BillID DESC;
            """;

        return QueryAsync(
            sql,
            command => AddCommonParameters(command, studentCode, yearStudy, termId),
            reader => new PaymentReceiptDto
            {
                BillID = GetInt32(reader, "BillID"),
                StudentID = GetString(reader, "StudentId")!,
                BillNumber = GetString(reader, "BillNumber"),
                BillSeries = GetString(reader, "BillSeries"),
                BillDate = GetNullableDateTime(reader, "BillDate"),
                YearStudy = GetString(reader, "YearStudy"),
                TermID = GetString(reader, "TermID"),
                Amount = GetDecimal(reader, "Amount"),
                DiscountAmount = GetDecimal(reader, "DiscountAmount"),
                PaymentMethodID = GetNullableInt32(reader, "PaymentMethodID"),
                Description = GetString(reader, "Descriptions"),
                CollectionContent = GetString(reader, "NoiDungThu"),
                EInvoiceNumber = GetString(reader, "SoHoaDonEinvoice"),
                EInvoiceUrl = GetString(reader, "LinkChuyenDoi")
            },
            cancellationToken);
    }

    public Task<IReadOnlyList<OutstandingDebtDto>> GetDebtsAsync(
        string studentCode,
        string? yearStudy,
        string? termId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT d.TransactionID,
                   d.StudentId,
                   d.YearStudy,
                   d.TermID,
                   d.ScheduleStudyUnitID,
                   c.CurriculumID,
                   c.CurriculumName,
                   d.FeeDetailTypeID,
                   ft.FeeDetailTypeName,
                   d.Descriptions,
                   CONVERT(bit, ISNULL(ft.IsFeeSemester, 0)) AS IsTuition,
                   ISNULL(d.SumAmountCurr, 0) AS AmountDue,
                   ISNULL(d.SumPaidAmount, 0) AS PaidAmount,
                   ISNULL(d.SumPaidLeft, 0) AS OffsetAmount,
                   ISNULL(d.SumAmountEx, 0) + ISNULL(d.SumAmountExDiff, 0) AS DiscountAmount,
                   ISNULL(d.SumAmountCurr, 0)
                       - ISNULL(d.SumPaidAmount, 0)
                       - ISNULL(d.SumPaidLeft, 0)
                       - ISNULL(d.SumAmountEx, 0)
                       - ISNULL(d.SumAmountExDiff, 0) AS RemainingAmount
            FROM [AccountsFee].[dbo].[tblTransactionDebts] d WITH (NOLOCK)
            INNER JOIN [AccountsFee].[dbo].[tblFeeDetailTypes] ft WITH (NOLOCK)
                ON ft.FeeDetailTypeID = d.FeeDetailTypeID
               AND ISNULL(ft.LePhiNgoaiTruong, 0) = 0
            LEFT JOIN dbo.psc_ScheduleStudyUnits s WITH (NOLOCK)
                ON s.ScheduleStudyUnitID = d.ScheduleStudyUnitID
            LEFT JOIN dbo.psc_StudyUnits su WITH (NOLOCK)
                ON su.StudyUnitID = s.StudyUnitID
            LEFT JOIN dbo.psc_Curriculums c WITH (NOLOCK)
                ON c.CurriculumID = su.CurriculumID
            WHERE d.StudentId = @StudentCode
              AND d.Locked <> 1
              AND ISNULL(d.SumAmountCurr, 0)
                    - ISNULL(d.SumPaidAmount, 0)
                    - ISNULL(d.SumPaidLeft, 0)
                    - ISNULL(d.SumAmountEx, 0)
                    - ISNULL(d.SumAmountExDiff, 0) > 0
              AND (@YearStudy IS NULL OR d.YearStudy = @YearStudy)
              AND (@TermID IS NULL OR d.TermID = @TermID)
            ORDER BY d.YearStudy DESC, d.TermID DESC, ft.OrderNumber, c.CurriculumName;
            """;

        return QueryAsync(
            sql,
            command => AddCommonParameters(command, studentCode, yearStudy, termId),
            reader => new OutstandingDebtDto
            {
                TransactionID = GetInt64(reader, "TransactionID"),
                StudentID = GetString(reader, "StudentId")!,
                YearStudy = GetString(reader, "YearStudy"),
                TermID = GetString(reader, "TermID"),
                ScheduleStudyUnitID = GetString(reader, "ScheduleStudyUnitID"),
                CurriculumID = GetString(reader, "CurriculumID"),
                CurriculumName = GetString(reader, "CurriculumName"),
                FeeDetailTypeID = GetNullableInt32(reader, "FeeDetailTypeID"),
                FeeDetailTypeName = GetString(reader, "FeeDetailTypeName"),
                Description = GetString(reader, "Descriptions"),
                IsTuition = GetBoolean(reader, "IsTuition"),
                AmountDue = GetDecimal(reader, "AmountDue"),
                PaidAmount = GetDecimal(reader, "PaidAmount"),
                OffsetAmount = GetDecimal(reader, "OffsetAmount"),
                DiscountAmount = GetDecimal(reader, "DiscountAmount"),
                RemainingAmount = GetDecimal(reader, "RemainingAmount")
            },
            cancellationToken);
    }

    public Task<IReadOnlyList<TuitionDeadlineDto>> GetDeadlinesAsync(
        string studentCode,
        string? yearStudy,
        string? termId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            WITH StudentCourses AS
            (
                SELECT DISTINCT program.CourseID
                FROM dbo.psc1_StudentStudyPrograms studentProgram WITH (NOLOCK)
                INNER JOIN dbo.psc1_StudyPrograms program WITH (NOLOCK)
                    ON program.StudyProgramID = studentProgram.StudyProgramID
                WHERE studentProgram.StudentID = @StudentCode
                  AND ISNULL(studentProgram.Type, 1) = 1
            )
            SELECT deadline.Id AS DeadlineID,
                   deadline.YearStudy,
                   deadline.TermId,
                   deadline.CourseId,
                   deadline.LoaiPhi,
                   deadline.Lan,
                   deadline.TimeName,
                   deadline.BeginDate,
                   deadline.EndDate,
                   COALESCE(deadline.HanCuoi, deadline.EndDate) AS OriginalDeadline,
                   extension.NgayBatDau AS ExtensionStartDate,
                   extension.NgayHetHan AS ExtensionEndDate,
                   extension.LyDo AS ExtensionReason,
                   COALESCE(extension.NgayHetHan,
                            deadline.NgayGiaHan,
                            deadline.HanCuoi,
                            deadline.EndDate) AS EffectiveDeadline
            FROM [AccountsFee].[dbo].[tblHanCuoi] deadline WITH (NOLOCK)
            OUTER APPLY
            (
                SELECT TOP (1)
                       personal.NgayBatDau,
                       personal.NgayHetHan,
                       personal.LyDo
                FROM [AccountsFee].[dbo].[tblGiaHan] personal WITH (NOLOCK)
                WHERE personal.StudentId = @StudentCode
                  AND personal.NamHoc = deadline.YearStudy
                  AND personal.HocKy = deadline.TermId
                  AND personal.Locked <> 1
                  AND (personal.LoaiPhi = deadline.LoaiPhi
                       OR NULLIF(personal.LoaiPhi, '') IS NULL
                       OR NULLIF(deadline.LoaiPhi, '') IS NULL)
                ORDER BY personal.NgayHetHan DESC, personal.Id DESC
            ) extension
            WHERE deadline.Locked <> 1
              AND (@YearStudy IS NULL OR deadline.YearStudy = @YearStudy)
              AND (@TermID IS NULL OR deadline.TermId = @TermID)
              AND (NULLIF(deadline.CourseId, '') IS NULL
                   OR EXISTS (SELECT 1 FROM StudentCourses course WHERE course.CourseID = deadline.CourseId))
            ORDER BY deadline.YearStudy DESC,
                     deadline.TermId DESC,
                     deadline.LoaiPhi,
                     deadline.Lan;
            """;

        return QueryAsync(
            sql,
            command => AddCommonParameters(command, studentCode, yearStudy, termId),
            reader =>
            {
                var extensionEndDate = GetNullableDateTime(reader, "ExtensionEndDate");
                var effectiveDeadline = GetNullableDateTime(reader, "EffectiveDeadline");
                return new TuitionDeadlineDto
                {
                    DeadlineID = GetInt32(reader, "DeadlineID"),
                    StudentID = studentCode,
                    YearStudy = GetString(reader, "YearStudy"),
                    TermID = GetString(reader, "TermId"),
                    CourseID = GetString(reader, "CourseId"),
                    FeeType = GetString(reader, "LoaiPhi"),
                    Round = GetNullableInt32(reader, "Lan"),
                    TimeName = GetString(reader, "TimeName"),
                    StartDate = GetNullableDateTime(reader, "BeginDate"),
                    EndDate = GetNullableDateTime(reader, "EndDate"),
                    OriginalDeadline = GetNullableDateTime(reader, "OriginalDeadline"),
                    IsExtended = extensionEndDate.HasValue,
                    ExtensionStartDate = GetNullableDateTime(reader, "ExtensionStartDate"),
                    ExtensionEndDate = extensionEndDate,
                    ExtensionReason = GetString(reader, "ExtensionReason"),
                    EffectiveDeadline = effectiveDeadline,
                    IsOverdue = effectiveDeadline.HasValue && effectiveDeadline.Value.Date < DateTime.Today
                };
            },
            cancellationToken);
    }

    private async Task<IReadOnlyList<T>> QueryAsync<T>(
        string sql,
        Action<DbCommand> configure,
        Func<DbDataReader, T> map,
        CancellationToken cancellationToken)
    {
        var connection = _db.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;

        if (shouldClose)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.CommandTimeout = 60;
            configure(command);

            var items = new List<T>();
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                items.Add(map(reader));
            }

            return items;
        }
        finally
        {
            if (shouldClose && connection.State == ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }
    }

    private static void AddCommonParameters(
        DbCommand command,
        string studentCode,
        string? yearStudy,
        string? termId)
    {
        AddParameter(command, "@StudentCode", studentCode, DbType.String, 20);
        AddParameter(command, "@YearStudy", yearStudy, DbType.String, 50);
        AddParameter(command, "@TermID", termId, DbType.String, 50);
    }

    private static void AddParameter(
        DbCommand command,
        string name,
        object? value,
        DbType dbType,
        int? size = null)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        parameter.DbType = dbType;
        if (size.HasValue)
        {
            parameter.Size = size.Value;
        }

        command.Parameters.Add(parameter);
    }

    private static string? GetString(DbDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal).Trim();
    }

    private static decimal GetDecimal(DbDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal) ? 0 : Convert.ToDecimal(reader.GetValue(ordinal));
    }

    private static decimal? GetNullableDecimal(DbDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal) ? null : Convert.ToDecimal(reader.GetValue(ordinal));
    }

    private static int GetInt32(DbDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return Convert.ToInt32(reader.GetValue(ordinal));
    }

    private static int? GetNullableInt32(DbDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal) ? null : Convert.ToInt32(reader.GetValue(ordinal));
    }

    private static long GetInt64(DbDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return Convert.ToInt64(reader.GetValue(ordinal));
    }

    private static bool GetBoolean(DbDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return !reader.IsDBNull(ordinal) && Convert.ToBoolean(reader.GetValue(ordinal));
    }

    private static DateTime? GetNullableDateTime(DbDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal) ? null : Convert.ToDateTime(reader.GetValue(ordinal));
    }
}
