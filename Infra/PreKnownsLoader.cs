using Core;
using Core.Tsv;
using Microsoft.EntityFrameworkCore;

namespace Infra;

public class PreKnownsLoader : IPreKnownsLoader
{
    private readonly PgContext _context;

    public PreKnownsLoader(PgContext context)
    {
        _context = context;
    }

    public async Task<PreKnowns> Load(CancellationToken ct)
    {
        var ageGroup = await _context.AgeGroup.ToDictionaryAsync(k => k.Label, ct);
        var currentStatus = await _context.CurrentStatus.ToDictionaryAsync(k => k.Label, ct);
        var ethnicity = await _context.Ethnicity.ToDictionaryAsync(k => k.Label, ct);
        var process = await _context.Process.ToDictionaryAsync(k => k.Label, ct);
        var race = await _context.Race.ToDictionaryAsync(k => k.Label, ct);
        var sex = await _context.Sex.ToDictionaryAsync(k => k.Label, ct);
        var symptomStatus = await _context.SymptomStatus.ToDictionaryAsync(k => k.Label, ct);
        var yn = await _context.Yn.ToDictionaryAsync(k => k.Label, ct);
        return new PreKnowns(ageGroup, currentStatus, ethnicity, process, race, sex, symptomStatus, yn);
    }
}
