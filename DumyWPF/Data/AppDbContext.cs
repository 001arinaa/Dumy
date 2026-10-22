using DumyWPF.Models;
using Microsoft.EntityFrameworkCore;

namespace DumyWPF.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Practice> Practices => Set<Practice>();
        public DbSet<MoodEntry> MoodEntries => Set<MoodEntry>();

        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>Шлях до файлу бази: %LocalAppData%\Dumy\dumy.db</summary>
        public static string DbPath
        {
            get
            {
                var folder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Dumy");
                Directory.CreateDirectory(folder);
                return Path.Combine(folder, "dumy.db");
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Practice>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Title).IsRequired();
                e.Property(p => p.Type).IsRequired();
                e.Property(p => p.ProfileTag).IsRequired();
                e.Property(p => p.Instructions).IsRequired();
            });

            modelBuilder.Entity<MoodEntry>(e =>
            {
                e.HasKey(m => m.Id);
                e.Property(m => m.EmotionState).IsRequired();
            });

            // Seeding: 18 практик зі стартової бібліотеки контенту
            modelBuilder.Entity<Practice>().HasData(
                new Practice
                {
                    Id = 1,
                    Title = "Заспокійлива йога від тривоги",
                    Type = "Йога",
                    ProfileTag = "Щоденний баланс",
                    TargetState = "Тривога",
                    DurationMin = 25,
                    Instructions = "1. Сядьте зручно, зробіть 5 повільних вдихів, видих довший за вдих.\n2. Поза дитини: 1–2 хв, плечі розслаблені, чоло на килимку.\n3. Поза метелика сидячи: 1 хв, спина м'яко округлена.\n4. Лягте, обійміть коліна й м'яко похитайтесь 1 хв.\n5. Завершіть позою відпочинку 2 хв.",
                    VideoUrl = "https://youtu.be/7ZXw-UKfPus?si=fvMfhFoeXjqT8uIz"
                },
                new Practice
                {
                    Id = 2,
                    Title = "Заземлення на килимку (повільні пози)",
                    Type = "Йога",
                    ProfileTag = "Відновлення та фокус",
                    TargetState = "Тривога",
                    DurationMin = 15,
                    Instructions = "1. Станьте на килимок, відчуйте опору стоп, 5 глибоких вдихів.\n2. «Кішка-корова» у темпі дихання, 1–2 хв.\n3. Поза дитини 2 хв.\n4. Нахил уперед сидячи 2 хв.\n5. Лежачи на спині, 3 хв спокійного дихання.",
                    VideoUrl = "https://youtu.be/lRl0GyZTfNI?si=hrx7jG4YjdtBdViN"
                },
                new Practice
                {
                    Id = 3,
                    Title = "Вечірня йога перед сном",
                    Type = "Йога",
                    ProfileTag = "Щоденний баланс",
                    TargetState = "Тривога",
                    DurationMin = 10,
                    Instructions = "1. Приглушіть світло й вимкніть сповіщення.\n2. Повільні нахили та скручування сидячи, 5 хв.\n3. Поза «фігура чотири» лежачи: по 2 хв на бік.\n4. Поза дитини 2 хв.\n5. Лежачи на спині, 5 хв глибокого дихання.",
                    VideoUrl = "https://youtu.be/q003PKT1_Aw?si=pk8gnwAUaXQx31HX"
                },
                new Practice
                {
                    Id = 4,
                    Title = "М'яка відновлювальна йога",
                    Type = "Йога",
                    ProfileTag = "Відновлення та фокус",
                    TargetState = "Втома",
                    DurationMin = 20,
                    Instructions = "1. Підготуйте подушку або валик, зручно ляжте.\n2. Кожну позу утримуйте 2–3 хв без зусиль.\n3. Розкриття грудей лежачи, потім скручування лежачи.\n4. Поза дитини 2 хв.\n5. Завершіть спокійним лежанням 2 хв.",
                    VideoUrl = "https://youtu.be/Cnn1OuIlAoQ?si=p3S_2W23DtmT6FFV"
                },
                new Practice
                {
                    Id = 5,
                    Title = "Шия й плечі після комп'ютера",
                    Type = "Йога",
                    ProfileTag = "Відновлення та фокус",
                    TargetState = "Втома",
                    DurationMin = 15,
                    Instructions = "1. Сядьте рівно, опустіть плечі від вух.\n2. Повільні нахили голови вбік, по 30 с.\n3. Кола плечима: 10 назад, 10 вперед.\n4. Руки в замок за спиною, розкрийте груди, 30 с.\n5. Скручування сидячи, по 30 с на бік. Без болю.",
                    VideoUrl = "https://youtu.be/RcLCatIX0og?si=zvIDABDNk_i9CEH4"
                },
                new Practice
                {
                    Id = 6,
                    Title = "Ранкова йога для бадьорості",
                    Type = "Йога",
                    ProfileTag = "Щоденний баланс",
                    TargetState = "Втома",
                    DurationMin = 10,
                    Instructions = "1. Потягніться стоячи, руки вгору, 5 вдихів.\n2. Нахил уперед із повільним випрямленням.\n3. «Кішка-корова» 1 хв.\n4. Собака мордою вниз 1 хв.\n5. 2–3 повільні кола привітання сонцю.",
                    VideoUrl = "https://youtu.be/K1n0-VDj6EU?si=OaluRb7vTvEitXck"
                },
                new Practice
                {
                    Id = 7,
                    Title = "Йога для скидання напруги",
                    Type = "Йога",
                    ProfileTag = "Щоденний баланс",
                    TargetState = "Дратування",
                    DurationMin = 10,
                    Instructions = "1. 5 хв розминки: плечі, шия, тазостегнові суглоби.\n2. Низький випад і розкриття стегон, по 1–2 хв на бік.\n3. Скручування сидячи або лежачи, по 1 хв на бік.\n4. На видиху видихайте повільно через рот, відпускаючи напругу.\n5. Завершіть позою відпочинку 3 хв.",
                    VideoUrl = "https://youtu.be/EmZ6tsOvOsI?si=EcbQgOWr1IOrAejx"
                },
                new Practice
                {
                    Id = 8,
                    Title = "Динамічний флоу",
                    Type = "Йога",
                    ProfileTag = "Щоденний баланс",
                    TargetState = "Дратування",
                    DurationMin = 25,
                    Instructions = "1. Розминка 3 хв.\n2. Плавні зв'язки (віньяса) у темпі дихання, 10–12 хв.\n3. Спокійні нахили 3 хв.\n4. 2 хв лежачи. Якщо є біль, зменшіть амплітуду.",
                    VideoUrl = "https://youtu.be/iWpFMrrRnNM?si=tOL9dxhCNmWDzOtv"
                },
                new Practice
                {
                    Id = 9,
                    Title = "Скручування й дихання",
                    Type = "Йога",
                    ProfileTag = "Відновлення та фокус",
                    TargetState = "Дратування",
                    DurationMin = 20,
                    Instructions = "1. Сядьте, вирівняйте спину.\n2. Подовжений видих: вдих на 4, видих на 6, 2 хв.\n3. Скручування сидячи, по 5 вдихів на бік.\n4. Лежачи, коліна повільно вбік, по 1 хв.\n5. 1 хв тиші.",
                    VideoUrl = "https://youtu.be/EKa2osQGMTQ?si=9ZfJLjMdBkX1UUKX"
                },
                new Practice
                {
                    Id = 10,
                    Title = "Ранковий флоу для балансу",
                    Type = "Йога",
                    ProfileTag = "Щоденний баланс",
                    TargetState = "Спокій",
                    DurationMin = 20,
                    Instructions = "1. 5 вдихів із витягнутими руками.\n2. «Кішка-корова» 1 хв.\n3. Низький випад, по 5 вдихів на бік.\n4. Воїн 2, по 5 вдихів на бік.\n5. Нахил уперед 1 хв, потім 2 хв сидячи.",
                    VideoUrl = "https://youtu.be/reg1DMLw3Fo?si=ycq6wcXVEzZG2INq"
                },
                new Practice
                {
                    Id = 11,
                    Title = "Йога на гнучкість",
                    Type = "Йога",
                    ProfileTag = "Щоденний баланс",
                    TargetState = "Спокій",
                    DurationMin = 20,
                    Instructions = "1. Розминка хребта 3 хв.\n2. Нахили вперед сидячи й стоячи, по 1–2 хв.\n3. Розкриття стегон 5 хв.\n4. Розтяжка задньої поверхні стегна, по 1 хв на ногу.\n5. Спокійне завершення 3 хв.",
                    VideoUrl = "https://youtu.be/qXRKKXFyLTs?si=kYFBIuhAWJtW9ajl"
                },
                new Practice
                {
                    Id = 12,
                    Title = "Баланс і стійкість (стоячі пози)",
                    Type = "Йога",
                    ProfileTag = "Щоденний баланс",
                    TargetState = "Спокій",
                    DurationMin = 20,
                    Instructions = "1. Поза гори, 5 вдихів, стопи тверді.\n2. Поза дерева: 30–60 с на ногу.\n3. Поза орла: 30 с на бік.\n4. Поза стільця, 5 вдихів.\n5. Завершіть позою гори 1 хв.",
                    VideoUrl = "https://youtu.be/EpRxH1v-nqc?si=RrHxfMBQEmFr8ngY"
                },
                new Practice
                {
                    Id = 13,
                    Title = "Розтяжка шиї й плечей",
                    Type = "Розтяжка",
                    ProfileTag = "Відновлення та фокус",
                    TargetState = "Втома",
                    DurationMin = 25,
                    Instructions = "1. Опустіть плечі, нахиліть голову вбік, 30 с.\n2. Розтяжка плеча через груди, по 30 с.\n3. Руки за спиною, розкрийте груди, 30 с.\n4. Повторіть коло двічі.",
                    VideoUrl = "https://youtu.be/imWc_27U9w8?si=_eTX0spiTnx3GOCy"
                },
                new Practice
                {
                    Id = 14,
                    Title = "Вправа для очей",
                    Type = "Розтяжка",
                    ProfileTag = "Відновлення та фокус",
                    TargetState = "Втома",
                    DurationMin = 2,
                    Instructions = "1. Подивіться вдалину на предмет за кілька метрів, 20 с.\n2. Покліпайте 10 разів.\n3. Повільно покрутіть очима по колу в обидва боки.\n4. Потріть долоні й прикрийте ними заплющені очі на 30 с.",
                    VideoUrl = null
                },
                new Practice
                {
                    Id = 15,
                    Title = "Дихання 4-7-8",
                    Type = "Дихання",
                    ProfileTag = "Щоденний баланс",
                    TargetState = "Тривога",
                    DurationMin = 3,
                    Instructions = "1. Сядьте зручно, спина пряма.\n2. Вдих носом на 4 рахунки.\n3. Затримка на 7.\n4. Повільний видих ротом на 8.\n5. 4 цикли. Якщо паморочиться голова, дихайте звичайно.",
                    VideoUrl = null
                },
                new Practice
                {
                    Id = 16,
                    Title = "Тактичне дихання 4x4",
                    Type = "Дихання",
                    ProfileTag = "Тактичний",
                    TargetState = "Тривога",
                    DurationMin = 3,
                    Instructions = "1. Вдих носом, 4 с.\n2. Затримка, 4 с.\n3. Видих, 4 с.\n4. Затримка, 4 с.\n5. Повторіть 4–6 циклів.",
                    VideoUrl = null
                },
                new Practice
                {
                    Id = 17,
                    Title = "Метод заземлення 5-4-3-2-1",
                    Type = "Заземлення",
                    ProfileTag = "Тактичний",
                    TargetState = "Тривога",
                    DurationMin = 3,
                    Instructions = "1. Назвіть 5 речей, які бачите.\n2. 4 речі, яких можете торкнутися.\n3. 3 звуки, які чуєте.\n4. 2 запахи.\n5. 1 смак або один глибокий вдих.",
                    VideoUrl = null
                },
                new Practice
                {
                    Id = 18,
                    Title = "SOS-протокол заземлення",
                    Type = "Заземлення",
                    ProfileTag = "Тактичний",
                    TargetState = "Тривога",
                    DurationMin = 2,
                    Instructions = "1. Натисніть SOS на головному екрані.\n2. Поставте стопи на опору, відчуйте тиск підлоги.\n3. Дихайте за анімацією: вдих і видих у темпі кола.\n4. Після завершення спрощено оцініть стан.",
                    VideoUrl = null
                }
            );
        }
    }
}
