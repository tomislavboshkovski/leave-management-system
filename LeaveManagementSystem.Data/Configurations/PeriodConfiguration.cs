using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeaveManagementSystem.Data.Configurations
{
    public class PeriodConfiguration : IEntityTypeConfiguration<Period>
    {
        public void Configure(EntityTypeBuilder<Period> builder)
        {
            var today = DateTime.Today;
            var currentYear = today.Month >= 7 ? today.Year : today.Year - 1;

            var period = new Period
            {
                Id = 1,
                Name = $"{currentYear}-{currentYear + 1} Period",
                StartDate = new DateOnly(currentYear, 7, 1),
                EndDate = new DateOnly(currentYear + 1, 6, 30)
            };

            builder.HasData(period);
        }
    }
}

