// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ElevPortalen.Services;
using Microsoft.EntityFrameworkCore;

namespace ElevPortalen.Areas.Identity.Pages.Account.Manage
{
    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<DeletePersonalDataModel> _logger;
        private readonly StudentService _studentService;
        private readonly CompanyService _companyService;

        public DeletePersonalDataModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<DeletePersonalDataModel> logger,
            StudentService studentService,
            CompanyService companyService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _studentService = studentService;
            _companyService = companyService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool RequirePassword { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            if (RequirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Incorrect password.");
                    return Page();
                }
            }

            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);

            // Added - deleting the Student data from the database on account delete.
            await DeleteProfileData(userId);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user.");
            }

            await _signInManager.SignOutAsync();

            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            return Redirect("~/");
        }

        private async Task<IActionResult> DeleteProfileData(string userId)
        {
            Guid userGuid;
            // Convert string userId to Guid
            if (Guid.TryParse(userId, out userGuid))
            {
                // Check in the student database
                var deleteStudentProfileData = await _studentService.GetStudentByGuid(userGuid);
                if (deleteStudentProfileData != null)
                {
                    await _studentService.Delete(deleteStudentProfileData.StudentId);
                }
                else
                {
                    // Check in the company database
                    var deleteCompanyProfileData = await _companyService.GetCompanyByGuid(userGuid);
                    if (deleteCompanyProfileData != null)
                    {
                        await _companyService.Delete(deleteCompanyProfileData.CompanyId);
                    }
                    else
                    {
                        // User not found in either database, sign out and redirect
                        await _signInManager.SignOutAsync();
                        return Redirect("~/");
                    }
                }
            }
            else
            {
                // Invalid ID request
                throw new InvalidOperationException("Invalid Id request");
            }

            // Add a return statement in case the method doesn't return in any of the conditions above
            return RedirectToPage(); // Or return any other appropriate result
        }


    }
}
