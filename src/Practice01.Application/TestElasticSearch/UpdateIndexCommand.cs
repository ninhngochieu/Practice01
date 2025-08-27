using MediatR;

namespace Practice01.Application.TestElasticSearch;

public class UpdateIndexCommand : IRequest
{

}


public class UpdateIndexCommandHandler : IRequestHandler<UpdateIndexCommand>
{
    Task IRequestHandler<UpdateIndexCommand>.Handle(UpdateIndexCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
