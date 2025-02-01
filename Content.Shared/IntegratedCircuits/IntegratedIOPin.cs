using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;
using System.Linq;

namespace Content.Shared.IntegratedCircuits
{
    ///
    /// Базовий клас для інтегрованих пінів (IO).
    ///
    [Serializable, NetSerializable]
    public abstract partial class IntegratedIO // Зроблено клас абстрактним
    {
        /// Тип IO піна (канал даних або імпульсний канал).
        public string IoType { get; } = IntegratedCircuitConstants.PinFunctionality.DataChannel; // Використовуємо константу з IntegratedCircuitConstants

        /// Тип піна (використовується для збереження конфігурації).
        public string? PinType { get; } // Зроблено тип допускаючим null

        /// Ім'я піна.
        public string Name { get; } = "input/output";

        /// Порядковий номер піна (для послідовності виконання).
        public int Ord { get; }

        /// Утримувач піна (інтегральна схема).
        public object? Holder { get; internal set; } // Зроблено Holder допускаючим null!

        /// Слабке посилання на дані, що зберігаються в піні.
        public object? Data { get; set; } // Зроблено тип допускаючим null

        /// Список пінів, з якими цей пін пов'язаний.
        public List<IntegratedIO> Linked { get; } = new List<IntegratedIO>();

        public IntegratedIO(object holder, string name = "input/output", object? data = null, string? pinType = null, int ord = 0) // Зроблено параметри data та pinType допускаючими null
        {
            Name = name;
            Holder = holder;
            Data = data;
            PinType = pinType;
            Ord = ord;

            if (Holder == null)
            {
                throw new ArgumentNullException(nameof(holder), "IntegratedIO spawned without a valid holder! This is a bug.");
            }
        }

        #region Public API - Methods for Pin Interaction and Data Handling

        public virtual void WriteDataToPin(object newData) // Перенесено write_data_to_pin proc
        {
            if (newData == null || newData is int || newData is string) // Перевірка на null, int та string
            {
                Data = newData;
            }
            else if (newData is List<object> newList) // Перевірка на List<object> -  C# list of objects
            {
                if (newList.Count > 0) // Щоб уникнути помилок, якщо список порожній
                {
                    var truncatedList = newList.GetRange(Math.Max(0, newList.Count - IntegratedCircuitConstants.MaxListLength), Math.Min(newList.Count, IntegratedCircuitConstants.MaxListLength)); // Копіювання та обрізання списку
                    Data = truncatedList; // Збереження обрізаної копії
                }
                else
                {
                    Data = newList; // Збереження порожнього списку без обрізання
                }
            }
            // WeakRef аналог в C# потребує додаткової уваги, поки що пропускаємо isweakref(new_data) перевірку

            if (Holder != null) // Перевірка на null перед викликом методу holder
            {
                // Припускаємо, що Holder має метод OnDataWritten. Потрібно уточнити тип Holder для безпечного виклику.
                if (Holder is IIntegratedCircuitHolder holder) // Приклад перевірки інтерфейсу (замінити IIntegratedCircuitHolder на правильний тип/інтерфейс)
                {
                    holder.OnDataWritten(); // Виклик OnDataWritten() на holder, якщо він реалізує інтерфейс
                }
                else
                {
                    // Якщо Holder не реалізує інтерфейс, можна спробувати викликати метод через reflection (але краще використовувати інтерфейс)
                    // Або просто пропустити виклик, якщо OnDataWritten не є обов'язковим для всіх типів Holder
                    // Holder.GetType().GetMethod("OnDataWritten")?.Invoke(Holder, null); // Приклад виклику через reflection (не рекомендується)
                    Console.WriteLine("Warning: Holder does not implement IIntegratedCircuitHolder, OnDataWritten() not called."); // Попередження, якщо Holder не має потрібного методу
                }
            }
        }

        public virtual bool IsValid()
        {
            return Data != null;
        }

        public virtual string DisplayData(object input) // Перенесено display_data proc
        {
            if (input == null)
            {
                return "(null)"; // Пусті дані - нічого показувати.
            }

            if (input is string text)
            {
                return $"(\"{text}\")"; // Обгортання рядка в лапки
            }

            if (input is List<object> myList)
            {
                var result = $"list\\[{myList.Count}\\](";
                if (myList.Count > 0)
                {
                    result += "<br>"; // HTML linebreak - може знадобитися заміна на щось інше для RobustToolbox UI
                    for (var i = 0; i < myList.Count; i++)
                    {
                        result += DisplayData(myList[i]); // Рекурсивний виклик для елементів списку
                        if (i < myList.Count - 1)
                        {
                            result += ",<br>"; // HTML linebreak - може знадобитися заміна
                        }
                    }
                    result += "<br>"; // HTML linebreak - може знадобитися заміна
                }
                result += ")";
                return result;
            }

            // WeakRef аналог в C# потребує додаткової уваги, поки що пропускаємо isweakref(input) та w.resolve()

            return $"({input})"; // Для чисел та інших типів даних - стандартне відображення
        }

        public virtual string DisplayPinType() // Перенесено display_pin_type proc
        {
            return IntegratedCircuitConstants.FormatTypes.Any; // За замовчуванням повертає IC_FORMAT_ANY
        }

        #endregion

        #region Pin Connection and Disconnection Methods

        /// Намагається привести дані піна до вказаного типу.
        public virtual T? DataAsType<T>() where T : class // Перенесено data_as_type proc, використовуємо generics та обмеження where T : class
        {
            if (Data is T output) // Використовуємо pattern matching 'is' та 'as'
            {
                return output; // Повертаємо приведене значення
            }

            return null; // Повертаємо null, якщо приведення не вдалося
        }

        protected virtual void DisconnectAll() // Перенесено disconnect_all proc, зроблено protected virtual
        {
            // Ітерація по копії списку, щоб уникнути проблем з модифікацією колекції під час ітерації
            var linkedPinsCopy = new List<IntegratedIO>(Linked); // Створення копії списку для ітерації

            foreach (var pin in linkedPinsCopy)
            {
                DisconnectPin(pin); // Виклик DisconnectPin для кожного пов'язаного піна
            }

            Linked.Clear(); // Очищення списку linked після роз'єднання всіх пінів
        }

        protected virtual void DisconnectPin(IntegratedIO pin) // Перенесено disconnect_pin proc, зроблено protected virtual
        {
            if (pin == null) // Додано перевірку на null для безпеки
                return;

            pin.Linked.Remove(this); // Видалення поточного піна (this) зі списку 'Linked' вхідного піна (pin)
            Linked.Remove(pin);      // Видалення вхідного піна (pin) зі списку 'Linked' поточного піна (this)
        }

        /// Передає дані з цього піна на всі пов'язані піни.
        protected virtual void PushData() // Перенесено push_data proc, зроблено protected virtual
        {
            foreach (var linkedPin in Linked) // Ітерація по списку Linked
            {
                if (Data != null) // Додано перевірку на null для Data
                {
                    linkedPin.WriteDataToPin(Data); // Виклик WriteDataToPin для кожного пов'язаного піна, передаючи поточні Data
                }
                // else - можна додати логування або іншу обробку, якщо Data == null є нештатною ситуацією
            }
        }

        // Отримує дані від пов'язаних пінів та записує їх в цей пін.
        protected virtual void PullData() // Перенесено pull_data proc, зроблено protected virtual
        {
            foreach (var linkedPin in Linked) // Ітерація по списку Linked
            {
                if (linkedPin != null) // Додано перевірку на null для linkedPin (безпека)
                {
                    if (linkedPin.Data != null) // Додано перевірку на null для linkedPin.Data
                    {
                        WriteDataToPin(linkedPin.Data); // Виклик WriteDataToPin для поточного піна, передаючи Data від linkedPin
                    }
                    // else - можна додати логування, якщо linkedPin.Data == null є нештатною ситуацією
                }
                // else - можна додати логування, якщо null linkedPin є нештатною ситуацією
            }
        }

        /// Повертає текстовий опис пінів, пов'язаних з цим піном.
        public virtual string GetLinkedToDesc() // Перенесено get_linked_to_desc proc, зроблено public virtual
        {
            if (Linked.Count > 0) // Перевірка довжини списку Linked
            {
                // Використовуємо LINQ для створення списку імен пов'язаних пінів та їх з'єднання через string.Join
                var linkedPinNames = Linked.Select(pin => pin.Name).ToList(); // Отримуємо імена пов'язаних пінів
                return $"the {string.Join(", ", linkedPinNames)}"; // Форматуємо список імен в рядок, аналогічно english_list в DM
            }

            return "nothing"; // Список порожній - повертаємо "nothing"
        }

        /// З'єднує цей пін з вказаним піном.
        protected virtual void ConnectPin(IntegratedIO pin) // Перенесено connect_pin proc, зроблено protected virtual
        {
            if (pin == null) // Додано перевірку на null для безпеки
                return;

            if (!Linked.Contains(pin)) // Перевірка, чи пін вже не пов'язаний, щоб уникнути дублювання
            {
                Linked.Add(pin);     // Додавання вхідного піна (pin) до списку Linked поточного піна (this)
            }
            if (!pin.Linked.Contains(this)) // Двостороння перевірка для уникнення дублювання з іншого боку
            {
                pin.Linked.Add(this); // Додавання поточного піна (this) до списку 'Linked' вхідного піна (pin) - ДВОСТОРОННІЙ ЗВ'ЯЗОК!
            }
        }

        /// Генерує випадкові дані або повідомлення про помилку та передає їх на пов'язані піни.
        protected virtual void Scramble() // Перенесено scramble proc, зроблено protected virtual
        {
            if (Data == null) // Перевірка на null для Data
                return;

            if (Data is int) // Перевірка, чи Data є числом
            {
                var random = new System.Random(); // Створення екземпляра Random (Використовуємо повне ім'я System.Random)
                WriteDataToPin(random.Next(-10000, 10001)); // Генерування випадкового числа та запис в пін
            }
            else if (Data is string) // Перевірка, чи Data є рядком
            {
                WriteDataToPin("ERROR"); // Запис "ERROR" в пін
            }

            PushData(); // Виклик PushData для розповсюдження даних
        }

        #endregion

        #region Lifecycle Methods

        public virtual void Destroy()
        {
            DisconnectAll();
            Data = null;
            Holder = null;
        }

        #endregion

        // TODO: Перенести ask_for_data_type(), ask_for_pin_data()
    }

    // Інтерфейс-приклад для Holder - ЗАМІНИТИ НА РЕАЛЬНИЙ ТИП/ІНТЕРФЕЙС
    public interface IIntegratedCircuitHolder
    {
        void OnDataWritten();
        void CheckThenDoWork(int ord, bool ignore_power = false);
        void InvestigateLog(string message, string category);
        bool CheckInteractivity(object user);
    }
}
