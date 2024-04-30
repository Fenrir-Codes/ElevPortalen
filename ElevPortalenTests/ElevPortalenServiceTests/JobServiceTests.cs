using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElevPortalen.Data;
using ElevPortalen.Models;
using ElevPortalen.Services;
using Microsoft.EntityFrameworkCore;
using Xunit.Sdk;

namespace ElevPortalenTests.ElevPortalenServiceTests {
    public class JobServiceTests {

        private readonly DbContextOptions<JobOfferDbContext> _options;
        private readonly JobOfferDbContext _context;
        private readonly JobService _jobService;

        public JobServiceTests()
        {
            // Set up the in-memory database options
            _options = new DbContextOptionsBuilder<JobOfferDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Create a new instance of the in-memory database context
            _context = new JobOfferDbContext(_options);

            // Initialize the JobService with the in-memory database context
            _jobService = new JobService(_context);
        }


        #region Create test1 - create Job Offer - create when model is valid
        [Fact]
        public async Task Create_ShouldCreateJobOffer_WhenValidJobOfferIsProvided() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); //Ensure InMemory db is clear
            var jobOffer = new JobOfferModel {
                JobOfferId = 1,
                CompanyId = 1,
                CompanyName = "TestCompany",
                ContactPerson = "John Doe",
                PhoneNumber = 123456789,
                Title = "Software Udvikler",
                JobAddress = "Gadevej1, Danmark",
                JobLink = "https://google.com/job",
                JobDetails = "Udviklere søges",
                NumberOfPositionsAvailable = 5,
                Speciality = "Datatekniker med speciale i programmering",
                DateOfPublish = DateTime.Now,
                Deadline = DateTime.Now.AddDays(30)
            };

            // Act
            var (message, isSuccess) = await _jobService.Create(jobOffer);

            // Assert
            Assert.True(isSuccess);
            Assert.Equal("Job offer has been created.", message);

            // Check if the job offer is added to the database
            var createdJobOffer = await _context.JobOfferDataBase.FirstOrDefaultAsync(j => j.JobOfferId == jobOffer.JobOfferId);
            Assert.NotNull(createdJobOffer);
        }
        #endregion

        #region Create test2 - create Job Offer - do not create duplicates with the same PK
        [Fact]
        public async Task Create_ShouldCaseAnEntityFrameworkException_WhenAttemptingToCreateTwoEntriesWithTheSamePK() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); //Ensure InMemory db is clear
            var jobOffer = new JobOfferModel {
                JobOfferId = 1,
                CompanyId = 1,
                CompanyName = "TestCompany",
                ContactPerson = "John Doe",
                PhoneNumber = 123456789,
                Title = "Software Udvikler",
                JobAddress = "Gadevej1, Danmark",
                JobLink = "https://google.com/job",
                JobDetails = "Udviklere søges",
                NumberOfPositionsAvailable = 5,
                Speciality = "Datatekniker med speciale i programmering",
                DateOfPublish = DateTime.Now,
                Deadline = DateTime.Now.AddDays(30)
            };

            // Add the job offer to the database
            _context.JobOfferDataBase.Add(jobOffer);
            await _context.SaveChangesAsync();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _jobService.Create(jobOffer));
            //Here we are not looking for the exception thrown by the method, but with EF when creating items with the same primary key
        }
        #endregion

        #region GetAllOffers test1 - Read all job offers to list - Should return all jobs
        [Fact]
        public async Task GetAllOffers_ShouldReturnAllJobOffers_WhenDataIsAvailable() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var jobOffers = new List<JobOfferModel>
            {
                new JobOfferModel {                 
                    JobOfferId = 1,
                    CompanyId = 1,
                    CompanyName = "TestCompany",
                    ContactPerson = "John Doe",
                    PhoneNumber = 123456789,
                    Title = "Software Udvikler",
                    JobAddress = "Gadevej1, Danmark",
                    JobLink = "https://google.com/job",
                    JobDetails = "Udviklere søges",
                    NumberOfPositionsAvailable = 5,
                    Speciality = "Datatekniker med speciale i programmering",
                    DateOfPublish = DateTime.Now,
                    Deadline = DateTime.Now.AddDays(30)}, 
                
                new JobOfferModel {                 
                    JobOfferId = 2,
                    CompanyId = 1,
                    CompanyName = "TestCompany",
                    ContactPerson = "John Doe",
                    PhoneNumber = 123456789,
                    Title = "Software Testere",
                    JobAddress = "Gadevej1, Danmark",
                    JobLink = "https://google.com/job",
                    JobDetails = "Testere søges",
                    NumberOfPositionsAvailable = 5,
                    Speciality = "Datatekniker med speciale i programmering",
                    DateOfPublish = DateTime.Now,
                    Deadline = DateTime.Now.AddDays(30)},

                new JobOfferModel {
                    JobOfferId = 3,
                    CompanyId = 2,
                    CompanyName = "TestCompany2",
                    ContactPerson = "Johnathan Doe",
                    PhoneNumber = 123456789,
                    Title = "Software Udviklere",
                    JobAddress = "Gadevej2221, Danmark",
                    JobLink = "https://google.com/job",
                    JobDetails = "Udviklere søges",
                    NumberOfPositionsAvailable = 2,
                    Speciality = "Datatekniker med speciale i programmering",
                    DateOfPublish = DateTime.Now,
                    Deadline = DateTime.Now.AddDays(30)},

            };
            await _context.JobOfferDataBase.AddRangeAsync(jobOffers);
            await _context.SaveChangesAsync();

            // Act
            var (offers, message, isSuccess) = await _jobService.GetAllOffers();

            // Assert
            Assert.True(isSuccess);
            Assert.Equal("Data load success", message);
            Assert.NotNull(offers);
            Assert.Equal(jobOffers.Count, offers.Count); // Check if all job offers are returned
        }
        #endregion

        #region GetAllOffers test2 - Read all job offers to list - Should return empty list when no data is available
        [Fact]
        public async Task GetAllOffers_ShouldReturnEmptyList_WhenNoDataIsAvailable() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear

            // Act
            var (offers, message, isSuccess) = await _jobService.GetAllOffers();

            // Assert
            Assert.True(isSuccess);
            Assert.Equal("Data load success", message);
            Assert.NotNull(offers);
            Assert.Empty(offers); // Check if the list of job offers is empty
        }
        #endregion

        #region GetOfferWithJobId test1 - Read one offer with the job Id - Should return the job offer when it exists
        [Fact]
        public async Task GetOfferWithJobId_ShouldReturnJobOffer_WhenItExists() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var jobOffer = new JobOfferModel{
                    JobOfferId = 1,
                    CompanyId = 1,
                    CompanyName = "TestCompany",
                    ContactPerson = "John Doe",
                    PhoneNumber = 123456789,
                    Title = "Software Udvikler",
                    JobAddress = "Gadevej1, Danmark",
                    JobLink = "https://google.com/job",
                    JobDetails = "Udviklere søges",
                    NumberOfPositionsAvailable = 5,
                    Speciality = "Datatekniker med speciale i programmering",
                    DateOfPublish = DateTime.Now,
                    Deadline = DateTime.Now.AddDays(30)
            }; 
            
            await _context.JobOfferDataBase.AddAsync(jobOffer);
            await _context.SaveChangesAsync();

            // Act
            var (message, isSuccess, offer) = await _jobService.GetOfferWithJobId(1);

            // Assert
            Assert.True(isSuccess);
            Assert.Equal("Data read successful.", message);
            Assert.NotNull(offer);
            Assert.Equal(jobOffer.JobOfferId, offer.JobOfferId); // Check if the returned job offer matches the expected one
        }
        #endregion

        #region GetOfferWithJobId test2 - Read one offer with the job Id - Should return null when the job offer does not exist
        [Fact]
        public async Task GetOfferWithJobId_ShouldReturnNull_WhenJobOfferDoesNotExist() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear

            // Act
            var (message, isSuccess, offer) = await _jobService.GetOfferWithJobId(1);

            // Assert
            Assert.True(isSuccess);
            Assert.Equal("Data read successful.", message);
            Assert.Null(offer);
        }
        #endregion

        #region GetOfferWithJobId test3 - Read one offer with the job Id - Returns based on correct id
        [Fact]
        public async Task GetOfferWithJobId_ShouldReturnOnlyOffersFromOneID_WhenOtherJobOffersExist() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var jobOffer1 = new JobOfferModel {
                JobOfferId = 1,
                CompanyId = 1,
                CompanyName = "TestCompany",
                ContactPerson = "John Doe",
                PhoneNumber = 123456789,
                Title = "Software Udvikler",
                JobAddress = "Gadevej1, Danmark",
                JobLink = "https://google.com/job",
                JobDetails = "Udviklere søges",
                NumberOfPositionsAvailable = 5,
                Speciality = "Datatekniker med speciale i programmering",
                DateOfPublish = DateTime.Now,
                Deadline = DateTime.Now.AddDays(30)
            }; 
            var jobOffer2 = new JobOfferModel {
                JobOfferId = 2,
                CompanyId = 2,
                CompanyName = "TestCompany2",
                ContactPerson = "John Doe2",
                PhoneNumber = 123456789,
                Title = "Software Udvikler",
                JobAddress = "Gadevej1, Danmark",
                JobLink = "https://google.com/job",
                JobDetails = "Udviklere søges",
                NumberOfPositionsAvailable = 5,
                Speciality = "Datatekniker med speciale i programmering",
                DateOfPublish = DateTime.Now,
                Deadline = DateTime.Now.AddDays(30)
            }; 

            await _context.JobOfferDataBase.AddRangeAsync(jobOffer1, jobOffer2);
            await _context.SaveChangesAsync();

            // Act
            var (message, isSuccess, offer) = await _jobService.GetOfferWithJobId(2); // Search for a job offer with ID 3

            // Assert
            Assert.True(isSuccess);
            Assert.Equal("Data read successful.", message);
            Assert.Equal("TestCompany2", offer.CompanyName);
            
        }
        #endregion

        #region GetOfferWithCompanyId test1 - Read one offer with the Company Id - Should return the job offer when it exists
        [Fact]
        public async Task GetOfferWithCompanyId_ShouldReturnJobOffer_WhenItExists() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var companyId = 1;
            var jobOffer = new JobOfferModel {
                JobOfferId = 1,
                CompanyId = companyId,
                CompanyName = "TestCompany",
                ContactPerson = "John Doe",
                PhoneNumber = 123456789,
                Title = "Software Udvikler",
                JobAddress = "Gadevej1, Danmark",
                JobLink = "https://google.com/job",
                JobDetails = "Udviklere søges",
                NumberOfPositionsAvailable = 5,
                Speciality = "Datatekniker med speciale i programmering",
                DateOfPublish = DateTime.Now,
                Deadline = DateTime.Now.AddDays(30)
            };

            await _context.JobOfferDataBase.AddAsync(jobOffer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _jobService.GetOfferWithCompanyId(companyId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TestCompany", result.CompanyName);
            Assert.Equal(companyId, result.CompanyId);
        }
        #endregion

        #region GetOfferWithCompanyId test2 - Read one offer with the Company Id - Should return null when the job offer does not exist
        [Fact]
        public async Task GetOfferWithCompanyId_ShouldReturnNull_WhenJobOfferDoesNotExist() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var companyId = 1; // Assuming no job offer exists for this company ID

            // Act
            var result = await _jobService.GetOfferWithCompanyId(companyId);

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region GetAllOffersByCompanyId test1 - Read all offers with the Company Id - Should return a list of job offers when they exist
        [Fact]
        public async Task GetAllOffersByCompanyId_ShouldReturnJobOffers_WhenTheyExist() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var companyId = 1;
            var jobOffer1 = new JobOfferModel {
                JobOfferId = 1,
                CompanyId = companyId,
                CompanyName = "TestCompany",
                ContactPerson = "John Doe",
                PhoneNumber = 123456789,
                Title = "Software Udvikler",
                JobAddress = "Gadevej1, Danmark",
                JobLink = "https://google.com/job",
                JobDetails = "Udviklere søges",
                NumberOfPositionsAvailable = 5,
                Speciality = "Datatekniker med speciale i programmering",
                DateOfPublish = DateTime.Now,
                Deadline = DateTime.Now.AddDays(30)
            };
            var jobOffer2 = new JobOfferModel {
                JobOfferId = 2,
                CompanyId = companyId,
                CompanyName = "TestCompany",
                ContactPerson = "John Doe",
                PhoneNumber = 123456789,
                Title = "Software Udvikler",
                JobAddress = "Gadevej1, Danmark",
                JobLink = "https://google.com/job",
                JobDetails = "Udviklere søges",
                NumberOfPositionsAvailable = 5,
                Speciality = "Datatekniker med speciale i programmering",
                DateOfPublish = DateTime.Now,
                Deadline = DateTime.Now.AddDays(30)
            };

            await _context.JobOfferDataBase.AddRangeAsync(jobOffer1, jobOffer2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _jobService.GetAllOffersByCompanyId(companyId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(companyId, result[0].CompanyId);
            Assert.Equal(companyId, result[1].CompanyId);
        }
        #endregion

        #region GetAllOffersByCompanyId test2 - Read all offers with the Company Id - Should return an empty list when no job offers exist
        [Fact]
        public async Task GetAllOffersByCompanyId_ShouldReturnEmptyList_WhenNoJobOffersExist() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var companyId = 1; // Assuming no job offers exist for this company ID

            // Act
            var result = await _jobService.GetAllOffersByCompanyId(companyId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        #endregion

        #region Update test1 - Update Job Offer - Should update the job offer when it exists
        //this test does not work - the issue could be with how context is tracking the entities or that the update method simply isn't working
        [Fact]
        public async Task Update_ShouldUpdateJobOffer_WhenItExists() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var originalJobOffer = new JobOfferModel {
                JobOfferId = 1,
                CompanyId = 1,
                CompanyName = "TestCompany",
                ContactPerson = "John Doe",
                PhoneNumber = 123456789,
                Title = "Software Udvikler",
                JobAddress = "Gadevej1, Danmark",
                JobLink = "https://google.com/job",
                JobDetails = "Udviklere søges",
                NumberOfPositionsAvailable = 5,
                Speciality = "Datatekniker med speciale i programmering",
                DateOfPublish = DateTime.Now,
                Deadline = DateTime.Now.AddDays(30)
            };

            // Add the original job offer to the database
            await _context.JobOfferDataBase.AddAsync(originalJobOffer);
            await _context.SaveChangesAsync();

            var updatedJobOffer = new JobOfferModel {
                JobOfferId = 1, 
                CompanyId = 1,
                CompanyName = "UpdatedCompany",
                ContactPerson = "John Doe",
                PhoneNumber = 123456789,
                Title = "Software Udvikler",
                JobAddress = "Gadevej1, Danmark",
                JobLink = "https://google.com/job",
                JobDetails = "Udviklere søges",
                NumberOfPositionsAvailable = 5,
                Speciality = "Datatekniker med speciale i programmering",
                DateOfPublish = DateTime.Now,
                Deadline = DateTime.Now.AddDays(30)
            };

            // Act

            var (message, isSuccess) = await _jobService.Update(updatedJobOffer);

            // Assert
            Assert.True(isSuccess);
            Assert.Equal("Job offer updated.", message);

            // Retrieve the updated job offer from the database
            var updatedOffer = await _context.JobOfferDataBase.AsNoTracking()
                                    .FirstOrDefaultAsync(offer => offer.JobOfferId == updatedJobOffer.JobOfferId);
            Assert.NotNull(updatedOffer);
            Assert.Equal(updatedJobOffer.CompanyName, updatedOffer.CompanyName);
        }
        #endregion

        #region Update test2 - Update Job Offer - Should return an error when the job offer does not exist
        [Fact]
        public async Task Update_ShouldReturnErrorMessage_WhenJobOfferDoesNotExist() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var updatedJobOffer = new JobOfferModel {
                JobOfferId = 1, // Assuming no job offer with this ID exists
                CompanyId = 1,
                CompanyName = "TestCompany",
                ContactPerson = "John Doe",
                PhoneNumber = 123456789,
                Title = "Software Udvikler",
                JobAddress = "Gadevej1, Danmark",
                JobLink = "https://google.com/job",
                JobDetails = "Udviklere søges",
                NumberOfPositionsAvailable = 5,
                Speciality = "Datatekniker med speciale i programmering",
                DateOfPublish = DateTime.Now,
                Deadline = DateTime.Now.AddDays(30)
            };

            // Act
            var (message, isSuccess) = await _jobService.Update(updatedJobOffer);

            // Assert
            Assert.False(isSuccess);
            Assert.Equal("Job offer not found.", message);
        }
        #endregion

        #region DeleteOffer test1 - Delete job offer by Id - Should delete the job offer when it exists
        [Fact]
        public async Task DeleteOffer_ShouldDeleteJobOffer_WhenItExists() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var jobOffer = new JobOfferModel {
                JobOfferId = 1,
                CompanyId = 1,
                CompanyName = "TestCompany",
                ContactPerson = "John Doe",
                PhoneNumber = 123456789,
                Title = "Software Udvikler",
                JobAddress = "Gadevej1, Danmark",
                JobLink = "https://google.com/job",
                JobDetails = "Udviklere søges",
                NumberOfPositionsAvailable = 5,
                Speciality = "Datatekniker med speciale i programmering",
                DateOfPublish = DateTime.Now,
                Deadline = DateTime.Now.AddDays(30)
            };

            await _context.JobOfferDataBase.AddAsync(jobOffer);
            await _context.SaveChangesAsync();

            // Act
            var (message, isSuccess) = await _jobService.DeleteOffer(1);

            // Assert
            Assert.True(isSuccess);
            Assert.Equal("JobOffer deleted successfully.", message);

            // Ensure that the job offer is no longer in the database
            var deletedOffer = await _context.JobOfferDataBase.FindAsync(1);
            Assert.Null(deletedOffer);
        }
        #endregion

        #region DeleteOffer test2 - Delete job offer by Id - Should return error message when job offer does not exist
        [Fact]
        public async Task DeleteOffer_ShouldReturnErrorMessage_WhenJobOfferDoesNotExist() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear

            // Act
            var (message, isSuccess) = await _jobService.DeleteOffer(1);

            // Assert
            Assert.False(isSuccess);
            Assert.Equal("Error while delete Joboffer.", message);
        }
        #endregion

        #region DeleteOffersByCompanyId test1 - Delete all job offers by Company Id - Should delete all job offers for the company when they exist
        [Fact]
        public async Task DeleteOffersByCompanyId_ShouldDeleteAllJobOffers_WhenTheyExist() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var companyId = 1;

            var jobOffers = new List<JobOfferModel>
            {
                new JobOfferModel
                {
                    JobOfferId = 1,
                    CompanyId = companyId,
                    CompanyName = "TestCompany",
                    ContactPerson = "John Doe",
                    PhoneNumber = 123456789,
                    Title = "Software Udvikler",
                    JobAddress = "Gadevej1, Danmark",
                    JobLink = "https://google.com/job",
                    JobDetails = "Udviklere søges",
                    NumberOfPositionsAvailable = 5,
                    Speciality = "Datatekniker med speciale i programmering",
                    DateOfPublish = DateTime.Now,
                    Deadline = DateTime.Now.AddDays(30)
                },
                new JobOfferModel
                {
                    JobOfferId = 2,
                    CompanyId = companyId,
                    CompanyName = "TestCompany",
                    ContactPerson = "John Doe",
                    PhoneNumber = 123456789,
                    Title = "Software Tester",
                    JobAddress = "Gadevej1, Danmark",
                    JobLink = "https://google.com/job",
                    JobDetails = "Testere søges",
                    NumberOfPositionsAvailable = 5,
                    Speciality = "Datatekniker med speciale i programmering",
                    DateOfPublish = DateTime.Now,
                    Deadline = DateTime.Now.AddDays(30)
                },
                // Add more job offers as needed
    };

            await _context.JobOfferDataBase.AddRangeAsync(jobOffers);
            await _context.SaveChangesAsync();

            // Act
            var (message, isSuccess) = await _jobService.DeleteOffersByCompanyId(companyId);

            // Assert
            Assert.True(isSuccess);
            Assert.Equal("All offers for company  deleted successfully.", message);

            // Ensure that all job offers for the company are deleted from the database
            var deletedOffers = await _context.JobOfferDataBase.Where(offer => offer.CompanyId == companyId).ToListAsync();
            Assert.Empty(deletedOffers);
        }
        #endregion

        #region DeleteOffersByCompanyId test2 - Delete all job offers by Company Id - Should return error message when no job offers exist for the company
        [Fact]
        public async Task DeleteOffersByCompanyId_ShouldReturnErrorMessage_WhenNoJobOffersExistForTheCompany() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var companyId = 1; // Assuming this company ID does not have any job offers

            // Act
            var (message, isSuccess) = await _jobService.DeleteOffersByCompanyId(companyId);

            // Assert
            Assert.False(isSuccess);
            Assert.Equal("No offers found for this company.", message);
        }
        #endregion



        #region
        #endregion

        #region TEMPLATE
        [Fact]
        public async void Method() {

            //ARRANGE
            await _context.Database.EnsureDeletedAsync(); //Ensure InMemory db is clear
            //_context.Database.CloseConnection();

            //ACT


            //ASSERT

        }
        #endregion


    } //jobservicetests class bracket end
}
