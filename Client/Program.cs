using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Server;

namespace Client
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Server.UserService.UserServiceClient(channel);

           // await UnaryCallExample1(client);
            await UnaryCallExample2(client);

            //await ServerStreamingCallExample(client);

            Console.WriteLine("Shutting down");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task UnaryCallExample2(Server.UserService.UserServiceClient client)
        {
            var reply = client.Authenticate(new UserSignInRequest
            {
                User = new UserSignInRequest.Types.UserSignInInfo
                {
                    Password = "test",
                    UserName = "test"
                }
            });
            Console.WriteLine("JWT: " + reply.JWT);
        }

        private static async Task UnaryCallExample1(Server.UserService.UserServiceClient client)
        {
            var reply = client.Create(new UserSignUpRequest
            {
                FirstName = "test",
                LastName = "test",
                UserName = "test",
                Email = "test",
                Password = "test",
            });
            Console.WriteLine("JWT: " + reply.JWT);
        }

        //private static async Task ServerStreamingCallExample(Server.UserService.UserServiceClient client)
        //{
        //    var cts = new CancellationTokenSource();
        //    cts.CancelAfter(TimeSpan.FromSeconds(3.5));

        //    using(var call = client.SayHellos(new HelloRequest { Name = "GreeterClient" }, cancellationToken: cts.Token))
        //    {
        //        try
        //        {
        //            await foreach(var message in call.ResponseStream.ReadAllAsync())
        //            {
        //                Console.WriteLine("Greeting: " + message.Message);
        //            }
        //        }
        //        catch(RpcException ex) when(ex.StatusCode == StatusCode.Cancelled)
        //        {
        //            Console.WriteLine("Stream cancelled.");
        //        }
        //    }
        //}
    }
}

