the time to beat!

03:26	PostgreSQL - postgres@localhost: data.tsv imported to data: 32,806,678 rows (19 min, 30 sec, 914 ms, 4.35 MB/s)


32806678/1170 = 100000/x

32806678x = 100000*1170
x = 3.56634707


1. v2.RoughDraft

1. Time

...
100000 records took 00:00:00.4597436
100000 records took 00:00:00.4641218
100000 records took 00:00:00.4672910
100000 records took 00:00:00.7798506
100000 records took 00:00:00.4700023
Overall time: 00:02:50.3618660

And then another three minutes to add foreign keys.

2. Notes

2.1. This was not a test

Accidentally processed the entire dataset. Meant to only ingest a small
sample. Makes the overall time much more impressive.

2.2. RawRow

Accidentally used the string::split version. Will have to run again to
know if this could reduce the time. Expect less

cols += "case_month";
cols += "res_state";
cols += "state_fips_code";
cols += "res_county";
cols += "county_fips_code";
cols += "age_group_id";
cols += "sex_id";
cols += "race_id";
cols += "ethnicity_id";
cols += "case_positive_specimen_interval";
cols += "case_onset_interval";
cols += "process_id";
cols += "exposure_yn_id";
cols += "current_status_id";
cols += "symptom_status_id";
cols += "hosp_yn_id";
cols += "icu_yn_id";
cols += "death_yn_id";
cols += "underlying_conditions_yn_id";
