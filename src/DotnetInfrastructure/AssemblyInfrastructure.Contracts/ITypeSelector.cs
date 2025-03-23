using System.Reflection;
using Infrastructure;
using Infrastructure.Either;

namespace DotnetInfrastructure.Contracts;

public interface ITypeSelector
{
    Either<Type, ExitRequested> SelectType(string typeIdentifier, Assembly assembly);
}