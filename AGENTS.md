# AGENTS.md

Homunculus is a Discord bot (Discord.Net / .NET 10) plus a small ImgFlip API helper library.

## Look here first

| Path | Role |
|------|------|
| `README.md` | Setup, build, test, deploy, config shape |
| `MarekMotykaBot/Program.cs` | DI registration and app startup |
| `MarekMotykaBot/Modules/` | Discord command modules |
| `MarekMotykaBot/Services/Core/` | Bot infrastructure (commands, timers, JSON, scanning) |
| `MarekMotykaBot/Services/External/` | YouTube, Imgur, ImgFlip, Nyaa, Unreal Tournament |
| `MarekMotykaBot/Resources/TextFiles/` | Runtime JSON data (local; not deployed by `deploy.sh`) |
| `ImfFlipAPI.APISource.Core/` | ImgFlip HTTP API client |
| `MarekMotykaBot.Tests/` | Unit tests |
| `deploy.sh` | Publish + rsync + systemd restart |

## Hard constraints

- Never commit secrets: `configuration.json`, `.env`, tokens, API keys, passwords.
- Do not change `deploy.sh` excludes for `Resources/TextFiles/`, `configuration.json`, `Logs/`, `Errors/`, or `Custom/` without an explicit reason — those keep live server state intact.
- Prefer existing service interfaces under `Services/*/Interfaces` over new ad-hoc helpers.
- Keep `AGENTS.md` short; put deeper detail in `README.md` (or `docs/` only if that becomes necessary).

## Verify

```bash
dotnet build Homunculus.slnx
dotnet test MarekMotykaBot.Tests/MarekMotykaBot.Tests.csproj
```

## Canonical docs

Canonical docs today: `README.md` and this file.

Update them in the same change when you alter user-facing bot behavior, public/module command surfaces, DI wiring, deploy/config requirements, or anything already documented here or in `README.md`.
