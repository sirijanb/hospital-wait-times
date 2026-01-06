using HQS.Application.DTOs;
using HQS.Domain.Entities;
using HQS.Infrastructure.Data;
using HQS.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HQS.Infrastructure.Services;

public class HospitalRepService
{
    private readonly UserManager<ApplicationUser> _userManager;
    // private readonly AppDbContext _dbContext;
    private readonly ApplicationDbContext _dbContext;

    public HospitalRepService(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task RegisterAsync(HospitalRepSignupDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            IsApproved = false
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            // throw new ApplicationException("Failed to register hospital representative");
            var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
            throw new ApplicationException($"Identity registration failed: {errors}");
        }
        try
        {
            await _userManager.AddToRoleAsync(user, "HospitalRep");

            var rep = new HospitalRep
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Email = user.Email,
                Name = dto.Name,
                HospitalId = dto.HospitalId,
                ContactNumber = dto.ContactNumber,
                IsApproved = false,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.HospitalRepresentatives.Add(rep);
            await _dbContext.SaveChangesAsync();
        }
        catch
        {
            // rollback identity user
            await _userManager.DeleteAsync(user);
            throw;
        }
    }

    public async Task<List<HospitalRep>> GetAllAsync()
    {
        return await _dbContext.HospitalRepresentatives
            .Include(r => r.Hospital)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task ApproveAsync(Guid repId)
    {
        var rep = await _dbContext.HospitalRepresentatives.FindAsync(repId);
        if (rep == null) return;

        rep.IsApproved = true;

        var user = await _userManager.FindByIdAsync(rep.UserId);
        if (user != null)
            user.IsApproved = true;

        await _dbContext.SaveChangesAsync();
    }

    public async Task RejectAsync(Guid repId)
    {
        var rep = await _dbContext.HospitalRepresentatives.FindAsync(repId);
        if (rep == null) return;

        _dbContext.HospitalRepresentatives.Remove(rep);

        var user = await _userManager.FindByIdAsync(rep.UserId);
        if (user != null)
            await _userManager.DeleteAsync(user);

        await _dbContext.SaveChangesAsync();
    }

    //new
    public async Task<Guid?> GetHospitalIdForUserAsync(string userId)
    {
        return await _dbContext.HospitalRepresentatives
            .Where(r => r.UserId == userId)
            .Select(r => r.HospitalId)
            .FirstOrDefaultAsync();
    }

}
