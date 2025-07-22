using backend.Http.Responses;
using backend.Services;
using Microsoft.AspNetCore.SignalR;
using AutoMapper;
using backend.Models;
using System.CommandLine;

namespace backend.Commands;

public class Profile : CommandBase
{
    public override string CommandName => "profile";
    public override string Description => "Atualiza seu perfil";
    public override bool ForAuthenticatedUsers => true;
    public override bool ForGuestUsers => false;

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
        Description = "Atualiza seu username",
    };
    private readonly Option<string> _password = new Option<string>("--password", "-pass") {
        Description = "Atualiza sua senha",
    };
    private readonly Option<string> _confirmPassword = new Option<string>("--confirm-password", "-confirm") {
        Description = "Confirma sua nova senha",
    };

    private readonly UserService _userService;
    private readonly IMapper _mapper;

    public Profile(UserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    public override async Task<CommandResult> Handle(ParseResult parseResult)
    {
        var username = parseResult.GetValue(_username);
        var password = parseResult.GetValue(_password);
        var confirmPassword = parseResult.GetValue(_confirmPassword);
        var user = await _userService.GetUserByIdAsync(AuthenticatedUserId);

        if (username == null && password == null)
        {
            return CommandResult.FailureResult("Username ou senha são obrigatórios", CommandName);
        }

        if (password != null && confirmPassword == null)
        {
            return CommandResult.FailureResult("Senha de confirmação é obrigatória para atualizar a senha", CommandName);
        }

        if (password != null && confirmPassword != null && password != confirmPassword)
        {
            return CommandResult.FailureResult("Senha e senha de confirmação não coincidem", CommandName);
        }

        if (username != null && await _userService.UserExistsAsync(username))
        {
            return CommandResult.FailureResult("Usuário com este username já existe", CommandName);
        }

        user.Username = username ?? user.Username;
        await _userService.UpdateUserAsync(user.Id, user, password);

        return new ProfileResult
        {
            Response = _mapper.Map<UserResponse>(user),
            Command = CommandName,
            Result = CommandResultEnum.Success,
            Message = "Perfil atualizado com sucesso",
        };
    }
}