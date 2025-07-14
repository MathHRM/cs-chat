using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace backend.Http.Controllers;

[ApiController]
[Route("api/a")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var command = ParseCommand("/join --chatId=123 aaaaa -c=2323 --dcdcdc -c -d=\"aa aa   aa\" bbbbbb");
        return Ok(command);
    }

    public static Dictionary<string, string>? ParseCommand(string input)
    {
        if (string.IsNullOrWhiteSpace(input) || !input.Trim().StartsWith("/"))
        {
            return null;
        }

        var results = new Dictionary<string, string>();

        var parts = Regex.Matches(input, @"[\""].+?[\""]|[^ ]+")
            .Cast<Match>()
            .Select(m => m.Value.Trim('"'))
            .ToList();

        if (parts.Count > 0)
        {
            results["command"] = parts[0].Substring(1);
        }

        int nonNamedArgIndex = 0;

        for (int i = 1; i < parts.Count; i++)
        {
            var part = parts[i];

            if (part.StartsWith("-"))
            {
                string[] argParts = part.Split(new[] { '=' }, 2);
                string argName = argParts[0].TrimStart('-');

                if (argParts.Length == 2)
                {
                    results[argName] = argParts[1];

                    continue;
                }

                results[argName] = "true";

                continue;
            }

            results[nonNamedArgIndex.ToString()] = part;
            nonNamedArgIndex++;
        }

        return results;
    }
}