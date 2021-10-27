namespace Core;

#nullable disable
public class CovidCase
{
    public int CovidCaseId { get; set; }

    public string CaseMonth { get; set; }
    public string ResState { get; set; }
    public string StateFipsCode { get; set; }
    public string ResCounty { get; set; }
    public string CountyFipsCode { get; set; }

    public int? AgeGroupId { get; set; }
    // public AgeGroup AgeGroup { get; set; }

    public int? SexId { get; set; }
    // public Sex Sex { get; set; }

    public int? RaceId { get; set; }
    // public Race Race { get; set; }

    public int? EthnicityId { get; set; }
    // public Ethnicity Ethnicity { get; set; }

    public string CasePositiveSpecimenInterval { get; set; }
    public string CaseOnsetInterval { get; set; }

    public int? ProcessId { get; set; }
    // public Process Process { get; set; }

    public int? ExposureYnId { get; set; }
    // public Yn ExposureYn { get; set; }

    public int? CurrentStatusId { get; set; }
    // public CurrentStatus CurrentStatus { get; set; }

    public int? SymptomStatusId { get; set; }
    // public SymptomStatus SymptomStatus { get; set; }

    public int? HospYnId { get; set; }
    // public Yn HospYn { get; set; }

    public int? IcuYnId { get; set; }
    // public Yn IcuYn { get; set; }

    public int? DeathYnId { get; set; }
    // public Yn DeathYn { get; set; }

    public int? UnderlyingConditionsYnId { get; set; }
    // public Yn UnderlyingConditionsYn { get; set; }
}
