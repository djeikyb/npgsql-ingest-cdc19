namespace Core.Tsv.RowParser;

public class SplitStringRowParser : IRowParser
{
    /// <inheritdoc />
    public void Parse(string row, RawRow raw)
    {
        var col = row.Split("\t");
        raw.CaseMonth = col[0];
        raw.ResState = col[1];
        raw.StateFipsCode = col[2];
        raw.ResCounty = col[3];
        raw.CountyFipsCode = col[4];
        raw.AgeGroup = col[5];
        raw.Sex = col[6];
        raw.Race = col[7];
        raw.Ethnicity = col[8];
        raw.CasePositiveSpecimenInterval = col[9];
        raw.CaseOnsetInterval = col[10];
        raw.Process = col[11];
        raw.ExposureYn = col[12];
        raw.CurrentStatus = col[13];
        raw.SymptomStatus = col[14];
        raw.HospYn = col[15];
        raw.IcuYn = col[16];
        raw.DeathYn = col[17];
        raw.UnderlyingConditionsYn = col[18];
    }
}
