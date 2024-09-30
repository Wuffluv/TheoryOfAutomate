	#pragma once
	#include <iostream>
	#include <unordered_set>
	#include <string>
	#include <list>
	#include <random>
	#include "Rule.h"
	#include <functional>

	using namespace std;

	/// Класс формального языка с леволинейной грамматикой и проверкой на зацикливание
	class FormalLanguage
	{
	private:
		/// Правила языка
		list <Rule> _rules;
		/// Максимально количество повторений
		int MaxRepetitionsCount;

		/// Проверяет правило на зацикливание
		/// "input" - Строка, к которой применяется правило
		/// "rule" - Правило языка
		/// "count" - Количество допустимых повторений

		bool CheckLoop(string input, Rule rule, int count = 5) {
			for (int i = 0; i < count; i++) {
				string key = rule.getKey();
				string value = rule.getValue();

				size_t pos = input.find(key);

				if (pos != -1) // вместо -1 = npos
				{
					input = input.erase(pos, key.length());
					input = input.insert(pos, value);
				}
				else
				{
					return false;
				}
			}
			return true;
		}

		void RefreshRules() {
			for (Rule& rule : _rules) {
				rule.setIsLooped(false);
			}
		}
	public:
		/// Конструктор
		FormalLanguage(list<Rule> rules, int count = 10000)
		{
			_rules = rules;
			MaxRepetitionsCount = count;
		}

		/// Сеттер
		void setRules(list <Rule> rule)
		{
			_rules = rule;
		}

		/// Геттер
		list<Rule> getRules() const
		{
			return _rules;
		}

		/// Сеттер
		void setMaxRepetitionsCount(int repeat)
		{
			MaxRepetitionsCount = repeat;
		}

		/// Геттер
		int getMaxRepetitionsCount()
		{
			return MaxRepetitionsCount;
		}

		/// Левостороний вывод.
		/// Строка, порожденная на основе правил языка.
		string OutputLeft()
		{
			string result = "S";
			int count = 0;
			while (count < MaxRepetitionsCount)
			{
				size_t pos = -1;

				// найдем крайний левый нетерминальный символ в цепочке
				for (const Rule& rule : _rules)
				{
					string key = rule.getKey();
					size_t findPos = result.find(key);
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
				list<Rule> rules = {};
				for (const Rule& rule : _rules)
				{
					string key = rule.getKey();
					if (pos == result.find(key))
					{
						rules.push_back(rule);
					}
				}
				// случайно выберем правило
				random_device random;
				mt19937 gen(random());
				uniform_int_distribution<int> dist(0, rules.size() - 1);
				int index = dist(gen);
				auto it = rules.begin();
				advance(it, index);
				Rule r = *it;
				size_t p = result.find(r.getKey());
				result = result.erase(p, r.getKey().length());
				result = result.insert(p, r.getValue());
				count++;
			}
			return result;
		}
		bool CompareGrammars(const FormalLanguage& grammar1, const FormalLanguage& grammar2)
		{
			list<Rule> rules1 = grammar1.getRules();
			list<Rule> rules2 = grammar2.getRules();
			// Проверяем количество правил в грамматиках
			if (rules1.size() != rules2.size()) {
				return false;
			}
			// Проверяем каждое правило в грамматиках
			for (const Rule& rule1 : rules1) {
				bool found = false;
				for (const Rule& rule2 : rules2) {
					if (rule1.getKey() == rule2.getKey() && rule1.getValue() == rule2.getValue()) {
						found = true;
						break;
					}
				}
				if (!found) {
					return false;
				}
			}
			return true;
		}
		/// Переводит строку на формальный язык
		/// "text" - Строка для перевода
		string Translate(string text)
		{
			int count = 0;
			bool isEnd = false;	// true - если ни одно из правил непреминимо
			while (count < MaxRepetitionsCount)
			{
				if (isEnd) break;
				count++;
				isEnd = true;
				// применяем по очереди каждое правило языка к строке
				for (Rule& rule : _rules)
				{
					if (!rule.getIsLooped())		// если правило зацикливает
					{
						string key = rule.getKey();
						string value = rule.getValue();
						int pos = text.find(key);
						if (pos != -1)	// если ключ найден
						{
							// если правило зацикливает перевод - запоминаем это
							if (CheckLoop(text, rule)) rule.setIsLooped(true);
							else
							{
								cout << text << endl;
								text = text.erase(pos, key.length());
								text = text.insert(pos, value);
								isEnd = false;
								break;
							}
						}
					}
					else rule.setIsLooped(!rule.getIsLooped());
				}
			}
			RefreshRules();
			return text;
		}

		void RemoveUnreachableStates(FormalLanguage& language)
		{
			// Получаем начальное состояние
			string initialState = "A";

			// Создаем множество для хранения достижимых состояний
			unordered_set<string> reachableStates;

			// Рекурсивная функция для обхода состояний
			function<void(const string&)> TraverseStates = [&](const string& currentState)
				{
					// Добавляем текущее состояние в множество достижимых состояний
					reachableStates.insert(currentState);

					// Ищем все переходы и рекурсивно обходим состояния, в которые можно перейти
					for (const Rule& rule : language.getRules())
					{
						if (rule.getKey() == currentState)
						{
							string nextState = rule.getValue();
							if (reachableStates.count(nextState) == 0)
							{
								TraverseStates(nextState);
							}
						}
					}
				};

			// Начинаем обход из начального состояния
			TraverseStates(initialState);

			// Создаем новый список правил, содержащий только достижимые состояния и связанные с ними переходы
			list<Rule> newRules;
			for (const Rule& rule : language.getRules())
			{
				if (reachableStates.count(rule.getKey()) != 0)
				{
					newRules.push_back(rule);
				}
			}

			// Обновляем правила языка с новым списком правил
			language.setRules(newRules);
		}

	};

