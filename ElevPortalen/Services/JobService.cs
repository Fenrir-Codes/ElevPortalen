using ElevPortalen.Data;
using ElevPortalen.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace ElevPortalen.Services
{
    public class JobService
    {
        private readonly JobOfferDbContext _context;

        #region constructor
        public JobService(JobOfferDbContext context)
        {
            _context = context;
        }
        #endregion

        #region create Job Offer
        public async Task<(string, bool)> Create(JobOfferModel Job)
        {
            try
            {
                _context.JobOfferDataBase.Add(Job); //Add data
                await _context.SaveChangesAsync(); // Save data

                return ("Job offer has been created.", true);
            }
            catch (DbUpdateException ex)
            {
                // Handle the exception and return an error message
                return ($"An error has ocurred: {ex.Message}", false);
            }
        }
        #endregion

        #region Read all job offers to list
        public async Task<(List<JobOfferModel>?, string, bool)> GetAllOffers()
        {
            try
            {
                var response = await _context.JobOfferDataBase.AsNoTracking().ToListAsync();
                return (response, "Data load success", true);
            }
            catch (DbException ex)
            {
                // Handle the exception, log, and return an error message along with false
                return (null, $"An error occurred: {ex.Message}", false);
            }
        }
        #endregion

        #region Read one offer with the job Id
        public async Task<JobOfferModel?> GetOfferWithJobId(int JobId)
        {
            try
            {
                return await _context.JobOfferDataBase.FirstOrDefaultAsync(offer => offer.JobOfferId == JobId);
            }
            catch (Exception ex)
            {
                // Handle the exception, log, and return null or throw an error as appropriate
                throw new Exception($"An error occurred : {ex.Message}");
            }
        }
        #endregion

        #region Read one offer with the Company Id
        public async Task<JobOfferModel?> GetOfferWithCompanyId(int companyId)
        {
            try
            {
                return await _context.JobOfferDataBase.FirstOrDefaultAsync(offer => offer.CompanyId == companyId);
            }
            catch (Exception ex)
            {
                // Handle the exception, log, and return null or throw an error as appropriate
                throw new Exception($"An error occurred : {ex.Message}");
            }
        }
        #endregion

        #region Read all offers with the Company Id
        public async Task<List<JobOfferModel>> GetAllOffersByCompanyId(int companyId)
        {
            try
            {
                return await _context.JobOfferDataBase.Where(offer => offer.CompanyId == companyId).ToListAsync();
            }
            catch (Exception ex)
            {
                // Handle the exception, log, and return null or throw an error as appropriate
                throw new Exception($"An error occurred : {ex.Message}");
            }
        }
        #endregion

        #region Update Job Offer
        public async Task<(string, bool)> Update(JobOfferModel updatedJob)
        {
            try
            {
                var JobToUpdate = await _context.JobOfferDataBase.FindAsync(updatedJob.JobOfferId);

                // If the response is not null
                if (JobToUpdate != null)
                {
                    JobToUpdate.Title = updatedJob.Title;
                    JobToUpdate.JobAddress = updatedJob.JobAddress;
                    JobToUpdate.JobLink = updatedJob.JobLink;
                    JobToUpdate.JobDetails = updatedJob.JobDetails;
                    JobToUpdate.NumberOfPositionsAvailable = updatedJob.NumberOfPositionsAvailable;
                    JobToUpdate.Speciality = updatedJob.Speciality;
                    JobToUpdate.DateOfPublish = updatedJob.DateOfPublish;
                    JobToUpdate.Deadline = updatedJob.Deadline;

                    _context.Entry(JobToUpdate).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    return ("Job offer updated.", true);
                }
                else
                {
                    return ("Job offer not found.", false);
                    // Return a message when the offer is not found
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while updating data in the database: {ex.Message}"); 
                // Return an error message if an exception occurs
            }
        }

        #endregion

        #region delete job offer by Id (just one offer)
        public async Task<(string, bool)> DeleteOffer(int jobOfferId)
        {
            try
            {
                var offerToDelete = await _context.JobOfferDataBase.FindAsync(jobOfferId);
                if (offerToDelete == null)
                {
                    return ("Error while delete Joboffer.", false);
                }

                _context.JobOfferDataBase.Remove(offerToDelete);
                await _context.SaveChangesAsync();

                return ("JobOffer deleted successfully.", true);
            }
            catch (DbUpdateException ex)
            {
                // Handle the exception and return an error message
                throw new InvalidOperationException($"An error occurred while deleting data from the database: {ex.Message}");
            }
        }
        #endregion

        #region Delete function for all the job offers a company have in the database (all offers with the company id)
        public async Task<(string, bool)> DeleteOffersByCompanyId(int companyId)
        {
            try
            {
                var offersToDelete = _context.JobOfferDataBase.Where(offer => offer.CompanyId == companyId);

                if (!offersToDelete.Any())
                {
                    return ($"No offers found for this company.", false);
                }

                _context.JobOfferDataBase.RemoveRange(offersToDelete);
                await _context.SaveChangesAsync();

                return ($"All offers for company  deleted successfully.", true);
            }
            catch (DbUpdateException ex)
            {
                // Handle the exception and return an error message
                throw new InvalidOperationException($"An error occurred while deleting data from the database: {ex.Message}");
            }
        }
        #endregion


    }
}
