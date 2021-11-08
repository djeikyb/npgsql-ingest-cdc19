namespace Core.Tsv.RowParser;

public class SubstrRowParser : IRowParser
{
    /// <inheritdoc />
    public void Parse(string row, RawRow raw)
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
                    raw.CaseMonth = fieldValue;
                    break;
                case 1:
                    raw.ResState = fieldValue;
                    break;
                case 2:
                    raw.StateFipsCode = fieldValue;
                    break;
                case 3:
                    raw.ResCounty = fieldValue;
                    break;
                case 4:
                    raw.CountyFipsCode = fieldValue;
                    break;
                case 5:
                    raw.AgeGroup = fieldValue;
                    break;
                case 6:
                    raw.Sex = fieldValue;
                    break;
                case 7:
                    raw.Race = fieldValue;
                    break;
                case 8:
                    raw.Ethnicity = fieldValue;
                    break;
                case 9:
                    raw.CasePositiveSpecimenInterval = fieldValue;
                    break;
                case 10:
                    raw.CaseOnsetInterval = fieldValue;
                    break;
                case 11:
                    raw.Process = fieldValue;
                    break;
                case 12:
                    raw.ExposureYn = fieldValue;
                    break;
                case 13:
                    raw.CurrentStatus = fieldValue;
                    break;
                case 14:
                    raw.SymptomStatus = fieldValue;
                    break;
                case 15:
                    raw.HospYn = fieldValue;
                    break;
                case 16:
                    raw.IcuYn = fieldValue;
                    break;
                case 17:
                    raw.DeathYn = fieldValue;
                    break;
                case 18:
                    raw.UnderlyingConditionsYn = fieldValue;
                    break;
                default:
                    throw new Exception($"Unexpected field index at position {pos}: {fieldNum}. Row {row}");
            }
        }
    }
}
