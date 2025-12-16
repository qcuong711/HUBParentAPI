using Microsoft.EntityFrameworkCore;
using Api.Domain.Entities;

namespace Api.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<StudentStudyUnit> StudentStudyUnits => Set<StudentStudyUnit>();
    public DbSet<StudentAverageScoresByYearStudyAll> StudentAverageScoresByYearStudyAll => Set<StudentAverageScoresByYearStudyAll>();
    public DbSet<StudentAverageGatherScoresByYearStudy> StudentAverageGatherScoresByYearStudy => Set<StudentAverageGatherScoresByYearStudy>();
    public DbSet<StudentAverageScoresByYearStudy> StudentAverageScoresByYearStudy => Set<StudentAverageScoresByYearStudy>();
    public DbSet<StudentAverageGatherScoresByTerms> StudentAverageGatherScoresByTerms => Set<StudentAverageGatherScoresByTerms>();
    public DbSet<StudentAverageScores> StudentAverageScores => Set<StudentAverageScores>();
    public DbSet<StudentAverageScoresGraduation> StudentAverageScoresGraduation => Set<StudentAverageScoresGraduation>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Curriculum> Curriculums => Set<Curriculum>();
    public DbSet<StudyUnit> StudyUnits => Set<StudyUnit>();
    public DbSet<StudyProgram> StudyPrograms => Set<StudyProgram>();
    public DbSet<StudentStudyStatus> StudentStudyStatuses => Set<StudentStudyStatus>();
    public DbSet<ClassStudent> ClassStudents => Set<ClassStudent>();
    public DbSet<StudyType> StudyTypes => Set<StudyType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StudentStudyUnit>(entity =>
        {
            entity.ToTable("psc_StudentStudyUnits");
            entity.HasKey(e => new { e.StudentID, e.StudyUnitID });
        });

        modelBuilder.Entity<StudentAverageScoresByYearStudyAll>(entity =>
        {
            entity.ToTable("psc_StudentAverageScoresByYearStudy_All");
            entity.HasNoKey();
        });

        modelBuilder.Entity<StudentAverageGatherScoresByYearStudy>(entity =>
        {
            entity.ToTable("psc_StudentAverageGatherScoresByYearStudy");
            entity.HasNoKey();
        });

        modelBuilder.Entity<StudentAverageScoresByYearStudy>(entity =>
        {
            entity.ToTable("psc_StudentAverageScoresByYearStudy");
            entity.HasNoKey();
        });

        modelBuilder.Entity<StudentAverageGatherScoresByTerms>(entity =>
        {
            entity.ToTable("psc_StudentAverageGatherScoresByTerms");
            entity.HasNoKey();
        });

        modelBuilder.Entity<StudentAverageScores>(entity =>
        {
            entity.ToTable("psc_StudentAverageScores");
            entity.HasNoKey();
        });
        modelBuilder.Entity<StudentAverageScoresGraduation>(entity =>
        {
            entity.ToTable("psc_StudentAverageScoresGraduation");
            entity.HasNoKey();
        });

        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.ToTable("psc_Assignments");
            entity.HasKey(e => e.AssignmentID);
        });

        modelBuilder.Entity<Curriculum>(entity =>
        {
            entity.ToTable("psc_Curriculums");
            entity.HasKey(e => e.CurriculumID);
        });

        modelBuilder.Entity<StudyUnit>(entity =>
        {
            entity.ToTable("psc_StudyUnits");
            entity.HasKey(e => e.StudyUnitID);
        });

        modelBuilder.Entity<StudyProgram>(entity =>
        {
            entity.ToTable("psc1_StudyPrograms");
            entity.HasKey(e => e.StudyProgramID);
        });

        modelBuilder.Entity<StudentStudyStatus>(entity =>
        {
            entity.ToTable("psc_StudentStudyStatus");
            entity.HasNoKey();
        });

        modelBuilder.Entity<ClassStudent>(entity =>
        {
            entity.ToTable("psc_ClassStudents");
            entity.HasKey(e => e.ClassStudentID);
        });

        modelBuilder.Entity<StudyType>(entity =>
        {
            entity.ToTable("psc_StudyTypes");
            entity.HasKey(e => e.StudyTypeID);
        });
    }
}
