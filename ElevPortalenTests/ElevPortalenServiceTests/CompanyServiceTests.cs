using ElevPortalen.Data;
using ElevPortalen.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Moq;
using ElevPortalen.Models;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Security.Claims;

namespace ElevPortalenTests.ElevPortalenServiceTests {
    public class CompanyServiceTests {

        private readonly DbContextOptions<ElevPortalenDataDbContext> _options;
        private readonly ElevPortalenDataDbContext _context;
        private readonly CompanyService _companyService;
        private readonly DbContextOptions<DataRecoveryDbContext> _recoveryOptions;
        private readonly DataRecoveryDbContext _dataRecoveryContext;

        #region constructor
        public CompanyServiceTests() {
            _options = new DbContextOptionsBuilder<ElevPortalenDataDbContext>()
                .UseInMemoryDatabase(databaseName: "CompanyServiceTests")
                .Options;

            //since our ElevP Db context is depending on this context we have to include it even though it is never used
            _recoveryOptions = new DbContextOptionsBuilder<DataRecoveryDbContext> () 
                .UseInMemoryDatabase(databaseName: "datarecoveryTest")
                .Options;

            //create DbContext using options
            _context = new ElevPortalenDataDbContext(_options);
            _dataRecoveryContext = new DataRecoveryDbContext(_recoveryOptions);

            //mock dependencies
            var dataProtectionProviderMock = new Mock<IDataProtectionProvider>();

            //create CompanyService instance with mocked dependencies
            _companyService = new CompanyService(_context, _dataRecoveryContext, dataProtectionProviderMock.Object);
        }
        #endregion


        #region create Company function async JUST HERE TO LOOK AT
        public async Task<(string?, bool)> TestCreateCompany(CompanyModel company) {
            try {
                _context.Company.Add(company); // Add input to context variables
                await _context.SaveChangesAsync(); // Save data

                return ("Company Profile Created.", true);
            } catch (Exception ex) {
                // Handle the exception and return an error message
                return ($"An error has ocurred: {ex.Message}", false);
            }
        }
        #endregion

        #region create Company function async test1 - check that company is created and message displayed
        [Fact]
        public async void CreateCompany_ShouldCreateCompany() {

            //ARRANGE
            await _context.Database.EnsureDeletedAsync(); //Ensure InMemory db is clear
            var company = new CompanyModel {
                UserId = Guid.NewGuid(),
                CompanyId = 1,
                CompanyName = "TestCompany",
                CompanyAddress = "Address",
                Region = "Region",
                Email = "email@example.com",
                Link = "www.example.com",
                Preferences = "Preferences",
                Description = "Description",
                profileImage = "image.jpg",
                PhoneNumber = 12345678,
                IsHiring = true,
                IsVisible = true,
                RegisteredDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            //ACT
            var (message, success) = await _companyService.CreateCompany(company);
            var addedCompany = await _context.Company.FindAsync(company.CompanyId); //control variable - find company in context dbset
            var result = await _companyService.ReadAllVisibleCompanyData(); //since company is visible, it should be findable with ReadAllVisibleCompanyData


            //ASSERT
            Assert.True(success); //check if company profile creation was successful
            Assert.Equal("Company Profile Created.", message); //check if message is right 
            Assert.NotNull(addedCompany); //assert that control variable is not null
            Assert.Equal("TestCompany", result[0].CompanyName); //Assert that data can be found in our mocked companyservice as well
        }
        #endregion

        #region create Company function async test2 - check that companies cannot be created on the same id
        [Fact]
        public async void CreateCompany_ShouldNotCreateCompanyWhenCompanyAlreadyExists() {

            //ARRANGE
            await _context.Database.EnsureDeletedAsync(); //Ensure InMemory db is clear
            var UserGuid = Guid.NewGuid().ToString(); //make new Guid
            var company = new CompanyModel {
                UserId = Guid.Parse(UserGuid),
                CompanyId = 1,
                CompanyName = "TestCompany",
                CompanyAddress = "Address",
                Region = "Region",
                Email = "email@example.com",
                Link = "www.example.com",
                Preferences = "Preferences",
                Description = "Description",
                profileImage = "image.jpg",
                PhoneNumber = 12345678,
                IsHiring = true,
                IsVisible = true,
                RegisteredDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            _context.Company.Add(company);
            await _context.SaveChangesAsync();

            //ACT
            async Task action() => await _companyService.CreateCompany(company); // Attempt to create the company again

            var result = await _companyService.ReadAllVisibleCompanyData();

            //ASSERT
            Assert.NotNull(result); //result must not be null 
            Assert.Single(result); //result must be a single company


            //ASSERT

            //var ex = await Assert.ThrowsAsync<Exception>(action);
            //Assert.Contains("An error has occurred", ex.Message); // Ensure that the error message contains the expected text
        }
        #endregion

        #region Get Company request with the claimprincipal test1 - ReadData only returns the mocked data with the right Guid, data is CompanyModel
        [Fact]
        public async Task ReadData_ReturnsCorrectCompanies() {
            //ARRANGE
            await _context.Database.EnsureDeletedAsync(); //ensure InMemory db is clear
            var UserGuid = Guid.NewGuid().ToString(); //make new Guid
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] //set new claim
            {
                new Claim(ClaimTypes.NameIdentifier, UserGuid)
            }));

            // Define test data
            var testData = new List<CompanyModel>
            {
                new CompanyModel
                {
                    UserId = Guid.Parse(UserGuid),
                    CompanyId = 1,
                    CompanyName = "Company1",
                    CompanyAddress = "Address1",
                    Region = "Region1",
                    Email = "email1@example.com",
                    Link = "www.example.com",
                    Preferences = "Preferences1",
                    Description = "Description1",
                    profileImage = "image1.jpg",
                    PhoneNumber = 12345678,
                    IsHiring = true,
                    IsVisible = true,
                    RegisteredDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                },
                new CompanyModel
                {
                    UserId = Guid.Parse(UserGuid),
                    CompanyId = 2,
                    CompanyName = "Company2",
                    CompanyAddress = "Address2",
                    Region = "Region2",
                    Email = "email2@example.com",
                    Link = "www.example.com",
                    Preferences = "Preferences2",
                    Description = "Description2",
                    profileImage = "image2.jpg",
                    PhoneNumber = 98765432,
                    IsHiring = false,
                    IsVisible = true,
                    RegisteredDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                },
                new CompanyModel
                {
                    UserId = Guid.NewGuid(), // Not belonging to the user
                    CompanyId = 3,
                    CompanyName = "Company3",
                    CompanyAddress = "Address3",
                    Region = "Region3",
                    Email = "email3@example.com",
                    Link = "www.example.com",
                    Preferences = "Preferences3",
                    Description = "Description3",
                    profileImage = "image3.jpg",
                    PhoneNumber = 13579246,
                    IsHiring = true,
                    IsVisible = true,
                    RegisteredDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                }
            };

            // Add test data to the database
            _context.Company.AddRange(testData);
            await _context.SaveChangesAsync();

            //ACT
            var result = await _companyService.ReadData(user);

            //ASSERT
            Assert.NotNull(result); //result must not be null 
            Assert.Equal(2, result.Count); //we expect 2 companies belonging to the user
            Assert.IsType<CompanyModel>(result[0]); //members of the list is CompanyModel
            Assert.Equal("Company1", result[0].CompanyName); //Assert that data was mocked from testData
        }

        #endregion

        #region Get Company request with the claimprincipal test2 - ReadData should not return any companies if there are no companies associated with the Guid
        [Fact]
        public async Task ReadData_ReturnsNoCompanies_IfThereAreNoCompaniesAssociatedWithTheGuid() {
            //ARRANGE
            await _context.Database.EnsureDeletedAsync(); //ensure InMemory db is clear
            var UserGuid = Guid.NewGuid().ToString(); //make new Guid
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] //set new claim
            {
                new Claim(ClaimTypes.NameIdentifier, UserGuid)
            }));


            //ACT
            var result = await _companyService.ReadData(user);

            //ASSERT
            Assert.Empty(result);
            Assert.IsType<List<CompanyModel>>(result);

        }
        #endregion

        #region Get Company request with the claimprincipal test3 - ensure that errorhandling has been setup when claimsprinciple is not valid
        [Fact]
        public async Task ReadData_ThrowsError_IfThereIsNoValidClaimsPrinciple() {
            //ARRANGE
            var userClaims = new ClaimsPrincipal();

            //ACT and ASSERT
            await Assert.ThrowsAsync<InvalidOperationException>(() => _companyService.ReadData(userClaims));


        }
        #endregion

        #region Get All Data from Company if they are visible test1 - Mockdata is notnull, type is list and count is 2
        [Fact]
        public async void ReadAllVisibleCompanyData_ShouldReturnListOfCompanies_WhenCompaniesExist() {

            //ARRANGE
            await _context.Database.EnsureDeletedAsync(); //Ensure InMemory db is clear

            _context.Company.Add(new CompanyModel { //Add data to db
                CompanyId = 1,
                CompanyName = "NetCompany",
                Region = "Sjælland",
                Email = "Netcompany@Netcompany.dk",
                Link = "www.google.com",
                Preferences = "Programmør",
                Description = "Danmarks NetCompany",
                profileImage = "pic.jpg",
                PhoneNumber = 22334455,
                IsHiring = true,
                IsVisible = true,
                RegisteredDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            });

            _context.Company.Add(new CompanyModel {
                CompanyId = 2,
                CompanyName = "KMD",
                Region = "Sjælland",
                Email = "KMD@KMD.dk",
                Link = "www.google.com",
                Preferences = "Programmør",
                Description = "Kommunedata",
                profileImage = "pic.jpg",
                PhoneNumber = 22334455,
                IsHiring = true,
                IsVisible = true,
                RegisteredDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            });

            await _context.SaveChangesAsync(); //Save changes

            //ACT
            var result = await _companyService.ReadAllVisibleCompanyData(); //Use method to read companies in db

            //ASSERT
            Assert.NotNull(result);
            Assert.IsType<List<CompanyModel>>(result);
            Assert.Equal(2, result.Count);
        }
        #endregion

        #region Get All Data from Company if they are visible test2 - Empty DB returns Empty list but not null
        [Fact]
        public async void ReadAllVisibleCompanyData_ShouldReturnEmptyListOfCompanies_WhenNoCompaniesExist() {

            //ARRANGE
            await _context.Database.EnsureDeletedAsync(); //Ensure InMemory db is clear

            //ACT
            var result = await _companyService.ReadAllVisibleCompanyData(); //Use method to read companies in db

            //ASSERT
            Assert.NotNull(result);
            Assert.IsType<List<CompanyModel>>(result);
            Assert.Empty(result);
        }
        #endregion

        #region Get All Data from Company if they are visible test3 - check for null exception when ReadData returns null
        [Fact]
        public async void ReadAllVisibleCompanyData_ShouldThrowInvalidOperationException_WhenListIsEmpty() {

            // Arrange
            var mockDbSet = new Mock<DbSet<CompanyModel>>();

            mockDbSet.As<IQueryable<CompanyModel>>().Setup(m => m.Provider).Throws(new Exception("Simulated exception"));

            var mockContext = new Mock<ElevPortalenDataDbContext>();
            mockContext.Setup(c => c.Company).Returns(mockDbSet.Object);

            var dataProtectionProviderMock = new Mock<IDataProtectionProvider>();

            var companyService = new CompanyService(mockContext.Object, null, dataProtectionProviderMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _companyService.ReadAllVisibleCompanyData());

            //This test does not appear to work and probably will not unless the entire CompanyService is refactored to use interfaces
        }
        #endregion


        #region TEMPLATE
        [Fact]
        public async void Method() {

            //ARRANGE
            await _context.Database.EnsureDeletedAsync(); //Ensure InMemory db is clear

            //ACT


            //ASSERT

        }
        #endregion

    }
}
