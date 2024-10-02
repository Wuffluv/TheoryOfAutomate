#include <iostream>
#include "../Lib/NFA_to_DFA.h"
#include "../Lib/FormalLanguage.h"
#include "../Lib/Rule.h"

using namespace std;

//СОСТОЯНИЯ  0  1  2   3   4
enum State { H, S, SA, AB, ER};// ER состояние ошибки

void Analizator(const string& text) {
    State now = H;
    int count = 0;
    string res = "";
    bool reachedFinalState = false;//флаг проверки последнего состояния
    do {
        // Проверка выхода за пределы индексации
        if (count < text.length()) {
            switch (now) {
            case H: {
                if (text[count] == '0')
                    now = S;
                else if (text[count] == '1')
                    now = SA;
                else
                    now = ER;
                break;
            }
            case SA: {
                if (text[count] == '0')
                    now = AB;
                else if (text[count] == '1')
                    now = AB;
                else
                    now = ER;
                break;
            }
            case AB: {
                if (text[count] == '1')
                    now = S;
                else
                    now = ER;
                break;
            }
            default:
                break;
            }

            res += to_string(now) + " ";
            count++;
        }
        else
            break;
    }
    while (now != ER && text[count] != '\u00A7'); 
    cout << res << endl;

   
}

int main()
{
    setlocale(LC_ALL, "RU");

    cout << "Задание 2.3" << endl;
    list<Rule> dict =
    {
        Rule("S", "0A"),
        Rule("S", "1B"),
        Rule("A", "0A"),
        Rule("B", "0B"),
        Rule("B", "1B")

    };

    PrintRules(dict);
    FormalLanguage fl(dict);

    
    take_input_static();
    print_output();
    create_state_transitions(start_state);
    print_dfa();



    cout << "Анализаторы цепочек " << endl;
    cout << "Состояния: H-0, S-1, SA-2, AB-3, ER-4" << endl;
    cout << "Цепочка 1011    \t:";
    Analizator("1011");
    cout << "Цепочка 0       \t:";
    Analizator("0");
    cout << "Цепочка 101011  \t:";
    Analizator("101011 ");
    cout << endl;

    cout << "G ({0,1}, {H,SA,AB,S},P,H)" << endl;

    return 0;
}
