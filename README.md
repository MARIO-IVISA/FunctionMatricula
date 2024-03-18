# Azure Function para Envio de Novas Matrículas

## Tags
`azure-function` `net6.0` `rabbitmq` `matriculas`

Este é um Azure Function desenvolvido em .NET 6.0, destinado ao envio de novas matrículas para uma fila RabbitMQ.

## Funcionamento

O Azure Function é acionado para enviar novas matrículas para uma fila RabbitMQ. Ele recebe os dados da matrícula como entrada e os envia para a fila configurada.

## Tecnologias Utilizadas

- Azure Function (.NET 6.0)
- RabbitMQ
