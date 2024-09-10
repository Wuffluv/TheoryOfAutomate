#include <iostream>
#include <string>
#include <vector>
#include <random>
#include <algorithm>
#include <map>
using namespace std;

/// <summary>
/// класс правило
/// </summary>
class Rule {
public:
    string A;/// множество порождающих символов 
    string B;/// множество порождаемых символов
    bool isLooped;/// ведет ли правило к зацикливанию

    /// конструктор правила: isLooped - false по дефолту
    Rule(string a, string b, bool l = false) {
        A = a;
        B = b;
        isLooped = l;
    }
};

/// <summary>
/// печать правил
/// </summary>
/// <param name="rule"></param>
void PrintRules(const vector<Rule>& rule) {
    cout << "Правила для языка:" << endl;
    for (int i = 0; i < rule.size(); i++) {
        cout << rule[i].A << " -> " << rule[i].B << endl;
    }
}
/// <summary>
/// класс формального языка с леволинейной грамматикой и проверкой на зацикливание
/// </summary>
class FormalLanguage {
private:
    /// <summary>
    /// вектор правил языка
    /// </summary>
    vector<Rule> rules;

    /// <summary>
    /// Максимально количество повторений
    /// </summary>
    unsigned MaxRepetitionsCount;

    /// <summary>
    /// проверка правила на зацикливание
    /// </summary>
    /// <param name="input"> строка, к которой применяется правило </param>
    /// <param name="rule"> правило </param>
    /// <param name="count"> количество допустимых повторений </param>
    /// <returns></returns>
    bool checkLoop(string input, Rule rule, unsigned count = 10) {
        for (int i = 0; i < count; i++)
        {
            string A = rule.A;
            string B = rule.B;
            size_t pos = input.find(A);
            if (pos != string::npos) {
                input.replace(pos, A.length(), B);
            }
            else return false;
        }
        return true;
    }

    /// <summary>
    /// обновление состояния isLooped всех правил
    /// </summary>
    void RefreshRules() {
        for (Rule& rule : rules) {
            rule.isLooped = false;
        }
    }

public:

    /// <summary>
    /// конструктор
    /// </summary>
    /// <param name="rules_"> вектор правил </param>
    /// <param name="count"> максимальное количество повторений - по дефолту 10000</param>
    FormalLanguage(vector<Rule> rules_, unsigned count = 100) {
        rules = rules_;
        MaxRepetitionsCount = count;
    }

    /// <summary>
    /// функция перевода строки на формальный язык
    /// </summary>
    /// <param name="text"> входная строка </param>
    /// <returns> выходная цепочка </returns>
    string Translate(string text) {
        int count = 0; /// счетчик максимального количества повторений
        bool isEnd = false; /// ни одно из правил неприменимо
        /// повторяем, пока не достигнем максимального количества повторений
        while (count < MaxRepetitionsCount) {
            if (isEnd) break; /// выход, если правила не применимы
            count++; /// инкремент счетчика
            isEnd = true; /// предполагаем, что это последняя итерация
            for (Rule& rule : rules) { /// проходимся по всем правилам отдельно
                if (!rule.isLooped) { /// приводит ли правило к зацикливанию
                    string A = rule.A; /// получаем символы из правила
                    string B = rule.B;

                    size_t pos = text.find(A); /// ищем из А
                    //cout << text << endl;
                    if (pos != string::npos) { /// если порождающий найден
                        if (checkLoop(text, rule)) { /// проверяем на зацикливание
                            rule.isLooped = true;
                        }
                        else { /// если зацикливания нет
                            text.replace(pos, A.length(), B); /// заменяем символы
                            isEnd = false; /// для следующей итерации обновляем
                            break;/// выход
                        }
                    }
                }
                else { /// правило приводит к зацикливанию
                    rule.isLooped = true;
                }
            }
        }
        RefreshRules(); /// обновление isLooped для правил перед выходом
        return text; /// возврат выходной цепочки
    }

    string OutputLeft(vector<string>& transformations)
    {
        transformations.clear();
        string result = "S";
        int count = 0;

        while (count < MaxRepetitionsCount)
        {
            size_t pos = string::npos;

            // найдем крайний левый нетерминальный символ в цепочке
            for (const Rule& rule : rules)
            {
                string key = rule.A;
                size_t findPos = result.find(key);
                if ((pos > findPos || pos == string::npos) && findPos != string::npos)
                {
                    pos = findPos;
                    break;
                }
            }
            // если не найдено ниодного подходящего правила - выходим
            if (pos == string::npos)
            {
                break;
            }
            // для правого
            // найдем все правила подходящие для крайнего левого нетерминального символа
            std::vector<Rule> rules_;
            for (const Rule& rule : rules) {
                std::string key = rule.A;
                if (pos == result.find(key)) {
                    rules_.push_back(rule);
                }
            }

            std::random_device rd;
            std::mt19937 gen(rd());
            std::uniform_int_distribution<> dis(0, rules_.size() - 1);
            int index = dis(gen);
            Rule r = rules_[index];

            size_t p = result.find(r.A);
            result.replace(p, r.A.length(), r.B);
            //cout << result << endl;
            transformations.push_back("( " + r.A + " --> " + r.B + " ): " + result);
            count++;
        }
        //cout << result << endl;
        return result;
    }

    string OutputLeft_Equivalence()
    {
        string result = "S";
        int count = 0;

        while (count < MaxRepetitionsCount)
        {
            size_t pos = string::npos;

            // найдем крайний левый нетерминальный символ в цепочке
            for (const Rule& rule : rules)
            {
                string key = rule.A;
                size_t findPos = result.find(key);
                if ((pos > findPos || pos == string::npos) && findPos != string::npos)
                {
                    pos = findPos;
                    break;
                }
            }
            // если не найдено ниодного подходящего правила - выходим
            if (pos == string::npos)
            {
                break;
            }
            // для правого
            // найдем все правила подходящие для крайнего левого нетерминального символа
            std::vector<Rule> rules_;
            for (const Rule& rule : rules) {
                std::string key = rule.A;
                if (pos == result.find(key)) {
                    rules_.push_back(rule);
                }
            }

            std::random_device rd;
            std::mt19937 gen(rd());
            std::uniform_int_distribution<> dis(0, rules_.size() - 1);
            int index = dis(gen);
            Rule r = rules_[index];

            size_t p = result.find(r.A);
            result.replace(p, r.A.length(), r.B);

            count++;
        }
        //cout << result << endl;
        return result;
    }


    string Transformations(string chain_) {
        string buf;
        string result;
        vector<string> transformations;
        bool found = false;
        int counter = 0;
        while (!found) {
            buf = this->OutputLeft(transformations);
            counter++;
            if (counter == 10000000) return "Цепочка не построена. Попробуйте ещё раз\n";
            if (buf == chain_) found = true;
        }

        for (int i = 0; i < transformations.size(); i++)
        {
            result += transformations[i] + "\n";
        }
        result.insert(0, "Начальный символ: S\n");

        return result;
    }
};

void FillVectorWithUniqueChars(string inputString, vector<char>& vector)
{
    for (char c : inputString) {
        if (std::find(vector.begin(), vector.end(), c) == vector.end()) {
            vector.push_back(c);
        }
    }
}

string equivalence(FormalLanguage fl1, FormalLanguage fl2) {
    vector<char> alphabet1, alphabet2;
    string buf1, buf2;
    vector<string> transformations;
    for (int i = 0; i < 10000; i++)
    {
        buf1 = fl1.OutputLeft_Equivalence();
        buf2 = fl2.OutputLeft_Equivalence();
        FillVectorWithUniqueChars(buf1, alphabet1);
        FillVectorWithUniqueChars(buf2, alphabet2);
    }
    
    sort(alphabet1.begin(), alphabet1.end());
    sort(alphabet2.begin(), alphabet2.end());
    cout << endl << "Символы в первом языке: ";
    for (int i = 0; i < alphabet1.size(); i++)
    {
        cout  << alphabet1[i] << " ";
    }
    cout << endl << "Символы во втором языке: ";
    for (int i = 0; i < alphabet2.size(); i++)
    {
        cout  << alphabet2[i] << " ";
    }
    cout << endl;
    bool isEqual = equal(alphabet1.begin(), alphabet1.end(), alphabet2.begin());
    if (isEqual) {
        return "Грамматики эквивалентны";
    }
    else {
        return "Грамматики не эквивалентны";
    }
}


class Grammar {
public:
    vector<string> Nonterminal;
    vector<string> Terminal;
    vector<Rule> P;
    string S;

    Grammar(vector<string> vn, vector<string> vt, vector<Rule> rules, string s = "S") : Nonterminal(vn), Terminal(vt), P(rules), S(s) {}

    string GetTypeGrammar() {
        bool isTypeOne = true;
        bool isTypeTwo = true;
        bool isTypeThree = true;

        bool isEachTermPosBigger = true;
        bool isEachTermPosSmaller = true;

        for (const Rule& r : P) {
            isTypeOne &= r.A.length() <= r.B.length();

            for (const string& vt : Terminal) {
                isTypeTwo &= r.A.find(vt) == string::npos;
            }

            if (isEachTermPosBigger || isEachTermPosSmaller) {
                vector<int> terminalPositions;
                vector<int> nonTerminalPositions;
                for (const string& vn : Nonterminal) {
                    int temp = r.B.find(vn);
                    if (temp != -1) {
                        nonTerminalPositions.push_back(temp);
                    }
                }
                for (const string& vt : Terminal) {
                    int temp = r.B.find(vt);
                    if (temp != -1) {
                        terminalPositions.push_back(temp);
                    }
                }
                for (int pos : terminalPositions) {
                    for (int posNonTerm : nonTerminalPositions) {
                        isEachTermPosBigger &= pos > posNonTerm;
                        isEachTermPosSmaller &= pos < posNonTerm;
                    }
                }
                if (!isEachTermPosBigger && !isEachTermPosSmaller) {
                    isTypeThree = false;
                }
            }
        }

        string res = "0";
        if (isTypeOne) res += " 1";
        if (isTypeTwo) res += " 2";
        if (isTypeThree) res += " 3";
        return res;
    }

    string MakeTree(string text) {
        int maxCount = 10000;
        int count = 0;
        vector<string> tree;
        tree.push_back(text);

        while (count < maxCount) {
            for (const Rule& rule : P) {
                string key = rule.A;
                string value = rule.B;

                int pos = text.find(value);
                if (pos != -1) {
                    text.replace(pos, value.length(), key);

                    string separator = "|";
                    for (int i = 0; i < pos; i++) {
                        separator = " " + separator;
                    }
                    tree.push_back(separator);
                    tree.push_back(text);
                }
            }
            count++;
        }

        for (auto it = tree.rbegin(); it != tree.rend(); ++it) {
            cout << *it << endl;
        }
        return text;
    }

};



//enum State { H, N, P, S, ER };
//map<State, string> StateToString = { { H, "H" },
//                                     { N, "N" },
//                                     { P, "P" },
//                                     { S, "S" },
//                                     { ER, "ER" },
//};
//
//// Function to convert a Color enum value to its string 
//// representation 
//string enumToString(State s)
//{
//    return StateToString[s];
//}
//
//static void Analizator(string text)
//{
//    State now = H;
//    int count = 0;
//    string res = "";
//    do
//    {
//        //проверка выхода за пределы индексации
//        if (count < text.length())
//        {
//            switch (now)
//            {
//            case H:
//            {
//                if (text[count] == '0') now = N;
//                else if (text[count] == '1') now = N;
//                else now = ER;
//                break;
//            }
//            case N:
//            {
//                if (text[count] == '0') now = N;
//                else if (text[count] == '1') now = N;
//                else if (text[count] == '.') now = P;
//                else now = ER;
//                break;
//            }
//            case P:
//            {
//                if (text[count] == '0') now = S;
//                else if (text[count] == '1') now = S;
//                else now = ER;
//                break;
//            }
//            case S:
//            {
//                if (text[count] == '1') now = S;
//                else if (text[count] == '0') now = S;
//                else now = ER;
//                break;
//            }
//            default:
//                break;
//            }
//
//            res += enumToString(now);
//            res += "";
//            count++;
//        }
//        else break;
//    } while (now != ER);
//    cout << "Цепочка состояний: " << res;
//
//    if (enumToString(now) == "S") cout << endl << "Цепочка " << text << " принадлежит данному языку" << endl << endl;
//
//    else cout << endl << "Цепочка " << text << " не принадлежит данному языку" << endl << endl;
//}
//
//enum State_2 { H_, A_, B_, S_, ER_ };
//
//map<State_2, string> StateToString2 = { { H_, "H" },
//                                     { A_, "A" },
//                                     { B_, "B" },
//                                     { S_, "S" },
//                                     { ER_, "ER" },
//};
//
//string enumToString_2(State_2 s)
//{
//    return StateToString2[s];
//}
//
//static void Analizator_2(string text)
//{
//    State_2 now = H_;
//    int count = 0;
//    string res = "";
//    do
//    {
//        //проверка выхода за пределы индексации
//        if (count < text.length())
//        {
//            switch (now)
//            {
//            case H_:
//            {
//                if (text[count] == '0') now = A_;
//                else if (text[count] == '1') now = A_;
//                else now = ER_;
//                break;
//            }
//            case A_:
//            {
//                if (text[count] == '0') now = A_;
//                else if (text[count] == '1') now = A_;
//                else if (text[count] == '+') now = B_;
//                else if (text[count] == '-') now = B_;
//                else if (text[count] == '|') now = S_;
//                else now = ER_;
//                break;
//            }
//            case B_:
//            {
//                if (text[count] == '0') now = A_;
//                else if (text[count] == '1') now = A_;
//                else now = ER_;
//                break;
//            }
//            case S:
//            {
//                now = ER_;
//                break;
//            }
//            default:
//                break;
//            }
//
//            res += enumToString_2(now);
//            res += "";
//            count++;
//        }
//        else break;
//    } while (now != ER_); 
//    cout << "Цепочка состояний: " << res;
//    if (enumToString_2(now) == "S") cout << endl << "Цепочка " << text << " принадлежит данному языку" << endl << endl;
//    else cout << endl << "Цепочка " << text << " не принадлежит данному языку" << endl << endl;
//}

enum State { H, N, S, ER };
map<State, string> StateToString = { { H, "H" },
                                     { N, "N" },  
                                     { S, "S" },
                                     { ER, "ER" }
};

// Function to convert a Color enum value to its string 
// representation 
string enumToString(State s)
{
    return StateToString[s];
}

//static void Analizator(string text)
//{
//    int b, c;
//    State now = H;
//    int count = 0;
//    string res = "";
//    do
//    {
//        //проверка выхода за пределы индексации
//        if (count < text.length())
//        {
//            switch (now)
//            {
//            case H:
//            {
//                c = text[count];
//                if (text[count] == '0') {
//                    now = N;
//                    b = c - '0';
//                    c = text[count + 1];
//                }
//                else if (text[count] == '1') {
//                    now = N;
//                    b = c - '0';
//                    c = text[count + 1];
//                }
//                else if (text[count] == '|') now = S;
//                else {
//                    now = H;
//                    c = text[count + 1];
//                }
//                break;
//            }
//            case N:
//            {
//                if (text[count] == '0') {
//                    now = N;
//                    b = 2 * b + c - '0';
//                    c = text[count + 1];
//                }
//                else if (text[count] == '1') {
//                    now = N;
//
//                    b = 2 * b + c - '0';
//                    c = text[count + 1];
//                }
//                else { 
//                    now = H; 
//                    c = text[count + 1];
//                    cout << b;
//                }
//                break;
//            }
//            case S:
//            {
//                break;
//            }
//            default:
//                break;
//            }
//
//            res += enumToString(now);
//            res += "";
//            count++;
//        }
//        else break;
//    } while (now != ER);
//    cout << endl << "Цепочка состояний: " << res;
//
//    //if (enumToString(now) == "S") cout << endl << "Цепочка " << text << " принадлежит данному языку" << endl << endl;
//
//    //else cout << endl << "Цепочка " << text << " не принадлежит данному языку" << endl << endl;
//}

static void Analizator(string text)
{
    int b, c;
    State now = H;
    int count = 0;
    string res = "";
    do
    {
        //проверка выхода за пределы индексации
        if (count < text.length())
        {
            switch (now)
            {
            case H:
            {
                c = text[count];
                if (text[count] == '0') {
                    now = N;
                    b = c - '0';
                    c = text[count + 1];
                }
                else if (text[count] == '1') {
                    now = N;
                    b = c - '0';
                    c = text[count + 1];
                }
                else if (text[count] == '|') now = S;
                else {
                    now = H;
                    c = text[count + 1];
                }
                break;
            }
            case N:
            {
                if (text[count] == '0') {
                    now = N;
                    b = 2 * b + c - '0';
                    c = text[count + 1];
                }
                else if (text[count] == '1') {
                    now = N;

                    b = 2 * b + c - '0';
                    c = text[count + 1];
                }
                else {
                    now = H;
                    c = text[count + 1];
                    cout << b;
                }
                break;
            }
            case S:
            {
                break;
            }
            default:
                break;
            }

            res += enumToString(now);
            res += "";
            count++;
        }
        else break;
    } while (now != ER);
    cout << endl << "Цепочка состояний: " << res;

    //if (enumToString(now) == "S") cout << endl << "Цепочка " << text << " принадлежит данному языку" << endl << endl;

    //else cout << endl << "Цепочка " << text << " не принадлежит данному языку" << endl << endl;
}

int main()
{
    setlocale(LC_ALL, "ru");
    vector<Rule> dict;
    string way;
    // задания из лабораторной 1
    {
        cout << "Задание 1.a" << endl;
    dict = {
        Rule("S", "T"),
        Rule("S", "T+S"),
        Rule("S", "T-S"),
        Rule("T", "F"),
        Rule("T", "F*T"),
        Rule("F", "a"),
        Rule("F", "b"),
    };
    PrintRules(dict);
    FormalLanguage fl1(dict);
    way = fl1.Transformations("a-b*a+b");
    cout << way;

    cout << "Задание 1.б" << endl;
    vector<Rule> dict2 = {
        Rule("S", "aSBC"),
        Rule("S", "abC"),
        Rule("CB", "BC"),
        Rule("bB", "bb"),
        Rule("bC", "bc"),
        Rule("cC", "cc"),
    };
    PrintRules(dict2);
    FormalLanguage fl2(dict2);
    way = fl2.Transformations("aaabbbccc");
    cout << way;

    cout << endl << "Задание 2.a" << endl;
    dict = {
        Rule("S", "aaCFD"),
        Rule("AD", "D"),
        Rule("F", "AFB"),
        Rule("F", "AB"),
        Rule("Cb", "bC"),
        Rule("AB", "bBA"),
        Rule("CB", "C"),
        Rule("Ab", "bA"),
        Rule("bCD", "Eps"),
    };
    PrintRules(dict);
    FormalLanguage fl3(dict);
    cout << "Цепочка: " << fl3.Translate("S") << endl;
    cout << "Язык: L = { a^n | n > 0 }" << endl;


    dict = {
        Rule("S", "A_|_"),
        Rule("S", "B_|_"),
        Rule("A", "a"),
        Rule("A", "Ba"),
        Rule("B", "b"),
        Rule("B", "Bb"),
        Rule("B", "Ab"),

    };
    PrintRules(dict);
    FormalLanguage fl4(dict);
    cout << "Цепочка: " << fl4.Translate("S") << endl;
    cout << "Язык: L = { a^n b^m _|_ | n >= 0, m >= 0 }" << endl;


    cout << endl << "Задание 3.a" << endl;
    cout << "Язык: L = { a^n b^m c^k | n, m, k > 0}" << endl;
    cout << "Грамматика: G: ({a, b, c}, {A, B, C}, P, S)" << endl;
    dict = {
        Rule("S", "aAB"),
        Rule("A", "aa"),
        Rule("B", "BbC"),
        Rule("B", "bbb"),
        Rule("C", "Cc"),
        Rule("C", "c"),
    };
    PrintRules(dict);
    FormalLanguage fl5(dict);
    cout << "Цепочка: " + fl5.Translate("S");


    cout << endl << "Задание 3.б" << endl;
    cout << "Язык: L = {0^n(10)^m | n, m >= 0}" << endl;
    cout << "Грамматика: G: ({0, 10}, {A, B}, P, S)" << endl;
    dict = {
        Rule("S", "0A"),
        Rule("A", "0B"),
        Rule("A", "0000B"),
        Rule("B", "1010B"),
        Rule("B", "1010")
    };
    PrintRules(dict);
    FormalLanguage fl6(dict);
    cout << "Цепочка: " + fl6.Translate("S");


    cout << endl << "Задание 3.в" << endl;
    cout << "Язык: L = {a1 a2 … ai ai … a2a1 | ai E {0, 1}}" << endl;
    cout << "Грамматика: G: ({0, 1}, {A, B}, P, S)" << endl;
    dict = {
        Rule("S", "A"),
        Rule("A", "10AB"),
        Rule("A", "10B"),
        Rule("B", "01"),
    };
    PrintRules(dict);
    FormalLanguage fl7(dict);
    cout << "Цепочка: " + fl7.Translate("S");

    cout << endl << "Задание 5" << endl;
    dict = {
        Rule("S", "aSL"),
        Rule("S", "aL"),
        Rule("L", "Kc"),
        Rule("cK", "Kc"),
        Rule("K", "b"),
    };
    PrintRules(dict);
    FormalLanguage fl8(dict, 1000000);
    cout << "Цепочка: " + fl8.Translate("S") << endl;

    dict = {
        Rule("S", "aSBc"),
        Rule("S", "abc"),
        Rule("cB", "Bc"),
        Rule("bB", "bb"),
    };
    PrintRules(dict);
    FormalLanguage fl9(dict, 1000000);
    cout << "Цепочка: " + fl9.Translate("S") << endl;
    //cout << equivalence(fl8, fl9) << endl;
    cout << "Грамматики эквиваленты т.к. они обе порождают язык L = { a^n b^m c^k | n, m, k > 0 }" << endl << endl;

    cout << endl << "Задание 6" << endl;
    dict = {
        Rule("S", "AB"),
        Rule("S", "ABS"),
        Rule("AB", "BA"),
        Rule("BA", "AB"),
        Rule("A", "a"),
        Rule("B", "b"),
    };
    PrintRules(dict);
    FormalLanguage fl10(dict);
    cout << "Цепочка: " + fl10.Translate("S") << endl;
    way = fl10.Transformations("ab");
    cout << way;

    dict = {
        Rule("S", "a"),
        Rule("S", "b")
    };
    PrintRules(dict);
    FormalLanguage fl11(dict);
    cout << "Цепочка: " + fl11.Translate("S") << endl << endl;

    /// Задание 7
    cout << "Задание 7" << endl << endl;
    dict = {
        Rule("S", "A.A"),
        Rule("A", "B"),
        Rule("A", "BA"),
        Rule("B", "0"),
        Rule("B", "1"),
    };
    PrintRules(dict);

    FormalLanguage fl12(dict);
    cout << "Цепочка: " + fl12.Translate("S") << endl;

    dict = {
        Rule("S", "0.A"),
        Rule("A", "0A"),
        Rule("A", "1"),
    };
    PrintRules(dict);
    FormalLanguage fl13(dict);
    cout << "Цепочка: " + fl13.Translate("S") << endl;
    cout << "Грамматика: G: ({A, B}, {0, 1, .}, P, S)";

    //cout << equivalence(fl12, fl13) << endl;
    cout << "Эти две грамматики эквивалентны, т.к. описывают один язык.";

    cout << "Задание 9" << endl;
    dict = {
        Rule("S", "aSbS"),
        Rule("S", "bSaS"),
        Rule("S", "E"),
    };
    PrintRules(dict);

    FormalLanguage fl14(dict);
    cout << "Цепочка: " + fl14.Translate("S") << endl;
    Grammar gr = {
        vector<string>{ "S" },
        vector<string>{ "a", "b", "E" },
        dict
    };

    cout << gr.MakeTree("aEbaEbE") << endl<< endl;
    cout << gr.MakeTree("abEaEbE") << endl << endl;

    cout << endl << endl<< "Задание 11 (а)" << endl;
    dict = {
        Rule("S", "0S"),
        Rule("S", "0B"),
        Rule("B", "1B"),
        Rule("B", "1C"),
        Rule("C", "1C"),
        Rule("C", "_|_")
    };
    PrintRules(dict);
    cout << "Язык: L = { 0^n 1^m _|_ | n, m > 0}" << endl;

    dict = {
        Rule("S", "A_|_"),
        Rule("A", "A1"),
        Rule("A", "B1"),
        Rule("B", "C1"),
        Rule("B", "CB1"),
        Rule("C", "0")
    };
    PrintRules(dict);
    cout << "Язык: L = { 0^n 1^m _|_ | n, m > 0}" << endl;

    cout << endl << "Задание 11 (б)" << endl;
    dict = {
        Rule("S", "aA"),
        Rule("S", "aB"),
        Rule("S", "bA"),
        Rule("A", "bS"),
        Rule("B", "aS"),
        Rule("B", "bB"),
        Rule("B", "_|_")
    };
    PrintRules(dict);
    cout << "Язык: L = { a^n b^m _|_ | n, m > 0}" << endl;

    dict = {
        Rule("S", "A_|_"),
        Rule("A", "ABb"),
        Rule("A", "Bb"),
        Rule("A", "Aa"),
        Rule("B", "a"),
        Rule("B", "b")
    };
    PrintRules(dict);
    cout << "Язык: L = { a^n b^m _|_ | n, m > 0}" << endl;


    cout << endl << "Задание 12" << endl;
    dict = {
        Rule("S", "S1"),
        Rule("S", "A0"),
        Rule("A", "A1"),
        Rule("A", "0"),
    };
    PrintRules(dict);
    cout << "Язык: L = { 0 (1)^n 0 (1)^m  | n, m >= 0}" << endl;

    dict = {
        Rule("S", "A1"),
        Rule("S", "B0"),
        Rule("S", "E1"),
        Rule("A", "S1"),
        Rule("B", "C1"),
        Rule("B", "D1"),
        Rule("C", "0"),
        Rule("D", "B1"),
        Rule("E", "E0"),
        Rule("E", "1"),
    };
    PrintRules(dict);
    cout << "Язык: L = { 0^n 1^m | n, m >= 0}" << endl;

    dict = {
        Rule("S", "S1"),
        Rule("S", "A0"),
        Rule("A", "A1"),
        Rule("A", "0"),
    };
    PrintRules(dict);
    cout << "Язык: L = { 0 (1)^n 0 (1)^m  | n, m >= 0 }" << endl;
    }
    
    cout << "Задание 1" << endl;

    Analizator("11.010");
    Analizator("0.1");
    Analizator("01.");
    Analizator("100");
    cout << "Данная грамматика порождает язык L = { 0^n 1^m . | n, m >= 0 }" << endl << endl;

    cout << "Задание 2" << endl;
    Analizator("1011|");
    Analizator("10+011|");
    Analizator("1-101+1|3");
    cout << "Грамматика: G: ({ 0, 1, +, - , | }, { A, B, S }, P, S)" << endl;

    dict = {
        Rule("S", "A|"),
        Rule("A", "A0"),
        Rule("A", "A1"),
        Rule("A", "0"),
        Rule("A", "1"),
        Rule("A", "B0"),
        Rule("A", "B1"),
        Rule("B", "A+"),  
        Rule("B", "A-"),
    };
    PrintRules(dict);
    cout << "Данная грамматика порождает язык L = { 0^n, 1^m, +^k, -^t , | | n, m > 0, k, t >= 0 }" << endl << endl;

    Analizator("1+101//p11+++1000/5|");
}