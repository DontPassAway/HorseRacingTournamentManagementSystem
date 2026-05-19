using AutoMapper;
using HorseRacing.Application.DTOs.Auth;
using HorseRacing.Application.Interfaces.Repositories;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Entities;
using HorseRacing.Domain.Enums;
using HorseRacing.Domain.Exceptions;

namespace HorseRacing.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IGenericRepository<User> _userRepo;
    private readonly IGenericRepository<HorseOwner> _ownerRepo;
    private readonly IGenericRepository<JockeyProfile> _jockeyRepo;
    private readonly IUnitOfWork _uow;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;

    public AuthService(
        IGenericRepository<User> userRepo,
        IGenericRepository<HorseOwner> ownerRepo,
        IGenericRepository<JockeyProfile> jockeyRepo,
        IUnitOfWork uow,
        IJwtService jwtService,
        IMapper mapper)
    {
        _userRepo = userRepo;
        _ownerRepo = ownerRepo;
        _jockeyRepo = jockeyRepo;
        _uow = uow;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existingEmail = await _userRepo.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (existingEmail != null)
            throw new BusinessException("Email already registered.");

        var existingUsername = await _userRepo.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (existingUsername != null)
            throw new BusinessException("Username already taken.");

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            Role = dto.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepo.AddAsync(user);
        await _uow.SaveChangesAsync();

        // Create role-specific profiles
        if (dto.Role == UserRole.HorseOwner)
        {
            await _ownerRepo.AddAsync(new HorseOwner { UserId = user.Id });
            await _uow.SaveChangesAsync();
        }
        else if (dto.Role == UserRole.Jockey)
        {
            await _jockeyRepo.AddAsync(new JockeyProfile { UserId = user.Id });
            await _uow.SaveChangesAsync();
        }

        var token = _jwtService.GenerateToken(user);
        return new AuthResponseDto(user.Id, user.Username, user.Email, user.FullName, user.Role, token);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepo.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new BusinessException("Invalid email or password.");

        if (!user.IsActive)
            throw new BusinessException("Your account has been deactivated.");

        var token = _jwtService.GenerateToken(user);
        return new AuthResponseDto(user.Id, user.Username, user.Email, user.FullName, user.Role, token);
    }

    public async Task<UserProfileDto> GetCurrentUserAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId)
            ?? throw new NotFoundException(nameof(User), userId);
        return _mapper.Map<UserProfileDto>(user);
    }
}
