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
		public class Rule {
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

			public Rule(string k, string v, bool l = false) {
				Key = k;
				Value = v;
				IsLooped = l;
			}
		}


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
		public class FormalLanguage {

			/// <summary>
			/// Правила языка
			/// </summary>
			private List<Rule> _rules { get; set; }
			/// <summary>
			/// Максимально количество повторений
			/// </summary>
			public uint MaxRepetitionsCount { get; set; }

			public FormalLanguage(List<Rule> rules, uint count = 10000) {
				_rules = rules;
				MaxRepetitionsCount = count;
			}

			/// <summary>
			/// Проверяет правило на зацикливание
			/// </summary>
			/// <param name="input">Строка, к которой применяется правило</param>
			/// <param name="rule">Правило языка</param>
			/// <param name="count">Количество допустимых повторений</param>
			/// <returns>true - если правило зацикливает перевод, иначе - false</returns>
			private bool CheckLoop(string input, Rule rule, int count = 5) {
				for (int i = 0; i < count; i++) {
					string key = rule.Key;
					string value = rule.Value;

					int pos = input.IndexOf(key);

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
			/// Переводит строку на формальный язык
			/// </summary>
			/// <param name="text">Строка для перевода</param>
			/// <returns>Строка на формальном языке</returns>
			public string Translate(string text)
			{
				int count = 0;
				bool isEnd = false;	// true - если ни одно из правил непреминимо
				while (count < MaxRepetitionsCount)
				{
					if (isEnd) break;

					count++;
					isEnd = true;
					// применяем по очереди каждое правило языка к строке
					foreach (Rule rule in _rules)
					{
						if (!rule.IsLooped)		// если правило зацикливает
						{
							string key = rule.Key;
							string value = rule.Value;

							int pos = text.IndexOf(key);

							if (pos != -1)	// если ключ найден
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

			private void RefreshRules() {
				foreach (Rule rule in _rules) {
					rule.IsLooped = false;
				}
			}
		}
		/// <summary>
		/// Грамматика формального языка
		/// </summary>
		public class Grammar {
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
			public Grammar(List<string> vn, List<string> vt, List<Rule> rules, string s = "S") {
				Nonterminal = vn;
				Terminal = vt;
				P = rules;
				S = s;
			}
			/// <summary>
			/// Возвращает тип грамматики
			/// </summary>
			/// <returns></returns>
			public string GetTypeGrammar() {
				bool isTypeOne = true;
				bool isTypeTwo = true;
				bool isTypeThree = true;

				bool isEachTermPosBigger = true;
				bool isEachTermPosSmaller = true;
				foreach (Rule r in P) {
					// проверка проинадлежности первому типу грамматики
					isTypeOne &= r.Key.Length <= r.Value.Length;

					// проверка принадлежности второму типу
					foreach (string vt in Terminal) {
						isTypeTwo &= !r.Key.Contains(vt);
					}
					
					if (isEachTermPosBigger || isEachTermPosSmaller)
					{
						List<int> terminlPositions = new();
						List<int> nonTerminlPositions = new();
						foreach (string vn in Nonterminal)
						{						
							int TEMP = r.Value.IndexOf(vn);
							if(TEMP != -1) { nonTerminlPositions.Add(TEMP); }
						}
						foreach (string vt in Terminal)
						{
							int TEMP = r.Value.IndexOf(vt);
							if (TEMP != -1) { terminlPositions.Add(TEMP); }	
						}
						foreach (int pos in terminlPositions)
						{
							foreach (int posNonTerm in nonTerminlPositions)
							{
								isEachTermPosBigger &= pos > posNonTerm;
								isEachTermPosSmaller &= pos < posNonTerm;
							}
						}
						if((isEachTermPosBigger == false)&&(isEachTermPosSmaller==false))
                        {
							isTypeThree = false;
                        }
					}
				}
				Console.WriteLine("Относится к типам по Хомскому:");
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
	
		static void Main(string[] args)
		{
			Console.OutputEncoding = Encoding.Unicode;

			Console.WriteLine("Первая лабораторная работа.");
			Console.WriteLine("Задание 2.");
			Console.WriteLine("Подпункт a)");

			List<Rule> dict = new()
			{
				new Rule("S", "aaCFD"),
				new Rule("AD", "D"),
				new Rule("F", "AFB"),
				new Rule("F", "AB"),
				new Rule("Cb", "bC"),
				new Rule("AB", "bBA"),
				new Rule("CB", "C"),
				new Rule("Ab", "bA"),
				new Rule("bCD", "\u03B5"),  // epsilon
			};
			PrintRules(dict);

			FormalLanguage fl = new(dict);
			Console.WriteLine("Цепочка: " + fl.Translate("S"));
			Console.WriteLine("Язык: L = { a^n | n > 0 }");
			Console.WriteLine();


			Console.WriteLine("Подпункт б)");
			dict = new()
			{
				new Rule("S", "A\u00A7"),   // \u27C2 - значок перпендикуляра
				new Rule("S", "B\u00A7"),
				new Rule("A", "a"),
				new Rule("A", "Ba"),
				new Rule("b", "b"),
				new Rule("b", "Bb"),
				new Rule("b", "Ab"),
			};
			PrintRules(dict);

			fl = new(dict);
			Console.WriteLine("Цепочка: " + fl.Translate("S"));
			Console.WriteLine("Язык: L = { (a\u00A7)^n | n>0 }");
			Console.WriteLine();




			Console.WriteLine("");
			Console.WriteLine("Задание 3.");
			Console.WriteLine("Подпункт a)");
			Console.WriteLine("Язык: L = { a^n b^m c^k | n, m, k > 0}");
			Console.WriteLine("Грамматика: G: ({a, b, c}, {A, B, C}, P, S)");
			dict = new()
			{
				new Rule("S", "aaB"),
				new Rule("B", "bCCCC"),
				new Rule("B", "b"),
				new Rule("C", "Cc"),
				new Rule("C", "c"),
			};
			PrintRules(dict);

			fl = new(dict);
			Console.WriteLine("Цепочка: " + fl.Translate("S"));
			Console.WriteLine();


			Console.WriteLine("Подпункт б)");
			Console.WriteLine("Язык: L = {0^n(10)^m | n, m ≥ 0}");
			Console.WriteLine("Грамматика: G: ({0, 10}, {A, B}, P, S)");
			dict = new()
			{
				new Rule("S", "0AB"),
				new Rule("A", "000"),
				new Rule("B", "1010"),
			};
			PrintRules(dict);

			fl = new(dict);
			Console.WriteLine("Цепочка: " + fl.Translate("S"));
			Console.WriteLine();


			Console.WriteLine("Подпункт в)");
			Console.WriteLine("Язык: L = {a1 a2 … an an … a2a1 | ai E {0, 1}}");
			Console.WriteLine("Грамматика: G: ({0, 1}, {A, B}, P, S)");
			dict = new()
			{
				new Rule("S", "AB"),
				new Rule("A", "1001010"),
				new Rule("B", "0101001"),
			};
			PrintRules(dict);

			fl = new(dict);
			Console.WriteLine("Цепочка: " + fl.Translate("S"));
			Console.WriteLine();

			Console.WriteLine("");



			Console.WriteLine("Задание 4.");
			Console.WriteLine("Подпункт a)");
			dict = new()
			{
				new Rule("S", "0A1"),
				new Rule("S", "01"),
				new Rule("0A", "00A1"),
				new Rule("A", "01"),
			};
			PrintRules(dict);

			Grammar gr = new(
				new List<string> { "S", "A" },
				new List<string> { "0", "1" },
				dict);
			Console.WriteLine("Грамматика: G: ({0, 1}, {S, A}, P, S)");
			Console.WriteLine(gr.GetTypeGrammar());
			Console.WriteLine();


			Console.WriteLine("Подпункт б)");
			dict = new()
			{
				new Rule("S", "Ab"),
				new Rule("A", "Aa"),
				new Rule("A", "ba"),
			};
			PrintRules(dict);

			gr = new(
				new List<string> { "S", "A" },
				new List<string> { "a", "b" },
				dict);
			Console.WriteLine("Грамматика: G: ({a, b}, {S, A}, P, S)");
			Console.WriteLine(gr.GetTypeGrammar());
			Console.WriteLine();


			Console.WriteLine("Задание 5");			
			dict = new()
			{
				new Rule("S", "aSL"),
				new Rule("S", "aL"),
				new Rule("L", "Kc"),
				new Rule("cK", "Kc"),
				new Rule("K", "b"),
			};
			PrintRules(dict);

			fl = new(dict);
			Console.WriteLine("Цепочка: " + fl.Translate("S"));
			Console.WriteLine("Язык: L = {a^n b^m c^k | a, b, k > 0}");
			Console.WriteLine();

			dict = new()
			{
				new Rule("S", "aSBc"),
				new Rule("S", "abc"),
				new Rule("cB", "Bc"),
				new Rule("bB", "bb"),
			};
			PrintRules(dict);

			fl = new(dict);
			Console.WriteLine("Цепочка: " + fl.Translate("S"));
			Console.WriteLine("Язык: L = {a^n b^m c^k | a, b, k > 0}");

			Console.WriteLine("Грамматики эквиваленты т.к. они определяют один и тот же язык");
			Console.WriteLine();


			Console.WriteLine("Задание 6.");
			dict = new()
			{
				new Rule("S", "AB"),
				new Rule("S", "ABS"),
				new Rule("AB", "BA"),
				new Rule("BA", "AB"),
				new Rule("A", "a"),
				new Rule("B", "b"),
			};
			PrintRules(dict);
			
			fl = new(dict);
			Console.WriteLine("Цепочка: " + fl.Translate("S"));
			Console.WriteLine();

			dict = new()
			{
				new Rule("S", "ab"),
			};
			PrintRules(dict);

			fl = new(dict);
			Console.WriteLine("Цепочка: " + fl.Translate("S"));
			Console.WriteLine();


			Console.WriteLine("Задание 7.");
			dict = new()
			{
				new Rule("S", "A.A"),
				new Rule("A", "B"),
				new Rule("A", "BA"),
				new Rule("B", "0"),
				new Rule("B", "1"),
			};
			PrintRules(dict);

			fl = new(dict);
			Console.WriteLine("Цепочка: " + fl.Translate("S"));
			Console.WriteLine();

			dict = new()
			{
				new Rule("S", "A.0"),
				new Rule("A", "0"),
				new Rule("A", "1"),
			};
			PrintRules(dict);
			fl = new(dict);
			Console.WriteLine("Цепочка: " + fl.Translate("S"));
			Console.WriteLine();


			Console.WriteLine("Задание 8.");
			Console.WriteLine("Подпункт a)");
			dict = new()
			{
				new Rule("S", "while (condition) { expression }"),
				new Rule("condition", "true"),
				new Rule("condition", "false"),
				new Rule("expression", "i = i + 1;"),
			};
			PrintRules(dict);

			fl = new(dict);
			Console.WriteLine(fl.Translate("S"));
			Console.WriteLine();

			Console.WriteLine("Подпункт б)");
			dict = new()
			{
				new Rule("S", "for(variableint;condition;changingvar){ circlebody }"),
				new Rule("variableint", "int i"),
				new Rule("condition", "i < 10"),
				new Rule("changingvar", "i++"),
				new Rule("circlebody", "printf(\"%d\", i);"),
			};
			PrintRules(dict);

			fl = new(dict);
			Console.WriteLine(fl.Translate("S"));
			Console.WriteLine();

			Console.WriteLine("Подпункт в)");
			dict = new()
			{
				new Rule("S", "do{circlebody} while (condition);"),
				new Rule("circlebody", "printf(\"%d\", i);"),
				new Rule("condition", "i < 10"),
			};
			PrintRules(dict);
			fl = new(dict);
			Console.WriteLine(fl.Translate("S"));
			Console.WriteLine();


			
			Console.WriteLine("Задание 9.");
			//new Rule("bCD", "\u03B5"),  // epsilon
			//a\u03B5b\u03B5a\u03B5b\u03B5
			dict = new()
			{
				new Rule("S", "aSbS"),
				new Rule("S", "bSaS"),
				new Rule("S", "\u03B5"),
			};
			gr = new(
				new List<string> { "S" },
				new List<string> { "a", "b", "\u03B5" },
				dict);
			// aEbaEbE
			Console.WriteLine(gr.MakeTree("a\u03B5ba\u03B5b\u03B5"));
			Console.WriteLine();

			Console.WriteLine("Задание 10.");
			Console.WriteLine("Подпункт a)");
			Regex IdentificatorRegex = new(@"^([a-z]|[A-Z]|_){1}([a-z]||[A-Z])*");

			Console.WriteLine(IdentificatorRegex.IsMatch("_ashfgdf65h5h3hhsvb"));
			Console.WriteLine(IdentificatorRegex.IsMatch("!_wqeqwrwfsdfgsdfg4535gasd"));
			Console.WriteLine(IdentificatorRegex.IsMatch("#34qwezxc"));
			Console.WriteLine(IdentificatorRegex.IsMatch("A_qwezxc324"));
			Console.WriteLine();

			Console.WriteLine("Подпункт b)");
			Regex RealConstantRegex = new(@"^[0-9]+(\.((e|E\+?\-?[0-9](f?|F?|l?|L?))|[0-9]*))*");
			Console.WriteLine(RealConstantRegex.IsMatch("Q_qwezxc"));
			Console.WriteLine(RealConstantRegex.IsMatch("0.123l"));
			Console.WriteLine(RealConstantRegex.IsMatch("123.123"));
			Console.WriteLine(RealConstantRegex.IsMatch("10.e10"));
			Console.WriteLine(RealConstantRegex.IsMatch("32.01-e10"));
			Console.WriteLine();

			Console.WriteLine("Задание 11.");
			Console.WriteLine("Подпункт а)");
			Console.WriteLine("Грамматика описывает язык 0^n 1^n 'Символ перепендикуляра' \u27C2");
			dict = new()
			{
				new Rule("S", "0S"),
				new Rule("S", "0B"),
				new Rule("B", "1B"),
				new Rule("B", "1C"),
				new Rule("C", "1C"),
				new Rule("C", "\u27C2"),

			};
			fl = new(dict);
			Console.WriteLine(fl.OutputLeft());

			dict = new()
			{
				new Rule("S", "A\u27C2"),
				new Rule("A", "A1"),
				new Rule("A", "CB1"),
				new Rule("B", "B1"),
				new Rule("B", "C1"),
				new Rule("B", "CB1"),
				new Rule("C", "0"),

			};
			fl = new(dict);
			Console.WriteLine(fl.OutputLeft());
			Console.WriteLine("Подпункт б)");
			Console.WriteLine("Грамматика описывает язык {a^n b^n} 'Символ перепендикуляра' \u27C2");
			dict = new()
			{
				new Rule("S", "aA"),
				new Rule("S", "aB"),
				new Rule("S", "bA"),
				new Rule("A", "bS"),
				new Rule("B", "aS"),
				new Rule("B", "bB"),
				new Rule("B", "\u27C2"),

			};
			fl = new(dict);
			Console.WriteLine(fl.OutputLeft());

			dict = new()
			{
				new Rule("S", "A\u27C2"),
				new Rule("A", "Ba"),
				new Rule("A", "Bb"),
				new Rule("A", "Ab"),
				new Rule("A", "ABa"),
				new Rule("A", "ABb"),
				new Rule("B", "a"),
				new Rule("B", "b"),

			};
			fl = new(dict);
			Console.WriteLine(fl.OutputLeft());
			Console.WriteLine();
			
			Console.WriteLine("Задание 12.");
			dict = new()
			{
				new Rule("S", "S1"),
				new Rule("S", "A0"),
				new Rule("A", "A1"),
				new Rule("A", "0"),
			};
			fl = new(dict);
			Console.WriteLine(fl.OutputLeft());
			dict = new()
			{
				new Rule("S", "A1"),
				new Rule("S", "B0"),
				new Rule("S", "E1"),
				new Rule("A", "S1"),
				new Rule("B", "C1"),
				new Rule("B", "D1"),
				new Rule("C", "0"),
				new Rule("D", "B1"),
				new Rule("E", "E0"),
				new Rule("E", "1"),
			};
			fl = new(dict);
			Console.WriteLine(fl.OutputLeft());
			dict = new()
			{
				new Rule("S", "S1"),
				new Rule("S", "A0"),
				new Rule("S", "A1"),
				new Rule("A", "0"),
			};
			fl = new(dict);
			Console.WriteLine(fl.OutputLeft());
		}
	}
}
