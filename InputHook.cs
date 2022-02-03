using System.Collections.Generic;
using System.Windows.Forms;

namespace MagicSwitcher
{
    public enum InputEventType
    {
        KeyDown, // Клавиша нажата
        KeyUp, // Клавиша отпущена
        MouseDown, // Кнопка мыши нажата
        MouseUp, // Кнопка мыши отпущена
        MouseWheel // Событие колеса мыши
    }

    // Событие клавиатуры или мыши
    public class InputEvent
    {
        public InputEventType EventType { get; set; } // Тип события
        public Keys Key { get; set; } // Какая клавиша клавиатуры
        public MouseButtons Button { get; set; } // Какая клавиша мыши
        public int Delta { get; set; } // Информация о событии колеса мыши

        public override string ToString()
        {
            string res = "";
            switch (EventType)
            {
                case InputEventType.KeyDown:
                    res += $"KD_{Key}";
                    break;
                case InputEventType.KeyUp:
                    res += $"KU_{Key}";
                    break;
                case InputEventType.MouseDown:
                    res += $"MD_{Button}";
                    break;
                case InputEventType.MouseUp:
                    res += $"MU_{Button}";
                    break;
                case InputEventType.MouseWheel:
                    res += $"MW_{Delta}";
                    break;
            }
            return res;
        }

        public override bool Equals(object obj)
        {
            var res = false;
            if (obj.GetType() == this.GetType())
            {
                InputEvent inputEvent = (InputEvent)obj;
                if(this.EventType == inputEvent.EventType)
                {
                    switch (inputEvent.EventType)
                    {
                        case InputEventType.KeyDown:
                        case InputEventType.KeyUp:
                            res = this.Key == inputEvent.Key;
                            break;
                        case InputEventType.MouseDown:
                        case InputEventType.MouseUp:
                            res = this.Button == inputEvent.Button;
                            break;
                        case InputEventType.MouseWheel:
                            res = this.Delta == inputEvent.Delta;
                            break;
                    }
                }
            }

            return res;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }

    // Последовательность событий клавиатуры
    public class InputSequence
    {
        // Наименование последовательности
        public string Name { get; set; }
        // Делегат и событие для отслеживания срабатывания
        public delegate void KbdShortcutHandler(string name);
        public event KbdShortcutHandler Triggered;

        // Последовательность нажатий клавиш для вызова события
        private readonly List<InputEvent> sequence = new List<InputEvent>();

        public InputSequence(string name)
        {
            Name = name;
        }

        // Длина последовательности
        public int Count
        {
            get { return sequence.Count; }
        }

        // Индексатор
        public InputEvent this[int index]
        {
            get { return sequence[index]; }
            set { sequence[index] = value; }
        }

        // Добавление события в последовательность
        public void Add(InputEvent inputEvent)
        {
            sequence.Add(inputEvent);
        }

        public override string ToString()
        {
            string res = Name + ":";
            foreach (var item in sequence)
            {
                res += " " + item.ToString();
            }
            return res;
        }

        public void Trigger()
        {
            Triggered(Name);
        }
    }

    // Класс, позволяющий отслеживать любую последовательность нажатий клавиш на клавиатуре и мыши
    public class InputHook
    {
        private readonly List<InputEvent> inputEvHistory = new List<InputEvent>();
        private readonly List<InputSequence> sequences = new List<InputSequence>();
        private const int MAX_KEY_SEQUENCE_LEN = 10;

        public InputHook()
        {
            GlobalKeyboardHook.Instance.KeyDown += new KeyEventHandler(KeyDown);
            GlobalKeyboardHook.Instance.KeyUp += new KeyEventHandler(KeyUp);
            GlobalMouseHook.Instance.MouseDown += new MouseEventHandler(MouseDown);
            GlobalMouseHook.Instance.MouseUp += new MouseEventHandler(MouseUp);
            GlobalMouseHook.Instance.MouseWheel += new MouseEventHandler(MouseWheel);
        }

        public void AddSequence(InputSequence sequence)
        {
            sequences.Add(sequence);
        }

        private void ChkInputEvHistory()
        {
            LogInputEvHistory();
            // Проверяем по очереди все последовательности
            foreach (var item in sequences)
            {
                // Если последовательность сработала
                if (IsTriggered(item))
                {
                    // Вызываем событие и прекращаем проверку
                    item.Trigger();
                    break;
                }
            }
        }

        private bool IsTriggered(InputSequence kbdShortcut)
        {
            int j = inputEvHistory.Count - 1;
            for (int i = kbdShortcut.Count - 1; i >= 0; i--)
            {
                if (j < 0)
                {
                    return false;
                }
                // System.Diagnostics.Debug.WriteLine($"Check: j={j} H: {kbdEvHistory[j].EventType}-{kbdEvHistory[j].Key}: i={i} S: {kbdShortcut[i].EventType}-{kbdShortcut[i].Key}");
                if ((inputEvHistory[j].EventType != kbdShortcut[i].EventType) ||
                    (inputEvHistory[j].Key != kbdShortcut[i].Key))
                {
                    //System.Diagnostics.Debug.WriteLine($" false 1");
                    return false;
                }
                j--;
            }
            //System.Diagnostics.Debug.WriteLine($" true");
            return true;
        }

        void LogInputEvHistory()
        {
            System.Diagnostics.Debug.WriteLine("---");
            for (int i = 0; i < inputEvHistory.Count; i++)
            {
                System.Diagnostics.Debug.WriteLine($"{i}. {inputEvHistory[i]}");
            }
            System.Diagnostics.Debug.WriteLine("---");
        }

        void AddInputEvToHistory(InputEvent inputEvent)
        {
            // System.Diagnostics.Debug.WriteLine(inputEvent.ToString());
            if (!IsRepeat(inputEvent))
            {
                inputEvHistory.Add(inputEvent);
                // В истории храним только несколько последних нажатий клавиш
                if (inputEvHistory.Count > MAX_KEY_SEQUENCE_LEN)
                {
                    inputEvHistory.RemoveAt(0);
                }
                ChkInputEvHistory();
            }
        }

        bool IsRepeat(InputEvent inputEvent)
        {
            var res = false;
            if (inputEvent.EventType == InputEventType.KeyDown || inputEvent.EventType == InputEventType.KeyUp)
            {
                for (int i = inputEvHistory.Count - 1; i >= 0; i--)
                {
                    if (inputEvHistory[i].EventType == InputEventType.KeyDown || inputEvHistory[i].EventType == InputEventType.KeyUp)
                    {
                        res = inputEvHistory[i].Equals(inputEvent);
                        break;
                    }
                }
            }

            return res;
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            AddInputEvToHistory(new InputEvent() { EventType = InputEventType.KeyDown, Key = e.KeyCode });
        }

        private void KeyUp(object sender, KeyEventArgs e)
        {
            AddInputEvToHistory(new InputEvent() { EventType = InputEventType.KeyUp, Key = e.KeyCode });
        }

        private void MouseDown(object sender, MouseEventArgs e)
        {
            AddInputEvToHistory(new InputEvent() { EventType = InputEventType.MouseDown, Button = e.Button });
        }

        private void MouseUp(object sender, MouseEventArgs e)
        {
            AddInputEvToHistory(new InputEvent() { EventType = InputEventType.MouseUp, Button = e.Button });
        }

        private void MouseWheel(object sender, MouseEventArgs e)
        {
            AddInputEvToHistory(new InputEvent() { EventType = InputEventType.MouseWheel, Delta = e.Delta });
        }
    }
}
