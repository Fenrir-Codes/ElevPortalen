using ElevPortalen.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ElevPortalen.Data;

namespace ElevPortalen.Services
{

    // Lavet af Jozsef
    public class StudentService
    {
        private readonly ElevPortalenDataDbContext _context;
        private readonly IDataProtector? _dataProtector;

        #region constructor
        public StudentService(ElevPortalenDataDbContext context, IDataProtectionProvider dataProtectionProvider)
        {
            _context = context;
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
                // Handle the exception and return an error message
                return $"An error har ocurred: {ex.Message}";
            }
        }
        #endregion

        #region Get one Student request
        public async Task<List<StudentModel>> ReadData(ClaimsPrincipal _user)
        {
            try
            {
                //Get all the data
                var response = await _context.Student.AsNoTracking().Where(user => user.UserId
                    == Guid.Parse(_user.FindFirstValue(ClaimTypes.NameIdentifier))).ToListAsync();

                if (response != null)
                {
                    return response; // return the data
                }
                else
                {
                    throw new ApplicationException("An error occurred while retrieving student data.");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving student data." + ex.Message);
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
                throw new ApplicationException("An error occurred while retrieving Student data." + ex.Message);
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
                throw new ApplicationException("An error occurred while retrieving Student data." + ex.Message);
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
                return $"Error: {ex.Message}"; // Return an error message if an exception occurs
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
                return $"An error has occurred: {ex.Message}";
            }
        }
        #endregion

        #region Get Student by Id
        public async Task<StudentModel> GetStudentById(int Id)
        {
            try
            {
                var student = await _context.Student
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.StudentId == Id);

                if (student != null)
                {
                    return student;
                }
                else
                {
                    throw new ApplicationException($"An error occurred while finding user's Id. Or no Id in database.");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving student data: {ex.Message}");
            }
        }
        #endregion

    }
}
