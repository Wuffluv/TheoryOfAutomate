#include <iostream>
#include "../Lib/FormalLanguage.h"
#include "../Lib/Rule.h"

using namespace std;

//СОСТОЯНИЯ  0   1  2 3  4
enum State { H, A, B, S, ER };// ER состояние ошибки

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
                    now = A;
                else if (text[count] == '1')
                    now = A;
                else
                    now = ER;
                break;
            }
            case A: {
                if (text[count] == '0')
                    now = A;
                else if (text[count] == '1')
                    now = A;
                else if (text[count] == '+')
                    now = B;
                else if (text[count] == '-')
                    now = B;
                else if (text[count] == '!')
                    now = S;
                else
                    now = ER;
                break;
            }
            case B: {
                if (text[count] == '0')
                    now = A;
                else if (text[count] == '1')
                    now = A;

                else
                    now = ER;
                break;
            }
            case S: {
                /*if (text[count] == '1')
                    now = S;
                else if (text[count] == '0')
                    now = S;
                else
                    now = ER;*/
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
    }// while (now != S && text[count] != '\u00A7');
    while (now != ER && text[count] != '\u00A7'); //now != State.S &&
    cout << res << endl;

    //проверяем было ли достугнуто конечное состояние 
    reachedFinalState = (now == S && count == text.length());
    if (reachedFinalState)
        cout << "Цепочка принадлежит данному языку." << std::endl;
    else
        cout << "Цепочка не принадлежит данному языку." << std::endl;
}




int main()
{


    setlocale(LC_ALL, "rus");

    cout << "Задание 2." << endl;
    list<Rule> dict =
    {
        Rule("S", "A0"),
        Rule("S", "A1"),
        Rule("A", "A0"),
        Rule("A", "A1"),
        Rule("A", "B+"),
        Rule("A", "B-"),
        Rule("A", "S!"),
        Rule("B", "A0"),
        Rule("B", "A1"),


    };


    PrintRules(dict);

    FormalLanguage fl(dict);






    cout << "Анализаторы цепочек " << endl;
    cout << "Состояния: H-0, A-1, B-2, S-3, ER-4" << endl;
    cout << "Цепочка 1011!    \t:";
    Analizator("1011!");
    cout << "Цепочка 10+011!    \t:";
    Analizator("10+011!");
    cout << "Цепочка 0-101+1!   \t:";
    Analizator("0-101+1!");


    cout << "G ({0,1,-,+,!}, {A,B,S},P,S)" << endl;


}
