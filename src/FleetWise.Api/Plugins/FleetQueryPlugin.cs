using System.ComponentModel;
using System.Text.Json;
using FleetWise.Infrastructure.Repositories;
using Microsoft.SemanticKernel;

namespace FleetWise.Api.Plugins;

/// <summary>
/// Semantic Kernel plugin that exposes fleet vehicle queries as kernel functions.
/// The LLM reads the [Description] attributes to decide which function to call
/// based on the user's natural language question.
/// </summary>
public class FleetQueryPlugin(IVehicleRepository vehicleRepo)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    [KernelFunction("get_fleet_summary")]
    [Description("Returns a summary of the fleet including total vehicle count and breakdowns by status, fuel type, and department. Use this when the user asks about fleet size, composition, or general fleet statistics.")]
    public async Task<string> GetFleetSummary()
    {
        var summary = await vehicleRepo.GetFleetSummaryAsync();
        var json = JsonSerializer.Serialize(summary, JsonOptions);
        return $"Fleet summary: {summary.TotalVehicles} total vehicles\n{json}";
    }
}
