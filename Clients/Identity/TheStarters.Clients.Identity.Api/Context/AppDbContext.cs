using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TheStarters.Clients.Identity.Api.Services.Identity.Models;

namespace TheStarters.Clients.Identity.Api.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options);