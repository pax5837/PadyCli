using Infrastructure;
using Infrastructure.Either;

namespace CsProjMover.Implementations;

internal interface IOptionsValidationService
{
    Either<CsProjectMoverOptions, GenericError> ValidateOptions(CsProjectMoverOptions options);
}