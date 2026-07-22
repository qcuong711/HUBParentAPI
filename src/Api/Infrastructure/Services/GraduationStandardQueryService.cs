using System.Data;
using System.Data.Common;
using Api.Application.Dtos;
using Api.Domain.Entities;
using Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Services;

public class GraduationStandardQueryService
{
    private const byte GraduationCriteriaType = 1;

    private static readonly StandardDefinition[] StandardDefinitions =
    [
        new("NN", "Ngoại ngữ"),
        new("TH", "Tin học"),
        new("TATC", "Tiếng Anh tăng cường"),
        new("KNM", "Kỹ năng mềm"),
        new("DK", "Nghiên cứu khoa học")
    ];

    private readonly AppDbContext _db;

    public GraduationStandardQueryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<GraduationStandardsDto> GetAsync(
        string studentCode,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SET NOCOUNT ON;

            DECLARE @StudyProgramID varchar(20);

            SELECT TOP (1) @StudyProgramID = studentProgram.StudyProgramID
            FROM dbo.psc1_StudentStudyPrograms studentProgram WITH (NOLOCK)
            WHERE studentProgram.StudentID = @StudentCode
            ORDER BY
                CASE WHEN ISNULL(studentProgram.Type, 1) = 1 THEN 0 ELSE 1 END,
                studentProgram.UpdateDate DESC,
                studentProgram.StudyProgramID;

            SELECT
                @StudyProgramID AS StudyProgramID,
                program.StudyProgramName
            FROM (VALUES (1)) source(Value)
            LEFT JOIN dbo.psc1_StudyPrograms program WITH (NOLOCK)
                ON program.StudyProgramID = @StudyProgramID;

            SELECT TOP (1)
                criteria.GraduationCriteriaID,
                criteria.GraduationCriteriaName,
                criteria.IsLanguageCertificate,
                criteria.IsITCertificate,
                criteria.IsKTTiengAnhTangCuong,
                criteria.IsKyNangMem,
                criteria.IsNCKH,
                criteria.ApplyDate,
                criteria.UpdateDate
            FROM dbo.psc_GraduationCriteriaStudyPrograms criteria WITH (NOLOCK)
            WHERE criteria.StudyProgramID = @StudyProgramID
              AND criteria.GraduationCriteriaTypeID = @CriteriaType
            ORDER BY criteria.ApplyDate DESC, criteria.UpdateDate DESC;

            SELECT evidence.Code, evidence.Source, evidence.EvidenceDate
            FROM
            (
                SELECT
                    certificate.CertificateTypeID AS Code,
                    CONVERT(varchar(40), 'studentCertificate') AS Source,
                    certificate.UpdateDate AS EvidenceDate
                FROM dbo.psc_grd_StudentCertificates certificate WITH (NOLOCK)
                WHERE certificate.StudentID = @StudentCode
                  AND certificate.IsHaveCertificate = 1
                  AND certificate.CertificateTypeID IN ('NN', 'TH', 'TATC', 'KNM', 'DK')

                UNION ALL

                SELECT
                    certificate.CertificateTypeID,
                    CONVERT(varchar(40), 'languageCertificate'),
                    certificate.UpdateDate
                FROM dbo.psc_Grd_StudentCertificatesNN certificate WITH (NOLOCK)
                WHERE certificate.StudentID = @StudentCode
                  AND certificate.IsHaveCertificate = 1
                  AND certificate.CertificateTypeID = 'NN'

                UNION ALL

                SELECT
                    result.CertificateTypeID,
                    CONVERT(varchar(40), 'certificateReview'),
                    result.UpdateDate
                FROM dbo.psc_Grd_CertificateJudgeResult result WITH (NOLOCK)
                WHERE result.StudentID = @StudentCode
                  AND result.Result = 1
                  AND result.CertificateTypeID IN ('NN', 'TH', 'TATC', 'KNM', 'DK')

                UNION ALL

                SELECT
                    CONVERT(varchar(20), 'NN'),
                    CONVERT(varchar(40), 'graduationReview'),
                    result.JudgeDate
                FROM dbo.psc_GraduationResults result WITH (NOLOCK)
                WHERE result.StudentID = @StudentCode
                  AND result.IsLanguageCertificate = 1

                UNION ALL

                SELECT
                    CONVERT(varchar(20), 'TH'),
                    CONVERT(varchar(40), 'graduationReview'),
                    result.JudgeDate
                FROM dbo.psc_GraduationResults result WITH (NOLOCK)
                WHERE result.StudentID = @StudentCode
                  AND result.IsITCertificate = 1
            ) evidence;
            """;

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
            AddParameter(command, "@StudentCode", studentCode, DbType.String, 20);
            AddParameter(command, "@CriteriaType", GraduationCriteriaType, DbType.Byte);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            string? studyProgramID = null;
            string? studyProgramName = null;
            if (await reader.ReadAsync(cancellationToken))
            {
                studyProgramID = GetString(reader, "StudyProgramID");
                studyProgramName = GetString(reader, "StudyProgramName");
            }

            GraduationCriteriaStudyProgram? criteria = null;
            if (await reader.NextResultAsync(cancellationToken)
                && await reader.ReadAsync(cancellationToken))
            {
                criteria = new GraduationCriteriaStudyProgram
                {
                    GraduationCriteriaID = reader.GetInt32(reader.GetOrdinal("GraduationCriteriaID")),
                    GraduationCriteriaName = GetString(reader, "GraduationCriteriaName"),
                    GraduationCriteriaTypeID = GraduationCriteriaType,
                    StudyProgramID = studyProgramID!,
                    IsLanguageCertificate = GetNullableBoolean(reader, "IsLanguageCertificate"),
                    IsITCertificate = GetNullableBoolean(reader, "IsITCertificate"),
                    IsKTTiengAnhTangCuong = GetNullableBoolean(reader, "IsKTTiengAnhTangCuong"),
                    IsKyNangMem = GetNullableBoolean(reader, "IsKyNangMem"),
                    IsNCKH = GetNullableBoolean(reader, "IsNCKH"),
                    ApplyDate = GetDateTime(reader, "ApplyDate") ?? DateTime.MinValue,
                    UpdateDate = GetDateTime(reader, "UpdateDate")
                };
            }

            var evidenceByCode = new Dictionary<string, EvidenceAccumulator>(StringComparer.OrdinalIgnoreCase);
            if (await reader.NextResultAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    var code = GetString(reader, "Code");
                    var source = GetString(reader, "Source");
                    if (code is null || source is null)
                    {
                        continue;
                    }

                    if (!evidenceByCode.TryGetValue(code, out var evidence))
                    {
                        evidence = new EvidenceAccumulator();
                        evidenceByCode.Add(code, evidence);
                    }

                    evidence.Sources.Add(source);
                    var evidenceDate = GetDateTime(reader, "EvidenceDate");
                    if (evidenceDate > evidence.LastUpdated)
                    {
                        evidence.LastUpdated = evidenceDate;
                    }
                }
            }

            var criteriaConfigured = criteria is not null;
            var standards = StandardDefinitions
                .Select(definition => BuildStandard(definition, criteria, evidenceByCode))
                .ToList();

            return new GraduationStandardsDto
            {
                StudentID = studentCode,
                HasStudyProgram = studyProgramID is not null,
                StudyProgramID = studyProgramID,
                StudyProgramName = studyProgramName,
                CriteriaConfigured = criteriaConfigured,
                GraduationCriteriaID = criteria?.GraduationCriteriaID,
                GraduationCriteriaName = criteria?.GraduationCriteriaName,
                CriteriaApplyDate = criteriaConfigured ? criteria!.ApplyDate : null,
                CriteriaUpdateDate = criteria?.UpdateDate,
                LatestEvidenceDate = standards.Max(x => x.LastUpdated),
                Standards = standards
            };
        }
        finally
        {
            if (shouldClose && connection.State == ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }
    }

    private static GraduationStandardDto BuildStandard(
        StandardDefinition definition,
        GraduationCriteriaStudyProgram? criteria,
        IReadOnlyDictionary<string, EvidenceAccumulator> evidenceByCode)
    {
        evidenceByCode.TryGetValue(definition.Code, out var evidence);
        var achieved = evidence is not null;
        var required = GetRequired(criteria, definition.Code);

        var status = criteria is null
            ? "notConfigured"
            : required != true
                ? "notRequired"
                : achieved
                    ? "achieved"
                    : "notAchieved";

        return new GraduationStandardDto
        {
            Code = definition.Code,
            Name = definition.Name,
            Required = required,
            Achieved = achieved,
            Status = status,
            LastUpdated = evidence?.LastUpdated,
            EvidenceSources = evidence?.Sources.OrderBy(x => x).ToList() ?? []
        };
    }

    private static bool? GetRequired(GraduationCriteriaStudyProgram? criteria, string code)
    {
        if (criteria is null)
        {
            return null;
        }

        return code switch
        {
            "NN" => criteria.IsLanguageCertificate == true,
            "TH" => criteria.IsITCertificate == true,
            "TATC" => criteria.IsKTTiengAnhTangCuong == true,
            "KNM" => criteria.IsKyNangMem == true,
            "DK" => criteria.IsNCKH == true,
            _ => false
        };
    }

    private static void AddParameter(
        DbCommand command,
        string name,
        object value,
        DbType dbType,
        int? size = null)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
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

    private static bool? GetNullableBoolean(DbDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal) ? null : reader.GetBoolean(ordinal);
    }

    private static DateTime? GetDateTime(DbDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
    }

    private sealed record StandardDefinition(string Code, string Name);

    private sealed class EvidenceAccumulator
    {
        public HashSet<string> Sources { get; } = new(StringComparer.OrdinalIgnoreCase);
        public DateTime? LastUpdated { get; set; }
    }
}
