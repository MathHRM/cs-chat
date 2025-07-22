using backend.Commands;
using Microsoft.AspNetCore.Mvc;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Drawing;
using System.Text;
namespace backend.Http.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly CommandHandler _commandHandler;

    public TestController(CommandHandler commandHandler)
    {
        _commandHandler = commandHandler;
    }

    private const string ASCII_CHARS = "@%#*+=-:. ";

    [HttpGet]
    [Route("testando")]
    public IActionResult Get()
    {
        try
        {
            // Get the path to the image file relative to the Controllers directory
            var originalImage = new Bitmap("C:\\Users\\MathHRM\\code\\chat\\backend\\Http\\Controllers\\image2.jpg");
            var resized = ResizeImage(originalImage, 10000);
            var grayscale = ToGrayscale(resized);
            string asciiArt = ToAscii(grayscale);
            return Ok(asciiArt);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return StatusCode(500, $"An error occurred: {ex.Message} {ex.GetType()} {ex.StackTrace}");
        }
    }

    private static Bitmap ResizeImage(Bitmap image, int newWidth)
    {
        int newHeight = (int)(image.Height * ((double)newWidth / image.Width) * 0.5);
        return new Bitmap(image, newWidth, newHeight);
    }

    private static Bitmap ToGrayscale(Bitmap image)
    {
        Bitmap grayscaleImage = new Bitmap(image.Width, image.Height);
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                Color pixelColor = image.GetPixel(x, y);
                int grayValue = (int)(pixelColor.R * 0.21 + pixelColor.G * 0.72 + pixelColor.B * 0.07);
                Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);
                grayscaleImage.SetPixel(x, y, grayColor);
            }
        }
        return grayscaleImage;
    }

    private static string ToAscii(Bitmap image)
    {
        var sb = new StringBuilder();
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                Color pixelColor = image.GetPixel(x, y);
                int brightness = pixelColor.R;
                int charIndex = (brightness * (ASCII_CHARS.Length - 1)) / 255;
                sb.Append(ASCII_CHARS[charIndex]);
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
