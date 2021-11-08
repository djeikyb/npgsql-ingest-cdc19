namespace Core.Tsv;

public class PreKnowns
{
    public readonly Dictionary<string, AgeGroup> AgeGroup;
    public readonly Dictionary<string, CurrentStatus> CurrentStatus;
    public readonly Dictionary<string, Ethnicity> Ethnicity;
    public readonly Dictionary<string, Process> Process;
    public readonly Dictionary<string, Race> Race;
    public readonly Dictionary<string, Sex> Sex;
    public readonly Dictionary<string, SymptomStatus> SymptomStatus;
    public readonly Dictionary<string, Yn> Yn;

    public PreKnowns(
        Dictionary<string, AgeGroup> ageGroup,
        Dictionary<string, CurrentStatus> currentStatus,
        Dictionary<string, Ethnicity> ethnicity,
        Dictionary<string, Process> process,
        Dictionary<string, Race> race,
        Dictionary<string, Sex> sex,
        Dictionary<string, SymptomStatus> symptomStatus,
        Dictionary<string, Yn> yn
    )
    {
        AgeGroup = ageGroup;
        CurrentStatus = currentStatus;
        Ethnicity = ethnicity;
        Process = process;
        Race = race;
        Sex = sex;
        SymptomStatus = symptomStatus;
        Yn = yn;
    }
}
