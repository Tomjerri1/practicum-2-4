using Ardalis.Result;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Nimble.Modulith.Customers.Domain.CustomerAggregate;
using Nimble.Modulith.Customers.Domain.Interfaces;
using Nimble.Modulith.Email.Contracts;
using Nimble.Modulith.Users.Contracts;
using Nimble.Modulith.Users;

namespace Nimble.Modulith.Customers.UseCases.Customers.Commands;

public record CreateCustomerCommand(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Street,
    string City,
    string State,
    string PostalCode,
    string Country
) : ICommand<Result<CustomerDto>>;

public record CustomerDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    AddressDto Address,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record AddressDto(
    string Street,
    string City,
    string State,
    string PostalCode,
    string Country);


public class CreateCustomerHandler(
    IRepository<Customer> repository, 
    IMediator mediator,
    UserManager<Nimble.Modulith.Users.ApplicationUser> userManager) 
    : ICommandHandler<CreateCustomerCommand, Result<CustomerDto>> 
{
    public async ValueTask<Result<CustomerDto>> Handle(CreateCustomerCommand command, CancellationToken ct)
    {
        var existingUser = await userManager.FindByEmailAsync(command.Email);
        string? temporaryPassword = null;

        if (existingUser == null)
        {
            temporaryPassword = Guid.NewGuid().ToString("N")[..12];

            var createUserCommand = new CreateUserCommand(command.Email, temporaryPassword);
            var userResult = await mediator.Send(createUserCommand, ct);

            if (!userResult.IsSuccess)
            {
                return Result<CustomerDto>.Error($"Failed to create user account: {userResult.Errors.FirstOrDefault()?.ToString()}");
            }
        }

        var customer = new Customer
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            PhoneNumber = command.PhoneNumber,
            Address = new Address
            {
                Street = command.Street,
                City = command.City,
                State = command.State,
                PostalCode = command.PostalCode,
                Country = command.Country
            },
            CreatedAt = DateTime.UtcNow
        };

        await repository.AddAsync(customer, ct);
        await repository.SaveChangesAsync(ct);

        if (temporaryPassword != null)
        {
            var emailBody = $@"
Welcome to our service!
Your account has been created successfully.

Email: {command.Email}
Temporary Password: {temporaryPassword}

Please log in and change your password as soon as possible.";

            var emailCommand = new SendEmailCommand(
                command.Email,
                "Welcome - Your Account Has Been Created",
                emailBody
            );

            await mediator.Send(emailCommand, ct);
        }

        return Result<CustomerDto>.Success(new CustomerDto(
            customer.Id, customer.FirstName, customer.LastName, customer.Email, customer.PhoneNumber,
            new AddressDto(customer.Address.Street, customer.Address.City, customer.Address.State, customer.Address.PostalCode, customer.Address.Country),
            customer.CreatedAt, customer.UpdatedAt));
    }
}