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

        #region Getting the skill Name as string to List
        public async Task<List<string>> GetSkills(StudentModel data)
        {
            if (data == null || data.Skills == null)
            {
                return null!;
            }

            List<string> skills = await Task.Run(() =>
                typeof(SkillModel).GetProperties()
                    .Where(skill => skill.PropertyType == typeof(bool) && (bool)skill.GetValue(data.Skills))
                    .Select(skill => skill.Name)
                    .ToList());

            return skills;
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
                    // Student already has skills, update the existing skills
                    existingSkills.CSharp = newSkills.CSharp;
                    existingSkills.Java = newSkills.Java;
                    existingSkills.DotNet = newSkills.DotNet;
                    existingSkills.Typescript = newSkills.Typescript;
                    existingSkills.Python = newSkills.Python;
                    existingSkills.PHP = newSkills.PHP;
                    existingSkills.CPlusPlus = newSkills.CPlusPlus;
                    existingSkills.C = newSkills.C;
                    existingSkills.Bootstrap = newSkills.Bootstrap;
                    existingSkills.Blazor = newSkills.Blazor;
                    existingSkills.JavaScript = newSkills.JavaScript;
                    existingSkills.HTML = newSkills.HTML;
                    existingSkills.CSS = newSkills.CSS;
                    existingSkills.SQL = newSkills.SQL;
                    existingSkills.OfficePack = newSkills.OfficePack;
                    existingSkills.CloudComputing = newSkills.CloudComputing;
                    existingSkills.VersionControl = newSkills.VersionControl;
                    existingSkills.NetWork = newSkills.NetWork;
                    existingSkills.ProblemSolving = newSkills.ProblemSolving;
                    existingSkills.Communikation = newSkills.Communikation;
                    existingSkills.TeamWorking = newSkills.TeamWorking;
                    existingSkills.WillingToLearn = newSkills.WillingToLearn;

                    await _context.SaveChangesAsync();

                    return null;
                }

                // Create new skills entry
                newSkills.StudentId = studentId;
                _context.StudentSkills.Add(newSkills);
                await _context.SaveChangesAsync();

                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create/update skills: {ex.Message}");
            }
        }


        #endregion

        #region Retrieve the skills with StudentId
        public async Task<SkillModel> GetSkillsByStudentId(int studentId)
        {
            try
            {
                var skills = await _context.StudentSkills
                    .Where(s => s.StudentId == studentId).FirstAsync();

                return skills;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while retrieving skills data: {ex.Message}");
            }
        }
        #endregion

        #region Update
        public async Task<string?> UpdateSkills(int studentId, SkillModel updatedSkills)
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
                    entry.HTML = updatedSkills.HTML;
                    entry.CSS = updatedSkills.CSS;
                    entry.SQL = updatedSkills.SQL;
                    entry.OfficePack = updatedSkills.OfficePack;
                    entry.CloudComputing = updatedSkills.CloudComputing;
                    entry.VersionControl = updatedSkills.VersionControl;
                    entry.NetWork = updatedSkills.NetWork;
                    entry.ProblemSolving = updatedSkills.ProblemSolving;
                    entry.Communikation = updatedSkills.Communikation;
                    entry.TeamWorking = updatedSkills.TeamWorking;
                    entry.WillingToLearn = updatedSkills.WillingToLearn;


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
