using System.Net;
using ImageMagick;

class Program
{
    static void Main(string[] args)
    {
        string gifFilePath = "C:\\Users\\junio\\source\\repos\\PokemonProxy\\PokemonProxy\\25.gif"; // Substitua pelo caminho do seu arquivo GIF
        List<string[]> frames = LoadAsciiFrames("https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-v/black-white/animated/3.gif");
        frames.RemoveAll(x => x.All(string.IsNullOrWhiteSpace));

        toGif(frames);

        //int frameDelay = 25; // Defina o atraso em milissegundos entre quadros
        //while (true)
        //{
        //    Console.Clear(); // Limpa a tela do console

        //    foreach (string[] frame in frames)
        //    {
        //        foreach (var line in frame)
        //        {
        //            Console.WriteLine(line);
        //        }
        //        Thread.Sleep(frameDelay); // Aguarda o atraso entre quadros
        //        Console.Clear(); // Limpa a tela para o próximo quadro
        //    }
        //}

    }

    private static List<string[]> LoadAsciiFrames(string gifFilePath)
    {

        using (WebClient client = new WebClient())
        {
            byte[] gifBytes = client.DownloadData(gifFilePath);

            using (MemoryStream inputMemoryStream = new MemoryStream(gifBytes))
            {

                //using (FileStream stream = File.OpenRead(gifFilePath))
                //{
                using (Image<Rgba32> image = (Image<Rgba32>)Image.Load(inputMemoryStream))
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
                //}
            }
        }
    }

    private static string ConvertToColorizedAsciiChar(Rgba32 pixel)
    {
        // Implemente a lógica para converter a cor do pixel em um caractere ASCII colorido
        int brightness = (int)(pixel.R * 0.3 + pixel.G * 0.59 + pixel.B * 0.11);
        char[] asciiChars = "  .,(*/#%&".ToCharArray();
        int index = brightness * (asciiChars.Length - 1) / 255;

        // Define a cor com base na intensidade
        string colorCode = $"\u001b[38;2;{pixel.R};{pixel.G};{pixel.B}m";

        // Retorna o caractere colorido com código de escape ANSI
        //return $"{colorCode}{asciiChars[index]}\u001b[0m";
        return asciiChars[index].ToString();
    }

    private static void toGif(List<string[]> list)
    {
        using (MemoryStream outputMemoryStream = new MemoryStream())
        {
            List<MagickImage> frames = new List<MagickImage>();

            MagickReadSettings settings = new MagickReadSettings
            {
                FontFamily = "Consolas, Courier New Regular",
                Font = "Consolas, Courier New Regular",
                FillColor = new MagickColor("white"),
                BackgroundColor = MagickColors.Black,

            };

            int maxWidth = 0;
            int totalHeight = 0;
            int fontSize = 14; // You can adjust this scaling factor as needed

            foreach (string[] asciiArt in list)
            {
                int maxLineLength = GetMaxLineLength(asciiArt);
                maxWidth = Math.Max(maxWidth, maxLineLength);
                totalHeight += asciiArt.Length;
            }

            // Define font size and line height
            int lineHeight = fontSize;

            // Calculate image width and height based on font size and ASCII art dimensions
            int imageWidth = maxWidth * fontSize;
            int imageHeight = totalHeight * lineHeight;

            int width = (list.MaxBy(x => x.Length).Length * fontSize);
            width = width - width / 3;
            int height = list.Count * fontSize + 1;
            for (int x = 0; x < list.Count; x++)
            {
                using (MagickImage image = new MagickImage(new MagickColor("black"), width, height))
                {
                    string[] asciiArt = list[x];
                    image.Settings.Font = "Consolas, Courier New Regular";
                    image.Settings.FontFamily = "Consolas, Courier New Regular";
                    image.Settings.FillColor = new MagickColor("white");

                    // Calculate the position to center the ASCII art vertically
                    int startY = (height - (asciiArt.Length * lineHeight)) / 2;

                    // Draw each line of ASCII art onto the image
                    for (int i = 0; i < asciiArt.Length; i++)
                    {
                        image.Draw(new DrawableText(10, startY + i * lineHeight, asciiArt[i]));
                    }

                    // Salve as imagens em um arquivo GIF
                    frames.Add((MagickImage)image.Clone());
                }
            }

            MagickImageCollection imageCollection = new MagickImageCollection(frames);
            imageCollection.Optimize();
            imageCollection.OptimizeTransparency();
            imageCollection.Write(outputMemoryStream, MagickFormat.Gif);

            // Salve o arquivo GIF em disco
            File.WriteAllBytes("output.gif", outputMemoryStream.ToArray());
        }

        Console.WriteLine("Arquivo GIF gerado com sucesso.");

    }
    static int GetMaxLineLength(string[] asciiArt)
    {
        int maxLength = 0;
        foreach (string line in asciiArt)
        {
            maxLength = Math.Max(maxLength, line.Length);
        }
        return maxLength;
    }
}
