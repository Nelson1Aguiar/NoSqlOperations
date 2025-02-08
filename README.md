# Library .NET - NoSQL

## Introdução
Esta é uma biblioteca genérica em .NET, criada para facilitar o uso de bancos NoSQL, atualmente englobando Redis e MongoDB. A biblioteca estabelece conexão automática ao Redis e ao MongoDB utilizando injeção de dependência. Além disso, inclui diversas operações pré-definidas para facilitar a manipulação de dados nesses bancos.

## Recursos
- Conexão automática ao Redis e ao MongoDB.
- Configuração simples via Singleton.
- Operações CRUD pré-implementadas para ambos os bancos.
- Suporte para cache no Redis.
- Facilidade na gestão de coleções no MongoDB.

## Instalação
Adicione a biblioteca ao seu projeto via NuGet:

```sh
Install-Package NoSql.Core
