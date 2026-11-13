using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Ktpt.MusicSchool.Data;

public static class Db
{
    public static MusicSchoolContext Create()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "connection.txt");
        string connection = File.ReadAllText(path).Trim();

        var options = new DbContextOptionsBuilder<MusicSchoolContext>()
            .UseSqlServer(connection)
            .Options;

        return new MusicSchoolContext(options);
    }
}