using System.Diagnostics;

namespace Core.Tsv
{
    public class BulkImporter
    {
        private readonly IPersistCases _repo;
        private readonly PreKnowns _preKnowns;

        private readonly StreamReader _csv;
        // private readonly RawRow _rawRow = new();

        public BulkImporter(IPersistCases repo,
            PreKnowns preKnowns,
            StreamReader csv
        )
        {
            _repo = repo;
            _preKnowns = preKnowns;
            _csv = csv;
        }

        public RawRow ReadRawLine()
        {
            string line = _csv.ReadLine()
                          ?? throw new Exception("Unexpected end of file.");
            var rawRow = new RawRow();
            rawRow.Load(line);
            return rawRow;
        }

        public static CovidCase RawToEntity(RawRow row, PreKnowns preKnowns)
        {
            var entity = new CovidCase();

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

            return entity;
        }

        public async Task Load(int maxBatchSize, int maxInserts, CancellationToken ct = default)
        {
            if (maxBatchSize > maxInserts) maxBatchSize = maxInserts;
            if (maxInserts < maxBatchSize) maxInserts = maxBatchSize;

            var hundrThou = new Stopwatch();
            hundrThou.Start();

            await _repo.Begin(ct);

            int total = 0;
            int batchNumber = 1;
            var t = new Stopwatch();
            while (total < maxInserts)
            {
                t.Reset();
                t.Start();
                var result = await Batch(maxBatchSize, ct);
                t.Stop();

                total += result.Inserted;

                // Console.WriteLine($"Batch #{batchNumber,5}. Total inserts: {total,9}. Batch size: {maxBatchSize}. Seconds: {t.Elapsed.TotalSeconds}");

                if (total % 100_000 == 0)
                {
                    Console.WriteLine($"100_000 records took {hundrThou.Elapsed.ToString()}");
                    hundrThou.Reset();
                    hundrThou.Start();
                }

                if (result.NoMoreData) break;

                batchNumber += 1;
            }

            await _repo.End(ct);
        }

        public async Task<(int Inserted, bool NoMoreData)> Batch(int batchSize, CancellationToken ct)
        {
            int inserts;
            bool eof = false;

            var entities = new List<CovidCase>();
            for (inserts = 0; inserts < batchSize; inserts++)
            {
                var line = _csv.ReadLine();
                if (line == null)
                {
                    // end of stream, no more data
                    eof = true;
                    break;
                }

                var rawRow = new RawRow();
                rawRow.Load(line);
                var entity = RawToEntity(rawRow, _preKnowns);
                entities.Add(entity);
            }

            await _repo.Persist(entities, ct);

            return (Inserted: inserts, NoMoreData: eof);
        }

        public async Task Load(int quantity, CancellationToken ct = default)
        {
            var entities = new List<CovidCase>();
            for (int i = 0; i < quantity; i++)
            {
                var rawRow = ReadRawLine();
                var entity = RawToEntity(rawRow, _preKnowns);
                entities.Add(entity);
            }

            await _repo.Persist(entities, ct);
        }
    }
}
