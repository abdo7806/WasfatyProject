using Microsoft.EntityFrameworkCore;
using Wasfaty.Application.DTOs.Doctors;
using Wasfaty.Application.Interfaces.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly ApplicationDbContext _context; // استبدل YourDbContext باسم سياق البيانات الخاص بك

    public DoctorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Doctor>> GetAllAsync()
    {
        /*   await _context.Users
         .Select(user => new UserDto
         {
             Id = user.Id,
             FullName = user.FullName,
             Email = user.Email,
             Role = (UserRoleEnum)user.RoleId,
             CreatedAt = user.CreatedAt
         }).ToListAsync();*/

        /*  return await _context.Doctors.Include(d => d.MedicalCenter)
              .Include(d => d.User).Select(doctor => new DoctorDto
              {
                  Id = doctor.Id,
                  UserId = doctor.UserId,
                  MedicalCenterId = doctor.MedicalCenterId,
                  Specialization = doctor.Specialization,
                  LicenseNumber = doctor.LicenseNumber,
              })            .Include(d => d.Pharmacy)
              .Include(d => d.User)
              .ToListAsync();*/

        return await _context.Doctors
            .Include(d => d.MedicalCenter)
            .Include(d => d.User)
            .ToListAsync();
    }

    public async Task<Doctor?> GetByIdAsync(int id)
    {
        return await _context.Doctors
            .Include(d => d.MedicalCenter)
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == id);
    }



    public async Task<Doctor> AddAsync(Doctor doctor)
    {
        await _context.Doctors.AddAsync(doctor);
        await _context.SaveChangesAsync();
        return doctor;
    }

    public async Task<Doctor> UpdateAsync(Doctor doctor)
    {
        _context.Doctors.Update(doctor);
        await _context.SaveChangesAsync();
        return doctor;
    }

    public async Task<bool> DeleteAsync(int id)
    {


        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {

                var doctor = await GetByIdAsync(id);

                if (doctor == null)
                {
                    return false; // الطبيب غير موجود
                }

                var User = doctor.User;
                if (User == null)
                {
                    return false; // المستخدم غير موجود
                }

                _context.Doctors.Remove(doctor);

                _context.Users.Remove(User);




                // حفظ التغييرات
                await _context.SaveChangesAsync();

                // تأكيد المعاملة
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                // إلغاء المعاملة في حالة حدوث خطأ
                await transaction.RollbackAsync();
                Console.WriteLine(ex.Message);
                return false;
            }
        }


    }

    public async Task<Doctor?> GetByUserIdAsync(int userId)
    {
        return await _context.Doctors
            .Include(d => d.MedicalCenter)
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.UserId == userId);
    }

    public async Task<DoctorDashboardDto> GetDashboardAsync(int doctorId)
    {
        try
        {
            var prescriptions = _context.Prescriptions.Where(p => p.DoctorId == doctorId);

            var totalPrescriptions = await prescriptions.CountAsync();
            var dispensedPrescriptions = await prescriptions.Where(p => p.IsDispensed).CountAsync();
            var pendingPrescriptions = await prescriptions.Where(p => !p.IsDispensed).CountAsync();
            var uniquePatients = await prescriptions.Select(p => p.PatientId).Distinct().CountAsync();

            return new DoctorDashboardDto
            {
                TotalPrescriptions = totalPrescriptions,
                DispensedPrescriptions = dispensedPrescriptions,
                PendingPrescriptions = pendingPrescriptions,
                UniquePatients = uniquePatients
            };
        }
        catch (Exception ex)
        {
            {
                return null;
            }

        }
    }
}