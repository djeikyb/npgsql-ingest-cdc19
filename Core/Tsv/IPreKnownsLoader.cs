namespace Core.Tsv;

public interface IPreKnownsLoader
{
    Task<PreKnowns> Load(CancellationToken ct);
}
