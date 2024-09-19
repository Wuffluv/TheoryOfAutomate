#include <iostream>
#include "../Lib/FormalLanguage.h"
#include "../Lib/Rule.h"

using namespace std;

//СОСТОЯНИЯ  0   1  2 3  4
enum State { H, N, P, S, ER };// ER состояние ошибки

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
                    now = N;
                else if (text[count] == '1')
                    now = N;
                else
                    now = ER;
                break;
            }
            case N: {
                if (text[count] == '0')
                    now = N;
                else if (text[count] == '1')
                    now = N;
                else if (text[count] == '.')
                    now = P;
                else
                    now = ER;
                break;
            }
            case P: {
                if (text[count] == '0')
                    now = S;
                else if (text[count] == '1')
                    now = S;
                else
                    now = ER;
                break;
            }
            case S: {
                if (text[count] == '1')
                    now = S;
                else if (text[count] == '0')
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
    }// while (now != S && text[count] != '\u00A7');
    while (now != ER && text[count] != '\u00A7');
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

    cout << "Задание 1." << endl;
    list<Rule> dict =
    {
        Rule("S", "S0"),
        Rule("S", "S1"),
        Rule("S", "P0"),
        Rule("S", "P1"),
        Rule("P", "N."),
        Rule("N", "0"),
        Rule("N", "1"),
        Rule("N", "N0"),
        Rule("N", "N1"),
    };


    PrintRules(dict);

    FormalLanguage fl(dict);
    // Два варианта вывода, разные результаты
    //Console.WriteLine(fl.OutputLeft());


    cout << ("Язык: L = { 0^n . 1^m  | n,m >0 }") << endl;


    cout << "Анализаторы цепочек " << endl;
    cout << "Состояния:  H-0, N-1,P-2,S-3, ER-4" << endl;
    cout << "Цепочка 11.010 \t: ";
    Analizator("11.010");
    cout << "Цепочка 0.1 \t: ";
    Analizator("0.1");
    cout << "Цепочка 01. \t: ";
    Analizator("01.");
    cout << "Цепочка 100 \t: ";
    Analizator("100");
}

