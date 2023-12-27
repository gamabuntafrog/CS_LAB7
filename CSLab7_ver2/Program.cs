using Microsoft.EntityFrameworkCore;

Console.WriteLine("Hello World!");

var context = new MyDbContext();

// a) Запит на вибірку

var emails = context.mailing_list.ToList();

// b) запит на вибірку з використанням спеціальних функцій

var importantMails = context.mailing_list
                .Where(mail => mail.theme.Contains("New"))
                .ToList();

// c) запит зі складним критерієм;



var result = context.mailing_list.Join(
                    context.subscriber,
                    mailing => mailing.to_subscriber,
                    subscriber => subscriber.id,
                        (mailing, subscriber) => new
                        {
                            Mailing = mailing,
                            Subscriber = subscriber
                        })
                        .Where(obj =>
                            obj.Mailing.theme.Contains("important") &&
                            obj.Mailing.send_date > new DateTime(2023, 01, 01) &&
                            obj.Subscriber.username.StartsWith("john"))
                        .ToList();


// d) запит з унікальними значеннями;

var mailing = context.mailing_list
                .FirstOrDefault(mail => mail.id == 1);

// e) запит з використанням обчислювального поля;

var mailingsAfterDate = context.mailing_list
                .Where(mail => mail.send_date > new DateTime(2023, 12, 01))
                .ToList();

// f) запит з групуванням по заданому полю, використовуючи умову групування;

var themesWithCount = context.mailing_list
    .GroupBy(mail => mail.theme)
    .Where(group => group.Count() > 1)
    .Select(group => new
    {
        Theme = group.Key,
        ThemeCount = group.Count()
    })
    .ToList();

//  g) запит із сортування по заданому полю в порядку зростання та спадання значень;

var mailingsOrderedByDate = context.mailing_list
                .OrderByDescending(mail => mail.send_date)
                .ToList();

// -- h) запит з використанням дій по модифікації записів.
var mailingsToUpdate = context.mailing_list
               .Where(mail => mail.theme.Contains("important"))
               .ToList();

foreach (var m in mailingsToUpdate)
{
    m.theme = "Updated Theme";
}

context.SaveChanges();

public class MailingList
{
    public int id { get; set; }
    // theme, mail_description, send_date, to_subscriber
    public string theme { get; set; }

    public string mail_descripiton { get; set; }
    public DateTime send_date { get; set; }
    public int to_subscriber { get; set; }
}

public class Subscriber
{
    public int id { get; set; }
    public string full_name { get; set; }

    public string address { get; set; }

    public string username { get; set; }

    public string password { get; set; }

}

public class MyDbContext : DbContext
{
    public DbSet<MailingList> mailing_list { get; set; }
    public DbSet<Subscriber> subscriber { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SA2_Kyrylenko;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
    }
}