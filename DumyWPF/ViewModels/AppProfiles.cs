namespace DumyWPF.ViewModels
{
    /// <summary>Назви профілів — збігаються зі значеннями ProfileTag у таблиці практик.</summary>
    public static class AppProfiles
    {
        public const string Daily = "Щоденний баланс";
        public const string Focus = "Відновлення та фокус";
        public const string Tactical = "Тактичний";
    }

    /// <summary>Стани настрою — збігаються зі значеннями TargetState у таблиці практик.</summary>
    public static class MoodStates
    {
        public const string Calm = "Спокій";
        public const string Anxious = "Тривога";
        public const string Tired = "Втома";
        public const string Irritated = "Дратування";

        // Службові типи записів в історії
        public const string Sos = "SOS Заземлення";
        public const string TacticalState = "Тактичний стан";
        public const string Exhaustion = "Виснаження";

        public static string EmojiFor(string state) => state switch
        {
            Calm => "😊",
            Anxious => "😟",
            Tired => "😫",
            Irritated => "😡",
            Sos => "🚨",
            TacticalState => "⚡",
            Exhaustion => "⚙",
            _ => "📝"
        };
    }

    /// <summary>Точні назви практик з бази (для швидкого запуску з екранів).</summary>
    public static class PracticeTitles
    {
        public const string Sos = "SOS-протокол заземлення";
        public const string TacticalBreathing = "Тактичне дихання 4x4";
        public const string Grounding = "Метод заземлення 5-4-3-2-1";
        public const string EyeExercise = "Вправа для очей";
        public const string Breathing478 = "Дихання 4-7-8";
    }
}
