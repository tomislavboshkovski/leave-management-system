using AutoMapper;
using LeaveManagementSystem.Application.Models.LeaveRequests;

namespace LeaveManagementSystem.Application.MappingProfiles
{
    public class LeaveRequestAutoMapperProfile : Profile
    {
        public LeaveRequestAutoMapperProfile()
        {
            CreateMap<LeaveRequestCreateVM, LeaveRequest>();
        }
    }
}
