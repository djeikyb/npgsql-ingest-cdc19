using System.Collections.Generic;
using System.Threading.Tasks;
using Core;

namespace Infra
{
    public class SeedData
    {
        private readonly PgContext _context;

        public SeedData(PgContext context)
        {
            _context = context;
        }

        public async Task Plant()
        {
            await _context.AgeGroup.AddRangeAsync(AgeGroup());
            await _context.CurrentStatus.AddRangeAsync(CurrentStatus());
            await _context.Ethnicity.AddRangeAsync(Ethnicity());
            await _context.Process.AddRangeAsync(Process());
            await _context.Race.AddRangeAsync(Race());
            await _context.Sex.AddRangeAsync(Sex());
            await _context.SymptomStatus.AddRangeAsync(SymptomStatus());
            await _context.Yn.AddRangeAsync(Yn());
        }

        private static IEnumerable<AgeGroup> AgeGroup()
        {
            yield return new() { Label = "0 - 17 years", BoundLower = 0, BoundUpper = 17 };
            yield return new() { Label = "18 to 49 years", BoundLower = 18, BoundUpper = 49 };
            yield return new() { Label = "50 to 64 years", BoundLower = 50, BoundUpper = 64 };
            yield return new() { Label = "65+ years", BoundLower = 65, BoundUpper = null };
            yield return new() { Label = "Missing", BoundLower = null, BoundUpper = null };
            yield return new() { Label = "NA", BoundLower = null, BoundUpper = null };
        }

        private static IEnumerable<CurrentStatus> CurrentStatus()
        {
            yield return new() { Label = "Laboratory-confirmed case" };
            yield return new() { Label = "Probable Case" };
        }

        private static IEnumerable<Ethnicity> Ethnicity()
        {
            yield return new() { Label = "NA" };
            yield return new() { Label = "Unknown" };
            yield return new() { Label = "Missing" };
            yield return new() { Label = "Non-Hispanic/Latino" };
            yield return new() { Label = "Hispanic/Latino" };
        }

        private static IEnumerable<Process> Process()
        {
            yield return new() { Label = "Missing" };
            yield return new() { Label = "Unknown" };
            yield return new() { Label = "Multiple" };
            yield return new() { Label = "Autopsy" };
            yield return new() { Label = "Clinical evaluation" };
            yield return new() { Label = "Contact tracing of case patient" };
            yield return new() { Label = "Laboratory reported" };
            yield return new() { Label = "Other detection method (specify)" };
            yield return new() { Label = "Provider reported" };
            yield return new() { Label = "Routine physical examination" };
            yield return new() { Label = "Routine surveillance" };
            yield return new() { Label = "Other" };
        }

        private static IEnumerable<Race> Race()
        {
            yield return new() { Label = "NA" };
            yield return new() { Label = "Unknown" };
            yield return new() { Label = "American Indian/Alaska Native" };
            yield return new() { Label = "Asian" };
            yield return new() { Label = "Black" };
            yield return new() { Label = "Missing" };
            yield return new() { Label = "Multiple/Other" };
            yield return new() { Label = "Native Hawaiian/Other Pacific Islander" };
            yield return new() { Label = "White" };
        }

        private static IEnumerable<Sex> Sex()
        {
            yield return new() { Label = "NA" };
            yield return new() { Label = "Missing" };
            yield return new() { Label = "Unknown" };
            yield return new() { Label = "Female" };
            yield return new() { Label = "Male" };
            yield return new() { Label = "Other" };
        }

        private static IEnumerable<SymptomStatus> SymptomStatus()
        {
            yield return new() { Label = "nul" };
            yield return new() { Label = "Missing" };
            yield return new() { Label = "Unknown" };
            yield return new() { Label = "Asymptomatic" };
            yield return new() { Label = "Symptomatic" };
        }

        private static IEnumerable<Yn> Yn()
        {
            yield return new() { Label = "Yes" };
            yield return new() { Label = "No" };
            yield return new() { Label = "Unknown" };
            yield return new() { Label = "Missing" };
            yield return new() { Label = "NA" };
            yield return new() { Label = "nul" };
        }
    }
}
