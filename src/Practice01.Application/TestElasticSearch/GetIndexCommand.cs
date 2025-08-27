using MediatR;

namespace Practice01.Application.TestElasticSearch;

public class GetIndexCommand : IRequest<string>
{

}

public class GetIndexCommandHandler : IRequestHandler<GetIndexCommand, string>
{
    public Task<string> Handle(GetIndexCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}