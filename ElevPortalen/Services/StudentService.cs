using ElevPortalen.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ElevPortalen.Data;
using System.Data;

namespace ElevPortalen.Services
{

    // Lavet af Jozsef
    public class StudentService
    {
        private readonly ElevPortalenDataDbContext _context;
        private readonly DataRecoveryDbContext _recoveryContext;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IDataProtector? _dataProtector;

        #region constructor
        public StudentService(ElevPortalenDataDbContext context, DataRecoveryDbContext recoveryContext, IDataProtectionProvider dataProtectionProvider, ApplicationDbContext applicationDbContext)
        {
            _context = context;
            _applicationDbContext = applicationDbContext;
            _recoveryContext = recoveryContext;
            _dataProtector = dataProtectionProvider.CreateProtector("ProtectData");
            //i just placed it here if need, we can use it to protect data
        }
        #endregion

        #region create Student function async
        public async Task<string> CreateStudent(StudentModel student)
        {
            try
            {
                _context.Student.Add(student); // Add input to context variables
                await _context.SaveChangesAsync(); // Save data

                return $"User Profile Created";
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error har ocurred: {ex.Message}");
            }
        }
        #endregion

        #region Get Student request (Read Data)
        public async Task<List<StudentModel>> ReadData(ClaimsPrincipal _user)
        {
            try
            {
                //Get all the data
                //var response = await _context.Student.AsNoTracking().Where(user => user.UserId
                //    == Guid.Parse(_user.FindFirstValue(ClaimTypes.NameIdentifier))).ToListAsync();

                //Including the skills
                var response = await _context.Student.Include(s => s.Skills).AsNoTracking()
                    .Where(user => user.UserId == Guid.Parse(_user.FindFirstValue(ClaimTypes.NameIdentifier))).ToListAsync();

                if (response != null)
                {
                    return response; // return the data
                }
                else
                {
                    throw new InvalidOperationException("An error occurred while retrieving student data.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while retrieving student data." + ex.Message);
            }
        }
        #endregion

        #region Get All Student data
        public async Task<List<StudentModel>> ReadAllStudentData()
        {
            try
            {
                //Get all the data
                var response = await _context.Student.AsNoTracking().ToListAsync();

                return response; // return the data
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while retrieving Student data." + ex.Message);
            }
        }
        #endregion

        #region Student count
        public async Task<int> GetStudentCountAsync()
        {
            try
            {
                // Get all the data
                int response = await _context.Student.CountAsync();

                return response; // return the data
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while retrieving Student data." + ex.Message);
            }
        }
        #endregion

        #region Student Update function
        public async Task<string> Update(StudentModel student)
        {
            try
            {
                var entry = await _context.Student.FindAsync(student.StudentId);

                // If the response is not null
                if (entry != null)
                {
                    entry.Title = student.Title;
                    entry.FirstName = student.FirstName;
                    entry.MiddleName = student.MiddleName;
                    entry.LastName = student.LastName;
                    entry.Address = student.Address;
                    entry.Description = student.Description;
                    entry.Speciality = student.Speciality;
                    entry.PhoneNumber = student.PhoneNumber;

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
                throw new InvalidOperationException(ex.Message); // Return an error message if an exception occurs
            }
        }
        #endregion

        #region Delete Student function
        public async Task<string> Delete(int studentId)
        {
            try
            {
                var student = await _context.Student.FindAsync(studentId);
                if (student != null)
                {
                    await CreateRecoveryData(student);// First create a recovery data

                    var entryToRemove = _context.Student.Local.FirstOrDefault(s => s.StudentId == student.StudentId);
                    if (entryToRemove != null)
                    {
                        _context.Entry(entryToRemove).State = EntityState.Detached;
                    }

                    _context.Student.Remove(student);
                    await _context.SaveChangesAsync();

                    return "The User Profile deleted Successfully.";
                }
                else
                {
                    return "Student not found.";
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error has occurred: {ex.Message}");
            }
        }
        #endregion

        #region Get Student by Id (Model)
        public async Task<StudentModel> GetStudentById(int Id)
        {
            try
            {
                var student = await _context.Student
                    .FirstOrDefaultAsync(s => s.StudentId == Id);

                if (student != null)
                {
                    return student;
                }
                else
                {
                    throw new InvalidOperationException($"An error occurred while finding user's Id. Or no Id in database.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while retrieving student data: {ex.Message}");
            }
        }
        #endregion

        #region Get Student by Id to list
        public async Task<List<StudentModel>> GetStudentByIdToList(int Id)
        {
            try
            {
                var student = await _context.Student
                    .Where(s => s.StudentId == Id).ToListAsync();

                if (student != null)
                {
                    return student;
                }
                else
                {
                    // Throw an exception if no student found
                    throw new InvalidOperationException($"No student found with Id: {Id}");
                }
            }
            catch (Exception ex)
            {
                // Instead of throwing a new exception with a message that includes the original exception message,
                // it's better to let the original exception propagate.
                throw;
            }
        }
        #endregion

        #region Get a student by its Guid
        public async Task<StudentModel> GetStudentByGuid(Guid id)
        {
            try
            {
                var student = await _context.Student.FirstOrDefaultAsync(s => s.UserId == id);

                if (student != null)
                {
                    return student;
                }
                else
                {
                    throw new InvalidOperationException($"An error occurred while finding user's Guid. Or no Guid in database.");
                }
            }
            catch (Exception ex)
            {

                throw new InvalidOperationException($"An error occurred while retrieving student data: {ex.Message}");

            }
        }
        #endregion

        #region create Recovery data function
        public async Task<string> CreateRecoveryData(StudentModel deletedStudent)
        {
            try
            {
                var recoveryData = new StudentRecoveryModel
                {
                    UserId = deletedStudent.UserId,
                    StudentId = deletedStudent.StudentId,
                    Title = deletedStudent.Title,
                    FirstName = deletedStudent.FirstName,
                    MiddleName = deletedStudent.MiddleName,
                    LastName = deletedStudent.LastName,
                    Email = deletedStudent.Email,
                    Address = deletedStudent.Address,
                    Description = deletedStudent.Description,
                    profileImage = deletedStudent.profileImage,
                    Speciality = deletedStudent.Speciality,
                    PhoneNumber = deletedStudent.PhoneNumber,
                    RegisteredDate = deletedStudent.RegisteredDate,
                    RecoveryCreationDate = DateTime.Now
                };

                _recoveryContext.StudentDataRecovery.Add(recoveryData); // Add input to context variables
                await _recoveryContext.SaveChangesAsync(); // Save data

                return $"Recovery Created";
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error message
                throw new InvalidOperationException($"An error has occurred: {ex.Message}");
            }
        }
        #endregion

        #region Function to check if student data exist in the recovery database
        public async Task<bool> CheckRecoveryDataExist(Guid id)
        {
            var data = await _recoveryContext.StudentDataRecovery.Where(s => s.UserId == id).FirstOrDefaultAsync();
            if (data != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Recover the data function for student
        public async Task<string> RecoverStudentData(Guid id)
        {
            try
            {
                // Retrieve recovery data based on the provided Guid
                var recoveryData = await _recoveryContext.StudentDataRecovery
                    .FirstOrDefaultAsync(s => s.UserId == id);

                if (recoveryData != null)
                {
                    // Assuming StudentModel has a constructor that takes StudentRecoveryModel as a parameter
                    var recoveredStudent = new StudentModel
                    {
                        //The StudentId is autoincrement therefor we do not use it otherwise we crash
                        UserId = recoveryData.UserId,
                        Title = recoveryData.Title,
                        FirstName = recoveryData.FirstName,
                        MiddleName = recoveryData.MiddleName,
                        LastName = recoveryData.LastName,
                        Email = recoveryData.Email,
                        Address = recoveryData.Address,
                        Description = recoveryData.Description,
                        profileImage = recoveryData.profileImage,
                        Speciality = recoveryData.Speciality,
                        PhoneNumber = recoveryData.PhoneNumber,
                        RegisteredDate = recoveryData.RegisteredDate,
                        UpdatedDate = DateTime.Now,
                    };
                    // Add the recovered student to the main context
                    _context.Student.Add(recoveredStudent);

                    // Remove the recovery data from the recovery context
                    _recoveryContext.StudentDataRecovery.Remove(recoveryData);

                    // Save changes to both contexts
                    await _context.SaveChangesAsync();
                    await _recoveryContext.SaveChangesAsync();

                    return "Data successfully recovered.";
                }
                else
                {
                    // Return a message indicating that recovery data does not exist
                    return $"No recovery data found for UserId: {id}.";
                }
            }
            catch (Exception ex)
            {
                // Return an error message if an exception occurs
                throw new InvalidOperationException($"Error recovering data: {ex.Message}");
            }
        }
        #endregion

    }
}
