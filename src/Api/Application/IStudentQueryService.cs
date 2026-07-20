using Api.Application.Dtos;

namespace Api.Application;

public interface IStudentQueryService
{
    Task<StudentRegistrationsDto> GetRegistrationsAsync(string studentCode, string? yearStudy, string? termId);
    Task<IReadOnlyList<StudentStudyStatusDto>> GetStudyStatusesAsync(string studentCode, string? yearStudy, string? termId);
    Task<IReadOnlyList<AdvisorDto>> GetAdvisorsAsync(string studentCode, string? yearStudy, string? termId);
}
