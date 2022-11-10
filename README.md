# client-sdk-csharp

[![openupm](https://img.shields.io/npm/v/com.hathora.client?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.hathora.client/)

[Nuget Package](https://www.nuget.org/packages/Hathora.ClientSdk)

## Publishing Instructions

### OpenUPM

Update package.json version to $VERSION; commit this change.
```bash
git tag $VERSION
git push origin $VERSION
```
Then openUPM will trigger a build pipeline; see https://openupm.com/packages/com.hathora.client/?subPage=pipelines
