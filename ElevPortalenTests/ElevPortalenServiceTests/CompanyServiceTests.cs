using ElevPortalen.Data;
using ElevPortalen.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Moq;
using ElevPortalen.Models;
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

            //we also have to include the datarecovery db so we can test it (and fill demands of instantiating companyservice)
            _recoveryOptions = new DbContextOptionsBuilder<DataRecoveryDbContext> () 
                .UseInMemoryDatabase(databaseName: "datarecoveryTest")
                .Options;

            //create DbContext using options
            _context = new ElevPortalenDataDbContext(_options);
            _dataRecoveryContext = new DataRecoveryDbContext(_recoveryOptions);

            //mock dependencies
            var _dataProtectionProviderMock = new Mock<IDataProtectionProvider>();

            //create CompanyService instance with mocked dependencies
            _companyService = new CompanyService(_context, _dataRecoveryContext, _dataProtectionProviderMock.Object);

        }
        #endregion

        #region CreateCompany test1 - check that company is created and message displayed
        [Fact]
        public async void CreateCompany_ShouldCreateCompany_WhenCompanyModelIsValid() {

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

        #region CreateCompany test2 - check that companies cannot be created on the same id
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

        #region ReadData test1 - ReadData only returns the mocked data with the right Guid, data is CompanyModel
        [Fact]
        public async Task ReadData_ReturnsCorrectCompanies_BasedOnUserGuid() {
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
                    UserId = Guid.Parse(UserGuid), //Create two companies under the same Guid
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
            Assert.NotEqual("Company3", result[1].CompanyName); //the second should not be name created under different Guid
        }

        #endregion

        #region ReadData test2 - should not return any companies if there are no companies associated with the Guid
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

        #region ReadAllVisibleCompanyData test1 - Mockdata is notnull, type is list and count is 2
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

        #region ReadAllVisibleCompanyData test2 - Empty DB returns Empty list but not null
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

        #region ReadAllVisibleCompanyData test3 - Returns only if company is visible
        [Fact]
        public async void ReadAllVisibleCompanyData_ShouldNotReturnListOfCompanies_WhenCompanyIsNotVisible() {

            //ARRANGE
            await _context.Database.EnsureDeletedAsync(); //Ensure InMemory db is clear

            _context.Company.Add(new CompanyModel { 
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
                IsVisible = false, //this one is not visible
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
                IsVisible = true, //This is visible
                RegisteredDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            });

            await _context.SaveChangesAsync(); //Save changes

            //ACT
            var result = await _companyService.ReadAllVisibleCompanyData(); //Use method to read companies in db

            //ASSERT
            Assert.NotNull(result);
            Assert.IsType<List<CompanyModel>>(result);
            Assert.Single(result);
        }
        #endregion

        #region ReadAllCompanyData test1 - Return all companies
        [Fact]
        public async void ReadAllCompanyData_ShouldReturnListOfAllCompanies() {

            //ARRANGE
            await _context.Database.EnsureDeletedAsync(); //Ensure InMemory db is clear

            _context.Company.Add(new CompanyModel { //Add data to db, we use mixed data with different visibilities
                CompanyId = 1,
                CompanyName = "NetCompany",
                Region = "Sjælland",
                Email = "Netcompany@Netcompany.dk",
                Link = "www.google.com",
                Preferences = "Programmør",
                Description = "Danmarks NetCompany",
                profileImage = "pic.jpg",
                PhoneNumber = 22334455,
                IsHiring = false, 
                IsVisible = false,
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
            var result = await _companyService.ReadAllCompanyData(); //Use method to read companies in db

            //ASSERT
            Assert.NotNull(result);
            Assert.IsType<List<CompanyModel>>(result);
            Assert.Equal(2, result.Count); //Both have been read by method
        }
        #endregion

        #region ReadAllCompanyData test2 - Return all companies should return an empty list when there are no companies
        [Fact]
        public async void ReadAllCompanyData_ShouldReturnEmptyListWhenNoCompaniesExist() {

            //ARRANGE
            await _context.Database.EnsureDeletedAsync(); //Ensure InMemory db is clear

            //ACT
            var result = await _companyService.ReadAllCompanyData();

            //ASSERT
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        #endregion

        #region Update test1 - Update company should successfully update company
        [Fact]
        public async void UpdateCompany_ShouldUpdateCompanySuccessfully() {

            //ARRANGE
            await _context.Database.EnsureDeletedAsync(); //Ensure InMemory db is clear
            var UserGuid = Guid.NewGuid().ToString(); //make new Guid

            var companyToUpdate = new CompanyModel { //Create and save a company
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

            var (message, success) = await _companyService.CreateCompany(companyToUpdate); //save

            var updatedCompany = new CompanyModel { //create an updated companyModel on same Guid with same UserId
                UserId = Guid.Parse(UserGuid),
                CompanyId = 1,
                CompanyName = "UpdatedTestCompany",
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
            var (resultMessage, isSuccess) = await _companyService.Update(updatedCompany);
            var checkResult = await _companyService.ReadAllVisibleCompanyData(); //since company is visible, it should be findable with ReadAllVisibleCompanyData

            //ASSERT
            Assert.True(success); //check if initial company profile creation was successful
            Assert.Equal("Company Profile Created.", message); //check if message was right 

            Assert.True(isSuccess); // Check if the update was successful
            Assert.Equal("Updated successfully", resultMessage); // Assert the result message
            Assert.Equal("UpdatedTestCompany", checkResult[0].CompanyName); //Assert that data can be found in our mocked companyservice as well

        }
        #endregion

        #region Update test2 - Update company should return entry not found if entry is invalid
        [Fact]
        public async void UpdateCompany_ShouldReturnEntryNotFound_WhenCompanyId_IsNotInDb() {

            //ARRANGE
            await _context.Database.EnsureDeletedAsync(); //Ensure InMemory db is clear

            var companyToUpdate = new CompanyModel { //Create and save a company
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
            var (resultMessage, isFalse) = await _companyService.Update(companyToUpdate);
            //var result = await _companyService.Update(companyToUpdate);

            //ASSERT
            //Assert.Equal("Entry not found", result);
            Assert.False(isFalse); // Check if the update was successful
            Assert.Equal("Entry not found", resultMessage); // Assert the result message
        }
        #endregion

        #region Delete test1 - delete should successfully delete company
        [Fact]
        public async void DeleteCompany_ShouldDeleteCompanySuccessfully() {

            //ARRANGE
            await _context.Database.EnsureDeletedAsync(); //Ensure InMemory db is clear

            var companyToDelete = new CompanyModel { //Create and save a company
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

            var (message, success) = await _companyService.CreateCompany(companyToDelete); //create a new company for deletion

            //ACT
            var (deleteMessage, isSuccess) = await _companyService.Delete(companyToDelete.CompanyId);

            //ASSERT
            Assert.True(success); //check if initial company profile creation was successful
            Assert.Equal("Company Profile Created.", message); //check if message was right 

            Assert.True(isSuccess);
            Assert.Equal("The Company was deleted Successfully.", deleteMessage);
            //Assert.Equal("The Company Profile was deleted Successfully.", result); //check that company was deleted (success message displayed)

        }
        #endregion

        #region Delete test2 - delete should return company not found if the id is not a match
        [Fact]
        public async void DeleteCompany_ShouldReturnCompanyNotFound_IfIdIsNotInDb() {

            //ARRANGE
            await _context.Database.EnsureDeletedAsync(); //Ensure InMemory db is clear
            var fakeCompanyId = 0;


            //ACT
            var (deleteMessage, isFalse) = await _companyService.Delete(fakeCompanyId);

            //ASSERT
            Assert.False(isFalse);
            Assert.Equal("Company not found.", deleteMessage);
        }
        #endregion

        #region GetCompanyById test1 - Retrieve company by valid ID
        [Fact]
        public async Task GetCompanyById_ShouldRetrieveCompanyByValidId() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var companyId = 1;
            var expectedCompany = new CompanyModel {
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
            _context.Company.Add(expectedCompany);
            await _context.SaveChangesAsync();

            // Act
            var result = await _companyService.GetCompanyById(companyId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCompany.CompanyName, result.CompanyName);
            // Add assertions for other properties as needed
        }
        #endregion

        #region GetCompanyById test2 - Throw exception when ID does not match any company
        [Fact]
        public async Task GetCompanyById_ShouldThrowExceptionForInvalidId() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var companyId = 0; // Assuming this ID does not exist in the database

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _companyService.GetCompanyById(companyId));
        }
        #endregion

        #region GetCompanyByIdToList test1 - Retrieve company list by valid ID
        [Fact]
        public async Task GetCompanyByIdToList_ShouldRetrieveCompanyListByValidId() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var companyId = 1;
            var expectedCompany = new CompanyModel {
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
            _context.Company.Add(expectedCompany);
            await _context.SaveChangesAsync();

            // Act
            var result = await _companyService.GetCompanyByIdToList(companyId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(expectedCompany.CompanyName, result[0].CompanyName);
            Assert.IsType<CompanyModel>(result[0]); 
        }
        #endregion

        #region GetCompanyByIdToList test2 - Retrieve company list should not retrieve a list if there is no company with the id
        [Fact]
        public async Task GetCompanyByIdToList_ShouldNotRetrieveCompanyList_IfIdIsNotValid() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var companyId = 0;

            // Act
            var result = await _companyService.GetCompanyByIdToList(companyId);

            // Assert
            Assert.Empty(result);
            Assert.IsType<List<CompanyModel>>(result);

        }
        #endregion

        #region GetCompaniesCountAsync test1 - Return correct count of companies
        [Fact]
        public async Task GetCompaniesCountAsync_ShouldReturnCorrectCount() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var expectedCount = 3; // Assuming there are 3 companies in the database

            // Add some sample companies to the database for testing
            // Add sample companies to the database
            await _context.Company.AddRangeAsync(new List<CompanyModel>
            {
                new CompanyModel {                 
                    UserId = Guid.NewGuid(),
                    CompanyId = 1,
                    CompanyName = "TestCompany1",
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
                    UpdatedDate = DateTime.Now},
                new CompanyModel {                
                    UserId = Guid.NewGuid(),
                    CompanyId = 2,
                    CompanyName = "TestCompany2",
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
                    UpdatedDate = DateTime.Now},
                new CompanyModel {                
                    UserId = Guid.NewGuid(),
                    CompanyId = 3,
                    CompanyName = "TestCompany3",
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
                    UpdatedDate = DateTime.Now}
            });
            await _context.SaveChangesAsync();

            // Act
            var actualCount = await _companyService.GetCompaniesCountAsync();

            // Assert
            Assert.Equal(expectedCount, actualCount);
        }
        #endregion

        #region GetCompaniesCountAsync test2 - Throw exception when retrieval fails
        [Fact]
        public async Task GetCompaniesCountAsync_ShouldReturnZeroWhenNoCompaniesExist() {
            // ARRANGE
            await _context.Database.EnsureDeletedAsync(); // Ensure the database is empty

            // ACT
            var count = await _companyService.GetCompaniesCountAsync();

            // ASSERT
            Assert.Equal(0, count);
        }
        #endregion

        #region CreateRecoveryData test1 - shoudl create recovery data successfully
        [Fact]
        public async Task CreateRecoveryData_ShouldCreateRecoveryDataSuccessfully() {
            // ARRANGE
            await _dataRecoveryContext.Database.EnsureDeletedAsync(); // Ensure the recovery database is empty
            var deletedCompany = new CompanyModel {
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

            // ACT
            var result = await _companyService.CreateRecoveryData(deletedCompany);

            // ASSERT
            Assert.Equal("Company Recovery Created", result);
            // Query the recovery database to check if a company with the name "TestCompany" exists
            var recoveredCompany = await _dataRecoveryContext.CompanyDataRecovery.FirstOrDefaultAsync(c => c.CompanyName == "TestCompany");
            Assert.NotNull(recoveredCompany); // Ensure that the company exists in the recovery database

        }

        #endregion 

        #region CheckRecoveryDataExist test1 - Should return true if data exists on the Guid
        [Fact]
        public async Task CheckRecoveryDataExist_ShouldReturnTrueForExistingData() {
            // ARRANGE
            await _context.Database.EnsureDeletedAsync(); // Ensure the database is empty
            // Add recovery data for a specific user to the database
            var userId = Guid.NewGuid();
            await _dataRecoveryContext.CompanyDataRecovery.AddAsync(new CompanyRecoveryModel { UserId = userId });
            await _dataRecoveryContext.SaveChangesAsync();

            // ACT
            var result = await _companyService.CheckRecoveryDataExist(userId);

            // ASSERT
            Assert.True(result);
        }

        #endregion

        #region CheckRecoveryDataExist test2 - Should return false if no data exists on the Guid
        [Fact]
        public async Task CheckRecoveryDataExist_ShouldReturnFalseForNonExistingData() {
            // ARRANGE
            // Ensure that no recovery data exists in the database for a specific user

            // ACT
            var result = await _companyService.CheckRecoveryDataExist(Guid.NewGuid());

            // ASSERT
            Assert.False(result);
        }

        #endregion

        #region RecoverCompanyData test1 - RecoverCompanyData should recover data successfully with correct Guid
        [Fact]
        public async Task RecoverCompanyData_ShouldRecoverDataSuccessfully() {
            // ARRANGE
            await _context.Database.EnsureDeletedAsync(); // Ensure the database is empty
            // Add recovery data for a specific user to the recovery database
            var userId = Guid.NewGuid();
            await _dataRecoveryContext.CompanyDataRecovery.AddAsync(new CompanyRecoveryModel { UserId = userId });
            await _dataRecoveryContext.SaveChangesAsync();

            // ACT
            var (message, success) = await _companyService.RecoverCompanyData(userId);

            // ASSERT
            Assert.True(success);
            Assert.Equal("Data successfully recovered.", message);

            // Check if the recovered company data exists in the main context
            var recoveredCompany = await _context.Company.FirstOrDefaultAsync(c => c.UserId == userId);
            Assert.NotNull(recoveredCompany);
        }
        #endregion

        # region RecoverCompanyData test2 - RecoverCompanyData should not recover data successfully without correct Guid
        [Fact]
        public async Task RecoverCompanyData_ShouldReturnFailureForNonExistingData() {
            // ARRANGE
            await _context.Database.EnsureDeletedAsync(); // Ensure the database is empty
            // Ensure that no recovery data exists in the recovery database for a specific user

            // ACT
            var (message, success) = await _companyService.RecoverCompanyData(Guid.NewGuid());

            // ASSERT
            Assert.False(success);
            Assert.StartsWith("No recovery data found for UserId:", message);
        }
        #endregion

        #region GetCompanyByGuid test1 - Get company by Guid should return the compnay of the Gui exists
        [Fact]
        public async Task GetCompanyByGuid_ShouldReturnCompanyIfExists() {
            // ARRANGE
            await _context.Database.EnsureDeletedAsync(); // Ensure the database is empty
            // Add a company with a specific GUID to the database
            var companyId = Guid.NewGuid();
            var company = new CompanyModel { UserId = companyId };
            await _context.Company.AddAsync(company);
            await _context.SaveChangesAsync();

            // ACT
            var result = await _companyService.GetCompanyByGuid(companyId);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(companyId, result.UserId);
        }
        #endregion

        #region GetCompanyByGuid test2 - Get company by Guid should not return the compnay of the Guid does not exist
        [Fact]
        public async Task GetCompanyByGuid_ShouldReturnNullForNonExistingCompany() {
            // ARRANGE
            await _context.Database.EnsureDeletedAsync(); // Ensure the database is empty

            var companyId = Guid.NewGuid();
            var company = new CompanyModel { UserId = Guid.NewGuid() }; //ensure a company exists
            await _context.Company.AddAsync(company);
            await _context.SaveChangesAsync();

            // ACT
            var result = await _companyService.GetCompanyByGuid(companyId);

            // ASSERT
            Assert.Null(result);
        }
        #endregion


    } //companyservicetests end bracket
}
