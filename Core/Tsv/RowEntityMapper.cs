namespace Core.Tsv;

internal static class RowEntityMapper
{
    public static CovidCase RawToEntity(RawRow row, PreKnowns preKnowns)
    {
        var entity = new CovidCase();
        RowToEntity(row, preKnowns, entity);
        return entity;
    }

    /// <summary>
    /// Copies row properties to
    /// </summary>
    /// <param name="row"></param>
    /// <param name="preKnowns"></param>
    /// <param name="entity"></param>
    public static void RowToEntity(RawRow row, PreKnowns preKnowns, CovidCase entity)
    {
        entity.CaseMonth = row.CaseMonth;
        entity.ResState = row.ResState;
        entity.StateFipsCode = row.StateFipsCode;
        entity.ResCounty = row.ResCounty;
        entity.CountyFipsCode = row.CountyFipsCode;

        if (row.AgeGroup != null)
        {
            entity.AgeGroupId = preKnowns.AgeGroup[row.AgeGroup].AgeGroupId;
        }

        if (row.Sex != null)
        {
            entity.SexId = preKnowns.Sex[row.Sex].SexId;
        }

        if (row.Race != null)
        {
            entity.RaceId = preKnowns.Race[row.Race].RaceId;
        }

        if (row.Ethnicity != null)
        {
            entity.EthnicityId = preKnowns.Ethnicity[row.Ethnicity].EthnicityId;
        }

        entity.CasePositiveSpecimenInterval = row.CasePositiveSpecimenInterval;
        entity.CaseOnsetInterval = row.CaseOnsetInterval;

        if (row.Process != null)
        {
            entity.ProcessId = preKnowns.Process[row.Process].ProcessId;
        }

        if (!string.IsNullOrEmpty(row.ExposureYn))
        {
            entity.ExposureYnId = preKnowns.Yn[row.ExposureYn].YnId;
        }

        if (row.CurrentStatus != null)
        {
            entity.CurrentStatusId = preKnowns.CurrentStatus[row.CurrentStatus].CurrentStatusId;
        }

        if (row.SymptomStatus != null)
        {
            entity.SymptomStatusId = preKnowns.SymptomStatus[row.SymptomStatus].SymptomStatusId;
        }

        if (!string.IsNullOrEmpty(row.HospYn))
        {
            entity.HospYnId = preKnowns.Yn[row.HospYn].YnId;
        }

        if (!string.IsNullOrEmpty(row.IcuYn))
        {
            entity.IcuYnId = preKnowns.Yn[row.IcuYn].YnId;
        }

        if (!string.IsNullOrEmpty(row.DeathYn))
        {
            entity.DeathYnId = preKnowns.Yn[row.DeathYn].YnId;
        }

        if (!string.IsNullOrEmpty(row.UnderlyingConditionsYn))
        {
            entity.UnderlyingConditionsYnId = preKnowns.Yn[row.UnderlyingConditionsYn].YnId;
        }
    }
}
