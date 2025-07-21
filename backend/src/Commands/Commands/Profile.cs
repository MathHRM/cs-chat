using backend.Http.Responses;
using backend.Services;
using Microsoft.AspNetCore.SignalR;
using AutoMapper;
using backend.Models;
using System.CommandLine;

namespace backend.Commands;

public class Profile : CommandBase
{
    private readonly UserService _userService;
    private readonly IMapper _mapper;

    public override string CommandName => "profile";

    public override string Description => "Update your profile";

    public override bool ForAuthenticatedUsers => true;
    public override bool ForGuestUsers => false;

    public Profile(UserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    public override Dictionary<string, CommandArgument>? Args => new Dictionary<string, CommandArgument>
    {
        {
            "username",
            new CommandArgument {
                Name = "username",
                Description = "Update your username",
                Alias = "u",
            }
        },
        {
            "password",
            new CommandArgument {
                Name = "password",
                Description = "Update your password",
                Alias = "pass",
            }
        },
        {
            "confirm-password",
            new CommandArgument {
                Name = "confirm-password",
                Description = "Confirm your new password",
                Alias = "confirm",
            }
        }
    };

    public override async Task<CommandResult> Handle(ParseResult parseResult)
    {
        var username = parseResult.GetValue<string>(_username);
        var password = parseResult.GetValue<string>(_password);
        var confirmPassword = parseResult.GetValue<string>(_confirmPassword);
        var user = await _userService.GetUserByIdAsync(AuthenticatedUserId);

        if (username == null && password == null)
        {
            return CommandResult.FailureResult("Name or password is required", CommandName);
        }

        if (password != null && confirmPassword == null)
        {
            return CommandResult.FailureResult("Confirm password is required", CommandName);
        }

        if (password != null && confirmPassword != null && password != confirmPassword)
        {
            return CommandResult.FailureResult("Password and confirm password do not match", CommandName);
        }

        user.Username = username ?? user.Username;
        await _userService.UpdateUserAsync(user.Id, user, password);

        return new ProfileResult
        {
            Response = _mapper.Map<UserResponse>(user),
            Command = CommandName,
            Result = CommandResultEnum.Success,
            Message = "Profile updated successfully",
        };
    }

    public override Command GetCommandInstance()
    {
        var command = new Command(CommandName, Description)
        {
            _username,
            _password,
            _confirmPassword
        };
        command.TreatUnmatchedTokensAsErrors = false;
        return command;
    }

    // Arguments
    private readonly Option<string> _username = new Option<string>("--username", "-u") {
        Description = "Update your username",
    };
    private readonly Option<string> _password = new Option<string>("--password", "-pass") {
        Description = "Update your password",
    };
    private readonly Option<string> _confirmPassword = new Option<string>("--confirm-password", "-confirm") {
        Description = "Confirm your new password",
    };
}