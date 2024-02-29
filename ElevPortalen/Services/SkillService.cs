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


        public SkillModel GetSkillsByStudentId(int studentId)
        {
            return _context.StudentSkills.FirstOrDefault(s => s.StudentId == studentId);
        }
    }
}
