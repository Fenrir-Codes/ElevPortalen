using ElevPortalen.Data;
using ElevPortalen.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace ElevPortalen.Services
{
    public class SkillService
    {
        private readonly ElevPortalenDataDbContext _context;
        private readonly IDataProtector? _dataProtector;

        #region constructor
        public SkillService(ElevPortalenDataDbContext context, IDataProtectionProvider dataProtectionProvider)
        {
            _context = context;
            _dataProtector = dataProtectionProvider.CreateProtector("ProtectData");
            //i just placed it here if need, we can use it to protect data
        }
        #endregion

        #region Create Skills
        public async Task<string?> CreateSkills(int studentId, SkillModel newSkills)
        {
            try
            {
                // Check if the student already has skills
                var existingSkills = await _context.StudentSkills.FirstOrDefaultAsync(s => s.StudentId == studentId);

                if (existingSkills != null)
                {
                    // Student already has skills, consider updating instead of creating
                    return "Skills already exist for the student. Consider updating instead of creating.";
                }

                // Create new skills entry
                var newEntry = new SkillModel
                {
                    StudentId = studentId,
                    CSharp = newSkills.CSharp,
                    Java = newSkills.Java,
                    DotNet = newSkills.DotNet,
                    Typescript = newSkills.Typescript,
                    Python = newSkills.Python,
                    PHP = newSkills.PHP,
                    CPlusPlus = newSkills.CPlusPlus,
                    C = newSkills.C,
                    Bootstrap = newSkills.Bootstrap,
                    Blazor = newSkills.Blazor,
                    JavaScript = newSkills.JavaScript,
                    HTML = newSkills.HTML,
                    CSS = newSkills.CSS,
                    SQL = newSkills.SQL,
                    MongoDB = newSkills.MongoDB,
                    OfficePack = newSkills.OfficePack,
                    CloudComputing = newSkills.CloudComputing,
                    VersionControl = newSkills.VersionControl,
                    OOP = newSkills.OOP

                };

                _context.StudentSkills.Add(newEntry);
                await _context.SaveChangesAsync();

                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create skills: {ex.Message}");
            }
        }

        #endregion

        #region Retrieve the skills with StudentId
        public async Task<SkillModel> GetSkillsByStudentId(int studentId)
        {
            try
            {
                var student = await _context.StudentSkills
                    .FirstOrDefaultAsync(s => s.StudentId == studentId);

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
                throw new InvalidOperationException($"An error occurred while retrieving skills data: {ex.Message}");
            }
        }
        #endregion

        #region Update
        public async Task<string?> UpdateSkills(int studentId,SkillModel updatedSkills)
        {
            try
            {
                var entry = await _context.StudentSkills.FindAsync(studentId);

                // If the entry is not null
                if (entry != null)
                {
                    // Update individual skills
                    entry.CSharp = updatedSkills.CSharp;
                    entry.Java = updatedSkills.Java;
                    entry.DotNet = updatedSkills.DotNet;
                    entry.Typescript = updatedSkills.Typescript;
                    entry.Python = updatedSkills.Python;
                    entry.PHP = updatedSkills.PHP;
                    entry.CPlusPlus = updatedSkills.CPlusPlus;
                    entry.C = updatedSkills.C;
                    entry.Bootstrap = updatedSkills.Bootstrap;
                    entry.Blazor = updatedSkills.Blazor;
                    entry.JavaScript = updatedSkills.JavaScript;
                    entry.Java = updatedSkills.Java;
                    entry.HTML = updatedSkills.HTML;
                    entry.CSS = updatedSkills.CSS;
                    entry.SQL = updatedSkills.SQL;
                    entry.MongoDB  = updatedSkills.MongoDB;
                    entry.OfficePack = updatedSkills.OfficePack;
                    entry.CloudComputing = updatedSkills.CloudComputing;
                    entry.VersionControl = updatedSkills.VersionControl;
                    entry.OOP = updatedSkills.OOP;

                    _context.Entry(entry).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    return "Skills updated successfully";
                }
                else
                {
                    return "Student not found"; // Return a message when the student is not found
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to update skills: {ex.Message}");
            }
        }
        #endregion

        #region Delete skills
        public async Task<string?> DeleteSkills(int studentId)
        {
            try
            {
                // Find the entry for the specified student
                var entry = await _context.StudentSkills.FirstOrDefaultAsync(s => s.StudentId == studentId);

                // If the entry is not null, delete it
                if (entry != null)
                {
                    _context.StudentSkills.Remove(entry);
                    await _context.SaveChangesAsync();

                    return null; // Return null when there is no error
                }
                else
                {
                    return "Skills not found for the specified student"; // Return a message when the skills are not found
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to delete skills: {ex.Message}");
            }
        }

        #endregion
    }
}
