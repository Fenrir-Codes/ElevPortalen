using ElevPortalen.Data;
using Microsoft.AspNetCore.DataProtection;

namespace ElevPortalen.Services
{
    public class SkillService
    {
        private readonly ElevPortalenDataDbContext _context;
        private readonly DataRecoveryDbContext _recoveryContext;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IDataProtector? _dataProtector;

        #region constructor
        public SkillService(ElevPortalenDataDbContext context, DataRecoveryDbContext recoveryContext, IDataProtectionProvider dataProtectionProvider, ApplicationDbContext applicationDbContext)
        {
            _context = context;
            _applicationDbContext = applicationDbContext;
            _recoveryContext = recoveryContext;
            _dataProtector = dataProtectionProvider.CreateProtector("ProtectData");
            //i just placed it here if need, we can use it to protect data
        }
        #endregion

    }
}
