namespace Core.Tsv;

public class PreKnowns
{
    public Dictionary<string, AgeGroup> AgeGroup;
    public Dictionary<string, CurrentStatus> CurrentStatus;
    public Dictionary<string, Ethnicity> Ethnicity;
    public Dictionary<string, Process> Process;
    public Dictionary<string, Race> Race;
    public Dictionary<string, Sex> Sex;
    public Dictionary<string, SymptomStatus> SymptomStatus;
    public Dictionary<string, Yn> Yn;

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
