#include <iostream>
#include <string>
#include <vector>
#include <random>
#include <algorithm>
#include <map>
#include <sstream>
#include <cctype>
using namespace std;


class ChatBot_ {
private:
    enum State { INIT, SESSION, STORE, ER };
    // текущее состояние
    State now;
    // имя
    string UserName;
    // фразы
    vector<string> phrases;

public:
    // конструктор в состояние INIT
    ChatBot_() {
        now = INIT;
    }

    // метод для запуска
    void Run() {
        do {
            string text;
            getline(cin, text); // считывание с консоли

            // определение состояния и вызов методов обработки
            switch (now) {
            case INIT:
                ProcessInit(text);
                break;
            case SESSION:
                ProcessSession(text);
                break;
            case STORE:
                ProcessStore(text);
                break;
            default:
                break;
            }
            // пока не достигнуто состояние ошибки
        } while (now != ER);
    }

private:

    // в нижний регистр
    string ConvertToLower(const string& input) {
        string result = input;
        for (char& c : result) {
            c = std::tolower(c);
        }
        return result;
    }

    // состояние INIT
    void ProcessInit(const string& text) {
        // разделение на слова
        istringstream iss(text);
        string firstWord, name;
        int number;

        iss >> firstWord >> name;
        // если команда login, то запоминаем имя после пробела
        if (firstWord == "login") {
            UserName = name;
            now = SESSION;
        }// иначе пользователь не авторизован
        else {
            cout << "Please introduce yourself first!" << endl;
            now = INIT;
        }
    }

    // состояние Session
    void ProcessSession(string& text) {
        text = ConvertToLower(text);
        // разделение на слова
        istringstream iss(text);
        string firstWord, num;
        int number;

        iss >> firstWord >> num;

        // приветсвие
        if (text == "hello!" || text == "hello" || text == "hi!") {
            cout << "Welcome, " + UserName << "!" << endl;
            now = SESSION;
        }
        // команда запомнить
        else if (text == "memorize") {
            now = STORE;
        }
        // команда "сказать", определяется по номеру фразы в массиве
        else if (firstWord == "say") {
            number = stoi(num);
            now = SESSION;
            // если такая фраза есть
            if (number >= 0 && number < phrases.size()) {
                string element = phrases[number];
                // вывод
                cout << element << endl;
            }
            else {
                // если записи такой нет
                cout << "No record" << endl;
                now = SESSION;
            }
        }
        // выход из состояния SESSION
        else if (text == "exit") {
            cout << "Goodbye!" << endl;
            now = INIT;
        }
        // в случае если ни одна команда не подойдет
        else {
            cout << "I don't understand you!" << endl;
            now = SESSION;
        }
    }
    
    // состояние Store - выход автоматический
    void ProcessStore(const string& text) {
        phrases.push_back(text);
        now = SESSION;
    }
};


///--------------------------------------- через функцию

enum State { INIT, SESSION, STORE, ER };
// через анализатор
//enum State { INIT, SESSION, STORE, ER };
map<State, string> StateToString = { { INIT, "INIT" },
                                     { SESSION, "SESSION" },
                                     { STORE, "STORE" },
                                     { ER, "ER"}
};

string enumToString(State s)
{
    return StateToString[s];
}
// фразы
vector<string> phrases;
// имя пользователя
string UserName;
// нижний регистр
string ConvertToLower(const string& input) {
    string result = input;
    for (char& c : result) {
        c = std::tolower(c);
    }
    return result;
}

static void ChatBot()
{
    State now = INIT;
    int count = 0;
    string c;

    do {
        string text;
        getline(cin, text);

        switch (now) {
        case INIT:
        {
            istringstream iss(text);
            string firstWord, name;
            int number;

            iss >> firstWord >> name;

            if (firstWord == "login") {
                UserName = name;
                now = SESSION;
            }
            else {
                now = INIT;
                cout << "Please introduce yourself first!" << endl;
            }
            break;
        }
        case SESSION:
        {
            text = ConvertToLower(text);
            istringstream iss(text);
            string firstWord, num;
            int number;

            iss >> firstWord >> num;
            if (text == "hello!" || text == "hello" || text == "hi!") {
                now = SESSION;
                cout << "Welcome, " + UserName << "!" << endl;
            }
            else if (text == "memorize") {
                now = STORE;
            }
            else if (firstWord == "say") {
                now = SESSION;
                number = stoi(num);
                if (number >= 0 && number < phrases.size()) {
                    string element = phrases[number];
                    cout << element << endl;
                }
                else {
                    cout << "No record" << endl;
                }
            }
            else if (text == "exit") {
                cout << "Goodbye!" << endl;
                now = INIT;
            }
            else {
                now = SESSION;
                cout << "I don't understand you!" << endl;
            }
            break;
        }
        case STORE:
        {
            phrases.push_back(text);
            now = SESSION;
            break;
        }
        default:
            break;
        }
    } while (now != ER);
}

int main() {
    //ChatBot();
    ChatBot_ bot;
    bot.Run();

    return 0;
}