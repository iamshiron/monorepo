using Shiron.Mutils.Core.DTOs;

namespace Shiron.Mutils.Core.Services;

public interface IOptimizerService {
    Task<OptimizerAnalysisResponse> AnalyzeAsync(Guid userId, OptimizerAnalysisRequest request, CancellationToken cancellationToken = default);
    Task<OptimizerSuggestionsResponse> GetSuggestionsAsync(Guid userId, CancellationToken cancellationToken = default);
}
