using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Application.Dtos;

namespace Api.Application;

public interface IScoreQueryService
{
    Task<IReadOnlyList<DetailedStudyUnitScoreDto>> GetStudyUnitScoresDetailedAsync(string studentCode);
}
