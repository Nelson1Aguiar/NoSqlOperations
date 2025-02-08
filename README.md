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
```

## Configuração
Adicione os singletons que deseja usar, é necessário adicionar tanto o de conexão quando o de operações do DataBase.

```sh
builder.Services.AddSingleton<IConnectionMongoDB, ConnectionMongoDB>();
builder.Services.AddSingleton<IMongoOperations, MongoOperations>();

builder.Services.AddSingleton<IConnectionRedis, ConnectionRedis>();
builder.Services.AddSingleton<IRedisOperation, RedisOperations>();
```

Adicione as strings de conexão no AppSettings com os devidos nomes:

```sh
"MongoConnection": "String de conexão do mongo",
"MongoDataBaseConnection": "nome do DataBase mongo",
"RedisConnection": "String de conexão redis:porta do redis,password=Sua senha"
```

## Uso
Exemplos para uso de métodos do mongo:

```sh
    public class MongoDocumentTRA
    {
        private readonly IMongoOperations _mongoOperations;

        public MongoDocumentTRA (IMongoOperations mongoOperations)
        {
            _mongoOperations = mongoOperations;
        }

        public void SavePerson(Person person)
        {
            _mongoOperations.InsertInMongoAsync(person, "Bmi");
        }

        public async Task<List<Person>> GetPersonListAsync(Person person)
        {

            List<Person> people = await _mongoOperations.GetListInMongoAsync<Person>(p =>
                                                              p.Name == person.Name &&
                                                              p.Id_user == person.Id_user,
                                                              "Bmi");
            return people;
        }

        public List<Person> GetPersonList(Person person)
        {

            List<Person> people = _mongoOperations.GetListInMongo<Person>(p =>
                                                              p.Name == person.Name &&
                                                              p.Id_user == person.Id_user,
                                                              "Bmi");
            return people;
        }

        public Person GetPerson(Person person)
        {

           Person personGet = _mongoOperations.GetInMongo<Person>(p =>
                                                                  p.PersonId == person.PersonId,
                                                                  "Bmi");
            return personGet;
        }

        public async Task<Person> GetPersonAsync(Person person)
        {

            Person personGet = await _mongoOperations.GetInMongoAsync<Person>(p =>
                                                                   p.PersonId == person.PersonId,
                                                                   "Bmi");
            return personGet;
        }

        public void UpdatePerson(Person person)
        {
            _mongoOperations.UpdateInMongoAsync<Person>( p =>
                                                p.PersonId == person.PersonId &&
                                                p.Id_user == person.Id_user,
                                                "Bmi", 
                                                person);
        }

        public void DeletePerson(Person person)
        {
            _mongoOperations.DeleteInMongoAsync<Person>(p =>
                                                p.PersonId == person.PersonId &&
                                                p.Id_user == person.Id_user,
                                                "Bmi");
        }
    }

```

Exemplos para uso de métodos do Redis:

```sh

    public class RedisTRA
    {
        private readonly IRedisOperation _redisOperation;

        public RedisTRA(IRedisOperation redisOperation)
        {
            _redisOperation = redisOperation;
        }

        public void PersonSetData(Person person, string redisKey)
        {
            _redisOperation.SetData(person, redisKey);
        }

        public Person PersonGetData(Person person, string redisKey)
        {
            Person personInCache = _redisOperation.GetData<Person>(redisKey);

            return person;
        }
        
        public List<Person> PersonGetAllDataByNameAndUserId(Person person, string redisKey)
        {
            List<Person> people = _redisOperation.GetAllDataByKey<Person>(redisKey);

            return people;
        }
    }

```

## Contribuição

Sinta-se à vontade para abrir issues e pull requests para melhorias na biblioteca.

##Licença

Este projeto está licenciado sob a MIT License - veja o arquivo LICENSE para mais detalhes.
