using MediatR;

namespace Practice01.Application.TestElasticSearch;

public class DeleteIndexCommand : IRequest
{

}

public class DeleteIndexCommandHandler : IRequestHandler<DeleteIndexCommand>
{
    Task IRequestHandler<DeleteIndexCommand>.Handle(DeleteIndexCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}