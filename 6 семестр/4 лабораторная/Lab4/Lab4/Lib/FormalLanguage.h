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

/// ����� ����������� ����� � ������������ ����������� � ��������� �� ������������
class FormalLanguage
{
private:
	/// ������� �����
	list <Rule> _rules;
	/// ����������� ���������� ����������
	int MaxRepetitionsCount;

	/// ��������� ������� �� ������������
	/// "input" - ������, � ������� ����������� �������
	/// "rule" - ������� �����
	/// "count" - ���������� ���������� ����������

	bool CheckLoop(string input, Rule rule, int count = 5) {
		for (int i = 0; i < count; i++) {
			string key = rule.getKey();
			string value = rule.getValue();

			size_t pos = input.find(key);

			if (pos != -1) // ������ -1 = npos
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
	/// �����������
	FormalLanguage(list<Rule> rules, int count = 10000)
	{
		_rules = rules;
		MaxRepetitionsCount = count;
	}

	/// ������
	void setRules(list <Rule> rule)
	{
		_rules = rule;
	}

	/// ������
	list<Rule> getRules() const
	{
		return _rules;
	}

	/// ������
	void setMaxRepetitionsCount(int repeat)
	{
		MaxRepetitionsCount = repeat;
	}

	/// ������
	int getMaxRepetitionsCount()
	{
		return MaxRepetitionsCount;
	}

	/// ������������ �����.
	/// ������, ����������� �� ������ ������ �����.
	string OutputLeft()
	{
		string result = "S";
		int count = 0;
		while (count < MaxRepetitionsCount)
		{
			size_t pos = -1;

			// ������ ������� ����� �������������� ������ � �������
			for (const Rule& rule : _rules)
			{
				string key = rule.getKey();
				size_t findPos = result.find(key);
				if ((pos > findPos || pos == -1) && findPos != -1)
				{
					pos = findPos;
				}

			}

			// ���� �� ������� �������� ����������� ������� - �������
			if (pos == -1)
			{
				break;
			}

			// ������ ��� ������ ���������� ��� �������� ������ ��������������� �������
			list<Rule> rules = {};
			for (const Rule& rule : _rules)
			{
				string key = rule.getKey();
				if (pos == result.find(key))
				{
					rules.push_back(rule);
				}
			}

			// �������� ������� �������
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

		// ��������� ���������� ������ � �����������
		if (rules1.size() != rules2.size()) {
			return false;
		}

		// ��������� ������ ������� � �����������
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
	/// ��������� ������ �� ���������� ����
	/// "text" - ������ ��� ��������
	string Translate(string text)
	{
		int count = 0;
		bool isEnd = false;	// true - ���� �� ���� �� ������ �����������
		while (count < MaxRepetitionsCount)
		{
			if (isEnd) break;

			count++;
			isEnd = true;
			// ��������� �� ������� ������ ������� ����� � ������
			for (Rule& rule : _rules)
			{
				if (!rule.getIsLooped())		// ���� ������� �����������
				{
					string key = rule.getKey();
					string value = rule.getValue();

					int pos = text.find(key);

					if (pos != -1)	// ���� ���� ������
					{
						// ���� ������� ����������� ������� - ���������� ���
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

	// ����� ��� ��������� ���� �������������� ��������, ������� ����� ���� ���������� �� ������� �������
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

	// ����� ��� �������� ������������ ������ �����
	// ����� �������� ������������ ���������
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
		// ��������� ��� �������� ������� � ���������� �������� �����
		struct KeyedRule {
			std::string key; // ������ �����
			Rule rule; // �������
		};

		std::list<KeyedRule> uniqueRules; // ����� ������ ��� ��������������� ������ � ���������� �������� �����

		// ������� ��� �������� ��������������� �������� ������ � ���������� ������� �����
		auto isEquivalent = [&](const Rule& rule1, const Rule& rule2, const std::string& key) {
			std::string value1 = rule1.getValue();
			std::string value2 = rule2.getValue();

			// ����� ������� ����� � ��������
			size_t keyPos1 = value1.find(key);
			size_t keyPos2 = value2.find(key);

			if (keyPos1 != std::string::npos && keyPos2 != std::string::npos) {
				// ���� ������ ����� ������������ � ����� ��������, ������� �� ��������������
				return value1 == value2;
			}
			else if (keyPos1 != std::string::npos || keyPos2 != std::string::npos) {
				// ���� ������ ����� ������������ ������ � ����� �� ������, ������� �� ����������������
				return false;
			}
			else {
				// ���� ������ ����� ����������� � ����� ��������, �� ��������� �� �� �������� ��� ��������
				return true;
			}
			};

		// ����� ������� ����� � ��������
		std::string key;

		// �������� � ���������� ��������������� ������ � ���������� �������� ����� � ����� ������
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

		// ������� �������, ���������� ������ ����� � ��������
		uniqueRules.remove_if([&](const KeyedRule& keyedRule) {
			return keyedRule.rule.getValue().find(key) != std::string::npos;
			});

		// �������� �������� ������ ������ �� ����� ������ ��� ������������� ���������
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

	//	// ���������� ������ � ����� �� ������ ������
	//	for (const auto& rule : rules) {
	//		const std::string& source = rule.getKey();
	//		const std::string& target = rule.getValue();

	//		graph[source].push_back(target);
	//	}

	//	// ���������� ����� � ���� GraphML
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

	//	// ���������� ������ �����
	//	for (const auto& node : graph) {
	//		outputFile << "    <node id=\"" << node.first << "\" />" << std::endl;
	//	}

	//	// ���������� ����� �����
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

	//	//// ���������� ������ � ����� �� ������ ������
	//	//for (const auto& rule : _rules) {
	//	//	const std::string& source = rule.getKey();
	//	//	const std::string& target = rule.getValue();

	//	//	char weight = target[0];
	//	//	std::string targetVertex = target.substr(1);

	//	//	graph[source].push_back(std::make_pair(weight, targetVertex));
	//	//}
	//	  // ���������� ������ � ����� �� ������ ������
	//	for (const auto& rule : _rules) {
	//		const std::string& source = rule.getKey();
	//		const std::string& target = rule.getValue();

	//		char weight = target[0];
	//		std::string targetVertex = target.substr(1);

	//		// ��������, ������������� �� ������ ������ �������� �����
	//		if (targetVertex != source) {
	//			// ���������� �������
	//			graph[targetVertex]; // ��������� ������ �������

	//			// ���������� �����
	//			graph[source].push_back(std::make_pair(weight, targetVertex));
	//		}
	//	}

	//	// ���������� ����� � ���� GraphML
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

	//	// ���������� ������ �����
	//	for (const auto& node : graph) {
	//		outputFile << "    <node id=\"" << node.first << "\" />" << std::endl;
	//	}

	//	// ���������� ����� �����
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
	std::map<char, int> weightMap; // �������� ���������� ����� �����

	// ���������� ������ � ����� �� ������ ������
	for (const auto& rule : _rules) {
		const std::string& source = rule.getKey();
		const std::string& target = rule.getValue();

		char weight = target[0];
		std::string targetVertex = target.substr(1);

		// ��������, ������������� �� ������ ������ �������� �����
		if (targetVertex != source) {
			// ���������� �������
			graph[targetVertex]; // ��������� ������ �������

			// ���������� �����
			if (weightMap.find(weight) == weightMap.end()) {
				// ���� ���� �� ���� �����, ���������� ����� ���������� �����
				weightMap[weight] = weightMap.size() + 1;
			}

			int numericWeight = weightMap[weight];
			graph[source].push_back(std::make_pair(numericWeight, targetVertex));
		}
	}

	// ���������� ����� � ���� GraphML
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

	// ���������� ������ �����
	for (const auto& node : graph) {
		outputFile << "    <node id=\"" << node.first << "\" />" << std::endl;
	}

	// ���������� ����� �����
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
