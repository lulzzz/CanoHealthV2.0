using CanoHealth.WebPortal.Core.Domain;
using System.Data.Entity.ModelConfiguration;

namespace CanoHealth.WebPortal.Persistance.EntityConfigurations
{
    public class ScheduleConfiguration : EntityTypeConfiguration<Schedule>
    {
        public ScheduleConfiguration()
        {
            //Table overrides

            //Primary keys overrides

            //Property configurations sorted alphabetically

            //Relationships configurations sorted alphabetically
            HasOptional(s => s.Schedule1)
                .WithMany(s => s.Schedules1)
                .HasForeignKey(s => s.RecurrenceID);
        }
    }
}