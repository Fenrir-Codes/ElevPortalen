using ElevPortalen.Data;
using ElevPortalen.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ElevPortalen.Services
{
    //Lavet af Jozsef
    public class CompanyService
    {
        private readonly ElevPortalenDataDbContext _context;
        private readonly IDataProtector? _dataProtector;

        #region constructor
        public CompanyService(ElevPortalenDataDbContext context, IDataProtectionProvider dataProtectionProvider)
        {
            _context = context;
            _dataProtector = dataProtectionProvider.CreateProtector("ProtectData"); 
            //i just placed it here if need, we can use it to protect data
        }
        #endregion

        #region create Company function async
        public async Task<string> CreateCompany(CompanyModel company)
        {
            try
            {
                _context.Company.Add(company); // Add input to context variables
                await _context.SaveChangesAsync(); // Save data

                return $"Company Profile Created";
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error message
                return $"An error har ocurred: {ex.Message}";
            }
        }
        #endregion

        #region Get Company request
        public async Task<List<CompanyModel>> ReadData(ClaimsPrincipal _user)
        {
            try
            {
                //Get all the data
                var response = await _context.Company.AsNoTracking().Where(user => user.UserId
                    == Guid.Parse(_user.FindFirstValue(ClaimTypes.NameIdentifier))).ToListAsync();

                return response; // return the decrypted data
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving Company data." + ex.Message);
            }
        }
        #endregion

        #region Get All Data from Company
        public async Task<List<CompanyModel>> ReadAllCompanyData()
        {
            try
            {
                //Get all the data
                var response = await _context.Company.AsNoTracking().ToListAsync();

                return response; // return the data
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving Company data." + ex.Message);
            }
        }
        #endregion

        #region Company Update function
        public async Task<string> Update(CompanyModel company)
        {
            try
            {
                var entry = await _context.Company.FindAsync(company.CompanyId);

                // If the response is not null
                if (entry != null)
                {
                    entry.CompanyName = company.CompanyName;
                    entry.CompanyAddress = company.CompanyAddress;
                    entry.Region = company.Region;
                    entry.Email = company.Email;
                    entry.Link = company.Link;
                    entry.Preferences = company.Preferences;
                    entry.Description = company.Description;
                    entry.PhoneNumber = company.PhoneNumber;
                    entry.IsHiring = company.IsHiring;
                    entry.IsVisible = company.IsVisible;

                    _context.Entry(entry).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    return $"Updated successfully";
                }
                else
                {
                    return $"Entry not found"; // Return a message when the entry is not found
                }

            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}"; // Return an error message if an exception occurs
            }
        }
        #endregion

        #region Delete Company function
        public async Task<string> Delete(CompanyModel company)
        {
            try
            {
                var entryToRemove = _context.Company.Local.FirstOrDefault(c => c.CompanyId == company.CompanyId);
                if (entryToRemove != null)
                {
                    _context.Entry(entryToRemove).State = EntityState.Detached;
                }
                _context.Company.Remove(company);
                await _context.SaveChangesAsync();

                return "The Company's Profile deleted Successfully.";
            }
            catch (Exception ex)
            {
                return $"An error has ocurred: {ex.Message}";
            }
        }
        #endregion

        #region Get Company by Id
        public async Task<CompanyModel> GetCompanyById(int companyId)
        {
            try
            {
                var company = await _context.Company.AsNoTracking().FirstOrDefaultAsync(c => c.CompanyId == companyId);
                if (company != null)
                {
                    return company;
                }
                else
                {
                    throw new ApplicationException("Company not found.");
                }
                
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving Company data: {ex.Message}");
            }
        }
        #endregion

    }
}
