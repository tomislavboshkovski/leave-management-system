using LeaveManagementSystem.Application.Models.LeaveAllocations;
using System.ComponentModel;

namespace LeaveManagementSystem.Application.Models.LeaveRequests
{
    public class ReviewLeaveRequestVM : LeaveRequestReadOnlyVM
    {
        public EmployeeListVM Employee { get; set; } = new EmployeeListVM();

        [DisplayName("Additional Information")]
        public string RequestComments { get; set; }
    }
}