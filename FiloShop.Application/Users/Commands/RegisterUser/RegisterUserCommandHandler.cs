using FiloShop.Application.Authentication;
using FiloShop.Domain.Shared.ValueObjects;
using FiloShop.Domain.Users.Entities;
using FiloShop.Domain.Users.IRepository;
using FiloShop.SharedKernel.CQRS.Commands;
using FiloShop.SharedKernel.Interfaces;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Application.Users.Commands.RegisterUser;

internal sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public RegisterUserCommandHandler(IUserRepository userRepository,
        IUnitOfWork unitOfWork, IAuthenticationService authenticationService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _authenticationService = authenticationService;
    }

    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = User.Create(FirstName.Create(request.FirstName).Value,
            LastName.Create(request.LastName).Value,
            Email.Create(request.Email).Value).Value;

        var identityId = await _authenticationService.RegisterAsync(
            user, request.Password, cancellationToken);

        user.SetIdentityId(identityId);

        _userRepository.Add(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}