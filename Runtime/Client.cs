using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hathora
{
    public class Client
    {
        private readonly string appId;
        private readonly string coordinatorHost;
        private readonly HttpClient httpClient;


        public Client(string appId, string coordinatorHost)
        {
            this.appId = appId;
            this.coordinatorHost = coordinatorHost;
            this.httpClient = new HttpClient();
        }

        public Client(string appId)
        {
            this.appId = appId;
            this.coordinatorHost = "coordinator.hathora.dev";
            this.httpClient = new HttpClient();
        }

        class LoginResponse
        {
            public string token;
        }

        public async Task<string> LoginAnonymous()
        {
            HttpResponseMessage loginResponse = await httpClient.PostAsync($"https://{coordinatorHost}/{appId}/login/anonymous", null);
            string loginBody = await loginResponse.Content.ReadAsStringAsync();
            LoginResponse login = JsonConvert.DeserializeObject<LoginResponse>(loginBody);
            return login.token;
        }

        class CreateResponse
        {
            public string stateId;
        }

        public async Task<string> Create(string token, byte[] body)
        {
            HttpRequestMessage createRequest = new HttpRequestMessage(HttpMethod.Post, $"https://{coordinatorHost}/{appId}/create");
            createRequest.Content = new ByteArrayContent(body);
            createRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            createRequest.Headers.Add("Authorization", token);
            HttpResponseMessage createResponse = await httpClient.SendAsync(createRequest);
            string createBody = await createResponse.Content.ReadAsStringAsync();
            CreateResponse create = JsonConvert.DeserializeObject<CreateResponse>(createBody);
            return create.stateId;
        }

        public async Task<ClientWebSocket> Connect(string token, string stateId)
        {
            ClientWebSocket webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(new Uri($"wss://{coordinatorHost}/connect/{appId}"), CancellationToken.None);
            var bytesToSend = Encoding.UTF8.GetBytes($"{{\"token\": \"{token}\", \"stateId\": \"{stateId}\"}}");
            await webSocket.SendAsync(bytesToSend, WebSocketMessageType.Binary, true, CancellationToken.None);
            return webSocket;
        }

        class Token
        {
            public string type;
            public string id;
            public string name;
            public int iat;
        }

        // Source: https://stackoverflow.com/a/39280625/834459
        public static string GetUserFromToken(string token)
        {
            var parts = token.Split('.');
            if (parts.Length > 2)
            {
                var decode = parts[1];
                var padLength = 4 - decode.Length % 4;
                if (padLength < 4)
                {
                    decode += new string('=', padLength);
                }
                var bytes = System.Convert.FromBase64String(decode);
                string json = System.Text.ASCIIEncoding.ASCII.GetString(bytes);
                Token jwt = JsonConvert.DeserializeObject<Token>(json);
                return jwt.id;
            }

            return "";
        }
    }
}
