using System;
using System.Collections.Generic;

/// Константи, що використовуються для інтегральних схем (IC).

public static class IntegratedCircuitConstants
{
    /// Функціональність пінів.
    public static class PinFunctionality
    {
        /// Канал даних
        public const string DataChannel = "data channel";
        /// Імпульсний канал
        public const string PulseChannel = "pulse channel";
    }

    /// Методи отримання схеми.
    public enum SpawnMethod
    {
        /// Схема доступна в стандартному ящику зі схемами.
        Default = 1,
        /// Дизайн схеми стане доступним після покращення IC принтера.
        Research = 2
    }

    /// Категорії, що допомагають розрізнити схеми за типами дій.
    [Flags]
    public enum ActionCategory
    {
        /// Схема може переміщувати збірку.
        Movement = 1 << 0,
        /// Схема може завдати шкоди.
        Combat = 1 << 1,
        /// Схема може взаємодіяти з чимось за межами збірки.
        LongRange = 1 << 2
    }

    /// Типи даних для відображення в інтерфейсі.
    public static class FormatTypes
    {
        /// Довільний тип даних
        public const string Any = "<ANY>";
        /// Текстовий тип даних
        public const string String = "<TEXT>";
        /// Символьний тип даних
        public const string Char = "<CHAR>";
        /// Колірний тип даних
        public const string Color = "<COLOR>";
        /// Числовий тип даних
        public const string Number = "<NUM>";
        /// Тип даних напрямку
        public const string Dir = "<DIR>";
        /// Булевий тип даних
        public const string Boolean = "<BOOL>";
        /// Тип даних посилання
        public const string Ref = "<REF>";
        /// Списковий тип даних
        public const string List = "<LIST>";
        /// Індексний тип даних
        public const string Index = "<INDEX>";
        /// Імпульсний тип даних
        public const string Pulse = "<PULSE>";
    }

    /// Інформація про типи пінів, включаючи формат відображення та тип даних.
    public static class PinTypeInfo
    {
        /// Інформація про довільний тип піна.
        public static readonly (string Format, string Type) Any = (FormatTypes.Any, PinTypes.Any);
        /// Інформація про імпульсний вхідний пін.
        public static readonly (string Format, string Type) PulseIn = (FormatTypes.Pulse, PinTypes.PulseIn);
        /// Інформація про імпульсний вихідний пін.
        public static readonly (string Format, string Type) PulseOut = (FormatTypes.Pulse, PinTypes.PulseOut);
        /// Інформація про текстовий пін.
        public static readonly (string Format, string Type) String = (FormatTypes.String, PinTypes.String);
        /// Інформація про символьний пін.
        public static readonly (string Format, string Type) Char = (FormatTypes.Char, PinTypes.Char);
        /// Інформація про колірний пін.
        public static readonly (string Format, string Type) Color = (FormatTypes.Color, PinTypes.Color);
        /// Інформація про числовий пін.
        public static readonly (string Format, string Type) Number = (FormatTypes.Number, PinTypes.Number);
        /// Інформація про пін напрямку.
        public static readonly (string Format, string Type) Dir = (FormatTypes.Dir, PinTypes.Dir);
        /// Інформація про булевий пін.
        public static readonly (string Format, string Type) Boolean = (FormatTypes.Boolean, PinTypes.Boolean);
        /// Інформація про пін посилання.
        public static readonly (string Format, string Type) Ref = (FormatTypes.Ref, PinTypes.Ref);
        /// Інформація про списковий пін.
        public static readonly (string Format, string Type) List = (FormatTypes.List, PinTypes.List);
        /// Інформація про індексний пін.
        public static readonly (string Format, string Type) Index = (FormatTypes.Index, PinTypes.Index);
        /// Інформація про пін самопосилання.
        public static readonly (string Format, string Type) SelfRef = (FormatTypes.Ref, PinTypes.SelfRef); // Використовує FormatTypes.Ref бо SelfRef та Ref мають однаковий формат
    }


    /// Типи даних пінів для конструктора.
    public static class PinTypes
    {
        /// Скорочений ідентифікатор для /datum/integrated_io
        public const string Any = "IO";
        /// Скорочений ідентифікатор для /datum/integrated_io/string
        public const string String = "StringIO";
        /// Скорочений ідентифікатор для /datum/integrated_io/char
        public const string Char = "CharIO";
        /// Скорочений ідентифікатор для /datum/integrated_io/color
        public const string Color = "ColorIO";
        /// Скорочений ідентифікатор для /datum/integrated_io/number
        public const string Number = "NumberIO";
        /// Скорочений ідентифікатор для /datum/integrated_io/dir
        public const string Dir = "DirIO";
        /// Скорочений ідентифікатор для /datum/integrated_io/boolean
        public const string Boolean = "BooleanIO";
        /// Скорочений ідентифікатор для /datum/integrated_io/ref
        public const string Ref = "RefIO";
        /// Скорочений ідентифікатор для /datum/integrated_io/lists
        public const string List = "ListIO";
        /// Скорочений ідентифікатор для /datum/integrated_io/index
        public const string Index = "IndexIO";
        /// Скорочений ідентифікатор для /datum/integrated_io/selfref
        public const string SelfRef = "SelfRefIO";
        /// Скорочений ідентифікатор для /datum/integrated_io/activate
        public const string PulseIn = "PulseInIO";
        /// Скорочений ідентифікатор для /datum/integrated_io/activate/out
        public const string PulseOut = "PulseOutIO";
    }

    /// Максимальна довжина списків даних в IC.
    public const int MaxListLength = 500;

    /// Ключове слово для ідентифікації схеми під час дослідження.
    public const string InvestigateCircuit = "circuit";

    /// Ідентифікатори пінів для використання в коді.
    public static class PinIdentifiers
    {
        /// Префікс для вхідних пінів.
        public const string Input = "I";
        /// Префікс для вихідних пінів.
        public const string Output = "O";
        /// Префікс для пінів активаторів.
        public const string Activator = "A";

        /// Генерує ідентифікатор вхідного піна з вказаним номером.
        public static string GetInputPinId(int number) => $"{Input}{number}";

        /// Генерує ідентифікатор вихідного піна з вказаним номером.
        public static string GetOutputPinId(int number) => $"{Output}{number}";

        /// Генерує ідентифікатор піна активатора з вказаним номером.
        public static string GetActivatorPinId(int number) => $"{Activator}{number}";
    }

    /// Перевіряє, чи не перевищує довжина списку максимальне значення.
    /// Викидає InvalidOperationException, якщо список занадто довгий.
    public static void ValidateListLength<T>(List<T> list)
    {
        if (list.Count > MaxListLength)
        {
            throw new InvalidOperationException($"List exceeds maximum length of {MaxListLength}");
        }
    }
}
