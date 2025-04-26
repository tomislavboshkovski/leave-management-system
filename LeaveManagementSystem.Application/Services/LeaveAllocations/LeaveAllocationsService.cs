
using AutoMapper;
using LeaveManagementSystem.Application.Models.LeaveAllocations;
using LeaveManagementSystem.Application.Services.Periods;
using LeaveManagementSystem.Application.Services.Users;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Application.Services.LeaveAllocations;

public class LeaveAllocationsService(ApplicationDbContext _context, IUserService _userService, IMapper _mapper, IPeriodsService _periodsService) : ILeaveAllocationsService
{
    public async Task AllocateLeave(string employeeId)
    {
        var leaveTypes = await _context.LeaveTypes
            .Where(q => !q.LeaveAllocations.Any(x => x.EmployeeId == employeeId))
            .ToListAsync();

        var period = await _periodsService.GetCurrentPeriod();
        var today = DateTime.Today;
        var isInSecondHalf = today.Month >= 1 && today.Month <= 6;

        foreach (var leaveType in leaveTypes)
        {
            int days;

            switch (leaveType.Name)
            {
                case "Parental Leave":
                    // Always allocate the full amount (e.g., 270 days)
                    days = leaveType.NumberOfDays;
                    break;

                case "Annual Leave":
                    // Allocate based on registration date
                    days = isInSecondHalf ? 11 : 21;
                    break;

                default:
                    // For Sick and Miscellaneous Leave, give the full amount
                    days = leaveType.NumberOfDays;
                    break;
            }

            var leaveAllocation = new LeaveAllocation
            {
                EmployeeId = employeeId,
                LeaveTypeId = leaveType.Id,
                PeriodId = period.Id,
                Days = days
            };

            _context.LeaveAllocations.Add(leaveAllocation);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<EmployeeAllocationVM> GetEmployeeAllocations(string? userId)
    {
        var user = string.IsNullOrEmpty(userId)
            ? await _userService.GetLoggedInUser()
            : await _userService.GetUserById(userId);

        var allocations = await GetAllocations(user.Id);
        var allocationVmList = _mapper.Map<List<LeaveAllocation>, List<LeaveAllocationVM>>(allocations);
        var leaveTypesCount = await _context.LeaveTypes.CountAsync();

        var employeeVm = new EmployeeAllocationVM
        {
            DateOfBirth = user.DateOfBirth,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Id = user.Id,
            LeaveAllocations = allocationVmList,
            IsCompletedAllocation = leaveTypesCount == allocations.Count
        };

        return employeeVm;
    }

    public async Task<List<EmployeeListVM>> GetEmployees()
    {
        var users = await _userService.GetEmployees();
        var employees = _mapper.Map<List<ApplicationUser>, List<EmployeeListVM>>(users.ToList());

        return employees;
    }

    public async Task<LeaveAllocationEditVM> GetEmployeeAllocation(int allocationId)
    {
        var allocation = await _context.LeaveAllocations
               .Include(q => q.LeaveType)
               .Include(q => q.Employee)
               .FirstOrDefaultAsync(q => q.Id == allocationId);

        var model = _mapper.Map<LeaveAllocationEditVM>(allocation);

        return model;
    }

    public async Task EditAllocation(LeaveAllocationEditVM allocationEditVm)
    {
        //var leaveAllocation = await GetEmployeeAllocation(allocationEditVm.Id);
        //if (leaveAllocation == null)
        //{
        //    throw new Exception("Leave allocation record does not exist.");
        //}
        //leaveAllocation.Days = allocationEditVm.Days;
        // option 1 _context.Update(leaveAllocation);
        // option 2 _context.Entry(leaveAllocation).State = EntityState.Modified;
        // await _context.SaveChangesAsync();

        await _context.LeaveAllocations
            .Where(q => q.Id == allocationEditVm.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(e => e.Days, allocationEditVm.Days));
    }

    public async Task<LeaveAllocation> GetCurrentAllocation(int leaveTypeId, string employeeId)
    {
        var period = await _periodsService.GetCurrentPeriod();
        var allocation = await _context.LeaveAllocations
                .FirstAsync(q => q.LeaveTypeId == leaveTypeId
                && q.EmployeeId == employeeId
                && q.PeriodId == period.Id);
        return allocation;
    }
    private async Task<List<LeaveAllocation>> GetAllocations(string? userId)
    {
        var period = await _periodsService.GetCurrentPeriod();
        var leaveAllocations = await _context.LeaveAllocations
           .Include(q => q.LeaveType)
           .Include(q => q.Period)
           .Where(q => q.EmployeeId == userId && q.Period.Id == period.Id)
           .ToListAsync();
        return leaveAllocations;
    }

    private async Task<bool> AllocationExists(string userId, int periodId, int leaveTypeId)
    {
        var exists = await _context.LeaveAllocations.AnyAsync(q =>
            q.EmployeeId == userId
            && q.LeaveTypeId == leaveTypeId
            && q.PeriodId == periodId
        );

        return exists;
    }

    public async Task<LeaveAllocation> GetLoggedInUserAllocation(int leaveTypeId)
    {
        var user = await _userService.GetLoggedInUser();
        return await GetCurrentAllocation(leaveTypeId, user.Id);
    }

}