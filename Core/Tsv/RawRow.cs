namespace Core.Tsv;

public class RawRow
{
    private void LoadV2(string row)
    {
        const char delim = '\t';
        int pos = 0;
        for (int fieldNum = 0; fieldNum < 19; fieldNum++)
        {
            var nextDelim = row.IndexOf(delim, pos);

            var len = nextDelim != -1
                ? nextDelim - pos
                : row.Length - pos;

            var fieldValue = row.Substring(pos, len);
            pos += len + 1;

            switch (fieldNum)
            {
                case 0:
                    CaseMonth = fieldValue;
                    break;
                case 1:
                    ResState = fieldValue;
                    break;
                case 2:
                    StateFipsCode = fieldValue;
                    break;
                case 3:
                    ResCounty = fieldValue;
                    break;
                case 4:
                    CountyFipsCode = fieldValue;
                    break;
                case 5:
                    AgeGroup = fieldValue;
                    break;
                case 6:
                    Sex = fieldValue;
                    break;
                case 7:
                    Race = fieldValue;
                    break;
                case 8:
                    Ethnicity = fieldValue;
                    break;
                case 9:
                    CasePositiveSpecimenInterval = fieldValue;
                    break;
                case 10:
                    CaseOnsetInterval = fieldValue;
                    break;
                case 11:
                    Process = fieldValue;
                    break;
                case 12:
                    ExposureYn = fieldValue;
                    break;
                case 13:
                    CurrentStatus = fieldValue;
                    break;
                case 14:
                    SymptomStatus = fieldValue;
                    break;
                case 15:
                    HospYn = fieldValue;
                    break;
                case 16:
                    IcuYn = fieldValue;
                    break;
                case 17:
                    DeathYn = fieldValue;
                    break;
                case 18:
                    UnderlyingConditionsYn = fieldValue;
                    break;
                default:
                    throw new Exception($"Unexpected field index at position {pos}: {fieldNum}. Row {row}");
            }
        }
    }

    private void Load(string row)
    {
        var col = row.Split("\t");
        CaseMonth = col[0];
        ResState = col[1];
        StateFipsCode = col[2];
        ResCounty = col[3];
        CountyFipsCode = col[4];
        AgeGroup = col[5];
        Sex = col[6];
        Race = col[7];
        Ethnicity = col[8];
        CasePositiveSpecimenInterval = col[9];
        CaseOnsetInterval = col[10];
        Process = col[11];
        ExposureYn = col[12];
        CurrentStatus = col[13];
        SymptomStatus = col[14];
        HospYn = col[15];
        IcuYn = col[16];
        DeathYn = col[17];
        UnderlyingConditionsYn = col[18];
    }

    public string? CaseMonth;
    public string? ResState;
    public string? StateFipsCode;
    public string? ResCounty;
    public string? CountyFipsCode;
    public string? AgeGroup;
    public string? Sex;
    public string? Race;
    public string? Ethnicity;
    public string? CasePositiveSpecimenInterval;
    public string? CaseOnsetInterval;
    public string? Process;
    public string? ExposureYn;
    public string? CurrentStatus;
    public string? SymptomStatus;
    public string? HospYn;
    public string? IcuYn;
    public string? DeathYn;
    public string? UnderlyingConditionsYn;

    public override string ToString()
    {
        return
            $"{nameof(CaseMonth)}: {CaseMonth}, {nameof(ResState)}: {ResState}, {nameof(StateFipsCode)}: {StateFipsCode}, {nameof(ResCounty)}: {ResCounty}, {nameof(CountyFipsCode)}: {CountyFipsCode}, {nameof(AgeGroup)}: {AgeGroup}, {nameof(Sex)}: {Sex}, {nameof(Race)}: {Race}, {nameof(Ethnicity)}: {Ethnicity}, {nameof(CasePositiveSpecimenInterval)}: {CasePositiveSpecimenInterval}, {nameof(CaseOnsetInterval)}: {CaseOnsetInterval}, {nameof(Process)}: {Process}, {nameof(ExposureYn)}: {ExposureYn}, {nameof(CurrentStatus)}: {CurrentStatus}, {nameof(SymptomStatus)}: {SymptomStatus}, {nameof(HospYn)}: {HospYn}, {nameof(IcuYn)}: {IcuYn}, {nameof(DeathYn)}: {DeathYn}, {nameof(UnderlyingConditionsYn)}: {UnderlyingConditionsYn}";
    }
}
