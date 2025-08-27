using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice01.Application.TestElasticSearch;
public class CreateIndexCommand : IRequest<string>
{

}


public class IndexCommandHandler : IRequestHandler<CreateIndexCommand, string>
{
    public Task<string> Handle(CreateIndexCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
