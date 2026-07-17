# Homunculus

Discord bot written with .NET and [Discord.Net](https://github.com/discord-net/Discord.Net). Solution name: Homunculus; main project: `MarekMotykaBot`.

## Layout

| Project / path | Purpose |
|----------------|---------|
| `MarekMotykaBot/` | Bot executable: modules, services, resources |
| `ImfFlipAPI.APISource.Core/` | ImgFlip API client library |
| `MarekMotykaBot.Tests/` | xUnit tests |
| `deploy.sh` | Release publish, rsync to host, systemd restart |
| `Directory.Build.props` | Shared `net10.0` TFM |
| `Directory.Packages.props` | Central package versions |

## Prerequisites

- .NET 10 SDK
- Discord bot token and any API credentials you enable (YouTube, Imgur, ImgFlip)
- For deploy: SSH access and a systemd unit on the target host

## Build and test

```bash
dotnet build Homunculus.slnx
dotnet test MarekMotykaBot.Tests/MarekMotykaBot.Tests.csproj
```

## Local config

Create `MarekMotykaBot/configuration.json` (gitignored). Shape:

```json
{
  "prefix": ".",
  "tokens": {
    "discord": "",
    "youtube": "",
    "imgurClient": "",
    "imgurSecret": "",
    "destinationServerId": 0,
    "destinationChannelId": 0
  },
  "credentials": {
    "imgFlipUser": "",
    "imgFlipPassword": ""
  },
  "configValues": {
    "resourcePath": "/Resources/TextFiles/",
    "UTfolderPath": "",
    "streamAliasId": ""
  }
}
```

JSON under `MarekMotykaBot/Resources/TextFiles/` is local runtime data (also gitignored via `*.json`).

## Deploy

1. Copy `.env.template` to `.env` and fill in values.
2. Ensure the remote host already has `configuration.json` and live `Resources/TextFiles/` (deploy does not overwrite them).
3. Run:

```bash
./deploy.sh
```

`deploy.sh` publishes `MarekMotykaBot` (Release), rsyncs the publish output (excluding live JSON/config/logs), and restarts `SERVICE_NAME` via systemd.

## Agent notes

See `AGENTS.md` for the short agent map and hard constraints.
