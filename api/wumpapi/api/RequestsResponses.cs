using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using wumpapi.game;
using wumpapi.structures;

namespace wumpapi.api;

/// <summary>
/// This File contains a list of all of the requests and server response data
/// This is a class because I wanted a comment at the top of the file and didnt want it to be associated with the request directly bellow this
/// </summary>
public static class RequestResponse;


public record CreateUserRequest([Required] string Username, [Required] string Password, [Required] string FirstName, [Required] string LastName, [Required][EmailAddress] string Email);
public record LoginUserRequest([Optional] string Username, [Optional][EmailAddress] string Email, [Required] string Password);

public record LoginResponse([Required] string SessionToken,[Required] User User);
public record ErrorResponse([Required] string Message);

public record ValidateAuthRequest([Required] string SessionToken);
public record ValidateAuthResponse([Required] bool Success, [Required] User? User);
public record RelationshipCreatedResponse([Required] Connection Connection);

public record LogoutRequest([Required] string SessionToken);

public record CreateRelationshipRequest([Required] string SessionToken, [Required] string TargetUser, [Required] string RelationshipName,[Required] string Data);
public record GetLeaderboardRequest([Required] string Category);

public record PlayerCountResponse([Required] int Players, int RequiredPlayers);

public record PlayerAuthRequest([Required] string SessionToken);
public record PlayerJoinResponse([Required] Player Player, [Required] User User);
public record AbilityRequest([Required] string SessionToken, [Required] int ItemSlot, [Required] string Target);

public record AllianceRequest([Required] string SessionToken, [Required] string AllianceName);
public record AllianceResponse([Required] Alliance Alliance);
public record AllianceLeaveRequest([Required] string SessionToken);

public record ShopPurchaseRequest([Required] string SessionToken, [Required] string ItemId);
public record ShopSellRequest([Required] string SessionToken, [Required] int ItemSlot);

public record ShopPurchaseResponse([Required] IItem Item);

public record PowerShareRequest([Required] string SessionToken, [Required] string Username, [Required] int PowerAmount);
public record ItemShareRequest([Required] string SessionToken, [Required] string Username, [Required] int Slot);