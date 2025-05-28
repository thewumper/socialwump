using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using wumpapi.structures;

namespace wumpapi.api;

public record CreateUserRequest([Required] string Username, [Required] string Password, [Required] string FirstName, [Required] string LastName, [Required][EmailAddress] string Email);
public record LoginUserRequest([Optional] string Username, [Optional][EmailAddress] string Email, [Required] string Password);

public record LoginResponse(string SessionToken, User User);
public record ErrorResponse(string Message);
public record RelationshipCreatedResponse(Connection Connection);

public record LogoutRequest(string SessionToken);

public record CreateRelationshipRequest(string SessionToken, string TargetUser, string RelationshipName, string Data);
public record GetLeaderboardRequest(string Category);