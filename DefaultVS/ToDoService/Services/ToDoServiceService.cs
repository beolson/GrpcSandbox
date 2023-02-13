using Google.Protobuf.Collections;
using Grpc.Core;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Caching.Memory;
using System;
using ToDoService;

namespace ToDoService.Services
{
    public class ToDoService : ToDo.ToDoBase
    {


        private readonly ILogger<ToDoService> _logger;
        public ToDoService(ILogger<ToDoService> logger)
        {
            _logger = logger;
        }

        //public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        //{
        //    return Task.FromResult(new HelloReply
        //    {
        //        Message = "Hello " + request.Name
        //    });
        //}

        private async Task CreateTable()
        {
            using var connection = new SqliteConnection("Data Source=hello.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
                    CREATE TABLE IF NOT EXISTS todos (
	                    id INTEGER PRIMARY KEY,
   	                    task TEXT NOT NULL
                    ); 
                ";


            await command.ExecuteNonQueryAsync();
        }

        public override async Task<CreateToDoResponse> CreateToDo(CreateToDoRequest request, ServerCallContext context)
        {
            await CreateTable();
            using var connection = new SqliteConnection("Data Source=hello.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
                    INSERT INTO ToDos (task)
                    VALUES ($task)
                ";
            command.Parameters.AddWithValue("$task", request.Task);

            await command.ExecuteNonQueryAsync();
            command.CommandText = "select last_insert_rowid()";

            int lastRowID = Convert.ToInt32((System.Int64)command.ExecuteScalar());

            return new CreateToDoResponse() { Id = lastRowID };

            //int32 id = 1;
            //return base.CreateToDo(request, context);
        }

        public override async Task<GetToDosResponse> GetToDos(GetToDosRequest request, ServerCallContext context)
        {

            using var connection = new SqliteConnection("Data Source=hello.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT id, task from ToDos
            ";

            var reader = await command.ExecuteReaderAsync();
            var response = new GetToDosResponse();
            while (await reader.ReadAsync())
            {
                var todo = new ToDoModel()
                {
                    Id = Convert.ToInt32(reader.GetInt64(0)),
                    Task = reader.GetString(1)
                };
                response.ToDos.Add(todo);
            }


            return response;
        }
    }
}