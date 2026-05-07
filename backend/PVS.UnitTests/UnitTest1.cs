using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PVS.Api.Common.Repository;
using PVS.Api.Data;
using PVS.Api.Models;
using PVS.Api.Modules.Auth.Dtos;
using PVS.Api.Modules.Auth.Services;

namespace PVS.UnitTests;

public class AuthServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"AuthServiceTests-{Guid.NewGuid()}")
            .Options;

        _context = new AppDbContext(options);
        _authService = new AuthService(_context);
    }

    [Fact]
    public async Task RegisterAsync_Should_Create_New_User()
    {
        var request = new RegisterRequest
        {
            Email = "new.user@example.com",
            Password = "Secure123!",
            FirstName = "New",
            LastName = "User",
            Role = "Agent"
        };

        var user = await _authService.RegisterAsync(request);

        Assert.NotNull(user);
        Assert.Equal(request.Email, user.Email);
        Assert.Equal(request.FirstName, user.FirstName);
        Assert.Equal(request.LastName, user.LastName);
        Assert.Equal(request.Role, user.Role);
        Assert.NotEmpty(user.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash));
    }

    [Fact]
    public async Task RegisterAsync_Should_Throw_For_Duplicate_Email()
    {
        var request = new RegisterRequest
        {
            Email = "duplicate@example.com",
            Password = "Secure123!",
            FirstName = "Duplicate",
            LastName = "User",
            Role = "Agent"
        };

        await _authService.RegisterAsync(request);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _authService.RegisterAsync(request);
        });
    }

    [Fact]
    public async Task AuthenticateAsync_Should_Return_User_When_Credentials_Are_Valid()
    {
        var request = new RegisterRequest
        {
            Email = "login@example.com",
            Password = "Secure123!",
            FirstName = "Login",
            LastName = "User"
        };

        var createdUser = await _authService.RegisterAsync(request);
        var authenticatedUser = await _authService.AuthenticateAsync(request.Email, request.Password);

        Assert.NotNull(authenticatedUser);
        Assert.Equal(createdUser.Id, authenticatedUser!.Id);
    }

    [Fact]
    public async Task AuthenticateAsync_Should_Return_Null_For_Invalid_Password()
    {
        var request = new RegisterRequest
        {
            Email = "invalidpass@example.com",
            Password = "Secure123!",
            FirstName = "Invalid",
            LastName = "Password"
        };

        await _authService.RegisterAsync(request);
        var authenticatedUser = await _authService.AuthenticateAsync(request.Email, "WrongPassword");

        Assert.Null(authenticatedUser);
    }

    [Fact]
    public async Task ChangePasswordAsync_Should_Update_PasswordHash()
    {
        var request = new RegisterRequest
        {
            Email = "changepass@example.com",
            Password = "Original123!",
            FirstName = "Change",
            LastName = "Password"
        };

        var user = await _authService.RegisterAsync(request);
        var result = await _authService.ChangePasswordAsync(user.Id, request.Password, "NewPassword123!");

        Assert.True(result);

        var updatedUser = await _context.Users.FindAsync(user.Id);
        Assert.NotNull(updatedUser);
        Assert.True(BCrypt.Net.BCrypt.Verify("NewPassword123!", updatedUser!.PasswordHash));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

public class GenericRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly GenericRepository<User> _repository;

    public GenericRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"GenericRepositoryTests-{Guid.NewGuid()}")
            .Options;

        _context = new AppDbContext(options);
        _repository = new GenericRepository<User>(_context);
    }

    [Fact]
    public async Task AddAsync_Should_Save_Entity()
    {
        var user = new User
        {
            Email = "repo@example.com",
            FirstName = "Repo",
            LastName = "Tester",
            PasswordHash = "password"
        };

        var saved = await _repository.AddAsync(user);

        Assert.Equal(1, saved);
        var persisted = await _repository.GetByIdAsync(user.Id);
        Assert.NotNull(persisted);
        Assert.Equal(user.Email, persisted!.Email);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Entities()
    {
        await _repository.AddAsync(new User { Email = "one@example.com", FirstName = "One", LastName = "User", PasswordHash = "pass1" });
        await _repository.AddAsync(new User { Email = "two@example.com", FirstName = "Two", LastName = "User", PasswordHash = "pass2" });

        var users = await _repository.GetAllAsync();

        Assert.Equal(2, users.Count());
    }

    [Fact]
    public async Task UpdateAsync_Should_Modify_Entity()
    {
        var user = new User
        {
            Email = "update@example.com",
            FirstName = "Before",
            LastName = "Change",
            PasswordHash = "password"
        };

        await _repository.AddAsync(user);
        user.FirstName = "After";

        var saved = await _repository.UpdateAsync(user);

        Assert.Equal(1, saved);
        var persisted = await _repository.GetByIdAsync(user.Id);
        Assert.Equal("After", persisted!.FirstName);
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Entity()
    {
        var user = new User
        {
            Email = "delete@example.com",
            FirstName = "Delete",
            LastName = "Tester",
            PasswordHash = "password"
        };

        await _repository.AddAsync(user);
        var deleted = await _repository.DeleteAsync(user);

        Assert.Equal(1, deleted);
        var persisted = await _repository.GetByIdAsync(user.Id);
        Assert.Null(persisted);
    }

    [Fact]
    public async Task FindAsync_Should_Filter_By_Predicate()
    {
        await _repository.AddAsync(new User { Email = "findme@example.com", FirstName = "Find", LastName = "Me", PasswordHash = "password" });
        await _repository.AddAsync(new User { Email = "other@example.com", FirstName = "Other", LastName = "User", PasswordHash = "password" });

        var found = await _repository.FindAsync(u => u.Email.Contains("findme"));

        Assert.Single(found);
        Assert.Equal("findme@example.com", found.First().Email);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
