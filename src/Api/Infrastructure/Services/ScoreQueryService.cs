using System.Data;
using Microsoft.EntityFrameworkCore;
using Api.Application;
using Api.Application.Dtos;
using Api.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;

namespace Api.Infrastructure.Services;

public class ScoreQueryService : IScoreQueryService
{
    private readonly AppDbContext _db;
    public ScoreQueryService(AppDbContext db) { _db = db; }

    public async Task<IReadOnlyList<DetailedStudyUnitScoreDto>> GetStudyUnitScoresDetailedAsync(string studentCode)
    {
        var query =
            from ssu in _db.StudentStudyUnits.AsNoTracking()
            where ssu.StudentID == studentCode
            join su in _db.StudyUnits.AsNoTracking() on ssu.StudyUnitID equals su.StudyUnitID into suj
            from su in suj.DefaultIfEmpty()
            join cur in _db.Curriculums.AsNoTracking() on su.CurriculumID equals cur.CurriculumID into curj
            from cur in curj.DefaultIfEmpty()
            join sstat in _db.StudentStudyStatuses.AsNoTracking()
                on new { StudentID = studentCode, YearStudy = su.YearStudy, TermID = su.TermID }
                equals new { StudentID = sstat.StudentID, YearStudy = sstat.YearStudy, TermID = sstat.TermID } into sstatj
            from sstat in sstatj.DefaultIfEmpty()
            join cls in _db.ClassStudents.AsNoTracking() on sstat.ClassStudentID equals cls.ClassStudentID into clsj
            from cls in clsj.DefaultIfEmpty()
            join sp in _db.StudyPrograms.AsNoTracking() on sstat.StudyProgramID equals sp.StudyProgramID into spj
            from sp in spj.DefaultIfEmpty()
            join st in _db.StudyTypes.AsNoTracking() on su.StudyTypeID equals st.StudyTypeID into stj
            from st in stj.DefaultIfEmpty()
            select new DetailedStudyUnitScoreDto
            {
                StudyUnitID = ssu.StudyUnitID,
                Mark10 = ssu.Mark10,
                Mark4 = ssu.Mark4,
                MarkLetter = ssu.MarkLetter,
                StudyUnitAlias = su.StudyUnitAlias,
                YearStudy = su.YearStudy ?? sstat.YearStudy,
                TermID = su.TermID ?? sstat.TermID,
                CurriculumID = su.CurriculumID,
                CurriculumName = cur.CurriculumName,
                ClassStudentID = sstat.ClassStudentID,
                ClassStudentName = cls.ClassStudentName,
                StudyProgramID = sstat.StudyProgramID,
                StudyProgramName = sp.StudyProgramName,
                StudyTypeID = su.StudyTypeID,
                StudyTypeName = st.StudyTypeName
            };
        try
        {
            var items = await query.ToListAsync();
            return items;
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"Lỗi schema hoặc cột không khớp khi truy vấn điểm chi tiết: {ex.Message}", ex);
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException($"Lỗi ánh xạ dữ liệu khi truy vấn điểm chi tiết: {ex.Message}", ex);
        }
    }
}
