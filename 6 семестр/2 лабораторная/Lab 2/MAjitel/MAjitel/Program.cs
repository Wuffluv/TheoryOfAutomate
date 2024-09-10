using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lab_1
{
    class Program
    {
        /// <summary>
        /// Правило языка
        /// </summary>
        public class Rule
        {
            /// <summary>
            /// Порождающая цепочка языка
            /// </summary>
            public string Key { get; set; }
            /// <summary>
            /// Порождаемая цепочка языка
            /// </summary>
            public string Value { get; set; }
            /// <summary>
            /// Введет ли правило зацикливанию.
            /// True - введет, false - правило не зацикливает грамматику
            /// </summary>
            public bool IsLooped { get; set; }

            public Rule(string k, string v, bool l = false)
            {
                Key = k;
                Value = v;
                IsLooped = l;
            }
        }


        /// <summary>
        /// Функция вывода всех правил
        /// </summary>
        /// <param name="R"></param>
        public static void PrintRules(List<Rule> R)
        {
            Console.WriteLine("Правила для языка");
            for (int i = 0; i < R.Count; i++)
            {
                Console.WriteLine("   \u2022" + R[i].Key + "-->" + R[i].Value);
            }
        }

        /// <summary>
        /// Класс формального языка с леволинейной грамматикой и проверкой на зацикливание
        /// </summary>
        public class FormalLanguage
        {

            /// <summary>
            /// Правила языка
            /// </summary>
            private List<Rule> _rules { get; set; }
            /// <summary>
            /// Максимально количество повторений
            /// </summary>
            public uint MaxRepetitionsCount { get; set; }

            public FormalLanguage(List<Rule> rules, uint count = 10000)
            {
                _rules = rules;
                MaxRepetitionsCount = count;
            }

            /// <summary>
            /// Проверяет правило на зацикливание
            /// </summary>
            /// <param name="input">Строка, к которой применяется правило</param>
            /// <param name="rule">Правило языка</param>
            /// <param name="count">Количество допустимых повторений</param>
            /// <param name="isReverse">Проверять ли с конца строки.</param>
            /// <returns>true - если правило зацикливает перевод, иначе - false</returns>
            private bool CheckLoop(string input, Rule rule, int count = 5, bool isReverse = false)
            {
                for (int i = 0; i < count; i++)
                {
                    string key = rule.Key;
                    string value = rule.Value;

                    int pos;
                    if (isReverse) pos = input.LastIndexOf(key);
                    else pos = input.IndexOf(key);
                    if (pos != -1)
                    {
                        input = input.Remove(pos, key.Length);
                        input = input.Insert(pos, value);
                    }
                    else return false;
                }

                return true;
            }
            /// <summary>
            /// Переводит строку на формальный язык
            /// </summary>
            /// <param name="text">Строка для перевода</param>
            /// <returns>Строка на формальном языке</returns>
            public string Translate(string text)
            {
                int count = 0;
                bool isEnd = false; // true - если ни одно из правил непреминимо
                while (count < MaxRepetitionsCount)
                {
                    if (isEnd) break;

                    count++;
                    isEnd = true;
                    // применяем по очереди каждое правило языка к строке
                    foreach (Rule rule in _rules)
                    {
                        if (!rule.IsLooped)     // если правило зацикливает
                        {
                            string key = rule.Key;
                            string value = rule.Value;

                            int pos = text.IndexOf(key);

                            if (pos != -1)  // если ключ найден
                            {
                                // если правило зацикливает перевод - запоминаем это
                                if (CheckLoop(text, rule)) rule.IsLooped = true;
                                else
                                {
                                    text = text.Remove(pos, key.Length);
                                    text = text.Insert(pos, value);
                                    isEnd = false;

                                    break;
                                }
                            }
                        }
                        else rule.IsLooped = !rule.IsLooped;
                    }
                }

                RefreshRules();
                return text;
            }
            /// <summary>
            /// Переводит строку на формальный язык. Анализирует строку справа
            /// </summary>
            /// <param name="text">Строка для перевода</param>
            /// <returns>Строка на формальном языке</returns>
            public string TranslateRight(string text)
            {
                int count = 0;
                bool isEnd = false; // true - если ни одно из правил непреминимо
                while (count < MaxRepetitionsCount)
                {
                    if (isEnd) break;

                    count++;
                    isEnd = true;
                    // применяем по очереди каждое правило языка к строке
                    foreach (Rule rule in _rules)
                    {
                        if (!rule.IsLooped)     // если правило зацикливает
                        {
                            string key = rule.Key;
                            string value = rule.Value;

                            int pos = text.LastIndexOf(key);

                            if (pos != -1)  // если ключ найден
                            {
                                // если правило зацикливает перевод - запоминаем это
                                if (CheckLoop(text, rule, isReverse: true)) rule.IsLooped = true;
                                else
                                {
                                    text = text.Remove(pos, key.Length);
                                    text = text.Insert(pos, value);
                                    isEnd = false;

                                    break;
                                }
                            }
                        }
                        else rule.IsLooped = !rule.IsLooped;
                    }
                }

                RefreshRules();
                return text;
            }
            /// <summary>
            /// Переводит строку на формальный язык. 
            /// Если встречается несколько выводов из одного нетерминального символа - берет случайный. 
            /// </summary>
            /// <param name="text">Строка для перевода.</param>
            /// <returns>Строка на формальном языке.</returns>	
            public string TranslateRandom(string text)
            {
                int count = 0;
                bool isEnd = false; // true - если ни одно из правил непреминимо
                while (count < MaxRepetitionsCount)
                {
                    if (isEnd) break;

                    count++;
                    isEnd = true;
                    // правила, которые применимы к текущему состоянию строки
                    List<Rule> checkedRules = new();
                    // применяем по очереди каждое правило языка к строке
                    foreach (Rule rule in _rules)
                    {
                        string key = rule.Key;
                        string value = rule.Value;

                        int pos = text.LastIndexOf(key);

                        if (pos != -1)  // если ключ найден
                        {
                            checkedRules.Add(rule);
                            isEnd = false;
                        }
                    }
                    Random random = new();
                    int index = random.Next(checkedRules.Count);
                    Rule ruleChecked = null;
                    if (checkedRules.Count != 0)
                    {
                        ruleChecked = checkedRules[index];
                    }

                    if (ruleChecked != null)
                    {
                        string k = ruleChecked.Key;
                        string v = ruleChecked.Value;
                        int p = text.LastIndexOf(k);
                        text = text.Remove(p, k.Length);
                        text = text.Insert(p, v);
                    }
                }

                // RefreshRules();
                return text;
            }

            /// <summary>
            /// Лвеосторонний вывод.
            /// </summary>
            /// <returns>Строка, порожденная на основе правил языка.</returns>
            public string OutputLeft()
            {

                string result = "S";
                int count = 0;
                while (count < MaxRepetitionsCount)
                {
                    int pos = -1;

                    // найдем крайний левый нетерминальный символ в цепочке
                    foreach (Rule rule in _rules)
                    {
                        string key = rule.Key;
                        int findPos = result.IndexOf(key);
                        if ((pos > findPos || pos == -1) && findPos != -1)
                        {
                            pos = findPos;
                        }

                    }

                    // если не найдено ниодного подходящего правила - выходим
                    if (pos == -1)
                    {
                        break;
                    }

                    // найдем все правил подходящие для крайнего левого нетерминального символа
                    List<Rule> rules = new();
                    foreach (Rule rule in _rules)
                    {
                        string key = rule.Key;
                        if (pos == result.IndexOf(key))
                        {
                            rules.Add(rule);
                        }
                    }

                    // случайно выберем правило
                    Random random = new();
                    int index = random.Next(rules.Count);
                    Rule r = rules[index];

                    int p = result.IndexOf(r.Key);
                    result = result.Remove(p, r.Key.Length);
                    result = result.Insert(p, r.Value);

                    count++;
                }

                return result;
            }

            /// <summary>
            /// Правосторонний вывод.
            /// </summary>
            /// <returns>Строка, порожденная на основе правил языка.</returns>
            public string OutputRight()
            {

                string result = "S";
                int count = 0;
                while (count < MaxRepetitionsCount)
                {
                    int pos = -1;

                    // найдем крайний правый нетерминальный символ в цепочке
                    foreach (Rule rule in _rules)
                    {
                        string key = rule.Key;
                        int findPos = result.IndexOf(key);
                        if ((pos < findPos || pos == -1) && findPos != -1)
                        {
                            pos = findPos;
                        }

                    }

                    // если не найдено ниодного подходящего правила - выходим
                    if (pos == -1)
                    {
                        break;
                    }

                    // найдем все правил подходящие для крайнего правого нетерминального символа
                    List<Rule> rules = new();
                    foreach (Rule rule in _rules)
                    {
                        string key = rule.Key;
                        if (pos == result.LastIndexOf(key))
                        {
                            rules.Add(rule);
                        }
                    }

                    // случайно выберем правило
                    Random random = new();
                    int index = random.Next(rules.Count);
                    Rule r = rules[index];

                    int p = result.LastIndexOf(r.Key);
                    result = result.Remove(p, r.Key.Length);
                    result = result.Insert(p, r.Value);

                    count++;
                }

                return result;
            }

            private void RefreshRules()
            {
                foreach (Rule rule in _rules)
                {
                    rule.IsLooped = false;
                }
            }
        }
        /// <summary>
        /// Грамматика формального языка
        /// </summary>
        public class Grammar
        {
            /// <summary>
            /// Множество терминальных символов
            /// </summary>
            public List<string> Nonterminal { get; set; }
            /// <summary>
            /// Множество терминальных символов
            /// </summary>
            public List<string> Terminal { get; set; }
            /// <summary>
            /// Множество правил (продукций) грамматики
            /// </summary>
            public List<Rule> P { get; set; }
            /// <summary>
            /// Целевой (начальный) символ грамматики
            /// </summary>
            public string S { get; set; }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="vn">Нетерминальные символы</param>
            /// <param name="vt">Nthvbyfkmyst cbvdjks</param>
            /// <param name="rules">Правила</param>
            /// <param name="s">Начальный символ</param>
            public Grammar(List<string> vn, List<string> vt, List<Rule> rules, string s = "S")
            {
                Nonterminal = vn;
                Terminal = vt;
                P = rules;
                S = s;
            }
            /// <summary>
            /// Возвращает тип грамматики
            /// </summary>
            /// <returns></returns>
            public string GetTypeGrammar()
            {
                bool isTypeOne = true;
                bool isTypeTwo = true;
                bool isTypeThree = true;

                bool isEachTermPosBigger = true;
                bool isEachTermPosSmaller = true;
                foreach (Rule r in P)
                {
                    // проверка проинадлежности первому типу грамматики
                    isTypeOne &= r.Key.Length <= r.Value.Length;

                    // проверка принадлежности второму типу
                    foreach (string vt in Terminal)
                    {
                        isTypeTwo &= !r.Key.Contains(vt);
                    }

                    // 
                    if (isEachTermPosBigger || isEachTermPosSmaller)
                    {
                        List<int> terminlPositions = new();
                        List<int> nonTerminlPositions = new();
                        foreach (string vn in Nonterminal)
                        {
                            int pos = r.Value.IndexOf(vn);
                            if (pos != -1) nonTerminlPositions.Add(pos);
                        }
                        foreach (string vt in Terminal)
                        {
                            int pos = r.Value.IndexOf(vt);
                            if (pos != -1) terminlPositions.Add(pos);
                        }
                        foreach (int pos in terminlPositions)
                        {
                            foreach (int posNonTerm in nonTerminlPositions)
                            {
                                isEachTermPosBigger &= pos > posNonTerm;
                                isEachTermPosSmaller &= pos < posNonTerm;
                            }
                        }
                    }
                }

                if ((isEachTermPosBigger && isEachTermPosSmaller) || (!isEachTermPosBigger && !isEachTermPosSmaller))
                {
                    isTypeThree = false;
                }
                string res = "0";
                if (isTypeOne) res += " 1";
                if (isTypeTwo) res += " 2";
                if (isTypeThree) res += " 3";
                return res;
            }
            /// <summary>
            /// Создает дерево вывода из цепочки символов
            /// </summary>
            /// <param name="text">Строка (цепочка символов), для которой нужно построить дерево</param>
            /// <returns></returns>
            public string MakeTree(string text)
            {
                int maxCount = 10000;
                int count = 0;
                List<string> tree = new();
                tree.Add(text);
                while (count < maxCount)
                {
                    foreach (Rule rule in P)
                    {
                        string key = rule.Key;
                        string value = rule.Value;

                        int pos = text.LastIndexOf(value);
                        if (pos != -1)
                        {
                            text = text.Remove(pos, value.Length);
                            text = text.Insert(pos, key);

                            string separator = "|";
                            for (int i = 0; i < pos; i++)
                            {
                                separator = " " + separator;
                            }
                            tree.Add(separator);
                            tree.Add(text);
                        }
                    }
                    count++;
                }
                tree.Reverse();

                foreach (string branch in tree)
                {
                    Console.WriteLine(branch);
                }
                return text;
            }

        }

        public enum State { H, A, D, B, S, ER };
        public static void Analizator(string text)
        {
            State now = State.H;
            int count = 0;
            string res = "";
            do
            {
                //проверка выхода за пределы индексации
                if (count < text.Length)
                {
                    switch (now)
                    {
                        case State.H:
                            {
                                if (text[count] == '0') now = State.A;
                                else if (text[count] == '1') now = State.S;
                                else now = State.ER;
                                break;

                            }
                        case State.A:
                            {
                                if (text[count] == '0') now = State.A;
                                else if (text[count] == '1') now = State.B;
                                else now = State.ER;
                                break;
                            }
                        case State.B:
                            {
                                if (text[count] == '0') now = State.B;
                                else if (text[count] == '1') now = State.B;
                                else now = State.ER;
                                break;
                            }
                        case State.S:
                            {
                                if (text[count] == '1') now = State.S;
                                else if (text[count] == '0') now = State.A;
                                else now = State.ER;
                                break;
                            }
                        default:
                            break;
                    }

                    res += now;
                    res += " ";
                    //Console.WriteLine(now);
                    count++;
                }
                else break;
            }
            while (now != State.ER && text[count] != '\u00A7'); //now != State.S &&
            Console.WriteLine(res);
        }


        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;


            Console.WriteLine();
            Console.WriteLine("Lab 2.");

            List<Rule> dict = new()
            {
                new Rule("S", "0A"),
                new Rule("S", "1B"),
                new Rule("A", "0A"),
                new Rule("A", "1B"),
                new Rule("B", "0B"),
                new Rule("B", "1B"),
                new Rule("B", "\u00A7"),

            };
            PrintRules(dict);

            FormalLanguage fl = new(dict);
            // Два варианта вывода, разные результаты
            //Console.WriteLine(fl.OutputLeft());
            Console.WriteLine("Цепочка: " + fl.TranslateRight("S"));
            Console.WriteLine("Язык: L = { 0^n 1^m \u00A7| n,m >0 }");
            Console.WriteLine();

            Console.WriteLine("Анализатор:");
            Analizator("110001111\u00A7");
            /*
			 * у нетерминалов A и B есть одинаковые правые части 1B
			 * Следовательно такой грамматике соотв. недетерминированный конечный автомат(НКА)
			 * Можно предложить алгоритм, который будет перебирать все варианты "сверток",
			 * если цепочка принадлежит языку, то будет найден путь, но если все варианты будут просмотрены
			 * и каждый будет завершаться неудачей, то цепочка языку не принадлежит.
			 * 
			 * Такой алгоритм неприемлим
			 */
        }
    }
}