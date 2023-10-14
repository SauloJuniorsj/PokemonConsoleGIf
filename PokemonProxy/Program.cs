using System;
using System.IO;
using System.Threading;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using static System.Net.Mime.MediaTypeNames;

class Program
{
    static void Main(string[] args)
    {
        string gifFilePath = "C:\\Users\\junio\\source\\repos\\PokemonProxy\\PokemonProxy\\25.gif"; // Substitua pelo caminho do seu arquivo GIF
        List<string[]> frames = LoadAsciiFrames(gifFilePath);
        int frameDelay = 25; // Defina o atraso em milissegundos entre quadros

        while (true)
        {
            Console.Clear(); // Limpa a tela do console

            foreach (string[] frame in frames)
            {
                foreach (var line in frame)
                {
                    Console.WriteLine(line);
                }
                Thread.Sleep(frameDelay); // Aguarda o atraso entre quadros
                Console.Clear(); // Limpa a tela para o próximo quadro
            }
        }
    }

    private static List<string[]> LoadAsciiFrames(string gifFilePath)
    {
        using (FileStream stream = File.OpenRead(gifFilePath))
        {
            using (Image<Rgba32> image = (Image<Rgba32>)SixLabors.ImageSharp.Image.Load(gifFilePath))
            {
                List<string[]> list = new List<string[]>();
                foreach (var item in image.Frames)
                {
                    int width = item.Width;
                    int height = item.Height;
                    string[] frames = new string[height];

                    for (int y = 0; y < height; y++)
                    {
                        string frame = "";
                        for (int x = 0; x < width; x++)
                        {
                            Rgba32 pixel = item[x, y];
                            frame += ConvertToColorizedAsciiChar(pixel);
                        }
                        frames[y] = frame;
                    }

                    list.Add(frames);
                }
                return list;
            }
        }
    }

    private static string ConvertToColorizedAsciiChar(Rgba32 pixel)
    {
        // Implemente a lógica para converter a cor do pixel em um caractere ASCII colorido
        int brightness = (int)(pixel.R * 0.3 + pixel.G * 0.59 + pixel.B * 0.11);
        char[] asciiChars = " .,(*/#%&".ToCharArray();
        int index = brightness * (asciiChars.Length - 1) / 255;

        // Define a cor com base na intensidade
        string colorCode = $"\u001b[38;2;{pixel.R};{pixel.G};{pixel.B}m";

        // Retorna o caractere colorido com código de escape ANSI
        return $"{colorCode}{asciiChars[index]}\u001b[0m";
    }

}
