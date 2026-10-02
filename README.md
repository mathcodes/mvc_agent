# MVC Agent

A small ASP.NET Core MVC app that sends a prompt to the OpenAI Chat Completions API and displays the response.

## Requirements

- .NET 8 SDK
- An OpenAI API key with API access

## Configure and run

From this directory, set the API key in the same terminal where you will run the app:

```sh
export OPENAI_API_KEY="your-api-key"
dotnet run
```

Open the local URL printed by `dotnet run`. The default model is `gpt-4o-mini`; change `OpenAI:Model` in `appsettings.json` to use another model available to your account. Do not commit API keys to source control.