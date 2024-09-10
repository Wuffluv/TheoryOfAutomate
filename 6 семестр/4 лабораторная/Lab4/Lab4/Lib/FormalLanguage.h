#pragma once
#include <iostream>
#include <string>
#include <list>
#include <random>
#include "Rule.h"
#include <set>
#include <unordered_set>
#include <stack>
#include <unordered_map>

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

	// Метод для получения всех нетерминальных символов, которые могут быть достигнуты из данного символа
	set<string> GetReachableSymbols(const string& symbol) const
	{
		set<string> reachableSymbols;
		reachableSymbols.insert(symbol);

		bool symbolsAdded = true;
		while (symbolsAdded)
		{
			symbolsAdded = false;
			for (const Rule& rule : _rules)
			{
				if (reachableSymbols.count(rule.getKey()) > 0 && reachableSymbols.count(rule.getValue()) == 0)
				{
					reachableSymbols.insert(rule.getValue());
					symbolsAdded = true;
				}
			}
		}

		return reachableSymbols;
	}

	// Метод для удаления недостижимых правил языка
	// Метод удаления недостижимых состояний
	void RemoveUnreachableRules(char startSymbol) {
		std::unordered_set<char> reachableSymbols;
		std::stack<char> stack;

		reachableSymbols.insert(startSymbol);
		stack.push(startSymbol);

		while (!stack.empty()) {
			char currentSymbol = stack.top();
			stack.pop();

			for (const auto& rule : _rules) {
				if (rule.getKey()[0] == currentSymbol) {
					for (char c : rule.getValue()) {
						if (reachableSymbols.find(c) == reachableSymbols.end()) {
							reachableSymbols.insert(c);
							stack.push(c);
						}
					}
				}
			}
		}

		_rules.erase(std::remove_if(_rules.begin(), _rules.end(), [&](const Rule& rule) {
			return reachableSymbols.find(rule.getKey()[0]) == reachableSymbols.end();
			}), _rules.end());
	}
	
	void RemoveEquivalentStates() {
		// Структура для хранения правила с выделенным символом ключа
		struct KeyedRule {
			std::string key; // Символ ключа
			Rule rule; // Правило
		};

		std::list<KeyedRule> uniqueRules; // Новый список для неповторяющихся правил с выделенным символом ключа

		// Функция для проверки эквивалентности значений правил с выделением символа ключа
		auto isEquivalent = [&](const Rule& rule1, const Rule& rule2, const std::string& key) {
			std::string value1 = rule1.getValue();
			std::string value2 = rule2.getValue();

			// Поиск символа ключа в правилах
			size_t keyPos1 = value1.find(key);
			size_t keyPos2 = value2.find(key);

			if (keyPos1 != std::string::npos && keyPos2 != std::string::npos) {
				// Если символ ключа присутствует в обоих правилах, считаем их эквивалентными
				return value1 == value2;
			}
			else if (keyPos1 != std::string::npos || keyPos2 != std::string::npos) {
				// Если символ ключа присутствует только в одном из правил, считаем их неэквивалентными
				return false;
			}
			else {
				// Если символ ключа отсутствует в обоих правилах, не принимаем их во внимание при проверке
				return true;
			}
			};

		// Поиск символа ключа в правилах
		std::string key;

		// Проверка и добавление неповторяющихся правил с выделенным символом ключа в новый список
		for (const auto& rule : _rules) {
			bool isDuplicate = false;
			for (const auto& uniqueRule : uniqueRules) {
				if (isEquivalent(rule, uniqueRule.rule, uniqueRule.key)) {
					isDuplicate = true;
					break;
				}
			}
			if (!isDuplicate) {
				key = rule.getValue();
				uniqueRules.push_back({ key, rule });
			}
		}

		// Удаляем правила, содержащие символ ключа в значении
		uniqueRules.remove_if([&](const KeyedRule& keyedRule) {
			return keyedRule.rule.getValue().find(key) != std::string::npos;
			});

		// Заменяем исходный список правил на новый список без эквивалентных состояний
		_rules.clear();
		for (const auto& keyedRule : uniqueRules) {
			_rules.push_back(keyedRule.rule);
		}
	}


	void PrintRules()
	{
		for (const Rule& rule : _rules)
		{
			cout << rule.getKey() << " -> " << rule.getValue() << endl;
		}
	}
	//void buildGraph(const std::list<Rule>& rules) {
	//	std::map<std::string, std::list<std::string>> graph;

	//	// Добавление вершин и ребер на основе правил
	//	for (const auto& rule : rules) {
	//		const std::string& source = rule.getKey();
	//		const std::string& target = rule.getValue();

	//		graph[source].push_back(target);
	//	}

	//	// Сохранение графа в файл GraphML
	//	std::ofstream outputFile("graph.graphml");
	//	if (!outputFile) {
	//		std::cout << "Failed to open output file." << std::endl;
	//		return;
	//	}

	//	outputFile << "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" << std::endl;
	//	outputFile << "<graphml xmlns=\"http://graphml.graphdrawing.org/xmlns\"" << std::endl;
	//	outputFile << "         xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"" << std::endl;
	//	outputFile << "         xsi:schemaLocation=\"http://graphml.graphdrawing.org/xmlns" << std::endl;
	//	outputFile << "                             http://graphml.graphdrawing.org/xmlns/1.0/graphml.xsd\">" << std::endl;
	//	outputFile << "  <graph id=\"G\" edgedefault=\"directed\">" << std::endl;

	//	// Добавление вершин графа
	//	for (const auto& node : graph) {
	//		outputFile << "    <node id=\"" << node.first << "\" />" << std::endl;
	//	}

	//	// Добавление ребер графа
	//	for (const auto& edge : graph) {
	//		const std::string& source = edge.first;
	//		const std::list<std::string>& targets = edge.second;

	//		for (const auto& target : targets) {
	//			outputFile << "    <edge source=\"" << source << "\" target=\"" << target << "\" />" << std::endl;
	//		}
	//	}

	//	outputFile << "  </graph>" << std::endl;
	//	outputFile << "</graphml>" << std::endl;

	//	outputFile.close();
	//}

	//void buildGraph() {
	//	std::map<std::string, std::list<std::pair<char, std::string>>> graph;

	//	//// Добавление вершин и ребер на основе правил
	//	//for (const auto& rule : _rules) {
	//	//	const std::string& source = rule.getKey();
	//	//	const std::string& target = rule.getValue();

	//	//	char weight = target[0];
	//	//	std::string targetVertex = target.substr(1);

	//	//	graph[source].push_back(std::make_pair(weight, targetVertex));
	//	//}
	//	  // Добавление вершин и ребер на основе правил
	//	for (const auto& rule : _rules) {
	//		const std::string& source = rule.getKey();
	//		const std::string& target = rule.getValue();

	//		char weight = target[0];
	//		std::string targetVertex = target.substr(1);

	//		// Проверка, соответствует ли второй символ значению ключу
	//		if (targetVertex != source) {
	//			// Добавление вершины
	//			graph[targetVertex]; // Добавляем пустую вершину

	//			// Добавление ребра
	//			graph[source].push_back(std::make_pair(weight, targetVertex));
	//		}
	//	}

	//	// Сохранение графа в файл GraphML
	//	std::ofstream outputFile("graph3.graphml");
	//	if (!outputFile) {
	//		std::cout << "Failed to open output file." << std::endl;
	//		return;
	//	}

	//	outputFile << "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" << std::endl;
	//	outputFile << "<graphml xmlns=\"http://graphml.graphdrawing.org/xmlns\"" << std::endl;
	//	outputFile << "         xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"" << std::endl;
	//	outputFile << "         xsi:schemaLocation=\"http://graphml.graphdrawing.org/xmlns" << std::endl;
	//	outputFile << "                             http://graphml.graphdrawing.org/xmlns/1.0/graphml.xsd\">" << std::endl;
	//	outputFile << "<key id=\"weight\" for=\"edge\" attr.name=\"weight\" attr.type=\"double\" />" << std::endl;
	//	outputFile << "  <graph id=\"G\" edgedefault=\"directed\">" << std::endl;

	//	// Добавление вершин графа
	//	for (const auto& node : graph) {
	//		outputFile << "    <node id=\"" << node.first << "\" />" << std::endl;
	//	}

	//	// Добавление ребер графа
	//	for (const auto& edge : graph) {
	//		const std::string& source = edge.first;
	//		const std::list<std::pair<char, std::string>>& targets = edge.second;

	//		for (const auto& target : targets) {
	//			char weight = target.first;
	//			const std::string& targetVertex = target.second;

	//			outputFile << "    <edge source=\"" << source << "\" target=\"" << targetVertex << "\">" << std::endl;
	//			outputFile << "      <data key=\"weight\">" << weight << "</data>" << std::endl;
	//			outputFile << "    </edge>" << std::endl;
	//		}
	//	}

	//	outputFile << "  </graph>" << std::endl;
	//	outputFile << "</graphml>" << std::endl;

	//	outputFile.close();
	//}

void buildGraph() {
	std::map<std::string, std::list<std::pair<char, std::string>>> graph;
	std::map<char, int> weightMap; // Хранение порядковых чисел весов

	// Добавление вершин и ребер на основе правил
	for (const auto& rule : _rules) {
		const std::string& source = rule.getKey();
		const std::string& target = rule.getValue();

		char weight = target[0];
		std::string targetVertex = target.substr(1);

		// Проверка, соответствует ли второй символ значению ключу
		if (targetVertex != source) {
			// Добавление вершины
			graph[targetVertex]; // Добавляем пустую вершину

			// Добавление ребра
			if (weightMap.find(weight) == weightMap.end()) {
				// Если веса не было ранее, генерируем новое порядковое число
				weightMap[weight] = weightMap.size() + 1;
			}

			int numericWeight = weightMap[weight];
			graph[source].push_back(std::make_pair(numericWeight, targetVertex));
		}
	}

	// Сохранение графа в файл GraphML
	std::ofstream outputFile("graph8.graphml");
	if (!outputFile) {
		std::cout << "Failed to open output file." << std::endl;
		return;
	}

	outputFile << "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" << std::endl;
	outputFile << "<graphml xmlns=\"http://graphml.graphdrawing.org/xmlns\"" << std::endl;
	outputFile << "         xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"" << std::endl;
	outputFile << "         xsi:schemaLocation=\"http://graphml.graphdrawing.org/xmlns" << std::endl;
	outputFile << "                             http://graphml.graphdrawing.org/xmlns/1.0/graphml.xsd\">" << std::endl;
	outputFile << "<key id=\"weight\" for=\"edge\" attr.name=\"weight\" attr.type=\"double\" />" << std::endl;
	outputFile << "  <graph id=\"G\" edgedefault=\"directed\">" << std::endl;

	// Добавление вершин графа
	for (const auto& node : graph) {
		outputFile << "    <node id=\"" << node.first << "\" />" << std::endl;
	}

	// Добавление ребер графа
	for (const auto& edge : graph) {
		const std::string& source = edge.first;
		const std::list<std::pair<char, std::string>>& targets = edge.second;

		for (const auto& target : targets) {
			int weight = target.first;
			const std::string& targetVertex = target.second;

			outputFile << "    <edge source=\"" << source << "\" target=\"" << targetVertex << "\">" << std::endl;
			outputFile << "      <data key=\"weight\">" << weight << "</data>" << std::endl;
			outputFile << "    </edge>" << std::endl;
		}
	}

	outputFile << "  </graph>" << std::endl;
	outputFile << "</graphml>" << std::endl;

	outputFile.close();
}
	
};

//void PrintRules(const list<Rule>& rules)
//{
//	for (const Rule& rule : rules)
//	{
//		cout << rule.getKey() << " -> " << rule.getValue() << endl;
//	}
//}
