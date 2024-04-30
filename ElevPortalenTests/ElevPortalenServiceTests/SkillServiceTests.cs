using ElevPortalen.Data;
using ElevPortalen.Models;
using ElevPortalen.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevPortalenTests.ElevPortalenServiceTests {
    public class SkillServiceTests {

        private readonly DbContextOptions<ElevPortalenDataDbContext> _options;
        private readonly ElevPortalenDataDbContext _context;
        private readonly SkillService _skillService;

        public SkillServiceTests() {

            //create InMemory DBs:
            _options = new DbContextOptionsBuilder<ElevPortalenDataDbContext>()
                .UseInMemoryDatabase(databaseName: "SkillServiceTests")
                .Options;

            //create DbContext using options
            _context = new ElevPortalenDataDbContext(_options);

            //mock dependencies
            var _dataProtectionProviderMock = new Mock<IDataProtectionProvider>();

            //create skillService instance with mocked dependencies
            _skillService = new SkillService(_context, _dataProtectionProviderMock.Object);
        }

        #region GetSkills test1 - return list of skills when model is valid
        [Fact]
        public async Task GetSkills_ShouldReturnListOfSkills_WhenValidStudentModelIsProvided() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var student = new StudentModel {
                Skills = new SkillModel {
                    CSharp = true,
                    Java = true,
                    HTML = true,
                    ProblemSolving = true
                }
            };

            var expectedSkills = new List<string> { "CSharp", "Java", "HTML", "ProblemSolving" };

            // Act
            var result = await _skillService.GetSkills(student);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<string>>(result);
            Assert.Equal(expectedSkills.Count, result.Count);
            Assert.True(expectedSkills.SequenceEqual(result));
        }
        #endregion

        #region GetSkills test2 - return empty list of skills when model is valid, skills are present, but all skills are false
        [Fact]
        public async Task GetSkills_ShouldReturnEmptyList_WhenStudentHasNoSkillsSetToTrue() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var student = new StudentModel {
                Skills = new SkillModel {
                    CSharp = false,
                    Java = false
                }
            };

            // Act
            var result = await _skillService.GetSkills(student);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            Assert.IsType<List<string>>(result);
        }
        #endregion

        #region GetSkills test3 - return null when studentmodel is null
        [Fact]
        public async Task GetSkills_ShouldReturnNull_WhenStudentModelIsNull() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            StudentModel student = null;

            // Act
            var result = await _skillService.GetSkills(student);

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region GetSkills test4 - reutrn null when studentmodel is valid but skills are null
        [Fact]
        public async Task GetSkills_ShouldReturnNull_WhenStudentSkillsIsNull() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var student = new StudentModel {
                Skills = null
            };

            // Act
            var result = await _skillService.GetSkills(student);

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region GetSkillsById test1 - Getting the skill Name as string to List - return list of skills belonging to student based on ID
        [Fact]
        public async Task GetSkillsById_ShouldReturnListOfSkills_WhenValidStudentIdIsProvided() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var student = new StudentModel { //create a new StudentModel with a set of skills
                UserId = Guid.NewGuid(), 
                StudentId = 1,
                FirstName = "Johnathan",
                LastName = "Doe",
                Email = "johnathanDoe@mail.com",
                Address = "GadeVej1",
                Description = "Description",
                profileImage = "image.jpg",
                Speciality = "Programmør",
                PhoneNumber = 12345678,
                RegisteredDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                Skills = new SkillModel {
                    StudentId = 1,
                    CSharp = true,
                    Java = true
                }
            };

            // Add test data to the database
            _context.Student.Add(student);
            await _context.SaveChangesAsync();

            var expectedSkills = new List<string> { "CSharp", "Java" };

            // Act
            var result = await _skillService.GetSkillsById(student.StudentId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<string>>(result);
            Assert.Equal(expectedSkills.Count, result.Count);
            Assert.True(expectedSkills.SequenceEqual(result));
        }
        #endregion

        #region GetSkillsById test2 - Getting the skill Name as string to List test2 - return list of skills belonging to student based on ID
        [Fact]
        public async Task GetSkillsById_ShouldReturnEmptyListOfSkills_WhenValidStudentIdIsProvided_ButStudentHasNoSkills() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var student = new StudentModel { //create a new StudentModel with a set of skills
                UserId = Guid.NewGuid(),
                StudentId = 1,
                FirstName = "Johnathan",
                LastName = "Doe",
                Email = "johnathanDoe@mail.com",
                Address = "GadeVej1",
                Description = "Description",
                profileImage = "image.jpg",
                Speciality = "Programmør",
                PhoneNumber = 12345678,
                RegisteredDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                Skills = new SkillModel {
                    StudentId = 1,
                    CSharp = false,
                    Java = false
                }
            };

            // Add test data to the database
            _context.Student.Add(student);
            await _context.SaveChangesAsync();

            // Act
            var result = await _skillService.GetSkillsById(student.StudentId);

            // Assert
            Assert.Empty(result);
            Assert.IsType<List<string>>(result);
        }
        #endregion

        #region GetSkillsById test3 - Getting the skill Name as string to List test3 - return list of skills belonging to student based on ID
        [Fact]
        public async Task GetSkillsById_ShouldReturnNull_WhenValidStudentIdIsProvided_ButStudentHasNoSkillModel() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var student = new StudentModel { //create a new StudentModel with a set of skills
                UserId = Guid.NewGuid(),
                StudentId = 1,
                FirstName = "Johnathan",
                LastName = "Doe",
                Email = "johnathanDoe@mail.com",
                Address = "GadeVej1",
                Description = "Description",
                profileImage = "image.jpg",
                Speciality = "Programmør",
                PhoneNumber = 12345678,
                RegisteredDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
            };

            // Add test data to the database
            _context.Student.Add(student);
            await _context.SaveChangesAsync();

            // Act
            var result = await _skillService.GetSkillsById(student.StudentId);

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region Create Skills test1 - CreateSkills should add new skills for a student without existing skills
        [Fact]
        public async Task CreateSkills_ShouldAddNewSkills_WhenStudentHasNoExistingSkills() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear

            var student = new StudentModel { //create a new StudentModel without skills
                UserId = Guid.NewGuid(),
                StudentId = 1,
                FirstName = "Johnathan",
                LastName = "Doe",
                Email = "johnathanDoe@mail.com",
                Address = "GadeVej1",
                Description = "Description",
                profileImage = "image.jpg",
                Speciality = "Programmør",
                PhoneNumber = 12345678,
                RegisteredDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
            };

            // Add test data to the database
            _context.Student.Add(student);
            await _context.SaveChangesAsync();

            var newSkills = new SkillModel {
                StudentId = 1,
                CSharp = true,
                Java = true,
                DotNet = true
            };

            // Act
            var (message, success) = await _skillService.CreateSkills(student.StudentId, newSkills);

            // Assert
            Assert.True(success);
            Assert.Equal("Skills were added successfully.", message);

            // Verify that the new skills were added to the database
            var createdSkills = await _context.StudentSkills.FirstOrDefaultAsync(s => s.StudentId == student.StudentId);
            Assert.NotNull(createdSkills);
        }
        #endregion

        #region Create Skills test2 - CreateSkills should overwrite existing skills for a student
        [Fact]
        public async Task CreateSkills_ShouldProbablyNotUpdateExistingSkills_WhenStudentHasSkills() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var studentId = 1;
            var existingSkills = new SkillModel {
                StudentId = studentId,
                CSharp = true,
                Java = false,
                DotNet = true
                // Add other existing skills as needed
            };
            // Add existing skills to the database
            _context.StudentSkills.Add(existingSkills);
            await _context.SaveChangesAsync();

            var newSkills = new SkillModel {
                StudentId = studentId,
                CSharp = false,
                Java = true,
                DotNet = true
                // Add new skills as needed
            };

            // Act
            var (message, success) = await _skillService.CreateSkills(studentId, newSkills);

            // Assert
            Assert.True(success);
            Assert.Equal("Skills were added successfully.", message);

            // Verify that the existing skills were updated in the database
            var updatedSkills = await _context.StudentSkills.FirstOrDefaultAsync(s => s.StudentId == studentId);
            Assert.NotNull(updatedSkills);
            Assert.False(updatedSkills.CSharp);
        }
        #endregion

        #region GetSkillsByStudentId test1 - Should return skills for a valid student ID
        [Fact]
        public async Task GetSkillsByStudentId_ShouldReturnSkills_ForValidStudentId() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var studentId = 1;
            var expectedSkills = new SkillModel {
                StudentId = studentId,
                CSharp = true,
                Java = true,
                DotNet = false
                // Add other expected skills as needed
            };
            // Add expected skills to the database
            _context.StudentSkills.Add(expectedSkills);
            await _context.SaveChangesAsync();

            // Act
            var result = await _skillService.GetSkillsByStudentId(studentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedSkills.StudentId, result.StudentId);
            Assert.Equal(expectedSkills.CSharp, result.CSharp);
            Assert.Equal(expectedSkills.Java, result.Java);
            Assert.Equal(expectedSkills.DotNet, result.DotNet);
            // Add assertions to check other skills if needed
        }
        #endregion

        #region GetSkillsByStudentId test2 - Should throw exception for invalid student ID
        [Fact]
        public async Task GetSkillsByStudentId_ShouldThrowException_ForInvalidStudentId() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var invalidStudentId = 0; 

            // Act and Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _skillService.GetSkillsByStudentId(invalidStudentId);
            });
        }
        #endregion

        #region UpdateSkills test1 - Should update skills for a valid student ID
        [Fact]
        public async Task UpdateSkills_ShouldUpdateSkills_ForValidStudentId() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var studentId = 1;
            var existingSkills = new SkillModel {
                StudentId = studentId,
                CSharp = true,
                Java = true,
                DotNet = false
                // Add other existing skills as needed
            };
            // Add existing skills to the database
            _context.StudentSkills.Add(existingSkills);
            await _context.SaveChangesAsync();

            var updatedSkills = new SkillModel {
                StudentId = studentId,
                CSharp = false,
                Java = false,
                DotNet = true
                // Add other updated skills as needed
            };

            // Act
            var (message, success) = await _skillService.UpdateSkills(studentId, updatedSkills);

            // Assert
            Assert.True(success);
            Assert.Equal("Skills updated successfully", message);

            // Retrieve updated skills from the database
            var updatedSkillsFromDb = await _context.StudentSkills.FirstOrDefaultAsync(s => s.StudentId == studentId);
            Assert.NotNull(updatedSkillsFromDb);
            Assert.Equal(updatedSkills.CSharp, updatedSkillsFromDb.CSharp);
            Assert.Equal(updatedSkills.Java, updatedSkillsFromDb.Java);
            Assert.Equal(updatedSkills.DotNet, updatedSkillsFromDb.DotNet);
            // Add assertions to check other skills if needed
        }
        #endregion

        #region UpdateSkills test2 - Should return false for invalid student ID
        [Fact]
        public async Task UpdateSkills_ShouldReturnFalse_ForInvalidStudentId() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var invalidStudentId = 0; 
            var updatedSkills = new SkillModel {
                // Provide updated skills
            };

            // Act
            var (message, success) = await _skillService.UpdateSkills(invalidStudentId, updatedSkills);

            // Assert
            Assert.False(success);
            Assert.Equal("An error has ocurred", message);
        }
        #endregion

        #region DeleteSkills test1 - Should delete skills for a valid student ID
        [Fact]
        public async Task DeleteSkills_ShouldDeleteSkills_ForValidStudentId() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var studentId = 1;
            var existingSkills = new SkillModel {
                StudentId = studentId,
                CSharp = true,
                Java = true,
                DotNet = false
                // Add other existing skills as needed
            };
            // Add existing skills to the database
            _context.StudentSkills.Add(existingSkills);
            await _context.SaveChangesAsync();

            // Act
            var resultMessage = await _skillService.DeleteSkills(studentId);

            // Assert
            Assert.Equal("Skills deleted successfully.", resultMessage);

            // Check if the skills are deleted from the database
            var deletedSkillsFromDb = await _context.StudentSkills.FirstOrDefaultAsync(s => s.StudentId == studentId);
            Assert.Null(deletedSkillsFromDb);
        }
        #endregion

        #region DeleteSkills test2 - Should return message for invalid student ID
        [Fact]
        public async Task DeleteSkills_ShouldReturnMessage_ForInvalidStudentId() {
            // Arrange
            await _context.Database.EnsureDeletedAsync(); // Ensure InMemory db is clear
            var invalidStudentId = 0; 

            // Act
            var resultMessage = await _skillService.DeleteSkills(invalidStudentId);

            // Assert
            Assert.Equal("Skills not found for the specified student", resultMessage);
        }
        #endregion

        #region
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


    }//skillServiceTests end bracket
}
