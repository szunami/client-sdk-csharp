# client-sdk-csharp

[![openupm](https://img.shields.io/npm/v/com.hathora.client?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.hathora.client/)

[Nuget Package](https://www.nuget.org/packages/Hathora.ClientSdk)

## Installation

### OpenUPM

```bash
openupm add com.hathora.client
```

### Nuget
```bash
dotnet add package Hathora.ClientSdk --version 2.0.0
```

## Usage 

```csharp
string appId = "...";
Hathora.Client client = new Hathora.Client(appId);
string token = await client.LoginAnonymous();
string roomId = await client.Create(token, new byte[] { });
ClientWebSocket webSocket = await client.Connect(token, roomId);
if (webSocket.State == WebSocketState.Open)
{
    ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
    WebSocketReceiveResult readResult = await webSocket.ReceiveAsync(bytesReceived, CancellationToken.None);
    await webSocket.SendAsync(Encoding.UTF8.GetBytes("{ message: \"Hello world\" }"), WebSocketMessageType.Binary, true, CancellationToken.None);
}

```

## Publishing Instructions

### OpenUPM

Update `package.json` version to $VERSION; commit this change.
```bash
git tag $VERSION
git push origin $VERSION
```
Then openUPM will trigger a build pipeline; see https://openupm.com/packages/com.hathora.client/?subPage=pipelines

### Nuget

Update the `client-sdk-csharp.csproj` `Version` property to `$VERSION`.
```bash
dotnet pack --configuration Release
nuget push ./bin/Release/Hathora.ClientSdk.$RELEASE.nupkg -Source https://api.nuget.org/v3/index.json
```
