// using Asp.Versioning;
// using MediatR;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Practice01.Application.TestMessage;
// using Practice01.Presentation.Common.ObjectResult;
//
// namespace Practice01.Presentation.Controllers;
//
// [ApiController]
// [Route("api/v{version:apiVersion}/[controller]")]
// [ApiVersion("1.0")]
// [ApiVersion("2.0")]
// public class TestKafkaController : ControllerBase
// {
//     private readonly IMediator _mediator;
//     private readonly CustomObjectResult _customObjectResult;
//
//     public TestKafkaController(IMediator mediator, CustomObjectResult customObjectResult)
//     {
//         _mediator = mediator;
//         _customObjectResult = customObjectResult;
//     }
//
//     [HttpPost("null-key")]
//     public async Task<IResult> ProduceNullKey([FromBody] NullKeyTestMessageCommand nullKeyTestWriteKafkaCommand)
//     {
//         await _mediator.Send(nullKeyTestWriteKafkaCommand);
//         return _customObjectResult.Return();
//     }
//
//     [HttpPost("key")]
//     public async Task<IResult> ProduceKey([FromBody] KeyTestMessageCommand nullKeyTestWriteKafkaCommand)
//     {
//         await _mediator.Send(nullKeyTestWriteKafkaCommand);
//         return _customObjectResult.Return();
//     }
// }