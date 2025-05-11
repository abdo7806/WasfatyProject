using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Wasfaty.Application.DTOs.Patients;
using Wasfaty.Application.DTOs.Users;


namespace Wasfaty.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Patient> AddAsync(Patient patient)
        {
            
                await _context.Patients.AddAsync(patient);
                await _context.SaveChangesAsync();
                return patient;
          
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                try
                {
                    _context.Patients.Remove(patient);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception e)
                {
                    return false;

                }

            }
            return false;
        }

        public async Task<IEnumerable<PatientDto>> GetAllAsync()
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

            var Patients = await _context.Patients.Include(p => p.User).ToListAsync();

            return Patients.Select(patient => new PatientDto
            {
                Id = patient.Id,
                UserId = patient.UserId,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                BloodType = patient.BloodType,
                User = new UserDto
                {
                    Id = patient.User.Id,
                    FullName = patient.User.FullName,
                    Email = patient.User.Email, 
                    Role = (UserRoleEnum)patient.User.RoleId,
                    CreatedAt = patient.User.CreatedAt,
                },

            }); // يتضمن الوصفات

        }
        public async Task<Patient> GetByIdAsync(int id)
        {
            /* var patient = await _context.Patients.Include(p => p.User).Include(p => p.Prescriptions).FirstOrDefaultAsync(p => p.Id == id);
             if (patient != null)
             {
                 return new Patient
                 {
                     Id = patient.Id,
                     UserId = patient.UserId,
                     DateOfBirth = patient.DateOfBirth,
                     Gender = patient.Gender,
                     BloodType = patient.BloodType,
                     User = new User
                     {
                         Id = patient.User.Id,
                         FullName = patient.User.FullName,
                         Email = patient.User.Email,
                         RoleId = patient.User.RoleId,
                         CreatedAt = patient.User.CreatedAt,
                     },

                 };
             }

             else
             {
                 return null;
             }*/

            return await _context.Patients
    .Include(p => p.User)
    .Include(p => p.Prescriptions)
    .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Patient>> SearchPatients(string term)
        {

            return _context.Patients
        .Include(p => p.User)
        .Include(p => p.Prescriptions)
        .Where(p =>
            p.User.FullName.Contains(term) ||
            p.User.Email.Contains(term) ||
            p.Id.ToString().Contains(term))
        .ToList();
             
        }

        public async Task<Patient> UpdateAsync(Patient patient)
        {


            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
            return patient;
        }
    }
}
