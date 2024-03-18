using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FunctionPreMatricula.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Internal;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FunctionPreMatricula
{
    public class Function1
    {
        private readonly ILogger _logger;
        private readonly ConnectionFactory _factory;

        public Function1(ILoggerFactory loggerFactory, ConnectionFactory factory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
            _factory = factory;
        }

        [Function("Matricula")]
        public async Task<IActionResult> PutMatricula([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "PreMatricula")] HttpRequestData req, FunctionContext context)
        {
            _logger.LogInformation("Está sendo processada a Function Matricula");

            try
            {
                using (var reader = new StreamReader(req.Body))
                {
                    var requestBody = await reader.ReadToEndAsync().ConfigureAwait(false);
                    var preMatricula = JsonConvert.DeserializeObject<MatriculaCadastro>(requestBody);

                    if (preMatricula is null)
                    {
                        return new OkObjectResult($"Error") { StatusCode = 500 };
                    }

                    var mensagemServiceResponse = EnviaMensagem(preMatricula);
                    return new OkObjectResult($"Mensagem Service: {mensagemServiceResponse}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar a Function Put-Matricula");
                return new StatusCodeResult(500);
            }
        }

        private string EnviaMensagem(MatriculaCadastro message)
        {
            try
            {
                using (var connection = _factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(
                            queue: "messages",
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

                        var stringfiedMessage = JsonConvert.SerializeObject(message);
                        var bytesMessage = Encoding.UTF8.GetBytes(stringfiedMessage);

                        channel.BasicPublish(
                            exchange: "",
                            routingKey: "messages",
                            basicProperties: null,
                            body: bytesMessage);
                    }
                }

                _logger.LogInformation("Aluno cadastrado com sucesso.");
                return "Aluno cadastrado";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar aluno.");
                throw;
            }
        }
    }
}
