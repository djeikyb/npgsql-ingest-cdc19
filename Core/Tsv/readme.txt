the time to beat!

03:26	PostgreSQL - postgres@localhost: data.tsv imported to data: 32,806,678 rows (19 min, 30 sec, 914 ms, 4.35 MB/s)


32806678/1170 = 100000/x

32806678x = 100000*1170
x = 3.56634707


1. v2.RoughDraft

1.1. Time

...
100000 records took 00:00:00.4597436
100000 records took 00:00:00.4641218
100000 records took 00:00:00.4672910
100000 records took 00:00:00.7798506
100000 records took 00:00:00.4700023
Overall time: 00:02:50.3618660

And then another three minutes to add foreign keys.

1.2. Notes

1.2.1. This was not a test

Accidentally processed the entire dataset. Meant to only ingest a small
sample. Makes the overall time much more impressive.

1.2.2. RawRow

Accidentally used the string::split version. Will have to run again to
know if this could reduce the time. Expect less

2. v3.RoughDraft

Worked to reduce allocations. Removed a few awaits. Reused the same
instance of RawRow. Still, there are gigs of string allocations. I
wonder if there's a version that works on byte arrays instead. Still
have to allocate strings for every value, but could halve the cost? At
most. Maybe only a quarter if there are fancy interning tricks at play.
It looks like allocations happen three major places: the stream's read
line, sub-stringing each field value, and creating the type of the write
row method. Every field value will be allocated, especially unique
field values. But the read line and method type creation..

100000 records took 00:00:00.4320766
100000 records took 00:00:00.4201095
100000 records took 00:00:00.4180719
100000 records took 00:00:00.4208011
100000 records took 00:00:00.4221561
Overall time: 00:02:42.3961661

Then I added added ConfigureAwait(false) to all the write commands. At
first I had impressive results. The 100k insert rate nearly began at
4/10s, whereas usually on these short 5mm runs I would guess the median
rate is 6-7/10s per 100k. After more testing, I'm not sure it's related
to any code change I made, and feels correlated with whether or not the
tables are dropped.

But then on a full run, slightly worse than before. As with the few
seconds of improvement this version initially brought, I'm not sure
if this result is significantly different from v1's time of 170s.

100000 records took 00:00:00.4354722
100000 records took 00:00:00.4308793
100000 records took 00:00:00.4856091
100000 records took 00:00:00.4469077
100000 records took 00:00:00.4338302
Overall time: 00:02:48.2847513


3. v4.RoughDraft

Minimize string allocations, use mem reader, write bytes.
Will need the CovidCase entity, can put the foreign keys back?
Can the program run initial migration, then at the end run a second migration?
Will need a second CovidCase model that has byte[] instead of string

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



4. SpanRowParser

        var pb4 = new Playbook(
            new SpanRowParser(),
            new PgCopy(context),
            new Core.Tsv.v3.RoughDraft(),
            config
        );

1,950.1 MB (max: 1,950.1 MB) by RawRow.AssignField (allocated type: String)
755.5 MB (max: 755.5 MB) by RoughDraft.Import (allocated type: String)
542.6 MB (max: 542.6 MB) by RawRow.AssignField (allocated type: String)
444.6 MB (max: 444.6 MB) by RawRow.AssignField (allocated type: String)
214.5 MB (max: 214.5 MB) by RoughDraft.Import (allocated type: String)
208.6 MB (max: 208.6 MB) by PgCopy.Persist (allocated type: <WriteRow>d__8)
199.4 MB (max: 199.4 MB) by RoughDraft.Import (allocated type: <Persist>d__6)
169.7 MB (max: 169.7 MB) by RoughDraft.Import (allocated type: String)
129.2 MB (max: 129.2 MB) by RoughDraft.Import (allocated type: Char[])
115.3 MB (max: 115.3 MB) by RoughDraft.Import (allocated type: String)

All AssignField stacktraces are:

at String.Ctor(ReadOnlySpan)
at RawRow.AssignField(ReadOnlySpan) in /Users/jacob/dev/me/active/cdc19/Core/Tsv/RawRow.cs:line 17 column 13
at SpanRowParser.Parse(String, RawRow) in /Users/jacob/dev/me/active/cdc19/Core/Tsv/RowParser/SpanRowParser.cs:line 8 column 9
at RoughDraft.Import(Stream, Playbook, PreKnowns, CancellationToken) in /Users/jacob/dev/me/active/cdc19/Core/Tsv/v3/RoughDraft.cs:line 40 column 13
at ExecutionContext.RunInternal(ExecutionContext, ContextCallback, Object)
at AsyncTaskMethodBuilder+AsyncStateMachineBox<VoidTaskResult,StartupHook+<ReceiveDeltas>d__3>.MoveNext(Thread)
at AwaitTaskContinuation.RunOrScheduleAction(IAsyncStateMachineBox, bool)
at Task.RunContinuations(Object)
at AsyncTaskMethodBuilder.SetResult()
at PgCopy.Persist(CovidCase, CancellationToken)
at ExecutionContext.RunInternal(ExecutionContext, ContextCallback, Object)
at AsyncTaskMethodBuilder+AsyncStateMachineBox<VoidTaskResult,StartupHook+<ReceiveDeltas>d__3>.MoveNext(Thread)
at AwaitTaskContinuation.RunOrScheduleAction(IAsyncStateMachineBox, bool)
at Task.RunContinuations(Object)
at AsyncTaskMethodBuilder.SetResult()
at PgCopy.WriteRow(NpgsqlBinaryImporter, CovidCase, CancellationToken)
at ExecutionContext.RunInternal(ExecutionContext, ContextCallback, Object)
at AsyncTaskMethodBuilder+AsyncStateMachineBox<VoidTaskResult,StartupHook+<ReceiveDeltas>d__3>.MoveNext(Thread)
at AwaitTaskContinuation.RunOrScheduleAction(IAsyncStateMachineBox, bool)
at Task.RunContinuations(Object)
at Task<__Canon>.TrySetResult()
at NpgsqlBinaryImporter+<Write>d__29<__Canon>.MoveNext()
at ExecutionContext.RunInternal(ExecutionContext, ContextCallback, Object)
at AsyncTaskMethodBuilder+AsyncStateMachineBox<VoidTaskResult,StartupHook+<ReceiveDeltas>d__3>.MoveNext(Thread)
at AwaitTaskContinuation.RunOrScheduleAction(IAsyncStateMachineBox, bool)
at Task.RunContinuations(Object)
at Task<__Canon>.TrySetResult()
at AsyncTaskMethodBuilder<__Canon>.SetExistingTaskResult(Task, )
at NpgsqlTypeHandler+<WriteWithLength>d__14<Int32>.MoveNext()
at ExecutionContext.RunInternal(ExecutionContext, ContextCallback, Object)
at AsyncTaskMethodBuilder+AsyncStateMachineBox<VoidTaskResult,StartupHook+<ReceiveDeltas>d__3>.MoveNext(Thread)
at AwaitTaskContinuation.RunOrScheduleAction(IAsyncStateMachineBox, bool)
at Task.RunContinuations(Object)
at Task<__Canon>.TrySetResult()
at NpgsqlWriteBuffer.Flush(bool, CancellationToken)
at ExecutionContext.RunInternal(ExecutionContext, ContextCallback, Object)
at AwaitTaskContinuation.RunOrScheduleAction(IAsyncStateMachineBox, bool)
at Task.RunContinuations(Object)
at Socket+AwaitableSocketAsyncEventArgs.InvokeContinuation(Action, Object, bool, bool)
at SocketAsyncEngine.System.Threading.IThreadPoolWorkItem.Execute()
at ThreadPoolWorkQueue.Dispatch()
at PortableThreadPool+WorkerThread.WorkerThreadStart()
at Thread+StartHelper.RunWorker()
at Thread+StartHelper.Run()
at Thread.StartCallback()


