using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Ktpt.MusicSchool.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;

namespace Ktpt.MusicSchool.Views;

public static class UiSupport
{
    private const string AssetsRoot = "avares://Ktpt.MusicSchool/Assets/";
    private static readonly Dictionary<string, Bitmap> cache = new();

    public static IBrush Brush(string key)
        => (IBrush)Application.Current!.Resources[key]!;

    public static Bitmap Picture(string? fileName)
    {
        string uri = AssetsRoot + "Images/" + fileName;
        if (string.IsNullOrWhiteSpace(fileName) || !AssetLoader.Exists(new Uri(uri)))
            uri = AssetsRoot + "picture.png";

        if (!cache.TryGetValue(uri, out Bitmap? bitmap))
        {
            using var stream = AssetLoader.Open(new Uri(uri));
            bitmap = new Bitmap(stream);
            cache[uri] = bitmap;
        }
        return bitmap;
    }

    public static string ErrorText(Exception error)
    {
        if (error is AppException) return error.Message;
        if (error is SqlException || error is DbUpdateException)
            return "Ошибка базы данных. Проверьте, что SQL Server запущен и строка в connection.txt верна.";
        if (error is IOException)
            return "Не найден файл connection.txt рядом с программой.";
        return "Операция не выполнена: " + error.Message;
    }
}