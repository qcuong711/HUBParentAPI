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
    public DbSet<StudentStudyUnitAssignment> StudentStudyUnitAssignments => Set<StudentStudyUnitAssignment>();
    public DbSet<StudentScheduleStudyUnit> StudentScheduleStudyUnits => Set<StudentScheduleStudyUnit>();
    public DbSet<ScheduleStudyUnit> ScheduleStudyUnits => Set<ScheduleStudyUnit>();
    public DbSet<StudentRegulationResult> StudentRegulationResults => Set<StudentRegulationResult>();
    public DbSet<StudyStatus> StudyStatuses => Set<StudyStatus>();
    public DbSet<Term> Terms => Set<Term>();
    public DbSet<WeekScheduleStudyUnit> WeekScheduleStudyUnits => Set<WeekScheduleStudyUnit>();
    public DbSet<WeekSchedule> WeekSchedules => Set<WeekSchedule>();
    public DbSet<Period> Periods => Set<Period>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<ProfessorWeekSchedule> ProfessorWeekSchedules => Set<ProfessorWeekSchedule>();
    public DbSet<Professor> Professors => Set<Professor>();
    public DbSet<ExaminationStudent> ExaminationStudents => Set<ExaminationStudent>();
    public DbSet<Examination> Examinations => Set<Examination>();
    public DbSet<ExaminationRoom> ExaminationRooms => Set<ExaminationRoom>();
    public DbSet<ExaminationSchedule> ExaminationSchedules => Set<ExaminationSchedule>();
    public DbSet<ProfessorClassStudent> ProfessorClassStudents => Set<ProfessorClassStudent>();

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

        modelBuilder.Entity<StudentStudyUnitAssignment>(entity =>
        {
            entity.ToTable("psc_StudentStudyUnitAssignments");
            entity.HasKey(e => new { e.StudentID, e.StudyUnitTypeID, e.AssignmentID, e.ScheduleStudyUnitID });
        });

        modelBuilder.Entity<StudentScheduleStudyUnit>(entity =>
        {
            entity.ToTable("psc_StudentScheduleStudyUnits");
            entity.HasKey(e => new { e.StudentID, e.ScheduleStudyUnitID });
        });

        modelBuilder.Entity<ScheduleStudyUnit>(entity =>
        {
            entity.ToTable("psc_ScheduleStudyUnits");
            entity.HasKey(e => e.ScheduleStudyUnitID);
        });

        modelBuilder.Entity<StudentRegulationResult>(entity =>
        {
            entity.ToTable("psc_StudentRegulationResult");
            entity.HasNoKey();
        });

        modelBuilder.Entity<StudyStatus>(entity =>
        {
            entity.ToTable("psc_StudyStatus");
            entity.HasKey(e => e.StudyStatusID);
        });

        modelBuilder.Entity<Term>(entity =>
        {
            entity.ToTable("psc_Terms");
            entity.HasKey(e => new { e.YearStudy, e.TermID });
        });

        modelBuilder.Entity<WeekScheduleStudyUnit>(entity =>
        {
            entity.ToTable("psc_Sch_WeekScheduleStudyUnits");
            entity.HasKey(e => new { e.ScheduleStudyUnitID, e.WeekScheduleID });
        });

        modelBuilder.Entity<WeekSchedule>(entity =>
        {
            entity.ToTable("psc_Sch_WeekSchedules");
            entity.HasKey(e => e.WeekScheduleID);
        });

        modelBuilder.Entity<Period>(entity =>
        {
            entity.ToTable("psc_Periods");
            entity.HasKey(e => e.PeriodID);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.ToTable("psc_Rooms");
            entity.HasKey(e => e.RoomID);
        });

        modelBuilder.Entity<ProfessorWeekSchedule>(entity =>
        {
            entity.ToTable("psc_Sch_ProfessorWeekSchedules");
            entity.HasNoKey();
        });

        modelBuilder.Entity<Professor>(entity =>
        {
            entity.ToTable("psc_Professors");
            entity.HasKey(e => e.ProfessorID);
        });

        modelBuilder.Entity<ExaminationStudent>(entity =>
        {
            entity.ToTable("psc_ExaminationStudents");
            entity.HasKey(e => new { e.ExaminationID, e.StudentID });
            entity.Property(e => e.ExaminationID).HasColumnName("Examination");
            entity.Property(e => e.IsAbsent).HasColumnName("VangThi");
        });

        modelBuilder.Entity<Examination>(entity =>
        {
            entity.ToTable("psc_Examinations");
            entity.HasKey(e => e.ExaminationID);
            entity.Property(e => e.ExaminationID).HasColumnName("Examination");
            entity.Property(e => e.PlannedExamDate).HasColumnName("NgayThiLanKeDuKien");
        });

        modelBuilder.Entity<ExaminationRoom>(entity =>
        {
            entity.ToTable("psc_Examination_Rooms");
            entity.HasKey(e => e.ID);
            entity.Property(e => e.ExaminationID).HasColumnName("Examination");
        });

        modelBuilder.Entity<ExaminationSchedule>(entity =>
        {
            entity.ToTable("psc_ExaminationSchedules");
            entity.HasKey(e => e.ExaminationScheduleID);
            entity.Property(e => e.ExaminationScheduleID).HasColumnName("Examination");
            entity.Property(e => e.PlannedExamDate).HasColumnName("NgayThiLanKeDuKien");
        });

        modelBuilder.Entity<ProfessorClassStudent>(entity =>
        {
            entity.ToTable("psc_ProfessorClassStudents");
            entity.HasKey(e => new { e.ProfessorID, e.ClassStudentID, e.YearStudy, e.TermID });
        });
    }
}
