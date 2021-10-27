using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra
{
    public class EntityConfigurations :
        IEntityTypeConfiguration<CovidCase>
        , IEntityTypeConfiguration<AgeGroup>
        , IEntityTypeConfiguration<CurrentStatus>
        , IEntityTypeConfiguration<Ethnicity>
        , IEntityTypeConfiguration<Process>
        , IEntityTypeConfiguration<Race>
        , IEntityTypeConfiguration<Sex>
        , IEntityTypeConfiguration<SymptomStatus>
        , IEntityTypeConfiguration<Yn>
    {
        public void Configure(EntityTypeBuilder<CovidCase> builder)
        {
            // builder.HasIndex(x => x.CaseMonth);
            // builder.Property(x => x.CaseMonth).IsRequired();
            //
            // builder.HasIndex(x => x.ResState);
            // builder.Property(x => x.ResState).IsRequired();
            //
            // builder.HasIndex(x => x.StateFipsCode);
            // builder.Property(x => x.StateFipsCode).IsRequired();
            //
            // builder.HasIndex(x => x.ResCounty);
            // builder.Property(x => x.ResCounty).IsRequired();
            //
            // builder.HasIndex(x => x.CountyFipsCode);
            // builder.Property(x => x.CountyFipsCode).IsRequired();
        }

        public void Configure(EntityTypeBuilder<AgeGroup> builder)
        {
            builder
                .HasIndex(x => x.Label)
                .IsUnique()
                ;
        }

        public void Configure(EntityTypeBuilder<CurrentStatus> builder)
        {
            builder
                .HasIndex(x => x.Label)
                .IsUnique()
                ;
        }

        public void Configure(EntityTypeBuilder<Ethnicity> builder)
        {
            builder
                .HasIndex(x => x.Label)
                .IsUnique()
                ;
        }

        public void Configure(EntityTypeBuilder<Process> builder)
        {
            builder
                .HasIndex(x => x.Label)
                .IsUnique()
                ;
        }

        public void Configure(EntityTypeBuilder<Race> builder)
        {
            builder
                .HasIndex(x => x.Label)
                .IsUnique()
                ;
        }

        public void Configure(EntityTypeBuilder<Sex> builder)
        {
            builder
                .HasIndex(x => x.Label)
                .IsUnique()
                ;
        }

        public void Configure(EntityTypeBuilder<SymptomStatus> builder)
        {
            builder
                .HasIndex(x => x.Label)
                .IsUnique()
                ;
        }

        public void Configure(EntityTypeBuilder<Yn> builder)
        {
            builder
                .HasIndex(x => x.Label)
                .IsUnique()
                ;
        }
    }
}
