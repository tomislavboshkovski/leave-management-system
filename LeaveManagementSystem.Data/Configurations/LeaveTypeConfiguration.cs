using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeaveManagementSystem.Data.Configurations
{
    public class LeaveTypeConfiguration : IEntityTypeConfiguration<LeaveType>
    {
        public void Configure(EntityTypeBuilder<LeaveType> builder)
        {
            builder.HasData(
                new LeaveType { Id = 1, Name = "Annual Leave", NumberOfDays = 21 },
                new LeaveType { Id = 2, Name = "Sick Leave", NumberOfDays = 5 },
                new LeaveType { Id = 3, Name = "Parental Leave", NumberOfDays = 270 },
                new LeaveType { Id = 4, Name = "Miscellaneous Leave", NumberOfDays = 5 }
            );
        }
    }
}
