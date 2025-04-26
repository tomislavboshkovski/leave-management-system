using LeaveManagementSystem.Application.Models.LeaveRequests;
using LeaveManagementSystem.Application.Services.LeaveAllocations;
using LeaveManagementSystem.Application.Services.LeaveRequests;
using LeaveManagementSystem.Application.Services.LeaveTypes;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeaveManagementSystem.Web.Controllers;

[Authorize]
public class LeaveRequestsController(ILeaveTypesService _leaveTypesService, ILeaveRequestsService _leaveRequestsService, ILeaveAllocationsService _leaveAllocationsService ) : Controller
{
    // Employee View requests
    public async Task<IActionResult> Index()
    {
        var model = await _leaveRequestsService.GetEmployeeLeaveRequests();
        return View(model);
    }

    // Employee Create requests
    public async Task<IActionResult> Create(int? leaveTypeId)
    {
        var leaveTypes = await _leaveTypesService.GetAll();
        var leaveTypesList = new SelectList(leaveTypes, "Id", "Name", leaveTypeId);
        var model = new LeaveRequestCreateVM
        {
            StartDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            LeaveTypes = leaveTypesList,
        };
        return View(model);
    }

    // Employee Create requests
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LeaveRequestCreateVM model)
    {
        // Run validation rules
        await ValidateLeaveRequestAsync(model);

        if (ModelState.IsValid)
        {
            await _leaveRequestsService.CreateLeaveRequest(model);
            return RedirectToAction(nameof(Index));
        }

        // Reload leave types if validation failed
        var leaveTypes = await _leaveTypesService.GetAll();
        model.LeaveTypes = new SelectList(leaveTypes, "Id", "Name");
        return View(model);
    }


    // Employee Cancel requests
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        await _leaveRequestsService.CancelLeaveRequest(id);
        return RedirectToAction(nameof(Index));
    }

    // Admin/Supe review requests
    [Authorize(Policy = "AdminSupervisorOnly")]
    public async Task<IActionResult> ListRequests()
    {
        var model = await _leaveRequestsService.AdminGetAllLeaveRequests();
        return View(model);
    }

    // Admin/Supe review requests
    public async Task<IActionResult> Review(int id)
    {
        var model = await _leaveRequestsService.GetLeaveRequestForReview(id);
        return View(model);
    }

    // Admin/Supe review requests
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Review(int id, bool approved)
    {
        await _leaveRequestsService.ReviewLeaveRequest(id, approved);
        return RedirectToAction(nameof(ListRequests));
    }

    private async Task ValidateLeaveRequestAsync(LeaveRequestCreateVM model)
    {
        var leaveType = await _leaveTypesService.Get<LeaveType>(model.LeaveTypeId);
        var allocation = await _leaveAllocationsService.GetLoggedInUserAllocation(model.LeaveTypeId);
        var requestedDays = (model.EndDate.DayNumber - model.StartDate.DayNumber) + 1;
        var today = DateTime.Today;

        // Check against allocation
        if (await _leaveRequestsService.RequestDatesExceedAllocation(model))
        {
            ModelState.AddModelError(string.Empty, "You have exceeded your allocation");
            ModelState.AddModelError(nameof(model.EndDate), "The number of days requested is invalid.");
        }

        // Rule: Annual Leave must be at least 10 days if requested July–Dec
        if (leaveType != null && leaveType.Name == "Annual Leave" &&
                today.Month >= 7 && today.Month <= 12 &&
                requestedDays < 10)
        {
            ModelState.AddModelError("NumberOfDays", "Annual Leave from July to December must be at least 10 days.");
        }


        // Rule: Sick and Miscellaneous leave must be between 1 and max available
        if ((leaveType != null && leaveType.Name == "Sick Leave" || leaveType.Name == "Miscellaneous Leave") &&
            (requestedDays < 1 || requestedDays > allocation.Days))
        {
            ModelState.AddModelError("NumberOfDays", $"You can request between 1 and {allocation.Days} day(s) for {leaveType.Name}.");
        }
    }


}
